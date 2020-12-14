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
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.IFields
{
    public class IGradientFieldGH : GH_Component
    {
        double delta = 0.1;
        Component kind = 0;
        enum Component { Eval_X = 0, Eval_Y = 1, Eval_Z = 2, Eval_Norm = 3 };


        /// <summary>
        /// Initializes a new instance of the IGradientFieldGH class.
        /// </summary>
        public IGradientFieldGH()
          : base("iGradientField", "iGradientField",
              "Compute the gradient of a field to specify the size of mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iField", "iF", "Field to evaluate.", GH_ParamAccess.item);
            pManager.AddNumberParameter("StepSize", "StepSize", "Step size. Default is "+delta, GH_ParamAccess.item, delta);
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
            IField auxfield = null;
            DA.GetData(0, ref auxfield);
            DA.GetData(1, ref delta);

            IField.Gradient field = new IField.Gradient();
            field.IField = auxfield;
            field.Delta = delta;
            field.Kind = (int) kind;

            DA.SetData(0, field);

            this.Message = kind.ToString();
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("Component", (int) kind);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            int aIndex = -1;
            if (reader.TryGetInt32("Component", ref aIndex))
            {
                kind = (Component) aIndex;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            foreach (Component c in Enum.GetValues(typeof(Component)))
                GH_Component.Menu_AppendItem(menu, c.ToString(), ComponentType, true, c == this.kind).Tag = c;
            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void ComponentType(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is Component)
            {
                kind = (Component) item.Tag;
                ExpireSolution(true);
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iGradientField;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("70e87d46-73db-4915-be60-8a17ef38e9ba"); }
        }
    }
}