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
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ITypes
{
    public struct IEntityInfo : IGH_Goo
    {
        public double _xmax, _xmin, _ymax, _ymin, _zmax, _zmin;
        private int _dim, _tag;
        private Point3d _pos;
        public Point3d Position { get => _pos; }
        public int Dimension { get => _dim; }
        public int Tag { get => _tag; }

        public IEntityInfo(int dim, int tag, Point3d position)
        {
            _dim = dim;
            _tag = tag;
            _pos = position;
            _xmax = _pos.X;
            _ymax = _pos.Y;
            _zmax = _pos.Z;
            _xmin = _pos.X;
            _ymin = _pos.Y;
            _zmin = _pos.Z;
        }

        public void SetBoundingBoxParameters(double xmin, double ymin, double zmin, double xmax, double ymax, double zmax)
        {
            _xmax = xmax;
            _ymax = ymax;
            _zmax = zmax;
            _xmin = xmin;
            _ymin = ymin;
            _zmin = zmin;
        }

        public BoundingBox GetBoundingBox()
        {
            return new BoundingBox(new Point3d(_xmin, _ymin, _zmin), new Point3d(_xmax, _ymax, _zmax));
        }

        public override string ToString()
        {
            string msg = "EntityInfo{ Tag: " + Tag + " - Dimension: " + Dimension + " }";
            return msg;
        }

        public bool IsValid
        {
            get
            {
                if (_dim == -1 || _tag == 0) return false;
                else return true;
            }
        }


        public string IsValidWhyNot {
            get
            {
                if (!IsValid) return "Invalid entity information. Errors during meshing may occured.";
                else return "Entity information.";
            }
        }

        public string TypeName => "IEntityGH";

        public string TypeDescription => "UnderlyingEntityInformation";

        public IGH_Goo Duplicate()
        {
            return (IGH_Goo) this.MemberwiseClone();
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
            if (typeof(T).IsAssignableFrom(typeof(IEntityInfo)))
            {
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
    }
}
