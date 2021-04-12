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
using System;

namespace Iguana.IguanaMesh.ITypes
{
    public struct IConstraint : IGH_Goo
    {
        private int _dim;
        private Object _geom;
        private double _val;
        private int _entityDim;
        private int _entityTag;
        private int _numberOfNodes;
        private double _divisionLength;

        public IConstraint(int dimension, Object geometry, double size, int entityDimension, int entityTag, int numberOfNodes=1, double divisionLength=1)
        {
            _dim = dimension;
            _geom = geometry;
            _val = size;
            _entityDim = entityDimension;
            _entityTag = entityTag;
            _divisionLength = divisionLength;
            _numberOfNodes = numberOfNodes;
        }

        public int NodesCountPerCurve { get => _numberOfNodes; }

        public double CurveDivisionLength { get => _divisionLength; }

        public Object RhinoGeometry { get => _geom; }

        public double Size { get => _val; }

        public int Dim { get => _dim; }

        public int EntityID { get => _entityTag; }

        public int EntityDim { get => _entityDim; }

        #region GH_methods
        public bool IsValid
        {
            get => !this.Equals(null);
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

        public override string ToString()
        {
            return "IguanaGmshConstraint";
        }

        public string TypeName
        {
            get => "IguanaGmshConstraint";
        }

        public string TypeDescription
        {
            get => ToString();
        }

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

        public bool CastTo<T>(out T target)
        {
            if (typeof(T).IsAssignableFrom(typeof(IConstraint)))
            {
                target = (T)(object)this;
                return true;
            }

            target = default(T);
            return false;
        }
        #endregion
    }
}
