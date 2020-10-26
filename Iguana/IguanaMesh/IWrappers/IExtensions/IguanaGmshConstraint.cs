﻿using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace Iguana.IguanaMesh.IWrappers.IExtensions
{
    public struct IguanaGmshConstraint : IGH_Goo
    {
        private int _dim;
        private Object _geom;
        private double _val;
        private int _entityDim;
        private int _entityTag;

        public IguanaGmshConstraint(int dimension, Object geometry, double size, int entityDimension, int entityTag)
        {
            _dim = dimension;
            _geom = geometry;
            _val = size;
            _entityDim = entityDimension;
            _entityTag = entityTag;
        }

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
            if (typeof(T).IsAssignableFrom(typeof(IguanaGmshConstraint)))
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
