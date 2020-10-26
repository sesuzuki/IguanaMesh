using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IMaxEigenHessianFieldGH : GH_Component
    {
        double delta = 0.1;

        /// <summary>
        /// Initializes a new instance of the IMaxEigenHessianGH class.
        /// </summary>
        public IMaxEigenHessianFieldGH()
          : base("iMaxEigenHessianField", "iMaxEHF",
              "Compute the maximum eigenvalue of the Hessian matrix of a Field, with the gradients evaluated by finite differences: F = max(eig(grad(grad(Field))))",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Field", "iF", "Field to evaluate.", GH_ParamAccess.item);
            pManager.AddNumberParameter("StepSize", "S", "Step size of finite differences. Default is " + delta, GH_ParamAccess.item, delta);
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

            IguanaGmshField.MaxEigenHessian field = new IguanaGmshField.MaxEigenHessian();
            field.IField = auxfield;
            field.Delta = delta;

            DA.SetData(0, field);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
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
            get { return new Guid("0a43a126-cb54-4fba-b6e6-58cff2b2ec9f"); }
        }
    }
}