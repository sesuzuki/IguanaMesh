using Iguana.IguanaMesh.IUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes.IElements
{
    public partial class ISurfaceElement : IElement
    {
        public class HighOrder
        {
            /// <summary>
            /// 6-node second order triangle (3 nodes associated with the vertices (A,B,C) and 3 with the edges (ab,bc,ca))
            /// Element Type Reference: 9
            /// </summary>
            public class Triangle6 : ISurfaceElement
            {
                public Triangle6(int[] vertices) : base(new int[] {vertices[0], vertices[3], vertices[1], vertices[4], vertices[2], vertices[5] }) { }

                public override string ToString()
                {
                    string eType = "(6-Nodes) ITriangle 2nd-Order";
                    return IHelpers.HighOrder2DElementsToString(eType, Vertices);
                }

                public override int[] GetNodesForFastDrawing()
                {
                    return new int[] { Vertices[0], Vertices[2], Vertices[4] };
                }

                public override int[] GetNodesForDetailedDrawing()
                {
                    return Vertices;
                }
            }

            /// <summary>
            /// 8-node second order quadrangle(4 nodes associated with the vertices (A,B,C,D) and 4 with the edges (ab,bc,cd,da) )
            /// Element Type Reference: 16
            /// </summary>
            public class Quadrangle8 : ISurfaceElement
            {
                public Quadrangle8(int[] vertices) : base(new int[] { vertices[0], vertices[4], vertices[1], vertices[5], vertices[2], vertices[6], vertices[3], vertices[7]}) { }


                public override string ToString()
                {
                    string eType = "(8-Nodes) IQuadrangle 2nd-Order";
                    return IHelpers.HighOrder2DElementsToString(eType, Vertices);
                }

                public override int[] GetNodesForFastDrawing()
                {
                    return new int[] { Vertices[0], Vertices[2], Vertices[4], Vertices[6] };
                }

                public override int[] GetNodesForDetailedDrawing()
                {
                    return Vertices;
                }
            }

            /// <summary>
            /// 9-node third order incomplete triangle (3 nodes associated with the vertices (A,B,C) and 6 with the edges (ab1,ab2,bc1,bc2,ca1,ca2))
            /// Element Type Reference: 20
            /// </summary>
            public class Triangle9 : ISurfaceElement
            {
                public Triangle9(int[] vertices) 
                    : base(new int[] { vertices[0],vertices[3],vertices[4],vertices[1], vertices[5],vertices[6],vertices[2],vertices[7],vertices[8] }) { }

                public override string ToString()
                {
                    string eType = "(9-Nodes) ITriangle 3rd-Order";
                    return IHelpers.HighOrder2DElementsToString(eType, Vertices, 3);
                }

                public override int[] GetNodesForFastDrawing()
                {
                    return new int[] { Vertices[0], Vertices[3], Vertices[6] };
                }

                public override int[] GetNodesForDetailedDrawing()
                {
                    return Vertices;
                }
            }

            /// <summary>
            /// 12-node fourth order incomplete triangle (3 nodes associated with the vertices (A,B,C) and 9 with the edges (ab1,ab2,ab3,bc1,bc2,bc3,ca1,ca2,ca3))
            /// Element Type Reference: 22
            /// </summary>
            public class Triangle12 : ISurfaceElement
            {
                public Triangle12(int[] vertices) 
                    : base(new int[] { vertices[0], vertices[3], vertices[4], vertices[5], 
                                        vertices[1], vertices[6], vertices[7], vertices[8], 
                                        vertices[2], vertices[9], vertices[10], vertices[11] }) { }

                public override string ToString()
                {
                    string eType = "(12-Nodes) ITriangle 4th-Order";
                    return IHelpers.HighOrder2DElementsToString(eType, Vertices, 4);
                }

                public override int[] GetNodesForFastDrawing()
                {
                    return new int[] { Vertices[0], Vertices[4], Vertices[8] };
                }

                public override int[] GetNodesForDetailedDrawing()
                {
                    return Vertices;
                }
            }

            /// <summary>
            /// 15-node fifth order incomplete triangle (3 nodes associated with the vertices (A,B,C) and 12 with the edges (ab1,ab2,ab3,ab4,bc1,bc2,bc3,bc4,ca1,ca2,ca3,ca4)) 
            /// Element Type Reference: 24
            /// </summary>
            public class Triangle15 : ISurfaceElement
            {
                public Triangle15(int[] vertices)
                    : base(new int[] { vertices[0], vertices[3], vertices[4], vertices[5], vertices[6],
                                        vertices[1], vertices[7], vertices[8], vertices[9], vertices[10],
                                        vertices[2], vertices[11], vertices[12], vertices[13], vertices[14] })
                { }

                public override string ToString()
                {
                    string eType = "(15-Nodes) ITriangle 5th-Order";
                    return IHelpers.HighOrder2DElementsToString(eType, Vertices, 5);
                }

                public override int[] GetNodesForFastDrawing()
                {
                    return new int[] { Vertices[0], Vertices[5], Vertices[10] };
                }

                public override int[] GetNodesForDetailedDrawing()
                {
                    return Vertices;
                }

            }
        }
    }
}
