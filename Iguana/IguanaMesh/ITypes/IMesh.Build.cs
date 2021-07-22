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

using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh
    {
        /// <summary>
        /// Build topologic relationships.
        /// </summary>
        public void BuildTopology(bool cleanPreviousTopology=false)
        {
            if (cleanPreviousTopology)
            {
                CleanAllElementsTopologicalData();
                CleanAllVerticesTopologicalData();
            }

            //BuildSiblingHalfFacets
            _elementTypes.Clear();
            Boolean flag1 = BuildAllElementsSiblingHalfFacets();
            Boolean flag2 = BuildAllVertexToHalfFacet();
            _renderMesh = new Mesh();

            string type = this.GetMeshTypeDescription();
            message = "IMesh (Vertices: " + Vertices.Count + "; Elements: " + Elements.Count + " ; Type: " + type + ")";

            if (!flag1 || !flag2)
            {
                message += "\n\n||||| Invalid Data-Structure |||||\nSibling Half-Facets (sibhfs): ";
                if (flag1) message += " Built;\n";
                else if (!flag1) message += " Errors Found;\n";

                message += "Vertex to Half-Facet (v2hf): ";
                if (flag2) message += " Built;\n";
                else if (!flag2) message += " Errors Found;\n";
                _valid = false;
            }
            else
            {
                UpdateGraphics();
                _valid = true;
            }
        }

        public bool UpdateAllElementsSiblingHalfFacets()
        {
            try
            {
                CleanAllElementsTopologicalData();
                IElement e;
                ElementsKeys.ForEach(eK =>
                {
                    int dim = _keyMaps[eK];
                    e = _elements[dim][eK];
                    InitializeElementTopologicalData(e);
                    _elements[dim][eK] = e;
                });
                BuildAllElementsSiblingHalfFacets();
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// First step: Construction of Sibling Half-Facets (sibhfs). </para>
        /// From element´s connectivity (input), it returns a cyclic mapping of sibbling half-V (output).
        /// (CAUTION) Note that the local half-facet indexing starts with 1; 
        /// </summary>
        private bool BuildAllElementsSiblingHalfFacets()
        {
            ElementsKeys.ForEach(elementID => BuildElementSiblingHalFacets(elementID));
            _tempVertexToHalfFacets.Clear();
            return true;
        }

        private void BuildElementSiblingHalFacets(int elementID)
        {
            int dim = _keyMaps[elementID];
            IElement e = _elements[dim][elementID];
            HashSet<long> vertexSiblings;
            _elementTypes.Add(e.ElementType);

            //Half-Facets from element e
            for (Int32 halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
            {
                if (e.GetSiblingHalfFacet(halfFacetID) == 0)
                {
                    int[] hf;
                    e.GetHalfFacet(halfFacetID, out hf);

                    Int64 current_KeyPair = (Int64)elementID << 32 | (Int64)halfFacetID;

                    // Find vertex with larger ID
                    Int32 v = hf.Max();

                    // Get adjacent vertices
                    int[] adj_v;
                    if (hf.Length == 1)
                    {
                        adj_v = new int[] { hf[0] };
                    }
                    else if (hf.Length == 2)
                    {
                        adj_v = new int[] { hf[0] == v ? hf[1] : hf[0] };
                    }
                    else
                    {
                        int idx = Array.IndexOf(hf, v);
                        int prev = idx - 1;
                        int next = idx + 1;
                        if (prev < 0) prev = hf.Length - 1;
                        if (next > hf.Length - 1) next = 0;

                        adj_v = new int[] { hf[prev], hf[next]};
                    }

                    for (int i = 0; i < adj_v.Length; i++)
                    {
                        vertexSiblings = _tempVertexToHalfFacets[adj_v[i]];

                        // Temporary list of sibhfs for cyclical mapping 
                        List<long> collectSibhf = new List<long>();

                        foreach (Int64 sibling_KeyPair in vertexSiblings)
                        {
                            int sib_elementID, sib_halfFacetID;
                            IHelpers.UnpackKey(sibling_KeyPair, out sib_elementID, out sib_halfFacetID);

                            // Check the dimensionality of the element to associate elements of the same dimension.
                            int sib_dim = _keyMaps[sib_elementID];

                            if (dim != 0 && !sib_elementID.Equals(elementID) && sib_dim == dim)
                            {
                                int[] hfs_us;
                                _elements[sib_dim][sib_elementID].GetHalfFacet(sib_halfFacetID, out hfs_us);

                                int eval = hf.Length;
                                // For 3D elements, we assume that two elements are associated if they share a common edge.
                                if (dim == 2) eval = 2;

                                if (hfs_us.Intersect(hf).Count() >= eval)                                
                                    e.SetSiblingHalfFacet(halfFacetID, sibling_KeyPair);                                
                            }

                            if (dim == 0 && sib_dim == 0)
                            {
                                int[] hfs_us;
                                _elements[sib_dim][sib_elementID].GetHalfFacet(sib_halfFacetID, out hfs_us);

                                if (hfs_us.Intersect(hf).Count() >= 1)                                
                                    collectSibhf.Add(sibling_KeyPair);
                                
                            }

                        }

                        // Cyclical Mapping of sibhf for 1D elements only
                        if (dim == 0)
                            for (int sib = 0; sib < collectSibhf.Count; sib++)
                            {
                                int next = sib + 1;
                                if (sib + 1 == collectSibhf.Count) next = 0;

                                int hf_e, hf_ind;
                                IHelpers.UnpackKey(collectSibhf[sib], out hf_e, out hf_ind);

                                _elements[dim][hf_e].SetSiblingHalfFacet(hf_ind, collectSibhf[next]);
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Second Step: Construction of mapping from vertex to an incident half-facet (v2hf).
        /// From element´s connectivity and cyclic mapping of sibblings half-V (input), it returns a collection of maps from vertices to incident half-facet (output).
        /// </summary>
        private Boolean BuildAllVertexToHalfFacet()
        {
            Boolean flag = true;
            try
            {
                foreach (Int32 elementID in ElementsKeys)
                {
                    IElement e = GetElementWithKey(elementID);

                    //Give border V higher priorities
                    for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                    {
                        //Vertices of the facet
                        int[] hf;
                        e.GetHalfFacet(halfFacetID, out hf);

                        for(int i=0; i< hf.Length; i++)
                        {
                            BuildVertexToHalfFacet(hf[i], elementID, halfFacetID);
                        }
                    }
                }
            }
            catch (Exception) { flag = false; }

            return flag;
        }

        public void UpdateGraphics()
        {
            _renderMesh = IRhinoGeometry.TryGetRhinoMesh(this);
        }

        private void BuildVertexToHalfFacet(int vKey, int elementID, int halfFacetID)
        {
            ITopologicVertex v = GetVertexWithKey(vKey);
            int dim = _keyMaps[elementID];

            if (v.V2HF[dim] == 0)
            {
                v.SetV2HF(dim, elementID, halfFacetID);
                SetVertex(vKey, v);
            }

            if (_elements[dim][elementID].GetSiblingHalfFacet(halfFacetID) == 0)
            {
                if (v.GetElementID(dim) != elementID || v.GetHalfFacetID(dim) != halfFacetID)
                {
                    v.SetV2HF(dim, elementID, halfFacetID);
                    SetVertex(vKey, v);
                }
            }
        }

        public void RemoveDuplicateVertices()
        {
            PointCloud cloud = new PointCloud();
            List<ITopologicVertex> vertices = Vertices;
            List<int> culledVKeys = new List<int>();
            Dictionary<int, IElement>[] modifiedElements = new Dictionary<int, IElement>[_maxDimension];
            for(int i=0; i<_maxDimension; i++)
            {
                foreach (KeyValuePair<int, IElement> entry in _elements[i])
                {
                    modifiedElements[i].Add(entry.Key, (IElement)entry.Value.Clone());
                }
            }
            Dictionary<int, ITopologicVertex> culledVertices = new Dictionary<int, ITopologicVertex>();
            ITopologicVertex v;
            IElement e;
            Point3d p;
            int idx;
            double t = 0.0001;

            for (int i=0; i<vertices.Count; i++)
            {
                v = vertices[i];
                p = v.RhinoPoint;

                idx = IKernel.EvaluatePoint(cloud, p, t);

                if(idx == -1)
                {
                    culledVertices.Add(v.Key, v);
                    culledVKeys.Add(v.Key);
                    cloud.Add(p);
                }
                else
                {
                    int[] eKeys = Topology.GetVertexIncidentElements(v.Key);

                    foreach(int eK in eKeys)
                    {
                        int dim = _keyMaps[eK];
                        e = modifiedElements[dim][eK];
                        int[] vE = new int[e.VerticesCount];

                        for(int j=0; j<vE.Length; j++)
                        {
                            vE[j] = e.Vertices[j];
                            if (vE[j] == v.Key)
                            {
                                vE[j] = culledVKeys[idx];
                            }
                        }

                        e.Vertices = vE;

                        modifiedElements[dim][eK] = e;
                    }
                }
            }

            _vertices = culledVertices;
            _elements = modifiedElements;
            BuildTopology(true);
        }
    }
}
