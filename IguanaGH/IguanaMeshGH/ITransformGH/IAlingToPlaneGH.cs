using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITransformGH
{
    public class IAlingToPlaneGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IAlingToPlaneGH class.
        /// </summary>
        public IAlingToPlaneGH()
          : base("iAlignToPlane", "iAlignMesh",
              "Align mesh to a given plane",
              "Iguana", "Transform")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana Mesh.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Plane", "Plane for aligning.", GH_ParamAccess.item);
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
            Plane target = new Plane();
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref target);

            Plane world = Plane.WorldXY;

            IMesh dM = mesh.DeepCopy();
            foreach (ITopologicVertex v in mesh.Vertices.VerticesValues)
            {
                Point3d p;
                target.RemapToPlaneSpace(v.RhinoPoint, out p);
                dM.Vertices.SetVertexPosition(v.Key, new IVector3D(p.X,p.Y,p.Z));
            }

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
            get { return new Guid("3c8d0adc-e5f6-407e-b119-3134bfa8f6d9"); }
        }
    }
}