/*
 * <Iguana>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ITypes
{
    public struct IPoint3D : IGH_Goo
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

        public IGH_Goo Duplicate()
        {
            return (IGH_Goo) new IPoint3D(X,Y,Z);
        }

        public IGH_GooProxy EmitProxy()
        {
            return null;
        }

        public bool CastFrom(object source)
        {
            if (typeof(GH_Point).IsAssignableFrom(source.GetType()))
            {
                Point3d vec = ((GH_Point)source).Value;
                this.X = vec.X;
                this.Y = vec.Y;
                this.Z = vec.Z;
                return true;
            }
            return false;
        }

        public bool CastTo<T>(out T target)
        {
            if (typeof(T).Equals(typeof(GH_Point)))
            {
                target = (T)(object)new GH_Point(new Point3d(X, Y, Z));
                return true;
            }
            target = default(T);
            return false;
        }

        public object ScriptVariable()
        {
            return this;
        }

        public bool Write(GH_IWriter writer)
        {
            return true;
        }

        public bool Read(GH_IReader reader)
        {
            return true;
        }

        public bool IsValid => true;

        public string IsValidWhyNot => "";

        public string TypeName => "IPoint3D";

        public string TypeDescription => "Three-dimensional point";
    }
}
