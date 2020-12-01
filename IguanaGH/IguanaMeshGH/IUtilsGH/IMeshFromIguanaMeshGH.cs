using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IMeshFromIguanaMeshGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMeshFromIguanaMeshGH class.
        /// </summary>
        public IMeshFromIguanaMeshGH()
          : base("iRhinoMesh", "iRhinoMesh",
              "Retrieve a Rhino mesh representation. Surface meshes are triangulated when needed and Volume meshes are represented as surfaces meshes by taking their naked half-facets.",
              "Iguana", "Utils")
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
            pManager.AddGenericParameter("Mesh", "M", "Resulting Rhino mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh iM = new IMesh();
            DA.GetData(0, ref iM);

            Mesh mesh = IRhinoGeometry.TryGetRhinoMesh(iM);

            DA.SetData(0, mesh);
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
                return Properties.Resources.iRhinoMesh;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4dcfd01a-1088-4046-a6ab-05ca4be4f747"); }
        }
    }
}