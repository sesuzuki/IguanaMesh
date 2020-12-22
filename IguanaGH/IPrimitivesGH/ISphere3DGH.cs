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
    public class ISphere3DGH : GH_Component
    {
        Point3d p = new Point3d();
        double r = 1;
        /// <summary>
        /// Initializes a new instance of the ISphere3DGH class.
        /// </summary>
        public ISphere3DGH()
          : base("iSphere3D", "iSphere3D",
              "Construct a volume mesh sphere",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Pt", "Base center point of the sphere", GH_ParamAccess.item, p);
            pManager.AddNumberParameter("Radius","Radius","Radius of the sphere",GH_ParamAccess.item, r);
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
            DA.GetData(0, ref p);
            DA.GetData(1, ref r);
            DA.GetData(2, ref solver);

            IMesh mesh = IKernel.IMeshingKernel.CreateVolumeMeshFromSphere(p, r, solver);

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
                return Properties.Resources.iSphere3d;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("de7bfdef-0962-4b0d-a045-771574628564"); }
        }
    }
}