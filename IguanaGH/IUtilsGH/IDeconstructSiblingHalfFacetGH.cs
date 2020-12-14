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

namespace IguanaMeshGH.IUtils
{
    public class IDeconstructSiblingHalfFacetGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IDeconstructSiblingHalfFacetGH class.
        /// </summary>
        public IDeconstructSiblingHalfFacetGH()
          : base("IDeconstructSiblingHalfFacet", "iDeconstructSHF",
              "Deconstruct sibling half-facet.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("SiblingHalfFacet", "SHF", "Base sibling half-facet.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Element", "e-Key", "Element key.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("HalfFacet", "hf-Key", "Half-facet key.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ITopologicSiblingHalfFacet shf = new ITopologicSiblingHalfFacet();
            DA.GetData(0, ref shf);

            DA.SetData(0, shf.ElementID);
            DA.SetData(1, shf.HalfFacetID);
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
                return Properties.Resources.iDeconstructHalfFacetSibling;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("762545ce-7de6-4515-9ddd-05342b47b3a8"); }
        }
    }
}