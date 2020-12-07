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
            public class ITriangle6 : ISurfaceElement
            {
                public ITriangle6(int[] vertices) : base(new int[] {vertices[0], vertices[3], vertices[1], vertices[4], vertices[2], vertices[5] }) { SetElementType(9); }

                public override IElement CleanCopy()
                {
                    IElement e = new ITriangle6(Vertices);
                    e.Key = Key;
                    return e;
                }

                public override string ToString()
                {
                    string eType = "(6-Nodes)-ITriangle-2nd-Order";
                    return IHelpers.HighOrder2DElementsToString(eType, Vertices);
                }

                public override int[] GetGmshFormattedVertices()
                {
                    return new int[] { Vertices[0], Vertices[2], Vertices[4], Vertices[1], Vertices[3], Vertices[5] };
                }
            }

            /// <summary>
            /// 8-node second order quadrangle(4 nodes associated with the vertices (A,B,C,D) and 4 with the edges (ab,bc,cd,da) )
            /// Element Type Reference: 16
            /// </summary>
            public class IQuadrangle8 : ISurfaceElement
            {
                public IQuadrangle8(int[] vertices) : base(new int[] { vertices[0], vertices[4], vertices[1], vertices[5], vertices[2], vertices[6], vertices[3], vertices[7]} ) { SetElementType(16); }

                public override IElement CleanCopy()
                {
                    IElement e = new IQuadrangle8(Vertices);
                    e.Key = Key;
                    return e;
                }

                public override string ToString()
                {
                    string eType = "(8-Nodes)-IQuadrangle-2nd-Order";
                    return IHelpers.HighOrder2DElementsToString(eType, Vertices);
                }

                public override int[] GetGmshFormattedVertices()
                {
                    return new int[] { Vertices[0], Vertices[2], Vertices[4], Vertices[6], Vertices[1], Vertices[3], Vertices[5], Vertices[7] };
                }
            }

            /// <summary>
            /// 9-node second order quadrangle(4 nodes associated with the vertices (A,B,C,D), 4 with the edges (ab,bc,cd,da) and 1 with face )
            /// Element Type Reference: 10
            /// </summary>
            public class IQuadrangle9 : ISurfaceElement
            {
                public int FaceNode { get; set; }
                public IQuadrangle9(int[] vertices) : base(new int[] { vertices[0], vertices[4], vertices[1], vertices[5], vertices[2], vertices[6], vertices[3], vertices[7] }) 
                {
                    FaceNode = vertices[8];
                    SetElementType(10); 
                }

                public override IElement CleanCopy()
                {
                    IElement e = new IQuadrangle9(Vertices);
                    e.Key = Key;
                    return e;
                }

                public override string ToString()
                {
                    string eBase = "(9-Nodes)-IQuadrangle-2nd-Order";
                    string eType = IHelpers.HighOrder2DElementsToString(eBase, Vertices);
                    eType += " || Face-Node{ " + FaceNode + " }";
                    return eType;
                }

                public override int[] GetGmshFormattedVertices()
                {
                    return new int[] { Vertices[0], Vertices[2], Vertices[4], Vertices[6], Vertices[1], Vertices[3], Vertices[5], Vertices[7] };
                }
            }

            /// <summary>
            /// 9-node third order incomplete triangle (3 nodes associated with the vertices (A,B,C) and 6 with the edges (ab1,ab2,bc1,bc2,ca1,ca2))
            /// Element Type Reference: 20
            /// </summary>
            public class ITriangle9 : ISurfaceElement
            {
                public ITriangle9(int[] vertices) 
                    : base(new int[] { vertices[0],vertices[3],vertices[4],vertices[1], vertices[5],vertices[6],vertices[2],vertices[7],vertices[8] }) { SetElementType(20); }

                public override IElement CleanCopy()
                {
                    IElement e = new ITriangle9(Vertices);
                    e.Key = Key;
                    return e;
                }

                public override string ToString()
                {
                    string eType = "(9-Nodes)-ITriangle-3rd-Order";
                    return IHelpers.HighOrder2DElementsToString(eType, Vertices, 3);
                }

                public override int[] GetGmshFormattedVertices()
                {
                    return new int[] { Vertices[0], Vertices[3], Vertices[6], Vertices[1], Vertices[2], Vertices[4], Vertices[5], Vertices[7], Vertices[8] };
                }
            }

            /// <summary>
            /// 12-node fourth order incomplete triangle (3 nodes associated with the vertices (A,B,C) and 9 with the edges (ab1,ab2,ab3,bc1,bc2,bc3,ca1,ca2,ca3))
            /// Element Type Reference: 22
            /// </summary>
            public class ITriangle12 : ISurfaceElement
            {
                public ITriangle12(int[] vertices) 
                    : base(new int[] { vertices[0], vertices[3], vertices[4], vertices[5], 
                                        vertices[1], vertices[6], vertices[7], vertices[8], 
                                        vertices[2], vertices[9], vertices[10], vertices[11] }) { SetElementType(22); }

                public override IElement CleanCopy()
                {
                    IElement e = new ITriangle12(Vertices);
                    e.Key = Key;
                    return e;
                }

                public override string ToString()
                {
                    string eType = "(12-Nodes)-ITriangle-4th-Order";
                    return IHelpers.HighOrder2DElementsToString(eType, Vertices, 4);
                }

                public override int[] GetGmshFormattedVertices()
                {
                    return new int[] { Vertices[0], Vertices[4], Vertices[8], Vertices[1],
                                         Vertices[2], Vertices[3], Vertices[5], Vertices[6],
                                            Vertices[7], Vertices[9], Vertices[10], Vertices[11],};
                }
            }

            /// <summary>
            /// 15-node fifth order incomplete triangle (3 nodes associated with the vertices (A,B,C) and 12 with the edges (ab1,ab2,ab3,ab4,bc1,bc2,bc3,bc4,ca1,ca2,ca3,ca4)) 
            /// Element Type Reference: 24
            /// </summary>
            public class ITriangle15 : ISurfaceElement
            {
                public ITriangle15(int[] vertices)
                    : base(new int[] { vertices[0], vertices[3], vertices[4], vertices[5], vertices[6],
                                        vertices[1], vertices[7], vertices[8], vertices[9], vertices[10],
                                        vertices[2], vertices[11], vertices[12], vertices[13], vertices[14] })
                { SetElementType(24); }

                public override IElement CleanCopy()
                {
                    IElement e = new ITriangle15(Vertices);
                    e.Key = Key;
                    return e;
                }

                public override string ToString()
                {
                    string eType = "(15-Nodes)-ITriangle-5th-Order";
                    return IHelpers.HighOrder2DElementsToString(eType, Vertices, 5);
                }

                public override int[] GetGmshFormattedVertices()
                {
                    return new int[] { Vertices[0], Vertices[5], Vertices[10], Vertices[1], Vertices[2],
                                        Vertices[3], Vertices[4], Vertices[6], Vertices[7], Vertices[8],
                                        Vertices[9], Vertices[11], Vertices[12], Vertices[13], Vertices[14],};
                }
            }
        }
    }
}
