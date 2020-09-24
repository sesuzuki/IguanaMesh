using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IPolygonalFace : IElement
    {
        public class ThirdOrder
        {
            /// <summary>
            /// 9-node third order incomplete triangle (3 nodes associated with the vertices (A,B,C) and 6 with the edges (ab1,ab2,bc1,bc2,ca1,ca2)) - Gmsh element Type: 20
            /// </summary>
            public class Triangle9 : IPolygonalFace
            {
                public Triangle9(int A, int B, int C, int ab1, int ab2, int bc1, int bc2, int ca1, int ca2) : base(new int[] { A, ab1, ab2, B, bc1, bc2, C, ca1, ca2 }) { }
            }

            /// <summary>
            /// 10-node third order incomplete triangle (3 nodes associated with the vertices (A,B,C) and 6 with the edges (ab1,ab2,bc1,bc2,ca1,ca2) and 1 with the face (Fa)) - Gmsh element Type: 21
            /// </summary>
            public class Triangle10 : IPolygonalFace
            {
                public int FaceNode { get; set; }
                public Triangle10(int A, int B, int C, int ab1, int ab2, int bc1, int bc2, int ca1, int ca2, int Fa) : base(new int[] { A, ab1, ab2, B, bc1, bc2, C, ca1, ca2 }) { FaceNode = Fa;  }
            }
        }
    }
}
