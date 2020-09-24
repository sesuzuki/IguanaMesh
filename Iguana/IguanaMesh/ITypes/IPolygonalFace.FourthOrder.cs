using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IPolygonalFace : IElement
    {
        public class FourthOrder
        {
            /// <summary>
            /// 12-node fourth order incomplete triangle (3 nodes associated with the vertices (A,B,C) and 9 with the edges (ab1,ab2,ab3,bc1,bc2,bc3,ca1,ca2,ca3)) - Gmsh element Type: 22
            /// </summary>
            public class Triangle12 : IPolygonalFace
            {
                public Triangle12(int A, int B, int C, int ab1, int ab2, int ab3, int bc1, int bc2, int bc3, int ca1, int ca2, int ca3) : base(new int[] { A, ab1, ab2, ab3, B, bc1, bc2, bc3, C, ca1, ca2, ca3 }) { }
            }

            /// <summary>
            /// 15-node fourth order triangle (3 nodes associated with the vertices (A,B,C) and 9 with the edges (ab1,ab2,ab3,bc1,bc2,bc3,ca1,ca2,ca3) and 3 with the face (Fa1, Fa2, Fa3)) - Gmsh element Type: 23
            /// </summary>
            public class Triangle15 : IPolygonalFace
            {
                public int[] FaceNode { get; set; }
                public Triangle15(int A, int B, int C, int ab1, int ab2, int ab3, int bc1, int bc2, int bc3, int ca1, int ca2, int ca3, int Fa1, int Fa2, int Fa3) : base(new int[] { A, ab1, ab2, ab3, B, bc1, bc2, bc3, C, ca1, ca2, ca3 }) { FaceNode = new int[] { Fa1, Fa2, Fa3 }; }
            }
        }
    }
}
