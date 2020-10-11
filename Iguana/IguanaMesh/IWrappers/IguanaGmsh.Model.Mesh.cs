using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.ICollections;
using Iguana.IguanaMesh.ITypes.IElements;
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace Iguana.IguanaMesh.IWrappers
{
    public static partial class IguanaGmsh
    {
        public enum MeshSolvers3D { Delaunay = 1, InitialMeshOnly = 3, Frontal = 4, MMG3D = 7, RTree = 9, HXT = 10 }

        public static partial class Model
        {
            public static partial class Mesh
            {
                /// <summary>
                /// Generate a mesh of the current model, up to dimension `dim' (0, 1, 2 or 3).
                /// </summary>
                /// <param name="dim"></param>
                public static void Generate(int dim)
                {
                    IWrappers.GmshModelMeshGenerate(dim, ref _ierr);
                }

                /// <summary>
                /// Get the nodes classified on the entity of dimension `dim'.  
                /// </summary>
                /// <param name="nodeTags_out"> `nodeTags_out' contains the node tags (their unique, strictly positive identification numbers).  </param>
                /// <param name="coord_out"> `coord' is a two-dimensional array that contains the x, y, z coordinates of the nodes. </param>
                /// <param name="parametricCoord_out"> If `dim' >= 0, `parametricCoord' contains the parametric coordinates([u1, u2, ...] or [u1, v1, u2, ...]) of the nodes, if available. </param>
                /// <param name="dim"/> If `dim' is negative (default), get all the nodes in the mesh. </param>
                public static void GetNodes(out int[] nodeTags_out, out double[][] coord_out, out double[][] parametricCoord_out, int dim = -1, int tag = -1)
                {
                    IntPtr nodeTags, coord, parametricCoord;
                    long nodeTags_Number, coord_Number, parametricCoord_Number;
                    IWrappers.GmshModelMeshGetNodes(out nodeTags, out nodeTags_Number, out coord, out coord_Number, out parametricCoord, out parametricCoord_Number, dim, tag, Convert.ToInt32(true), Convert.ToInt32(true), ref _ierr);

                    nodeTags_out = new int[nodeTags_Number];
                    coord_out = new double[coord_Number][];
                    parametricCoord_out = new double[parametricCoord_Number][];

                    // Tags
                    if (nodeTags_Number > 0)
                    {
                        // Coordinates
                        var xyz = new double[coord_Number];
                        Marshal.Copy(coord, xyz, 0, (int)coord_Number);
                        // Keys
                        var keys = new long[nodeTags_Number];
                        Marshal.Copy(nodeTags, keys, 0, (int)nodeTags_Number);
                        // uvw
                        var uvw = new double[parametricCoord_Number];
                        Marshal.Copy(parametricCoord, uvw, 0, (int)parametricCoord_Number);

                        for (int i = 0; i < nodeTags_Number; i++)
                        {
                            coord_out[i] = new double[] { xyz[i * 3], xyz[i * 3 + 1], xyz[i * 3 + 2] };
                            nodeTags_out[i] = (int)keys[i];

                            if (dim == 1) parametricCoord_out[i] = new double[] { uvw[i * dim] };
                            else if (dim == 2) parametricCoord_out[i] = new double[] { uvw[i * dim], uvw[i * dim + 1] };
                            else if (dim == 3) parametricCoord_out[i] = new double[] { uvw[i * dim], uvw[i * dim + 1], uvw[i * dim + 2] };
                        }
                    }

                    // Delete unmanaged allocated memory
                    IWrappers.GmshFree(nodeTags);
                    IWrappers.GmshFree(coord);
                    IWrappers.GmshFree(parametricCoord);
                }

                public static bool TryGetIVertexCollection(out IVertexCollection vertices, int dim = -1, int tag = -1)
                {
                    vertices = new IVertexCollection();
                    try
                    {
                        IntPtr nodeTags, coord, parametricCoord;
                        long nodeTags_Number, coord_Number, parametricCoord_Number;
                        IWrappers.GmshModelMeshGetNodes(out nodeTags, out nodeTags_Number, out coord, out coord_Number, out parametricCoord, out parametricCoord_Number, dim, tag, Convert.ToInt32(true), Convert.ToInt32(true), ref _ierr);

                        if (coord_Number > 0 && nodeTags_Number > 0)
                        {
                            // Coordinates
                            var xyz = new double[coord_Number];
                            Marshal.Copy(coord, xyz, 0, (int)coord_Number);
                            // Keys
                            var keys = new long[nodeTags_Number];
                            Marshal.Copy(nodeTags, keys, 0, (int)nodeTags_Number);
                            // uvw
                            var uvw = new double[parametricCoord_Number];
                            Marshal.Copy(parametricCoord, uvw, 0, (int)parametricCoord_Number);

                            for (int i = 0; i < nodeTags_Number; i++)
                            {
                                ITopologicVertex v = new ITopologicVertex(xyz[i * 3], xyz[i * 3 + 1], xyz[i * 3 + 2], (int)keys[i]);
                                vertices.AddVertex((int)keys[i], v);
                            }
                        }

                        // Delete unmanaged allocated memory
                        IWrappers.GmshFree(nodeTags);
                        IWrappers.GmshFree(coord);
                        IWrappers.GmshFree(parametricCoord);

                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

                /// <summary>
                /// Get the elements classified on the entity of dimension `dim'.
                /// `elementTypes' contains the MSH types of the elements (e.g. `2' for 3-node triangles: see `getElementProperties' to obtain the properties for a given element type). 
                /// `elementTags' is a vector of the same length as `elementTypes'; each entry is a vector containing the tags (unique, strictly positive identifiers) of the elements of the corresponding type.
                /// `nodeTags' is also a vector of the same length as `elementTypes'; each entry is a vector of length equal to the number of elements of the given type times the number N of nodes for this type of element, 
                /// that contains the node tags of all the elements of the given type, concatenated: [e1n1, e1n2, ..., e1nN, e2n1, ...]. 
                /// </summary>
                /// <param name="elementTypes"></param>
                /// <param name="elementTypes_n"></param>
                /// <param name="elementTags"></param>
                /// <param name="elementTags_n"></param>
                /// <param name="elementTags_nn"></param>
                /// <param name="nodeTags"></param>
                /// <param name="nodeTags_n"></param>
                /// <param name="nodeTags_nn"></param>
                /// <param name="dim"> If `dim' is negative (default), get all the elements in the mesh.  </param>
                public static void GetElements(out int[] elementTypes_out, out int[][] elementTags_out, out int[][] nodeTags_out, int dim = -1, int tag = -1)
                {
                    IntPtr elementTypes, elementTags, nodeTags, elementTags_n, nodeTags_n;
                    long elementTypes_Number, elementTags_NNumber, nodeTags_NNumber;

                    IWrappers.GmshModelMeshGetElements(out elementTypes, out elementTypes_Number, out elementTags, out elementTags_n, out elementTags_NNumber, out nodeTags, out nodeTags_n, out nodeTags_NNumber, dim, tag, ref _ierr);

                    var types_temp = new long[elementTypes_Number];
                    var eTags_n = new long[elementTags_NNumber];
                    var nTags_n = new long[nodeTags_NNumber];

                    Marshal.Copy(elementTypes, types_temp, 0, (int)elementTypes_Number);
                    Marshal.Copy(elementTags_n, eTags_n, 0, (int)elementTags_NNumber);
                    Marshal.Copy(nodeTags_n, nTags_n, 0, (int)nodeTags_NNumber);

                    var nTags_ptr = new IntPtr[nodeTags_NNumber];
                    var eTags_ptr = new IntPtr[elementTags_NNumber];

                    Marshal.Copy(nodeTags, nTags_ptr, 0, (int)nodeTags_NNumber);
                    Marshal.Copy(elementTags, eTags_ptr, 0, (int)elementTags_NNumber);

                    elementTags_out = new int[elementTags_NNumber][];
                    nodeTags_out = new int[nodeTags_NNumber][];
                    elementTypes_out = IHelpers.ToIntArray(types_temp);

                    for (int i = 0; i < elementTags_NNumber; i++)
                    {
                        // Initializing containers
                        var temp1 = new long[eTags_n[i]];
                        var temp2 = new long[nTags_n[i]];

                        // Marshalling
                        Marshal.Copy(eTags_ptr[i], temp1, 0, (int)eTags_n[i]);
                        Marshal.Copy(nTags_ptr[i], temp2, 0, (int)nTags_n[i]);

                        elementTags_out[i] = IHelpers.ToIntArray(temp1);
                        nodeTags_out[i] = IHelpers.ToIntArray(temp2);
                    }

                    // Delete unmanaged allocated memory
                    IWrappers.GmshFree(elementTypes);
                    IWrappers.GmshFree(elementTags);
                    IWrappers.GmshFree(nodeTags);
                    IWrappers.GmshFree(elementTags_n);
                    IWrappers.GmshFree(nodeTags_n);

                    foreach (IntPtr ptr in nTags_ptr) IWrappers.GmshFree(ptr);
                    foreach (IntPtr ptr in eTags_ptr) IWrappers.GmshFree(ptr);
                }

                /// <summary>
                /// Try to return IElements from the gmsh
                /// </summary>
                /// <param name="dim"> 2 for surface element, 3 for volume elements and -1 for all elements. Default is -1. </param>
                /// <returns></returns>
                public static bool TryGetIElementCollection(out IElementCollection elements, out HashSet<int> parsedNodes, int dim = -1)
                {
                    parsedNodes = new HashSet<int>();
                    elements = new IElementCollection();

                    try
                    {
                        IntPtr elementTypes, elementTags, nodeTags, elementTags_n, nodeTags_n;
                        long elementTypes_Number, elementTags_NNumber, nodeTags_NNumber;

                        IWrappers.GmshModelMeshGetElements(out elementTypes, out elementTypes_Number, out elementTags, out elementTags_n, out elementTags_NNumber, out nodeTags, out nodeTags_n, out nodeTags_NNumber, dim, -1, ref _ierr);

                        var eTypes = new long[elementTypes_Number];
                        var eTags_n = new long[elementTags_NNumber];
                        var nTags_n = new long[nodeTags_NNumber];

                        Marshal.Copy(elementTypes, eTypes, 0, (int)elementTypes_Number);
                        Marshal.Copy(elementTags_n, eTags_n, 0, (int)elementTags_NNumber);
                        Marshal.Copy(nodeTags_n, nTags_n, 0, (int)nodeTags_NNumber);

                        var nTags_ptr = new IntPtr[nodeTags_NNumber];
                        var eTags_ptr = new IntPtr[elementTags_NNumber];

                        Marshal.Copy(nodeTags, nTags_ptr, 0, (int)nodeTags_NNumber);
                        Marshal.Copy(elementTags, eTags_ptr, 0, (int)elementTags_NNumber);

                        var eTags_val = new long[elementTags_NNumber][];
                        var nTags_val = new long[nodeTags_NNumber][];

                        for (int i = 0; i < elementTags_NNumber; i++)
                        {
                            // Initializing containers
                            eTags_val[i] = new long[eTags_n[i]];
                            nTags_val[i] = new long[nTags_n[i]];

                            // Marshalling
                            Marshal.Copy(eTags_ptr[i], eTags_val[i], 0, (int)eTags_n[i]);
                            Marshal.Copy(nTags_ptr[i], nTags_val[i], 0, (int)nTags_n[i]);

                            // Building elements
                            int element_type = (int)eTypes[i];
                            int nodes_per_element = (int)(nTags_n[i] / eTags_n[i]);
                            int number_of_elements = nTags_val[i].Length / nodes_per_element;

                            parsedNodes = IguanaGmshElementType.TryParseToIguanaElement(element_type, nTags_val[i], nodes_per_element, number_of_elements, ref elements);

                        }

                        // Delete unmanaged allocated memory
                        IWrappers.GmshFree(elementTypes);
                        IWrappers.GmshFree(elementTags);
                        IWrappers.GmshFree(nodeTags);
                        IWrappers.GmshFree(elementTags_n);
                        IWrappers.GmshFree(nodeTags_n);

                        foreach (IntPtr ptr in nTags_ptr) IWrappers.GmshFree(ptr);
                        foreach (IntPtr ptr in eTags_ptr) IWrappers.GmshFree(ptr);

                        return true;
                    }
                    catch (Exception) { return false; }
                }

                /// <summary>
                /// Optimize the mesh of the current model using.
                /// <param name="method"> `method' (empty for default tetrahedral mesh optimizer, "Netgen" for Netgen optimizer, "HighOrder" for
                /// direct high-order mesh optimizer, "HighOrderElastic" for high-order elastic smoother, "HighOrderFastCurving" for fast curving algorithm,
                /// "Laplace2D" for Laplace smoothing, "Relocate2D" and "Relocate3D" for node relocation)</param>
                /// <param name="niter"> Number of Iterations. Default is 5. </param>
                public static void Optimize(string method = default, int niter = 5)
                {
                    if (method == default) method = "";
                    IguanaGmsh.IWrappers.GmshModelMeshOptimize(method, -1, niter, null, IntPtr.Zero, ref _ierr);
                }

                /// <summary>
                /// Remove duplicate nodes in the mesh of the current model.
                /// </summary>
                public static void RemoveDuplicateNodes() {
                    IguanaGmsh.IWrappers.GmshModelMeshRemoveDuplicateNodes(ref _ierr);
                }

                /// <summary>
                /// Split (into two triangles) all quadrangles in surface `tag' whose quality is lower than `quality'. 
                /// </summary>
                /// <param name="quality"> Quality of the surface. </param>
                /// <param name="tag"> If `tag' < 0, split quadrangles in all surfaces. </param>
                public static void SplitQuadrangles(double quality, int tag = -1) {
                    IguanaGmsh.IWrappers.GmshModelMeshSplitQuadrangles(quality, tag, ref _ierr);
                }

                /// <summary>
                /// Set a mesh size constraint on the model entities `dimTags'. Currently only entities of dimension 0 (points) are handled.
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="size"></param>
                public static void SetSize(Tuple<int, int>[] dimTags, double size)
                {
                    int[] dimTags_flatten = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrappers.GmshModelMeshSetSize(dimTags_flatten, dimTags_flatten.LongLength, size, ref _ierr);
                }

                /// <summary>
                /// Set mesh size constraints at the given parametric points `parametricCoord'
                /// on the model entity of dimension `dim' and tag `tag'. Currently only
                /// entities of dimension 1 (lines) are handled.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="parametricCoord"></param>
                /// <param name="sizes"></param>
                public static void SetSizeAtParametricPoints(int dim, int tag, double[] parametricCoord, double[] sizes) {
                    IWrappers.GmshModelMeshSetSizeAtParametricPoints(dim, tag, parametricCoord, parametricCoord.LongLength, sizes, sizes.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set a transfinite meshing constraint on the curve `tag', with `numNodes'
                /// nodes distributed according to `meshType' and `coef'. Currently supported
                /// types are "Progression" (geometrical progression with power `coef') and
                /// "Bump" (refinement toward both extremities of the curve).
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="numNodes"></param>
                /// <param name="meshType"></param>
                /// <param name="coef"></param>
                public static void SetTransfiniteCurve(int tag, int numNodes, string meshType = "Progression", double coef = 1)
                {
                    IWrappers.GmshModelMeshSetTransfiniteCurve(tag, numNodes, meshType, coef, ref _ierr);
                }

                /// <summary>
                /// Set a transfinite meshing constraint on the surface `tag'. `arrangement'
                /// describes the arrangement of the triangles when the surface is not flagged
                /// as recombined: currently supported values are "Left", "Right",
                /// "AlternateLeft" and "AlternateRight". `cornerTags' can be used to specify
                /// the(3 or 4) corners of the transfinite interpolation explicitly;
                /// specifying the corners explicitly is mandatory if the surface has more that
                /// 3 or 4 points on its boundary.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="arrangement"></param>
                /// <param name="cornerTags"></param>
                public static void SetTransfiniteSurface(int tag, string arrangement = "Left", int[] cornerTags = default)
                {
                    if (cornerTags == default) cornerTags = new int[0];
                    IWrappers.GmshModelMeshSetTransfiniteSurface(tag, arrangement, cornerTags, cornerTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set a transfinite meshing constraint on the surface `tag'. `cornerTags' can be used to specify the(6 or 8) corners of the transfinite interpolation explicitly.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="cornerTags"></param>
                public static void SetTransfiniteVolume(int tag, int[] cornerTags = default)
                {
                    if (cornerTags == default) cornerTags = new int[0];
                    IWrappers.GmshModelMeshSetTransfiniteVolume(tag, cornerTags, cornerTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set a recombination meshing constraint on the model entity of dimension
                /// `dim' and tag `tag'. Currently only entities of dimension 2 (to recombine
                /// triangles into quadrangles) are supported.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                public static void SetRecombine(int dim, int tag)
                {
                    IWrappers.GmshModelMeshSetRecombine(dim, tag, ref _ierr);
                }

                /// <summary>
                /// Set a smoothing meshing constraint on the model entity of dimension `dim' and tag `tag'. `val' iterations of a Laplace smoother are applied.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"></param>
                public static void SetSmoothing(int dim, int tag, int val)
                {
                    IWrappers.GmshModelMeshSetSmoothing(dim, tag, val, ref _ierr);
                }

                /// <summary>
                /// Set a reverse meshing constraint on the model entity of dimension `dim' and
                /// tag `tag'. If `val' is true, the mesh orientation will be reversed with
                /// respect to the natural mesh orientation(i.e.the orientation consistent with the orientation of the geometry). If `val' is false, the mesh is left as-is.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"></param>
                public static void SetReverse(int dim, int tag, bool val)
                {
                    IWrappers.GmshModelMeshSetReverse(dim, tag, Convert.ToInt32(val), ref _ierr);
                }

                /// <summary>
                /// Set the meshing algorithm on the model entity of dimension `dim' and tag
                /// `tag'. Currently only supported for `dim' == 2.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"></param>
                public static void SetAlgorithm(int dim, int tag, int val)
                {
                    IWrappers.GmshModelMeshSetAlgorithm(dim, tag, val, ref _ierr);
                }

                /// <summary>
                /// Force the mesh size to be extended from the boundary, or not, for the model
                /// entity of dimension `dim' and tag `tag'. 
                /// Currently only supported for `dim' == 2.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"></param>
                public static void SetSizeFromBoundary(int dim, int tag, bool val)
                {
                    IWrappers.GmshModelMeshSetSizeFromBoundary(dim, tag, Convert.ToInt32(val), ref _ierr);
                }

                /// <summary>
                /// Set a compound meshing constraint on the model entities of dimension `dim'
                /// and tags `tags'. During meshing, compound entities are treated as a single
                /// discrete entity, which is automatically reparametrized.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tags"></param>
                public static void SetCompound(int dim, int[] tags)
                {
                    IWrappers.GmshModelMeshSetCompound(dim, tags, tags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set meshing constraints on the bounding surfaces of the volume of tag `tag'
                /// so that all surfaces are oriented with outward pointing normals.Currently
                /// only available with the OpenCASCADE kernel, as it relies on the STL triangulation.
                /// </summary>
                /// <param name="tag"></param>
                public static void SetOutwardOrientation(int tag)
                {
                    IWrappers.GmshModelMeshSetOutwardOrientation(tag, ref _ierr);
                }

                /// <summary>
                /// Renumber the node tags in a continuous sequence.
                /// </summary>
                public static void RenumberNodes()
                {
                    IWrappers.GmshModelMeshRenumberNodes(ref _ierr);
                }

                /// <summary>
                /// Renumber the element tags in a continuous sequence.
                /// </summary>
                public static void RenumberElements()
                {
                    IWrappers.GmshModelMeshRenumberElements(ref _ierr);
                }

                /// <summary>
                /// Set the meshes of the entities of dimension `dim' and tag `tags' as
                /// periodic copies of the meshes of entities `tagsMaster', using the affine
                /// transformation specified in `affineTransformation' (16 entries of a 4x4
                /// matrix, by row). If used after meshing, generate the periodic node
                /// correspondence information assuming the meshes of entities `tags'
                /// effectively match the meshes of entities `tagsMaster' (useful for
                /// structured and extruded meshes). Currently only available for @code{dim} == 1 and @code { dim } == 2. 
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tags"></param>
                /// <param name="tagsMaster"></param>
                /// <param name="affineTransform"></param>
                public static void SetPeriodic(int dim, int[] tags, int[] tagsMaster, double[] affineTransform)
                {
                    IWrappers.GmshModelMeshSetPeriodic(dim, tags, tags.LongLength, tagsMaster, tagsMaster.LongLength, affineTransform, affineTransform.LongLength, ref _ierr);
                }

                /// <summary>
                /// Classify ("color") the surface mesh based on the angle threshold `angle'
                /// (in radians), and create new discrete surfaces, curves and points
                /// accordingly.If `boundary' is set, also create discrete curves on the 
                /// boundary if the surface is open.If `forReparametrization' is set, create
                /// edges and surfaces that can be reparametrized using a single map.If
                /// `curveAngle' is less than Pi, also force curves to be split according to
                /// `curveAngle'.
                /// </summary>
                /// <param name="angle"></param>
                /// <param name="boundary"></param>
                /// <param name="forReparametrization"></param>
                /// <param name="curveAngle"></param>
                public static void ClassifySurfaces(double angle, bool boundary, bool forReparametrization, double curveAngle)
                {
                    IWrappers.GmshModelMeshClassifySurfaces(angle, Convert.ToInt32(boundary), Convert.ToInt32(forReparametrization), curveAngle, ref _ierr);
                }

                /// <summary>
                /// Create a geometry for the discrete entities `dimTags' (represented solely
                /// by a mesh, without an underlying CAD description), i.e.create a
                /// parametrization for discrete curves and surfaces, assuming that each can be
                /// parametrized with a single map.If `dimTags' is empty, create a geometry
                /// for all the discrete entities.
                /// </summary>
                /// <param name="dimTags"></param>
                public static void CreateGeometry(long[] dimTags=default)
                {
                    if (dimTags == default) dimTags = new long[0];
                    IWrappers.GmshModelMeshCreateGeometry(dimTags, dimTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Create a boundary representation from the mesh if the model does not have
                /// one(e.g.when imported from mesh file formats with no BRep representation
                /// of the underlying model). If `makeSimplyConnected' is set, enforce simply 
                /// connected discrete surfaces and volumes.If `exportDiscrete' is set, clear
                /// any built-in CAD kernel entities and export the discrete entities in the
                /// built-in CAD kernel.
                /// </summary>
                /// <param name="makeSimplyConnected"></param>
                /// <param name="exportDiscrete"></param>
                public static void CreateTopology(bool makeSimplyConnected, bool exportDiscrete)
                {
                    IWrappers.GmshModelMeshCreateTopology(Convert.ToInt32(makeSimplyConnected), Convert.ToInt32(exportDiscrete), ref _ierr);
                }

                /// <summary>
                /// Compute a basis representation for homology spaces after a mesh has been
                /// generated.The computation domain is given in a list of physical group tags
                /// `domainTags'; if empty, the whole mesh is the domain. The computation
                /// subdomain for relative homology computation is given in a list of physical
                /// group tags `subdomainTags'; if empty, absolute homology is computed. The
                /// dimensions homology bases to be computed are given in the list `dim'; if
                /// empty, all bases are computed.Resulting basis representation chains are
                /// stored as physical groups in the mesh.
                /// </summary>
                /// <param name="domainTags"></param>
                /// <param name="subdomainTags"></param>
                /// <param name="dims"></param>
                public static void ComputeHomology(int[] domainTags, int[] subdomainTags, int[] dims)
                {
                    IWrappers.GmshModelMeshComputeHomology(domainTags, domainTags.LongLength, subdomainTags, subdomainTags.LongLength, dims, dims.LongLength, ref _ierr);
                }

                /// <summary>
                /// Compute a basis representation for cohomology spaces after a mesh has been
                /// generated.The computation domain is given in a list of physical group tags
                /// `domainTags'; if empty, the whole mesh is the domain. The computation
                /// subdomain for relative cohomology computation is given in a list of
                /// physical group tags `subdomainTags'; if empty, absolute cohomology is
                /// computed.The dimensions homology bases to be computed are given in the
                /// list `dim'; if empty, all bases are computed. Resulting basis
                /// representation cochains are stored as physical groups in the mesh.
                /// </summary>
                /// <param name="domainTags"></param>
                /// <param name="subdomainTags"></param>
                /// <param name="dims"></param>
                public static void ComputeCohomology(int[] domainTags, int[] subdomainTags, [In, Out] int[] dims)
                {
                    IWrappers.GmshModelMeshComputeCohomology(domainTags, domainTags.LongLength, subdomainTags, subdomainTags.LongLength, dims, dims.LongLength, ref _ierr);
                }

                /// <summary>
                /// Compute a cross field for the current mesh. The function creates 3 views: the H function, the Theta function and cross directions.
                /// Return the tags of the views
                /// </summary>
                /// <param name="viewTags_out"></param>
                public static void MeshComputeCrossField(out int[] viewTags_out)
                {
                    IntPtr viewTags;
                    long viewTags_n;
                    IWrappers.GmshModelMeshComputeCrossField(out viewTags, out viewTags_n, ref _ierr);

                    viewTags_out = null;
                    if (viewTags_n > 0)
                    {
                        viewTags_out = new int[viewTags_n];
                        Marshal.Copy(viewTags, viewTags_out, 0, (int)viewTags_n);
                    }
                    IWrappers.GmshFree(viewTags);
                }

                /// <summary>
                /// Add a new mesh size field of type `fieldType'. 
                /// If `tag' is positive, assign the tag explicitly; otherwise a new tag is assigned automatically.Return the field tag.
                /// </summary>
                /// <param name="fieldType"></param>
                /// <param name="tag"></param>
                /// <returns></returns>
                public static int MeshFieldAdd(string fieldType, int tag = -1)
                {
                    return IWrappers.GmshModelMeshFieldAdd(fieldType, tag, ref _ierr);
                }

                /// <summary>
                /// Remove the field with tag `tag'.
                /// </summary>
                /// <param name="tag"></param>
                public static void MeshFieldRemove(int tag)
                {
                    IWrappers.GmshModelMeshFieldRemove(tag, ref _ierr);
                }

                /// <summary>
                /// Set the numerical option `option' to value `value' for field `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="option"></param>
                /// <param name="value"></param>
                public static void MeshFieldSetNumber(int tag, string option, double value)
                {
                    IWrappers.GmshModelMeshFieldSetNumber(tag, option, value, ref _ierr);
                }

                /// <summary>
                /// Set the string option `option' to value `value' for field `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="option"></param>
                /// <param name="value"></param>
                public static void MeshFieldSetString(int tag, string option, string value)
                {
                    IWrappers.GmshModelMeshFieldSetString(tag, option, value, ref _ierr);
                }

                /// <summary>
                /// Set the numerical list option `option' to value `value' for field `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="option"></param>
                /// <param name="value"></param>
                public static void MeshFieldSetNumbers(int tag, string option, double[] value)
                {
                    IWrappers.GmshModelMeshFieldSetNumbers(tag, option, value, value.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set the field `tag' as the background mesh size field. 
                /// </summary>
                /// <param name="tag"></param>
                public static void MeshFieldSetAsBackgroundMesh(int tag)
                {
                    IWrappers.GmshModelMeshFieldSetAsBackgroundMesh(tag, ref _ierr);
                }

                /// <summary>
                /// Set the field `tag' as a boundary layer size field.
                /// </summary>
                /// <param name="tag"></param>
                public static void MeshFieldSetAsBoundaryLayer(int tag)
                {
                    IWrappers.GmshModelMeshFieldSetAsBoundaryLayer(tag, ref _ierr);
                }

                /// <summary>
                /// Refine the mesh of the current model by uniformly splitting the elements.
                /// </summary>
                public static void Refine()
                {
                    IWrappers.GmshModelMeshRefine(ref _ierr);
                }

                /// <summary>
                /// Recombine the mesh of the current model.
                /// </summary>
                public static void Recombine()
                {
                    IWrappers.GmshModelMeshRecombine(ref _ierr);
                }

                /// <summary>
                /// Embed the model entities of dimension `dim' and tags `tags' in the (`inDim', `inTag') model entity.
                /// The embedded entities should not be part of the boundary of the entity `inTag', whose mesh will conform to the mesh of the embedded entities.
                /// </summary>
                /// <param name="dim"> The dimension `dim' can 0, 1 or 2 and must be strictly smaller than `inDim', which must be either 2 or 3. </param>
                /// <param name="tags"></param>
                /// <param name="inDim"></param>
                /// <param name="inTag"></param>
                public static void Embed(int dim, int[] tags, int inDim, int inTag)
                {
                    IWrappers.GmshModelMeshEmbed(dim, tags, tags.LongLength, inDim, inTag, ref _ierr);
                }

                /// <summary>
                /// Remove embedded entities from the model entities `dimTags'. if `dim' is >= 0, only remove embedded entities of the given dimension (e.g.embedded points if `dim' == 0).
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="dim"></param>
                public static void RemoveEmbedded(int[] dimTags, int dim) {
                    IWrappers.GmshModelMeshRemoveEmbedded(dimTags, dimTags.LongLength, dim, ref _ierr);
                }

                /// <summary>
                /// Reclassify all nodes on their associated model entity, based on the
                /// elements.Can be used when importing nodes in bulk(e.g.by associating
                /// them all to a single volume), to reclassify them correctly on model
                /// surfaces, curves, etc.after the elements have been set.
                /// </summary>
                public static void ReclassifyNodes()
                {
                    IWrappers.GmshModelMeshReclassifyNodes(ref _ierr);
                }

                /// <summary>
                /// Add nodes classified on the model entity of dimension `dim' and tag `tag'.
                /// `nodeTags' contains the node tags (their unique, strictly positive
                /// identification numbers). `coord' is a vector of length 3 times the length
                /// of `nodeTags' that contains the x, y, z coordinates of the nodes,
                /// concatenated: [n1x, n1y, n1z, n2x, ...]. The optional `parametricCoord'
                /// vector contains the parametric coordinates of the nodes, if any.The length
                /// of `parametricCoord' can be 0 or `dim' times the length of `nodeTags'. If
                /// the `nodeTags' vector is empty, new tags are automatically assigned to the
                /// nodes.
                /// </summary>
                public static void AddNodes(int dim, int tag, long[] nodeTags, double[] coord, double[] parametricCoord = default)
                {
                    if (parametricCoord == default) parametricCoord = new double[0];
                    IWrappers.GmshModelMeshAddNodes(dim, tag, nodeTags, nodeTags.LongLength, coord, coord.LongLength, parametricCoord, parametricCoord.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add nodes classified on the model entity of dimension `dim' and tag `tag'.
                /// `nodeTags' contains the node tags (their unique, strictly positive
                /// identification numbers). `coord' is a vector of length 3 times the length
                /// of `nodeTags' that contains the x, y, z coordinates of the nodes,
                /// concatenated: [n1x, n1y, n1z, n2x, ...]. The optional `parametricCoord'
                /// vector contains the parametric coordinates of the nodes, if any.The length
                /// of `parametricCoord' can be 0 or `dim' times the length of `nodeTags'. If
                /// the `nodeTags' vector is empty, new tags are automatically assigned to the
                /// nodes.
                /// </summary>
                public static void AddNodes(int tag, MeshVertexList vertices, MeshTextureCoordinateList textures)
                {
                    long[] nodeTags;
                    double[] coord;
                    IHelpers.ParseRhinoVertices(vertices, out coord, out nodeTags);
                    double[] parametricCoord;
                    IHelpers.ParseRhinoTextures(textures, out parametricCoord);

                    AddNodes(2, tag, nodeTags, coord, parametricCoord);
                }

                /// <summary>
                /// Add elements classified on the entity of dimension `dim' and tag `tag'.
                /// `types' contains the MSH types of the elements (e.g. `2' for 3-node
                /// triangles: see the Gmsh reference manual). `elementTags' is a vector of the
                /// same length as `types'; each entry is a vector containing the tags (unique,
                /// strictly positive identifiers) of the elements of the corresponding type.
                /// `nodeTags' is also a vector of the same length as `types'; each entry is a
                /// vector of length equal to the number of elements of the given type times
                /// the number N of nodes per element, that contains the node tags of all the
                /// elements of the given type, concatenated: [e1n1, e1n2, ..., e1nN, e2n1,...].
                /// </summary>
                public static void AddElements(int dim, int tag, long[] elementTypes, int[][] elementTags, int[][] nodeTags)
                {
                    int[] elementTags_flatten = IHelpers.FlattenIntArray(elementTags);
                    int[] nodesTags_flatten = IHelpers.FlattenIntArray(nodeTags);
                    IntPtr elementTags_parse = Marshal.AllocCoTaskMem(elementTags_flatten.Length);
                    IntPtr nodesTags_parse = Marshal.AllocCoTaskMem(nodesTags_flatten.Length);

                    IWrappers.GmshModelMeshAddElements(dim, tag, elementTypes, elementTypes.LongLength, elementTags_parse, elementTypes.LongLength, elementTags_flatten.LongLength, nodesTags_parse, elementTypes.LongLength, nodesTags_flatten.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add elements classified on the entity of dimension `dim' and tag `tag'.
                /// `types' contains the MSH types of the elements (e.g. `2' for 3-node
                /// triangles: see the Gmsh reference manual). `elementTags' is a vector of the
                /// same length as `types'; each entry is a vector containing the tags (unique,
                /// strictly positive identifiers) of the elements of the corresponding type.
                /// `nodeTags' is also a vector of the same length as `types'; each entry is a
                /// vector of length equal to the number of elements of the given type times
                /// the number N of nodes per element, that contains the node tags of all the
                /// elements of the given type, concatenated: [e1n1, e1n2, ..., e1nN, e2n1,...].
                /// </summary>
                public static void AddElements(int tag, MeshFaceList faces)
                {
                    MeshFace f;
                    List<int> tagsT = new List<int>();
                    List<int> nodesT = new List<int>();
                    List<int> tagsQ = new List<int>();
                    List<int> nodesQ = new List<int>();

                    for (int i = 0; i < faces.Count; i++)
                    {
                        f = faces[i];
                        if (f.IsTriangle)
                        {
                            tagsT.Add(i);
                            nodesT.AddRange(new int[] { f.A, f.B, f.C });
                        }
                        else if (f.IsQuad)
                        {
                            tagsQ.Add(i);
                            nodesQ.AddRange(new int[] { f.A, f.B, f.C, f.D });
                        }
                    }

                    long[] elementTypes = new long[0];
                    if (tagsT.Count > 0 && tagsQ.Count > 0) elementTypes = new long[] { 2, 3 };
                    else if (tagsT.Count > 0 && tagsQ.Count == 0) elementTypes = new long[] { 2 };
                    else if (tagsT.Count == 0 && tagsQ.Count > 0) elementTypes = new long[] { 3 };

                    int[] elementTags_flatten = tagsT.Concat(tagsQ).ToArray();
                    int[] nodesTags_flatten = nodesT.Concat(nodesQ).ToArray();
                    IntPtr elementTags_parse = Marshal.AllocCoTaskMem(elementTags_flatten.Length);
                    IntPtr nodesTags_parse = Marshal.AllocCoTaskMem(nodesTags_flatten.Length);

                    IWrappers.GmshModelMeshAddElements(2, tag, elementTypes, elementTypes.LongLength, elementTags_parse, elementTypes.LongLength, elementTags_flatten.LongLength, nodesTags_parse, elementTypes.LongLength, nodesTags_flatten.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add elements of type `elementType' classified on the entity of tag `tag'.
                /// `elementTags' contains the tags (unique, strictly positive identifiers) of
                /// the elements of the corresponding type. `nodeTags' is a vector of length
                /// equal to the number of elements times the number N of nodes per element,
                /// that contains the node tags of all the elements, concatenated: [e1n1, e1n2,
                /// ..., e1nN, e2n1, ...]. If the `elementTag' vector is empty, new tags are
                /// automatically assigned to the elements.
                /// </summary>
                public static void AddElementsByType(int tag, int elementType, long[] elementTags, long[] nodeTags) {
                    IWrappers.GmshModelMeshAddElementsByType(tag, elementType, elementTags, elementTags.LongLength, nodeTags, nodeTags.LongLength, ref _ierr);
                }
            }
        }
    }
}
