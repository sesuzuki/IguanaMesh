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
    public class IGaussCurvatureGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IGausCurvatureGH class.
        /// </summary>
        public IGaussCurvatureGH()
          : base("iGaussCurvature", "iGauss",
              "Computes the discrete Gaussian curvature.",
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Gauss", "G", "Gaussian Curvature.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Vertex", "v-Key", "Vertices keys.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            DA.GetData(0, ref mesh);

            int[] vKeys = new int[mesh.VerticesCount];
            double[] gauss = new double[mesh.VerticesCount];
            int i = 0;
            foreach (int vK in mesh.VerticesKeys)
            {
                vKeys[i] = vK;
                gauss[i] = mesh.Topology.ComputesGaussianCurvature(vK);
                i++;
            }

            DA.SetDataList(0, gauss);
            DA.SetDataList(1, vKeys);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iGauss;

            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8410e8ff-7fbe-4030-9f35-461332959830"); }
        }
    }
}