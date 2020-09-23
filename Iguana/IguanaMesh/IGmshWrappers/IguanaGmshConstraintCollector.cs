using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IGmshWrappers
{
    public class IguanaGmshConstraintCollector
    {
        private double[] _ptsSize = new double[0];
        private double[] _crvSize = new double[0];
        private Point3d[] _pts = new Point3d[0];
        private Curve[] _crv = new Curve[0];

        public IguanaGmshConstraintCollector(List<Point3d> pts, List<double> ptsSize, List<Curve> crv, List<double> crvSize)
        {
            if (pts != null || pts.Count>0)
            {
                _pts = pts.ToArray();
                _ptsSize = new double[pts.Count];
                if (pts.Count == ptsSize.Count)
                {
                    for (int i = 0; i < pts.Count; i++) _ptsSize[i] = ptsSize[i];
                }
                else
                {
                    for (int i = 0; i < pts.Count; i++) _ptsSize[i] = ptsSize[0];
                }
            }

            if (crv != null || crv.Count>0)
            {
                _crv = crv.ToArray();
                _crvSize = new double[crv.Count];
                if (crv.Count == crvSize.Count)
                {
                    for (int i = 0; i < crv.Count; i++) _crvSize[i] = crvSize[i];
                }
                else
                {
                    for (int i = 0; i < crv.Count; i++) _crvSize[i] = crvSize[0];
                }
            }
        }

        public bool HasPointConstraints()
        {
            if (_pts != null || _pts.Length==0) return true;
            else return false;
        }

        public int GetPointConstraintCount()
        {
            if (_pts != null) return _pts.Length;
            else return 0;
        }

        public int GetCurveConstraintCount()
        {
            if (_crv != null) return _crv.Length;
            else return 0;
        }

        public bool HasCurveConstraints()
        {
            if (_crv != null || _crv.Length==0) return true;
            else return false;
        }

        public Tuple<Point3d, double> GetPointConstraint(int idx)
        {
            try
            {
                return Tuple.Create(_pts[idx], _ptsSize[idx]);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public Tuple<Curve, double> GetCurveConstraint(int idx)
        {
            try
            {
                return Tuple.Create(_crv[idx], _crvSize[idx]);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region GH_methods
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

        public override string ToString()
        {
            return "IGmshSolverOptions";
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
            if (typeof(T).IsAssignableFrom(typeof(IguanaGmshConstraintCollector)))
            {
                target = default(T);
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
