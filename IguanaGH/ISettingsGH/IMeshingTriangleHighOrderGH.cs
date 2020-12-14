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
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.ISettings
{
    public class IMeshingTriangleHighOrderGH : GH_Component
    {
        MeshSolvers2D solver = MeshSolvers2D.Automatic;
        ISolver2D solverOpt;
        double sizeFactor = 1.0, size = 1.0;
        int smoothingSteps = 10, ho_optimization = 0, minElemPerTwoPi = 6;
        bool adaptive = false;

        /// <summary>
        /// Initializes a new instance of the IMeshingTriangleHighOrderGH class.
        /// </summary>
        public IMeshingTriangleHighOrderGH()
          : base("iQuadraticTria", "iQuadraticTria",
              "Solver configuration for quadratic triangle-mesh generation.",
              "Iguana", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Size Factor", "SizeFactor", "Factor applied to all mesh element sizes. Default value is " + sizeFactor, GH_ParamAccess.item, sizeFactor);
            pManager.AddNumberParameter("Size", "Size", "Target size of mesh element. Default value is " + size, GH_ParamAccess.item, size);
            pManager.AddNumberParameter("Minimum Size", "MinSize", "Minimum mesh element size.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Maximun Size", "MaxSize", "Maximum mesh element size.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Curvature Adapt", "Adaptive", "Automatically compute mesh element sizes from curvature. It overrides the target global mesh element size at input nodes. Default value is " + adaptive.ToString(), GH_ParamAccess.item, adaptive);
            pManager.AddIntegerParameter("Mininimum Elements", "MinElements", "Minimum number of elements per 2PI. Default value is " + minElemPerTwoPi, GH_ParamAccess.item, minElemPerTwoPi);
            pManager.AddIntegerParameter("Smoothing Steps", "Smoothing", "Number of smoothing steps applied to the final mesh. Default value is " + smoothingSteps, GH_ParamAccess.item, smoothingSteps);
            pManager.AddIntegerParameter("Optimization", "Optimization", "Optimizatio method of high-order elements (0: None, 1: Optimization, 2: Elastic+optimization, 3: Elastic, 4: Fast-curving). Default value is " + ho_optimization, GH_ParamAccess.item, ho_optimization);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iSettings", "iSettings", "Solver configuration for quadratic triangle-mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            solverOpt = new ISolver2D();
            double maxSize=1, minSize=1;

            DA.GetData(0, ref sizeFactor);
            DA.GetData(1, ref size);
            DA.GetData(2, ref minSize);
            DA.GetData(3, ref maxSize);
            DA.GetData(4, ref adaptive);
            DA.GetData(5, ref minElemPerTwoPi);
            DA.GetData(6, ref smoothingSteps);
            DA.GetData(7, ref ho_optimization);

            solverOpt.MeshingAlgorithm = (int)solver;
            solverOpt.CharacteristicLengthFactor = sizeFactor;
            solverOpt.CharacteristicLengthMin = minSize;
            solverOpt.CharacteristicLengthMax = maxSize;
            solverOpt.CharacteristicLengthFromCurvature = adaptive;
            solverOpt.MinimumElementsPerTwoPi = minElemPerTwoPi;
            solverOpt.OptimizationSteps = smoothingSteps;
            solverOpt.HighOrderOptimize = ho_optimization;
            solverOpt.ElementOrder = 2;
            solverOpt.Size = size;

            DA.SetData(0, solverOpt);

            this.Message = "6Tria";
        }


        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("MeshSolvers2D", (int)solver);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            int aIndex = -1;
            if (reader.TryGetInt32("MeshSolvers2D", ref aIndex))
            {
                solver = (MeshSolvers2D)aIndex;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            foreach (MeshSolvers2D s in Enum.GetValues(typeof(MeshSolvers2D)))
                GH_Component.Menu_AppendItem(menu, s.ToString(), SolverType, true, s == this.solver).Tag = s;
            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void SolverType(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is MeshSolvers2D)
            {
                this.solver = (MeshSolvers2D)item.Tag;
                ExpireSolution(true);
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iTriasHighOrderSettings;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6d6b1366-d01b-45b7-8d81-901fd820caf3"); }
        }
    }
}