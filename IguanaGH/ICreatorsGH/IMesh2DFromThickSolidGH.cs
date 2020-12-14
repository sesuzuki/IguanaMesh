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
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;
using System.Collections.Generic;
using Grasshopper.Kernel.Data;
using Iguana.IguanaMesh;

namespace IguanaMeshGH.ICreators
{
    public class IMesh2DFromThickSolidGH : GH_Component
    {
        double offset = 0.2;
        bool cut = false;

        /// <summary>
        /// Initializes a new instance of the IMesh2DFromHollowedVolumesGH class.
        /// </summary>
        public IMesh2DFromThickSolidGH()
          : base("iThickMesh2D", "iThickMesh2D",
              "Create a two-dimensional mesh from a brep.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "Brep", "Base brep.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Offset", "Offset", "Offset distance", GH_ParamAccess.item, offset);
            pManager.AddBooleanParameter("Cut", "Cut", "Enable boolean difference.", GH_ParamAccess.item, cut);
            pManager.AddIntegerParameter("Exclude", "Exclude", "IDs of two-dimensional entities to exclude form the meshing process.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iMeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iConstraints", "iConstraints", "Geometric constraints for mesh generation.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.list);
            pManager.AddGenericParameter("iSettings", "iSettings", "Shell mesh settings.", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
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
            Brep geom = null;
            ISolver2D solver = new ISolver2D();
            List<int> excludeSrfTag = new List<int>();
            IField field = null;
            List<IConstraint> constraints = new List<IConstraint>();
            List<ITransfinite> transfinites = new List<ITransfinite>();

            DA.GetData(0, ref geom);
            DA.GetData(1, ref offset);
            DA.GetData(2, ref cut);
            DA.GetDataList(3, excludeSrfTag);
            DA.GetData(4, ref field);
            DA.GetDataList(5, constraints);
            DA.GetDataList(6, transfinites);
            DA.GetData(7, ref solver);

            if (geom.IsSolid)
            {
                string logInfo;
                GH_Structure<IEntityInfo> entities;
                IMesh mesh = IKernel.IMeshingKernel.CreateShellMeshFromThickSolid(geom, excludeSrfTag, offset, cut, solver, out logInfo, out entities, constraints, transfinites, field);

                DA.SetData(0, mesh);
                DA.SetDataTree(1, entities);
                DA.SetData(2, logInfo);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Brep should be closed.");
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
                return Properties.Resources.iThick2D;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f64cb3e5-33d4-4cd2-9956-63fa923f2599"); }
        }
    }
}