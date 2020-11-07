using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IModifiers
{
    public static partial class ISubdividor
    {
        public static IVector3D ComputeAveragePosition(int[] keys, IMesh m)
        {
            IVector3D v = new IVector3D();
            for (int i = 0; i < keys.Length; i++) v += m.Vertices.GetVertexWithKey(keys[i]).Position;
            v /= keys.Length;
            return v;
        }

        public static Point3d ComputeAveragePoint(int[] keys, IMesh m)
        {
            IVector3D v = new IVector3D();
            for (int i = 0; i < keys.Length; i++) v += m.Vertices.GetVertexWithKey(keys[i]).Position;
            v /= keys.Length;
            return new Point3d(v.X,v.Y,v.Z);
        }

        public static IVector3D ComputeAveragePosition(IVector3D A, IVector3D B)
        {
            IVector3D v = A + B;
            v /= 2;
            return v;
        }
    }
}
