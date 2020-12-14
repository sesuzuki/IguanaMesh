/*
 * <IguanaMesh>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.IConstraints
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
          : base("iCurveTransfinite", "iCurveTransfinite",
              "Set a transfinite meshing constraint on a given curve of the underlying model.",
              "Iguana", "Constraints")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("CurveID", "ID", "ID of the curve.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("NodesCount","NodesCount", "Number of nodes to be uniformly placed nodes on the curve. Default is " + count, GH_ParamAccess.item, count);
            pManager.AddNumberParameter("Coeficient", "Coeficient", "Geometrical progression with power Coef for node distribution when using Progression type. Default is " + coef, GH_ParamAccess.item, coef);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iTransfinite","iTransfinite", "Transfinite constraint for mesh generation.", GH_ParamAccess.item);
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

            ITransfinite data = new ITransfinite();
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
                return Properties.Resources.iTransfiniteCurve;
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