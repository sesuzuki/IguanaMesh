using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public class IBarElement : IElement
    {


        /// <summary>
        /// <para> General constructor for a bar. </para>
        /// Element Type Reference: -1 for bars.
        /// <para><paramref name="vertices"/> : A collection of vertex identifiers. </para>
        /// </summary>
        ///
        public IBarElement(int[] vertices) : base(vertices, 2, 1, -1) { }

        public IBarElement(int A, int B) : base(new int[] { A, B }, 2, 1, -1) { }

        public override IElement CleanCopy()
        {
            IElement e = new IBarElement(Vertices);
            e.Key = Key;
            return e;
        }

        /// <summary>
        /// <para> Element´s description . </para>
        /// </summary>
        ///
        public override string ToString()
        {
            string msg = "IBar{";
            for (int i = 0; i < 2; i++)
            {
                int idx = Vertices[i];
                if (i < VerticesCount - 1) msg += idx + ";";
                else msg += idx + "}";
            }
            return msg;
        }

        /// <summary>
        /// <para> Returns a Half-Facet of type Half-Vertex. A Half-Vertex is the zero-dimensional sub-entity of an edge. </para>
        /// <paramref name="index"/> : The local index of the Half-Facet to search within the element.
        /// <paramref name="halfFacet"/> : A sub-list to store the vertex identifiers representing the Half-Facet.
        /// </summary>
        ///
        public override bool GetHalfFacet(int index, out int[] halfFacet)
        {
            halfFacet = null;
            if (index > 0 && index <= HalfFacetsCount)
            {
                if (index == 1)
                {
                    halfFacet = new int[] { Vertices[0] };
                }
                else
                {
                    halfFacet = new int[] { Vertices[1] };
                }
                return true;
            }
            else return false;
        }

        public override bool AddVertex(int vertexKey)
        {
            return false;
        }

        public override bool RemoveVertex(int vertexKey)
        {
            return false;
        }

        public override bool GetHalfFacetWithPrincipalNodesOnly(int index, out int[] halfFacets)
        {
            return GetHalfFacet(index, out halfFacets);
        }

        public override int[] GetGmshFormattedVertices()
        {
            return Vertices;
        }
    }
}
