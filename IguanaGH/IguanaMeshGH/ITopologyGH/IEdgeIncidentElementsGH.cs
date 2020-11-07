using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITopologyGH
{
    public class IEdgeIncidentElementsGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IEdgeIncidentElementsGH class.
        /// </summary>
        public IEdgeIncidentElementsGH()
          : base("iEdgeIncidentElements", "iEIE",
              "Retrieve all incident elements of a given edge.",
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iTopologicEdge", "iEdge", "Topologic edge.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("eKeys", "eKeys", "Keys of the elements.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            ITopologicEdge e = new ITopologicEdge();
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref e);

            int[] eIdx = mesh.Topology.GetEdgeIncidentElements(e.ID_Start, e.ID_End);

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
            get { return new Guid("4bdc15a2-8576-44b8-9a3e-50adc62fd968"); }
        }
    }
}