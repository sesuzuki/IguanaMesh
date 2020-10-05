using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers.ISolver
{
    public class IguanaGmshSolver2D : IguanaGmshSolver
    {
        /// <summary>
        /// 2D mesh algorithm (1: MeshAdapt, 2: Automatic, 3: Initial mesh only, 5: Delaunay, 6: Frontal-Delaunay, 7: BAMG, 8: Frontal-Delaunay for Quads, 9: Packing of Parallelograms). Default value: 6
        /// </summary>
        public MeshSolvers2D MeshingAlgorithm { get; set; }

        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        ////// Optimization SETUP
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////

        #region Optimization

        /// <summary>
        /// Use closest point to compute 2D crossfield.
        /// Default value: true
        /// </summary>
        [DefaultValue(true)]
        public bool CrossFieldClosestPoint { get; set; }

        /// <summary>
        /// Ratio between mesh sizes at nodes of a same edge (used in BAMG).
        /// Default value: 1.8
        /// </summary>
        [DefaultValue(1.8)]
        public double SmoothRatio { get; set; }

        /// <summary>
        /// Number of refinement steps in the MeshAdapt-based 2D algorithms.
        /// Default value: 10
        /// </summary>
        [DefaultValue(10)]
        public int RefineSteps { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        ////// RECOMBINATION SETUP (Tri => Quad)
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////

        #region Recombination

        /// <summary>
        /// Mesh recombination algorithm for recombine the current mesh. 
        /// Default value: Simple
        /// </summary>
        [DefaultValue(0)]
        public int RecombinationAlgorithm { get; set; }

        /// <summary>
        /// Apply recombination algorithm.
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public bool RecombineAll { get; set; }

        /// <summary>
        /// Number of topological optimization passes (removal of diamonds, ...) of recombined surface meshes.
        /// Default value: 5
        /// </summary>
        [DefaultValue(5)]
        public int RecombineOptimizeTopology { get; set; }

        #endregion

        public void ApplyBasic2DSettings()
        {
            IguanaGmsh.Option.SetNumber("Mesh.Algorithm", (int) MeshingAlgorithm);
            IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFactor", CharacteristicLengthFactor);
            IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthMin", CharacteristicLengthMin);
            IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthMax", CharacteristicLengthMax);
            IguanaGmsh.Option.SetNumber("Mesh.MinimumCurvePoints", MinimumCurvePoints);

            if (CharacteristicLengthFromCurvature)
            {
                IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromParametricPoints", 0);
                IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 1);
            }
            else
            {
                IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 0);
                IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromParametricPoints", 1);
            }
        }

        public void ApplyAdvanced2DSettings()
        {
            if (RecombineAll)
            {
                IguanaGmsh.Option.SetNumber("Mesh.RecombinationAlgorithm", (int) RecombinationAlgorithm);
                IguanaGmsh.Option.SetNumber("Mesh.RecombineOptimizeTopology", OptimizationSteps);
                IguanaGmsh.Option.SetNumber("Mesh.RecombineAll", 1);
                IguanaGmsh.Model.Mesh.Recombine();
            }else IguanaGmsh.Option.SetNumber("Mesh.RecombineAll", 0);

            if (Subdivide)
            {
                IguanaGmsh.Option.SetNumber("Mesh.SubdivisionAlgorithm", SubdivisionAlgorithm);
                IguanaGmsh.Model.Mesh.Refine();
            } else IguanaGmsh.Option.SetNumber("Mesh.SubdivisionAlgorithm", 0);

            if (Optimize)
            {
                IguanaGmsh.Option.SetNumber("Mesh.QualityType", QualityType);
                IguanaGmsh.Option.SetNumber("Mesh.Optimize", 1);
                string method = OptimizationAlgorithm;
                if (method == "Standard") method = "";
                IguanaGmsh.Model.Mesh.Optimize(method, OptimizationSteps);
            }else IguanaGmsh.Option.SetNumber("Mesh.Optimize", 0);
        }
    }
}
