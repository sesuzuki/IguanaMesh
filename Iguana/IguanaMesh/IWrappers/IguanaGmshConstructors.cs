using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.ICollections;
using Iguana.IguanaMesh.IWrappers.IConstraints;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iguana.IguanaMesh.IWrappers
{
    public static class IguanaGmshConstructors
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

        public static void OCCEmbedConstraintsOnSurface(List<IguanaGmshConstraint> constraints, int surfaceTag, ref IVertexCollection vertices, bool synchronize=false)
        {
            int count = constraints.Count;

            if (count>0)
            {
                List<int> embedPts = new List<int>();
                PointCloud cloudPts = new PointCloud();
                IguanaGmshConstraint data;
                Point3d p1, p2, p3;
                Line ln;
                double dist;
                for (int i = 0; i < count; i++)
                {
                    data = constraints[i];
                    int tag;
                    bool flag1 = false, flag2=false;
                    switch (data.Dim) {
                        case 0:
                            p1 = (Point3d) data.RhinoGeometry;
                            if (cloudPts.Count == 0) flag1 = true;
                            else
                            {
                                int cIdx = cloudPts.ClosestPoint(p1);
                                p2 = cloudPts[cIdx].Location;
                                dist = p1.DistanceTo(p2);
                                if (dist > 0.1) flag1 = true;
              
                            }

                            if (flag1)
                            {
                                cloudPts.Add(p1);
                                tag = IguanaGmsh.Model.GeoOCC.AddPoint(p1.X, p1.Y, p1.Z, data.Size);
                                vertices.AddVertex(tag, new ITopologicVertex(p1.X, p1.Y, p1.Z));
                                embedPts.Add(tag);
                            }

                            break;
                        case 1:
                            ln = (Line)data.RhinoGeometry;
                            p1 = ln.From;
                            p2 = ln.To;

                            if (cloudPts.Count == 0) { flag1 = true; flag2 = false; }
                            else
                            {
                                int cIdx = cloudPts.ClosestPoint(p1);
                                p3 = cloudPts[cIdx].Location;
                                dist = p1.DistanceTo(p3);
                                if (dist > 0.1) flag1 = true;
                                cIdx = cloudPts.ClosestPoint(p2);
                                p3 = cloudPts[cIdx].Location;
                                dist = p2.DistanceTo(p3);
                                if (dist > 0.1) flag2 = true;
                            }

                            if (flag1)
                            {
                                cloudPts.Add(p1);
                                tag = IguanaGmsh.Model.GeoOCC.AddPoint(p1.X, p1.Y, p1.Z, data.Size);
                                vertices.AddVertex(tag, new ITopologicVertex(p1.X, p1.Y, p1.Z));
                                embedPts.Add(tag);
                            }

                            if (flag2)
                            {
                                cloudPts.Add(p2);
                                tag = IguanaGmsh.Model.GeoOCC.AddPoint(p2.X, p2.Y, p2.Z, data.Size);
                                vertices.AddVertex(tag, new ITopologicVertex(p2.X, p2.Y, p2.Z));
                                embedPts.Add(tag);
                            }

                            break;
                    }
                }

                if(synchronize) IguanaGmsh.Model.GeoOCC.Synchronize();

                IguanaGmsh.Model.Mesh.Embed(0, embedPts.ToArray(), 2, surfaceTag);
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
