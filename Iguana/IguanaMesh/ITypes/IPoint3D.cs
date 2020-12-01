using Iguana.IguanaMesh.IModifiers;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ITypes
{
    public struct IPoint3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public IPoint3D(double x,double y,double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public IPoint3D(double x, double y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        public static IPoint3D operator -(IPoint3D point, IVector3D vector)
        {
            return new IPoint3D(point.X - vector.X, point.Y - vector.Y, point.Z - vector.Z);
        }

        public static IPoint3D operator +(IPoint3D point, IVector3D vector)
        {
            return new IPoint3D(point.X + vector.X, point.Y + vector.Y, point.Z + vector.Z);
        }

        public static IVector3D operator +(IPoint3D point1, IPoint3D point2)
        {
            return new IVector3D(point1.X + point2.X, point1.Y + point2.Y, point1.Z + point2.Z);
        }

        public static IVector3D operator -(IPoint3D point1, IPoint3D point2)
        {
            return new IVector3D(point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z);
        }

        public static IVector3D operator *(IPoint3D point, double number)
        {
            return new IVector3D(point.X * number, point.Y * number, point.Z * number);
        }

        public static IVector3D operator /(IPoint3D point, double number)
        {
            return new IVector3D(point.X / number, point.Y / number, point.Z / number);
        }

        public double DistanceTo(IPoint3D pt)
        {
            IVector3D vec = this - pt;
            return vec.Mag();
        }

        public void Transform(ITransformX m)
        {
            double weight = m.TransformationMatrix.GetData(3, 0) * X + m.TransformationMatrix.GetData(3, 1) * Y + m.TransformationMatrix.GetData(3, 2) * Z + m.TransformationMatrix.GetData(3, 3);
            double x = ( m.TransformationMatrix.GetData(0,0) * X + m.TransformationMatrix.GetData(0,1) * Y + m.TransformationMatrix.GetData(0,2) * Z + m.TransformationMatrix.GetData(0,3) ) / weight;
            double y = ( m.TransformationMatrix.GetData(1,0) * X + m.TransformationMatrix.GetData(1,1) * Y + m.TransformationMatrix.GetData(1,2) * Z + m.TransformationMatrix.GetData(1,3) ) / weight;
            double z = ( m.TransformationMatrix.GetData(2,0) * X + m.TransformationMatrix.GetData(2,1) * Y + m.TransformationMatrix.GetData(2,2) * Z + m.TransformationMatrix.GetData(2,3) ) / weight;
            X = x;
            Y = y;
            Z = z;
        }

        public Point3d RhinoPoint
        {
            get => new Point3d(X,Y,Z);
        }
    }
}
