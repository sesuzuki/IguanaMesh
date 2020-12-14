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
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;

namespace Iguana.IguanaMesh.ITypes
{
    public struct ITopologicEdge : IGH_Goo, ICloneable, IGH_PreviewData
    {
        private ITopologicVertex _start, _end;

        public ITopologicVertex Start { get => _start; }
        public ITopologicVertex End { get => _end; }
        public int ID_Start { get => _start.Key; }
        public int ID_End { get => _end.Key; }


        public ITopologicEdge(ITopologicVertex start, ITopologicVertex end)
        {
            _start = start;
            _end = end;
        }

        public object Clone()
        {
            ITopologicEdge v = new ITopologicEdge(_start,_end);
            return v;
        }

        public override bool Equals(object obj)
        {
            ITopologicEdge edge = (ITopologicEdge)obj;
            if (edge.Start == _start && edge.End == _end) return true;
            else if (edge.End == _start && edge.Start == _end) return true;
            else return false;
        }

        public static bool operator ==(ITopologicEdge e1, ITopologicEdge e2)
        {
            if (e1.Start == e2._start && e1.End == e2._end) return true;
            else if (e1.End == e2._start && e1.Start == e2._end) return true;
            else return false;
        }

        public static bool operator !=(ITopologicEdge e1, ITopologicEdge e2)
        {
            if (e1.Start != e2._start || e1.End != e2._end) return true;
            else if (e1.End != e2._start || e1.Start != e2._end) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IGH_Preview methods

        public void DrawViewportMeshes(GH_PreviewMeshArgs args){ }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            args.Pipeline.DrawLine(Start.RhinoPoint, End.RhinoPoint, args.Color, args.Thickness);
        }

        #endregion

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
            get => "ITopologicEdge";
        }

        public string TypeDescription
        {
            get => "ITopologicEdge (" + Start.DistanceTo(End) + ")";
        }

        public BoundingBox ClippingBox => new BoundingBox(new Point3d[] { Start.RhinoPoint, End.RhinoPoint });

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
            if (typeof(T).IsAssignableFrom(typeof(ITopologicEdge)))
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
