using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IPolygonalFace : IElement
    {
        public class FirstOrder
        {
            /// <summary>
            /// 3-node triangle - Gmsh element Type: 2
            /// </summary>
            public class Triangle : IPolygonalFace
            {
                public Triangle(int A, int B, int C) : base(new int[] { A, B, C }) { }
            }

            /// <summary>
            /// 4-node quadrangle. - Gmsh element Type: 3
            /// </summary>
            public class Quadrangle : IPolygonalFace
            {
                public Quadrangle(int A, int B, int C, int D) : base(new int[] { A, B, C, D }) { }
            }
        }
    }
}
