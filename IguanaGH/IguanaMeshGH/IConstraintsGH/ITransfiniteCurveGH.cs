using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IConstraintsGH
{
    public class ITransfiniteCurveGH : GH_Component
    {
        int count = 10;
        double coef = 1.0;
        TransCurve type = TransCurve.Progression;

        private enum TransCurve { Progression=0, Bump=1 }

        /// <summary>
        /// Initializes a new instance of the ITransfiniteCurveGH class.
        /// </summary>
        public ITransfiniteCurveGH()
          : base("iTransfiniteCurve", "iTransCrv",
              "Set a transfinite meshing constraint on the curve ID. Types are Progression (geometrical progression) and Bump (refinement toward both extremities of the curve)",
              "Iguana", "Constraints")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("CurveID", "ID", "ID of the curve.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("NodesNumber","N", "Number of nodes to be uniformly placed nodes on the curve. Default is " + count, GH_ParamAccess.item, count);
            pManager.AddNumberParameter("Coeficient", "Coef", "Geometrical progression with power Coef for node distribution when using Progression type. Default is " + coef, GH_ParamAccess.item, coef);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iTransfinite","iTransfinite", "Iguana constraint collector for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int tag = -1;
            DA.GetData(0, ref tag);
            DA.GetData(1, ref count);
            DA.GetData(2, ref coef);

            IguanaGmshTransfinite data = new IguanaGmshTransfinite();
            data.Dim = 1;
            data.Tag = tag;
            data.MethodType = type.ToString();
            data.NodesNumber = count;
            data.Coef = coef;

            DA.SetData(0, data);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("TransCurve", (int) type);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            int aIndex = -1;
            if (reader.TryGetInt32("TransCurve", ref aIndex))
            {
                type = (TransCurve)aIndex;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            foreach (TransCurve s in Enum.GetValues(typeof(TransCurve)))
                GH_Component.Menu_AppendItem(menu, s.ToString(), GetType, true, s == this.type).Tag = s;
            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void GetType(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is TransCurve)
            {
                this.type = (TransCurve)item.Tag;
                ExpireSolution(true);
            }
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
            get { return new Guid("c3678475-1fa6-48e3-9702-b8e183753406"); }
        }
    }
}