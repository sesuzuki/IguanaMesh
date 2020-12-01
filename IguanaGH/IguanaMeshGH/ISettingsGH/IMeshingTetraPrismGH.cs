using System;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.ISolver;

namespace IguanaGH.IguanaMeshGH.ISettingsGH
{
    public class IMeshingTetraPrismGH : GH_Component
    {
        MeshSolvers3D solver = MeshSolvers3D.Delaunay;
        IguanaGmshSolver3D solverOpt;
        double sizeFactor = 1.0, minSize = 0, maxSize = 1e+22, qualityThreshold = 0.3;
        int steps = 10, qualityType = 2, minPts = 10, minElemPerTwoPi = 6;
        bool adaptive = false;

        /// <summary>
        /// Initializes a new instance of the IMeshing3DGH class.
        /// </summary>
        public IMeshingTetraPrismGH()
          : base("iTetraPrismSettings", "iTetraPrism",
              "Configuration for 3D tetrahedrons+prisms mesh generation.",
              "Iguana", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Size Factor", "SizeFactor", "Factor applied to all mesh element sizes. Default value is " + sizeFactor, GH_ParamAccess.item, sizeFactor);
            pManager.AddNumberParameter("Minimum Size", "MinSize", "Minimum mesh element size. Default value is " + minSize, GH_ParamAccess.item, minSize);
            pManager.AddNumberParameter("Maximun Size", "MaxSize", "Maximum mesh element size. Default value is " + maxSize, GH_ParamAccess.item, maxSize);
            pManager.AddIntegerParameter("Minimum Points", "MinPoints", "Minimum number of points used to mesh edge-surfaces. Default value is " + minPts, GH_ParamAccess.item, minPts);
            pManager.AddBooleanParameter("Curvature Adapt", "Adaptive", "Automatically compute mesh element sizes from curvature. It overrides the target global mesh element size at input nodes. Default value is " + adaptive.ToString(), GH_ParamAccess.item, adaptive);
            pManager.AddIntegerParameter("Minimum Elements", "MinElements", "Minimum number of elements per 2PI. Default value is " + minElemPerTwoPi, GH_ParamAccess.item, minElemPerTwoPi);
            pManager.AddIntegerParameter("Optimization Steps", "Steps", "Number of optimization steps applied to the final mesh. Default value is "+ steps, GH_ParamAccess.item, steps);
            pManager.AddIntegerParameter("Quality Type", "Quality", "Type of quality measure for element optimization (0: SICN => signed inverse condition number, 1: SIGE => signed inverse gradient error, 2: gamma => vol/sum_face/max_edge, 3: Disto => minJ/maxJ). Default value is " + qualityType, GH_ParamAccess.item, qualityType);
            pManager.AddNumberParameter("Quality Threshold", "Threshold", "Quality threshold for element optimization. Default value is " + qualityThreshold, GH_ParamAccess.item, qualityThreshold);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshingOptions", "iS3D", "Solver configuration for 3D prism-mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            solverOpt = new IguanaGmshSolver3D();

            DA.GetData(0, ref sizeFactor);
            DA.GetData(1, ref minSize);
            DA.GetData(2, ref maxSize);
            DA.GetData(3, ref minPts);
            DA.GetData(4, ref adaptive);
            DA.GetData(5, ref minElemPerTwoPi);
            DA.GetData(6, ref steps);
            DA.GetData(7, ref qualityType);
            DA.GetData(8, ref qualityThreshold);

            solverOpt.MeshingAlgorithm = solver;
            solverOpt.CharacteristicLengthFactor = sizeFactor;
            solverOpt.CharacteristicLengthMin = minSize;
            solverOpt.CharacteristicLengthMax = maxSize;
            solverOpt.CharacteristicLengthFromCurvature = adaptive;
            solverOpt.OptimizationSteps = steps;
            solverOpt.OptimizeThreshold = qualityThreshold;

            string method = Enum.GetName(typeof(ElementQualityType), qualityType);
            if (method != null) qualityType = 2;
            solverOpt.QualityType = qualityType;

            DA.SetData(0, solverOpt);

            this.Message = "4Tetra+6Prism";
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
                return Properties.Resources.iTetrahedronSettings;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b79bf80c-d410-4ad5-b9db-0b914fbafc8c"); }
        }
    }
}