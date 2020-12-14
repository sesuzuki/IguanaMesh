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
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;
using Iguana.IguanaMesh;

namespace IguanaMeshGH.ICreators
{
    public class IMesh2DFromPipeGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMesh2DFromPipeGH class.
        /// </summary>
        public IMesh2DFromPipeGH()
          : base("iPipe2D", "iPipe2D",
              "Create a mesh by extruding a curve along another curve.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Profile Curve", "Profile", "Profile curve.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Extrusion Curve", "Extrusion", "Extrusion curve.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iMeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iConstraints", "iConstraints", "Geometric constraints for mesh generation.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.list);
            pManager.AddGenericParameter("iSettings", "iSettings", "Shell mesh settings.", GH_ParamAccess.item);
            pManager[2].Optional = true;
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
            ISolver2D solver = new ISolver2D();
            IField field = null;
            Curve profCrv = null, extrCrv = null;
            List<IConstraint> constraints = new List<IConstraint>();
            List<ITransfinite> transfinites = new List<ITransfinite>();

            DA.GetData(0, ref profCrv);
            DA.GetData(1, ref extrCrv);
            DA.GetData(2, ref field);
            DA.GetDataList(3, constraints);
            DA.GetDataList(4, transfinites);
            DA.GetData(5, ref solver);

            string logInfo;
            GH_Structure<IEntityInfo> entities;
            IMesh mesh = IKernel.IMeshingKernel.CreateShellMeshFromPipe(profCrv, extrCrv, solver, out logInfo, out entities, constraints, transfinites, field);

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
                return Properties.Resources.iPipe;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a878cd2b-e67e-47d8-9336-2a2765d073c0"); }
        }
    }
}