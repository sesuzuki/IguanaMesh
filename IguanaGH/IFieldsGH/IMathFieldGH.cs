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
    public class IMathFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMathFieldGH class.
        /// </summary>
        public IMathFieldGH()
          : base("iMathField", "iMathField",
              "Compute a field from a mathematical expression to specify the size of mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iFieldList", "iFieldList", "Fields to evaluate. If empty, the current field will be evaluated.", GH_ParamAccess.tree);
            pManager.AddTextParameter("Expression", "Expression", "Mathematical expression to evaluate. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and mathematical functions.\nThe expression is used to modulate all mesh element sizes.", GH_ParamAccess.item);
            pManager[0].Optional = true;
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
            string evalF = "";
            DA.GetData(1, ref evalF);

            List<IField> fields = new List<IField>();
            foreach (IGH_Goo data in Params.Input[0].VolatileData.AllData(true))
            {
                IField f;
                if (data.CastTo(out f))
                {
                    fields.Add(f);
                }
            }

            IField.MathEval field = new IField.MathEval();
            field.F = evalF;
            field.Fields = fields;

            DA.SetData(0, field);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iMathField;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c1284250-0b71-45c4-91b1-1ef7f94d0c3d"); }
        }
    }
}