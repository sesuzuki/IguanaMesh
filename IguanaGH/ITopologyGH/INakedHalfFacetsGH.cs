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
using System.Collections.Generic;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.ITopology
{
    public class INakedHalfFacetsGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the INakedHalfFacetsGH class.
        /// </summary>
        public INakedHalfFacetsGH()
          : base("iNakedHalfFacets", "iNakedHalfFacets",
              "Retrieve all the naked half-facets.",
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Vertex", "v-Key", "Vertices Keys", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Element", "e-Key", "Element Key.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Half-facet", "hf-Key", "Half-facets keys.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh iM = new IMesh();
            DA.GetData(0, ref iM);

            List<int> naked = new List<int>();
            List<int> elementsID = new List<int>();
            List<int> halfFacetsID = new List<int>();
            int[] hf;
            foreach (IElement e in iM.Elements)
            {
                for (int i = 1; i <= e.HalfFacetsCount; i++)
                {
                    if (e.IsNakedSiblingHalfFacet(i))
                    {
                        e.GetHalfFacet(i, out hf);
                        naked.AddRange(hf);
                        halfFacetsID.Add(i);
                        elementsID.Add(e.Key);
                    }
                }
            }

            DA.SetDataList(0, naked);
            DA.SetDataList(1, elementsID);
            DA.SetDataList(2, halfFacetsID);
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
                return Properties.Resources.iNakedHalfFacets;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4de9c3b5-aa66-47b0-9433-f096fe1c6bdb"); }
        }
    }
}