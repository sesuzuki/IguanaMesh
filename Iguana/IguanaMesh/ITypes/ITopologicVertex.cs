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

using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
using Rhino.Geometry;
using System;
using Grasshopper.Kernel;

namespace Iguana.IguanaMesh.ITypes
{
    public struct ITopologicVertex : IGH_Goo, ICloneable, IGH_PreviewData
    {
        public double X { get => _pos.X; set => _pos.X = value; }
        public double Y { get => _pos.Y; set => _pos.Y = value; }
        public double Z { get => _pos.Z; set => _pos.Z = value; }
        public double U { get; set; }
        public double V { get; set; }
        public double W { get; set; }
        public int Key { get; set; }
        public IPoint3D Position { get => _pos; set => _pos = value; }

        private IPoint3D _pos;
        private Int64[] _v2hf;
        private bool[] _visits;

        public ITopologicVertex(double _x, double _y, double _z, int _key=-1)
        {
            this._pos = new IPoint3D(_x, _y, _z);
            this.U = 0;
            this.V = 0;
            this.W = 0;
            this.Key = _key;
            this._v2hf = new Int64[3];
            _visits = new bool[2];
        }

        public ITopologicVertex(Point3d pt, int _key=-1)
        {
            this._pos = new IPoint3D(pt.X, pt.Y, pt.Z);
            this.U = 0;
            this.V = 0;
            this.W = 0;
            this.Key = _key;
            this._v2hf = new Int64[3];
            _visits = new bool[2];
        }

        public ITopologicVertex(ITopologicVertex v)
        {
            this._pos = new IPoint3D(v.X, v.Y, v.Z);
            this.U = v.U;
            this.V = v.V;
            this.W = v.W;
            this.Key = v.Key;
            this._v2hf = new Int64[3];
            _visits = new bool[2];
        }

        public ITopologicVertex(IPoint3D v, int _key = -1)
        {
            this._pos = v;
            this.U = 0;
            this.V = 0;
            this.W = 0;
            this.Key = _key;
            this._v2hf = new Int64[3];
            _visits = new bool[2];
        }

        public ITopologicVertex(double _x, double _y, double _z, double _u, double _v, double _w, int _key=-1)
        {
            this._pos = new IPoint3D(_x, _y, _z);
            this.U = _u;
            this.V = _v;
            this.W = _w;
            this.Key = _key;
            this._v2hf = new Int64[3];
            _visits = new bool[2];
        }

        public void CleanTopologicalData()
        {
            _v2hf = new Int64[3];
        }

        public void SetV2HF(int dim, Int32 elementID, Int32 halfFacetID)
        {
            _v2hf[dim] = (Int64) elementID << 32 | (Int64) halfFacetID;
        }

        public void SetV2HF(int dim, Int64 sibData)
        {
            _v2hf[dim] = sibData;
        }

        public Int32 GetElementID(int dim) {
            if (dim >= 0 && dim <= 2) return (Int32)(_v2hf[dim] >> 32);
            else return -1;
        }

        public Int32 GetHalfFacetID(int dim)
        {
            if (dim >= 0 && dim <= 2) return (Int32)_v2hf[dim];
            else return -1;
        }

        public Int64[] V2HF
        {
            get => _v2hf;
        }

        public bool IsIsolated()
        {
            if (_v2hf[0] != 0 || _v2hf[1] != 0 || _v2hf[2] != 0) return false;
            else return true;
        }

        public double[] TextureCoordinates
        {
            get => new double[] { U, V, W };
            set
            {
                U = value[0];
                V = value[1];
                W = value[2];
            }
        }

        public double DistanceTo(ITopologicVertex vertex)
        {
            return IVector3D.Dist(new IVector3D(X,Y,Z), new IVector3D(vertex.X, vertex.Y, vertex.Z));
        }

        public double DistanceTo(Point3d pt)
        {
            return IVector3D.Dist(new IVector3D(X, Y, Z), new IVector3D(pt.X, pt.Y, pt.Z));
        }

        public object Clone()
        {
            ITopologicVertex v = new ITopologicVertex(X, Y, Z, U, V, W);
            v.Key = Key;
            v._v2hf = _v2hf;
            v._visits = _visits;
            return v;
        }

        /// <summary>
        /// Copy without topologic data.
        /// </summary>
        /// <returns></returns>
        public ITopologicVertex CleanCopy()
        {
            return new ITopologicVertex(X, Y, Z, U, V, W, Key);
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            args.Pipeline.DrawPoint(RhinoPoint, Rhino.Display.PointStyle.X, 5, args.Color);
        }

        public override bool Equals(object obj)
        {
            Boolean flag = false;
            try
            {
                ITopologicVertex vertex = (ITopologicVertex) obj;
                if (vertex.X == X && vertex.Y == Y && vertex.Z == Z) flag = true;
            }
            catch (Exception) {}

            return flag;
        }

        public static bool operator ==(ITopologicVertex v1, ITopologicVertex v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z) return true;
            else return false;
        }

        public static bool operator !=(ITopologicVertex v1, ITopologicVertex v2)
        {
            if (v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Point3d RhinoPoint
        {
            get => new Point3d(X,Y,Z);
        }

        public override string ToString()
        {
            return "ITopologicVertex (Key:" + Key + " X:" + X.ToString("F") + " Y:" + Y.ToString("F") + " Z:" + Z.ToString("F") + ")\n";
        }

        public string SiblingHalfFacetDataToString()
        {
            string msg = "Vertex ID: " + Key + " =>\n";
            if(_v2hf[0] != 0) msg += "1D-Element ID: " + GetElementID(0) + " :: Half-Facet ID: " + GetHalfFacetID(0) + "\n";
            if(_v2hf[1] != 0) msg += "2D-Element ID: " + GetElementID(1) + " :: Half-Facet ID: " + GetHalfFacetID(1) + "\n";
            if(_v2hf[2] != 0) msg += "2D-Element ID: " + GetElementID(2) + " :: Half-Facet ID: " + GetHalfFacetID(2) + "\n";

            return msg;
        }

        #region IGH_Goo methods
        public bool IsValid
        {
            get => this!=null;
        }

        public string IsValidWhyNot
        {
            get
            {
                string msg;
                if (this.IsValid) msg = string.Empty;
                else msg = string.Format("Invalid type.", this.TypeName);
                return msg;
            }
        }

        public string TypeName
        {
            get => "ITopologicVertex";
        }

        public string TypeDescription
        {
            get => ToString();
        }

        public BoundingBox ClippingBox => new BoundingBox(new[] { RhinoPoint });

        public IGH_Goo Duplicate()
        {
            return (IGH_Goo)this.MemberwiseClone();
        }

        public IGH_GooProxy EmitProxy()
        {
            return null;
        }

        public bool CastFrom(object source)
        {
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
        #endregion
    }
}
