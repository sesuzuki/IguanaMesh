using Iguana.IguanaMesh.IUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes.ICollections
{
    public class IElementCollection
    {
        private Dictionary<int, IElement> _elements;

        //Temporary data structures for construction
        private Dictionary<Int32, List<Int64>> _tempVertexToHalfFacets;
        private Dictionary<Int64, List<Int32>> _tempVertexToAdjacentHalfFacets;
        private IMesh mesh;

        public IElementCollection()
        {
            _elements = new Dictionary<int, IElement>();
            _tempVertexToAdjacentHalfFacets = new Dictionary<Int64, List<Int32>>();
            _tempVertexToHalfFacets = new Dictionary<Int32, List<Int64>>();
        }

        public IElementCollection ShallowCopy()
        {
            return (IElementCollection)this.MemberwiseClone();
        }

        public IElementCollection DeepCopy()
        {
            IElementCollection copy = ShallowCopy();
            copy._elements = _elements.ToDictionary(entry => entry.Key, entry => (IElement) entry.Value.Clone());
            return copy;
        }

        /// <summary>
        /// Copy without topologic data.
        /// </summary>
        /// <returns></returns>
        public IElementCollection CleanCopy()
        {
            IElementCollection copy = new IElementCollection();
            _elements.Values.All(e =>
            {
                IElement cE = e.CleanCopy();
                cE.Key = e.Key;
                copy.AddElement(cE.Key, cE);
                return true;
            });
            return copy;
        }

        internal void CleanTopologicalData()
        {
            IElement e;
            foreach(int eK in ElementsKeys)
            {
                e = _elements[eK];
                e.CleanTopologicalData();
                _elements[eK] = e;
            }
        }

        public void AddElement(int key, IElement element)
        {
            if (key <= 0) key = FindNextKey();
            element.Key = key;

            InitializeTopologicalData(element);
            _elements.Add(key, element);
        }

        /// <summary>
        /// Construction of Sibling Half-Facets (sibhfs). </para>
        /// From element´s connectivity (input), it returns a cyclic mapping of sibbling half-V (output).
        /// (CAUTION) Note that the local half-facet indexing starts with 1; 
        /// </summary>
        internal bool BuildTopologicalData()
        {
            try
            {
                //local half-facet indexing start with 1 and not 0; 
                foreach (Int32 elementID in _elements.Keys)
                {
                    BuildElementTopologicalData(elementID);
                }
                //_tempVertexToAdjacentHalfFacets.Clear();
                //_tempVertexToHalfFacets.Clear();
                return true;
            }catch(Exception){ return false; }
        }

        private void BuildElementTopologicalData(int elementID)
        {
            IElement e = _elements[elementID];

            //Half-Facets from element e (Faces to edges)
            for (Int32 halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
            {
                if (e.GetSiblingHalfFacet(halfFacetID) == 0)
                {
                    //Adjacent vertices
                    int[] hf;
                    e.GetHalfFacet(halfFacetID, out hf);

                    //Find vertex with larger ID
                    int v = hf.Max();

                    Int64 sibData = (Int64)elementID << 32 | (Int64)halfFacetID;

                    // Adjacent vertices
                    List<Int32> us = _tempVertexToAdjacentHalfFacets[sibData];

                    //Step : Find half-V in _tempVertexToHalEdges(v) subject to v2adj(v,.)=us;
                    // Half-V in _tempVertexToHalEdges associated with v;
                    List<Int64> hfs_V = _tempVertexToHalfFacets[v];

                    foreach (Int64 hfs_v_f in hfs_V)
                    {
                        List<Int32> hfs_us = _tempVertexToAdjacentHalfFacets[hfs_v_f];

                        if (!hfs_v_f.Equals(sibData))
                        {
                            int eval = hfs_us.Count < us.Count ? hfs_us.Count : us.Count;
                            if (hfs_us.Intersect(us).Count() == eval)
                            {
                                e.SetSiblingHalfFacet(halfFacetID, hfs_v_f);
                            }
                        }
                    }
                }
            }
        }

        public bool UpdateAllTopologicalData() 
        {
            try
            {
                CleanTopologicalData();
                IElement e;
                foreach(int eK in _elements.Keys)
                {
                    e = _elements[eK];
                    InitializeTopologicalData(e);
                    _elements[eK] = e;
                }
                BuildTopologicalData();
                return true;
            }
            catch (Exception) { return false; }
        }

        // Initialize the topological data of an element
        internal void InitializeTopologicalData(IElement element)
        {
            // Initialize topological data
            for (Int32 halfFacetID = 1; halfFacetID <= element.HalfFacetsCount; halfFacetID++)
            {
                //Adjacent vertices
                Int32[] hf;
                element.GetHalfFacet(halfFacetID, out hf);

                //Find vertex with larger ID
                Int32 v = hf.Max();

                Int64 sibData = (Int64)element.Key << 32 | (Int64)halfFacetID;

                if (!_tempVertexToHalfFacets.ContainsKey(v)) _tempVertexToHalfFacets.Add(v, new List<Int64> { });
                _tempVertexToHalfFacets[v].Add(sibData);

                if (!_tempVertexToAdjacentHalfFacets.ContainsKey(sibData)) _tempVertexToAdjacentHalfFacets.Add(sibData, new List<int>());
                _tempVertexToAdjacentHalfFacets[sibData].AddRange(hf);
            }
        }

        public void AddRangeElements(List<IElement> elements)
        {
            elements.ForEach(e => AddElement(e.Key, e));
        }

        public void Clean()
        {
            _elements = new Dictionary<int, IElement>();
        }

        public void CleanVisits()
        {
            Parallel.ForEach(_elements.Values, e =>
            {
                e.Visited = false;
            });
        }

        public bool ContainsKey(int key)
        {
            return _elements.ContainsKey(key);
        }

        /// <summary>
        /// Delete an element and update topological data. Returns the keys of vertices that need to be updated.
        /// </summary>
        internal bool DeleteElementTopologically(int eKey, out int[] vertices)
        {
            vertices = new int[0];
            try
            {
                Int64[] eData = _elements[eKey].GetSiblingHalfFacets();
                int sib_e, sibsib_e, sibsibsib_e, sib_hf, sibsib_hf, sibsibsib_hf;
                Int64 sibsib_pack, sibsibsib_pack;
                foreach (Int64 sib_pack in eData)
                {
                    IHelpers.UnpackKey(sib_pack, out sib_e, out sib_hf);
                    sibsib_pack = _elements[sib_e].GetSiblingHalfFacet(sib_hf);
                    IHelpers.UnpackKey(sibsib_pack, out sibsib_e, out sibsib_hf);
                    if (sibsib_e == eKey)
                    {
                        _elements[sib_e].SetSiblingHalfFacet(sib_hf, 0, 0);
                        _tempVertexToAdjacentHalfFacets.Remove(sib_pack);
                        _elements.Remove(eKey);
                        BuildElementTopologicalData(sib_e);
                        vertices = _elements[sib_e].Vertices;
                    }
                    else
                    {
                        while (sibsib_e != eKey)
                        {
                            sibsibsib_pack = _elements[sibsib_e].GetSiblingHalfFacet(sibsib_hf);
                            IHelpers.UnpackKey(sibsibsib_pack, out sibsibsib_e, out sibsibsib_hf);
                            if (sibsibsib_e == eKey)
                            {
                                _elements[sibsib_e].SetSiblingHalfFacet(sibsib_hf, sib_e, sib_hf);
                                _tempVertexToAdjacentHalfFacets.Remove(sibsibsib_pack);
                                _elements.Remove(eKey);
                                BuildElementTopologicalData(sibsibsib_e);
                                vertices = _elements[sib_e].Vertices;
                            }
                            sibsib_e = sibsibsib_e;
                        }
                    }
                }
                return true;
            }
            catch (Exception) { return false; }
        }

        public IElement GetElementWithKey(int key)
        {
            return _elements[key];
        }

        public int FindNextKey()
        {
            List<int> keys = ElementsKeys;
            keys.Sort();
            if (ElementsKeys.Count == 0) return 1;
            else return keys[keys.Count - 1] + 1;
        }

        public List<int> ElementsKeys
        {
            get => _elements.Keys.ToList();
        }

        public List<IElement> ElementsValues
        {
            get => _elements.Values.ToList();
        }

        public int Count
        {
            get => _elements.Count;
        }

        public override string ToString()
        {
            return "IElementCollection{" + Count + "}";
        }
    }
}
