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
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Iguana.IguanaMesh;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaMeshGH.ICreators
{
    public class IMesh2DFromCurveExtrusion : GH_Component
    {
        Vector3d dir = new Vector3d(0, 0, 1);

        /// <summary>
        /// Initializes a new instance of the IMesh2DFromExtrusion class.
        /// </summary>
        public IMesh2DFromCurveExtrusion()
          : base("iExtrudeCurve", "iExtrudeCrv",
              "Create a mesh from a curve extrusion.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Base closed curve.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Direction", "Direction", "Direction.", GH_ParamAccess.item, dir);
            pManager.AddNumberParameter("Length", "Length", "Extrusion length.", GH_ParamAccess.item, 1);
            pManager.AddGenericParameter("MeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.list);
            pManager.AddGenericParameter("iSettings", "iSettings", "Shell mesh settings.", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana surface mesh.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Entities", "Entities", "Information about the underlying entities used for meshing.", GH_ParamAccess.tree);
            pManager.AddTextParameter("Info", "Info", "Log information about the meshing process.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crv = null;
            double length = 1;
            IField field = null;
            ISolver2D solver = new ISolver2D();
            List<ITransfinite> transfinite = new List<ITransfinite>();

            DA.GetData(0, ref crv);
            DA.GetData(1, ref dir);
            DA.GetData(2, ref length);
            DA.GetData(3, ref field);
            DA.GetDataList(4, transfinite);
            DA.GetData(5, ref solver);

            string logInfo;
            GH_Structure<IEntityInfo> entities;
            IMesh mesh = IKernel.IMeshingKernel.CreateShellMeshFromCurveExtrusion(crv, dir, length, solver, out logInfo, out entities, default, transfinite, field);

            DA.SetData(0, mesh);
            DA.SetDataTree(1, entities);
            DA.SetData(2, logInfo);
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
                return Properties.Resources.iExtrudeCurve;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7d121dfc-064f-4f74-a9dd-268c296d47be"); }
        }
    }
}