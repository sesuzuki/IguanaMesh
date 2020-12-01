using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IModifiers;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITransformGH
{
    public class IMirrorMeshGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMirrorMeshGH class.
        /// </summary>
        public IMirrorMeshGH()
          : base("iMirror", "iMirror",
              "Mirror a mesh",
              "Iguana", "Transform")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana Mesh.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Plane", "Reference plane for mirroring.", GH_ParamAccess.item);
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
            Plane pl = new Plane();
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref pl);

            Point3d o = pl.Origin;
            Vector3d n = pl.Normal;
            IPlane plane = new IPlane(o.X, o.Y, o.Z, n.X, n.Y, n.Z);
            IMesh dM = IModifier.Mirror(mesh, plane);

            DA.SetData(0, dM);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iMirror;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8858ccf5-6a2f-47bb-baa0-a9345408e4cc"); }
        }
    }
}