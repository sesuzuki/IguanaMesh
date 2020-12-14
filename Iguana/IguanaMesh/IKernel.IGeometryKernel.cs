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

using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Iguana.IguanaMesh
{
    public static partial class IKernel
    {
        public static partial class IGeometryKernel
        {

            #region Iguana Methods

            public static void EmbedConstraints(List<IConstraint> constraints, int dim = 2, int tag = -1)
            {
                if (constraints.Count == 0) return;
                if (constraints == default) return;

                Tuple<int, int>[] dimTags;
                IModel.GetEntities(out dimTags, dim);

                if (tag == -1)
                {
                    var mainTags = dimTags.Select(keyPair => keyPair.Item2).ToArray();

                    if (mainTags.Length == 1) tag = mainTags[0];
                    else if (mainTags.Length > 1 && dim == 2)
                    {
                        tag = IBuilder.AddSurfaceLoop(mainTags);
                    }
                }

                int count = constraints.Count;

                if (count > 0)
                {
                    List<int> ptsTags = new List<int>();
                    List<int> crvTags = new List<int>();
                    List<int> srfTags = new List<int>();
                    PointCloud ptsCloud = new PointCloud();

                    IConstraint data;
                    double t = 0.0001;

                    for (int i = 0; i < count; i++)
                    {
                        data = constraints[i];

                        switch (data.Dim)
                        {
                            case 0:
                                Point3d p = (Point3d)data.RhinoGeometry;
                                int idx = EvaluatePoint(ptsCloud, p, t);

                                if (idx == -1)
                                {
                                    ptsTags.Add(IBuilder.AddPoint(p.X, p.Y, p.Z, data.Size));
                                    ptsCloud.Add(p);
                                }

                                break;

                            case 1:
                                Polyline poly = (Polyline)data.RhinoGeometry;
                                crvTags.AddRange(CreateUnderlyingLinesFromPolyline(poly, data.Size, ref ptsTags, ref ptsCloud, t));
                                break;

                            case 2:
                                Curve crv = (Curve)data.RhinoGeometry;
                                crvTags.AddRange(CreateUnderlyingSplinesFromCurve(crv, data.Size));
                                break;
                            case 3:
                                Brep b = (Brep)data.RhinoGeometry;
                                List<Point3d> patch;
                                Curve[] crvArr;
                                IRhinoGeometry.GetBrepFaceMeshingData(b, 0, 20, out crvArr, out patch);
                                srfTags.Add(CreateUnderlyingSurfaceFromCurve(crvArr[0], data.Size, patch, false));
                                break;
                        }
                    }

                    IBuilder.Synchronize();

                    if (tag != -1)
                    {
                        if (ptsTags.Count > 0) IMeshingKernel.IBuilder.Embed(0, ptsTags.ToArray(), dim, tag);
                        if (crvTags.Count > 0) IMeshingKernel.IBuilder.Embed(1, crvTags.ToArray(), dim, tag);
                        if (dim == 3)
                        {
                            if (srfTags.Count > 0) IMeshingKernel.IBuilder.Embed(2, srfTags.ToArray(), dim, tag);
                        }
                    }
                }
            }

            public static int CreateUnderlyingSurfaceFromCurve(Curve crv, double size, List<Point3d> patchs = default, bool synchronize = false)
            {
                int wireTag = CreateUnderlyingCurveLoopFromCurve(crv, size);
                return CreateUnderlyingSurface(wireTag, patchs, synchronize);
            }

            public static int CreateUnderlyingSurface(int wireTag, List<Point3d> patchs = default, bool synchronize = false)
            {
                // 1._ Check points to patch
                int tag;
                if (patchs == default) tag = -1;
                else
                {
                    double[] centroid = new double[3];
                    int count = patchs.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Point3d p = patchs[i];
                        centroid[0] += p.X;
                        centroid[1] += p.Y;
                        centroid[2] += p.Z;
                    }
                    tag = IBuilder.AddPoint(centroid[0] / count, centroid[1] / count, centroid[2] / count);
                }

                // 3._ Build OCC Geometry
                int surfaceTag = IBuilder.AddSurfaceFilling(wireTag, tag);

                // 5._ Synchronize model
                if (synchronize) IBuilder.Synchronize();

                return surfaceTag;
            }

            public static int CreateUnderlyingCurveLoopFromCurve(Curve crv, double size, bool synchronize = false)
            {
                int[] crvTags = CreateUnderlyingSplinesFromCurve(crv, size);
                int wireTag = IBuilder.AddCurveLoop(crvTags);
                if (synchronize) IBuilder.Synchronize();
                return wireTag;
            }

            public static int CreateUnderlyingPolylineFromPolyline(Polyline poly, double size)
            {
                List<int> ptsTag = new List<int>();
                PointCloud ptsCloud = new PointCloud();
                int[] crvTags = CreateUnderlyingLinesFromPolyline(poly, size, ref ptsTag, ref ptsCloud);
                int wireTag = IBuilder.AddCurveLoop(crvTags);
                return wireTag;
            }

            public static int CreateUnderlyingPolylineFromCurve(Curve curve, double size)
            {
                List<int> ptsTag = new List<int>();
                PointCloud ptsCloud = new PointCloud();
                int[] crvTags = CreateUnderlyingLinesFromCurve(curve, size);
                int wireTag = IBuilder.AddCurveLoop(crvTags);
                return wireTag;
            }

            public static int CreateUnderlyingPlaneSurface(Curve boundary, List<Curve> holes, double size)
            {
                int[] crv_tags = new int[holes.Count + 1];
                crv_tags[0] = CreateUnderlyingPolylineFromCurve(boundary, size);

                for (int i = 0; i < holes.Count; i++) crv_tags[i + 1] = CreateUnderlyingPolylineFromCurve(holes[i], size);

                int surfaceTag = IBuilder.AddPlaneSurface(crv_tags);

                return surfaceTag;
            }

            public static int[] CreateUnderlyingLinesFromPolyline(Polyline poly, double size, ref List<int> ptsTags, ref PointCloud ptsCloud, double t = 0.001, bool synchronize = false)
            {
                Point3d p;
                int tag;
                int[] crvTags = new int[poly.SegmentCount];

                int[] tempTags = new int[poly.Count];
                int idx;
                for (int j = 0; j < poly.Count; j++)
                {
                    p = poly[j];
                    idx = EvaluatePoint(ptsCloud, p, t);

                    if (idx == -1)
                    {
                        tag = IBuilder.AddPoint(p.X, p.Y, p.Z, size);
                        ptsTags.Add(tag);
                        ptsCloud.Add(p);
                        idx = ptsCloud.Count - 1;
                    }

                    tempTags[j] = idx;
                }

                for (int j = 0; j < poly.SegmentCount; j++)
                {
                    crvTags[j] = IBuilder.AddLine(ptsTags[tempTags[j]], ptsTags[tempTags[j + 1]]);
                }

                if (synchronize) IBuilder.Synchronize();

                return crvTags;
            }

            public static int[] CreateUnderlyingLinesFromCurve(Curve crv, double size, bool synchronize = false)
            {
                Polyline poly;
                crv.TryGetPolyline(out poly);

                List<int> ptsTags = new List<int>();
                PointCloud ptsCloud = new PointCloud();
                int tag;
                Point3d p;

                int[] tempTags = new int[poly.Count];
                int idx;
                for (int i = 0; i < poly.Count; i++)
                {
                    p = poly[i];
                    idx = EvaluatePoint(ptsCloud, p, 0.001);

                    if (idx == -1)
                    {
                        tag = IBuilder.AddPoint(p.X, p.Y, p.Z, size);
                        ptsTags.Add(tag);
                        ptsCloud.Add(p);
                        idx = ptsCloud.Count - 1;
                    }

                    tempTags[i] = idx;
                }

                int[] dimTags = new int[poly.SegmentCount];
                for (int i = 0; i < poly.SegmentCount; i++)
                {
                    dimTags[i] = IBuilder.AddLine(ptsTags[tempTags[i]], ptsTags[tempTags[i + 1]]);
                }

                if (synchronize) IBuilder.Synchronize();
                return dimTags;
            }

            public static int[] CreateUnderlyingSplinesFromCurve(Curve crv, double size, bool synchronize = false)
            {
                // Covert curve into polycurve
                PolyCurve pc = crv.ToArcsAndLines(Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, Rhino.RhinoDoc.ActiveDoc.ModelAngleToleranceRadians, 0, 1);

                // Divide points into 4 groups and create splines
                int remain = pc.SegmentCount % 4;
                int count = (pc.SegmentCount - remain) / 4;
                int[] dimTags = new int[4];

                int sIdx = 0, pIdx, tag;
                PointCloud ptsCloud = new PointCloud();
                List<int> allTags = new List<int>();
                Point3d p;

                for (int i = 0; i < 4; i++)
                {

                    if (i == 3) count += remain;
                    int[] ptTags = new int[count + 1];

                    for (int j = 0; j < count; j++)
                    {
                        p = pc.SegmentCurve(sIdx).PointAtStart;
                        pIdx = EvaluatePoint(ptsCloud, p, 0.0001);

                        if (pIdx == -1)
                        {
                            tag = IBuilder.AddPoint(p.X, p.Y, p.Z, size);
                            ptsCloud.Add(p);
                            allTags.Add(tag);
                        }
                        else tag = allTags[pIdx];

                        ptTags[j] = tag;

                        if (j == count - 1)
                        {
                            p = pc.SegmentCurve(sIdx).PointAtEnd;
                            pIdx = EvaluatePoint(ptsCloud, p, 0.001);

                            if (pIdx == -1)
                            {
                                tag = IBuilder.AddPoint(p.X, p.Y, p.Z, size);
                                ptsCloud.Add(p);
                                allTags.Add(tag);
                            }
                            else tag = allTags[pIdx];

                            ptTags[j + 1] = tag;
                        }
                        sIdx++;
                    }

                    dimTags[i] = IBuilder.AddSpline(ptTags);
                }

                if (synchronize) IBuilder.Synchronize();

                return dimTags;
            }


            #endregion

            #region Gmsh Methods
            internal static class IBuilder
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
                    return IWrap.GmshModelGeoAddPoint(x, y, z, meshSize, tag, ref _ierr);
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
                    return IWrap.GmshModelGeoAddLine(startTag, endTag, tag, ref _ierr);
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
                    return IWrap.GmshModelGeoAddCircleArc(startTag, centerTag, endTag, tag, nx, ny, nz, ref _ierr);
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
                    return IWrap.GmshModelGeoAddEllipseArc(startTag, centerTag, majorTag, endTag, tag, nx, ny, nz, ref _ierr);
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
                    int tag_crv = IWrap.GmshModelGeoAddCurveLoop(curveTags, curveTags.Length, tag, ref _ierr);
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
                    return IWrap.GmshModelGeoAddSpline(pointTags, pointTags.LongLength, tag, ref _ierr);
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
                    return IWrap.GmshModelGeoAddBSpline(pointTags, pointTags.LongLength, tag, ref _ierr);
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
                    return IWrap.GmshModelGeoAddBezier(pointTags, pointTags.LongLength, tag, ref _ierr);
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
                    return IWrap.GmshModelGeoAddCompoundSpline(curveTags, curveTags.LongLength, numIntervals, tag, ref _ierr);
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
                    return IWrap.GmshModelGeoAddCompoundBSpline(curveTags, curveTags.LongLength, numIntervals, tag, ref _ierr);
                }

                /// <summary>
                /// Add a plane surface defined by one or more closed polylines. 
                /// </summary>
                /// <param name="wireTags"> The first closed polyline in `wireTags' defines the exterior contour; additional curve loop define holes. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.Return the tag of the surface. </param>
                /// <returns></returns>
                public static int AddPlaneSurface(int[] wireTags, int tag = -1)
                {
                    return IWrap.GmshModelGeoAddPlaneSurface(wireTags, wireTags.LongLength, tag, ref _ierr);
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
                    return IWrap.GmshModelGeoAddSurfaceFilling(new int[] { wireTag }, 1, tag, sphereCenterTag, ref _ierr);
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
                    return IWrap.GmshModelGeoAddSurfaceLoop(surfaceTags, surfaceTags.LongLength, tag, ref _ierr);
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
                    return IWrap.GmshModelGeoAddVolume(shellTags, shellTags.LongLength, tag, ref _ierr);
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

                    IWrap.GmshModelGeoExtrude(arr, arr.LongLength, dx, dy, dz, out out_DimTags, out outDimTags_n, numElements, numElements.LongLength, heights, heights.LongLength, Convert.ToInt32(recombine), ref _ierr);

                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(out_DimTags, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrap.GmshFree(out_DimTags);
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

                    IWrap.GmshModelGeoRevolve(arr, arr.LongLength, x, y, z, ax, ay, az, angle, out out_DimTags, out outDimTags_n, numElements, numElements.LongLength, heights, heights.LongLength, Convert.ToInt32(recombine), ref _ierr);

                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(out_DimTags, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrap.GmshFree(out_DimTags);
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

                    IWrap.GmshModelGeoTwist(arr, arr.LongLength, x, y, z, dx, dy, dz, ax, ay, az, angle, out out_DimTags, out outDimTags_n, numElements, numElements.LongLength, heights, heights.LongLength, Convert.ToInt32(recombine), ref _ierr);

                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(out_DimTags, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrap.GmshFree(out_DimTags);
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
                    IWrap.GmshModelGeoMeshSetTransfiniteCurve(tag, nPoints, meshType, coef, ref _ierr);
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
                    IWrap.GmshModelGeoMeshSetTransfiniteSurface(tag, arrangement, cornerTags, cornerTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set a transfinite meshing constraint on the surface `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="cornerTags"> `cornerTags' can be used to specify the(6 or 8) corners of the transfinite interpolation explicitly.  </param>
                public static void SetTransfiniteVolume(int tag, int[] cornerTags, ref int ierr)
                {
                    IWrap.GmshModelGeoMeshSetTransfiniteVolume(tag, cornerTags, cornerTags.LongLength, ref _ierr);
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
                    IWrap.GmshModelGeoMeshSetRecombine(dim, tag, angle, ref _ierr);
                }

                /// <summary>
                /// Set a smoothing meshing constraint on the model entity of dimension `dim' and tag `tag'.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"> `val' iterations of a Laplace smoother are applied. </param>
                public static void SetSmoothing(int dim, int tag, int val)
                {
                    IWrap.GmshModelGeoMeshSetSmoothing(dim, tag, val, ref _ierr);
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
                    IWrap.GmshModelGeoMeshSetReverse(dim, tag, Convert.ToInt32(val), ref _ierr);
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
                    IWrap.GmshModelGeoMeshSetAlgorithm(dim, tag, val, ref _ierr);
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
                    IWrap.GmshModelGeoMeshSetSizeFromBoundary(dim, tag, val, ref _ierr);
                }

                /// <summary>
                /// Synchronize the built-in CAD representation with the current IguanaGmsh model.
                /// This can be called at any time, but since it involves a non trivial amount of processing, the number of synchronization points should normally be minimized.
                /// </summary>
                public static void Synchronize()
                {
                    IWrap.GmshModelGeoSynchronize(ref _ierr);
                }

                /// <summary>
                /// Translate the model entities `dimTags' along (`dx', `dy', `dz').
                /// </summary>
                public static void Translate(Tuple<int, int>[] dimTags, double dx, double dy, double dz)
                {
                    var tagsArr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelGeoTranslate(tagsArr, tagsArr.LongLength, dx, dy, dz, ref _ierr);
                }

                /// <summary>
                /// Rotate the model entities `dimTags' of `angle' radians around the axis of
                /// revolution defined by the point(`x', `y', `z') and the direction (`ax',`ay', `az').
                /// </summary>
                public static void Rotate(Tuple<int, int>[] dimTags, double x, double y, double z, double ax, double ay, double az, double angle)
                {
                    var tagsArr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelGeoRotate(tagsArr, tagsArr.LongLength, x, y, z, ax, ay, az, angle, ref _ierr);
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

                    IWrap.GmshModelGeoCopy(tagsArr, tagsArr.LongLength, out dimTags_parse, out outDimTags_n, ref _ierr);

                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(dimTags_parse, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    Free(dimTags_parse);
                }

                /// <summary>
                /// Scale the model entities `dimTag' by factors `a', `b' and `c' along the
                /// three coordinate axes; use(`x', `y', `z') as the center of the homothetic transformation.
                /// </summary>
                public static void Dilate(Tuple<int, int>[] dimTags, double x, double y, double z, double a, double b, double c)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelGeoDilate(arr, arr.LongLength, x, y, z, a, b, c, ref _ierr);
                }


                /// <summary>
                /// Mirror the model entities `dimTag', with respect to the plane of equation
                /// `a' * x + `b' * y + `c' * z + `d' = 0. 
                /// </summary>
                public static void Mirror(Tuple<int, int>[] dimTags, double a, double b, double c, double d)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelGeoMirror(arr, arr.LongLength, a, b, c, d, ref _ierr);
                }

                /// <summary>
                /// Mirror the model entities `dimTag', with respect to the plane of equation
                /// `a' * x + `b' * y + `c' * z + `d' = 0. (This is a synonym for `mirror',
                /// which will be deprecated in a future release.)
                /// </summary>
                public static void Symmetrize(Tuple<int, int>[] dimTags, double a, double b, double c, double d)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelGeoSymmetrize(arr, arr.LongLength, a, b, c, d, ref _ierr);
                }

                /// <summary>
                /// Remove the entities `dimTags'. If `recursive' is true, remove all the entities on their boundaries, down to dimension 0.
                /// </summary>
                public static void Remove(Tuple<int, int>[] dimTags, bool recursive = false)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelGeoRemove(arr, arr.LongLength, Convert.ToInt32(recursive), ref _ierr);
                }

                /// <summary>
                /// Remove all duplicate entities (different entities at the same geometrical location).
                /// </summary>
                public static void RemoveAllDuplicates()
                {
                    IWrap.GmshModelGeoRemoveAllDuplicates(ref _ierr);
                }

                /// <summary>
                /// Split the model curve of tag `tag' on the control points `pointTags'.
                /// Return the tags `curveTags' of the newly created curves.
                /// </summary>
                public static void SplitCurve(int tag, int[] pointTags, out int[] curveTags)
                {
                    IntPtr cP;
                    long curveTags_n;

                    IWrap.GmshModelGeoSplitCurve(tag, pointTags, pointTags.LongLength, out cP, out curveTags_n, ref _ierr);

                    curveTags = new int[curveTags_n];
                    Marshal.Copy(cP, curveTags, 0, (int)curveTags_n);

                    Free(cP);
                }

                /// <summary>
                /// Set a mesh size constraint on the model entities `dimTags'. Currently only
                /// entities of dimension 0 (points) are handled.
                /// </summary>
                public static void SetSize(Tuple<int, int>[] dimTags, double size)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelGeoMeshSetSize(arr, arr.LongLength, size, ref _ierr);
                }

                #endregion
            }
        }
    }
}
