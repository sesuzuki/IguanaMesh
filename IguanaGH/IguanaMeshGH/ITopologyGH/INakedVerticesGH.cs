using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITopologyGH
{
    public class INakedVerticesGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AHF_INakedVertices class.
        /// </summary>
        public INakedVerticesGH()
          : base("iNakedVertices", "iNV",
              "Retrieve the naked vertices of an Array-Based Half-Facet (AHF) Mesh Data Structure",
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "The Iguana mesh to extract naked edges.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Naked Vertices", "V", "Naked vertices of the AHF-IMesh", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Naked Vertices Keys", "iNV", "Naked vertices keys of the AHF-IMesh", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            DA.GetData(0, ref mesh);

            List<int> naked = mesh.Topology.GetNakedVertices();
            List<Point3d> vNaked = new List<Point3d>();

            foreach(int vK in naked)
            {
                vNaked.Add(mesh.Vertices.GetVertexWithKey(vK).RhinoPoint);
            }

            DA.SetDataList(0, vNaked);
            DA.SetDataList(1, naked);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.AHF_NakedVertices;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7cc2bf49-9338-4499-99ee-6e16055292d4"); }
        }
    }
}