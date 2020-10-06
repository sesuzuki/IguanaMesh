using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.ICollections;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iguana.IguanaMesh.IWrappers.IExtensions
{
    public static class IguanaGmshFactory
    {
        public static int OCCSurfacePatch(NurbsCurve crv, List<Point3d> patchs=default, bool synchronize=false)
        {
            // 1._ Build Points
            double cParam = crv.Domain.Length / 4;
            double[] cList = new double[] { cParam, cParam * 2, cParam * 3, cParam * 4 };
            Curve[] cCrv = crv.Split(cList);

            int count = 10;
            int[] dup = new int[4];
            int[] crvTags = new int[4];

            for (int i = 0; i < cCrv.Length; i++)
            {
                Curve c = cCrv[i];
                double t = c.Domain.Length / (count - 1);

                int[] ptTags = new int[count];
                for (int j = 0; j < count; j++)
                {
                    Point3d p = c.PointAt(j * t + c.Domain.T0);

                    if (i == 0)
                    {
                        ptTags[j] = IguanaGmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z);
                        if (j == 0) dup[0] = ptTags[j];
                        if (j == count - 1) dup[1] = ptTags[j];
                    }
                    else if (i == 3)
                    {
                        if (j == 0) ptTags[j] = dup[3];
                        else if (j == count - 1) ptTags[j] = dup[0];
                        else ptTags[j] = IguanaGmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z);
                    }
                    else
                    {
                        if (j == 0) ptTags[j] = dup[i];
                        else
                        {
                            ptTags[j] = IguanaGmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z);
                            if (j == count - 1) dup[i + 1] = ptTags[j];
                        }
                    }
                }

                crvTags[i] = IguanaGmsh.Model.GeoOCC.AddSpline(ptTags);
            }

            // 2._ Check points to patch
            if (patchs == default) patchs = new List<Point3d>();
            int[] patchPts = new int[patchs.Count];

            Point3d p2;
            for (int i = 0; i < patchs.Count; i++)
            {
                p2 = patchs[i];
                patchPts[i] = IguanaGmsh.Model.GeoOCC.AddPoint(p2.X, p2.Y, p2.Z);
            }


            // 3._ Build OCC Geometry
            int wireTag = IguanaGmsh.Model.GeoOCC.AddCurveLoop(crvTags);
            int surfaceTag = IguanaGmsh.Model.GeoOCC.AddSurfaceFilling(wireTag, patchPts);

            // 5._ Synchronize model
            if (synchronize) IguanaGmsh.Model.GeoOCC.Synchronize();

            return surfaceTag;
        }

        internal static int EvaluatePoint(PointCloud pts, Point3d p, double t)
        {
            int idx = pts.ClosestPoint(p);
            if (idx != -1 && p.DistanceTo(pts[idx].Location) > t) idx = -1;
            return idx;
        }

        public static void OCCEmbedConstraintsOnSurface(List<IguanaGmshConstraint> constraints, int surfaceTag, bool synchronize=false)
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
                NurbsCurve crv;
                double t = 0.001;
                int tag, idx;
                int[] tempTags;

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
                            poly = (Polyline) data.RhinoGeometry;
                            tempTags = new int[poly.Count];
                            for (int j = 0; j < poly.Count; j ++)
                            {
                                p = poly[j];
                                idx = EvaluatePoint(pts, p, t);

                                if (idx == -1)
                                {
                                    tag = IguanaGmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z, data.Size);
                                    ptsTags.Add(tag);
                                    pts.Add(p);
                                    idx = pts.Count-1;
                                }

                                tempTags[j] = idx;
                            }

                            for (int j = 0; j < poly.SegmentCount; j++)
                            {
                                crvTags.Add(IguanaGmsh.Model.GeoOCC.AddLine(ptsTags[tempTags[j]], ptsTags[tempTags[j + 1]]));
                            }

                            break;

                        case 2:
                            crv = (NurbsCurve) data.RhinoGeometry;
                            crv.MakePiecewiseBezier(true);
                            NurbsCurvePointList cPts = crv.Points;
                            NurbsCurveKnotList cKnots = crv.Knots;

                            tempTags = new int[cPts.Count];
                            double[] weightPts = new double[cPts.Count];

                            for (int j = 0; j < cPts.Count; j++)
                            {
                                p = cPts[j].Location;
                                weightPts[j] = cPts.GetWeight(j);

                                idx = EvaluatePoint(pts, p, t);

                                if (idx == -1)
                                {
                                    tag = IguanaGmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z, data.Size);
                                    ptsTags.Add(tag);
                                    pts.Add(p);
                                    idx = pts.Count - 1;
                                }

                                tempTags[j] = idx;
                            }

                            crvTags.Add(IguanaGmsh.Model.GeoOCC.AddBSpline(tempTags, crv.Degree, weightPts));

                            break;
                    }
                }

                if (synchronize) IguanaGmsh.Model.GeoOCC.Synchronize();

                if( ptsTags.Count > 0 ) IguanaGmsh.Model.Mesh.Embed(0, ptsTags.ToArray(), 2, surfaceTag);
                if( crvTags.Count > 0 ) IguanaGmsh.Model.Mesh.Embed(1, crvTags.ToArray(), 2, surfaceTag);
            }
        }

        public static int GmshCurveLoop(Curve crv, IguanaGmshSolver2D solverOpt)
        {
            Polyline poly;
            crv.TryGetPolyline(out poly);

            // Close the polyline if not closed
            if (!poly.IsClosed) poly.Add(poly[0]);

            int count = poly.Count - 1;
            int[] ln_tags = new int[count];

            Point3d pt = poly[0];
            double size = solverOpt.TargetMeshSizeAtNodes[0];
            int init = IguanaGmsh.Model.Geo.AddPoint(pt.X, pt.Y, pt.Z, size);
            int next, prev=init;
            for (int i = 1; i < count; i++)
            {
                pt = poly[i];
 
                if (solverOpt.TargetMeshSizeAtNodes.Count == count) size = solverOpt.TargetMeshSizeAtNodes[i];

                // Add points
                next = IguanaGmsh.Model.Geo.AddPoint(pt.X, pt.Y, pt.Z, size);

                // Add lines
                ln_tags[i-1] = IguanaGmsh.Model.Geo.AddLine(prev, next);
                prev = next;
            }
            ln_tags[count-1] = IguanaGmsh.Model.Geo.AddLine(prev, init);

            return IguanaGmsh.Model.Geo.AddCurveLoop(ln_tags);
        }
    }
}
