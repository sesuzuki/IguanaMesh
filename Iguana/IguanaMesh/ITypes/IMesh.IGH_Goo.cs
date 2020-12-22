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
using Rhino;
using Rhino.Geometry;
using System;
using System.Linq;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh : IGH_GeometricGoo
    {
        public bool IsValid
        {
            get => _valid;
        }

        public string IsValidWhyNot
        {
            get
            {
                if (!_valid) return "Topologic errors appeared during the construction of the mesh.";
                else if (_vertices.Count == 0 && _elements.Count == 0) return "Mesh was initialized with 0 vertices and 0 elements.";
                else return string.Empty;
            }
        }

        public string TypeName
        {
            get => "IMesh";
        }

        public string TypeDescription
        {
            get => ToString();
        }

        public BoundingBox Boundingbox
        {
            get{
                return new BoundingBox(Vertices.Select(v => v.RhinoPoint));
            }
        }

        public Guid ReferenceID { get => Guid.Empty; set => value = Guid.Empty; }

        public bool IsReferencedGeometry => false;

        public bool IsGeometryLoaded => true;

        public IGH_Goo Duplicate()
        {
            return (IGH_Goo) this.DeepCopy();
        }

        public IGH_GooProxy EmitProxy()
        {
            return null;
        }

        public bool CastFrom(object source)
        {
            if (typeof(GH_Mesh).IsAssignableFrom(source.GetType()))
            {
                this.initData();
                this.AddRhinoMesh(((GH_Mesh)source).Value, false, 0.01);
                return true;
            }
            return false;
        }

        public bool CastTo<T>(out T target)
        {
            if (typeof(T).Equals(typeof(GH_Mesh)))
            {
                target = (T) (object) new GH_Mesh(IRhinoGeometry.TryGetRhinoMesh(this));
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

        public IGH_GeometricGoo DuplicateGeometry()
        {
            return this.DeepCopy();
        }

        public BoundingBox GetBoundingBox(Transform xform)
        {
            return BoundingBox.Empty;
        }

        public IGH_GeometricGoo Transform(Transform xform)
        {
            return null;
        }

        public IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            return null;
        }

        public bool LoadGeometry()
        {
            return false;
        }

        public bool LoadGeometry(RhinoDoc doc)
        {
            return false;
        }

        public void ClearCaches(){ }
    }
}
