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

namespace IguanaMeshGH.IUtilsGH
{
    public class IDeconstructEntityGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IDeconstructEntityGH class.
        /// </summary>
        public IDeconstructEntityGH()
          : base("iDeconstructEntity", "iDecontructEntity",
              "Deconstruct information of the underlying entity used for meshing",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iEntity", "iEntity", "The underlying entity to be deconstructed.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("ID", "ID", "ID of the entity.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Dim", "Dim", "Dimension of the entity.", GH_ParamAccess.item);
            pManager.AddPointParameter("Position", "Position", "Reference position of the entity. For entities with dimension higher than 0 (curves, surfaces and volumes), reference position is the centroid of the entity.", GH_ParamAccess.item);
            pManager.AddBoxParameter("BoundingBox", "BoundingBox", "BoundingBox for entites with dimension higher than 0 (curves, surfaces and volumes).", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IEntityInfo info = new IEntityInfo();
            DA.GetData(0, ref info);

            BoundingBox bb = new BoundingBox();
            if (info.Dimension > 0) bb = info.GetBoundingBox();

            DA.SetData(0, info.Tag);
            DA.SetData(1, info.Dimension);
            DA.SetData(2, info.Position);
            DA.SetData(3, bb);
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
                return Properties.Resources.iDeconstructEntity;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("129b44a9-07cf-4de9-8ee1-1910b9613bdd"); }
        }
    }
}