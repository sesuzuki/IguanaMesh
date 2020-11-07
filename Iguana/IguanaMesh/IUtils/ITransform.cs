using Iguana.IguanaMesh.ITypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IUtils
{
    public static class ITransform
    {
        public static IMesh Scale(IMesh mesh, double sX, double sY, double sZ)
        {
            IMesh dup = mesh.DeepCopy();

            Parallel.ForEach(mesh.Vertices.VerticesKeys, vK =>
            {
                ITopologicVertex v = mesh.Vertices.GetVertexWithKey(vK);
                v.X *= sX;
                v.Y *= sY;
                v.Z *= sZ;
                dup.Vertices.SetVertexPosition(vK, v.Position);
            });

            return dup;
        }

        public static IMesh Move(IMesh mesh, double sX, double sY, double sZ)
        {
            IMesh dup = mesh.DeepCopy();

            Parallel.ForEach(mesh.Vertices.VerticesKeys, vK =>
            {
                ITopologicVertex v = mesh.Vertices.GetVertexWithKey(vK);
                v.X += sX;
                v.Y += sY;
                v.Z += sZ;
                dup.Vertices.SetVertexPosition(vK, v.Position);
            });

            return dup;
        }

        public static IMesh Rotate(IMesh mesh, IVector3D axis, double angle)
        {
            IMesh dup = mesh.DeepCopy();

            Parallel.ForEach(mesh.Vertices.VerticesKeys, vK =>
            {
                ITopologicVertex v = mesh.Vertices.GetVertexWithKey(vK);
                IVector3D pos = RotateVertex(v,axis, angle);
                dup.Vertices.SetVertexPosition(vK, pos);
            });

            return dup;
        }

        internal static IVector3D RotateVertex(ITopologicVertex v, IVector3D axis, double angle)
        {
            axis.Norm();
            IVector3D nV = new IVector3D(v.Position);
            double a = angle * Math.PI / 180;
            double t1 = 1 - Math.Cos(a);
            double cosA = Math.Cos(a);
            double sinA = Math.Sin(a);
            double x2 = Math.Pow(axis.X, 2);
            double y2 = Math.Pow(axis.Y, 2);
            double z2 = Math.Pow(axis.Z, 2);
            double xy = axis.X * axis.Y;
            double yz = axis.Y * axis.Z;
            double xz = axis.X * axis.Z;
            nV.X += (cosA + x2 * t1) + (xy * t1 - axis.Z * sinA) + (xz * t1 + axis.Y * sinA);
            nV.Y += (xy * t1 + axis.Z * sinA) + (cosA + y2 * t1) + (yz * t1 - axis.X * sinA);
            nV.Z += (xz * t1 - axis.Y * sinA) + (yz * t1 + axis.X * sinA) + (cosA + z2 * t1);
            return nV;
        }
    }
}
