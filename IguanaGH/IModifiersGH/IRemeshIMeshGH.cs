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

namespace IguanaMeshGH.IModifiers
{
    public class IRemeshSurfaceMeshGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IRemeshGH class.
        /// </summary>
        public IRemeshSurfaceMeshGH()
          : base("iRemeshIMesh", "iRemeshIMesh",
              "Remesh an existing Iguana mesh",
              "Iguana", "Modifiers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iMeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iConstraints", "iConstraints", "Geometric constraints for mesh generation.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.list);
            pManager.AddGenericParameter("iSettings", "iSettings", "Shell/Volume mesh settings.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
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
            IMesh old = null;
            ISolver solver = null;
            IField field = null;
            List<IConstraint> constraints = new List<IConstraint>();
            List<ITransfinite> transfinite = new List<ITransfinite>();

            DA.GetData(0, ref old);
            DA.GetData(1, ref field);
            DA.GetDataList(2, constraints);
            DA.GetDataList(3, transfinite);
            DA.GetData(4, ref solver);

            string logInfo;
            GH_Structure<IEntityInfo> entities;
            IMesh mesh = new IMesh();
            if (solver.IsShellSolver)
            {
                mesh = IKernel.IMeshingKernel.RemeshShellMesh(old, (ISolver2D)solver, out logInfo, out entities, constraints, transfinite, field);
            }
            else
            {
                mesh = IKernel.IMeshingKernel.RemeshVolumeMesh(old, (ISolver3D)solver, out logInfo, out entities, constraints, transfinite, field);
            }

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
                return Properties.Resources.iRemesh;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("247e3397-9030-4fab-8e56-d6f628cc11e9"); }
        }
    }
}