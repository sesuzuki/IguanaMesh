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
using Iguana.IguanaMesh;
using Rhino.Geometry;

namespace IguanaMeshGH.IPrimitives
{
    public class ITorus3DGH : GH_Component
    {
        Point3d pt = new Point3d();
        double r1 = 1, r2 = 0.5, ang = 360;

        /// <summary>
        /// Initializes a new instance of the ITorus3DGH class.
        /// </summary>
        public ITorus3DGH()
          : base("iTorus3D", "iTorus3D",
              "Construct a volume mesh torus",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Center Point", "Pt", "Center Point.", GH_ParamAccess.item, pt);
            pManager.AddNumberParameter("Radius1", "R1", "Parameter for the semi major axis.", GH_ParamAccess.item, r1);
            pManager.AddNumberParameter("Radius2", "R2", "Parameter for the semi minor axis.", GH_ParamAccess.item, r2);
            pManager.AddNumberParameter("Angle", "Angle", "Defines the angular opening in degrees (from 0 to 360).", GH_ParamAccess.item, ang);
            pManager.AddGenericParameter("iSettings", "iSettings", "Volume mesh settings.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana volume mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ISolver3D solver = new ISolver3D();

            DA.GetData(0, ref pt);
            DA.GetData(1, ref r1);
            DA.GetData(2, ref r2);
            DA.GetData(3, ref ang);
            DA.GetData(4, ref solver);

            IMesh mesh = IKernel.IMeshingKernel.CreateVolumeMeshFromTorus(pt, r1, r2, ang, solver);

            DA.SetData(0, mesh);
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
                return Properties.Resources.iTorus3d;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9617528e-1e99-47b0-a6a8-57b0f0e486aa"); }
        }
    }
}