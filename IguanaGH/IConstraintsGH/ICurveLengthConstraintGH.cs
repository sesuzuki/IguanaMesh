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
    public class ICurveLengthConstraintGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ILineConstraintGH class.
        /// </summary>
        public ICurveLengthConstraintGH()
          : base("iCurveLengthConstraint", "iCurveLengthConstraint",
              "Embed a curve divided by length to constraint mesh generation.",
              "Iguana", "Constraints")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Curve to use as a geometric constraint.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Size", "Size", "Target global mesh element size at the constraint curve.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "Length", "Length used to divide the curve.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iConstraint", "iConstraint", "Constraint for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crv = null;
            double size = 0;
            double length = 0;
            DA.GetData(0, ref crv);
            DA.GetData(1, ref size);
            DA.GetData(2, ref length);

            IConstraint constraints = new IConstraint(1, crv, size, -1, -1, 1, length);

            DA.SetData(0, constraints);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iCurveLengthConstraints;
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