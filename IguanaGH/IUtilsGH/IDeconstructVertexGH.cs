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
using Rhino.Geometry;

namespace IguanaMeshGH.IUtils
{
    public class IDeconstructVertexGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IDeconstructVertexGH class.
        /// </summary>
        public IDeconstructVertexGH()
          : base("IDeconstructVertex", "iDeconstructVertex",
              "Deconstruct Iguana vertex.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iVertex", "iVertex", "Base Iguana vertex.", GH_ParamAccess.item);        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Vertex", "v-Key", "´Vertex key.", GH_ParamAccess.item);
            pManager.AddPointParameter("Position", "Position", "Position.", GH_ParamAccess.item);
            pManager.AddPointParameter("TextureCoordinates", "Texture", "Texture coordinate.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element", "e-Key", "Element key to which this vertex is associated (One per dimension).", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Half-facet", "hf-Key", "Half-facet key to which this vertex is associated (One per dimension).", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ITopologicVertex vertex = new ITopologicVertex();
            DA.GetData(0, ref vertex);

            Point3d position = new Point3d(vertex.X, vertex.Y, vertex.Z);
            Point3d uvw = new Point3d(vertex.U, vertex.V, vertex.W );
            List<int> eID = new List<int>();
            List<int> hfID = new List<int>();
            for (int i=0; i<vertex.V2HF.Length; i++)
            {
                if(vertex.V2HF[i] != 0)
                {
                    eID.Add(vertex.GetElementID(i));
                    hfID.Add(vertex.GetHalfFacetID(i));
                }
            }

            DA.SetData(0, vertex.Key);
            DA.SetData(1, position);
            DA.SetData(2, uvw);
            DA.SetDataList(3, eID);
            DA.SetDataList(4, hfID);
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
                return Properties.Resources.iDeconstructVertex;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d901acf2-71b5-4d32-91d4-e58653ceca86"); }
        }
    }
}