using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IPolygonalFace : IElement
    {
        public class SecondOrder
        {
            /// <summary>
            /// 6-node second order triangle (3 nodes associated with the vertices (A,B,C) and 3 with the edges (ab,bc,ca)) - Gmsh element Type: 9
            /// </summary>
            public class Triangle6 : IPolygonalFace
            {
                public Triangle6(int A, int B, int C, int ab, int bc, int ca) : base(new int[] { A, ab, B, bc, C, ca }) { }
            }

            /// <summary>
            /// 8-node second order quadrangle(4 nodes associated with the vertices (A,B,C,D) and 4 with the edges (ab,bc,cd,da) ) - Gmsh element Type: 16
            /// </summary>
            public class Quadrangle8 : IPolygonalFace
            {
                public Quadrangle8(int A, int B, int C, int D, int ab, int bc, int cd, int da) : base(new int[] { A, ab, B, bc, C, cd, D, da }) { }
            }

            /// <summary>
            /// 9-node second order quadrangle(4 nodes associated with the vertices (A,B,C,D) and 4 with the edges (ab,bc,cd,da) and 1 with the face (Fa) ) - Gmsh element Type: 10
            /// </summary>
            public class Quadrangle9 : IPolygonalFace
            {
                public int FaceNode { get; set; }
                public Quadrangle9(int A, int B, int C, int D, int ab, int bc, int cd, int da, int Fa) : base(new int[] { A, ab, B, bc, C, cd, D, da }) { FaceNode = Fa; }
            }
        }
    }
}
