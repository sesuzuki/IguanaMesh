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
        public static partial class IGeometryOCCKernel
        {

            #region Iguana Methods
            public static void EmbedConstraints(List<IConstraint> constraints, int dim = 2, int tag = -1)
            {
                if (constraints == null || constraints == default || constraints.Count == 0) return;

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
                    Curve crv;

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
                                crv = (Curve)data.RhinoGeometry;
                                crvTags.AddRange(CreateUnderlyingLinesFromCurveDividedByLength(crv, data.Size, data.CurveDivisionLength));
                                break;

                            case 2:
                                crv = (Curve)data.RhinoGeometry;
                                crvTags.AddRange(CreateUnderlyingLinesFromCurveDividedByCount(crv, data.Size,data.NodesCountPerCurve));
                                break;
                            case 3:
                                Brep b = (Brep)data.RhinoGeometry;
                                List<Point3d> patch;
                                Curve[] crvArr;
                                IRhinoGeometry.GetBrepFaceMeshingData(b, 0, 20, out crvArr, out patch);
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
                if (patchs == default) patchs = new List<Point3d>();
                int[] patchPts = new int[patchs.Count];

                Point3d p2;
                for (int i = 0; i < patchs.Count; i++)
                {
                    p2 = patchs[i];
                    patchPts[i] = IBuilder.AddPoint(p2.X, p2.Y, p2.Z);
                }

                // 3._ Build OCC Geometry
                int surfaceTag = IBuilder.AddSurfaceFilling(wireTag, patchPts);

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
                int wireTag = IBuilder.AddWire(crvTags);
                return wireTag;
            }

            public static int CreateUnderlyingPolylineFromCurveDividedByLength(Curve curve, double size, double length)
            {
                List<int> ptsTag = new List<int>();
                PointCloud ptsCloud = new PointCloud();
                int[] crvTags = CreateUnderlyingLinesFromCurveDividedByLength(curve, size, length);
                int wireTag = IBuilder.AddCurveLoop(crvTags);
                return wireTag;
            }

            public static int CreateUnderlyingPolylineFromCurveDividedByCount(Curve curve, double size, int count)
            {
                List<int> ptsTag = new List<int>();
                PointCloud ptsCloud = new PointCloud();
                int[] crvTags = CreateUnderlyingLinesFromCurveDividedByCount(curve, size, count);
                int wireTag = IBuilder.AddCurveLoop(crvTags);
                return wireTag;
            }

            public static int CreateUnderlyingPlaneSurfaceDividedByLength(Curve boundary, List<Curve> holes, double size, double length)
            {
                int[] crv_tags = new int[holes.Count + 1];
                crv_tags[0] = CreateUnderlyingPolylineFromCurveDividedByLength(boundary, size, length);

                for (int i = 0; i < holes.Count; i++) crv_tags[i + 1] = CreateUnderlyingPolylineFromCurveDividedByLength(holes[i], size, length);

                int surfaceTag = IBuilder.AddPlaneSurface(crv_tags);

                return surfaceTag;
            }

            public static int CreateUnderlyingPlaneSurfaceDividedByCount(Curve boundary, List<Curve> holes, double size, int count)
            {
                int[] crv_tags = new int[holes.Count + 1];
                crv_tags[0] = CreateUnderlyingPolylineFromCurveDividedByCount(boundary, size, count);

                for (int i = 0; i < holes.Count; i++) crv_tags[i + 1] = CreateUnderlyingPolylineFromCurveDividedByCount(holes[i], size, count);

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

            public static int[] CreateUnderlyingLinesFromCurveDividedByCount(Curve crv, double size, int nodeNumber, bool synchronize = false)
            {
                if (!crv.IsLinear())
                {
                    Curve[] seg = crv.DuplicateSegments();

                    List<int> ptTags = new List<int>();
                    bool isClosed = crv.IsClosed;
                    Point3d p;

                    for (int i = 0; i < seg.Length; i++)
                    {
                        Curve c = seg[i];
                        double[] t = c.DivideByCount(nodeNumber, true);

                        int count = t.Length - 1;
                        if (i == seg.Length - 1 && !isClosed) count = t.Length;

                        for (int j = 0; j < count; j++)
                        {
                            p = c.PointAt(t[j]);
                            ptTags.Add(IBuilder.AddPoint(p.X, p.Y, p.Z, size));
                        }
                    }

                    List<int> lnTags = new List<int>();
                    for (int i = 0; i < ptTags.Count - 1; i++)
                    {
                        lnTags.Add(IBuilder.AddLine(ptTags[i], ptTags[i + 1]));
                    }

                    if (isClosed)
                    {
                        lnTags.Add(IBuilder.AddLine(ptTags[ptTags.Count - 1], ptTags[0]));
                    }

                    if (synchronize) IBuilder.Synchronize();
                    return lnTags.ToArray();
                }
                else
                {
                    List<int> ptTags = new List<int>();
                    Point3d p;
                    double[] t = crv.DivideByCount(nodeNumber, true);

                    for (int i = 0; i < t.Length; i++)
                    {
                        p = crv.PointAt(t[i]);
                        ptTags.Add(IBuilder.AddPoint(p.X, p.Y, p.Z, size));
                    }

                    List<int> lnTags = new List<int>();
                    for (int i = 0; i < ptTags.Count - 1; i++)
                    {
                        lnTags.Add(IBuilder.AddLine(ptTags[i], ptTags[i + 1]));
                    }

                    if (synchronize) IBuilder.Synchronize();
                    return lnTags.ToArray();
                }
            }

            public static int[] CreateUnderlyingLinesFromCurveDividedByLength(Curve crv, double size, double length, bool synchronize = false)
            {
                if (!crv.IsLinear())
                {
                    Curve[] seg = crv.DuplicateSegments();

                    List<int> ptTags = new List<int>();
                    bool isClosed = crv.IsClosed;
                    Point3d p;

                    for (int i = 0; i < seg.Length; i++)
                    {
                        Curve c = seg[i];
                        double[] t = c.DivideByLength(length, true);

                        for (int j = 0; j < t.Length; j++)
                        {
                            p = c.PointAt(t[j]);
                            ptTags.Add(IBuilder.AddPoint(p.X, p.Y, p.Z, size));
                        }

                        if (i == seg.Length - 1 && !isClosed)
                        {
                            p = c.PointAtEnd;
                            ptTags.Add(IBuilder.AddPoint(p.X, p.Y, p.Z, size));
                        }
                    }

                    List<int> lnTags = new List<int>();
                    for (int i = 0; i < ptTags.Count - 1; i++)
                    {
                        lnTags.Add(IBuilder.AddLine(ptTags[i], ptTags[i + 1]));
                    }

                    if (isClosed)
                    {
                        lnTags.Add(IBuilder.AddLine(ptTags[ptTags.Count - 1], ptTags[0]));
                    }

                    if (synchronize) IBuilder.Synchronize();
                    return lnTags.ToArray();
                }
                else
                {
                    List<int> ptTags = new List<int>();
                    Point3d p;
                    double[] t = crv.DivideByLength(length, true);

                    for (int i = 0; i < t.Length; i++)
                    {
                        p = crv.PointAt(t[i]);
                        ptTags.Add(IBuilder.AddPoint(p.X, p.Y, p.Z, size));
                    }
                    p = crv.PointAtEnd;
                    ptTags.Add(IBuilder.AddPoint(p.X, p.Y, p.Z, size));

                    List<int> lnTags = new List<int>();
                    for (int i = 0; i < ptTags.Count - 1; i++)
                    {
                        lnTags.Add(IBuilder.AddLine(ptTags[i], ptTags[i + 1]));
                    }

                    if (synchronize) IBuilder.Synchronize();
                    return lnTags.ToArray();
                }
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
            public static class IBuilder
            {
                /// <summary>
                /// Add a geometrical point in the OpenCASCADE CAD representation, at coordinates(`x', `y', `z').
                /// Return the tag of the point. 
                /// (Note that the point will be added in the current model only after `synchronize' is called. This behavior holds for all the entities added in the occ module.)
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="meshSize"> If `meshSize' is > 0, add a meshing constraint at that point. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddPoint(double x, double y, double z, double meshSize = -1, int tag = -1)
                {
                    return IWrap.GmshModelOccAddPoint(x, y, z, meshSize, tag, ref _ierr);
                }

                /// <summary>
                /// Add a straight line segment between the two points with tags `startTag' and `endTag'.
                /// </summary>
                /// <param name="startTag"> Start point tag </param>
                /// <param name="endTag"> End point tag </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddLine(int startTag, int endTag, int tag = -1)
                {
                    return IWrap.GmshModelOccAddLine(startTag, endTag, tag, ref _ierr);

                }

                /// <summary>
                /// Add a circle arc between the two points with tags `startTag' and `endTag', with center `centerTag'.
                /// Return the tag of the circle arc.
                /// </summary>
                /// <param name="startTag"> Start point tag </param>
                /// <param name="centerTag"> Center point tag </param>
                /// <param name="endTag"> End point tag </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddCircleArc(int startTag, int centerTag, int endTag, int tag = -1)
                {
                    return IWrap.GmshModelOccAddCircleArc(startTag, centerTag, endTag, tag, ref _ierr);
                }

                /// <summary>
                /// Add a circle of center (`x', `y', `z') and radius `r'. 
                /// If `angle1' and `angle2' are specified, create a circle arc between the two angles. 
                /// Return the tag of the circle.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="r"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="angle1"></param>
                /// <param name="angle2"></param>
                /// <returns></returns>
                public static int AddCircle(double x, double y, double z, double r, int tag = -1, double angle1 = 0, double angle2 = 2 * Math.PI)
                {
                    return IWrap.gmshModelOccAddCircle(x, y, z, r, tag, angle1, angle2, ref _ierr);
                }

                /// <summary>
                /// Add an ellipse arc between the two points `startTag' and `endTag', with center `centerTag' and major axis point `majorTag'. 
                /// Return the tag of the ellipse arc. 
                /// (Note that OpenCASCADE does not allow creating ellipse arcs with the major radius smaller than the minor radius)
                /// </summary>
                /// <param name="startTag"></param>
                /// <param name="centerTag"></param>
                /// <param name="majorTag"></param>
                /// <param name="endTag"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                /// 
                public static int AddEllipseArc(int startTag, int centerTag, int majorTag, int endTag, int tag = -1)
                {
                    return IWrap.GmshModelOccAddEllipseArc(startTag, centerTag, majorTag, endTag, tag, ref _ierr);
                }

                /// <summary>
                /// Add an ellipse of center (`x', `y', `z') and radii `r1' and `r2' along the x- and y-axes respectively.
                /// If `angle1' and `angle2' are specified, create an ellipse arc between the two angles.
                /// Return the tag of the ellipse.
                /// (Note that OpenCASCADE does not allow creating ellipses with the major radius (along the x-axis) smaller than or equal to the minor radius (along the y-axis): rotate the shape or use `addCircle' in such cases)
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="r1"></param>
                /// <param name="r2"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <param name="angle1"></param>
                /// <param name="angle2"></param>
                /// 
                /// <returns></returns>
                public static int AddEllipse(double x, double y, double z, double r1, double r2, int tag = -1, double angle1 = 0, double angle2 = 2 * Math.PI)
                {
                    return IWrap.GmshModelOccAddEllipse(x, y, z, r1, r2, tag, angle1, angle2, ref _ierr);
                }

                /// <summary>
                /// Add a spline (C2 b-spline) curve going through the points `pointTags'.
                /// Create a periodic curve if the first and last points are the same.
                /// Return the tag of the spline curve.
                /// </summary>
                /// <param name="pointTags"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>

                public static int AddSpline(int[] pointTags, int tag = -1)
                {
                    return IWrap.gmshModelOccAddSpline(pointTags, pointTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a b-spline curve of degree `degree' with `pointTags' control points.
                /// If `weights', `knots' or `multiplicities' are not provided, default parameters are computed automatically.
                /// Create a periodic curve if the first and last points are the same. 
                /// Return the tag of the b-spline
                /// </summary>
                /// <param name="pointTags"></param>
                /// <param name="degree"></param>
                /// <param name="weights"></param>
                /// <param name="knots"></param>
                /// <param name="multiplicities"> </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>

                public static int AddBSpline(int[] pointTags, int degree, double[] weights = default, double[] knots = default, int[] multiplicities = default, int tag = -1)
                {
                    if (weights == default) weights = new double[0];
                    if (knots == default) knots = new double[0];
                    if (multiplicities == default) multiplicities = new int[0];
                    return IWrap.GmshModelOccAddBSpline(pointTags, pointTags.LongLength, tag, degree, weights, weights.LongLength, knots, knots.LongLength, multiplicities, multiplicities.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add a Bezier curve with `pointTags' control points.
                /// Return the tag of the Bezier curve.
                /// </summary>
                /// <param name="pointTags"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddBezier(int[] pointTags, int tag = -1)
                {
                    return IWrap.GmshModelOccAddBezier(pointTags, pointTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a wire (open or closed) formed by the curves `curveTags'.
                /// Return the tag of the wire.
                /// Note that an OpenCASCADE wire can be made of curves that share geometrically identical(but topologically different) points.
                /// </summary>
                /// <param name="curveTags"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="checkClosed"></param>
                /// <returns></returns>
                public static int AddWire(int[] curveTags, bool checkClosed = false, int tag = -1)
                {
                    return IWrap.GmshModelOccAddWire(curveTags, curveTags.LongLength, tag, Convert.ToInt32(checkClosed), ref _ierr);
                }

                /// <summary>
                /// Add a curve loop (a closed wire) formed by the curves `curveTags'.
                /// Note that an OpenCASCADE curve loop can be made of curves that share geometrically identical(but topologically different) points.
                /// Return the tag of the curve loop.
                /// </summary>
                /// <param name="curveTags"> `curveTags' should contain tags of curves forming a closed loop. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <returns></returns>
                public static int AddCurveLoop(int[] curveTags, int tag = -1)
                {
                    return IWrap.GmshModelOccAddCurveLoop(curveTags, curveTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a rectangle with lower left corner at (`x', `y', `z') and upper right corner at (`x' + `dx', `y' + `dy', `z'). 
                /// Return the tag of the rectangle.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="tag"></param>
                /// <param name="roundedRadius"> Round the corners if `roundedRadius' is nonzero .</param>
                /// <returns></returns>
                public static int AddRectangle(double x, double y, double z, double dx, double dy, int tag = -1, double roundedRadius = 0)
                {
                    return IWrap.GmshModelOccAddRectangle(x, y, z, dx, dy, tag, roundedRadius, ref _ierr);
                }

                /// <summary>
                /// Add a disk with center (`xc', `yc', `zc') and radius `rx' along the x-axis and `ry' along the y-axis. 
                /// Return the tag of the disk.
                /// </summary>
                /// <param name="xc"></param>
                /// <param name="yc"></param>
                /// <param name="zc"></param>
                /// <param name="rx"></param>
                /// <param name="ry"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <returns></returns>
                public static int AddDisk(double xc, double yc, double zc, double rx, double ry, int tag = -1)
                {
                    return IWrap.GmshModelOccAddDisk(xc, yc, zc, rx, ry, tag, ref _ierr);
                }


                /// <summary>
                /// Add a plane surface defined by one or more curve loops (or closed wires) `wireTags'.  
                /// Return the tag of the surface.
                /// </summary>
                /// <param name="wireTags"> The first curve loop defines the exterior contour; additional curve loop define holes. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <returns></returns>
                public static int AddPlaneSurface(int[] wireTags, int tag = -1)
                {
                    return IWrap.GmshModelOccAddPlaneSurface(wireTags, wireTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a surface filling the curve loop `wireTag'. 
                /// Return the tag of the surface.
                /// </summary>
                /// <param name="wireTag"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <param name="pointTags"> If `pointTags' are provided, force the surface to pass through the given points. </param>
                /// <returns></returns>
                public static int AddSurfaceFilling(int wireTag, int[] pointTags = default, int tag = -1)
                {
                    if (pointTags == default) pointTags = new int[0];
                    return IWrap.GmshModelOccAddSurfaceFilling(wireTag, tag, pointTags, pointTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add a BSpline surface filling the curve loop `wireTag'. 
                /// The curve loop should be made of 2, 3 or 4 BSpline curves. 
                /// Return the tag of the surface.
                /// </summary>
                /// <param name="wireTag"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="type"> The optional `type' argument specifies the type of filling: "Stretch" creates the flattest patch, "Curved" (the default) creates the most rounded patch, and "Coons" creates a rounded patch with less depth than "Curved".  </param>
                /// <returns></returns>
                public static int AddBSplineFilling(int wireTag, string type = "Curved", int tag = -1)
                {
                    return IWrap.GmshModelOccAddBSplineFilling(wireTag, tag, type, ref _ierr);
                }

                /// <summary>
                /// Add a Bezier surface filling the curve loop `wireTag'. 
                /// The curve loop should be made of 2, 3 or 4 BSpline curves. 
                /// Return the tag of the surface.
                /// </summary>
                /// <param name="wireTag"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="type"> The optional `type' argument specifies the type of filling: "Stretch" creates the flattest patch, "Curved" (the default) creates the most rounded patch, and "Coons" creates a rounded patch with less depth than "Curved".  </param>
                /// <returns></returns>
                public static int AddBezierFilling(int wireTag, string type = "Curved", int tag = -1)
                {
                    return IWrap.GmshModelOccAddBezierFilling(wireTag, tag, type, ref _ierr);
                }

                /// <summary>
                /// Add a b-spline surface of degree `degreeU' x `degreeV'
                /// If `weights', `knotsU', `knotsV', `multiplicitiesU' or `multiplicitiesV' are not provided, default parameters are computed automatically. 
                /// Return the tag of the b-spline surface.
                /// </summary>
                /// <param name="pointTags"> `pointTags' are control points given as a single vector [Pu1v1, ... Pu`numPointsU'v1, Pu1v2, ...]. </param>
                /// <param name="numPointsU"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <param name="degreeU"></param>
                /// <param name="degreeV"></param>
                /// <param name="weights"></param>
                /// <param name="knotsU"></param>
                /// <param name="knotsV"></param>
                /// <param name="multiplicitiesU"></param>
                /// <param name="multiplicitiesV"></param>
                /// <returns></returns>
                public static int AddBSplineSurface(int[] pointTags, int numPointsU, int degreeU, int degreeV, double[] weights = default, double[] knotsU = default, double[] knotsV = default, int[] multiplicitiesU = default, int[] multiplicitiesV = default, int tag = -1)
                {
                    if (weights == default) weights = new double[0];
                    if (knotsU == default) knotsU = new double[0];
                    if (knotsV == default) knotsV = new double[0];
                    if (multiplicitiesU == default) multiplicitiesU = new int[0];
                    if (multiplicitiesV == default) multiplicitiesV = new int[0];
                    return IWrap.GmshModelOccAddBSplineSurface(pointTags, pointTags.LongLength, numPointsU, tag, degreeU, degreeV, weights, weights.LongLength, knotsU, knotsU.LongLength, knotsV, knotsV.LongLength, multiplicitiesU, multiplicitiesU.LongLength, multiplicitiesV, multiplicitiesV.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add a Bezier surface.
                /// Return the tag of the b-spline surface.
                /// </summary>
                /// <param name="pointTags"> `pointTags' control points given as a single vector [Pu1v1, ... Pu`numPointsU'v1, Pu1v2, ...].  </param>
                /// <param name="numPointsU"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddBezierSurface(int[] pointTags, int numPointsU, int tag = -1)
                {
                    return IWrap.GmshModelOccAddBezierSurface(pointTags, pointTags.LongLength, numPointsU, tag, ref _ierr);
                }

                /// <summary>
                /// Add a surface loop (a closed shell) formed by `surfaceTags'.  
                /// Return the tag of the surface loop.
                /// </summary>
                /// <param name="surfaceTags"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <param name="sewing"> Setting `sewing' allows to build a shell made of surfaces that share geometrically identical (but topologically different) curves. </param>
                /// <returns></returns>
                public static int AddSurfaceLoop(int[] surfaceTags, bool sewing = false, int tag = -1)
                {
                    return IWrap.GmshModelOccAddSurfaceLoop(surfaceTags, surfaceTags.LongLength, tag, Convert.ToInt32(sewing), ref _ierr);
                }

                /// <summary>
                /// Add a volume (a region) defined by one or more surface loops `shellTags'. 
                /// Return the tag of the volume.
                /// </summary>
                /// <param name="shellTags"> The first surface loop defines the exterior boundary; additional surface loop define holes. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwisea new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddVolume(int[] shellTags, int tag = -1)
                {
                    return IWrap.GmshModelOccAddVolume(shellTags, shellTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a sphere of center (`xc', `yc', `zc') and radius `r'. 
                /// The optional `angle1' and `angle2' arguments define the polar angle opening(from -Pi/2 to Pi/2). 
                /// The optional `angle3' argument defines the azimuthal opening (from 0 to 2*Pi).  
                /// Return the tag of the sphere.
                /// </summary>
                /// <param name="xc"></param>
                /// <param name="yc"></param>
                /// <param name="zc"></param>
                /// <param name="radius"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="angle1"></param>
                /// <param name="angle2"></param>
                /// <param name="angle3"></param>
                /// <returns></returns>
                public static int AddSphere(double xc, double yc, double zc, double radius, int tag = -1, double angle1 = -Math.PI / 2, double angle2 = Math.PI / 2, double angle3 = 2 * Math.PI)
                {
                    return IWrap.GmshModelOccAddSphere(xc, yc, zc, radius, tag, angle1, angle2, angle3, ref _ierr);
                }

                /// <summary>
                /// Add a parallelepipedic box defined by a point (`x', `y', `z') and the extents along the x-, y- and z-axes. 
                /// Return the tag of the box.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <returns></returns>
                public static int AddBox(double x, double y, double z, double dx, double dy, double dz, int tag = -1)
                {
                    return IWrap.GmshModelOccAddBox(x, y, z, dx, dy, dz, tag, ref _ierr);
                }

                /// <summary>
                /// Add a cylinder, defined by the center (`x', `y', `z') of its first circular face, 
                /// the 3 components(`dx', `dy', `dz') of the vector defining its axis and its radius `r'. 
                /// The optional `angle' argument defines the angular opening (from 0 to 2*Pi). 
                /// Return the tag of the cylinder.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="r"></param>
                /// <param name="tag"></param>
                /// <param name="angle"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddCylinder(double x, double y, double z, double dx, double dy, double dz, double r, double angle = 2 * Math.PI, int tag = -1)
                {
                    return IWrap.GmshModelOccAddCylinder(x, y, z, dx, dy, dz, r, tag, angle, ref _ierr);
                }

                /// <summary>
                /// Add a cone, defined by the center (`x', `y', `z') of its first circular face, 
                /// the 3 components of the vector(`dx', `dy', `dz') defining its axis
                /// and the two radii `r1' and `r2' of the faces (these radii can be zero). 
                /// Return the tag of the cone.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="r1"></param>
                /// <param name="r2"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="angle"> `angle' defines the optional angular opening (from 0 to 2*Pi).  </param>
                /// <returns></returns>
                public static int AddCone(double x, double y, double z, double dx, double dy, double dz, double r1, double r2, double angle = Math.PI, int tag = -1)
                {
                    return IWrap.GmshModelOccAddCone(x, y, z, dx, dy, dz, r1, r2, tag, angle, ref _ierr);
                }

                /// <summary>
                /// Add a right angular wedge, defined by the right-angle point (`x', `y', `z')
                /// and the 3 extends along the x-, y- and z-axes(`dx', `dy', `dz'). 
                /// Return the tag of the wedge.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="ltx"> The optional argument `ltx' defines the top extent along the x-axis. </param>
                /// <returns></returns>
                public static int AddWedge(double x, double y, double z, double dx, double dy, double dz, double ltx = -1, int tag = -1)
                {
                    return IWrap.GmshModelOccAddWedge(x, y, z, dx, dy, dz, tag, ltx, ref _ierr);
                }

                /// <summary>
                /// Add a torus, defined by its center (`x', `y', `z') and its 2 radii `r' and `r2'. 
                /// Return the tag of the wedge.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="r1"></param>
                /// <param name="r2"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.</param>
                /// <param name="angle"> The optional argument `angle' defines the angular opening(from 0 to 2*Pi)</param>
                /// <returns></returns>
                public static int AddTorus(double x, double y, double z, double r1, double r2, double angle = Math.PI, int tag = -1)
                {
                    return IWrap.GmshModelOccAddTorus(x, y, z, r1, r2, tag, angle, ref _ierr);
                }

                /// <summary>
                /// Add a volume (if the optional argument `makeSolid' is set) or surfaces defined through the open or closed wires `wireTags'.
                /// </summary>
                /// <param name="wireTags"></param>
                /// <param name="outDimTags"> The new entities are returned in `outDimTags'. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.</param>
                /// <param name="makeSolid"></param>
                /// <param name="makeRuled"> If the optional argument `makeRuled' is set, the surfaces created on the boundary are forced to be ruled surfaces.</param>
                /// <param name="maxDegree"> If `maxDegree' is positive, set the maximal degree of resulting surface.</param>
                /// <returns></returns>
                public static void AddThruSections(int[] wireTags, out Tuple<int, int>[] dimTags, int tag, bool makeSolid = true, bool makeRuled = false, int maxDegree = -1)
                {
                    IntPtr dtP;
                    long outDimTags_n;
                    IWrap.GmshModelOccAddThruSections(wireTags, wireTags.LongLength, out dtP, out outDimTags_n, tag, Convert.ToInt32(makeSolid), Convert.ToInt32(makeRuled), maxDegree, ref _ierr);

                    dimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(dtP, temp, 0, (int)outDimTags_n);

                        dimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrap.GmshFree(dtP);
                }

                /// <summary>
                /// Add a hollowed volume built from an initial volume `volumeTag' and a set of faces from this volume `excludeSurfaceTags', which are to be removed. 
                /// The remaining faces of the volume become the walls of the hollowed solid, with thickness `offset'. 
                /// </summary>
                /// <param name="volumeTag"></param>
                /// <param name="excludeSurfaceTags"></param>
                /// <param name="offset"></param>
                /// <param name="outDimTags"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                public static void AddThickSolid(int volumeTag, int[] excludeSurfaceTags, double offset, out Tuple<int, int>[] outDimTags, int tag = -1)
                {
                    IntPtr dimTags;
                    long outDimTags_n;
                    IWrap.GmshModelOccAddThickSolid(volumeTag, excludeSurfaceTags, excludeSurfaceTags.LongLength, offset, out dimTags, out outDimTags_n, tag, ref _ierr);

                    outDimTags = null;
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(dimTags, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }
                    IWrap.GmshFree(dimTags);
                }

                /// <summary>
                /// Extrude the model entities `dimTags' by translation along (`dx', `dy', `dz'). 
                /// Return extruded entities in `outDimTags'.
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="outDimTags"></param>
                /// <param name="numElements"> If `numElements' is not empty, also extrude the mesh: the entries in `numElements' give the number of elements in each layer. </param>
                /// <param name="heights"> If `height' is not empty, it provides the (cumulative) height of the different layers, normalized to 1.</param>
                /// <param name="recombine"></param>
                public static void Extrude(Tuple<int, int>[] dimTags, double dx, double dy, double dz, out Tuple<int, int>[] outDimTags, int[] numElements = default, double[] heights = default, bool recombine = false)
                {
                    IntPtr out_DimTags;
                    long outDimTags_n;
                    if (numElements == default) numElements = new int[0];
                    if (heights == default) heights = new double[0];

                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccExtrude(arr, arr.LongLength, dx, dy, dz, out out_DimTags, out outDimTags_n, numElements, numElements.LongLength, heights, heights.LongLength, Convert.ToInt32(recombine), ref _ierr);
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
                /// Remove all duplicate entities (different entities at the same geometrical location) after intersecting(using boolean fragments) all highest dimensional entities.
                /// </summary>
                public static void RemoveAllDuplicates()
                {
                    IWrap.GmshModelOccRemoveAllDuplicates(ref _ierr);
                }

                /// <summary>
                /// Extrude the model entities `dimTags' by rotation of `angle' radians around
                /// the axis of revolution defined by the point(`x', `y', `z') and the
                /// direction (`ax', `ay', `az'). Return extruded entities in `outDimTags'. If
                /// `numElements' is not empty, also extrude the mesh: the entries in
                /// `numElements' give the number of elements in each layer. 
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="ax"></param>
                /// <param name="ay"></param>
                /// <param name="az"></param>
                /// <param name="angle">  When the mesh is extruded the angle should be strictly smaller than 2*Pi. </param>
                /// <param name="outDimTags"></param>
                /// <param name="numElements"></param>
                /// <param name="heights"> If `height' is not empty, it provides the(cumulative) height of the different layers, normalized to 1. </param>
                /// <param name="recombine"></param>
                public static void Revolve(Tuple<int, int>[] dimTags, double x, double y, double z, double ax, double ay, double az, double angle, out Tuple<int, int>[] outDimTags, int[] numElements, double[] heights = null, bool recombine = true)
                {
                    IntPtr out_DimTags;
                    long outDimTags_n;
                    if (numElements == default) numElements = new int[0];
                    if (heights == default) heights = new double[0];

                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccRevolve(arr, arr.LongLength, x, y, z, ax, ay, az, angle, out out_DimTags, out outDimTags_n, numElements, numElements.LongLength, heights, heights.LongLength, Convert.ToInt32(recombine), ref _ierr);
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
                /// Add a pipe by extruding the entities `dimTags' along the wire `wireTag'.
                /// Return the pipe in `outDimTags'.
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="wireTag"></param>
                /// <param name="outDimTags"></param>
                public static void AddPipe(Tuple<int, int>[] dimTags, int wireTag, out Tuple<int, int>[] outDimTags)
                {
                    IntPtr out_dimTags;
                    long outDimTags_n;
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccAddPipe(arr, arr.LongLength, wireTag, out out_dimTags, out outDimTags_n, ref _ierr);
                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(out_dimTags, temp, 0, (int)outDimTags_n);

                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrap.GmshFree(out_dimTags);
                }

                /// <summary>
                /// Fillet the volumes `volumeTags' on the curves `curveTags' with radii
                /// `radii'. The `radii' vector can either contain a single radius, as many
                /// radii as `curveTags', or twice as many as `curveTags' (in which case
                /// different radii are provided for the begin and end points of the curves).
                /// Return the filleted entities in `outDimTags'. Remove the original volume if
                /// `removeVolume' is set. 
                /// </summary>    
                public static void Fillet(int[] volumeTags, int[] curveTags, double[] radii, out Tuple<int, int>[] dimTags, bool removeVolume = false)
                {
                    IntPtr dtP;
                    long outDimTags_n;
                    IWrap.GmshModelOccFillet(volumeTags, volumeTags.LongLength, curveTags, curveTags.LongLength, radii, radii.LongLength, out dtP, out outDimTags_n, Convert.ToInt32(removeVolume), ref _ierr);
                    dimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(dtP, temp, 0, (int)outDimTags_n);
                        dimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrap.GmshFree(dtP);
                }

                /// <summary>
                /// Chamfer the volumes `volumeTags' on the curves `curveTags' with distances `distances' measured on surfaces `surfaceTags'. 
                /// The `distances' vector can either contain a single distance, as many distances as `curveTags' and `surfaceTags', 
                /// or twice as many as `curveTags' and `surfaceTags' (in which case the first in each pair is measured on the corresponding surface in `surfaceTags', 
                /// the other on the other adjacent surface). 
                /// Return the chamfered entities in `outDimTags'. 
                /// </summary>
                /// <param name="volumeTags"></param>
                /// <param name="curveTags"></param>
                /// <param name="surfaceTags"></param>
                /// <param name="distances"></param>
                /// <param name="dimTags"></param>
                /// <param name="removeVolume"> Remove the original volume if `removeVolume' is set. </param>
                public static void Chamfer(int[] volumeTags, int[] curveTags, int[] surfaceTags, double[] distances, out Tuple<int, int>[] dimTags, bool removeVolume = false)
                {
                    IntPtr dtP;
                    long outDimTags_n;
                    IWrap.GmshModelOccChamfer(volumeTags, volumeTags.LongLength, curveTags, curveTags.LongLength, surfaceTags, surfaceTags.LongLength, distances, distances.LongLength, out dtP, out outDimTags_n, Convert.ToInt32(removeVolume), ref _ierr);

                    dimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(dtP, temp, 0, (int)outDimTags_n);

                        dimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrap.GmshFree(dtP);
                }

                /// <summary>
                /// Get the center of mass of the OpenCASCADE entity of dimension `dim' and tag `tag'
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                public static void GetCenterOfMass(int dim, int tag, out double x, out double y, out double z)
                {
                    IWrap.GmshModelOccGetCenterOfMass(dim, tag, out x, out y, out z, ref _ierr);
                }

                /// <summary>
                /// Synchronize the OpenCASCADE CAD representation with the current IguanaGmsh model.
                /// This can be called at any time, but since it involves a non trivial amount of processing, the number of synchronization points should normally be minimized.
                /// </summary>
                public static void Synchronize()
                {
                    IWrap.GmshModelOccSynchronize(ref _ierr);
                }

                /// <summary>
                /// Compute the boolean union (the fusion) of the entities `objectDimTags' and
                /// `toolDimTags'. Return the resulting entities in `outDimTags'. If `tag' is
                /// positive, try to set the tag explicitly(only valid if the boolean
                /// operation results in a single entity). Remove the object if `removeObject'
                /// is set.Remove the tool if `removeTool' is set. 
                /// </summary>
                /// <param name="objectDimTags"></param>
                /// <param name="toolDimTags"></param>
                /// <param name="outDimTags_out"></param>
                /// <param name="tag"></param>
                /// <param name="removeObject"></param>
                /// <param name="removeTool"></param>
                public static void Fuse(Tuple<int, int>[] objectDimTags, Tuple<int, int>[] toolDimTags, out Tuple<int, int>[] outDimTags_out, int tag = -1, bool removeObject = true, bool removeTool = true)
                {
                    IntPtr outDimTags, outDimTagsMap, outDimTagsMap_n;
                    long outDimTags_n, outDimTagsMap_nn;

                    var arrObj = IHelpers.FlattenIntTupleArray(objectDimTags);
                    var arrTool = IHelpers.FlattenIntTupleArray(toolDimTags);
                    IWrap.GmshModelOccFuse(arrObj, arrObj.LongLength, arrTool, arrTool.LongLength, out outDimTags, out outDimTags_n, out outDimTagsMap, out outDimTagsMap_n, out outDimTagsMap_nn, tag, Convert.ToInt32(removeObject), Convert.ToInt32(removeTool), ref _ierr);

                    outDimTags_out = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(outDimTags, temp, 0, (int)outDimTags_n);

                        outDimTags_out = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrap.GmshFree(outDimTags);
                    IWrap.GmshFree(outDimTagsMap);
                    IWrap.GmshFree(outDimTagsMap_n);
                }

                /// <summary>
                /// Compute the boolean intersection (the common parts) of the entities
                /// `objectDimTags' and `toolDimTags'. Return the resulting entities in
                /// `outDimTags'. If `tag' is positive, try to set the tag explicitly(only
                /// valid if the boolean operation results in a single entity). Remove the
                /// object if `removeObject' is set. Remove the tool if `removeTool' is set.
                /// </summary>
                /// <param name="objectDimTags"></param>
                /// <param name="toolDimTags"></param>
                /// <param name="outDimTags_out"></param>
                /// <param name="tag"></param>
                /// <param name="removeObject"></param>
                /// <param name="removeTool"></param>
                public static void Intersect(Tuple<int, int>[] objectDimTags, Tuple<int, int>[] toolDimTags, out Tuple<int, int>[] outDimTags_out, int tag = -1, bool removeObject = true, bool removeTool = true)
                {
                    IntPtr outDimTags, outDimTagsMap, outDimTagsMap_n;
                    long outDimTags_n, outDimTagsMap_nn;

                    var arrObj = IHelpers.FlattenIntTupleArray(objectDimTags);
                    var arrTool = IHelpers.FlattenIntTupleArray(toolDimTags);
                    IWrap.GmshModelOccIntersect(arrObj, arrObj.LongLength, arrTool, arrTool.LongLength, out outDimTags, out outDimTags_n, out outDimTagsMap, out outDimTagsMap_n, out outDimTagsMap_nn, tag, Convert.ToInt32(removeObject), Convert.ToInt32(removeTool), ref _ierr);
                    outDimTags_out = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(outDimTags, temp, 0, (int)outDimTags_n);

                        outDimTags_out = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrap.GmshFree(outDimTags);
                    IWrap.GmshFree(outDimTagsMap);
                    IWrap.GmshFree(outDimTagsMap_n);
                }

                /// <summary>
                /// Compute the boolean difference between the entities `objectDimTags' and
                /// `toolDimTags'. Return the resulting entities in `outDimTags'. If `tag' is
                /// positive, try to set the tag explicitly(only valid if the boolean
                /// operation results in a single entity). Remove the object if `removeObject'
                /// is set.Remove the tool if `removeTool' is set.
                /// </summary>
                /// <param name="objectDimTags"></param>
                /// <param name="toolDimTags"></param>
                /// <param name="outDimTags_out"></param>
                /// <param name="tag"></param>
                /// <param name="removeObject"></param>
                /// <param name="removeTool"></param>
                public static void Cut(Tuple<int, int>[] objectDimTags, Tuple<int, int>[] toolDimTags, out Tuple<int, int>[] dimTags, int tag = -1, bool removeObject = true, bool removeTool = true)
                {
                    int[] objectDimTags_flatten = IHelpers.FlattenIntTupleArray(objectDimTags);
                    int[] toolDimTags_flatten = IHelpers.FlattenIntTupleArray(toolDimTags);
                    IntPtr outDimTags, outDimTagsMap, outDimTagsMap_n;
                    long outDimTags_n, outDimTagsMap_nn;
                    IWrap.GmshModelOccCut(objectDimTags_flatten, objectDimTags_flatten.LongLength, toolDimTags_flatten, toolDimTags_flatten.LongLength, out outDimTags, out outDimTags_n, out outDimTagsMap, out outDimTagsMap_n, out outDimTagsMap_nn, tag, Convert.ToInt32(removeObject), Convert.ToInt32(removeTool), ref _ierr);
                    dimTags = null;
                    if (outDimTags_n > 0)
                    {
                        var temp = new long[outDimTags_n];
                        Marshal.Copy(outDimTags, temp, 0, (int)outDimTags_n);
                        dimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrap.GmshFree(outDimTags);
                    IWrap.GmshFree(outDimTagsMap);
                    IWrap.GmshFree(outDimTagsMap_n);
                }

                /// <summary>
                /// Compute the boolean fragments (general fuse) of the entities
                /// `objectDimTags' and `toolDimTags'. Return the resulting entities in
                /// `outDimTags'. If `tag' is positive, try to set the tag explicitly(only
                /// valid if the boolean operation results in a single entity). Remove the
                /// object if `removeObject' is set. Remove the tool if `removeTool' is set.
                /// </summary>
                /// <param name="objectDimTags"></param>
                /// <param name="toolDimTags"></param>
                /// <param name="outDimTags_out"></param>
                /// <param name="tag"></param>
                /// <param name="removeObject"></param>
                /// <param name="removeTool"></param>
                public static void Fragment(Tuple<int, int>[] objectDimTags, Tuple<int, int>[] toolDimTags, out Tuple<int, int>[] dimTags, out Tuple<int, int>[][] dimTagsMap, int tag = -1, bool removeObject = true, bool removeTool = true)
                {
                    int[] objectDimTags_flatten = IHelpers.FlattenIntTupleArray(objectDimTags);
                    int[] toolDimTags_flatten = IHelpers.FlattenIntTupleArray(toolDimTags);

                    IntPtr outDimTags, outDimTagsMap, outDimTagsMap_n;
                    long outDimTags_n, outDimTagsMap_nn;
                    IWrap.GmshModelOccFragment(objectDimTags_flatten, objectDimTags_flatten.LongLength, toolDimTags_flatten, toolDimTags_flatten.LongLength, out outDimTags, out outDimTags_n, out outDimTagsMap, out outDimTagsMap_n, out outDimTagsMap_nn, tag, Convert.ToInt32(removeObject), Convert.ToInt32(removeTool), ref _ierr);

                    dimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(outDimTags, temp, 0, (int)outDimTags_n);
                        dimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    var tempMapSize = new long[outDimTagsMap_nn];
                    var tempMap = new IntPtr[outDimTagsMap_nn];
                    Marshal.Copy(outDimTagsMap_n, tempMapSize, 0, (int)outDimTagsMap_nn);
                    Marshal.Copy(outDimTagsMap, tempMap, 0, (int)outDimTagsMap_nn);

                    dimTagsMap = new Tuple<int, int>[outDimTagsMap_nn][];

                    for (int i = 0; i < outDimTagsMap_nn; i++)
                    {
                        // Marshalling
                        var temp = new int[tempMapSize[i]];
                        Marshal.Copy(tempMap[i], temp, 0, (int)tempMapSize[i]);

                        dimTagsMap[i] = IHelpers.GraftIntTupleArray(temp);
                    }

                    IWrap.GmshFree(outDimTags);
                    IWrap.GmshFree(outDimTagsMap);
                    IWrap.GmshFree(outDimTagsMap_n);
                }

                /// <summary>
                /// Translate the model entities `dimTags' along (`dx', `dy', `dz'). 
                /// </summary>
                public static void Translate(Tuple<int, int>[] dimTags, double dx, double dy, double dz)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccTranslate(arr, arr.LongLength, dx, dy, dz, ref _ierr);
                }

                /// <summary>
                /// Rotate the model entities `dimTags' of `angle' radians around the axis of
                /// revolution defined by the point(`x', `y', `z') and the direction (`ax', `ay', `az').
                /// </summary>
                public static void Rotate(Tuple<int, int>[] dimTags, double x, double y, double z, double ax, double ay, double az, double angle)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccRotate(arr, arr.LongLength, x, y, z, ax, ay, az, angle, ref _ierr);
                }

                /// <summary>
                /// Scale the model entities `dimTag' by factors `a', `b' and `c' along the
                /// three coordinate axes; use(`x', `y', `z') as the center of the homothetic transformation.
                /// </summary>
                public static void Dilate(Tuple<int, int>[] dimTags, double x, double y, double z, double a, double b, double c)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccDilate(arr, arr.LongLength, x, y, z, a, b, c, ref _ierr);
                }

                /// <summary>
                /// Apply a symmetry transformation to the model entities `dimTag', with
                /// respect to the plane of equation `a' * x + `b' * y + `c' * z + `d' = 0.
                /// </summary>
                public static void Mirror(Tuple<int, int>[] dimTags, double a, double b, double c, double d)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccMirror(arr, arr.LongLength, a, b, c, d, ref _ierr);
                }

                /// <summary>
                /// Apply a symmetry transformation to the model entities `dimTag', with
                /// respect to the plane of equation `a' * x + `b' * y + `c' * z + `d' = 0.
                /// (This is a synonym for `mirror', which will be deprecated in a future release.) 
                /// </summary>
                public static void Symmetrize(Tuple<int, int>[] dimTags, double a, double b, double c, double d)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccSymmetrize(arr, arr.LongLength, a, b, c, d, ref _ierr);
                }

                /// <summary>
                /// Apply a general affine transformation matrix `a' (16 entries of a 4x4
                /// matrix, by row; only the 12 first can be provided for convenience) to the model entities `dimTag'. 
                /// </summary>
                public static void AffineTransfom(Tuple<int, int>[] dimTags, out double[] a)
                {
                    IntPtr aP;
                    long a_n;

                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccAffineTransform(arr, arr.LongLength, out aP, out a_n, ref _ierr);

                    a = new double[a_n];
                    Marshal.Copy(aP, a, 0, (int)a_n);

                    Free(aP);
                }

                /// <summary>
                /// Copy the entities `dimTags'; the new entities are returned in `outDimTags'.
                /// </summary>
                public static void Copy(Tuple<int, int>[] dimTags, out Tuple<int, int>[] outDimTags)
                {
                    IntPtr dtP;
                    long outDimTags_n;

                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccCopy(arr, arr.LongLength, out dtP, out outDimTags_n, ref _ierr);

                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(dtP, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    Free(dtP);
                }

                /// <summary>
                /// Remove the entities `dimTags'. If `recursive' is true, remove all the
                /// entities on their boundaries, down to dimension 0. 
                /// </summary>
                public static void Remove(Tuple<int, int>[] dimTags, bool recursive = false)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccRemove(arr, arr.LongLength, Convert.ToInt32(recursive), ref _ierr);
                }

                /// <summary>
                /// Apply various healing procedures to the entities `dimTags' (or to all the
                /// entities in the model if `dimTags' is empty). Return the healed entities in
                /// `outDimTags'. Available healing options are listed in the Gmsh reference manual.
                /// </summary>
                public static void HealShapes(out Tuple<int, int>[] outDimTags, Tuple<int, int>[] dimTags, double tolerance, bool fixDegenerated, bool fixSmallEdges, bool fixSmallFaces, bool sewFaces, bool makeSolids)
                {
                    IntPtr dtP;
                    long outDimTags_n;

                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccHealShapes(out dtP, out outDimTags_n, arr, arr.LongLength, tolerance, Convert.ToInt32(fixDegenerated), Convert.ToInt32(fixSmallEdges), Convert.ToInt32(fixSmallFaces), Convert.ToInt32(sewFaces), Convert.ToInt32(makeSolids), ref _ierr);

                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(dtP, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    Free(dtP);
                }

                /// <summary>
                /// Get all the OpenCASCADE entities. If `dim' is >= 0, return only the
                /// entities of the specified dimension(e.g.points if `dim' == 0). The
                /// entities are returned as a vector of (dim, tag) integer pairs.
                /// </summary>
                public static void GetEntities(out Tuple<int, int>[] dimTags, int dim)
                {
                    IntPtr dtP;
                    long dimTags_n;
                    IWrap.GmshModelOccGetEntities(out dtP, out dimTags_n, dim, ref _ierr);

                    dimTags = new Tuple<int, int>[0];
                    if (dimTags_n > 0)
                    {
                        var temp = new int[dimTags_n];
                        Marshal.Copy(dtP, temp, 0, (int)dimTags_n);
                        dimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    Free(dtP);
                }

                /// <summary>
                /// Get the OpenCASCADE entities in the bounding box defined by the two points
                /// (`xmin', `ymin', `zmin') and (`xmax', `ymax', `zmax'). If `dim' is >= 0,
                /// return only the entities of the specified dimension(e.g.points if `dim' == 0).
                /// </summary>
                public static void GetEntitiesInBoundingBox(double xmin, double ymin, double zmin, double xmax, double ymax, double zmax, out int[] tags, int dim)
                {
                    IntPtr tP;
                    long tags_n;
                    IWrap.GmshModelOccGetEntitiesInBoundingBox(xmin, ymin, zmin, xmax, ymax, zmax, out tP, out tags_n, dim, ref _ierr);

                    tags = new int[tags_n];
                    Marshal.Copy(tP, tags, 0, (int)tags_n);

                    Free(tP);
                }

                /// <summary>
                /// Get the bounding box (`xmin', `ymin', `zmin'), (`xmax', `ymax', `zmax') of
                /// the OpenCASCADE entity of dimension `dim' and tag `tag'.
                /// </summary>
                public static void GetBoundingBox(int dim, int tag, out double xmin, out double ymin, out double zmin, out double xmax, out double ymax, out double zmax)
                {
                    IWrap.GmshModelOccGetBoundingBox(dim, tag, out xmin, out ymin, out zmin, out xmax, out ymax, out zmax, ref _ierr);
                }

                /// <summary>
                /// Set a mesh size constraint on the model entities `dimTags'. Currently only
                /// entities of dimension 0 (points) are handled.
                /// </summary>
                public static void SetSize(Tuple<int, int>[] dimTags, double size)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelOccMeshSetSize(arr, arr.LongLength, size, ref _ierr);
                }

                /// <summary>
                /// Get the mass of the OpenCASCADE entity of dimension `dim' and tag `tag'.
                /// </summary>
                public static void GetMass(int dim, int tag, out double mass)
                {
                    IWrap.GmshModelOccGetMass(dim, tag, out mass, ref _ierr);
                }

                /// <summary>
                /// Get the matrix of inertia (by row) of the OpenCASCADE entity of dimension `dim' and tag `tag'.
                /// </summary>
                public static void GetMatrixOfInertia(int dim, int tag, out double[] mat)
                {
                    IntPtr mP;
                    long mat_n;
                    IWrap.GmshModelOccGetMatrixOfInertia(dim, tag, out mP, out mat_n, ref _ierr);

                    mat = new double[mat_n];
                    Marshal.Copy(mP, mat, 0, (int)mat_n);

                    Free(mP);
                }

                /// <summary>
                /// Import BREP, STEP or IGES shapes from the file `fileName'. The imported
                /// entities are returned in `outDimTags'. If the optional argument
                /// `highestDimOnly' is set, only import the highest dimensional entities in
                /// the file.The optional argument `format' can be used to force the format of
                /// the file (currently "brep", "step" or "iges"). 
                /// </summary>
                public static void ImportShapes(string fileName, out Tuple<int, int>[] outDimTags, bool highestDimOnly = false, string format = "step")
                {
                    IntPtr dtP;
                    long outDimTags_n;
                    IWrap.GmshModelOccImportShapes(fileName, out dtP, out outDimTags_n, Convert.ToInt32(highestDimOnly), format, ref _ierr);

                    outDimTags = new Tuple<int, int>[0];
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(dtP, temp, 0, (int)outDimTags_n);
                        outDimTags = IHelpers.GraftIntTupleArray(temp);
                    }

                    Free(dtP);
                }
                #endregion
            }
        }
    }
}
