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
    public class ICurvatureGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IGausCurvatureGH class.
        /// </summary>
        public ICurvatureGH()
          : base("iCurvature", "iCurvature",
              "Estimates the tensor of curvature at a given vertex (Only for two-dimensional meshes).\n" +
              "See: \"Taubin, Gabriel. (1995). Estimating the tensor of curvature of a surface from a polyhedralapproximation. Computer Vision, IEEE International Conference on. 902-907. 10.1109/ICCV.1995.466840.\"", 
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Vertex", "v-Key", "Vertex key.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("T1", "T1", "Maximum principal curvature direction.", GH_ParamAccess.item);
            pManager.AddVectorParameter("T2", "T2", "Minimum principal curvature direction.", GH_ParamAccess.item);
            pManager.AddNumberParameter("k1", "k1", "Maximum principal curvature value.", GH_ParamAccess.item);
            pManager.AddNumberParameter("k2", "k2", "Minimum principal curvature value.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mean", "Mean", "Mean curvature.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Gauss", "Gauss", "Gauss curvature.", GH_ParamAccess.item);
            pManager.AddTextParameter("Type", "Type", "Vertex type according to estimated curvature.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            int vKey = 0;
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref vKey);

            double k1, k2, gauss, mean;
            IVector3D T1, T2;

            mesh.Topology.ComputeVertexCurvatureTensor(vKey, out T1, out T2, out k1, out k2, out mean, out gauss);

            string type = "";
            if (k1 > 0 && k2 > 0) type = "Elliptic";
            else if (k1 > 0 && k2 == 0) type = "Parabolic";
            else if (k1 > 0 && k2 < 0) type = "Hyperbolic";
            else if (k1 == 0 && k2 > 0) type = "Parabolic";
            else if (k1 == 0 && k2 == 0) type = "Flat Umbilic";
            else if (k1 == 0 && k2 < 0) type = "Parabolic";
            else if (k1 < 0 && k2 > 0) type = "Hyperbolic";
            else if (k1 < 0 && k2 == 0) type = "Parabolic";
            else if (k1 < 0 && k2 < 0) type = "Elliptic";
            else if (k1 == k2) type = "Umbilic";

            DA.SetData(0, T1);
            DA.SetData(1, T2);
            DA.SetData(2, k1);
            DA.SetData(3, k2);
            DA.SetData(4, mean);
            DA.SetData(5, gauss);
            DA.SetData(6, type);
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
                return Properties.Resources.iCurvature;

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