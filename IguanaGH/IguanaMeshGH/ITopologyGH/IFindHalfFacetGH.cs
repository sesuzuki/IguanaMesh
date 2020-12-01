using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITopologyGH
{
    public class IFindHalfFacetGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IFindHalfFacetGH class.
        /// </summary>
        public IFindHalfFacetGH()
          : base("iFindHalfFacet", "iFindHalfFacet",
              "Find the half-facet associated with the pair of keys",
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element", "e-Key", "Element key.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Half-Facet", "e-Key", "Local half-facet key.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Vertices", "v-Key", "Vertex keys.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Dimension", "Dim", "Topologic dimension of the half-facet.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            int eKey=0, hfKey=0;
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref eKey);
            DA.GetData(2, ref hfKey);

            IElement e = mesh.GetElementWithKey(eKey);
            int[] hf;
            e.GetHalfFacet(hfKey, out hf);
            int dim = 1;
            if (e.TopologicDimension == 3) dim = 2;

            DA.SetDataList(0, hf);
            DA.SetData(1, dim);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iHalfFacetFind;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("851f3431-a9f6-45a9-9c96-ef6b71a50790"); }
        }
    }
}