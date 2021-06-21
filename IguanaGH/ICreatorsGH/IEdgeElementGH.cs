using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace IguanaMeshGH.ICreatorsGH
{
    public class IEdgeElementGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IEdgeElementGH class.
        /// </summary>
        public IEdgeElementGH()
          : base("iEdgeElement", "iEdgeElement",
              "A one-dimensional edge element.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("First", "N1", "First vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Second", "N2", "Second vertex.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iElement", "iE", "Iguana element", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int N1 = 0, N2 = 0;
            DA.GetData(0, ref N1);
            DA.GetData(1, ref N2);


            IBarElement e = new IBarElement(N1, N2);

            DA.SetData(0, e);
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
            get { return new Guid("6dd1dcf5-6348-4cec-8fab-ce53273dcf9e"); }
        }
    }
}