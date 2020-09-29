using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes.IElements
{
    public partial class IPyramidElement : IElement
    {
        /// <summary>
        /// Generic constructor for a 5-node pyramid element.
        /// Element Type Reference: 7
        /// NOTE: Vertices on an AHF-IElement needs to be sorted according to the CFD General Notation System.\nSee: https://cgns.github.io/CGNS_docs_current/sids/conv.html
        /// <para><paramref name="vertices"/> : List of vertices. </para>
        /// </summary>
        public IPyramidElement(int[] vertices) : base(vertices, 5, 3) { }


        /// <summary>
        /// Generic constructor for a 5-node pyramid element.
        /// Element Type Reference: 7
        /// NOTE: Vertices on an AHF-IElement needs to be sorted according to the CFD General Notation System.\nSee: https://cgns.github.io/CGNS_docs_current/sids/conv.html
        /// <para><paramref name="N1"/> : First vertex identifier. </para>
        /// <para><paramref name="N2"/> : Second vertex identifier. </para>
        /// <para><paramref name="N3"/> : Third vertex identifier. </para>
        /// <para><paramref name="N4"/> : Fourth vertex identifier. </para>
        /// <para><paramref name="N5"/> : Fifth vertex identifier. </para>
        /// </summary>
        ///
        public IPyramidElement(int N1, int N2, int N3, int N4, int N5) : base(new int[] { N1, N2, N3, N4, N5 }, 5, 3) { }

        /// <summary>
        /// <para> Element´s description . </para>
        /// </summary>
        public override string ToString()
        {
            string msg = "IPyramidElement{";
            for (int i = 0; i < VerticesCount; i++)
            {
                int idx = Vertices[i];
                if (i < VerticesCount - 1) msg += idx + ";";
                else msg += idx + "}";
            }
            return msg;
        }

        public override bool GetHalfFacet(int index, out int[] halfFacets)
        {
            Boolean flag = true;
            halfFacets = null;

            switch (index)
            {
                case 1:
                    halfFacets = new int[] { Vertices[0], Vertices[1], Vertices[2], Vertices[3] };
                    break;
                case 2:
                    halfFacets = new int[] { Vertices[0], Vertices[1], Vertices[4] };
                    break;
                case 3:
                    halfFacets = new int[] { Vertices[1], Vertices[2], Vertices[4] };
                    break;
                case 4:
                    halfFacets = new int[] { Vertices[2], Vertices[3], Vertices[4] };
                    break;
                case 5:
                    halfFacets = new int[] { Vertices[3], Vertices[0], Vertices[4] };
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

        public override bool GetHalfFacetWithPrincipalNodesOnly(int index, out int[] halfFacets)
        {
            return GetHalfFacet(index, out halfFacets);
        }
    }
}
