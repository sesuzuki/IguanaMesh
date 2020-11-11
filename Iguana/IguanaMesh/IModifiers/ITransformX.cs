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
        public IMatrix T { get; set; }
        public IMatrix InvertT { get; set; }

        public ITransformX()
        {
            T = IMatrix.Identity4x4Matrix;
            InvertT = IMatrix.InvertIdentity4x4Matrix;
        }

        public void Translate(IPoint3D pt, double factor=1.0)
        {
            double[][] data1 = new double[4][];
            data1[0] = new double[]{ 1, 0, 0, factor* pt.X};
            data1[1] = new double[]{ 0, 1, 0, factor* pt.Y};
            data1[2] = new double[]{ 0, 0, 1, factor* pt.Z};
            data1[3] = new double[]{ 0, 0, 0, 1 };

            IMatrix m1 = new IMatrix(data1);
            T = IMatrix.Mult(T, m1);

            double[][] data2 = new double[4][];
            data2[0] = new double[] { 1, 0, 0, -factor * pt.X };
            data2[1] = new double[] { 0, 1, 0, -factor * pt.Y };
            data2[2] = new double[] { 0, 0, 1, -factor * pt.Z };
            data2[3] = new double[] { 0, 0, 0, 1 };
            IMatrix m2 = new IMatrix(data2);
            InvertT = IMatrix.Mult(InvertT, m2);
        }

        public void RotateAboutOrigin(double angle, IVector3D axis)
        {
            axis.Norm();
            double s = Math.Sin(angle);
            double c = Math.Cos(angle);
            double[][] data = new double[4][];
            data[0] = new double[] { axis.X * axis.X + (1 - axis.X * axis.X) * c, axis.X * axis.Y * (1 - c) - axis.Z * s, axis.X * axis.Z * (1 - c) + axis.Y * s, 0 };
            data[1] = new double[] { axis.X * axis.Y * (1 - c) + axis.Z * s, axis.Y * axis.Y + (1 - axis.Y * axis.Y) * c, axis.Y * axis.Z * (1 - c) - axis.X * s, 0 };
            data[2] = new double[] { axis.X * axis.Z * (1 - c) - axis.Y * s, axis.Y * axis.Z * (1 - c) + axis.X * s, axis.Z * axis.Z + (1 - axis.Z * axis.Z) * c, 0 };
            data[3] = new double[] { 0, 0, 0, 1 };
            IMatrix m = new IMatrix(data);
            T = IMatrix.Mult(T, m);

            m.Transpose();
            InvertT = IMatrix.Mult(InvertT, m);
        }

        public void RotateAboutAxis(double angle, IPoint3D pt, IVector3D axis)
        {
            Translate(pt,-1);
            RotateAboutOrigin(angle, axis);
            Translate(pt);
        }
    }
}
