using Iguana.IguanaMesh.IWrappers.IExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers.ISolver
{
    public class IguanaGmshSolver3D : IguanaGmshSolver
    {

        public IguanaGmshSolver3D() : base()
        {
            RecombinationAlgorithm = 0;
            RecombineAll = false;
            ElementOrder = 1;
            HighOrderPassMax = 25;
        }

        /// <summary>
        /// 3D mesh algorithm (1: Delaunay, 3: Initial mesh only, 4: Frontal, 7: MMG3D, 9: R-tree, 10: HXT). Default value: 1
        /// </summary>
        public MeshSolvers3D MeshingAlgorithm { get; set; }

        /// <summary>
        /// Tolerance for initial 3D Delaunay mesher.
        /// Default value: 1e-08
        /// </summary>
        [DefaultValue(1e-08)]
        public double ToleranceInitialDelaunay { get; set; }

        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        ////// Optimization SETUP
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////

        #region Optimization

        /// <summary>
        /// Apply n barycentric smoothing passes to the 3D cross field
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public bool SmoothCrossField { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        ////// RECOMBINATION SETUP
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////

        #region Recombination

        /// <summary>
        /// Mesh recombination algorithm for recombine the current mesh. 
        /// Default value: Simple
        /// </summary>
        public int RecombinationAlgorithm { get; set; }

        /// <summary>
        /// Apply recombination algorithm.
        /// Default value: false
        /// </summary>
        public bool RecombineAll { get; set; }

        /// <summary>
        /// Apply recombination3D algorithm to all volumes, ignoring per-volume spec (experimental)
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public bool Recombine3DAll { get; set; }

        /// <summary>
        /// 3d recombination level (0: hex, 1: hex+prisms, 2: hex+prism+pyramids) (experimental).
        /// Default value: 0
        /// </summary>
        [DefaultValue(false)]
        public bool Recombine3DLevel { get; set; }

        /// <summary>
        /// 3d recombination conformity type (0: nonconforming, 1: trihedra, 2: pyramids+trihedra, 3:pyramids+hexSplit+trihedra, 4:hexSplit+trihedra)(experimental).
        /// Default value: 0
        /// </summary>
        [DefaultValue(false)]
        public bool Recombine3DConformity { get; set; }

        #endregion

        public void ApplySolverSettings(IguanaGmshField field = null)
        {
            IguanaGmsh.Option.SetNumber("Mesh.Algorithm", (int) MeshingAlgorithm);
            IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFactor", CharacteristicLengthFactor);
            IguanaGmsh.Option.SetNumber("Mesh.MinimumCurvePoints", MinimumCurvePoints);
            IguanaGmsh.Option.SetNumber("Mesh.ElementOrder", ElementOrder);

            if (ElementOrder != 1)
            {
                IguanaGmsh.Option.SetNumber("Mesh.HighOrderOptimize", HighOrderOptimize);
                IguanaGmsh.Option.SetNumber("Mesh.HighOrderPassMax", HighOrderPassMax);
            }

            if (field != null)
            {
                field.ApplyField();
                IguanaGmsh.Model.MeshField.SetAsBackgroundMesh(field.Tag);
                IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 0);
                IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromPoints", 0);
                IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthExtendFromBoundary", 0);
            }
            else
            {
                IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthMin", CharacteristicLengthMin);
                IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthMax", CharacteristicLengthMax);

                if (CharacteristicLengthFromCurvature)
                {
                    IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromPoints", 0);
                    IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 1);
                    IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthExtendFromBoundary", 1);
                }
                else
                {
                    IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 0);
                    IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromPoints", 1);
                    IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthExtendFromBoundary", 1 );
                }
            }

            IguanaGmsh.Option.SetNumber("Mesh.OptimizeThreshold", OptimizeThreshold);
            IguanaGmsh.Option.SetNumber("Mesh.QualityType", QualityType);
            IguanaGmsh.Model.Mesh.Optimize("NetGen", OptimizationSteps);

            if (RecombineAll)
            {
                IguanaGmsh.Option.SetNumber("Mesh.RecombinationAlgorithm", (int) RecombinationAlgorithm);
                IguanaGmsh.Option.SetNumber("Mesh.RecombineAll", 1);
                IguanaGmsh.Model.Mesh.Recombine();
            }
            else IguanaGmsh.Option.SetNumber("Mesh.RecombineAll", 0);
        }
    }
}
