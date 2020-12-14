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
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh;
using Rhino.Geometry;
using Grasshopper.Kernel.Data;

namespace IguanaMeshGH.ICreators
{
    public class IMesh3DFromSurfaceExtrusionGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IVolumeMeshFromExtrusion class.
        /// </summary>
        public IMesh3DFromSurfaceExtrusionGH()
          : base("iExtrudeSurface", "iExtrudeSrf",
              "Extrude a suface along a vector to generate a three-dimensional mesh.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "Srf", "Base surface.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Direction", "Direction", "Direction.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "Length", "Extrusion length.", GH_ParamAccess.list, 1);
            pManager.AddIntegerParameter("Divisions", "Divisions", "Number of divisions per extrusion", GH_ParamAccess.list, 1);
            pManager.AddGenericParameter("iMeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iConstraints", "iConstraints", "Geometric constraints for mesh generation.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.list);
            pManager.AddGenericParameter("iSettings", "iSettings", "Volume meshing settings.", GH_ParamAccess.item);
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
            Brep b = null;
            Vector3d dir = new Vector3d();
            List<double> lengths = new List<double>();
            List<int> divisions = new List<int>();
            IField field = null;
            ISolver3D solver = new ISolver3D();
            List<IConstraint> constraints = new List<IConstraint>();
            List<ITransfinite> transfinites = new List<ITransfinite>();

            DA.GetData(0, ref b);
            DA.GetData(1, ref dir);
            DA.GetDataList(2, lengths);
            DA.GetDataList(3, divisions);
            DA.GetData(4, ref field);
            DA.GetDataList(5, constraints);
            DA.GetDataList(6, transfinites);
            DA.GetData(7, ref solver);

            // Extract required data from base surface
            if (!b.IsSolid && b.Faces.Count == 1)
            {
                string logInfo;
                GH_Structure<IEntityInfo> entities;
                IMesh mesh = IKernel.IMeshingKernel.CreateVolumeMeshFromSurfaceExtrusion(b, dir, divisions, lengths, solver, out logInfo, out entities, constraints, transfinites, field);

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
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iSurfaceExtrude;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("56d8270a-6fbe-49a2-bfd8-f7ac1b1f88b5"); }
        }
    }
}