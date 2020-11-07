using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITransformGH
{
    public class IRotateMeshGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ITwistGH class.
        /// </summary>
        public IRotateMeshGH()
          : base("iRotateMesh", "iRotate",
              "Rotate mesh",
              "Iguana", "Transform")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana Mesh.", GH_ParamAccess.item);
            pManager.AddLineParameter("Line", "Axis", "Rotation axis.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Angle", "Angle", "Rotation angle (Degrees).", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            Line ln = new Line();
            double angle = 0;
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref ln);
            DA.GetData(2, ref angle);

            Vector3d vec = ln.From - ln.To;
            IVector3D axis = new IVector3D(vec.X, vec.Y, vec.Z);
            IMesh dM = Iguana.IguanaMesh.IUtils.ITransform.Rotate(mesh, axis, angle);

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
            get { return new Guid("924a79f7-df5b-47b5-ab27-64d578327e05"); }
        }
    }
}