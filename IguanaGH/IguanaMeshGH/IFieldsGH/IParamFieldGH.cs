using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ISettingsGH
{
    public class IParamFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IParamFieldGH class.
        /// </summary>
        public IParamFieldGH()
          : base("iParamField", "iParamF",
              "Evaluates a Field in parametric coordinates: F = Field(FX, FY, FZ)",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Field", "iF", "Field to evaluate.", GH_ParamAccess.item);
            pManager.AddTextParameter("FX", "FX", "X component of parametric function.", GH_ParamAccess.item);
            pManager.AddTextParameter("FY", "FY", "Y component of parametric function.", GH_ParamAccess.item);
            pManager.AddTextParameter("FZ", "FZ", "Z component of parametric function.", GH_ParamAccess.item);
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
            string fx = "", fy = "", fz = "";
            DA.GetData(1, ref fx);
            DA.GetData(2, ref fy);
            DA.GetData(3, ref fz);

            IguanaGmshField auxField = null;
            DA.GetData(0, ref auxField);

            IguanaGmshField.Param field = new IguanaGmshField.Param();
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
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
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