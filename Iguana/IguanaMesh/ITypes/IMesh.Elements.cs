using Iguana.IguanaMesh.ITypes.IElements;
using Iguana.IguanaMesh.IUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh
    {
        internal void CleanAllElementsTopologicalData()
        {
            _tempVertexToHalfFacets.Clear();
            IElement e;
            foreach (int eK in ElementsKeys)
            {
                e = _elements[eK];
                e.CleanTopologicalData();
                if (e.TopologicDimension == 3) Initialize3DElement(e);
                else Initialize2DElement(e);
                _elements[eK] = e;
            }
        }

        public void AddElement(IElement element)
        {
            element.Key = elementKey;
            if (element.TopologicDimension == 3) Initialize3DElement(element);
            else Initialize2DElement(element);
            _elements.Add(elementKey, element);
            elementKey++;
        }


        // Initialize the topological data of an element
        internal void InitializeElementTopologicalData(IElement element)
        {
            // Initialize topological data
            for (int halfFacetID = 1; halfFacetID <= element.HalfFacetsCount; halfFacetID++)
            {
                //Adjacent vertices
                int[] hf;
                element.GetFirstLevelHalfFacet(halfFacetID, out hf);

                long sibData = IHelpers.PackKeyPair(element.Key,halfFacetID);

                //Find vertex with larger ID
                int v = hf.Max();
                if (!_tempVertexToHalfFacets.ContainsKey(v)) _tempVertexToHalfFacets.Add(v, new List<long>());
                _tempVertexToHalfFacets[v].Add(sibData);
            }
        }

        internal void Initialize3DElement(IElement element)
        {
            // Initialize topological data
            for (int hf_parent = 1; hf_parent <= element.HalfFacetsCount; hf_parent++)
            {
                //Adjacent vertices
                int[] hf_P;
                element.GetFirstLevelHalfFacet(hf_parent, out hf_P);

                for(int hf_child=1; hf_child<=hf_P.Length; hf_child++)
                {
                    int[] hf_C;
                    element.GetSecondLevelHalfFacet(hf_parent, hf_child, out hf_C);

                    long sibData = IHelpers.PackTripleKey(element.Key, hf_parent, hf_child);

                    //Find vertex with larger ID
                    int v = hf_C.Max();
                    if (!_tempVertexToHalfFacets.ContainsKey(v)) _tempVertexToHalfFacets.Add(v, new List<long>());
                    _tempVertexToHalfFacets[v].Add(sibData);
                }
            }
        }

        internal void Initialize2DElement(IElement element)
        {
            // Initialize topological data
            for (int hf_parent = 1; hf_parent <= element.HalfFacetsCount; hf_parent++)
            {
                //Adjacent vertices
                int[] hf;
                element.GetFirstLevelHalfFacet(hf_parent, out hf);

                long sibData = IHelpers.PackTripleKey(element.Key, hf_parent, 1);

                //Find vertex with larger ID
                int v = hf.Max();
                if (!_tempVertexToHalfFacets.ContainsKey(v)) _tempVertexToHalfFacets.Add(v, new List<long>());
                _tempVertexToHalfFacets[v].Add(sibData);
            }
        }

        public void AddRangeElements(List<IElement> elements)
        {
            elements.ForEach(e => AddElement(e));
        }

        public void CleanElements()
        {
            _elements = new Dictionary<int, IElement>();
            elementKey = 1;
        }

        public void CleanElementsVisits()
        {
            Parallel.ForEach(_elements.Values, e =>
            {
                e.Visited = false;
            });
        }

        public bool ContainsElementKey(int key)
        {
            return _elements.ContainsKey(key);
        }

        public void DeleteElements(IEnumerable<int> eKeys, bool updateTopology = true)
        {
            foreach(int eK in eKeys)
            {
                _elements.Remove(eK);
            }
            if (updateTopology) BuildTopology(true);
        }

        /// <summary>
        /// Delete an element and update topological data. Returns the keys of vertices that need to be updated.
        /// </summary>
        public void DeleteElement(int eKey, bool updateTopology=true)
        {
            _elements.Remove(eKey);
            if(updateTopology) BuildTopology(true);
        }

        public IElement GetElementWithKey(int key)
        {
            return _elements[key];
        }

        public List<int> ElementsKeys
        {
            get => _elements.Keys.ToList();
        }

        public List<IElement> Elements
        {
            get => _elements.Values.ToList();
        }

        public int ElementsCount
        {
            get => _elements.Count;
        }
    }
}
