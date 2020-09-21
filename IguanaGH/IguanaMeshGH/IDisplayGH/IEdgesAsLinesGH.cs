using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IDisplayGH
{
    public class IEdgesAsLinesGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IEdgesAsLinesGH class.
        /// </summary>
        public IEdgesAsLinesGH()
          : base("iEdgesAsLines", "iE-1D",
              "Retrieve one-dimensional elements as lines.",
              "Iguana", "Display")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "The Iguana mesh to extract faces.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("iElements as lines.", "L", "One-dimensional elements as lines", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = null;
            DA.GetData(0, ref mesh);

            List<Line> edges = IRhinoGeometry.GetEdgesAsLines(mesh);

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
            get { return new Guid("ed8a4c8a-be04-4cee-b3bd-43be341fc9e9"); }
        }
    }
}