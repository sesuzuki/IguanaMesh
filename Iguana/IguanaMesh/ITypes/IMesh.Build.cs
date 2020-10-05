using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh
    {
        /// <summary>
        /// <para> First Step: Construction of Sibling Half-Facets (sibhfs). </para>
        /// <para> From element´s connectivity (input), it returns a cyclic mapping of sibbling half-V (output). </para>
        /// </summary>
        private Boolean BuildSiblingHalfFacets()
        {
            Boolean flag = true;
            try
            {
                //Part 1
                foreach (Int32 elementID in Elements.ElementsKeys)
                {

                    IElement e = Elements.GetElementWithKey(elementID);

                    //Half-facets from element
                    for (Int32 halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                    {
                        //Adjacent vertices
                        Int32[] us;
                        e.GetHalfFacet(halfFacetID, out us);

                        //Find vertex with larger ID
                        Int32 v = us[0];
                        us.ToList().ForEach(idx => { if (v < idx) v = idx; });

                        Int64 sibData = (Int64)elementID << 32 | (Int64)halfFacetID;

                        if (!_tempVertexToHalfFacets.ContainsKey(v)) _tempVertexToHalfFacets.Add(v, new List<Int64> { });
                        _tempVertexToHalfFacets[v].Add(sibData);

                        if (!_tempVertexToAdjacentHalfFacets.ContainsKey(sibData)) _tempVertexToAdjacentHalfFacets.Add(sibData, new List<int>());
                        _tempVertexToAdjacentHalfFacets[sibData].AddRange(us);
                    }
                }

                //Part 2
                //local half-facet indexing start with 1 and not 0; 
                foreach (Int32 elementID in Elements.ElementsKeys)
                {
                    IElement e = Elements.GetElementWithKey(elementID);

                    //Half-Facets from element e (Faces to edges)
                    for (Int32 halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                    {
                        if (e.GetSiblingHalfFacet(halfFacetID) == 0)
                        {
                            //Adjacent vertices
                            int[] hf;
                            e.GetHalfFacet(halfFacetID, out hf);

                            //Find vertex with larger ID
                            int v = hf[0];
                            hf.ToList().ForEach(idx => { if (v < idx) v = idx; });

                            Int64 sibData = (Int64)elementID << 32 | (Int64)halfFacetID;

                            // Adjacent vertices
                            List<Int32> us = _tempVertexToAdjacentHalfFacets[sibData];

                            //Step : Find half-V in _tempVertexToHalEdges(v) subject to v2adj(v,.)=us;
                            // Half-V in _tempVertexToHalEdges associated with v;
                            List<Int64> hfs_V = _tempVertexToHalfFacets[v];

                            foreach (Int64 hfs_v_f in hfs_V)
                            {
                                List<Int32> hfs_us = _tempVertexToAdjacentHalfFacets[hfs_v_f];

                                if (!hfs_v_f.Equals(sibData) && hfs_us.All(el => us.Contains(el)))
                                {
                                    e.SetSiblingHalfFacet(halfFacetID, hfs_v_f);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { flag = false; }

            return flag;
        }

        /// <summary>
        /// <para> Second Step: Construction of mapping from vertex to an incident half-facet (v2hf). </para>
        /// <para> From element´s connectivity and cyclic mapping of sibblings half-V (input), it returns a collection of maps from vertices to incident half-facet (output). </para>
        /// </summary>
        private Boolean BuildVertexToHalfFacet()
        {
            Boolean flag = true;
            try
            {
                foreach (Int32 elementID in Elements.ElementsKeys)
                {
                    IElement e = Elements.GetElementWithKey(elementID);
                    ITopologicVertex v;

                    for (Int32 vertexID = 0; vertexID < e.VerticesCount; vertexID++)
                    {

                        Int32 vKey = e.Vertices[vertexID];
                        v = Vertices.GetVertexWithKey(vKey);

                        if (v.V2HF == 0)
                        {
                            v.SetV2HF(elementID, vertexID);

                            Vertices.SetVertex(vKey, v);
                        }
                    }


                    //Give border V higher priorities
                    for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                    {
                        if (e.GetSiblingHalfFacet(halfFacetID) == 0)
                        {
                            //Vertices of the facet
                            int[] hf;
                            e.GetHalfFacet(halfFacetID, out hf);

                            foreach (int vKey in hf)
                            {
                                v = Vertices.GetVertexWithKey(vKey);

                                if (v.GetElementID() != elementID || v.GetHalfFacetID() != halfFacetID)
                                {
                                    v.SetV2HF(elementID, halfFacetID);
                                    Vertices.SetVertex(vKey, v);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { flag = false; }

            return flag;
        }

        /// <summary>
        /// Build topologic relationships.
        /// </summary>
        public void BuildTopology()
        {
            Boolean flag1 = BuildSiblingHalfFacets();
            Boolean flag2 = BuildVertexToHalfFacet();
            //CullFloatingVertices();

            string type = this.GetMeshTypeDescription();
            message = "AHF-Mesh (Vertices: " + Vertices.Count + "; Elements: " + Elements.Count + " ; Type: " + type + ")";

            if (!flag1 || !flag2)
            {
                message += "\n\n||||| Invalid Data-Structure |||||\nSibling Half-Facets (sibhfs): ";
                if (flag1) message += " Built;\n";
                else if (!flag1) message += " Errors Found;\n";

                message += "Vertex to Half-Facet (v2hf): ";
                if (flag2) message += " Built;\n";
                else if (!flag2) message += " Errors Found;\n";
            }
        }
    }
}
