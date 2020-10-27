using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Iguana.IguanaMesh.ITypes
{
    public struct IVector3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public IVector3D(double _x, double _y, double _z)
        {
            this.X = _x;
            this.Y = _y;
            this.Z = _z;
        }

        public IVector3D(IVector3D vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }

        public IVector3D(Vector3d vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }

        public IVector3D Copy()
        {
            return new IVector3D(X, Y, Z);
        }

        public double Sq()
        {
            return Math.Abs(X * X + Y * Y + Z * Z);
        }

        public override String ToString()
        {
            String text = "( " + String.Format("{0,4}", X , Y , Z) + " )";
            return text;
        }

        public void Set(double _x, double _y, double _z)
        {
            this.X = _x;
            this.Y = _y;
            this.Z = _z;
        }

        public void Set(IVector3D vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }

        public void Add(IVector3D vector)
        {
            X += vector.X;
            Y += vector.Y;
            Z += vector.Z;
        }

        public void Add(double _x, double _y, double _z)
        {
            this.X += _x;
            this.Y += _y;
            this.Z += _z;
        }

        public void Sub(IVector3D vector)
        {
            this.X -= vector.X;
            this.Y -= vector.Y;
            this.Z -= vector.Z;
        }

        public void Mult(double scalar)
        {
            this.X *= scalar;
            this.Y *= scalar;
            this.Z *= scalar;
        }

        public void Div(double scalar)
        {
            this.X /= scalar;
            this.Y /= scalar;
            this.Z /= scalar;
        }

        public void Cross(IVector3D vector)
        {
            double Cx = X * vector.Z - X * vector.Y;
            double Cy = Y * vector.X - Y * vector.Z;
            double Cz = Z * vector.Y - Z * vector.X;
            X = Cx;
            Y = Cy;
            Z = Cz;
        }

        public double Mag()
        {
            double magnitude = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
            if (magnitude == Double.NaN) magnitude = 0;
            return magnitude;
        }

        public static IVector3D Sub(IVector3D vector1, IVector3D vector2)
        {
            return new IVector3D(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        public static IVector3D operator -(IVector3D vector1, IVector3D vector2)
        {
            IVector3D newVector = new IVector3D(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
            return newVector;
        }

        public static IVector3D Mult(IVector3D vector, double scalar)
        {
            IVector3D newVector = new IVector3D(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
            return newVector;
        }

        public static IVector3D operator *(IVector3D vector, double scalar)
        {
            IVector3D newVector = new IVector3D(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
            return newVector;
        }

        public static IVector3D Add(IVector3D vector1, IVector3D vector2)
        {
            IVector3D newVector = new IVector3D(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
            return newVector;
        }

        public static IVector3D operator +(IVector3D vector1, IVector3D vector2)
        {
            IVector3D newVector = new IVector3D(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
            return newVector;
        }

        public static IVector3D MassiveAddition(IVector3D[] vectorsToAdd)
        {
            IVector3D newVector = new IVector3D();
            foreach (IVector3D v in vectorsToAdd)
            {
                newVector.X += v.X;
                newVector.Y += v.Y;
                newVector.Z += v.Z;
            }
            return newVector;
        }

        public static double Dot(IVector3D vector1, IVector3D vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        public void Norm()
        {
            double magnitude = this.Mag();
            if (magnitude != 0)
            {
                X /= magnitude;
                Y /= magnitude;
                Z /= magnitude;
            }
            else
            {
                X = 0;
                Y = 0;
                Z = 0;
            }
        }

        public void Limit(double maxMagnitude)
        {
            double magnitude = this.Mag();
            if (magnitude > maxMagnitude)
            {
                X = (X / magnitude) * maxMagnitude;
                Y = (Y / magnitude) * maxMagnitude;
                Z = (Z / magnitude) * maxMagnitude;
            }
        }

        public static double Dist(IVector3D vector1, IVector3D vector2)
        {
            IVector3D vec = IVector3D.Sub(vector1, vector2);
            return vec.Mag();
        }

        public static Double AngleBetween(IVector3D vector1, IVector3D vector2)
        {
            Double dot = IVector3D.Dot(vector1, vector2);
            Double m1 = Math.Sqrt((vector1.X * vector1.X) + (vector1.Y * vector1.Y) + (vector1.Z * vector1.Z));
            Double m2 = Math.Sqrt((vector2.X * vector2.X) + (vector2.Y * vector2.Y) + (vector2.Z * vector2.Z));
            Double angle = Math.Acos(dot / (m1 * m2));
            if (Double.IsNaN(angle)) angle = (double)0;
            return angle;
        }

        public static double AngleBetween(IVector3D vector1, IVector3D vector2, IVector3D origin)
        {
            IVector3D v1 = IVector3D.Sub(vector1, origin);
            IVector3D v2 = IVector3D.Sub(vector2, origin);
            double dot = IVector3D.Dot(v1, v2);
            double cos = dot / (v1.Mag() * v2.Mag());
            double angle = Math.Acos(cos);
            if (Double.IsNaN(angle)) angle = (double)0;
            return angle;
        }

        public static IVector3D Norm(IVector3D vector)
        {
            IVector3D unit = new IVector3D(vector);
            unit.Norm();
            return unit;
        }

        public static IVector3D VectorWithMagnitude(IVector3D vector, double magnitude)
        {
            IVector3D unit = IVector3D.Norm(vector);
            unit.Mult(magnitude);
            return unit;
        }

        public static IVector3D Div(IVector3D vector, double scalar)
        {
            IVector3D newVector = new IVector3D();
            if (scalar != 0) newVector = new IVector3D(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
            return newVector;
        }

        public static IVector3D operator /(IVector3D vector, double scalar)
        {
            IVector3D newVector = new IVector3D();
            if (scalar != 0) newVector = new IVector3D(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
            return newVector;
        }

        public static IVector3D Cross(IVector3D vector1, IVector3D vector2, Boolean unitize)
        {
            IVector3D crossVector;
            double Cx = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            double Cy = vector1.Z * vector2.X - vector1.X * vector2.Z;
            double Cz = vector1.X * vector2.Y - vector1.Y * vector2.X;
            crossVector = new IVector3D(Cx, Cy, Cz);
            if (unitize) crossVector.Norm();
            return crossVector;
        }

        public static IVector3D operator *(IVector3D vector1, IVector3D vector2)
        {
            IVector3D crossVector;
            double Cx = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            double Cy = vector1.Z * vector2.X - vector1.X * vector2.Z;
            double Cz = vector1.X * vector2.Y - vector1.Y * vector2.X;
            crossVector = new IVector3D(Cx, Cy, Cz);
            return crossVector;
        }

        public static IVector3D operator *(IVector3D vector, ITopologicVertex vertex)
        {
            IVector3D crossVector;
            double Cx = vector.Y * vertex.Z - vector.Z * vertex.Y;
            double Cy = vector.Z * vertex.X - vector.X * vertex.Z;
            double Cz = vector.X * vertex.Y - vector.Y * vertex.X;
            crossVector = new IVector3D(Cx, Cy, Cz);
            return crossVector;
        }

        public override Boolean Equals(Object obj)
        {
            Boolean equal = false;
            try
            {
                IVector3D vector = (IVector3D)obj;
                if (X == vector.X && Y == vector.Y && Z == vector.Z)
                {
                    equal = true;
                }
                else equal = false;
            }
            catch (Exception) { }

            return equal;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Boolean operator ==(IVector3D vector1, IVector3D vector2)
        {
            return vector1.X == vector2.X && vector1.Y == vector2.Y && vector1.Z == vector2.Z;
        }

        public static Boolean operator !=(IVector3D vector1, IVector3D vector2)
        {
            return vector1.X != vector2.X || vector1.Y != vector2.Y || vector1.Z != vector2.Z;
        }

        public void Reverse()
        {
            X *= -1;
            Y *= -1;
            Z *= -1;
        }

        public static IVector3D Rotate2D(IVector3D vector, double angle)
        {
            Double x = vector.X * Math.Cos(angle) - vector.Y * Math.Sin(angle);
            Double y = vector.Y * Math.Sin(angle) + vector.Y * Math.Cos(angle);
            Double z = vector.Z;
            IVector3D rVector = new IVector3D(x, y, z);
            return rVector;
        }

        public static IVector3D CreateVector(IVector3D toVector, IVector3D fromVector)
        {
            return IVector3D.Sub(toVector, fromVector);
        }

        public static IVector3D CreateUnitVector(IVector3D toVector, IVector3D fromVector)
        {
            IVector3D vector = IVector3D.Sub(toVector, fromVector);
            vector.Norm();
            return vector;
        }

        public static IVector3D Reverse(IVector3D vec)
        {
            IVector3D vector = new IVector3D(vec.X, vec.Y, vec.Z);
            vector.Reverse();
            return vector;
        }

        public static IVector3D Add(IVector3D vector, double value)
        {
            vector.X += value;
            vector.Y += value;
            vector.Z += value;
            return vector;
        }

        public static IVector3D operator +(IVector3D vector, double value)
        {
            vector.X += value;
            vector.Y += value;
            vector.Z += value;
            return vector;
        }

        public static IVector3D operator +(IVector3D vector, ITopologicVertex vertex)
        {
            vector.X += vertex.X;
            vector.Y += vertex.Y;
            vector.Z += vertex.Z;
            return vector;
        }

        public static IVector3D operator -(IVector3D vector, double value)
        {
            vector.X -= value;
            vector.Y -= value;
            vector.Z -= value;
            return vector;
        }

        public static IVector3D operator -(IVector3D vector, ITopologicVertex vertex)
        {
            vector.X -= vertex.X;
            vector.Y -= vertex.Y;
            vector.Z -= vertex.Z;
            return vector;
        }

        public void SetMagnitude(double magnitude)
        {
            this.Norm();
            this.Mult(magnitude);
        }

        public Double? GetVectorComponent(int index)
        {
            Double? value;
            switch (index)
            {
                case 0:
                    value = X;
                    break;
                case 1:
                    value = Y;
                    break;
                case 2:
                    value = Z;
                    break;
                default:
                    value = null;
                    break;
            }

            return value;
        }

        public void SetVectorComponent(int index, double value)
        {
            switch (index)
            {
                case 0:
                    X = value;
                    break;
                case 1:
                    Y = value;
                    break;
                case 2:
                    Z = value;
                    break;
            }
        }

        public double[] ToArray()
        {
            return new double[] { X, Y, Z };
        }

        public List<double> ToList()
        {
            return new List<double>(){ X, Y, Z };
        }
    }
}
