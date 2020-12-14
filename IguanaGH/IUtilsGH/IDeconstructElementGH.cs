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
    public class IDeconstructElementGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IDeconstructElementGH class.
        /// </summary>
        public IDeconstructElementGH()
          : base("iDeconstructElement", "iDeconstructElement",
              "Deconstruct Iguana element.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("iElement", "e-Key", "Element key.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Vertices", "v-Key", "Vertex keys.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Dimension", "Dim", "Topologic dimension of the element.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Half-facet", "hf-key", "Half-facets keys of the element. Half-facets are represent by vertices keys.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Sibling Half-facet", "shf-key", "Sibling half-facets keys of the element. The first item is the sibling element key and the second item is the sibiling half-facet key.", GH_ParamAccess.list);
            pManager.AddTextParameter("Type","Type","Type of element",GH_ParamAccess.item);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            int eKey = -1;
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref eKey);

            IElement e = mesh.GetElementWithKey(eKey);
            ITopologicHalfFacet[] hf = new ITopologicHalfFacet[e.HalfFacetsCount];
            ITopologicSiblingHalfFacet[] shf = new ITopologicSiblingHalfFacet[e.HalfFacetsCount];
            for (int i=0; i<e.HalfFacetsCount; i++)
            {
                int[] data;
                e.GetHalfFacet(i + 1, out data);
                hf[i] = new ITopologicHalfFacet(data);
                shf[i] = new ITopologicSiblingHalfFacet(e.GetSiblingElementID(i + 1), e.GetSiblingHalfFacetID(i + 1));
            }

            DA.SetDataList(0, e.Vertices);
            DA.SetData(1, e.TopologicDimension);
            DA.SetDataList(2, hf);
            DA.SetDataList(3, shf);
            DA.SetData(4, e.ToString());
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iDeconstructElement;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("63dd9b7c-89ba-4b41-b525-1855e7aa0213"); }
        }
    }
}