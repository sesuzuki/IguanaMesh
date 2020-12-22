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
using Rhino.Geometry;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ICreators;

namespace IguanaMeshGH.IPrimitives
{
    public class IPlaneGH : GH_Component
    {
        private IMesh mesh;
        /// <summary>
        /// Initializes a new instance of the AHF_Plane class.
        /// </summary>
        public IPlaneGH()
          : base("iPlane", "iPlane",
              "Creates a structured shell quad-mesh plane.",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddRectangleParameter("Boundary", "B", "Rectangle describing boundary of plane", GH_ParamAccess.item, new Rectangle3d(Plane.WorldXY, new Interval(-10,10), new Interval(-10,10)));
            pManager.AddIntegerParameter("Width Count", "W", "Number of faces along the {x} direction.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("Height Count", "H", "Number of faces along the {y} direction", GH_ParamAccess.item, 10);
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
            Rectangle3d b = new Rectangle3d();
            int u = 0;
            int v = 0;

            //Retreive vertices and elements
            DA.GetData(0, ref b);
            DA.GetData(1, ref u);
            DA.GetData(2, ref v);

            //Build AHF-DataStructure
            mesh = IMeshCreator.CreatePlane(u, v, b.Width, b.Height, b.Plane); 

            DA.SetData(0, mesh);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iPlane;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("67212277-6fd6-4843-9af1-f8de7b98deb9"); }
        }
    }
}