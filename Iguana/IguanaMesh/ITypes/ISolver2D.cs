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
    public class ISolver2D : ISolver
    {
        public ISolver2D() : base(2)
        {
            CrossFieldClosestPoint = true;
            SmoothRatio = 1.8;
            RefineSteps = 10;
            RecombinationAlgorithm = 0;
            RecombineAll = false;
            RecombineOptimizeTopology = 5;
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
            MeshingAlgorithm = 2;
        }

        /// <summary>
        /// 2D mesh algorithm (1: MeshAdapt, 2: Automatic, 3: Initial mesh only, 5: Delaunay, 6: Frontal-Delaunay, 7: BAMG, 8: Frontal-Delaunay for Quads, 9: Packing of Parallelograms). Default value: 6
        /// </summary>
        public int MeshingAlgorithm { get; set; }

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
        public bool CrossFieldClosestPoint { get; set; }

        /// <summary>
        /// Ratio between mesh sizes at nodes of a same edge (used in BAMG).
        /// Default value: 1.8
        /// </summary>
        public double SmoothRatio { get; set; }

        /// <summary>
        /// Number of refinement steps in the MeshAdapt-based 2D algorithms.
        /// Default value: 10
        /// </summary>
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
        public int RecombinationAlgorithm { get; set; }

        /// <summary>
        /// Apply recombination algorithm.
        /// Default value: false
        /// </summary>
        public bool RecombineAll { get; set; }

        /// <summary>
        /// Number of topological optimization passes (removal of diamonds, ...) of recombined surface meshes.
        /// Default value: 5
        /// </summary>
        public int RecombineOptimizeTopology { get; set; }

        #endregion

        public void ApplySolverSettings(IField field = null)
        {
            IKernel.SetOptionNumber("Mesh.Algorithm", MeshingAlgorithm);
            IKernel.SetOptionNumber("Mesh.ElementOrder", ElementOrder);
            IKernel.SetOptionNumber("Mesh.MinimumElementsPerTwoPi", MinimumElementsPerTwoPi);
            IKernel.SetOptionNumber("Mesh.CharacteristicLengthMin", CharacteristicLengthMin);
            IKernel.SetOptionNumber("Mesh.CharacteristicLengthMax", CharacteristicLengthMax);

            if (ElementOrder != 1)
            {
                IKernel.SetOptionNumber("Mesh.HighOrderOptimize", HighOrderOptimize);
                IKernel.SetOptionNumber("Mesh.HighOrderPassMax", HighOrderPassMax);
                IKernel.SetOptionNumber("Mesh.SecondOrderIncomplete", Convert.ToInt32(SecondOrderIncomplete));
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

            if (RecombineAll)
            {
                IKernel.SetOptionNumber("Mesh.RecombinationAlgorithm", (int)RecombinationAlgorithm);
                IKernel.SetOptionNumber("Mesh.RecombineOptimizeTopology", OptimizationSteps);
                IKernel.SetOptionNumber("Mesh.RecombineAll", 1);
            }
            else IKernel.SetOptionNumber("Mesh.RecombineAll", 0);

            if (Subdivide)
            {
                IKernel.SetOptionNumber("Mesh.SubdivisionAlgorithm", SubdivisionAlgorithm);
            }
            else IKernel.SetOptionNumber("Mesh.SubdivisionAlgorithm", 0);

            IKernel.SetOptionNumber("Mesh.Smoothing", OptimizationSteps);
        }
    }
}
