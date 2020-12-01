using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ISettingsGH
{
    public class IMeshingQuadTriaHighOrderGH : GH_Component
    {
        MeshSolvers2DQuads solver = MeshSolvers2DQuads.QuadsFrontalDelaunay;
        IguanaGmshSolver2D solverOpt;
        double sizeFactor = 1.0, minSize = 0, maxSize = 1e+22;
        int recombine = 2, ho_optimization = 0, smoothingSteps = 10, minPts = 10, minElemPerTwoPi = 6;
        bool adaptive;

        /// <summary>
        /// Initializes a new instance of the IMeshingQuadHighOrderGH class.
        /// </summary>
        public IMeshingQuadTriaHighOrderGH()
          : base("iQuadTriaHighOrderSettings", "iQuadTriaHighOrder",
              "Configuration for 2D quads-trias high-order mesh generation.",
              "Iguana", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Size Factor", "SizeFactor", "Factor applied to all mesh element sizes. Default value is " + sizeFactor, GH_ParamAccess.item, sizeFactor);
            pManager.AddNumberParameter("Minimum Size", "MinSize", "Minimum mesh element size. Default value is " + minElemPerTwoPi, GH_ParamAccess.item, minSize);
            pManager.AddNumberParameter("Maximun Size", "MaxSize", "Maximum mesh element size. Default value is " + maxSize, GH_ParamAccess.item, maxSize);
            pManager.AddIntegerParameter("Minimun Points", "MinPoints", "Minimum number of points used to mesh edge-surfaces. Default value is " + minPts, GH_ParamAccess.item, minPts);
            pManager.AddBooleanParameter("Curvature Adapt", "Adaptive", "Automatically compute mesh element sizes from curvature. It overrides the target global mesh element size at input nodes. Default value is " + adaptive.ToString(), GH_ParamAccess.item, adaptive);
            pManager.AddIntegerParameter("Mininimum Elements", "MinElements", "Minimum number of elements per 2PI. Default value is " + minElemPerTwoPi, GH_ParamAccess.item, minElemPerTwoPi);
            pManager.AddIntegerParameter("Recombination Method", "Method", "Method to combine triangles into quadrangles (1: Blossom, 2: Simple full-quad, 3: Blossom full-quad). Default value is " + recombine, GH_ParamAccess.item, recombine);
            pManager.AddIntegerParameter("Smoothing Steps", "Smoothing", "Number of smoothing steps applied to the final mesh. Default value is " + smoothingSteps, GH_ParamAccess.item, smoothingSteps);
            pManager.AddIntegerParameter("Optimization", "Optimization", "Optimizatio method of high-order elements (0: None, 1: Optimization, 2: Elastic+optimization, 3: Elastic, 4: Fast-curving). Default value is " + ho_optimization, GH_ParamAccess.item, ho_optimization);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshingOptions", "iS2D", "Solver configuration for 2D quad-high-order mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            solverOpt = new IguanaGmshSolver2D();

            DA.GetData(0, ref sizeFactor);
            DA.GetData(1, ref minSize);
            DA.GetData(2, ref maxSize);
            DA.GetData(3, ref minPts);
            DA.GetData(4, ref adaptive);
            DA.GetData(5, ref minElemPerTwoPi);
            DA.GetData(6, ref recombine);
            DA.GetData(7, ref smoothingSteps);
            DA.GetData(8, ref ho_optimization);

            solverOpt.MeshingAlgorithm = (int)solver;
            solverOpt.CharacteristicLengthFactor = sizeFactor;
            solverOpt.CharacteristicLengthMin = minSize;
            solverOpt.CharacteristicLengthMax = maxSize;
            solverOpt.CharacteristicLengthFromCurvature = adaptive;
            solverOpt.MinimumCurvePoints = minPts;
            solverOpt.MinimumElementsPerTwoPi = minElemPerTwoPi;
            solverOpt.OptimizationSteps = smoothingSteps;
            solverOpt.HighOrderOptimize = ho_optimization;
            solverOpt.ElementOrder = 2;

            string method;

            method = Enum.GetName(typeof(RecombinationAlgorithm), recombine);
            if (method == null) recombine = 2;
            solverOpt.RecombinationAlgorithm = recombine;
            solverOpt.RecombineAll = true;

            DA.SetData(0, solverOpt);

            this.Message = "8Quads+6Trias";
        }


        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("MeshSolvers2DQuads", (int)solver);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            int aIndex = -1;
            if (reader.TryGetInt32("MeshSolvers2DQuads", ref aIndex))
            {
                solver = (MeshSolvers2DQuads)aIndex;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            foreach (MeshSolvers2DQuads s in Enum.GetValues(typeof(MeshSolvers2DQuads)))
                GH_Component.Menu_AppendItem(menu, s.ToString(), SolverType, true, s == this.solver).Tag = s;
            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void SolverType(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is MeshSolvers2DQuads)
            {
                this.solver = (MeshSolvers2DQuads)item.Tag;
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
                return Properties.Resources.iQuadHighOrderSettings;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e350de41-be22-4f36-9f7e-61d8d3cd2416"); }
        }
    }
}