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
    public class ILineConstraintGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ILineConstraintGH class.
        /// </summary>
        public ILineConstraintGH()
          : base("iCurveConstraint", "iCurveConstraint",
              "Embed a curve to constraint mesh generation.",
              "Iguana", "Constraints")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Curve to use as a geometric constraint.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Size", "Size", "Target global mesh element size at the constraint curve.", GH_ParamAccess.list);

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
            List<Curve> crv = new List<Curve>();
            List<double> sizes = new List<double>();
            DA.GetDataList(0, crv);
            DA.GetDataList(1, sizes);

            if (crv.Count == sizes.Count)
            {
                IConstraint[] constraints = new IConstraint[crv.Count];
                for (int i = 0; i < constraints.Length; i++)
                {
                    Polyline pl;
                    bool flag = crv[i].TryGetPolyline(out pl);
                    if (flag) constraints[i] = new IConstraint(1, pl, sizes[i], -1, -1);
                    else constraints[i] = new IConstraint(2, crv[i], sizes[i], -1, -1);
                }
                DA.SetDataList(0, constraints);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Curve count should equal to size count.");
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iCurveConstraints;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9f579201-4df0-41e5-bb3c-30f041e25eac"); }
        }
    }
}