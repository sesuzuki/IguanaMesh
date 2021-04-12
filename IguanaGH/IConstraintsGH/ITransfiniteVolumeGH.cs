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
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using static Iguana.IguanaMesh.ITypes.ITransfinite;

namespace IguanaMeshGH.IConstraints
{
    public class ITransfiniteVolumeGH : GH_Component
    {
        TransfiniteSurfaceType type = TransfiniteSurfaceType.Left;

        /// <summary>
        /// Initializes a new instance of the ITransfiniteVolumeGH class.
        /// </summary>
        public ITransfiniteVolumeGH()
          : base("iVolumeTransfinite", "iVolumeTransfinite",
              "Set a transfinite meshing constraint on a given volume of the underlying model.",
              "Iguana", "Constraints")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("VolumeID", "ID", "ID of the volume.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iTransfinite", "iTransfinite", "Transfinite constraint for mesh generation.", GH_ParamAccess.item);
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

            double[] cList = new double[corners.Count];
            for (int i = 0; i < corners.Count; i++) cList[i] = corners[i];

            ITransfinite data = new ITransfinite();
            data.Dim = 3;
            data.Tag = tag;
            data.MethodType = type.ToString();

            DA.SetData(0, data);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("type", (int)type);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            int aIndex = -1;
            if (reader.TryGetInt32("type", ref aIndex))
            {
                type = (TransfiniteSurfaceType)aIndex;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            foreach (TransfiniteSurfaceType s in Enum.GetValues(typeof(TransfiniteSurfaceType)))
                GH_Component.Menu_AppendItem(menu, s.ToString(), GetType, true, s == this.type).Tag = s;
            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void GetType(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is TransfiniteSurfaceType)
            {
                this.type = (TransfiniteSurfaceType)item.Tag;
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
                return Properties.Resources.iTransfiniteVolume;
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