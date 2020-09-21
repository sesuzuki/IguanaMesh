using System;
using System.Linq;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;

namespace Iguana.IguanaMesh.ITypes
{
    /// <summary>
    /// <para> Abstract class for the explicit construction of a d-dimensional entitiy in an Array-based Data Structure. 
    /// </summary>
    ///
    public abstract class IElement : IGH_Goo, ICloneable
    {
        public int TopologicDimension { get; set; }
        public int Key { get; set; } = -1;

        //Default value Int64 is 0. This values denotes naked hf or not intiliazed values. 
        public Int64[] _siblingHalfFacets;
        public int[] VerticesKeys { get; set; }

        /// <summary>
        /// <para> Customizable constructor. </para>
        /// <para><paramref name="verticesKeys"/> : A collection of vertex identifiers. </para>
        /// <paramref name="dimension"/> : The topologic dimension of the element. The dimension should be larger than 0 and smaller or equal to 3.  
        /// </summary>
        ///
        public IElement(int[] vKeys, int halfFacetCount, int topologicDimension)
        {
            init(vKeys, halfFacetCount, topologicDimension);
        }

        public void init(int[] vKeys, int halfFacetCount, int topologicDimension)
        {
            VerticesKeys = vKeys;
            TopologicDimension = topologicDimension;
            _siblingHalfFacets = new Int64[halfFacetCount];
        }

        /// <summary>
        /// Get or set the collection of vertex identifiers. 
        /// </summary>
        ///
        public int VerticesCount
        {
            get => VerticesKeys.Length;
        }

        public int HalfFacetsCount
        {
            get => _siblingHalfFacets.Length;
        }

        public Int32 GetSiblingElementID(int index)
        {
            //local indexing convention of half-facets start with 1;
            try
            {
                Int64 sibData = _siblingHalfFacets[index - 1];
                if (sibData < 0) sibData *= -1;
                return (Int32)(sibData >> 32);
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"{e.GetType().Name}: {index} is outside the bounds of the array. Local indexing convention of half-facets start with 1.");
                return -1;
            }
        }

        public Int32 GetSiblingHalfFacetID(int index)
        {
            //local indexing convention of half-facets start with 1;
            try
            {
                Int64 sibData = _siblingHalfFacets[index - 1];
                if (sibData < 0) sibData *= -1;
                return (Int32) sibData;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"{e.GetType().Name}: {index} is outside the bounds of the array. Local indexing convention of half-facets start with 1.");
                return 0;
            }
        }

        public void RegisterSiblingHalfFacetVisit(int index)
        {
            //local indexing convention of half-facets start with 1;
            try
            {
                if(_siblingHalfFacets[index-1] > 0) _siblingHalfFacets[index - 1] *= -1;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"{e.GetType().Name}: {index} is outside the bounds of the array. Local indexing convention of half-facets start with 1.");
            }
        }
        public void ClearSiblingHalfFacetVisit(int index)
        {
            //local indexing convention of half-facets start with 1;
            try
            {
                if (_siblingHalfFacets[index - 1] < 0) _siblingHalfFacets[index-1] *= -1;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"{e.GetType().Name}: {index} is outside the bounds of the array. Local indexing convention of half-facets start with 1.");
            }
        }
        public Boolean IsSiblingHalfFacetVisited(int index)
        {
            //local indexing convention of half-facets start with 1;
            try
            {
                if (_siblingHalfFacets[index-1] < 0) return true;
                else return false;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"{e.GetType().Name}: {index} is outside the bounds of the array. Local indexing convention of half-facets start with 1.");
                return false;
            }
        }

        public Boolean IsNakedSiblingHalfFacet(int index)
        {
            //local indexing convention of half-facets start with 1;
            try
            {
                if (_siblingHalfFacets[index - 1] == 0) return true;
                else return false;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"{e.GetType().Name}: {index} is outside the bounds of the array. Local indexing convention of half-facets start with 1.");
                return false;
            }
        }

        public void SetSiblingHalfFacet(int index, Int32 elementID, Int32 halfFacetID)
        {
            //local indexing convention of half-facets start with 1;
            try
            {
                _siblingHalfFacets[index - 1] = (Int64)elementID << 32 | (Int64)halfFacetID;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"{e.GetType().Name}: {index} is outside the bounds of the array. Local indexing convention of half-facets start with 0.");
            }
        }

        public void SetSiblingHalfFacet(int index, Int64 sibData)
        {
            //local indexing convention of half-facets start with 1;
            try
            {
                _siblingHalfFacets[index - 1] = sibData;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"{e.GetType().Name}: {index} is outside the bounds of the array. Local indexing convention of half-facets start with 0.");
            }
        }

        public Int64 GetSiblingHalfFacet(int index)
        {
            //local indexing convention of half-facets start with 1;
            try
            {
                return _siblingHalfFacets[index - 1];
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"{e.GetType().Name}: {index} is outside the bounds of the array. Local indexing convention of half-facets start with 0.");
                return 0;
            }
        }

        public Int64[] GetSiblingHalfFacets()
        {
            return _siblingHalfFacets;
        }

        /// <summary>
        /// Removes all vertex identifiers. 
        /// </summary>
        ///
        public void Clear()
        {
            VerticesKeys = new int[0];
            _siblingHalfFacets = new Int64[0];
            TopologicDimension = 0;
        }

        /// <summary>
        /// Reverse the direction of the element.
        /// </summary>
        public void Flip()
        {
            VerticesKeys.Reverse();
        }

        public Boolean Visited
        {
            get
            {
                foreach(Int64 data in _siblingHalfFacets)
                {
                    if (data > 0) return false;
                    else if (data == 0)
                    {
                        if (BitConverter.GetBytes(data) != BitConverter.GetBytes(-0)) return false;
                    }
                }
                return true;
            }
            set
            {
                for (int i=0; i<_siblingHalfFacets.Length; i++)
                {
                    Int64 sibData = _siblingHalfFacets[i];
                    if (value == true && sibData > 0) sibData *= -1;
                    else if (value == false && sibData < 0) sibData *= -1;
                    _siblingHalfFacets[i] = sibData;
                };
            }
        }

        /// <summary>
        /// <para> Abstract method to add a vertex identifier to the element. </para> 
        /// <paramref name="vertex"/> : The vertex identifier to add.
        /// </summary>
        ///
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


        /// <summary>
        /// <para> Element´s description . </para>
        /// </summary>
        ///
        public override string ToString()
        {
            string msg = "AHF-IElement{";
            for(int i=0; i< VerticesCount; i++)
            {
                int idx = VerticesKeys[i];
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
