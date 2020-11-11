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
        public static IPoint3D ComputeAveragePosition(int[] keys, IMesh m)
        {
            IVector3D v = new IVector3D();
            for (int i = 0; i < keys.Length; i++) v += m.GetVertexWithKey(keys[i]).Position;
            v /= keys.Length;
            return new IPoint3D(v.X,v.Y,v.Z);
        }

        public static Point3d ComputeAveragePoint(int[] keys, IMesh m)
        {
            IVector3D v = new IVector3D();
            for (int i = 0; i < keys.Length; i++) v += m.GetVertexWithKey(keys[i]).Position;
            v /= keys.Length;
            return new Point3d(v.X,v.Y,v.Z);
        }

        public static IPoint3D ComputeAveragePosition(IPoint3D A, IPoint3D B)
        {
            return new IPoint3D((A.X+B.X)/2, (A.Y + B.Y) / 2, (A.Z + B.Z) / 2);
        }
    }
}
