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
using Iguana.IguanaMesh.ICreators;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaMeshGH.IPrimitives
{
    public class ICube2DGH : GH_Component
    {
        private int u = 10, v = 10, w = 10;
        private Boolean weld = false;
        private double tolerance = 0.01;

        /// <summary>
        /// Initializes a new instance of the ICubeGH class.
        /// </summary>
        public ICube2DGH()
          : base("iCube", "iCube",
              "Construct a structured shell mesh cube.",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Base Plane", "B", "Base plane to construct the cube mesh.", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddIntervalParameter("DomainX", "X", "Domain of the box in the {x} direction.", GH_ParamAccess.item, new Interval(-1, 1));
            pManager.AddIntervalParameter("DomainY", "Y", "Domain of the box in the {y} direction.", GH_ParamAccess.item, new Interval(-1, 1));
            pManager.AddIntervalParameter("DomainZ", "Z", "Domain of the box in the {z} direction.", GH_ParamAccess.item, new Interval(-1, 1));
            pManager.AddIntegerParameter("U Count", "U", "Number of faces along the {x} direction.", GH_ParamAccess.item, u);
            pManager.AddIntegerParameter("V Count", "V", "Number of faces along the {y} direction.", GH_ParamAccess.item, v);
            pManager.AddIntegerParameter("W Count", "W", "Number of faces along the {z} direction.", GH_ParamAccess.item, w);
            pManager.AddBooleanParameter("Weld", "Weld", "Weld creases in the mesh.", GH_ParamAccess.item, weld);
            pManager.AddNumberParameter("Tolerance", "t", "Welding tolerance (Vertices smaller than this tolerance will be merged)", GH_ParamAccess.item, tolerance);
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
            Plane pl = new Plane();
            Interval x = new Interval();
            Interval y = new Interval();
            Interval z = new Interval();

            DA.GetData(0, ref pl);
            DA.GetData(1, ref x);
            DA.GetData(2, ref y);
            DA.GetData(3, ref z);
            DA.GetData(4, ref u);
            DA.GetData(5, ref v);
            DA.GetData(6, ref w);
            DA.GetData(7, ref weld);
            DA.GetData(8, ref tolerance);

            IMesh mesh = IMeshCreator.CreateCube(pl, x, y, z, u, v, w, weld, tolerance);

            DA.SetData(0, mesh);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iCube2D;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("cdc195bc-2a8d-41a2-9b32-775f14ae1f01"); }
        }
    }
}