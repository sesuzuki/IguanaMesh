using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.IUtils;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace Iguana.IguanaMesh.ITypes
{
    /// <summary>
    /// Abstract class for the explicit construction of a d-dimensional entitiy in an Array-based Data Structure. 
    /// </summary>
    ///
    public abstract class IElement : IGH_Goo, ICloneable, IGH_PreviewData
    {
        public int TopologicDimension { get; set; }
        public int Key { get; set; } = -1;

        /// Sibling half-facets are packed within a 64-bits integer. 
        /// The default value is 0 representing naked half-facets when 
        /// the topology is build or not intiliazed values when the topology wasn´t built. 
        internal long[][] _siblingHalfFacets;
        internal bool[][] _visits;
        private Brep brep;
        private int _elementType;

        public int ElementType { get => _elementType; }
        public int[] Vertices { get; set; }

        /// <summary>
        /// Copy without topologic data.
        /// </summary>
        /// <returns></returns>
        public abstract IElement CleanCopy();

        /// <summary>
        /// <para> Customizable constructor. </para>
        /// <para><paramref name="verticesKeys"/> : A collection of vertex identifiers. </para>
        /// <paramref name="dimension"/> : The topologic dimension of the element. The dimension should be larger than 0 and smaller or equal to 3.  
        /// </summary>
        public IElement(int[] vKeys, int halfFacetCount, int topologicDimension, int elementType)
        {
            Init(vKeys, halfFacetCount, topologicDimension, elementType);
        }

        /// <summary>
        /// Initializes element data
        /// </summary>
        protected void Init(int[] vKeys, int halfFacetCount, int topologicDimension, int elementType)
        {
            Vertices = vKeys;
            TopologicDimension = topologicDimension;
            _siblingHalfFacets = new long[halfFacetCount][];
            _visits = new bool[halfFacetCount][];
            _elementType = elementType;
        }

        /// <summary>
        /// Return the index of the half-facet containing the given vertices keys. Return -1 if the given combination of vertices keys is not found in a half-facet of the element. 
        /// </summary>
        public int GetFirstLevelHalfFacetIndexContainingVertices(int[] vertexKeys)
        {
            int[] hf;
            for(int i=1; i<=HalfFacetsCount; i++)
            {
                GetFirstLevelHalfFacet(i, out hf);
                hf.ToList();
                if (vertexKeys.All(vK => hf.Contains(vK))) return i;
            }
            return 0;
        }

        /// <summary>
        /// Get or set the collection of vertex identifiers. 
        /// </summary>
        ///
        public int VerticesCount
        {
            get => Vertices.Length;
        }

        public abstract void CleanTopologicalData();

        public int HalfFacetsCount
        {
            get => _siblingHalfFacets.Length;
        }

        public abstract int[] GetGmshFormattedVertices();

        public int GetSiblingElementID(int parentIndex, int childIndex=1)
        {
            return IHelpers.UnpackFirst32BitsOnTripleKey(_siblingHalfFacets[parentIndex - 1][childIndex-1]);
        }

        public int GetParentSiblingHalfFacetID(int parentIndex, int childIndex = 1)
        {
            return IHelpers.UnpackSecond16BitsOnTripleKey(_siblingHalfFacets[parentIndex - 1][childIndex-1]);
        }

        public int GetChildSiblingHalfFacetID(int parentIndex, int childIndex = 1)
        {
            return IHelpers.UnpackThird16BitsOnTripleKey(_siblingHalfFacets[parentIndex - 1][childIndex - 1]);
        }

        public void RegisterHalfFacetVisit(int parentIndex, int childIndex = 1)
        {
            _visits[parentIndex - 1][childIndex - 1] = true;         
        }

        internal void SetElementType(int elementType) 
        { 
            _elementType = elementType; 
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public void ClearSiblingHalfFacetVisit(int parentIndex, int childIndex = 1)
        {
            _visits[parentIndex - 1][childIndex - 1] = false;
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public Boolean IsHalfFacetVisited(int parentIndex, int childIndex=1)
        {
            return _visits[parentIndex - 1][childIndex - 1];
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public Boolean IsNakedSiblingHalfFacet(int parentIndex, int childIndex=1)
        {
            if (_siblingHalfFacets[parentIndex- 1][childIndex-1] == 0) return true;
            else return false;
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public void SetSiblingHalfFacet(int elementID, int halfFacetID, int parentIndex, int childIndex=1)
        {
            _siblingHalfFacets[parentIndex - 1][childIndex -1] = IHelpers.PackKeyPair(elementID,halfFacetID);
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public void SetSiblingHalfFacet(long sibData, int parentIndex, int childIndex = 1)
        {
            _siblingHalfFacets[parentIndex - 1][childIndex-1] = sibData;
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public long GetSiblingHalfFacet(int parentIndex, int childIndex = 1)
        {
            return _siblingHalfFacets[parentIndex - 1][childIndex - 1];
        }

        public long[] GetFlattenSiblingHalfFacets()
        {
            return IHelpers.FlattenLongArray(_siblingHalfFacets);
        }

        public int[] GetListOfUniqueSiblingElements()
        {
            HashSet<int> set = new HashSet<int>();
            int key;
            for(int i=0; i<_siblingHalfFacets.Length; i++)
            {
                for(int j=0;j<_siblingHalfFacets[i].Length; j++)
                {
                    key = IHelpers.UnpackFirst32BitsOnTripleKey(_siblingHalfFacets[i][j]);
                    set.Add(key);
                }
            }
            return set.ToArray();
        }

        public long[][] SiblingHalfFacets
        {
            get => _siblingHalfFacets;
        }

        /// <summary>
        /// Removes all vertex identifiers. 
        /// </summary>
        public void Clear()
        {
            Vertices = new int[0];
            _siblingHalfFacets = new long[0][];
            TopologicDimension = 0;
            _visits = new bool[0][];
        }

        /// <summary>
        /// Reverse the direction of the element.
        /// </summary>
        public void Flip()
        {
            Vertices.Reverse();
        }

        public Boolean Visited
        {
            get
            {
                for(int i = 0; i < _visits.Length; i++)
                {
                    for(int j=0; j< _visits[i].Length; j++)
                    {
                        if (_visits[i][j] == false) return false;
                    }
                }
                return true;
            }
            set
            {
                for (int i=0; i<_visits.Length; i++)
                {
                    for (int j = 0; j < _visits[i].Length; j++)
                    {
                        _visits[i][j] = value;
                    }
                };
            }
        }

        /// <summary>
        /// Abstract method to return halfacets with only principal nodes.
        /// With this method mid-nodes of high-order elements will not be returned.
        /// The method is used for displaying rhino objects.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="halfFacets"></param>
        /// <returns></returns>
        public abstract bool GetHalfFacetWithPrincipalNodesOnly(int index, out int[] halfFacets);

        /// <summary>
        /// <para> Abstract method to return a Half-Facet. A Half-Facet is a (d-1)-dimensional sub-entity of a d-dimensional entity. </para>
        /// <paramref name="index"/> : The local index of the Half-Facet to search within the element.
        /// <paramref name="halfFacets"/> : A sub-list to store all the vertex identifiers representing the Half-Facet.
        /// </summary>
        ///
        public abstract bool GetFirstLevelHalfFacet(int index, out int[] halfFacets);
        
        public virtual bool GetSecondLevelHalfFacet(int parentIndex, int childIndex, out int[] halfFacet)
        {
            int[] hf_parent;
            GetFirstLevelHalfFacet(parentIndex, out hf_parent);

            halfFacet = new int[0]; ;
            if (childIndex > 0 && childIndex <= hf_parent.Length)
            {
                if (childIndex < hf_parent.Length)
                {
                    halfFacet = new int[] { hf_parent[childIndex - 1], hf_parent[childIndex] };
                }
                else
                {
                    halfFacet = new int[] { hf_parent[childIndex - 1], hf_parent[0] };
                }
                return true;
            }
            else return false;
        }

        public int GetHalFacetContainingVertices(int[] vKey)
        {
            for(int i=1; i<=HalfFacetsCount; i++)
            {
                int[] hf;
                GetFirstLevelHalfFacet(i, out hf);
                var eval = vKey.Intersect(hf);
                if (eval.Count() == vKey.Length) return i;
            }
            return 0;
        }

        /// <summary>
        /// <para> Element´s description . </para>
        /// </summary>
        ///
        public override string ToString()
        {
            string msg = "IElement{";
            for(int i=0; i< VerticesCount; i++)
            {
                int idx = Vertices[i];
                if (i < VerticesCount - 1) msg += idx + ";";
                else msg += idx + "}";
            }

            return msg;
        }

        public string SiblingHalfFacetDataToString()
        {
            string msg = "IElement (Key: " + Key + " :: Topologic Dimension: " + TopologicDimension + " :: Number of Vertices: " + VerticesCount + " :: Number of Half-Facets: " + HalfFacetsCount + ")\n";
            for (int i = 1; i <= HalfFacetsCount; i++)
            {
                if (GetSiblingHalfFacet(i) != 0)
                {
                    if (TopologicDimension == 2) msg += "Face Element ID: " + GetSiblingElementID(i) + " :: Half-Edge ID: " + GetParentSiblingHalfFacetID(i) + "\n";
                    if (TopologicDimension == 3) msg += "Solid Element ID: " + GetSiblingElementID(i) + " :: Half-Face ID: " + GetParentSiblingHalfFacetID(i) + "\n";
                }
                else
                {
                    msg += "Naked Half-Edge" + "\n";
                }
            }
            return msg;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #region IGH_Preview methods
        public void BuildRhinoGeometry(IMesh mesh)
        {
            brep = IRhinoGeometry.GetBrepFromElement(mesh, Key);
        }

        public BoundingBox ClippingBox => throw new NotImplementedException();

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            args.Pipeline.DrawBrepShaded(brep, args.Material);
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            Curve[] temp = brep.DuplicateNakedEdgeCurves(true, true);
            foreach (Curve c in temp)
            {
                args.Pipeline.DrawCurve(c, args.Color);
            }
        }
        #endregion

        #region IGH_Goo methods
        public bool IsValid
        {
            get => this.Equals(null);
        }

        public string IsValidWhyNot
        {
            get
            {
                string msg;
                if (this.IsValid) msg = string.Empty;
                else msg = string.Format("Invalid type.", this.TypeName);
                return msg;
            }
        }

        public string TypeName
        {
            get
            {
                string msg = "AHF-Element is undefined";
                switch (TopologicDimension)
                {
                    case 1:
                        msg = "AHF-IEdge";
                        break;
                    case 2:
                        msg = "AHF-IFace";
                        break;
                    case 3:
                        msg = "AHF-ICell";
                        break;
                }
                return msg;
            }
        }

        public string TypeDescription
        {
            get => ToString();
        }

        public IGH_Goo Duplicate()
        {
            return (IGH_Goo)this.MemberwiseClone();
        }

        public IGH_GooProxy EmitProxy()
        {
            return null;
        }

        public bool CastFrom(object source)
        {
            return false;
        }

        public bool CastTo<T>(out T target)
        {
            if (typeof(T).IsAssignableFrom(typeof(IElement)))
            {
                if (this == null)
                    target = default(T);
                else
                    target = (T)(object)this;
                return true;
            }

            target = default(T);
            return false;
        }

        public object ScriptVariable()
        {
            return this;
        }

        public bool Write(GH_IWriter writer)
        {
            return true;
        }

        public bool Read(GH_IReader reader)
        {
            return true;
        }
        #endregion
    }
}
