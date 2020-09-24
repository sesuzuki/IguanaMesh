using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using System.Collections.Generic;
using System.ComponentModel;

namespace Iguana.IguanaMesh.IGmshWrappers
{
    public enum MeshSolvers2D { MeshAdapt = 1, Automatic = 2, InitialMeshOnly = 3, Delaunay = 5, TriFrontalDelaunay = 6, BAMG = 7, QuadsFrontalDelaunay = 8, PackingOfParallelograms = 9 }

    public class IguanaGmshSolverOptions
    {
        private List<double> meshSizes = new List<double>() { 1.0 };
        /// <summary>
        /// 2D mesh algorithm (1: MeshAdapt, 2: Automatic, 3: Initial mesh only, 5: Delaunay, 6: Frontal-Delaunay, 7: BAMG, 8: Frontal-Delaunay for Quads, 9: Packing of Parallelograms). Default value: 6
        /// 3D mesh algorithm (1: Delaunay, 3: Initial mesh only, 4: Frontal, 7: MMG3D, 9: R-tree, 10: HXT). Default value: 1
        /// </summary>
        public int MeshingAlgorithm { get; set; }

        /// <summary>
        /// Optimization method (Standard, Netgen, HighOrder, HighOrderElastic, HighOrderFastCurving, Laplace2D, Relocate2D, Relocate3D)
        /// </summary>
        public string OptimizationAlgorithm { get; set; }

        /// <summary>
        /// Target mesh size at input nodes. If the number of size values is not equal to the number of nodes, the first item of the size value list is assigned to all nodes.
        /// Default favlue is 1.0;
        /// </summary>
        public List<double> TargetMeshSize {
            get => meshSizes;
            set => meshSizes = value;
        }

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
        /// Maximum anisotropy of the mesh.
        /// Default value: 1e+33
        /// </summary>
        [DefaultValue(1e33)]
        public double AnisoMax { get; set; }

        /// <summary>
        /// Threshold angle (in degrees) between faces normals under which we allow an edge swap.
        /// Default value: 10
        /// </summary>
        [DefaultValue(10)]
        public double AllowSwapAngle { get; set; }

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
        /// Element order(1: first order elements).
        /// Default value: 1
        /// </summary>
        [DefaultValue(1)]
        public int ElementOrder { get; set; }

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
        /// Optimize the mesh using Netgen to improve the quality of tetrahedral elements.
        /// Default value: 0
        /// </summary>
        [DefaultValue(false)]
        public bool OptimizeNetgen { get; set; }

        /// <summary>
        /// Mesh recombination algorithm(0: simple, 1: blossom, 2: simple full-quad, 3: blossom full-quad)
        /// For recombine the current mesh into quadrangles. This operation triggers a synchronization of the CAD model with the internal Gmsh model. 
        /// Default value: 1
        /// </summary>
        [DefaultValue(1)]
        public int RecombinationAlgorithm { get; set; }

        /// <summary>
        /// Apply recombination algorithm to all surfaces, ignoring per-surface spec.
        /// Default value: 0
        /// </summary>
        [DefaultValue(false)]
        public bool RecombineAll { get; set; }

        /// <summary>
        /// Number of topological optimization passes (removal of diamonds, ...) of recombined surface meshes.
        /// Default value: 5
        /// </summary>
        [DefaultValue(5)]
        public int RecombineOptimizeTopology { get; set; }

        /// <summary>
        /// Apply recombination3D algorithm to all volumes, ignoring per-volume spec (experimental).
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

        /// <summary>
        /// Number of refinement steps in the MeshAdapt-based 2D algorithms.
        /// Default value: 10
        /// </summary>
        [DefaultValue(10)]
        public int RefineSteps { get; set; }

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


        /// <summary>
        /// Number of smoothing steps applied to the final mesh.
        /// Default value: 1
        /// </summary>
        [DefaultValue(1)]
        public int Smoothing { get; set; }

        /// <summary>
        /// Apply n barycentric smoothing passes to the 3D cross field
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public bool SmoothCrossField { get; set; }

        /// <summary>
        /// Use closest point to compute 2D crossfield.
        /// Default value: true
        /// </summary>
        [DefaultValue(true)]
        public bool CrossFieldClosestPoint { get; set; }

        /// <summary>
        /// Smooth the mesh normals?
        /// Default value: 0
        /// </summary>
        [DefaultValue(false)]
        public bool SmoothNormals { get; set; }


        /// <summary>
        /// Ratio between mesh sizes at nodes of a same edge (used in BAMG).
        /// Default value: 1.8
        /// </summary>
        [DefaultValue(1.8)]
        public double SmoothRatio { get; set; }

        /// <summary>
        /// Mesh subdivision algorithm(0: all quadrangles, 1: all hexahedra, 2: barycentric).
        /// Default value: 0
        /// </summary>
        [DefaultValue(0)]
        public int SubdivisionAlgorithm { get; set; }

        /// <summary>
        /// Mesh subdivision algorithm(0: all quadrangles, 1: all hexahedra, 2: barycentric).
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public bool Subdivide { get; set; }


        /// <summary>
        /// Skip a model edge in mesh generation if its length is less than user’s defined tolerance.
        /// Default value: false
        /// </summary>
        [DefaultValue(false)]
        public int ToleranceEdgeLength { get; set; }

        /// <summary>
        /// Tolerance for initial 3D Delaunay mesher.
        /// Default value: 1e-08
        /// </summary>
        [DefaultValue(1e-08)]
        public double ToleranceInitialDelaunay { get; set; }

        public void ApplyBasicPreProcessing2D()
        {
            Gmsh.Option.SetNumber("Mesh.Algorithm", (int) MeshingAlgorithm);
            Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFactor", CharacteristicLengthFactor);
            Gmsh.Option.SetNumber("Mesh.CharacteristicLengthMin", CharacteristicLengthMin);
            Gmsh.Option.SetNumber("Mesh.CharacteristicLengthMax", CharacteristicLengthMax);

            if (CharacteristicLengthFromCurvature)
            {
                Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFromParametricPoints", 0);
                Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 1);
            }
            else
            {
                Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 0);
                Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFromParametricPoints", 1);
            }
        }

        public void ApplyBasicPostProcessing2D()
        {
            if (RecombineAll)
            {
                Gmsh.Option.SetNumber("Mesh.RecombinationAlgorithm", RecombinationAlgorithm);
                Gmsh.Model.Mesh.Recombine();
            }

            if (Subdivide)
            {
                Gmsh.Option.SetNumber("Mesh.SubdivisionAlgorithm", SubdivisionAlgorithm);
                Gmsh.Model.Mesh.Refine();
            }

            if (Optimize)
            {
                Gmsh.Model.Mesh.Optimize(OptimizationAlgorithm, Smoothing);
            }
        }

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
            if (typeof(T).IsAssignableFrom(typeof(IguanaGmshSolverOptions)))
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
