using Iguana.IguanaMesh.IUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers
{
    public static partial class IguanaGmsh
    {
        public static partial class Model
        {
            public static partial class Geo
            {

                /// <summary>
                /// Add a geometrical point in the built-in CAD representation, at coordinates (`x', `y', `z').
                /// (Note that the point will be added in the current model only after `synchronize' is called. 
                /// This behavior holds for all the entities added in the geo module.) 
                /// Return the tag of the point. 
                /// </summary>
                /// <param name="x"> X coordinate </param>
                /// <param name="y"> Y coordinate </param>
                /// <param name="z"> Z coordinate </param>
                /// <param name="meshSize"> If `meshSize' is > 0, add a meshing constraint at that point </param>
                /// <param name="tag"> Point Tag. If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically </param>
                /// <returns></returns>
                public static int AddPoint(double x, double y, double z, double meshSize = -1, int tag = -1)
                {
                    return IWrappers.GmshModelGeoAddPoint(x, y, z, meshSize, tag, ref _ierr);
                }

                /// <summary>
                /// Add a straight line segment between the two points with tags `startTag' and `endTag'.
                /// Return the tag of the line.
                /// </summary>
                /// <param name="startTag"> Tag of the starting point </param>
                /// <param name="endTag"> Tag of the end point </param>
                /// <param name="tag"> Line Tag. If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddLine(int startTag, int endTag, int tag = -1)
                {
                    return IWrappers.GmshModelGeoAddLine(startTag, endTag, tag, ref _ierr);
                }

                /// <summary>
                /// Add a circle arc (strictly smaller than Pi) between the two points with tags `startTag' and `endTag', with center `centertag'. 
                /// If(`nx', `ny', `nz') != (0, 0, 0), explicitly set the plane of the circle arc. 
                /// Return the tag of the circle arc.
                /// </summary>
                /// <param name="startTag"></param>
                /// <param name="centerTag"></param>
                /// <param name="endTag"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="nx"></param>
                /// <param name="ny"></param>
                /// <param name="nz"></param>
                /// <returns></returns>
                public static int AddCircleArc(int startTag, int centerTag, int endTag, int tag = -1, double nx = 0.0, double ny = 0.0, double nz = 0.0)
                {
                    return IWrappers.GmshModelGeoAddCircleArc(startTag, centerTag, endTag, tag, nx, ny, nz, ref _ierr);
                }

                /// <summary>
                /// Add an ellipse arc (strictly smaller than Pi) between the two points `startTag' and `endTag', with center `centerTag' and major axis point `majorTag'. 
                /// If(`nx', `ny', `nz') != (0, 0, 0), explicitly set the plane of the circle arc. 
                /// Return the tag of the ellipse arc.
                /// </summary>
                /// <param name="startTag"></param>
                /// <param name="centerTag"></param>
                /// <param name="majorTag"></param>
                /// <param name="endTag"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="nx"></param>
                /// <param name="ny"></param>
                /// <param name="nz"></param>
                /// <returns></returns>
                public static int AddEllipseArc(int startTag, int centerTag, int majorTag, int endTag, int tag = -1, double nx = 0.0, double ny = 0.0, double nz = 0.0)
                {
                    return IWrappers.GmshModelGeoAddEllipseArc(startTag, centerTag, majorTag, endTag, tag, nx, ny, nz, ref _ierr);
                }

                /// <summary>
                /// Add a closed polyline (a curve loop in gmsh) formed by the curves `curveTags'. 
                /// Return the tag of the closed polyline.
                /// </summary>
                /// <param name="curveTags"> `curveTags' should contain (signed) tags of model enties of dimension 1 forming a closed loop: a negative tag signifies that the underlying curve is considered with reversed orientation. </param>
                /// <param name="tag"> Polyline Tag. If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddCurveLoop(int[] curveTags, int tag = -1)
                {
                    int tag_crv = IWrappers.GmshModelGeoAddCurveLoop(curveTags, curveTags.Length, tag, ref _ierr);
                    return tag_crv;
                }

                /// <summary>
                /// Add a spline (Catmull-Rom) curve going through the points `pointTags'. 
                /// Create a periodic curve if the first and last points are the same.
                /// Return the tag of the spline curve.
                /// </summary>
                /// <param name="pointTags"> Tags of points constituing the polyline. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected  automatically. </param>
                /// <returns></returns>
                public static int AddSpline(int[] pointTags, int tag = -1)
                {
                    return IWrappers.GmshModelGeoAddSpline(pointTags, pointTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a cubic b-spline curve with `pointTags' control points.
                /// Creates a periodic curve if the first and last points are the same. Return the tag of the b-spline curve.
                /// </summary>
                /// <param name="pointTags"> Tags of points constituing the polyline. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected  automatically. </param>
                /// <returns></returns>
                public static int AddBSpline(int[] pointTags, int tag = -1)
                {
                    return IWrappers.GmshModelGeoAddBSpline(pointTags, pointTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a Bezier curve with `pointTags' control points. 
                /// Return the tag of the Bezier curve.
                /// </summary>
                /// <param name="pointTags"> Tags of points constituing the polyline. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected  automatically. </param>
                /// <returns></returns>
                public static int AddBezier(int[] pointTags, int tag = -1)
                {
                    return IWrappers.GmshModelGeoAddBezier(pointTags, pointTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a spline (Catmull-Rom) going through points sampling the curves in `curveTags'. 
                /// Return the tag of the spline.
                /// </summary>
                /// <param name="curveTags"> Tags of curves constituing the compund Spline. </param>
                /// <param name="numIntervals"> The density of sampling points on each curve. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected  automatically. </param>
                /// <returns></returns>
                public static int AddCompoundSpline(int[] curveTags, int numIntervals, int tag = -1)
                {
                    return IWrappers.GmshModelGeoAddCompoundSpline(curveTags, curveTags.LongLength, numIntervals, tag, ref _ierr);
                }

                /// <summary>
                /// Add a b-spline with control points sampling the curves in `curveTags'.
                /// Return the tag of the b-spline.
                /// </summary>
                /// <param name="curveTags"> Tags of curves constituing the compund Spline. </param>
                /// <param name="numIntervals"> The density of sampling points on each curve.  </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected  automatically. </param>
                /// <returns></returns>
                public static int AddCompoundBSpline(int[] curveTags, int numIntervals, int tag = -1)
                {
                    return IWrappers.GmshModelGeoAddCompoundBSpline(curveTags, curveTags.LongLength, numIntervals, tag, ref _ierr);
                }

                /// <summary>
                /// Add a plane surface defined by one or more closed polylines. 
                /// </summary>
                /// <param name="wireTags"> The first closed polyline in `wireTags' defines the exterior contour; additional curve loop define holes. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.Return the tag of the surface. </param>
                /// <returns></returns>
                public static int AddPlaneSurface(int[] wireTags, int tag = -1)
                {
                    return IWrappers.GmshModelGeoAddPlaneSurface(wireTags, wireTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a surface filling the curve loops in `wireTags'. 
                /// Currently only a single curve loop is supported; 
                /// This curve loop should be composed by 3 or 4 curves only.
                /// Return the tag of the surface.
                /// </summary>
                /// <param name="wireTag"> The tag of the closed curve loop. </param>
                /// <param name="sphereCenterTag"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected  automatically. </param>
                /// <returns></returns>
                public static int AddSurfaceFilling(int wireTag, int sphereCenterTag = -1, int tag = -1)
                {
                    return IWrappers.GmshModelGeoAddSurfaceFilling(new int[] { wireTag }, 1, tag, sphereCenterTag, ref _ierr);
                }

                /// <summary>
                /// Add a surface loop (a closed shell) formed by `surfaceTags'.  
                /// Return the tag of the shell.
                /// </summary>
                /// <param name="surfaceTags"> Tag of the surfaces. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected  automatically. </param>
                /// <returns></returns>
                public static int AddSurfaceLoop(int[] surfaceTags, int tag = -1)
                {
                    return IWrappers.GmshModelGeoAddSurfaceLoop(surfaceTags, surfaceTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a volume (a region) defined by one or more shells `shellTags'. 
                /// Return the tag of the volume.
                /// </summary>
                /// <param name="shellTags"> The first surface loop defines the exterior boundary; additional surface loop define holes. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected  automatically. </param>
                /// <returns></returns>
                public static int AddVolume(int[] shellTags, int tag = -1)
                {
                    return IWrappers.GmshModelGeoAddVolume(shellTags, shellTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Extrude the model entities `dimTags' by translation along (`dx', `dy',`dz')
                /// If `dx' == `dy' == `dz' == 0, the entities are extruded along their normal.
                /// Return extruded entities in `outDimTags'.
                /// </summary>
                /// <param name="dimTags"> Contains the node tags of all the entities to extrude, concatenated: [dim1, tag, ..., dimN, tagN, ...]</param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="outDimTags"></param>
                /// <param name="numElements"> If `numElements' is not empty, also extrude the mesh: the entries in `numElements' give the number of elements in each layer. </param>
                /// <param name="heights"> If `height' is not empty, it provides the (cumulative) height of the different layers, normalized to 1.  </param>
                /// <param name="recombine"></param>
                public static void Extrude(Tuple<int, int>[] dimTags, double dx, double dy, double dz, out Tuple<int, int>[] outDimTags, int[] numElements = default, double[] heights = default, bool recombine = false)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IntPtr out_DimTags;
                    long outDimTags_n;
                    if (numElements == default) numElements = new int[0];
                    if (heights == default) heights = new double[0];

                    IWrappers.GmshModelGeoExtrude(arr, arr.LongLength, dx, dy, dz, out out_DimTags, out outDimTags_n, numElements, numElements.LongLength, heights, heights.LongLength, Convert.ToInt32(recombine), ref _ierr);

                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(out_DimTags, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrappers.GmshFree(out_DimTags);
                }




                /// <summary>
                /// Extrude the model entities `dimTags' by rotation of `angle' radians around the axis of revolution defined by the point(`x', `y', `z') and the direction (`ax', `ay', `az'). 
                /// The angle should be strictly smaller than Pi.
                /// Return extruded entities in `outDimTags'.
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="ax"></param>
                /// <param name="ay"></param>
                /// <param name="az"></param>
                /// <param name="angle"></param>
                /// <param name="outDimTags"></param>
                /// <param name="numElements"> If `numElements' is not empty, also extrude the mesh: the entries in `numElements' give the number of elements in each layer. </param>
                /// <param name="heights"> If `height' is not empty, it provides the (cumulative) height of the different layers, normalized to 1.</param>
                /// <param name="recombine"></param>
                public static void Revolve(Tuple<int, int>[] dimTags, double x, double y, double z, double ax, double ay, double az, double angle, out Tuple<int, int>[] outDimTags, int[] numElements = default, double[] heights = default, bool recombine = false)
                {
                    IntPtr out_DimTags;
                    long outDimTags_n;
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    if (numElements == default) numElements = new int[0];
                    if (heights == default) heights = new double[0];

                    IWrappers.GmshModelGeoRevolve(arr, arr.LongLength, x, y, z, ax, ay, az, angle, out out_DimTags, out outDimTags_n, numElements, numElements.LongLength, heights, heights.LongLength, Convert.ToInt32(recombine), ref _ierr);

                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(out_DimTags, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrappers.GmshFree(out_DimTags);
                }

                /// <summary>
                /// Extrude the model entities `dimTags' by a combined translation and rotation of `angle' radians, along (`dx', `dy', `dz') and around the axis of revolution defined by the point(`x', `y', `z') and the direction (`ax', ay', `az'). 
                /// The angle should be strictly smaller than Pi.Return extruded entities in `outDimTags'.
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="ax"></param>
                /// <param name="ay"></param>
                /// <param name="az"></param>
                /// <param name="angle"></param>
                /// <param name="outDimTags"></param>
                /// <param name="numElements"> If `numElements' is not empty, also extrude the mesh: the entries in `numElements' give the number of elements in each layer. </param>
                /// <param name="heights"> If `height' is not empty, it provides the (cumulative) height of the different layers, normalized to 1. </param>
                /// <param name="recombine"></param>
                public static void Twist(Tuple<int, int>[] dimTags, double x, double y, double z, double dx, double dy, double dz, double ax, double ay, double az, double angle, out Tuple<int, int>[] outDimTags, int[] numElements = default, double[] heights = default, bool recombine = false)
                {
                    IntPtr out_DimTags;
                    long outDimTags_n;
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    if (numElements == default) numElements = new int[0];
                    if (heights == default) heights = new double[0];

                    IWrappers.GmshModelGeoTwist(arr, arr.LongLength, x, y, z, dx, dy, dz, ax, ay, az, angle, out out_DimTags, out outDimTags_n, numElements, numElements.LongLength, heights, heights.LongLength, Convert.ToInt32(recombine), ref _ierr);

                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(out_DimTags, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrappers.GmshFree(out_DimTags);
                }

                /// <summary>
                /// Set a transfinite meshing constraint on the curve `tag', with `numNodes' nodes distributed according to `meshType' and `coef'. 
                /// Currently supported types are "Progression" (geometrical progression with power `coef') and "Bump" (refinement toward both extremities of the curve).
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="nPoints"></param>
                /// <param name="meshType"></param>
                /// <param name="coef"></param>
                public static void SetTransfiniteCurve(int tag, int nPoints, string meshType, double coef)
                {
                    IWrappers.GmshModelGeoMeshSetTransfiniteCurve(tag, nPoints, meshType, coef, ref _ierr);
                }

                /// <summary>
                /// Set a transfinite meshing constraint on the surface `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="arrangement"> `arrangement' describes the arrangement of the triangles when the surface is not flagged as recombined: 
                /// currently supported values are "Left", "Right", "AlternateLeft" and "AlternateRight" </param>
                /// <param name="cornerTags"> `cornerTags' can be used to specify the(3 or 4) corners of the transfinite interpolation explicitly; 
                /// specifying the corners explicitly is mandatory if the surface has more that 3 or 4 points on its boundary.; </param>
                public static void SetTransfiniteSurface(int tag, string arrangement, int[] cornerTags)
                {
                    IWrappers.GmshModelGeoMeshSetTransfiniteSurface(tag, arrangement, cornerTags, cornerTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set a transfinite meshing constraint on the surface `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="cornerTags"> `cornerTags' can be used to specify the(6 or 8) corners of the transfinite interpolation explicitly.  </param>
                public static void SetTransfiniteVolume(int tag, int[] cornerTags, ref int ierr)
                {
                    IWrappers.GmshModelGeoMeshSetTransfiniteVolume(tag, cornerTags, cornerTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set a recombination meshing constraint on the model entity of dimension `dim' and tag `tag'. 
                /// Currently only entities of dimension 2 (to recombine triangles into quadrangles) are supported.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="angle"></param>
                public static void SetRecombine(int dim, int tag, double angle)
                {
                    IWrappers.GmshModelGeoMeshSetRecombine(dim, tag, angle, ref _ierr);
                }

                /// <summary>
                /// Set a smoothing meshing constraint on the model entity of dimension `dim' and tag `tag'.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"> `val' iterations of a Laplace smoother are applied. </param>
                public static void SetSmoothing(int dim, int tag, int val)
                {
                    IWrappers.GmshModelGeoMeshSetSmoothing(dim, tag, val, ref _ierr);
                }

                /// <summary>
                /// Set a reverse meshing constraint on the model entity of dimension `dim' and tag `tag'.  
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"> If `val' is true, the mesh orientation will be reversed with respect to the natural mesh orientation(i.e.the orientation consistent with the orientation of the geometry).
                /// If `val' is false, the mesh is left as-is. </param>
                public static void SetReverse(int dim, int tag, bool val)
                {
                    IWrappers.GmshModelGeoMeshSetReverse(dim, tag, Convert.ToInt32(val), ref _ierr);
                }

                /// <summary>
                /// Set the meshing algorithm on the model entity of dimension `dim' and tag `tag'. 
                /// Currently only supported for `dim' == 2.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"></param>
                public static void SetAlgorithm(int dim, int tag, int val)
                {
                    IWrappers.GmshModelGeoMeshSetAlgorithm(dim, tag, val, ref _ierr);
                }

                /// <summary>
                /// Force the mesh size to be extended from the boundary, or not, for the model entity of dimension `dim' and tag `tag'. 
                /// Currently only supported for `dim' == 2. 
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"></param>
                public static void SetSizeFromBoundary(int dim, int tag, int val)
                {
                    IWrappers.GmshModelGeoMeshSetSizeFromBoundary(dim, tag, val, ref _ierr);
                }

                /// <summary>
                /// Synchronize the built-in CAD representation with the current IguanaGmsh model.
                /// This can be called at any time, but since it involves a non trivial amount of processing, the number of synchronization points should normally be minimized.
                /// </summary>
                public static void Synchronize()
                {
                    IWrappers.GmshModelGeoSynchronize(ref _ierr);
                }

                /// <summary>
                /// Translate the model entities `dimTags' along (`dx', `dy', `dz').
                /// </summary>
                public static void Translate(Tuple<int, int>[] dimTags, double dx, double dy, double dz)
                {
                    var tagsArr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrappers.GmshModelGeoTranslate(tagsArr, tagsArr.LongLength, dx, dy, dz, ref _ierr);
                }

                /// <summary>
                /// Rotate the model entities `dimTags' of `angle' radians around the axis of
                /// revolution defined by the point(`x', `y', `z') and the direction (`ax',`ay', `az').
                /// </summary>
                public static void Rotate(Tuple<int, int>[] dimTags, double x, double y, double z, double ax, double ay, double az, double angle)
                {
                    var tagsArr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrappers.GmshModelGeoRotate(tagsArr, tagsArr.LongLength, x, y, z, ax, ay, az, angle, ref _ierr);
                }

                /// <summary>
                /// Copy the entities `dimTags'; the new entities are returned in `outDimTags'. 
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="outDimTags"></param>
                public static void Copy(Tuple<int, int>[] dimTags, out Tuple<int, int>[] outDimTags)
                {
                    IntPtr dimTags_parse;
                    long outDimTags_n;
                    var tagsArr = IHelpers.FlattenIntTupleArray(dimTags);

                    IWrappers.GmshModelGeoCopy(tagsArr, tagsArr.LongLength, out dimTags_parse, out outDimTags_n, ref _ierr);

                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(dimTags_parse, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IguanaGmsh.Free(dimTags_parse);
                }


                /// <summary>
                /// Scale the model entities `dimTag' by factors `a', `b' and `c' along the
                /// three coordinate axes; use(`x', `y', `z') as the center of the homothetic transformation.
                /// </summary>
                public static void Dilate(Tuple<int, int>[] dimTags, double x, double y, double z, double a, double b, double c)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrappers.GmshModelGeoDilate(arr, arr.LongLength, x, y, z, a, b, c, ref _ierr);
                }


                /// <summary>
                /// Mirror the model entities `dimTag', with respect to the plane of equation
                /// `a' * x + `b' * y + `c' * z + `d' = 0. 
                /// </summary>
                public static void Mirror(Tuple<int, int>[] dimTags, double a, double b, double c, double d)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrappers.GmshModelGeoMirror(arr, arr.LongLength, a, b, c, d, ref _ierr);
                }

                /// <summary>
                /// Mirror the model entities `dimTag', with respect to the plane of equation
                /// `a' * x + `b' * y + `c' * z + `d' = 0. (This is a synonym for `mirror',
                /// which will be deprecated in a future release.)
                /// </summary>
                public static void Symmetrize(Tuple<int, int>[] dimTags, double a, double b, double c, double d)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrappers.GmshModelGeoSymmetrize(arr, arr.LongLength, a, b, c, d, ref _ierr);
                }

                /// <summary>
                /// Remove the entities `dimTags'. If `recursive' is true, remove all the entities on their boundaries, down to dimension 0.
                /// </summary>
                public static void Remove(Tuple<int, int>[] dimTags, bool recursive=false) {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrappers.GmshModelGeoRemove(arr, arr.LongLength, Convert.ToInt32(recursive), ref _ierr);
                }

                /// <summary>
                /// Remove all duplicate entities (different entities at the same geometrical location).
                /// </summary>
                public static void RemoveAllDuplicates()
                {
                    IWrappers.GmshModelGeoRemoveAllDuplicates(ref _ierr);
                }

                /// <summary>
                /// Split the model curve of tag `tag' on the control points `pointTags'.
                /// Return the tags `curveTags' of the newly created curves.
                /// </summary>
                public static void SplitCurve(int tag, int[] pointTags, out int[] curveTags) {
                    IntPtr cP;
                    long curveTags_n;

                    IWrappers.GmshModelGeoSplitCurve(tag, pointTags, pointTags.LongLength, out cP, out curveTags_n, ref _ierr);

                    curveTags = new int[curveTags_n];
                    Marshal.Copy(cP, curveTags, 0, (int) curveTags_n);

                    IguanaGmsh.Free(cP);
                }

                /// <summary>
                /// Set a mesh size constraint on the model entities `dimTags'. Currently only
                /// entities of dimension 0 (points) are handled.
                /// </summary>
                public static void SetSize(Tuple<int,int>[] dimTags, double size) {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrappers.GmshModelGeoMeshSetSize(arr, arr.LongLength, size, ref _ierr);
                }
            }
        }
    }
}
