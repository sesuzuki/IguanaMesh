using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IBallFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IBallFieldGH class.
        /// </summary>
        public IBallFieldGH()
          : base("iBallField", "iBallF",
              "Ball field to specify the size of the mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Sphere", "S", "Base sphere.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness", "T", "Thickness of a transition layer outside the ball. Default value is 1.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Size Inside", "SI", "Element sizes inside the sphere. Default value is 1.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Size Outside", "SE", "Element sizes outside the sphere. Default value is 1.", GH_ParamAccess.item, 1);
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
            Surface srf = null;
            double Thickness = 1;
            double VIn = 1;
            double VOut = 1;

            DA.GetData(0, ref srf);
            DA.GetData(1, ref Thickness);
            DA.GetData(2, ref VIn);
            DA.GetData(3, ref VOut);

            if (srf.IsSphere())
            {
                Sphere s;
                srf.TryGetSphere(out s);
                IguanaGmshField.Ball field = new IguanaGmshField.Ball();
                field.XCenter = s.Center.X;
                field.YCenter = s.Center.Y;
                field.ZCenter = s.Center.Z;
                field.Radius = s.Radius;
                field.Thickness = Thickness;
                field.VIn = VIn;
                field.VOut = VOut;

                DA.SetData(0, field);
            }
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
            get { return new Guid("8ecf1f78-fcb8-4b49-851b-2011dd4fe362"); }
        }
    }
}