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
        private List<Tuple<Point3d, double>> data;

        public IguanaGmshConstraintCollector(List<Point3d> pts, List<double> factor1, List<PolylineCurve> poly=default, List<double> factor2=default)
        {
            data = new List<Tuple<Point3d, double>>();
            if (pts.Count == factor1.Count)
            {
                for (int i = 0; i < pts.Count; i++)
                {
                    data.Add(Tuple.Create(pts[i],factor1[i]));
                }
            }
            else
            {
                for (int i = 0; i < pts.Count; i++)
                {
                    data.Add(Tuple.Create(pts[i], factor1[0]));
                }
            }

            if (poly != default)
            {
                if (poly.Count == factor2.Count)
                {
                    for (int i = 0; i < poly.Count; i++)
                    {
                        PolylineCurve pl = poly[i];
                        double f = factor2[i];
                        int count = pl.PointCount;
                        if (pl.IsClosed) count -= 1;
                        for (int j = 0; j < count; j++) data.Add(Tuple.Create(pl.Point(j), f));
                    }
                }
                else
                {
                    double f = factor2[0];
                    for (int i = 0; i < poly.Count; i++)
                    {
                        PolylineCurve pl = poly[i];
                        int count = pl.PointCount;
                        if (pl.IsClosed) count -= 1;
                        for (int j = 0; j < count; j++) data.Add(Tuple.Create(pl.Point(j), f));
                    }
                }
            }
        }

        public bool HasConstraints()
        {
            if (data.Count!=0) return true;
            else return false;
        }

        public int GetPointConstraintCount()
        {
            return data.Count;
        }

        public Tuple<Point3d, double> GetConstraint(int idx)
        {
            return data[idx];
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
