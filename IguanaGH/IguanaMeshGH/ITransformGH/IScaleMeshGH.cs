using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types.Transforms;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITransformGH
{
    public class IScaleMeshGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IScaleMeshGH class.
        /// </summary>
        public IScaleMeshGH()
          : base("iScaleMesh", "iScaleMesh",
              "Scale mesh",
              "Iguana", "Transform")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana Mesh.", GH_ParamAccess.item);
            pManager.AddNumberParameter("ScaleX", "X", "Scale factor along the X axis.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("ScaleY", "Y", "Scale factor along the Y axis.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("ScaleZ", "Z", "Scale factor along the Z axis.", GH_ParamAccess.item, 1);
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
            double x = 1, y = 1, z = 1;
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref x);
            DA.GetData(2, ref y);
            DA.GetData(3, ref z);

            IMesh dM = Iguana.IguanaMesh.IUtils.ITransform.Scale(mesh, x,y,z);

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
            get { return new Guid("f4486a65-8960-4f5e-95cf-b22f88d9f2cd"); }
        }
    }
}