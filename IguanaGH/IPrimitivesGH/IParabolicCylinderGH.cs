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
    public class IParabolicCylinderGH : GH_Component
    {
        private IMesh mesh;

        /// <summary>
        /// Initializes a new instance of the IParabolicCylinderGH class.
        /// </summary>
        public IParabolicCylinderGH()
          : base("iParabolicCylinder", "iParaCyl",
              "Creates a parabolic cylinder quad-mesh.",
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
            pManager.AddNumberParameter("Param a", "a", "Constant parabolic parameter.", GH_ParamAccess.item, 1.0);
            pManager.AddIntervalParameter("Domain {x}", "D1", "Domain in the {x} direction. The domain should be bounded between -∞ to +∞.", GH_ParamAccess.item, new Interval(-1, 1));
            pManager.AddIntervalParameter("Domain {y}", "D2", "Domain in the {y} direction. The domain should be bounded between -∞ to +∞.", GH_ParamAccess.item, new Interval(-1, 1));
            pManager.AddPlaneParameter("Plane", "Pl", "Construction plane.", GH_ParamAccess.item, Plane.WorldXY);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
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
            double a = 1;
            Interval D1 = new Interval(-1, 1);
            Interval D2 = new Interval(-1, 1);

            //Retreive vertices and elements
            DA.GetData(0, ref u);
            DA.GetData(1, ref v);
            DA.GetData(2, ref a);
            DA.GetData(3, ref D1);
            DA.GetData(4, ref D2);
            DA.GetData(5, ref pl);


            //Build AHF-DataStructure
            mesh = IMeshCreator.CreateParabolicCylinder(u, v, a, D1, D2, pl);

            DA.SetData(0, mesh);
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
                return Properties.Resources.iParabolic;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("adcc58e2-fea1-4005-8ca7-9e058fb7e70d"); }
        }
    }
}