using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public struct ITopologicHalfFacet : IGH_Goo
    {
        private int[] _hf;
        public ITopologicHalfFacet(int[] halfFacet)
        {
            _hf = halfFacet;
        }

        public override string ToString()
        {
            string msg = "IHalfFacet{";
            for (int i = 0; i < _hf.Length-1; i++)
            {
                msg += _hf[i] + ", ";
            }
            msg += _hf[_hf.Length-1] + "}";
            return msg;
        }

        #region IGH_Goo methods
        public int[] Keys => _hf;

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

        public string TypeName
        {
            get => ToString();
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
            if (typeof(T).IsAssignableFrom(typeof(ITopologicHalfFacet)))
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
        #endregion
    }
}
