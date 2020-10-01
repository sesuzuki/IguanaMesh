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
            crv.MakePiecewiseBezier(true);
            NurbsCurvePointList pts = crv.Points;
            NurbsCurveKnotList knots = crv.Knots;

            int[] curvePts = new int[pts.Count];
            double[] weightPts = new double[pts.Count];

            // 1._ Build Points
            ControlPoint p1;
            for (int i = 0; i < pts.Count - 1; i++)
            {
                p1 = pts[i];
                curvePts[i] = IguanaGmsh.Model.GeoOCC.AddPoint(p1.X, p1.Y, p1.Z);
                weightPts[i] = pts.GetWeight(i);
            }
            weightPts[pts.Count - 1] = weightPts[0];
            curvePts[pts.Count - 1] = curvePts[0];

            // 2._ Check points to patch
            int[] patchPts;
            if (patchs == default) patchPts = new int[0];
            else patchPts = new int[patchs.Count];

            Point3d p2;             
            for (int i = 0; i < patchs.Count; i++)
            {
                p2 = patchs[i];
                patchPts[i] = IguanaGmsh.Model.GeoOCC.AddPoint(p2.X, p2.Y, p2.Z);
            }

            // 3._ Build OCC Geometry
            int occCurve = IguanaGmsh.Model.GeoOCC.AddBSpline(curvePts, crv.Degree, weightPts);
            int occWire = IguanaGmsh.Model.GeoOCC.AddWire(new int[] { occCurve });
            int surfaceTag = IguanaGmsh.Model.GeoOCC.AddSurfaceFilling(occWire, patchPts);

            // 5._ Synchronize model
            if(synchronize) IguanaGmsh.Model.GeoOCC.Synchronize();

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
                double t = 0.001;
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
                            poly = (Polyline) data.RhinoGeometry;
                            int[] tempTags = new int[poly.Count];
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
                    }
                }

                if (synchronize) IguanaGmsh.Model.GeoOCC.Synchronize();

                if ( ptsTags.Count > 0 ) IguanaGmsh.Model.Mesh.Embed(0, ptsTags.ToArray(), 2, surfaceTag);
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
