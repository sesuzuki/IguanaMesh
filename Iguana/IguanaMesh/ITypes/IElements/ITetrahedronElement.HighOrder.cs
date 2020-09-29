using Iguana.IguanaMesh.IUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes.IElements
{
    public partial class ITetrahedronElement : IElement
    {
        public class HighOrder
        {
            public class ITetrahedron10 : ITetrahedronElement
            {
                /// <summary>
                /// 10-node second order tetrahedron(4 nodes associated with the vertices and 6 with the edges).
                /// Element Type Reference: 11
                /// NOTE: Vertices on an AHF-IElement needs to be sorted according to the CFD General Notation System.\nSee: https://cgns.github.io/CGNS_docs_current/sids/conv.html
                /// </summary>
                public ITetrahedron10(int[] vertices) : base(vertices) { }

                /// <summary>
                /// <para> Element´s description . </para>
                /// </summary>
                public override string ToString()
                {
                    string eType = "(10-Nodes) ITetrahedron 2nd-Order";
                    return IHelpers.HighOrder3DElementsToString(eType, Vertices, 4);
                }

                public override bool GetHalfFacet(int index, out int[] halfFacets)
                {
                    Boolean flag = true;
                    halfFacets = null;

                    switch (index)
                    {
                        case 1:
                            halfFacets = new int[] { Vertices[0], Vertices[7], Vertices[3], Vertices[9], Vertices[1], Vertices[4] };
                            break;
                        case 2:
                            halfFacets = new int[] { Vertices[3], Vertices[9], Vertices[1], Vertices[5], Vertices[2], Vertices[8] };
                            break;
                        case 3:
                            halfFacets = new int[] { Vertices[1], Vertices[4], Vertices[0], Vertices[6], Vertices[2], Vertices[5] };
                            break;
                        case 4:
                            halfFacets = new int[] { Vertices[0], Vertices[7], Vertices[3], Vertices[8], Vertices[2], Vertices[6] };
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
                    Boolean flag = true;
                    halfFacets = null;

                    switch (index)
                    {
                        case 1:
                            halfFacets = new int[] { Vertices[0], Vertices[3], Vertices[1] };
                            break;
                        case 2:
                            halfFacets = new int[] { Vertices[3], Vertices[1], Vertices[2] };
                            break;
                        case 3:
                            halfFacets = new int[] { Vertices[1], Vertices[0], Vertices[2] };
                            break;
                        case 4:
                            halfFacets = new int[] { Vertices[0], Vertices[3], Vertices[2] };
                            break;
                        default:
                            flag = false;
                            break;
                    }

                    return flag;
                }
            }
        }
    }
}
