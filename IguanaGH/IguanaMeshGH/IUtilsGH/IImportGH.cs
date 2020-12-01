using System;
using System.Collections.Generic;
using System.IO;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;
using static IguanaGH.IguanaMeshGH.IUtilsGH.IExportGH;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IImportGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IImportGH class.
        /// </summary>
        public IImportGH()
          : base("iImportMesh", "iImportMesh",
              "Import a mesh file.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("FilePath", "FilePath", "File path to export mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana surface mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string file = "";
            DA.GetData(0, ref file);
            string ext = Path.GetExtension(file);

            IMesh mesh = null;
            bool flag = false;
            foreach (string format in Enum.GetNames(typeof(MeshFormats)) )
            {
                string eval = format.Insert(0, ".");
                if (eval.Equals(ext))
                {
                    flag = true;
                    break;
                }
            }

            if (flag)
            {
                IguanaGmsh.Initialize();
                IguanaGmsh.Merge(file);
                Tuple<int, int>[] dimTags;
                IguanaGmsh.Model.GetEntities(out dimTags);
                mesh = IguanaGmshFactory.TryGetIMesh(dimTags[0].Item1);
                IguanaGmsh.FinalizeGmsh();
                DA.SetData(0, mesh);
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iImport;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9955a9bc-d403-4db2-988d-5a0bc4b7ce85"); }
        }
    }
}