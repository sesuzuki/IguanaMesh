using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh : IGH_Goo
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
                else if(_vertices.Count==0 && _elements.Count==0) return "Mesh was initialized with 0 vertices and 0 elements.";
                else return string.Empty;
            }
        }

        public string TypeName
        {
            get => "IMesh";
        }

        public string TypeDescription
        {
            get { return ("Defines an Iguana Mesh"); }
        }

        public BoundingBox Boundingbox => throw new NotImplementedException();

        public Guid ReferenceID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsReferencedGeometry => throw new NotImplementedException();

        public bool IsGeometryLoaded => throw new NotImplementedException();

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
            return false;
        }

        public bool CastTo<T>(out T target)
        {
            if (typeof(T).IsAssignableFrom(typeof(IMesh)))
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
