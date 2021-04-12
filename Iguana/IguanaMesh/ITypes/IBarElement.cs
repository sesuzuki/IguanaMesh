using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public class IBarElement : IElement
    {
        public IBarElement(int[] vertices) : base(vertices, 2, 1, 1){}

        public IBarElement(int A, int B) : base(new[] { A,B }, 2, 1, 1){}

        public override bool AddVertex(int vertexKey)
        {
            return false;
        }

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
            string msg = "IBar{" + Vertices[0] + ";" + Vertices[1] + "}";
            return msg;
        }

        public override int[] GetGmshFormattedVertices()
        {
            throw new NotImplementedException();
        }

        public override bool GetHalfFacet(int index, out int[] halfFacets)
        {
            halfFacets = null;
            if (index > 0 && index <= HalfFacetsCount)
            {
                if (index == 1)
                {
                    halfFacets = new int[] { Vertices[0] };
                }
                else
                {
                    halfFacets = new int[] { Vertices[1] };
                }
                return true;
            }
            else return false;
        }

        public override bool GetHalfFacetWithPrincipalNodesOnly(int index, out int[] halfFacets)
        {
            return GetHalfFacet(index, out halfFacets);
        }

        public override bool RemoveVertex(int vertexKey)
        {
            return false;
        }
    }
}
