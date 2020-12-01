using Iguana.IguanaMesh.ITypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IModifiers
{
    public class ITransformX
    {
        public IMatrix TransformationMatrix { get; set; }

        public ITransformX()
        {
            TransformationMatrix = IMatrix.Identity4x4Matrix;
        }

        public void Translate(double x, double y, double z, double factor=1.0)
        {
            double[][] data = new double[4][];
            data[0] = new double[]{ 1, 0, 0, factor* x};
            data[1] = new double[]{ 0, 1, 0, factor* y};
            data[2] = new double[]{ 0, 0, 1, factor* z};
            data[3] = new double[]{ 0, 0, 0, 1 };

            IMatrix m = new IMatrix(data);
            TransformationMatrix = IMatrix.Mult(TransformationMatrix, m);
        }

        /// <summary>
        /// Rotate around an axis. Angle in degrees.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="axis"></param>
        public void RotateAboutOrigin(double angle, IVector3D axis)
        {
            axis.Norm();
            double a = angle * Math.PI / 180;
            double cosA = Math.Cos(a);
            double sinA = Math.Sin(a);
            double xx = Math.Pow(axis.X, 2);
            double yy = Math.Pow(axis.Y, 2);
            double zz = Math.Pow(axis.Z, 2);
            double xy = axis.X * axis.Y;
            double yz = axis.Y * axis.Z;
            double xz = axis.X * axis.Z;

            double[][] data = new double[4][];
            data[0] = new double[] { xx + (1 - xx) * cosA, xy * (1 - cosA) - axis.Z * sinA, xz * (1 - cosA) + axis.Y * sinA, 0 };
            data[1] = new double[] { xy * (1 - cosA) + axis.Z * sinA, yy + (1 - yy) * cosA, yz * (1 - cosA) - axis.X * sinA, 0 };
            data[2] = new double[] { xz * (1 - cosA) - axis.Y * sinA, yz * (1 - cosA) + axis.X * sinA, zz + (1 - zz) * cosA, 0 };
            data[3] = new double[] { 0, 0, 0, 1 };
            IMatrix m = new IMatrix(data);
            TransformationMatrix = IMatrix.Mult(TransformationMatrix,m);
        }

        public void RotateAboutAxis(double angle, IPoint3D origin, IVector3D axis)
        {
            Translate(origin.X, origin.Y, origin.Z);
            RotateAboutOrigin(angle, axis);
            Translate(origin.X, origin.Y, origin.Z,-1);
        }

        public void Scale(double x, double y, double z)
        {
            double[][] data = new double[4][];
            data[0] = new double[] { x, 0, 0, 0 };
            data[1] = new double[] { 0, y, 0, 0 };
            data[2] = new double[] { 0, 0, z, 0 };
            data[3] = new double[] { 0, 0, 0, 1 };
            IMatrix m = new IMatrix(data);
            TransformationMatrix = IMatrix.Mult(TransformationMatrix, m);
        }

        public void Mirror(IPlane plane)
        {
            IVector3D n = plane.Normal;
            IMatrix tensor = n.TensorProduct(n);
            double dot_plane = IVector3D.Dot(plane.Origin, n);

            double[][] data = new double[4][];
            data[0] = new double[]{ 1 - 2 * tensor.GetData(0, 0), -2 * tensor.GetData(0, 1), -2 * tensor.GetData(0, 2), 0 };
            data[1] = new double[]{ -2 * tensor.GetData(1, 0), 1 - 2 * tensor.GetData(1, 1), -2 * tensor.GetData(1, 2), 0 };
            data[2] = new double[]{ -2 * tensor.GetData(2, 0), -2 * tensor.GetData(2, 1), 1 - 2 * tensor.GetData(2, 2), 0 };
            data[3] = new double[]{ 2 * dot_plane * n.X, 2 * dot_plane * n.Y, 2 * dot_plane * n.Z, 1 };
            IMatrix m = new IMatrix(data);
            TransformationMatrix = IMatrix.Mult(TransformationMatrix, m);
        }
    }
}
