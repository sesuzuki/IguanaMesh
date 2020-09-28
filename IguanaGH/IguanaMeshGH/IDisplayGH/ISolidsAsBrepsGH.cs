using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IDisplay
{
    public class ISolidsAsBrepsGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ISolidsAsBrepsGH class.
        /// </summary>
        public ISolidsAsBrepsGH()
          : base("iElementsAsBReps", "iE-3D",
              "Retrieve three-dimensional elements as BReps.",
              "Iguana", "Display")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "The Iguana mesh to extract three-dimesional elements.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iElements as BReps.", "B", "Three-dimensional elements as BReps", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = null;
            DA.GetData(0, ref mesh);

            List<Brep> solids = IRhinoGeometry.GetSolidsAsBrep(mesh);

            /*GH_Structure<GH_Curve> solids = new GH_Structure<GH_Curve>();
            Dictionary<int, List<PolylineCurve>> crv = IRhinoGeometry.GetSolidsAsPoly(mesh);
            foreach(int k in crv.Keys)
            {
                GH_Path path = new GH_Path(k);
                foreach(PolylineCurve poly in crv[k])
                {
                    GH_Curve c = new GH_Curve(poly);

                    solids.Append(c, path);
                }
            }*/

            DA.SetDataList(0, solids);
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
            get { return new Guid("a3ce945c-4fab-41cf-b315-b7d22a26fd81"); }
        }
    }
}