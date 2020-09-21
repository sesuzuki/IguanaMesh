using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITopologyGH
{
    public class INakedEdgesGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AHF_NakedEdgesGH class.
        /// </summary>
        public INakedEdgesGH()
          : base("iNakedEdges", "iNE",
              "Retrieve the naked edges of an Array-Based Half-Facet (AHF) Mesh Data Structure",
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "The Iguana mesh to be extract naked edges.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Naked edges", "iNE", "Naked edges of the AHF-IMesh", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            DA.GetData(0, ref mesh);

            List<Tuple<int,int>> naked = mesh.Topology.GetNakedEdges();
            List<Line> edges = new List<Line>();
            foreach(Tuple<int,int> e in naked)
            {
                Point3d p1 = mesh.Vertices.GetVertexWithKey(e.Item1).RhinoPoint;
                Point3d p2 = mesh.Vertices.GetVertexWithKey(e.Item2).RhinoPoint;
                Line ln = new Line(p1, p2);
                edges.Add(ln);
            }

            DA.SetDataList(0, edges);
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
            get { return new Guid("11441796-abf5-4bdf-af6c-1fe2148e83b2"); }
        }
    }
}