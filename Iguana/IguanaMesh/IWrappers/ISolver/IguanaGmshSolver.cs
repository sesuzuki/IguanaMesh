using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using System.Collections.Generic;
using System.ComponentModel;

namespace Iguana.IguanaMesh.IWrappers.ISolver
{
    public enum MeshSolvers2D { MeshAdapt = 1, Automatic = 2, InitialMeshOnly = 3, Delaunay = 5, TriFrontalDelaunay = 6, BAMG = 7, QuadsFrontalDelaunay = 8, PackingOfParallelograms = 9 }
    public enum MeshSolvers3D { Delaunay = 1, InitialMeshOnly = 3, Frontal = 4, MMG3D = 7, RTree = 9, HXT = 10 }
    public enum OptimizationAlgorithm { Standard=0, Netgen=1, HighOrder=2, HighOrderElastic=3, HighOrderFastCurving=4, Laplace2D=5, Relocate2D=6, Relocate3D=7 }
    public enum ElementQualityType { SICN=0, SIGE=1, Gamma=2, Disto=3 }
    public enum SubdivisionAlgorithm { AllQuads=0, AllHexa=1, Barycentric=2 }
    public enum RecombinationAlgorithm { Simple=0, Blossom=1, SimpleFullQuad=2, BlossomFullQuad=3  }

    public abstract class IguanaGmshSolver
    {
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        ////// BASIC SETUP
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////

        #region Setup

        /// <summary>
        /// Element order(1: first order elements).
        /// Default value: 1
        /// </summary>
        [DefaultValue(1)]
        public int ElementOrder { get; set; }

