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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh
    {
        internal void CleanAllElementsTopologicalData()
        {
            _tempVertexToHalfFacets.Clear();
            IElement e;
            for (int i = 0; i < _maxDimension; i++)
            {
                if (_elements[i]!=null || _elements[i].Count>0)
                {
                    var keys = _elements[i].Keys;
                    foreach (int eK in keys)
                    {
                        e = _elements[i][eK];
                        e.CleanTopologicalData();
                        InitializeElementTopologicalData(e);
                    }
                }
            }
        }

        public void AddElement(IElement element)
        {
            element.Key = elementKey;
            InitializeElementTopologicalData(element);
            _elements[element.TopologicDimension-1].Add(elementKey, element);
            _keyMaps.Add(elementKey, element.TopologicDimension-1);
            elementKey++;
        }

        // Initialize the topological data of an element
        internal void InitializeElementTopologicalData(IElement element)
        {
            // Initialize topological data
            for (Int32 halfFacetID = 1; halfFacetID <= element.HalfFacetsCount; halfFacetID++)
            {
                //Adjacent vertices
                Int32[] hf;
                element.GetHalfFacet(halfFacetID, out hf);

                Int64 sibData = (Int64)element.Key << 32 | (Int64)halfFacetID;

                //Find vertex with larger ID
                //Int32 v = hf.Max();

                foreach (int v in hf)
                {
                    if (!_tempVertexToHalfFacets.ContainsKey(v)) _tempVertexToHalfFacets.Add(v, new HashSet<long>());
                    _tempVertexToHalfFacets[v].Add(sibData);
                }
            }
        }

        public void AddRangeElements(List<IElement> elements)
        {
            elements.ForEach(e => AddElement(e));
        }

        public void CleanElements()
        {
            _elements = new Dictionary<int, IElement>[_maxDimension];
            for(int i=0; i<_maxDimension; i++)
            {
                _elements[i] = new Dictionary<int, IElement>();
            }
        }

        public void CleanElementsVisits()
        {
            for (int i = 0; i < _maxDimension; i++)
            {
                Parallel.ForEach(_elements[i].Values, e =>
                {
                    e.Visited = false;
                });
            }
        }

        public bool ContainsElementKey(int key)
        {
            int dim = _keyMaps[key];
            return _elements[dim].ContainsKey(key);
        }

        public void DeleteElements(IEnumerable<int> eKeys, bool updateTopology = true)
        {
            foreach(int eK in eKeys)
            {
                DeleteElement(eK, false);
            }
            if (updateTopology) BuildTopology(true);
        }

        /// <summary>
        /// Delete an element and update topological data. Returns the keys of vertices that need to be updated.
        /// </summary>
        public void DeleteElement(int eKey, bool updateTopology=true)
        {
            int dim = _keyMaps[eKey];
            _elements[dim].Remove(eKey);
            _keyMaps.Remove(eKey);

            if(updateTopology) BuildTopology(true);
        }

        public IElement GetElementWithKey(int key)
        {
            if (_keyMaps.ContainsKey(key))
            {
                int dim = _keyMaps[key];
                return _elements[dim][key];
            }
            else return null;
        }

        public int FindNextElementKey()
        {
            return elementKey;
        }

        public List<int> ElementsKeys
        {
            get => _keyMaps.Keys.ToList();
        }

        public List<IElement> Elements
        {
            get
            {
                List<IElement> result = new List<IElement>();
                for(int i=0; i<_maxDimension; i++)
                {
                    if (_elements[i] != null)
                    {
                        result.AddRange(_elements[i].Values);
                    }
                }
                return result;
            }
        }

        public int ElementsCount
        {
            get => _keyMaps.Count;
        }
    }
}
