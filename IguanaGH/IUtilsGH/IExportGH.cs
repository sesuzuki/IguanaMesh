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
using System.IO;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.IUtils
{
    public class IExportGH : GH_Component
    {
        IKernel.IMeshingKernel.MeshFormats ext = IKernel.IMeshingKernel.MeshFormats.stl;
        /// <summary>
        /// Initializes a new instance of the IExportGH class.
        /// </summary>
        public IExportGH()
          : base("iExportMesh", "iExport",
              "Export Iguana mesh.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana Mesh.", GH_ParamAccess.item);
            pManager.AddTextParameter("FilePath", "FilePath", "File path to export mesh.", GH_ParamAccess.item);
            pManager.AddTextParameter("FileName", "FileName", "File name.", GH_ParamAccess.item, "MeshFile_" + DateTime.Today.ToShortDateString().Replace("/","_")); 
            pManager.AddBooleanParameter("SaveFile", "Save", "Save file", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("SavedFile", "SavedFile", "Export information.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            string path = "";
            string name = "";
            Boolean save = false;
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref path);
            DA.GetData(2, ref name);
            DA.GetData(3, ref save);

            name += "." + ext.ToString();
            string filePath = Path.GetDirectoryName(path) + '\\' + name;

            if (save)
            {

                IKernel.IMeshingKernel.ExportToFile(mesh, filePath);
            }

            DA.SetData(0, filePath);
            this.Message = name.ToString();
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("MeshFormats", (int) ext);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            int aIndex = -1;
            if (reader.TryGetInt32("MeshFormats", ref aIndex))
            {
                ext = (IKernel.IMeshingKernel.MeshFormats)aIndex;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            foreach (IKernel.IMeshingKernel.MeshFormats s in Enum.GetValues(typeof(IKernel.IMeshingKernel.MeshFormats)))
                GH_Component.Menu_AppendItem(menu, s.ToString(), SolverType, true, s == this.ext).Tag = s;
            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void SolverType(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is IKernel.IMeshingKernel.MeshFormats)
            {
                this.ext = (IKernel.IMeshingKernel.MeshFormats)item.Tag;
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
                return Properties.Resources.iExport;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d91a38df-a7f1-4b4f-b475-960febc2f3b1"); }
        }
    }
}