        /// <summary>
        /// Skip a model edge in mesh generation if its length is less than user’s defined tolerance.
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public int ToleranceEdgeLength { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        ////// Optimization SETUP
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////

        #region Optimization

        /// <summary>
        /// Optimization method (Standard, Netgen, HighOrder, HighOrderElastic, HighOrderFastCurving, Laplace2D, Relocate2D, Relocate3D)
        /// </summary>
        public string OptimizationAlgorithm { get; set; }

        /// <summary>
        /// Type of quality measure (0: SICN~signed inverse condition number, 1: SIGE~signed inverse gradient error, 2: gamma~vol/sum_face/max_edge, 3: Disto~minJ/maxJ)
        /// Default value: 2
        /// </summary>
        [DefaultValue(2)]
        public int QualityType { get; set; }

        /// <summary>
        /// Optimize the mesh to improve the quality of tetrahedral elements
        /// Default value: true
        /// </summary>
        [DefaultValue(true)]
        public bool Optimize { get; set; }

        /// <summary>
        /// Optimize tetrahedra that have a quality below...
        /// Default value: 0.3
        /// </summary>
        [DefaultValue(0.3)]
        public double OptimizeThreshold { get; set; }

        /// <summary>
        /// Number of optimization steps applied to the final mesh.
        /// Default value: 10
        /// </summary>
        [DefaultValue(10)]
        public int OptimizationSteps { get; set; }

        /// <summary>
        /// Smooth the mesh normals
        /// Default value: 0
        /// </summary>
        [DefaultValue(false)]
        public bool SmoothNormals { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        ////// SUBDIVISION SETUP
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////

        #region Subdivision

        /// <summary>
        /// Mesh subdivision algorithm(0: all quadrangles, 1: all hexahedra, 2: barycentric).
        /// Default value: 0
        /// </summary>
        [DefaultValue(0)]
        public int SubdivisionAlgorithm { get; set; }

        /// <summary>
        /// Apply subdivision.
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public bool Subdivide { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        ////// Principal Common Parameters
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////

        #region Principal Common Parameters

        /// <summary>
        /// Target mesh size at input nodes. If the number of size values is not equal to the number of nodes, the first item of the size value list is assigned to all nodes.
        /// Default favlue is 1.0;
        /// </summary>
        public List<double> TargetMeshSizeAtNodes { get; set; }

        /// <summary>
        /// Factor applied to all mesh element sizes.
        /// Default value: 1
        /// </summary>
        [DefaultValue(1)]
        public double CharacteristicLengthFactor { get; set; }

        /// <summary>
        /// Minimum mesh element size.
        /// Default value: 0
        /// </summary>
        [DefaultValue(0)]
        public double CharacteristicLengthMin { get; set; }

        /// <summary>
        /// Maximum mesh element size.
        /// Default value: 1e+22
        /// </summary>
        [DefaultValue(1e22)]
        public double CharacteristicLengthMax { get; set; }

        /// <summary>
        /// Automatically compute mesh element sizes from curvature.
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public bool CharacteristicLengthFromCurvature { get; set; }

        /// <summary>
        /// Compute mesh element sizes from values given at geometry points.
        /// Default value: true
        /// </summary>
        [DefaultValue(true)]
        public bool CharacteristicLengthFromPoints { get; set; }

        /// <summary>
        /// Compute mesh element sizes from values given at geometry points defining parametric curves.
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public double CharacteristicLengthFromParametricPoints { get; set; }

        /// <summary>
        /// Minimum number of nodes used to mesh circles and ellipses.
        /// Default value: 7
        /// </summary>
        [DefaultValue(7)]
        public int MinimumCirclePoints { get; set; }

        /// <summary>
        /// Minimum number of points used to mesh curves other than lines, circles and ellipses.
        /// Default value: 3
        /// </summary>
        [DefaultValue(3)]
        public int MinimumCurvePoints { get; set; }

        /// <summary>
        /// Minimum number of elements per 2 * Pi radians when the mesh size is adapted to the curvature. 
        /// Default value: 6
        /// </summary>
        [DefaultValue(6)]
        public int MinimumElementsPerTwoPi { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        ////// Special Common Parameters
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////

        #region Special Common Parameters

        /// <summary>
        /// Maximum anisotropy of the mesh.
        /// Default value: 1e+33
        /// </summary>
        [DefaultValue(1e33)]
        public double AnisoMax { get; set; }

        #endregion
        /// <summary>
        /// Threshold angle below which normals are not smoothed.
        /// Default value: 30
        /// </summary>
        [DefaultValue(30)]
        public double AngleSmoothNormals { get; set; }

        /// <summary>
        /// Consider connected facets as overlapping when the dihedral angle between the facets is smaller than the user’s defined tolerance.
        /// Default value: 0.1
        /// </summary>
        [DefaultValue(0.1)]
        public double AngleToleranceFacetOverlap { get; set; }

        /// <summary>
        /// Threshold angle (in degrees) between faces normals under which we allow an edge swap.
        /// Default value: 10
        /// </summary>
        [DefaultValue(10)]
        public double AllowSwapAngle { get; set; }

        /// <summary>
        /// Seed of pseudo-random number generator.
        /// Default value: 1
        /// </summary>
        [DefaultValue(1)]
        public double RandomSeed { get; set; }

        /// <summary>
        /// Seed of pseudo-random number generator.
        /// Default value: 1
        /// </summary>
        [DefaultValue(1)]
        public double RandomFactor { get; set; }


        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        ////// High Order Parameters
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////

        #region High Order Parameters

        /// <summary>
        /// Maximum number of iterations in high-order optimization pass.
        /// Default value: 100
        /// </summary>
        [DefaultValue(100)]
        public int HighOrderIterMax { get; set; }

        /// <summary>
        /// Number of layers around a problematic element to consider for high-order optimization.
        /// Default value: 6
        /// </summary>
        [DefaultValue(6)]
        public int HighOrderNumLayers { get; set; }

        /// <summary>
        /// Optimize high-order meshes? (0: none, 1: optimization, 2: elastic+optimization, 3: elastic, 4: fast curving).
        /// Default value: 0
        /// </summary>
        [DefaultValue(0)]
        public int HighOrderOptimize { get; set; }

        /// <summary>
        /// Maximum number of high-order optimization passes(moving barrier).
        /// Default value: 25
        /// </summary>
        [DefaultValue(25)]
        public int HighOrderPassMax { get; set; }

        /// <summary>
        /// Force location of nodes for periodic meshes using periodicity transform(0: assume identical parametrisations, 1: invert parametrisations, 2: compute closest point.
        /// Default value: 0
        /// </summary>
        [DefaultValue(0)]
        public int HighOrderPeriodic { get; set; }

        /// <summary>
        /// Poisson ratio of the material used in the elastic smoother for high-order meshes(between -1.0 and 0.5, excluded).
        /// Default value: 0.33
        /// </summary>
        [DefaultValue(0.33)]
        public double HighOrderPoissonRatio { get; set; }

        /// <summary>
        /// Try to fix flipped surface mesh elements in high-order optimizer?
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public bool HighOrderPrimSurfMesh { get; set; }

        /// <summary>
        /// Try to optimize distance to CAD in high-order optimizer?
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public bool HighOrderDistCAD { get; set; }

        /// <summary>
        /// Minimum threshold for high-order element optimization.
        /// Default value: 0.1
        /// </summary>
        [DefaultValue(0.1)]
        public double HighOrderThresholdMin { get; set; }

        /// <summary>
        /// Maximum threshold for high-order element optimization
        /// Default value: 2
        /// </summary>
        [DefaultValue(2)]
        public int HighOrderThresholdMax { get; set; }

        /// <summary>
        /// Create incomplete second order elements? (8-node quads, 20-node hexas, etc.).
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public bool SecondOrderIncomplete { get; set; }

        /// <summary>
        /// Should second order nodes (as well as nodes generated with subdivision algorithms) simply be created by linear interpolation?
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public bool SecondOrderLinear { get; set; }

        #endregion

        #region GH_methods
        public bool IsValid
        {
            get => this.Equals(null);
        }

        public string IsValidWhyNot
        {
            get
            {
                string msg;
                if (this.IsValid) msg = string.Empty;
                else msg = string.Format("Invalid type.", this.TypeName);
                return msg;
            }
        }

        public override string ToString()
        {
            return "IGmshSolverOptions";
        }

        public string TypeName
        {
            get => ToString();
        }

        public string TypeDescription
        {
            get => ToString();
        }

        public IGH_Goo Duplicate()
        {
            return (IGH_Goo)this.MemberwiseClone();
        }

        public IGH_GooProxy EmitProxy()
        {
            return null;
        }

        public bool CastFrom(object source)
        {
            return false;
        }

        public bool CastTo<T>(out T target)
        {
            if (typeof(T).IsAssignableFrom(typeof(IguanaGmshSolver)))
            {
                target = default(T);
                return true;
            }

            target = default(T);
            return false;
        }

        public object ScriptVariable()
        {
            return this;
        }

        public bool Write(GH_IWriter writer)
        {
            return true;
        }

        public bool Read(GH_IReader reader)
        {
            return true;
        }
        #endregion

    }
}
