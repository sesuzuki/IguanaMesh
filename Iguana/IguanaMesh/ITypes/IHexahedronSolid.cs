using System;

namespace Iguana.IguanaMesh.ITypes
{
    public class IHexahedronSolid : IElement
    {
        /// <summary>
        /// <para> General constructor for a hexahedron solid. </para>
        /// NOTE: Vertices on an AHF-IElement needs to be sorted according to the CFD General Notation System.\nSee: https://cgns.github.io/CGNS_docs_current/sids/conv.html
        /// <para><paramref name="vertices"/> : A collection of vertex identifiers. </para>
        /// </summary>
        ///
        public IHexahedronSolid(int[] vertices) : base(vertices, 6, 3) { }

        /// <summary>
        /// <para> Specific constructor for a hexahedron solid. </para>
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
        public IHexahedronSolid(int N1, int N2, int N3, int N4, int N5, int N6, int N7, int N8) : base(new int[] { N1, N2, N3, N4, N5, N6, N7, N8 }, 6, 3) { }

        /// <summary>
        /// <para> Element´s description . </para>
        /// </summary>
        ///
        public override string ToString()
        {
            string msg = "AHF-IHexahedronSolid{";
            for (int i = 0; i < VerticesCount; i++)
            {
                int idx = VerticesKeys[i];
                if (i < VerticesCount - 1) msg += idx + ";";
                else msg += idx + "}";
            }
            return msg;
        }

        public override bool GetHalfFacet(int index, out int[] halfFacets)
        {
            Boolean flag = true;
            halfFacets = null;

            switch(index){
                case 1:
                    halfFacets = new int[] { VerticesKeys[0], VerticesKeys[3], VerticesKeys[2], VerticesKeys[1] };
                    break;
                case 2:
                    halfFacets = new int[] { VerticesKeys[0], VerticesKeys[1], VerticesKeys[5], VerticesKeys[4] };
                    break;
                case 3:
                    halfFacets = new int[] { VerticesKeys[1], VerticesKeys[2], VerticesKeys[6], VerticesKeys[5] };
                    break;
                case 4:
                    halfFacets = new int[] { VerticesKeys[2], VerticesKeys[3], VerticesKeys[7], VerticesKeys[6] };
                    break;
                case 5:
                    halfFacets = new int[] { VerticesKeys[0], VerticesKeys[4], VerticesKeys[7], VerticesKeys[3] };
                    break;
                case 6:
                    halfFacets = new int[] { VerticesKeys[4], VerticesKeys[5], VerticesKeys[6], VerticesKeys[7] };
                    break;
                default:
                    flag = false;
                    break;
            }

            return flag;
        }

        public override bool AddVertex(int vertexKey)
        {
            throw new NotImplementedException();
        }

        public override bool RemoveVertex(int vertexKey)
        {
            throw new NotImplementedException();
        }
    }
}
