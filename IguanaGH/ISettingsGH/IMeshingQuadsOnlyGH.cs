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

using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using System;
using System.Windows.Forms;

namespace IguanaMeshGH.ISettings
{
    public class IMeshingQuadsOnlyGH : GH_Component
    {
        int smoothingSteps = 10, minElemPerTwoPi = 6;
        bool adaptive = false, massiveRefinement = false;

        /// <summary>
        /// Initializes a new instance of the IMeshingOptions2D class.
        /// </summary>
        public IMeshingQuadsOnlyGH()
          : base("iLinearQuad", "iLinearQuad",
              "Solver onfiguration for linear quad-mesh generation.",
              "Iguana", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Size", "Size", "Target size of mesh element. Default value is the average between the min. and max. size.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Minimum Size", "MinSize", "Minimum mesh element size.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Maximun Size", "MaxSize", "Maximum mesh element size.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Curvature Adapt", "Adaptive", "Automatically compute mesh element sizes from curvature. It overrides the target global mesh element size at input nodes. Default value is " + adaptive.ToString(), GH_ParamAccess.item, adaptive);
            pManager.AddIntegerParameter("Mininimum Elements", "MinElements", "Minimum number of elements per 2PI. Default value is " + minElemPerTwoPi, GH_ParamAccess.item, minElemPerTwoPi);
            pManager.AddIntegerParameter("Smoothing Steps", "Smoothing", "Number of smoothing steps applied to the final mesh. Default value is " + smoothingSteps, GH_ParamAccess.item, smoothingSteps);
            pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iSettings", "iSettings", "Solver configuration for quadratic quad-mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            MeshSolvers2D solver = MeshSolvers2D.Automatic;
            ISolver2D solverOpt = new ISolver2D();
            double maxSize = 1, minSize = 1;

            DA.GetData(1, ref minSize);
            DA.GetData(2, ref maxSize);
            DA.GetData(3, ref adaptive);
            DA.GetData(4, ref minElemPerTwoPi);
            DA.GetData(5, ref smoothingSteps);

            maxSize *= 2;
            minSize *= 2;
            if (minSize < 0.1 && MassiveRefinement == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, solverOpt.MinSizeWarning);
                minSize = 0.1;
            }

            double size = (maxSize+minSize)/ 2;
            DA.GetData(0, ref size);
            size *= 2;

            solverOpt.MeshingAlgorithm = (int)solver;
            solverOpt.CharacteristicLengthMin = minSize;
            solverOpt.CharacteristicLengthMax = maxSize;
            solverOpt.CharacteristicLengthFromCurvature = adaptive;
            solverOpt.MinimumElementsPerTwoPi = minElemPerTwoPi;
            solverOpt.OptimizationSteps = smoothingSteps;
            solverOpt.RecombinationAlgorithm = 1;
            solverOpt.RecombineAll = true;
            solverOpt.SubdivisionAlgorithm = 1;
            solverOpt.Subdivide = true;
            solverOpt.Size = size;

            DA.SetData(0, solverOpt);

            this.Message = "4Quad";
        }


        public bool MassiveRefinement
        {
            get { return massiveRefinement; }
            set
            {
                massiveRefinement = value;
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Massive Refinement", MassiveRefinement);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            bool refFlag = false;
            if (reader.TryGetBoolean("Massive Refinement", ref refFlag))
            {
                MassiveRefinement = refFlag;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem item = Menu_AppendItem(menu, "Massive Refinement", Menu_MassivePreviewClicked, true, MassiveRefinement);
            item.ToolTipText = "CAUTION: When checked, disable the imposed limit of minimum element-size.\nNote that setting a too small element-size might lead to over-refinements which can radically increase the computational time of the meshing process.";
        }

        private void Menu_MassivePreviewClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("Massive Refinement");
            MassiveRefinement = !MassiveRefinement;
            ExpireSolution(true);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iQuadSettings;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("596D240A-299F-4E24-8670-9B5DCB680B66"); }
        }
    }
}
