using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IMeshingOptions2DGH : GH_Component
    {
        MeshSolvers2D solver = MeshSolvers2D.TriFrontalDelaunay;
        IGmshSolverOptions solverOpt = new IGmshSolverOptions();
        List<double> sizes = new List<double>() { 1.0 };
        double swapAngle=10, sizeFactor=1.0, minSize=0, maxSize= 1e+22, seed=1;
        int reAlg=0, smoothingStep=1, optiPasses=5;
        bool adaptCrv=false, optimize=true;
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
            pManager.AddNumberParameter("Swap Angle", "SwapAngle", "Threshold angle (in degrees) between faces normals under which we allow an edge swap. Default value is 10", GH_ParamAccess.item, swapAngle);
            pManager.AddNumberParameter("Target Mesh Size", "MeshSize", "Target mesh size at input nodes. If the number of size values is not equal to the number of nodes, the first item of the list is assigned to all nodes. Default value is 1.0.", GH_ParamAccess.list, sizes);
            pManager.AddNumberParameter("Size Factor", "SizeFactor", "Factor applied to all mesh element sizes. Default value is 1.0.", GH_ParamAccess.item, sizeFactor);
            pManager.AddNumberParameter("Minimum Size", "MinSize", "Minimum mesh element size. Default value is 0.0.", GH_ParamAccess.item, minSize);
            pManager.AddNumberParameter("Maximun Size", "MaxSize", "Maximum mesh element size. Default value is 1e+22.", GH_ParamAccess.item, maxSize);
            pManager.AddNumberParameter("Seed Random Number", "Seed", "Seed of pseudo-random number generator. Default value is 1.", GH_ParamAccess.item, seed);
            pManager.AddIntegerParameter("Recombination Algorithm", "Recombination", "Mesh recombination algorithm (0: simple, 1: blossom, 2: simple full-quad, 3: blossom full-quad). Default value is 0.", GH_ParamAccess.item, reAlg);
            pManager.AddIntegerParameter("Smoothing Steps", "Smoothing", "Number of smoothing steps applied to the final mesh. Default value is 1.", GH_ParamAccess.item, smoothingStep);
            pManager.AddIntegerParameter("OptimizeTopology", "OptimizeTopology", "Number of topological optimization passes of recombined surface meshes. Default value is 5.", GH_ParamAccess.item, optiPasses);
            pManager.AddBooleanParameter("Curvature Adapt", "AdaptCurvature", "Automatically compute mesh element sizes from curvature. Default value is false.", GH_ParamAccess.item, adaptCrv);
            pManager.AddBooleanParameter("Optimize", "Optimize", "Optimize the mesh to improve the quality of tetrahedral elements. Default value is true.", GH_ParamAccess.item, optimize);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshingOptions", "iOpt", "Solver configuration for 2D mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            solverOpt = new IGmshSolverOptions();
            sizes = new List<double>();

            DA.GetData(0, ref swapAngle);
            DA.GetDataList(1, sizes);
            DA.GetData(2, ref sizeFactor);
            DA.GetData(3, ref minSize);
            DA.GetData(4, ref maxSize);
            DA.GetData(5, ref seed);
            DA.GetData(6, ref reAlg);
            DA.GetData(7, ref smoothingStep);
            DA.GetData(8, ref optiPasses);
            DA.GetData(9, ref adaptCrv);
            DA.GetData(10, ref optimize);

            solverOpt.MeshingAlgorithm = (int)solver;
            solverOpt.TargetMeshSize = sizes;
            solverOpt.AllowSwapAngle = swapAngle;
            solverOpt.CharacteristicLengthFactor = sizeFactor;
            solverOpt.CharacteristicLengthMin = minSize;
            solverOpt.CharacteristicLengthMax = maxSize;
            solverOpt.RandomSeed = seed;
            solverOpt.RecombinationAlgorithm = reAlg;
            solverOpt.Smoothing = smoothingStep;
            solverOpt.RecombineOptimizeTopology = optiPasses;
            solverOpt.CharacteristicLengthFromCurvature = adaptCrv;
            solverOpt.Optimize = optimize;

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