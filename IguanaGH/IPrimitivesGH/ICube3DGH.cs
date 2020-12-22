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
    public class ICube3DGH : GH_Component
    {
        private int u = 5, v = 5, w = 5;

        /// <summary>
        /// Initializes a new instance of the ICube3DGH class.
        /// </summary>
        public ICube3DGH()
          : base("iCube3D", "iCube3D",
              "Construct a volume mesh cube",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("Base Box", "B", "Base box to construct the cube mesh.", GH_ParamAccess.item, new Box(Plane.WorldXY, new Interval(-1, 1), new Interval(-1, 1), new Interval(-1, 1)));
            pManager.AddIntegerParameter("U Count", "U", "Number of faces along the {x} direction.", GH_ParamAccess.item, u);
            pManager.AddIntegerParameter("V Count", "V", "Number of faces along the {y} direction.", GH_ParamAccess.item, v);
            pManager.AddIntegerParameter("W Count", "W", "Number of faces along the {z} direction.", GH_ParamAccess.item, w);
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
            Box box = new Box();
            ISolver3D solver = new ISolver3D();

            DA.GetData(0, ref box);
            DA.GetData(1, ref u);
            DA.GetData(2, ref v);
            DA.GetData(3, ref w);
            DA.GetData(4, ref solver);

            IMesh mesh = IKernel.IMeshingKernel.CreateVolumeMeshFromBox(box, u, v, w, solver);

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
                return Properties.Resources.iCube3d;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9bff3195-49d8-421a-88e4-d9133a62fc59"); }
        }
    }
}