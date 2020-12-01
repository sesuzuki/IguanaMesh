using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iguana.IguanaMesh.IUtils;

namespace Iguana.IguanaMesh.ITypes.IElements
{
    public partial class IHexahedronElement : IElement
    {
        public class HighOrder
        {
            /// <summary>
            /// 20-node second order hexahedron(8 nodes associated with the vertices and 12 with the edges). 
            /// Element Type Reference: 17
            /// </summary>
            public class IHexahedron20 : IHexahedronElement
            {
                public IHexahedron20(int[] vertices) : base(vertices) { this.SetElementType(17); }

                public override IElement CleanCopy()
                {
                    IElement e  = new IHexahedron20(Vertices);
                    e.Key = Key;
                    return e;
                }

                public override string ToString()
                {
                    string eType = "(20-Nodes)-IHexahedron-2nd-Order";
                    return IHelpers.HighOrder3DElementsToString(eType, Vertices, 8);
                }

                public override bool GetHalfFacet(int index, out int[] halfFacets)
                {
                    Boolean flag = true;
                    halfFacets = null;

                    switch (index)
                    {
                        case 1:
                            halfFacets = new int[] { Vertices[0], Vertices[8], Vertices[1], Vertices[12], Vertices[5], Vertices[16], Vertices[4], Vertices[10] };
                            break;
                        case 2:
                            halfFacets = new int[] { Vertices[3], Vertices[15], Vertices[7], Vertices[19], Vertices[6], Vertices[14], Vertices[2], Vertices[13] };
                            break;
                        case 3:
                            halfFacets = new int[] { Vertices[0], Vertices[10], Vertices[4], Vertices[17], Vertices[7], Vertices[15], Vertices[3], Vertices[9] };
                            break;
                        case 4:
                            halfFacets = new int[] { Vertices[4], Vertices[16], Vertices[5], Vertices[18], Vertices[6], Vertices[19], Vertices[7], Vertices[17] };
                            break;
                        case 5:
                            halfFacets = new int[] { Vertices[5], Vertices[12], Vertices[1], Vertices[11], Vertices[2], Vertices[14], Vertices[6], Vertices[18] };
                            break;
                        case 6:
                            halfFacets = new int[] { Vertices[1], Vertices[8], Vertices[0], Vertices[9], Vertices[3], Vertices[13], Vertices[2], Vertices[11] };
                            break;
                        default:
                            flag = false;
                            break;
                    }

                    return flag;
                }

                public override bool GetHalfFacetWithPrincipalNodesOnly(int index, out int[] halfFacets)
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
                            halfFacets = new int[] { Vertices[1], Vertices[8], Vertices[0], Vertices[9], Vertices[3], Vertices[13], Vertices[2], Vertices[11] };
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
