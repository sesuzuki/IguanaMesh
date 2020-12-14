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
    public class ICatmullClarkGH : GH_Component
    {
        bool flag = false;

        /// <summary>
        /// Initializes a new instance of the ICatmullClarkGH class.
        /// </summary>
        public ICatmullClarkGH()
          : base("iCatmullClark", "iCatmullClark",
              "Apply Catmull-Clark subdivision algorithm.",
              "Iguana", "Modifiers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana Mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("SubDivisions","SubD","Number of subdividing iterations.",GH_ParamAccess.item,1);
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

            if (iter > 1 && flag == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Subdivision level was lower from " + iter + " to 1. Enable 'Massive Subdivision'.");
                iter = 1;
            }

            int count = 0;
            IMesh mesh = new IMesh();
            while (count < iter)
            {
                mesh = ISubdividor.CatmullClark(old);
                old = mesh;
                count++;
            }

            DA.SetData(0, mesh);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("flag", flag);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            bool refFlag = false;
            if (reader.TryGetBoolean("flag", ref refFlag))
            {
                flag = refFlag;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            GH_Component.Menu_AppendItem(menu, "MassiveSubdivision", EnableMassiveSubdivision, true, flag = false);
            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void EnableMassiveSubdivision(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                if (flag) flag = false;
                else flag = true;
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
                return Properties.Resources.ICatmullClark;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f2691a0a-ce5e-4169-9474-4e0c13543081"); }
        }
    }
}