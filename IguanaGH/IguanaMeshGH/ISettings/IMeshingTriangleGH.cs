using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.ISolver;

namespace IguanaGH.IguanaMeshGH.ISettings
{
    public class IMeshingTriangleGH : GH_Component
    {
        MeshSolvers2D solver = MeshSolvers2D.TriFrontalDelaunay;
        IguanaGmshSolver2D solverOpt = new IguanaGmshSolver2D();
        List<double> sizes = new List<double>() { 1.0 };
        double sizeFactor=1.0, minSize=0, maxSize= 1e+22, qualityThreshold=0.3;
        int optimize=0, steps=10, subdivide=-1, qualityType=2;
        bool adaptive = false;

        /// <summary>
        /// Initializes a new instance of the IMeshingOptions2D class.
        /// </summary>
        public IMeshingTriangleGH()
          : base("iTriangleSettings", "iTri",
              "Configuration for 2D triangle-mesh generation.",
              "Iguana", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Node Sizes", "Sizes", "Target global mesh element size at input nodes. If the number of size values is not equal to the number of nodes, the first item of the list is assigned to all nodes. Default value is 1.0.", GH_ParamAccess.list, sizes);
            pManager.AddNumberParameter("Size Factor", "Factor", "Factor applied to all mesh element sizes. Default value is 1.0.", GH_ParamAccess.item, sizeFactor);
            pManager.AddNumberParameter("Minimum Size", "MinSize", "Minimum mesh element size. Default value is 0.0.", GH_ParamAccess.item, minSize);
            pManager.AddNumberParameter("Maximun Size", "MaxSize", "Maximum mesh element size. Default value is 1e+22.", GH_ParamAccess.item, maxSize);
            pManager.AddBooleanParameter("Curvature Adapt", "Adaptive", "Automatically compute mesh element sizes from curvature. It overrides the target global mesh element size at input nodes. Default value is false.", GH_ParamAccess.item, adaptive);
            pManager.AddIntegerParameter("Optimize", "Optimize", "Optimization method (-1: No optimization, 0: Standard, 1: Netgen, 2: HighOrder, 3: HighOrderElastic, 4: HighOrderFastCurving, 5: Laplace2D, 6: Relocate2D) Default value is 0.", GH_ParamAccess.item, optimize);
            pManager.AddIntegerParameter("Optimization Steps", "Steps", "Number of optimization steps applied to the final mesh. Default value is 10.", GH_ParamAccess.item, steps);
            pManager.AddIntegerParameter("Subdivide", "Subdivide", "Mesh subdivision algorithm (-1: No subdivision, 0: all quadrangles, 1: all hexahedra, 2: barycentric). Default value is -1.", GH_ParamAccess.item, subdivide);
            pManager.AddIntegerParameter("Quality Type", "Quality", "Type of quality measure for element optimization (0: SICN => signed inverse condition number, 1: SIGE => signed inverse gradient error, 2: gamma => vol/sum_face/max_edge, 3: Disto => minJ/maxJ). Default value is 2.", GH_ParamAccess.item, qualityType);
            pManager.AddNumberParameter("Quality Threshold", "Qt", "Quality threshold for element optimization. Default value is 0.3.", GH_ParamAccess.item, qualityThreshold);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshingOptions", "iOpt2D", "Solver configuration for 2D triangle-mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            solverOpt = new IguanaGmshSolver2D();
            sizes = new List<double>();

            DA.GetDataList(0, sizes);
            DA.GetData(1, ref sizeFactor);
            DA.GetData(2, ref minSize);
            DA.GetData(3, ref maxSize);
            DA.GetData(4, ref adaptive);
            DA.GetData(5, ref optimize);
            DA.GetData(6, ref steps);
            DA.GetData(7, ref subdivide);
            DA.GetData(8, ref qualityType);
            DA.GetData(9, ref qualityThreshold);

            solverOpt.MeshingAlgorithm = solver;
            solverOpt.TargetMeshSizeAtNodes = sizes;
            solverOpt.CharacteristicLengthFactor = sizeFactor;
            solverOpt.CharacteristicLengthMin = minSize;
            solverOpt.CharacteristicLengthMax = maxSize;
            solverOpt.CharacteristicLengthFromCurvature = adaptive;

            string method;
            if (optimize == -1) solverOpt.Optimize = false;
            else
            {
                method = Enum.GetName(typeof(OptimizationAlgorithm), optimize);
                if (method == null || method == "Relocate3D") method = "Standard";
                solverOpt.Optimize = true;
                solverOpt.OptimizationAlgorithm = method;
                solverOpt.OptimizationSteps = steps;
                solverOpt.OptimizeThreshold = qualityThreshold;
            }

            if (subdivide == -1) solverOpt.Subdivide = false;
            else
            {
                method = Enum.GetName(typeof(SubdivisionAlgorithm), subdivide);
                if (method == null) subdivide = 0;
                solverOpt.Subdivide = true;
                solverOpt.SubdivisionAlgorithm = subdivide;
            }

            method = Enum.GetName(typeof(ElementQualityType), qualityType);
            if (method != null)
            {
                solverOpt.QualityType = qualityType;
            }

            DA.SetData(0, solverOpt);
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
                this.solver = (MeshSolvers2D) item.Tag;
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
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("290adaa4-95b4-48fa-a370-13d965d41c34"); }
        }
    }
}