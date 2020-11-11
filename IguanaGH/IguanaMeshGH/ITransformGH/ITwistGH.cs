using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IModifiers;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITransformGH
{
    public class ITwistGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ITwistGH class.
        /// </summary>
        public ITwistGH()
          : base("iTwist", "iTwist",
              "Twist a mesh",
              "Iguana", "Transform")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddLineParameter("Line", "Line", "Base axis.", GH_ParamAccess.item);
            pManager.AddNumberParameter("AngleFactor", "AngleFactor", "Twisting angle factor.", GH_ParamAccess.item, 0.05);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            Line axis = new Line();
            double angle = 0.1;

            DA.GetData(0, ref mesh);
            DA.GetData(1, ref axis);
            DA.GetData(2, ref angle);

            IMesh dM = IModifier.Twist(mesh, axis, angle);

            DA.SetData(0, dM);
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
            get { return new Guid("b88c5041-ad92-48e1-8b8b-62996ba41dbe"); }
        }
    }
}