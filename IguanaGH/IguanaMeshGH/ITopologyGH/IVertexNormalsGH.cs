using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITopologyGH
{
    public class IVertexNormalsGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IVertexNormalsGH class.
        /// </summary>
        public IVertexNormalsGH()
          : base("iVertexNormals", "iVNormals",
              "Computes all vertex normals.",
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Normals", "N", "Normals as vectors.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Vertices", "v-Keys", "Vertex keys.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            DA.GetData(0, ref mesh);

            Vector3d[] normals = new Vector3d[mesh.Vertices.Count];
            IVector3D v;
            int[] eKeys = new int[mesh.Vertices.Count];
            int idx = 0;
            foreach (int vK in mesh.VerticesKeys)
            {
                v = mesh.Topology.ComputeVertexNormal(vK);
                normals[idx] = new Vector3d(v.X,v.Y,v.Z);
                eKeys[idx] = vK;
                idx++;
            }

            DA.SetDataList(0, normals);
            DA.SetDataList(1, eKeys);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
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
            get { return new Guid("d251b3c1-271e-4743-982b-828866f2bca0"); }
        }
    }
}