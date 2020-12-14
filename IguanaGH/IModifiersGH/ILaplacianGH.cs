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
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.IModifiers
{
    public class ILaplacianGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ILaplacianGH class.
        /// </summary>
        public ILaplacianGH()
          : base("iLaplacianSmooth", "iLaplacianSmooth",
              "Apply Laplacian smoothing.",
              "Iguana", "Modifiers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana Mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Steps", "Steps", "Subdivision steps.", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Naked", "Naked", "Smooth naked vertices.", GH_ParamAccess.item, true);
            pManager.AddIntegerParameter("Vertices", "v-Keys", "Vertices to exclude.", GH_ParamAccess.list);
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "The modified Iguana Mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh old = new IMesh();
            int step = 1;
            bool naked = true;
            List<int> exclude = new List<int>();
            DA.GetData(0, ref old);
            DA.GetData(1, ref step);
            DA.GetData(2, ref naked);
            DA.GetDataList(3, exclude);

            IMesh mesh = IModifier.LaplacianSmoother(old, step, naked, exclude);

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
                return Properties.Resources.iSmooth;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e4a63414-5a78-4300-85f1-619f69445f6a"); }
        }
    }
}