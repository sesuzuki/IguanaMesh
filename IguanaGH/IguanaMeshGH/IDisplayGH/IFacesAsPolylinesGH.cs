using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IDisplay
{
    public class IFacesAsPolylinesGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AHF_IFacesAsPolylines class.
        /// </summary>
        public IFacesAsPolylinesGH()
          : base("iElementsAsPolylines", "iE-2D",
              "Retrieve two-dimensional elements as polylines.",
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
            pManager.AddGenericParameter("iElements as Polylines.", "C", "Two-dimensional elements as closed poylines", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = null;
            DA.GetData(0, ref mesh);

            List<Polyline> faces = IRhinoGeometry.GetFacesAsPolylines(mesh);

            DA.SetDataList(0, faces);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.AHF_IFaceAsPolyline;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("cfb5834b-bb4d-4183-a543-c43c41807b70"); }
        }
    }
}