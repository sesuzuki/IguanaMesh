using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Iguana.IguanaMesh.IGmshWrappers;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IMeshingOptions2DGH : GH_Component
    {
        MeshSolvers2D solver = MeshSolvers2D.TriFrontalDelaunay;
        IguanaGmshSolverOptions solverOpt = new IguanaGmshSolverOptions();
        List<double> sizes = new List<double>() { 1.0 };
        double sizeFactor=1.0, minSize=0, maxSize= 1e+22;
        int reAlg=0, optMe=0, smoothingStep=10, subMe=0;
        bool adaptCrv = false, optimize = true, recombine = false, subD = false;
        /// <summary>
        /// Initializes a new instance of the IMeshingOptions2D class.
        /// </summary>
        public IMeshingOptions2DGH()
          : base("iOptions for 2D mesh generation", "iMeshingOptions2D",
              "Configuration for 2D mesh generation.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Target Mesh Sizes", "Sizes", "Target global mesh element size at input nodes. If the number of size values is not equal to the number of nodes, the first item of the list is assigned to all nodes. Default value is 1.0.", GH_ParamAccess.list, sizes);
            pManager.AddNumberParameter("Size Factor", "Factor", "Factor applied to all mesh element sizes. Default value is 1.0.", GH_ParamAccess.item, sizeFactor);
            pManager.AddNumberParameter("Minimum Size", "MinSize", "Minimum mesh element size. Default value is 0.0.", GH_ParamAccess.item, minSize);
            pManager.AddNumberParameter("Maximun Size", "MaxSize", "Maximum mesh element size. Default value is 1e+22.", GH_ParamAccess.item, maxSize);
            pManager.AddBooleanParameter("Curvature Adapt", "Adaptive", "Automatically compute mesh element sizes from curvature. It overrides the target global mesh element size at input nodes. Default value is false.", GH_ParamAccess.item, adaptCrv);
            pManager.AddBooleanParameter("Recombine to Quads", "Recombine", "Apply an algorithm for recombining the current triangular elements into quadrangles. Default value is false.", GH_ParamAccess.item, recombine);
            pManager.AddIntegerParameter("Recombination Method", "Recombine_Method", "Mesh recombination method (0: Simple, 1: Blossom, 2: Simple full-quad, 3: Blossom full-quad). Default value is 0.", GH_ParamAccess.item, reAlg);
            pManager.AddBooleanParameter("Optimize", "Optimize", "Optimize the mesh to improve the quality of elements. Default value is true.", GH_ParamAccess.item, optimize);
            pManager.AddIntegerParameter("Optimization Method", "Optimization_Method", "Optimization method (0: Laplace2D, 1: Relocate2D) Default value is 0.", GH_ParamAccess.item, optMe);
            pManager.AddIntegerParameter("Optimization Steps", "Steps", "Number of optimization steps applied to the final mesh. Default value is 10.", GH_ParamAccess.item, smoothingStep);
            pManager.AddBooleanParameter("Subdivide Elements", "Refine", "Subdivide mesh. Default value is false.", GH_ParamAccess.item, subD);
            pManager.AddIntegerParameter("Refine Method", "Refine_Method", "Mesh subdivision algorithm (0: all quadrangles, 1: all hexahedra, 2: barycentric). Default value is false.", GH_ParamAccess.item, subMe);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshingOptions", "iSettings2D", "Solver configuration for 2D mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            solverOpt = new IguanaGmshSolverOptions();
            sizes = new List<double>();

            DA.GetDataList(0, sizes);
            DA.GetData(1, ref sizeFactor);
            DA.GetData(2, ref minSize);
            DA.GetData(3, ref maxSize);
            DA.GetData(4, ref adaptCrv);
            DA.GetData(5, ref recombine);
            DA.GetData(6, ref reAlg);
            DA.GetData(7, ref optimize);
            DA.GetData(8, ref optMe);
            DA.GetData(9, ref smoothingStep);
            DA.GetData(10, ref subD);

            solverOpt.MeshingAlgorithm = solver;
            solverOpt.TargetMeshSizeAtNodes = sizes;
            solverOpt.CharacteristicLengthFactor = sizeFactor;
            solverOpt.CharacteristicLengthMin = minSize;
            solverOpt.CharacteristicLengthMax = maxSize;
            solverOpt.RecombinationAlgorithm = reAlg;
            solverOpt.RecombineAll = recombine;
            solverOpt.Smoothing = smoothingStep;
            solverOpt.CharacteristicLengthFromCurvature = adaptCrv;
            solverOpt.Optimize = optimize;
            solverOpt.Subdivide = subD;
            solverOpt.SubdivisionAlgorithm = subMe;
               

            switch (optMe)
            {
                case 0:
                    solverOpt.OptimizationAlgorithm = "Laplace2D";
                    break;
                case 1:
                    solverOpt.OptimizationAlgorithm = "Relocate2D";
                    break;
                default:
                    solverOpt.OptimizationAlgorithm = "Laplace2D";
                    break;

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