/*
 * <Iguana>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ITypes
{
    /// <summary>
    /// Abstract class for the explicit construction of a d-dimensional entitiy in an Array-based Data Structure. 
    /// </summary>
    public abstract class IElement : IGH_Goo, ICloneable, IGH_PreviewData
    {
        public int TopologicDimension { get; set; }
        public int Key { get; set; } = -1;

        /// <summary>
        /// Sibling half-facets are packed within a 64-bits integer. The default value is 0 representing naked half-facets when the topology is build or not intiliazed values when the topology wasn´t built. 
        /// </summary>
        private Int64[] _siblingHalfFacets;
        private bool[] _visits;
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
        ///
        public IElement(int[] vKeys, int halfFacetCount, int topologicDimension, int elementType)
        {
            init(vKeys, halfFacetCount, topologicDimension, elementType);
        }

        /// <summary>
        /// Initializes element data
        /// </summary>
        protected void init(int[] vKeys, int halfFacetCount, int topologicDimension, int elementType)
        {
            Vertices = vKeys;
            TopologicDimension = topologicDimension;
            _siblingHalfFacets = new Int64[halfFacetCount];
            _visits = new bool[halfFacetCount];
            _elementType = elementType;
        }

        /// <summary>
        /// Return the index of the half-facet containing the given vertices keys. Return -1 if the given combination of vertices keys is not found in a half-facet of the element. 
        /// </summary>
        public int GetHalfFacetIndexContainingVertices(int[] vertexKeys)
        {
            int[] hf;
            for(int i=1; i<=HalfFacetsCount; i++)
            {
                GetHalfFacet(i, out hf);
                hf.ToList();
                if (vertexKeys.All(vK => hf.Contains(vK))) return i;
            }
            return -1;
        }

        /// <summary>
        /// Get or set the collection of vertex identifiers. 
        /// </summary>
        ///
        public int VerticesCount
        {
            get => Vertices.Length;
        }

        public void CleanTopologicalData()
        {
            _siblingHalfFacets = new Int64[HalfFacetsCount];
        }

        public int HalfFacetsCount
        {
            get => _siblingHalfFacets.Length;
        }

        public abstract int[] GetGmshFormattedVertices();

        public Int32 GetSiblingElementID(int index)
        {
            return (Int32)(_siblingHalfFacets[index - 1] >> 32);
        }

        public Int32 GetSiblingHalfFacetID(int index)
        {
            return (Int32)_siblingHalfFacets[index - 1];
        }

        public void RegisterHalfFacetVisit(int index)
        {
            _visits[index - 1] = true;         
        }

        internal void SetElementType(int elementType) 
        { 
            _elementType = elementType; 
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public void ClearSiblingHalfFacetVisit(int index)
        {

            _visits = new bool[HalfFacetsCount];
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public Boolean IsHalfFacetVisited(int index)
        {
            return _visits[index - 1];
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public Boolean IsNakedSiblingHalfFacet(int index)
        {
            if (_siblingHalfFacets[index - 1] == 0) return true;
            else return false;
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public void SetSiblingHalfFacet(int index, Int32 elementID, Int32 halfFacetID)
        {
            _siblingHalfFacets[index - 1] = (Int64)elementID << 32 | (Int64)halfFacetID;
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public void SetSiblingHalfFacet(int index, Int64 sibData)
        {
            _siblingHalfFacets[index - 1] = sibData;
        }

        /// <summary>
        /// Local Half-Facet indexing convention starts with 1;
        /// </summary>
        public Int64 GetSiblingHalfFacet(int index)
        {
            return _siblingHalfFacets[index - 1];
        }

        public Int64[] GetSiblingHalfFacets()
        {
            return _siblingHalfFacets;
        }

        /// <summary>
        /// Removes all vertex identifiers. 
        /// </summary>
        public void Clear()
        {
            Vertices = new int[0];
            _siblingHalfFacets = new Int64[0];
            TopologicDimension = 0;
            _visits = new bool[0];
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
                foreach(bool v in _visits)
                {
                    if (v==false) return false;
                }
                return true;
            }
            set
            {
                for (int i=0; i<_visits.Length; i++)
                {
                    _visits[i] = value;
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
        /// Abstract method to add a vertex identifier to the element.
        /// <paramref name="vertex"/> : The vertex identifier to add.
        /// </summary>
        public abstract Boolean AddVertex(int vertexKey);

        /// <summary>
        /// <para> Remove a vertex identifier from the element. </para> 
        /// <paramref name="vertex"/> : The vertex to remove.
        /// </summary>
        ///
        public abstract Boolean RemoveVertex(int vertexKey);

        /// <summary>
        /// <para> Abstract method to return a Half-Facet. A Half-Facet is a (d-1)-dimensional sub-entity of a d-dimensional entity. </para>
        /// <paramref name="index"/> : The local index of the Half-Facet to search within the element.
        /// <paramref name="halfFacets"/> : A sub-list to store all the vertex identifiers representing the Half-Facet.
        /// </summary>
        ///
        public abstract Boolean GetHalfFacet(int index, out int[] halfFacets);


        public int GetHalFacetContainingVertices(int[] vKey)
        {
            for(int i=1; i<=HalfFacetsCount; i++)
            {
                int[] hf;
                GetHalfFacet(i, out hf);
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
                    if (TopologicDimension == 2) msg += "Face Element ID: " + GetSiblingElementID(i) + " :: Half-Edge ID: " + GetSiblingHalfFacetID(i) + "\n";
                    if (TopologicDimension == 3) msg += "Solid Element ID: " + GetSiblingElementID(i) + " :: Half-Face ID: " + GetSiblingHalfFacetID(i) + "\n";
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
