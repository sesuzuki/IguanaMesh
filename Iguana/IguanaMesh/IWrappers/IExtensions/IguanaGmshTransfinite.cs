using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers.IExtensions
{
    public struct IguanaGmshTransfinite : IGH_Goo
    {
        public int Dim { get; set; }
        public int Tag { get; set; }
        public int[] Corners { get; set; }
        public int NodesNumber { get; set; }
        public double Coef { get; set; }
        public string MethodType { get; set; }

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
            return "IguanaGmshTransfinite";
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
            if (typeof(T).IsAssignableFrom(typeof(IguanaGmshTransfinite)))
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
