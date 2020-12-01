using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITopologyGH
{
    public class IEdgeNormalsGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IEdgeNormalsGH class.
        /// </summary>
        public IEdgeNormalsGH()
          : base("iEdgeNormal", "iEdgeNormal",
              "Computes the normal of an edge",
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
            pManager.AddPointParameter("Centers", "C", "Element centers as points.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iEdges", "iEdges", "Iguana edges.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            DA.GetData(0, ref mesh);

            Vector3d[] normals;
            Point3d[] centers;
            ITopologicEdge[] edges;
            mesh.Topology.ComputeEdgeNormals(out normals, out centers, out edges);

            DA.SetDataList(0, normals);
            DA.SetDataList(1, centers);
            DA.SetDataList(2, edges);
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
                return Properties.Resources.iEdgeNormals;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("40f4ea76-2620-4426-8560-3accde2cc986"); }
        }
    }
}