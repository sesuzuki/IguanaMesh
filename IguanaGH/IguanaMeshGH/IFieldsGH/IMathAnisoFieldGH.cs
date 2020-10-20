using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IMathAnisoFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMathAnisoFieldGH class.
        /// </summary>
        public IMathAnisoFieldGH()
          : base("iMathAnisoField", "iAMathF",
              "Anisotropic math field to specify the size of the mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Function at element 11", "M11", "Mathematical expression to evaluate at element 11 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions. Default is expression is F2 + Sin(z)", GH_ParamAccess.item);
            pManager.AddTextParameter("Function at element 12", "M12", "Mathematical expression to evaluate at element 12 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions. Default is expression is F2 + Sin(z)", GH_ParamAccess.item);
            pManager.AddTextParameter("Function at element 13", "M13", "Mathematical expression to evaluate at element 13 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions. Default is expression is F2 + Sin(z)", GH_ParamAccess.item);
            pManager.AddTextParameter("Function at element 22", "M22", "Mathematical expression to evaluate at element 22 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions. Default is expression is F2 + Sin(z)", GH_ParamAccess.item);
            pManager.AddTextParameter("Function at element 23", "M23", "Mathematical expression to evaluate at element 23 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions. Default is expression is F2 + Sin(z)", GH_ParamAccess.item);
            pManager.AddTextParameter("Function at element 33", "M33", "Mathematical expression to evaluate at element 33 of the metric tensor. The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions. Default is expression is F2 + Sin(z)", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshField", "iMF", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string m11 = "F2 + Sin(z)";
            string m12 = "F2 + Sin(z)";
            string m13 = "F2 + Sin(z)";
            string m22 = "F2 + Sin(z)";
            string m23 = "F2 + Sin(z)";
            string m33 = "F2 + Sin(z)";

            DA.GetData(0, ref m11);
            DA.GetData(1, ref m12);
            DA.GetData(2, ref m13);
            DA.GetData(3, ref m22);
            DA.GetData(4, ref m23);
            DA.GetData(5, ref m33);

            IguanaGmshField.MathEvalAniso field = new IguanaGmshField.MathEvalAniso();
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
            get { return new Guid("80f2a042-16aa-453c-ad05-18d07803c0cc"); }
        }
    }
}