/*
 * <IguanaMesh>
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

using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaMeshGH.IConstraints
{
    public class IPointConstraintGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IConstraintCollectorGH class.
        /// </summary>
        public IPointConstraintGH()
          : base("iPointConstraint", "iPointConstraint",
              "Embed a point to constraint mesh generation.",
              "Iguana", "Constraints")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Pts", "Geometric constraint point.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Size", "Sizes", "Target global mesh element size.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iConstraint", "iConstraint", "Constraint for mesh generation.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> pts = new List<Point3d>();
            List<double> values = new List<double>();
            DA.GetDataList(0, pts);
            DA.GetDataList(1, values);

            IConstraint[] constraints = new IConstraint[pts.Count];
            double val = values[0];
            for (int i = 0; i < pts.Count; i++)
            {
                if (pts.Count == values.Count) val = values[i];
                constraints[i] = new IConstraint(0, pts[i], val, -1, -1);
            }

            DA.SetDataList(0, constraints);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iPointConstraints;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6b8276c2-fd3e-4a2a-939c-3644b8dfb953"); }
        }
    }
}