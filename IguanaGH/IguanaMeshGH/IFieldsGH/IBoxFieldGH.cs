using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IBoxFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IBoxFieldGH class.
        /// </summary>
        public IBoxFieldGH()
          : base("iBoxField", "iBoxF",
              "Box field to specify the size of the mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("Box", "B", "Base box.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness", "T", "Thickness of a transition layer outside the ball. Default value is 1.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Size Inside", "SI", "Element sizes inside the sphere. Default value is 1.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Size Outside", "SE", "Element sizes outside the sphere. Default value is 1.", GH_ParamAccess.item, 1);
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
            Box b= new Box();
            double Thickness = 1;
            double VIn = 1;
            double VOut = 1;

            DA.GetData(0, ref b);
            DA.GetData(1, ref Thickness);
            DA.GetData(2, ref VIn);
            DA.GetData(3, ref VOut);


            IguanaGmshField.Box field = new IguanaGmshField.Box();
            field.Thickness= Thickness;
            field.VIn = VIn;
            field.VOut = VOut;
            field.XMax = b.X.Max;
            field.XMin = b.X.Min;
            field.YMax = b.Y.Max;
            field.YMin = b.Y.Min;
            field.ZMax = b.Z.Max;
            field.ZMin = b.Z.Min;

            DA.SetData(0, field);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iBoxField;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("85019ee5-c915-4364-8676-cfa54b5b8af4"); }
        }
    }
}