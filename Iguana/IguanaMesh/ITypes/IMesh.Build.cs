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
                    e = _elements[eK];
                    InitializeElementTopologicalData(e);
                    _elements[eK] = e;
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
            IElement e = _elements[elementID];
            IElement nE;
            HashSet<long> vertexSiblings;

            //Half-Facets from element e (Faces to edges)
            for (Int32 halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
            {
                if (e.GetSiblingHalfFacet(halfFacetID) == 0)
                {
                    //Adjacent vertices
                    int[] hf;
                    e.GetHalfFacet(halfFacetID, out hf);

                    Int64 current_KeyPair = (Int64)elementID << 32 | (Int64)halfFacetID;

                    for (int i = 0; i < hf.Length; i++)
                    {
                        vertexSiblings = _tempVertexToHalfFacets[hf[i]];
                        vertexSiblings.OrderBy(data => (Int64)data << 32).ToList();

                        foreach (Int64 sibling_KeyPair in vertexSiblings)
                        {
                            int sib_elementID, sib_halfFacetID;
                            IHelpers.UnpackKey(sibling_KeyPair, out sib_elementID, out sib_halfFacetID);
                            nE = _elements[sib_elementID];

                            if (!sibling_KeyPair.Equals(current_KeyPair))
                            {
                                int[] hfs_us;
                                nE.GetHalfFacet(sib_halfFacetID, out hfs_us);

                                int eval = hfs_us.Length < hf.Length ? hfs_us.Length : hf.Length;

                                if (hfs_us.Intersect(hf).Count() == eval && e.GetSiblingHalfFacet(halfFacetID) == 0)
                                {
                                    e.SetSiblingHalfFacet(halfFacetID, sibling_KeyPair);

                                    e = nE;
                                    current_KeyPair = sibling_KeyPair;
                                    halfFacetID = sib_halfFacetID;
                                }
                            }
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

            if (v.V2HF == 0)
            {
                v.SetV2HF(elementID, halfFacetID);
                SetVertex(vKey, v);
            }

            if (_elements[elementID].GetSiblingHalfFacet(halfFacetID) == 0)
            {
                if (v.GetElementID() != elementID || v.GetHalfFacetID() != halfFacetID)
                {
                    v.SetV2HF(elementID, halfFacetID);
                    SetVertex(vKey, v);
                }
            }
        }

        public void RemoveDuplicateVertices()
        {
            PointCloud cloud = new PointCloud();
            List<ITopologicVertex> vertices = Vertices;
            List<int> culledVKeys = new List<int>();
            Dictionary<int, IElement> modifiedElements = new Dictionary<int, IElement>(_elements);
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
                        e = modifiedElements[eK];
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

                        modifiedElements[eK] = e;
                    }
                }
            }

            _vertices = culledVertices;
            _elements = modifiedElements;
            BuildTopology(true);
        }
    }
}
