using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITopologyGH
{
    public class IVertexAdjacentVerticesGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IVertexAdjacentVertices class.
        /// </summary>
        public IVertexAdjacentVerticesGH()
          : base("iVertexAdjacentVertices", "iVAV",
              "Retrieve all adjacent vertices of a given vertex.",
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Vertex", "v-Key", "Vertex key.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Vertices", "v-Key", "Keys of adjacent vertices.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            int key = -1;
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref key);

            int[] eIdx = mesh.Topology.GetVertexAdjacentVertices(key);

            DA.SetDataList(0, eIdx);
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
            get { return new Guid("7716320a-9dd5-442b-8feb-3b82117dfed4"); }
        }
    }
}