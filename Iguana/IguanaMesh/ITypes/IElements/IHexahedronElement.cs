using Rhino.Geometry;
using System;

namespace Iguana.IguanaMesh.ITypes.IElements
{
    public partial class IHexahedronElement : IElement
    {
        /// <summary>
        /// Generic constructor for an 8-node hexahedron element.
        /// Element Type Reference: 5
        /// NOTE: Vertices on an AHF-IElement needs to be sorted according to the CFD General Notation System.\nSee: https://cgns.github.io/CGNS_docs_current/sids/conv.html
        /// <para><paramref name="vertices"/> : List of vertices. </para>
        /// </summary>
        public IHexahedronElement(int[] vertices) : base(vertices, 6, 3, 5) 
        {
            for (int i = 0; i < HalfFacetsCount; i++)
            {
                _siblingHalfFacets[i] = new long[4];
                _visits[i] = new bool[4];
            }
        }

        /// <summary>
        /// Generic constructor for an 8-node hexahedron element.
        /// Element Type Reference: 5
        /// NOTE: Vertices on an AHF-IElement needs to be sorted according to the CFD General Notation System.\nSee: https://cgns.github.io/CGNS_docs_current/sids/conv.html
        /// <para><paramref name="N1"/> : First vertex identifier. </para>
        /// <para><paramref name="N2"/> : Second vertex identifier. </para>
        /// <para><paramref name="N3"/> : Third vertex identifier. </para>
        /// <para><paramref name="N4"/> : Fourth vertex identifier. </para>
        /// <para><paramref name="N5"/> : Fifth vertex identifier. </para>
        /// <para><paramref name="N6"/> : Sixth vertex identifier. </para>
        /// <para><paramref name="N7"/> : Seventh vertex identifier. </para>
        /// <para><paramref name="N8"/> : Eighth vertex identifier. </para>
        /// </summary>
        ///
        public IHexahedronElement(int N1, int N2, int N3, int N4, int N5, int N6, int N7, int N8) : base(new int[] { N1, N2, N3, N4, N5, N6, N7, N8 }, 6, 3, 5) 
        {
            for (int i = 0; i < HalfFacetsCount; i++)
            {
                _siblingHalfFacets[i] = new long[4];
                _visits[i] = new bool[4];
            }
        }

        public override void CleanTopologicalData()
        {
            for (int i = 0; i < HalfFacetsCount; i++)
            {
                _siblingHalfFacets[i] = new long[4];
                _visits[i] = new bool[4];
            }
        }

        public override IElement CleanCopy()
        {
            IElement e = new IHexahedronElement(Vertices);
            e.Key = Key;
            return e;
        }

        /// <summary>
        /// <para> Element´s description . </para>
        /// </summary>
        ///
        public override string ToString()
        {
            string msg = "IHexahedronElement{";
            for (int i = 0; i < VerticesCount; i++)
            {
                int idx = Vertices[i];
                if (i < VerticesCount - 1) msg += idx + ";";
                else msg += idx + "}";
            }
            return msg;
        }

        public override bool GetFirstLevelHalfFacet(int index, out int[] halfFacets)
        {
            Boolean flag = true;
            halfFacets = null;

            switch (index)
            {
                case 1:
                    halfFacets = new int[] { Vertices[0], Vertices[1], Vertices[5], Vertices[4] };
                    break;
                case 2:
                    halfFacets = new int[] { Vertices[3], Vertices[7], Vertices[6], Vertices[2] };
                    break;
                case 3:
                    halfFacets = new int[] { Vertices[0], Vertices[4], Vertices[7], Vertices[3] };
                    break;
                case 4:
                    halfFacets = new int[] { Vertices[4], Vertices[5], Vertices[6], Vertices[7] };
                    break;
                case 5:
                    halfFacets = new int[] { Vertices[5], Vertices[1], Vertices[2], Vertices[6] };
                    break;
                case 6:
                    halfFacets = new int[] { Vertices[1], Vertices[0], Vertices[3], Vertices[2] };
                    break;
                default:
                    flag = false;
                    break;
            }

            return flag;
        }

        public override bool GetHalfFacetWithPrincipalNodesOnly(int index, out int[] halfFacets)
        {
            return GetFirstLevelHalfFacet(index, out halfFacets);
        }

        public override int[] GetGmshFormattedVertices()
        {
            return Vertices;
        }
    }
}
