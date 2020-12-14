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

namespace IguanaMeshGH.ICreators
{
    public class IConstructMeshGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMesh class.
        /// </summary>
        public IConstructMeshGH()
          : base("iMeshConstructor", "iMeshConstructor",
              "Iguana mesh constructor from a list of topologic vertices and a list of Elements.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iVertices", "iVertices", "List of vertices.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iElements", "iElements", "List of elements.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();

            foreach (var obj in base.Params.Input[0].VolatileData.AllData(true))
            {
                ITopologicVertex v;
                obj.CastTo<ITopologicVertex>(out v);
                if (v != null) mesh.AddVertex(v.Key, v);
            }

            foreach (var obj in base.Params.Input[1].VolatileData.AllData(true))
            {
                IElement e;
                obj.CastTo<IElement>(out e);
                if (e != null) mesh.AddElement(e);
            }
            mesh.BuildTopology();

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
                return Properties.Resources.iConstructor;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2b1e3e0d-7b5c-49d9-835f-eda2a20050b9"); }
        }
    }
}