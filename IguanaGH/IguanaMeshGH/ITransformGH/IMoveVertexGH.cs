using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITransformGH
{
    public class IMoveVertexGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMoveVertexGH class.
        /// </summary>
        public IMoveVertexGH()
          : base("iMoveVertex", "iMoveVertex",
              "Move the position of a vertex",
              "Iguana", "Transform")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Vertex", "v-Key", "Vertex key.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Vector", "T", "Translation vector.", GH_ParamAccess.item);
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
            Vector3d vec = new Vector3d();
            int vKey = 0;

            DA.GetData(0, ref mesh);
            DA.GetData(1, ref vKey);
            DA.GetData(2, ref vec);

            IMesh dM = mesh.DeepCopy();

            ITopologicVertex v = dM.Vertices.GetVertexWithKey(vKey);
            dM.Vertices.SetVertexPosition(vKey, v.Position + new IVector3D(vec.X, vec.Y, vec.Z));

            DA.SetData(0, dM);
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
            get { return new Guid("fb58c8dc-010b-4dc2-b78a-1443a2e6d82c"); }
        }
    }
}