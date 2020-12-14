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
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.IFields
{
    public class IParamFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IParamFieldGH class.
        /// </summary>
        public IParamFieldGH()
          : base("iParametricField", "iParametricField",
              "Computes a field in parametric coordinates to specify the size of mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iField", "iF", "Field to evaluate.", GH_ParamAccess.item);
            pManager.AddTextParameter("X", "X", "X component of parametric function.", GH_ParamAccess.item);
            pManager.AddTextParameter("Y", "Y", "Y component of parametric function.", GH_ParamAccess.item);
            pManager.AddTextParameter("Z", "Z", "Z component of parametric function.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iField", "iField", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string fx = "", fy = "", fz = "";
            DA.GetData(1, ref fx);
            DA.GetData(2, ref fy);
            DA.GetData(3, ref fz);

            IField auxField = null;
            DA.GetData(0, ref auxField);

            IField.Param field = new IField.Param();
            field.IField = auxField;
            field.FX = fx;
            field.FY = fy;
            field.FZ = fz;

            DA.SetData(0, field);
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iParamField;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6c854cb4-5f6c-497c-85d8-2a6fb91fddb2"); }
        }
    }
}