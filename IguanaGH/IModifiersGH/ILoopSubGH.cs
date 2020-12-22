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
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.IModifiers
{
    public class ILoopSubGH : GH_Component
    {
        bool _massiveSubd = false;

        /// <summary>
        /// Initializes a new instance of the ILoopSubGH class.
        /// </summary>
        public ILoopSubGH()
          : base("iLoopSubdivision", "iLoop",
              "Apply Loop subdivision algorithm.",
              "Iguana", "Modifiers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana Mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("SubDivisions", "SubD", "Number of subdividing iterations.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "The modified Iguana Mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh old = new IMesh();
            int iter = 1;
            DA.GetData(0, ref old);
            DA.GetData(1, ref iter);

            if(iter>1 && MassiveSubdivision == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Subdivision level was lower from " + iter + " to 1. For larger subdivision iterations, enable 'Massive Subdivision'.");               
                iter = 1;
            }

            int count = 0;
            IMesh mesh = new IMesh();

            while (count < iter)
            {
                mesh = ISubdividor.Loop(old);
                old = mesh;
                count++;
            }

            DA.SetData(0, mesh);
        }

        public bool MassiveSubdivision
        {
            get { return _massiveSubd; }
            set
            {
                _massiveSubd = value;
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Massive Subdivision", MassiveSubdivision);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            bool refFlag = false;
            if (reader.TryGetBoolean("Massive Subdivision", ref refFlag))
            {
                MassiveSubdivision = refFlag;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem item = Menu_AppendItem(menu, "Massive Subdivision", Menu_MassivePreviewClicked, true, MassiveSubdivision);
            item.ToolTipText = "CAUTION: When checked, disable the imposed limit of maximum subdivision iterations.\nThis might take a long time to compute.";
        }

        private void Menu_MassivePreviewClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("Massive Subdivision");
            MassiveSubdivision = !MassiveSubdivision;
            ExpireSolution(true);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.ILoop;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d34d8754-30a8-4726-ba4e-92f635fc34a8"); }
        }
    }
}