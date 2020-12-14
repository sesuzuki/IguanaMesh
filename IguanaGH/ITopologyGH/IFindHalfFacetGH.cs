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

namespace IguanaMeshGH.ITopology
{
    public class IFindHalfFacetGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IFindHalfFacetGH class.
        /// </summary>
        public IFindHalfFacetGH()
          : base("iFindHalfFacet", "iFindHalfFacet",
              "Find the half-facet associated with the pair of keys",
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element", "e-Key", "Element key.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Half-Facet", "e-Key", "Local half-facet key.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Vertices", "v-Key", "Vertex keys.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Dimension", "Dim", "Topologic dimension of the half-facet.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            int eKey=0, hfKey=0;
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref eKey);
            DA.GetData(2, ref hfKey);

            IElement e = mesh.GetElementWithKey(eKey);
            int[] hf;
            e.GetHalfFacet(hfKey, out hf);
            int dim = 1;
            if (e.TopologicDimension == 3) dim = 2;

            DA.SetDataList(0, hf);
            DA.SetData(1, dim);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iHalfFacetFind;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("851f3431-a9f6-45a9-9c96-ef6b71a50790"); }
        }
    }
}