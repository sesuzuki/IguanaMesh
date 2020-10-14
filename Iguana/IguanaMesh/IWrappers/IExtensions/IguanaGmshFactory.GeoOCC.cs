using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers.IExtensions
{
    public static partial class IguanaGmshFactory
    {
        public static class GeoOCC
        {
            public static int SpherePatch(Sphere sphere)
            {
                Point3d c = sphere.Center;
                int srfTag = IguanaGmsh.Model.GeoOCC.AddSphere(c.X, c.Y, c.Z, sphere.Radius, -Math.PI / 2, Math.PI / 2, 2 * Math.PI);
                return srfTag;
            }

            public static int CurveLoopFromRhinoCurve(Curve crv, double size)
            {
                int[] crvTags = SplinesFromRhinoCurve(crv, size);
                int wireTag = IguanaGmsh.Model.GeoOCC.AddWire(crvTags);
                return wireTag;
            }

            public static int[] LinesFromRhinoPolyline(Polyline poly, double size, ref List<int> ptsTags, PointCloud ptsCloud = default, double t = 0.001)
            {
                Point3d p;
                int tag;
                int[] crvTags = new int[poly.SegmentCount];

                if (ptsCloud == default)
                {
                    int[] tempTags = new int[poly.Count];
                    int idx;
                    for (int j = 0; j < poly.Count; j++)
                    {
                        p = poly[j];
                        idx = EvaluatePoint(ptsCloud, p, t);

                        if (idx == -1)
                        {
                            tag = IguanaGmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z, size);
                            ptsTags.Add(tag);
                            ptsCloud.Add(p);
                            idx = ptsCloud.Count - 1;
                        }

                        tempTags[j] = idx;
                    }

                    for (int j = 0; j < poly.SegmentCount; j++)
                    {
                        crvTags[j] = IguanaGmsh.Model.GeoOCC.AddLine(ptsTags[tempTags[j]], ptsTags[tempTags[j + 1]]);
                    }
                }
                else
                {

                    for (int j = 0; j < poly.Count; j++)
                    {
                        p = poly[j];
                        tag = IguanaGmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z, size);
                        ptsTags.Add(tag);
                    }

                    for (int j = 0; j < poly.SegmentCount; j++)
                    {
                        crvTags[j] = IguanaGmsh.Model.GeoOCC.AddLine(ptsTags[j], ptsTags[j + 1]);
                    }
                }

                return crvTags;
            }

            public static int[] SplinesFromRhinoCurve(Curve crv, double size)
            {
                // Covert curve into polycurve
                PolyCurve pc = crv.ToArcsAndLines(Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, Rhino.RhinoDoc.ActiveDoc.ModelAngleToleranceRadians, 0, 1);

                // Divide points into 4 groups and create splines
                int remain = pc.SegmentCount % 4;
                int count = (pc.SegmentCount - remain) / 4;
                int[] crvTags = new int[4];

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
                            tag = IguanaGmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z, size);
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
                                tag = IguanaGmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z, size);
                                ptsCloud.Add(p);
                                allTags.Add(tag);
                            }
                            else tag = allTags[pIdx];

                            ptTags[j + 1] = tag;
                        }
                        sIdx++;
                    }

                    crvTags[i] = IguanaGmsh.Model.GeoOCC.AddSpline(ptTags);
                }


                return crvTags;
            }

            public static int SurfacePatch(Curve crv, IguanaGmshSolver solverOptions, List<Point3d> patchs = default, bool synchronize = false)
            {
                // 1._ Check points to patch
                if (patchs == default) patchs = new List<Point3d>();
                int[] patchPts = new int[patchs.Count];

                Point3d p2;
                for (int i = 0; i < patchs.Count; i++)
                {
                    p2 = patchs[i];
                    patchPts[i] = IguanaGmsh.Model.GeoOCC.AddPoint(p2.X, p2.Y, p2.Z);
                }

                // 3._ Build OCC Geometry
                int wireTag = CurveLoopFromRhinoCurve(crv, solverOptions.TargetMeshSizeAtNodes[0]);
                int surfaceTag = IguanaGmsh.Model.GeoOCC.AddSurfaceFilling(wireTag, patchPts);

                // 5._ Synchronize model
                if (synchronize) IguanaGmsh.Model.GeoOCC.Synchronize();

                return surfaceTag;
            }

            public static void EmbedConstraintsOnSurface(List<IguanaGmshConstraint> constraints, int surfaceTag, bool synchronize = false)
            {
                int count = constraints.Count;

                if (count > 0)
                {
                    List<int> ptsTags = new List<int>();
                    List<int> crvTags = new List<int>();

                    PointCloud pts = new PointCloud();

                    IguanaGmshConstraint data;
                    Point3d p;
                    Polyline poly;
                    Curve crv;
                    double t = 0.0001;
                    int tag, idx;

                    for (int i = 0; i < count; i++)
                    {
                        data = constraints[i];

                        switch (data.Dim)
                        {
                            case 0:
                                p = (Point3d)data.RhinoGeometry;
                                idx = EvaluatePoint(pts, p, t);

                                if (idx == -1)
                                {
                                    tag = IguanaGmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z, data.Size);
                                    ptsTags.Add(tag);
                                    pts.Add(p);
                                }

                                break;

                            case 1:
                                poly = (Polyline)data.RhinoGeometry;
                                crvTags.AddRange(LinesFromRhinoPolyline(poly, data.Size, ref ptsTags, pts, t));
                                break;

                            case 2:
                                crv = (Curve)data.RhinoGeometry;
                                crvTags.AddRange(SplinesFromRhinoCurve(crv, data.Size));
                                break;
                        }
                    }

                    if (synchronize) IguanaGmsh.Model.GeoOCC.Synchronize();

                    if (ptsTags.Count > 0) IguanaGmsh.Model.Mesh.Embed(0, ptsTags.ToArray(), 2, surfaceTag);
                    if (crvTags.Count > 0) IguanaGmsh.Model.Mesh.Embed(1, crvTags.ToArray(), 2, surfaceTag);
                }
            }
        }
    }
}
