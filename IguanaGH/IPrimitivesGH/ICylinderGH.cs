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
    public class AHF_CylinderGH : GH_Component
    {
        private double r1 = 1, r2 = 1, h = 1;
        private int u = 30, v = 20;
        private IMesh mesh;

        /// <summary>
        /// Initializes a new instance of the AHF_CylinderGH class.
        /// </summary>
        public AHF_CylinderGH()
          : base("iCylinder", "iCylinder",
              "Construct a structured shell quad-mesh cylinder.",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Base Plane", "B", "Base plane to construct the cylinder.", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddIntegerParameter("U Count", "U", "Number of faces along the {x} direction.", GH_ParamAccess.item, u);
            pManager.AddIntegerParameter("V Count", "V", "Number of faces along the {y} direction.", GH_ParamAccess.item, v);
            pManager.AddNumberParameter("Lower Radius", "R1", "Lower radius of the cylinder.", GH_ParamAccess.item, r1);
            pManager.AddNumberParameter("Upper Radius", "R2", "Upper radius of the cylinder.", GH_ParamAccess.item, r2);
            pManager.AddNumberParameter("Height", "H", "Height of the tube.", GH_ParamAccess.item, h);
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
            mesh = new IMesh();

            DA.GetData(0, ref pl);
            DA.GetData(1, ref u);
            DA.GetData(2, ref v);
            DA.GetData(3, ref r1);
            DA.GetData(4, ref r2);
            DA.GetData(5, ref h);

            //Build AHF-DataStructure
            mesh = IMeshCreator.CreateCylinder(u, v, r1, r2, h, pl);

            DA.SetData(0, mesh);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iCylinder;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2add74aa-66d2-4a41-b1d9-8d112646c31c"); }
        }
    }
}