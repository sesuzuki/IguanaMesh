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
    public class AHF_TorusGH : GH_Component
    {
        private IMesh mesh;

        /// <summary>
        /// Initializes a new instance of the AHF_TorusGH class.
        /// </summary>
        public AHF_TorusGH()
          : base("iTorus", "iTorus",
              "Creates a structured shell quad-mesh torus.",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("U Count", "U", "Number of faces along the {x} direction.", GH_ParamAccess.item, 30);
            pManager.AddIntegerParameter("V Count", "V", "Number of faces along the {y} direction.", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Semi major", "R1", "Parameter for the semi major axis.", GH_ParamAccess.item, 5.0);
            pManager.AddNumberParameter("Semi minor", "R2", "Parameter for the semi minor axis.", GH_ParamAccess.item, 2.0);
            pManager.AddIntervalParameter("Domain {x}", "D1", "Domain in the {x} direction. The domain should be bounded between 0 to 2π.", GH_ParamAccess.item, new Interval(0, 2 * Math.PI));
            pManager.AddIntervalParameter("Domain {y}", "D2", "Domain in the {y} direction. The domain should be bounded between 0 to 2π.", GH_ParamAccess.item, new Interval(0, 2 * Math.PI));
            pManager.AddPlaneParameter("Plane", "Pl", "Construction plane.", GH_ParamAccess.item, Plane.WorldXY);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
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
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane pl = new Plane();
            int u = 30;
            int v = 10;
            double R1 = 5;
            double R2 = 5;
            Interval D1 = new Interval(0, 2 * Math.PI);
            Interval D2 = new Interval(0, 2 * Math.PI);

            //Retreive vertices and elements
            DA.GetData(0, ref u);
            DA.GetData(1, ref v);
            DA.GetData(2, ref R1);
            DA.GetData(3, ref R2);
            DA.GetData(4, ref D1);
            DA.GetData(5, ref D2);
            DA.GetData(6, ref pl);


            //Build AHF-DataStructure
            mesh = IMeshCreator.CreateTorus(u, v, R1, R2, D1, D2, pl);

            DA.SetData(0, mesh);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iTorus;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("619d8415-8d2d-4cc1-8c11-b2ec76c020f5"); }
        }
    }
}