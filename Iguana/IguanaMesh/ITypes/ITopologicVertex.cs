using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Iguana.IguanaMesh.ITypes
{
    public struct ITopologicVertex : IGH_Goo, ICloneable
    {
        public double X { get => _pos.X; set => _pos.X = value; }
        public double Y { get => _pos.Y; set => _pos.Y = value; }
        public double Z { get => _pos.Z; set => _pos.Z = value; }
        public double U { get; set; }
        public double V { get; set; }
        public double W { get; set; }
        public int Key { get; set; }
        public IVector3D Normal { get; set; }
        public IVector3D Position { get => _pos; set => _pos = value; }

        private IVector3D _pos;
        private Int64 _v2hf;

        public ITopologicVertex(double _x, double _y, double _z, int _key=-1)
        {
            this._pos = new IVector3D(_x, _y, _z);
            this.U = 0;
            this.V = 0;
            this.W = 0;
            this.Key = _key;
            this.Normal = new IVector3D();
            this._v2hf = 0;
        }

        public ITopologicVertex(Point3d pt, int _key=-1)
        {
            this._pos = new IVector3D(pt.X, pt.Y, pt.Z);
            this.U = 0;
            this.V = 0;
            this.W = 0;
            this.Key = _key;
            this.Normal = new IVector3D();
            this._v2hf = 0;
        }

        public ITopologicVertex(double _x, double _y, double _z, double _u, double _v, double _w, int _key=-1)
        {
            this._pos = new IVector3D(_x, _y, _z);
            this.U = _u;
            this.V = _v;
            this.W = _w;
            this.Key = _key;
            this.Normal = new IVector3D();
            this._v2hf = 0;
        }

        public void SetV2HF(Int32 elementID, Int32 halfFacetID)
        {
            _v2hf = (Int64) elementID << 32 | (Int64) halfFacetID;
        }

        public void SetV2HF(Int64 sibData)
        {
            _v2hf = sibData;
        }

        public Int32 GetElementID() {
            Int64 sibData = _v2hf;
            if (sibData < 0) sibData *= -1;
            return (Int32)(sibData >> 32);
        }

        public Int32 GetHalfFacetID()
        {
            Int64 sibData = _v2hf;
            if (sibData < 0) sibData *= -1;
            return (Int32)sibData;
        }

        public void RegisterHalfFacetVisit()
        {
            if(_v2hf > 0) _v2hf *= -1;
        }
        public void ClearHalfFacetVisit()
        {
            if (_v2hf < 0) _v2hf *= -1;
        }
        public Boolean IsHalfFacetVisited()
        {
            if (_v2hf < 0) return true;
            else return false;
        }

        public Boolean IsNakedHalfFacet()
        {
            if (_v2hf == 0) return true;
            else return false;
        }

        public Int64 V2HF
        {
            get => _v2hf;
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
            return v;
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
            return "Vertex ID: " + Key + " :: Element ID: " + GetElementID() + " :: Half-Facet ID: " + GetHalfFacetID() + "\n";
        }

        #region IGH_Goo methods
        public bool IsValid
        {
            get => this.Equals(null);
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
            if (typeof(T).IsAssignableFrom(typeof(ITopologicVertex)))
            {
                if (this == null)
                    target = default(T);
                else
                    target = (T)(object)this;
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
