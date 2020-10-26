using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IConstraintsGH
{
    public class ITransfiniteSurfaceGH : GH_Component
    {
        private enum TransSurface { Left=0, Right=1, AlternateLeft=2, AlternateRight=3 }
        TransSurface type = TransSurface.Left;

        /// <summary>
        /// Initializes a new instance of the ITransfiniteSurfaceGH class.
        /// </summary>
        public ITransfiniteSurfaceGH()
          : base("iTransfiniteSurface", "iTransSrf",
              "Set a transfinite meshing constraint on the surface ID",
              "Iguana", "Constraints")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("SurfaceID", "ID", "ID of the surface.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Corners", "C", "Specify the (3 or 4) corners of the transfinite interpolation explicitly. This field is mandatory if the surface has more than 3 or 4 points on its boundary.", GH_ParamAccess.list);
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
            data.Dim = 2;
            data.Tag = tag;
            data.MethodType = type.ToString();
            data.Corners = corners.ToArray();

            DA.SetData(0, data);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("TransSurface", (int)type);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            int aIndex = -1;
            if (reader.TryGetInt32("TransSurface", ref aIndex))
            {
                type = (TransSurface)aIndex;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            foreach (TransSurface s in Enum.GetValues(typeof(TransSurface)))
                GH_Component.Menu_AppendItem(menu, s.ToString(), GetType, true, s == this.type).Tag = s;
            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void GetType(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is TransSurface)
            {
                this.type = (TransSurface)item.Tag;
                ExpireSolution(true);
            }
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
            get { return new Guid("e78885f1-7d50-4282-8d6f-d257ab360e19"); }
        }
    }
}