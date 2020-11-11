using Iguana.IguanaMesh.IUtils;
using Rhino.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes.IElements
{
    public partial class IPrismElement : IElement
    {
        public class HighOrder
        {
            public class IPrism15 : IPrismElement
            {
                /// <summary>
                /// Generic constructor for a 15-node second order prism(6 nodes associated with the vertices and 9 with the edges).
                /// Element Type Reference: 18
                /// NOTE: Vertices on an AHF-IElement needs to be sorted according to the CFD General Notation System.\nSee: https://cgns.github.io/CGNS_docs_current/sids/conv.html
                /// </summary>
                ///
                public IPrism15(int[] vertices) : base(vertices) { SetElementType(18); }

                public override IElement CleanCopy()
                {
                    IElement e = new IPrism15(Vertices);
                    e.Key = Key;
                    return e;
                }

                /// <summary>
                /// <para> Element´s description . </para>
                /// </summary>
                public override string ToString()
                {
                    string eType = "(15-Nodes) IPrism 2nd-Order";
                    return IHelpers.HighOrder3DElementsToString(eType, Vertices, 6);
                }

                public override bool GetHalfFacet(int index, out int[] halfFacets)
                {
                    Boolean flag = true;
                    halfFacets = null;

                    switch (index)
                    {
                        case 1:
                            halfFacets = new int[] { Vertices[0], Vertices[7], Vertices[2], Vertices[11], Vertices[5], Vertices[13], Vertices[3], Vertices[8] };
                            break;
                        case 2:
                            halfFacets = new int[] { Vertices[2], Vertices[9], Vertices[1], Vertices[10], Vertices[4], Vertices[14], Vertices[5], Vertices[11] };
                            break;
                        case 3:
                            halfFacets = new int[] { Vertices[1], Vertices[6], Vertices[0], Vertices[8], Vertices[3], Vertices[12], Vertices[4], Vertices[10] };
                            break;
                        case 4:
                            halfFacets = new int[] { Vertices[0], Vertices[7], Vertices[2], Vertices[9], Vertices[1], Vertices[6] };
                            break;
                        case 5:
                            halfFacets = new int[] { Vertices[3], Vertices[13], Vertices[5], Vertices[14], Vertices[4], Vertices[12] };
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
                            halfFacets = new int[] { Vertices[0], Vertices[2], Vertices[5], Vertices[3] };
                            break;
                        case 2:
                            halfFacets = new int[] { Vertices[2], Vertices[1], Vertices[4], Vertices[5] };
                            break;
                        case 3:
                            halfFacets = new int[] { Vertices[1], Vertices[0], Vertices[3], Vertices[4] };
                            break;
                        case 4:
                            halfFacets = new int[] { Vertices[0], Vertices[2], Vertices[1] };
                            break;
                        case 5:
                            halfFacets = new int[] { Vertices[3], Vertices[5], Vertices[4] };
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
