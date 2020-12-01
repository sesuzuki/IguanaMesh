using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
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
          : base("iGradientField", "iGradF",
              "Compute the finite difference gradient of a Field: F = (Field(X + Delta/2) - Field(X - Delta/2))",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Field", "iF", "Field to evaluate.", GH_ParamAccess.item);
            pManager.AddNumberParameter("StepSize", "S", "Step size of finite differences. Default is "+delta, GH_ParamAccess.item, delta);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshField", "iF", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IguanaGmshField auxfield = null;
            DA.GetData(0, ref auxfield);
            DA.GetData(1, ref delta);

            IguanaGmshField.Gradient field = new IguanaGmshField.Gradient();
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