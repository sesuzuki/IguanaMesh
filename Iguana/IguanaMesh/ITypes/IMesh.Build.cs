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
                _valid = false;
            }
            else _valid = true;
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
            try
            {
                ElementsKeys.ForEach(elementID => BuildElementSiblingHalFacets(elementID));
                _tempVertexToHalfFacets.Clear();
                return true;
            }
            catch (Exception) { return false; }
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

                    //int v = hf.Max();
                    //List<Int64> vertexSiblings = _tempVertexToHalfFacets[v];

                    for (int i = 0; i < hf.Length; i++)
                    {
                        vertexSiblings = _tempVertexToHalfFacets[hf[i]];

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
                                if (hfs_us.Intersect(hf).Count() == eval)
                                {
                                    e.SetSiblingHalfFacet(halfFacetID, sibling_KeyPair);
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
    }
}
