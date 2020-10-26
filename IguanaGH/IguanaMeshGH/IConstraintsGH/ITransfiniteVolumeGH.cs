using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IConstraintsGH
{
    public class ITransfiniteVolumeGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ITransfiniteVolumeGH class.
        /// </summary>
        public ITransfiniteVolumeGH()
          : base("iTransfiniteVolume", "iTransVol",
              "Set a transfinite meshing constraint on the volume ID",
              "Iguana", "Constraints")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("VolumeID", "ID", "ID of the volume.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Corners", "C", "Specify the 6 or 8 corners of the transfinite interpolation explicitly.", GH_ParamAccess.list);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iTransfinite", "iTransfinite", "Iguana constraint collector for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int tag = -1;
            List<int> corners = new List<int>();
            DA.GetData(0, ref tag);
            DA.GetDataList(1, corners);

            double[] cList = new double[corners.Count];
            for (int i = 0; i < corners.Count; i++) cList[i] = corners[i];

            IguanaGmshTransfinite data = new IguanaGmshTransfinite();
            data.Dim = 3;
            data.Tag = tag;
            data.Corners = corners.ToArray();

            DA.SetData(0, data);
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
            get { return new Guid("7c48a92b-4d65-476e-8c00-431386e7250e"); }
        }
    }
}