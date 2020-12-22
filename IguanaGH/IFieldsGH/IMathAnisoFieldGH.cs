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
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.IFields
{
    public class IMathAnisoFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMathAnisoFieldGH class.
        /// </summary>
        public IMathAnisoFieldGH()
          : base("iAnisoMathField", "iAnisoMathField",
              "Compute an anisotropic math field to specify the size of mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iFieldList", "iFieldList", "List of fields to evaluate.", GH_ParamAccess.tree);
            pManager.AddTextParameter("M11", "M11", "Mathematical expression to evaluate at element 11 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions.", GH_ParamAccess.item);
            pManager.AddTextParameter("M12", "M12", "Mathematical expression to evaluate at element 12 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions.", GH_ParamAccess.item);
            pManager.AddTextParameter("M13", "M13", "Mathematical expression to evaluate at element 13 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions.", GH_ParamAccess.item);
            pManager.AddTextParameter("M22", "M22", "Mathematical expression to evaluate at element 22 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions.", GH_ParamAccess.item);
            pManager.AddTextParameter("M23", "M23", "Mathematical expression to evaluate at element 23 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions.", GH_ParamAccess.item);
            pManager.AddTextParameter("M33", "M33", "Mathematical expression to evaluate at element 33 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
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
            string m11 = "";
            string m12 = "";
            string m13 = "";
            string m22 = "";
            string m23 = "";
            string m33 = "";

            List<IField> fields = new List<IField>();
            foreach (IGH_Goo data in Params.Input[0].VolatileData.AllData(true))
            {
                IField f;
                if (data.CastTo(out f))
                {
                    fields.Add(f);
                }
            }

            DA.GetData(1, ref m11);
            DA.GetData(2, ref m12);
            DA.GetData(3, ref m13);
            DA.GetData(4, ref m22);
            DA.GetData(5, ref m23);
            DA.GetData(6, ref m33);

            IField.MathEvalAniso field = new IField.MathEvalAniso();
            field.m11 = m11;
            field.m12 = m12;
            field.m13 = m13;
            field.m22 = m22;
            field.m23 = m23;
            field.m33 = m33;

            DA.SetData(0, field);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iMathAnisoField;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("80f2a042-16aa-453c-ad05-18d07803c0cc"); }
        }
    }
}