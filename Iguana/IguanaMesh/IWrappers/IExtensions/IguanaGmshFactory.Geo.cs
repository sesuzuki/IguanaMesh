using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers.IExtensions
{
    public static partial class IguanaGmshFactory
    {
        public static class Geo
        {
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
                int next, prev = init;
                for (int i = 1; i < count; i++)
                {
                    pt = poly[i];

                    if (solverOpt.TargetMeshSizeAtNodes.Count == count) size = solverOpt.TargetMeshSizeAtNodes[i];

                    // Add points
                    next = IguanaGmsh.Model.Geo.AddPoint(pt.X, pt.Y, pt.Z, size);

                    // Add lines
                    ln_tags[i - 1] = IguanaGmsh.Model.Geo.AddLine(prev, next);
                    prev = next;
                }
                ln_tags[count - 1] = IguanaGmsh.Model.Geo.AddLine(prev, init);

                return IguanaGmsh.Model.Geo.AddCurveLoop(ln_tags);
            }

            public static int GmshPlaneSurface(Curve outer, List<Curve> inner, IguanaGmshSolver2D solverOptions)
            {
                int[] crv_tags = new int[inner.Count + 1];
                crv_tags[0] = IguanaGmshFactory.Geo.GmshCurveLoop(outer, solverOptions);

                for (int i = 0; i < inner.Count; i++) crv_tags[i + 1] = IguanaGmshFactory.Geo.GmshCurveLoop(inner[i], solverOptions);

                int surfaceTag = IguanaGmsh.Model.Geo.AddPlaneSurface(crv_tags);

                return surfaceTag;
            }

            public static void GmshSurfaceLoopFromMeshes(Mesh[] meshes, bool synchronize = false)
            {
                List<long> nodes, triangles;
                List<double> xyz;
                GetConstructiveDataFromMeshes(meshes, out nodes, out triangles, out xyz);
                GmshSurfaceLoopFromDataList(nodes, triangles, xyz);
            }

            public static void GmshSurfaceFromBrep(Brep b, bool synchronize=false)
            {
                List<long> nodes, triangles;
                List<double> xyz;
                GetConstructiveDataFromBrep(b, out nodes, out triangles, out xyz);
                GmshSurfaceLoopFromDataList(nodes, triangles, xyz, synchronize);
            }

            public static void GmshSurfaceLoopFromDataList(List<long> nodes, List<long> triangles, List<double> xyz, bool synchronize=false)
            {
                // Create one discrete surface, with its bounding curves:
                IguanaGmsh.Model.AddDiscreteEntity(2, 1, new int[] { });

                // Add all the nodes on the surface (for simplicity... see below):
                IguanaGmsh.Model.Mesh.AddNodes(2, 1, nodes.ToArray(), xyz.ToArray());
                IguanaGmsh.Model.Mesh.AddElementsByType(1, 2, new long[] { }, triangles.ToArray());

                double angle = 40 * Math.PI / 180;

                // For complex geometries, patches can be too complex, too elongated or too
                // large to be parametrized; setting the following option will force the
                // creation of patches that are amenable to reparametrization:
                bool forceParametrizablePatches = true;

                // For open surfaces include the boundary edges in the classification process:
                bool includeBoundary = true;

                // Force curves to be split on given angle:
                double curveAngle = 180 * Math.PI / 180;

                //IguanaGmsh.Model.Mesh.CreateTopology(true, false);
                IguanaGmsh.Model.Mesh.ClassifySurfaces(angle, includeBoundary, forceParametrizablePatches, curveAngle);
                IguanaGmsh.Model.Mesh.CreateGeometry();

                if (synchronize) IguanaGmsh.Model.Geo.Synchronize();
            }

            public static void GmshVolumeFromBrep(Brep b, bool synchronize=false)
            {
                GmshSurfaceFromBrep(b);

                Tuple<int, int>[] s;
                IguanaGmsh.Model.GetEntities(out s, 2);

                var sl = s.Select(ss => ss.Item2).ToArray();
                var l = IguanaGmsh.Model.Geo.AddSurfaceLoop(sl);

                var v = IguanaGmsh.Model.Geo.AddVolume(new[] { l });

                if(synchronize) IguanaGmsh.Model.Geo.Synchronize();
            }

            public static void GmshVolumeFromMeshes(Mesh[] meshes, bool synchronize = false)
            {
                GmshSurfaceLoopFromMeshes(meshes);

                Tuple<int, int>[] s;
                IguanaGmsh.Model.GetEntities(out s, 2);

                var sl = s.Select(ss => ss.Item2).ToArray();
                var l = IguanaGmsh.Model.Geo.AddSurfaceLoop(sl);

                var v = IguanaGmsh.Model.Geo.AddVolume(new[] { l });

                if (synchronize) IguanaGmsh.Model.Geo.Synchronize();
            }

            public static void GmshSurfaceFromMesh(Mesh mesh, out Tuple<int,int>[] dimTags)
            {
                List<long> nodes, triangles;
                List<double> xyz;
                GetConstructiveDataFromMesh(mesh, out nodes, out triangles, out xyz);
                GmshSurfaceLoopFromDataList(nodes, triangles, xyz);

                IguanaGmsh.Model.GetEntities(out dimTags, 2);
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
                                    tag = IguanaGmsh.Model.Geo.AddPoint(p.X, p.Y, p.Z, data.Size);
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

                    if (synchronize) IguanaGmsh.Model.Geo.Synchronize();

                    if (ptsTags.Count > 0) IguanaGmsh.Model.Mesh.Embed(0, ptsTags.ToArray(), 2, surfaceTag);
                    if (crvTags.Count > 0) IguanaGmsh.Model.Mesh.Embed(1, crvTags.ToArray(), 2, surfaceTag);
                }
            }

            public static void EmbedConstraintsOnBrep(List<IguanaGmshConstraint> constraints, bool synchronize = false)
            {
                int count = constraints.Count;

                if (count > 0)
                {
                    List<int> ptsTags = new List<int>();
                    Dictionary<Int64, List<int>> embedPoints = new Dictionary<Int64, List<int>>();
                    Dictionary<Int64, List<int>> embedCurve = new Dictionary<Int64, List<int>>();
                    Dictionary<Int64, List<int>> embedSurface = new Dictionary<Int64, List<int>>();

                    PointCloud pts = new PointCloud();

                    IguanaGmshConstraint data;
                    Point3d p;
                    Polyline poly;
                    Curve crv;
                    Brep b;
                    double t = 0.0001;
                    int tag, idx;
                    Int64 key;

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
                                    tag = IguanaGmsh.Model.Geo.AddPoint(p.X, p.Y, p.Z, data.Size);
                                    ptsTags.Add(tag);
                                    pts.Add(p);

                                    key = (Int64)data.EntityDim << 32 | (Int64) data.EntityID;
                                    if (!embedPoints.ContainsKey(key)) embedPoints.Add(key, new List<int>());
                                    embedPoints[key].Add(tag);
                                }

                                break;

                            case 1:
                                poly = (Polyline)data.RhinoGeometry;
                                int[] polyTags = LinesFromRhinoPolyline(poly, data.Size, ref ptsTags, pts, t);

                                key = (Int64)data.EntityDim << 32 | (Int64)data.EntityID;
                                if (!embedCurve.ContainsKey(key)) embedPoints.Add(key, new List<int>());
                                embedCurve[key].AddRange(polyTags);
                                break;

                            case 2:
                                crv = (Curve)data.RhinoGeometry;
                                int[] crvTags = SplinesFromRhinoCurve(crv, data.Size);

                                key = (Int64)data.EntityDim << 32 | (Int64)data.EntityID;
                                if (!embedCurve.ContainsKey(key)) embedCurve.Add(key, new List<int>());
                                embedCurve[key].AddRange(crvTags);
                                break;

                            case 3:
                                b = (Brep)data.RhinoGeometry;
                                List<Point3d> patch;
                                Curve[] crvArr;
                                IRhinoGeometry.GetBrepFaceMeshingData(b, 0, 20, out crvArr, out patch);
                                int wireTag = IguanaGmshFactory.Geo.CurveLoopFromRhinoCurve(crvArr[0], data.Size);
                                int surfaceTag = IguanaGmshFactory.Geo.SurfacePatch(wireTag, patch, false);

                                key = (Int64)data.EntityDim << 32 | (Int64)data.EntityID;
                                if (!embedSurface.ContainsKey(key)) embedSurface.Add(key, new List<int>());
                                embedSurface[key].Add(surfaceTag);
                                break;
                        }
                    }

                    if (synchronize) IguanaGmsh.Model.Geo.Synchronize();

                    if (embedPoints.Count > 0)
                    {
                        foreach(Int64 id in embedPoints.Keys) IguanaGmsh.Model.Mesh.Embed(0, embedPoints[id].ToArray(), (Int32)(id >> 32), (Int32)id);
                    }
                    if (embedCurve.Count > 0)
                    {
                        foreach (Int64 id in embedCurve.Keys) IguanaGmsh.Model.Mesh.Embed(1, embedCurve[id].ToArray(), (Int32)(id >> 32), (Int32)id);
                    }
                    if (embedSurface.Count > 0)
                    {
                        foreach (Int64 id in embedSurface.Keys) IguanaGmsh.Model.Mesh.Embed(2, embedSurface[id].ToArray(), (Int32)(id >> 32), (Int32)id);
                    }
                }
            }

            public static int CurveLoopFromRhinoCurve(Curve crv, double size)
            {
                int[] crvTags = SplinesFromRhinoCurve(crv, size);
                int wireTag = IguanaGmsh.Model.Geo.AddCurveLoop(crvTags);
                return wireTag;
            }

            public static int SurfacePatch(int wireTag, List<Point3d> patchs = default, bool synchronize = false)
            {
                // 1._ Check points to patch
                int sphereCenterTag = -1;
                if (patchs != default)
                {
                    double[] sphereCenter = new double[3];
                    for (int i = 0; i < patchs.Count; i++)
                    {
                        sphereCenter[0] += patchs[i].X;
                        sphereCenter[1] += patchs[i].Y;
                        sphereCenter[2] += patchs[i].Z;
                    }
                    sphereCenter[0] /= patchs.Count;
                    sphereCenter[1] /= patchs.Count;
                    sphereCenter[2] /= patchs.Count;

                    sphereCenterTag = IguanaGmsh.Model.Geo.AddPoint(sphereCenter[0], sphereCenter[1], sphereCenter[2]);
                }

                // 3._ Build Geometry
                int surfaceTag = IguanaGmsh.Model.Geo.AddSurfaceFilling(wireTag, sphereCenterTag);

                // 5._ Synchronize model
                if (synchronize) IguanaGmsh.Model.Geo.Synchronize();

                return surfaceTag;
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
                            tag = IguanaGmsh.Model.Geo.AddPoint(p.X, p.Y, p.Z, size);
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
                                tag = IguanaGmsh.Model.Geo.AddPoint(p.X, p.Y, p.Z, size);
                                ptsCloud.Add(p);
                                allTags.Add(tag);
                            }
                            else tag = allTags[pIdx];

                            ptTags[j + 1] = tag;
                        }
                        sIdx++;
                    }

                    crvTags[i] = IguanaGmsh.Model.Geo.AddSpline(ptTags);
                }
                return crvTags;
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
                            tag = IguanaGmsh.Model.Geo.AddPoint(p.X, p.Y, p.Z, size);
                            ptsTags.Add(tag);
                            ptsCloud.Add(p);
                            idx = ptsCloud.Count - 1;
                        }

                        tempTags[j] = idx;
                    }

                    for (int j = 0; j < poly.SegmentCount; j++)
                    {
                        crvTags[j] = IguanaGmsh.Model.Geo.AddLine(ptsTags[tempTags[j]], ptsTags[tempTags[j + 1]]);
                    }
                }
                else
                {

                    for (int j = 0; j < poly.Count; j++)
                    {
                        p = poly[j];
                        tag = IguanaGmsh.Model.Geo.AddPoint(p.X, p.Y, p.Z, size);
                        ptsTags.Add(tag);
                    }

                    for (int j = 0; j < poly.SegmentCount; j++)
                    {
                        crvTags[j] = IguanaGmsh.Model.Geo.AddLine(ptsTags[j], ptsTags[j + 1]);
                    }
                }

                return crvTags;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="triangulate"> Return triangulated data from the given Iguana mesh </param>
            /// <param name="angle"></param>
            /// <param name="curveAngle"></param>
            /// <param name="forceParametrizablePatches"> For complex geometries, patches can be too complex, too elongated or too large to be parametrized; setting the following option will force the creation of patches that are amenable to reparametrization:</param>
            /// <param name="includeBoundary"> For open surfaces include the boundary edges in the classification process: </param>
            public static void GmshFromIguanaMesh(IMesh mesh, int dim, int tag, bool triangulate =false, double angle=0, double curveAngle=180, bool forceParametrizablePatches = true, bool includeBoundary = true)
            {
                int[] elementTypes;
                long[][] elementTags, elementNodes;
                long[] nodeTags;
                double[] position;
                if (!triangulate)
                {
                    GetElementDataFromIguanaMesh(mesh, out elementTypes, out elementTags, out elementNodes);
                    GetNodeDataFromIguanaMesh(mesh, out nodeTags, out position);
                }
                else GetTriangulatedDataFromIguanaMesh(mesh, out nodeTags, out position, out elementTypes, out elementTags, out elementNodes);

                IguanaGmsh.Model.AddDiscreteEntity(dim, tag, new int[] { });
                IguanaGmsh.Model.Mesh.AddNodes(dim, tag, nodeTags, position);

                for (int i = 0; i < elementTypes.Length; i++)
                {
                    IguanaGmsh.Model.Mesh.AddElementsByType(tag, elementTypes[i], elementTags[i], elementNodes[i]);
                }

                IguanaGmsh.Model.Mesh.ClassifySurfaces(angle * Math.PI / 180, includeBoundary, forceParametrizablePatches, curveAngle * Math.PI / 180);
                IguanaGmsh.Model.Mesh.CreateGeometry();

                IguanaGmsh.Model.Geo.Synchronize();
            }
        }
    }
}
