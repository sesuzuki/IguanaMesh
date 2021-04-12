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
using System.Collections.Generic;

namespace Iguana.IguanaMesh.ITypes
{
    public struct ITransfinite : IGH_Goo
    {
        public enum TransfiniteSurfaceType { Left = 0, Right = 1, AlternateLeft = 2, AlternateRight = 3 }
        public enum TransfiniteCurveType { Progression = 0, Bump = 1 }

        public int Dim { get; set; }
        public int Tag { get; set; }
        public int[] Corners { get; set; }
        public int NodesNumber { get; set; }
        public double Coef { get; set; }
        public string MethodType { get; set; }

        /// <summary>
        /// Transfinite surface constraint.
        /// </summary>
        /// <param name="tag"> ID of the underlying surface. </param>
        /// <param name="type"> Surface distribution type. </param>
        /// <param name="corners"> Specify the 3 or 4 corners of the transfinite interpolation explicitly. </param>
        /// <returns></returns>
        public static ITransfinite Surface(int tag, TransfiniteSurfaceType type, List<int> corners=default)
        {
            if (corners == default) corners = new List<int>();

            ITransfinite data = new ITransfinite();
            data.Dim = 2;
            data.Tag = tag;
            data.MethodType = type.ToString();
            data.Corners = corners.ToArray();

            return data;
        }

        /// <summary>
        /// Transfinite curve constraint.
        /// </summary>
        /// <param name="tag"> ID of the underlying curve. </param>
        /// <param name="type"> Curve distribution type. </param>
        /// <param name="nodesCount"> Number of nodes to be uniformly placed nodes on the curve. </param>
        /// <param name="coef"> Geometrical progression with power Coef for node distribution when using Progression type. </param>
        /// <returns></returns>
        public static ITransfinite Curve(int tag, TransfiniteCurveType type, int nodesCount, double coef)
        {
            ITransfinite data = new ITransfinite();
            data.Dim = 1;
            data.Tag = tag;
            data.MethodType = type.ToString();
            data.NodesNumber = nodesCount;
            data.Coef = coef;

            return data;
        }

        public static ITransfinite Volume(int tag, TransfiniteSurfaceType type)
        {
            ITransfinite data = new ITransfinite();
            data.Dim = 3;
            data.Tag = tag;
            data.MethodType = type.ToString();

            return data;
        }

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
            return "ITransfinite";
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
            if (typeof(T).IsAssignableFrom(typeof(ITransfinite)))
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
