﻿using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public struct ITopologicSiblingHalfFacet
    {
        public int ElementID { get; set; }
        public int HalfFacetID { get; set; }

        public ITopologicSiblingHalfFacet(int elementID, int halfFacetID)
        {
            ElementID = elementID;
            HalfFacetID = halfFacetID;
        }

        public override string ToString()
        {
            string msg = "ISiblingHalfFacet{ElementID(" + ElementID + ") :: HalfFacetID(" +HalfFacetID + ")}";
            return msg;
        }

        #region IGH_Goo methods

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
            if (typeof(T).IsAssignableFrom(typeof(ITopologicSiblingHalfFacet)))
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