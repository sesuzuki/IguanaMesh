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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public class IPlane
    {
		private IVector3D n, u, v;
		public IPoint3D Origin { get; set; }
		public IVector3D Normal { get => n; set { n = value; n.Norm(); } }
		public IVector3D AxisY { get => v; }
		public IVector3D AxisX { get => u; }

		public IPlane(IPoint3D origin, IVector3D axisX, IVector3D axisY)
		{
			Normal = IVector3D.Cross(axisX,axisY,true);
			Origin = origin;
			BuildAxes();
		}

		public IPlane(IPoint3D origin, IVector3D normal)
		{
			Normal = normal;
			Origin = origin;
			BuildAxes();
		}

		public IPlane(IPoint3D origin, IPoint3D ptX, IPoint3D ptY)
		{
			IVector3D xx = IVector3D.CreateVector(ptX,origin);
			IVector3D yy = IVector3D.CreateVector(ptY,origin);
			Normal = IVector3D.Cross(xx,yy,true);
			Origin = origin;
			BuildAxes();
		}

		public IPlane(double ox, double oy, double oz, double nx, double ny, double nz)
		{
			Origin = new IPoint3D(ox, oy, oz);
			Normal = new IVector3D(nx, ny, nz);
			BuildAxes();
		}

		public double Determinant()
        {
			IVector3D n = Normal;
			n.Reverse();
			return n.Dot(Origin);
		}

		public void Flip()
		{
			Normal.Reverse();
			BuildAxes();
		}

		public String toString()
		{
			return "IPlane {o(" + Origin.X + ", " + Origin.Y + ", " + Origin.Z + ") :: n(" + Normal.X + ", " + Normal.Y + ", " + Normal.Z + ")}";
		}

		private void BuildAxes()
		{
			double x = Math.Abs(Normal.X);
			double y = Math.Abs(Normal.Y);
			if (x >= y) u = new IVector3D(Normal.Z, 0, -Normal.X);
			else u = new IVector3D(0, Normal.Z, -Normal.Y);
			u.Norm();
			v = IVector3D.Cross(Normal, u, true);
		}

		public IPoint3D GetPlanePointFromRelativePlane(IPoint3D ptInPlane, IPlane plane)
        {
			IPoint3D ptWorld = plane.GetWorldPointFromLocalPlane(ptInPlane);
			return GetPlanePointFromWorldPlane(ptWorld);
        }

		public IPoint3D GetPlanePointFromWorldPlane(IPoint3D pt)
		{
			IPoint3D local = new IPoint3D();
			local.X = u.X * (pt.X - Origin.X) + u.Y * (pt.Y - Origin.Y) + u.Z * (pt.Z - Origin.Z);
			local.Y = v.X * (pt.X - Origin.X) + v.Y * (pt.Y - Origin.Y) + v.Z * (pt.Z - Origin.Z);
			local.Z = n.X * (pt.X - Origin.X) + n.Y * (pt.Y - Origin.Y) + n.Z * (pt.Z - Origin.Z);
			return local;
		}

		public IPoint3D GetWorldPointFromLocalPlane(IPoint3D pt)
		{
			IPoint3D world = new IPoint3D();
			world.X = Origin.X + pt.X * u.X + pt.Y * v.X;
			world.Y = Origin.Y + pt.X * u.Y + pt.Y * v.Y;
			world.Z = Origin.Z + pt.X * u.Z + pt.Y * v.Z;
			return world;
		}

		public IPoint3D GetPoint(double x, double y)
		{
			IPoint3D pt = new IPoint3D();
			pt.X = Origin.X + x * u.X + y * v.X;
			pt.Y = Origin.Y + x * u.Y + y * v.Y;
			pt.Z = Origin.Z + x * u.Z + y * v.Z;
			return pt;
		}

		public IPoint3D GetPoint(double x, double y, double z)
		{
			IPoint3D pt = new IPoint3D();
			pt.X = Origin.X + x * u.X + y * v.X + z * n.X;
			pt.Y = Origin.Y + x * u.Y + y * v.Y + z * n.Y;
			pt.Z = Origin.Z + x * u.Z + y * v.Z + z * n.Z;
			return pt;
		}

		public IPoint3D GetClosestPoint(IPoint3D p)
		{
			double t = IVector3D.Dot(Normal,new IVector3D(p.X,p.Y,p.Z)) + Determinant();
			return new IPoint3D(p.X - t * Normal.X, p.Y - t * Normal.Y, p.Z - t * Normal.Z);
		}

		public double DistanceToPlane(IPoint3D pt)
        {
			return Normal.Dot(pt) + Determinant();
		}

		public static IPlane WorldXY { get => new IPlane(0, 0, 0, 0, 0, 1); }
		public static IPlane WorldXZ { get => new IPlane(0, 0, 0, 0, 1, 0); }
		public static IPlane WorldYZ { get => new IPlane(0, 0, 0, 1, 0, 0); }
	}
}
