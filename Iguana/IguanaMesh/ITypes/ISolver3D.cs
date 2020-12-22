/*
 * <Iguana>
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

namespace Iguana.IguanaMesh.ITypes
{
    public class ISolver3D : ISolver
    {
        public ISolver3D() : base(3)
        {
            RecombinationAlgorithm = 0;
            RecombineAll = false;
            ElementOrder = 1;
            HighOrderPassMax = 25;
            Size = 1.0;
            RecombinationAlgorithm = 0;
            RecombineAll = false;
            ElementOrder = 1;
            HighOrderPassMax = 25;
            Subdivide = false;
            SubdivisionAlgorithm = 0;
            SecondOrderIncomplete = true;
            CharacteristicLengthFactor = 1.0;
            MinimumCurvePoints = 3;
            HighOrderOptimize = 0;
            CharacteristicLengthMin = 0;
            CharacteristicLengthMax = 1e22;
            OptimizationSteps = 5;
            Size = 1.0;
        }

        /// <summary>
        /// 3D mesh algorithm (1: Delaunay, 3: Initial mesh only, 4: Frontal, 7: MMG3D, 9: R-tree, 10: HXT). Default value: 1
        /// </summary>
        public MeshSolvers3D MeshingAlgorithm { get; set; }

        /// <summary>
        /// Tolerance for initial 3D Delaunay mesher.
        /// Default value: 1e-08
        /// </summary>
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
        public bool SmoothCrossField { get; set; }

        public int OptimizationMethod { get; set; }

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
        public bool Recombine3DAll { get; set; }

        /// <summary>
        /// 3d recombination level (0: hex, 1: hex+prisms, 2: hex+prism+pyramids) (experimental).
        /// Default value: 0
        /// </summary>
        public bool Recombine3DLevel { get; set; }

        /// <summary>
        /// 3d recombination conformity type (0: nonconforming, 1: trihedra, 2: pyramids+trihedra, 3:pyramids+hexSplit+trihedra, 4:hexSplit+trihedra)(experimental).
        /// Default value: 0
        /// </summary>
        public bool Recombine3DConformity { get; set; }

        #endregion

        public void ApplySolverSettings(IField field = null)
        {
            IKernel.SetOptionNumber("Mesh.Algorithm", (int)MeshingAlgorithm);
            IKernel.SetOptionNumber("Mesh.ElementOrder", ElementOrder);
            IKernel.SetOptionNumber("Mesh.MinimumElementsPerTwoPi", MinimumElementsPerTwoPi);
            IKernel.SetOptionNumber("Mesh.CharacteristicLengthMin", CharacteristicLengthMin);
            IKernel.SetOptionNumber("Mesh.CharacteristicLengthMax", CharacteristicLengthMax);

            if (ElementOrder != 1)
            {
                IKernel.SetOptionNumber("Mesh.HighOrderOptimize", HighOrderOptimize);
                IKernel.SetOptionNumber("Mesh.HighOrderPassMax", HighOrderPassMax);
            }

            if (field != null)
            {
                field.ApplyField();
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldAsBackgroundMesh(field.Tag);
                IKernel.SetOptionNumber("Mesh.CharacteristicLengthFromCurvature", 0);
                IKernel.SetOptionNumber("Mesh.CharacteristicLengthFromPoints", 0);
                IKernel.SetOptionNumber("Mesh.CharacteristicLengthExtendFromBoundary", 0);
            }
            else
            {
                IKernel.SetOptionNumber("Mesh.CharacteristicLengthExtendFromBoundary", 1);

                if (CharacteristicLengthFromCurvature)
                {
                    IKernel.SetOptionNumber("Mesh.CharacteristicLengthFromPoints", 0);
                    IKernel.SetOptionNumber("Mesh.CharacteristicLengthFromCurvature", 1);
                }
                else
                {
                    IKernel.SetOptionNumber("Mesh.CharacteristicLengthFromCurvature", 0);
                    IKernel.SetOptionNumber("Mesh.CharacteristicLengthFromPoints", 1);
                }
            }

            IKernel.SetOptionNumber("Mesh.OptimizeThreshold", OptimizeThreshold);
            IKernel.SetOptionNumber("Mesh.QualityType", QualityType);
            IKernel.SetOptionNumber("Mesh.OptimizeNetgen", OptimizationMethod);

            if (RecombineAll)
            {
                IKernel.SetOptionNumber("Mesh.RecombinationAlgorithm", (int)RecombinationAlgorithm);
                IKernel.SetOptionNumber("Mesh.RecombineAll", 1);
            }
            else IKernel.SetOptionNumber("Mesh.RecombineAll", 0);
        }
    }
}
