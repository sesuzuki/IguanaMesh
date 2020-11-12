using Iguana.IguanaMesh.IUtils;
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
            //try
            //{
                Elements.ForEach(e =>
                {
                    //BuildElementSiblingHalFacets(elementID)
                    if (e.TopologicDimension == 3) Build3DElementSiblingHalFacets(e);
                    else Build2DElementSiblingHalFacets(e);
                });
                _tempVertexToHalfFacets.Clear();
                return true;
            //}
            //catch (Exception) { return false; }
        }

        private void Build2DElementSiblingHalFacets(IElement e)
        {
            int elementID = e.Key;
            IElement nE;

            //Half-Facets from element e (Faces to edges)
            for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
            {
                if (e.GetSiblingHalfFacet(halfFacetID) == 0)
                {
                    //Adjacent vertices
                    int[] hf;
                    e.GetFirstLevelHalfFacet(halfFacetID, out hf);

                    int v = hf.Max();

                    long current_KeyPair = IHelpers.PackTripleKey(elementID, halfFacetID, 1);

                    List<long> vertexSiblings = _tempVertexToHalfFacets[v];

                    foreach (long sibling_KeyPair in vertexSiblings)
                    {
                        int sib_elementID, sib_halfFacetID_parent, sib_halfFacetID_child;
                        IHelpers.UnpackTripleKey(sibling_KeyPair, out sib_elementID, out sib_halfFacetID_parent, out sib_halfFacetID_child);
                        nE = _elements[sib_elementID];

                        if (!sibling_KeyPair.Equals(current_KeyPair))
                        {
                            int[] hfs_us;
                            nE.GetFirstLevelHalfFacet(sib_halfFacetID_parent, out hfs_us);

                            int eval = hfs_us.Length <= hf.Length ? hfs_us.Length : hf.Length;
                            if (hfs_us.Intersect(hf).Count() == eval)
                            {
                                e.SetSiblingHalfFacet(sibling_KeyPair, halfFacetID);
                            }
                        }
                    }
                }
            }
        }

        private void Build3DElementSiblingHalFacets(IElement e)
        {
            int elementID = e.Key;
            IElement nE;

            for (int hf_parent = 1; hf_parent <= e.HalfFacetsCount; hf_parent++)
            {
                //Adjacent vertices
                int[] hf_P;
                e.GetFirstLevelHalfFacet(hf_parent, out hf_P);

                for (int hf_child = 1; hf_child <= hf_P.Length; hf_child++)
                {
                    if (e.GetSiblingHalfFacet(hf_parent, hf_child) == 0)
                    {
                        int[] hf_C;
                        e.GetSecondLevelHalfFacet(hf_parent, hf_child, out hf_C);

                        //Find vertex with larger ID
                        int v = hf_C.Max();

                        long current_KeyPair = IHelpers.PackTripleKey(elementID, hf_parent, hf_child);

                        List<long> vertexSiblings = _tempVertexToHalfFacets[v];

                        foreach (long sibling_KeyTriple in vertexSiblings)
                        {
                            int sib_elementID, sib_halfFacetID_parent, sib_halfFacetID_child;
                            IHelpers.UnpackTripleKey(sibling_KeyTriple, out sib_elementID, out sib_halfFacetID_parent, out sib_halfFacetID_child);
                            nE = _elements[sib_elementID];

                            if (!sibling_KeyTriple.Equals(current_KeyPair))
                            {
                                int[] hfs_us;
                                nE.GetSecondLevelHalfFacet(sib_halfFacetID_parent, sib_halfFacetID_child, out hfs_us);

                                int eval = hfs_us.Length <= hf_C.Length ? hfs_us.Length : hf_C.Length;
                                if (hfs_us.Intersect(hf_C).Count() == eval)
                                {
                                    e.SetSiblingHalfFacet(sibling_KeyTriple, hf_parent, hf_child);
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
                foreach (int elementID in ElementsKeys)
                {
                    IElement e = GetElementWithKey(elementID);

                    if (e.TopologicDimension == 3)
                    {
                        //Give border V higher priorities
                        for (int halfFacet_parent = 1; halfFacet_parent <= e.HalfFacetsCount; halfFacet_parent++)
                        {
                            //Vertices of the facet
                            int[] hf_P;
                            e.GetFirstLevelHalfFacet(halfFacet_parent, out hf_P);

                            for (int halfFacet_child = 1; halfFacet_child <= hf_P.Length; halfFacet_child++)
                            {
                                int[] hf_C;
                                e.GetSecondLevelHalfFacet(halfFacet_parent, halfFacet_child, out hf_C);

                                foreach(int vK in hf_C) BuildVertexToHalfFacet(vK, elementID, halfFacet_parent, halfFacet_child);
                            }
                        }
                    }
                    else
                    {
                        //Give border V higher priorities
                        for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                        {
                            //Vertices of the facet
                            int[] hf;
                            e.GetFirstLevelHalfFacet(halfFacetID, out hf);

                            foreach (int vK in hf) BuildVertexToHalfFacet(vK, elementID, halfFacetID, 1);
                        }
                    }
                }
            }
            catch (Exception) { flag = false; }

            return flag;
        }

        private void BuildVertexToHalfFacet(int vKey, int elementID, int halfFacet_parent, int halfFacet_child)
        {
            ITopologicVertex v = GetVertexWithKey(vKey);

            if (v.V2HF == 0)
            {
                v.SetV2HF(elementID, halfFacet_parent, halfFacet_child);
                SetVertex(vKey, v);
            }

            if (_elements[elementID].GetSiblingHalfFacet(halfFacet_parent, halfFacet_child) == 0)
            {
                if (v.GetElementID() != elementID || v.GetParentHalfFacetID() != halfFacet_parent || v.GetChildHalfFacetID() != halfFacet_child)
                {
                    v.SetV2HF(elementID, halfFacet_parent, halfFacet_child);
                    SetVertex(vKey, v);
                }
            }
        }
    }
}
