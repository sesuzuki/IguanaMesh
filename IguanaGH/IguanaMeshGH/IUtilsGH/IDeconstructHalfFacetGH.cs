using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IDeconstructHalfFacetGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IDeconstructVertexKeysCollectionGH class.
        /// </summary>
        public IDeconstructHalfFacetGH()
          : base("iDeconstructHalfFacet", "iDeconstructHF",
              "Deconstruct half-facet.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("HalfFacet", "HF", "Base half-facet.", GH_ParamAccess.item);
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
            ITopologicHalfFacet hf = new ITopologicHalfFacet();
            DA.GetData(0, ref hf);
            int dim = 1;
            if (hf.Keys.Length > 2) dim = 2;
            DA.SetDataList(0, hf.Keys);
            DA.SetData(1, dim);
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
                return Properties.Resources.iDeconstructHalfFacet;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e7e73a37-cd7c-4708-9d0f-0bdb4e322ccc"); }
        }
    }
}