using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IModifiers;
using Iguana.IguanaMesh.ITypes;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using Rhino.PlugIns;

namespace Iguana.IguanaMesh.IUtils
{
    public static class IRhinoGeometry
    {
        public enum DrawIDs { ShowEntities=0, Entities_0D = 1, Entities_1D = 2, Entities_2D=3, Entities_3D = 4, HideEntities =5 };

        public static void DrawElementsID(IGH_PreviewArgs args, double[][] entitiesID, DrawIDs drawType)
        {
            if (entitiesID != null)
            {
                foreach (double[] data in entitiesID)
                {
                    Color color = Color.Aqua;
                    if (data[3] == 1) color = Color.Gold;
                    else if (data[3] == 2) color = Color.DarkGreen;
                    else if (data[3] == 3) color = Color.Navy;

                    Point3d world = new Point3d(data[0], data[1], data[2]);
                    Transform xform = args.Viewport.GetTransform(CoordinateSystem.World, CoordinateSystem.Screen);
                    world.Transform(xform);
                    Point2d screen = new Point2d(world);

                    switch (drawType)
                    {
                        case DrawIDs.ShowEntities:
                            args.Display.Draw2dText(data[4].ToString(), color, screen, false);
                            break;
                        case DrawIDs.Entities_0D:
                            if(data[3] == 0) args.Display.Draw2dText(data[4].ToString(), color, screen, false);
                            break;
                        case DrawIDs.Entities_1D:
                            if (data[3] == 1) args.Display.Draw2dText(data[4].ToString(), color, screen, false);
                            break;
                        case DrawIDs.Entities_2D:
                            if (data[3] == 2) args.Display.Draw2dText(data[4].ToString(), color, screen, false);
                            break;
                        case DrawIDs.Entities_3D:
                            if (data[3] == 3) args.Display.Draw2dText(data[4].ToString(), color, screen, false);
                            break;
                    }
                    
                }
            }
        }

        public static void GetBrepFaceNakedBoundaries(Brep b, int index, out Curve[] crv)
        {
            BrepLoopList loops = b.Faces[index].Loops;
            crv = new Curve[loops.Count];
            for (int i = 0; i < loops.Count; i++)
            {
                crv[i] = loops[i].To3dCurve();
            }
        }

        public static void GetBrepFaceOuterNakedBoundary(Brep b, int index, out Curve crv)
        {
            BrepLoopList loops = b.Faces[index].Loops;
            crv = loops[0].To3dCurve();
        }

        public static bool GetBrepFaceMeshingData(Brep b, int indexFace, int count, out Curve[] nakedBoundary, out List<Point3d> patchingPoints)
        {
            nakedBoundary = null;
            patchingPoints = new List<Point3d>();

            try
            {
                GetBrepFaceNakedBoundaries(b, indexFace, out nakedBoundary);

                // Surface points
                Point3d p;
                List<Point3d> pts = new List<Point3d>();
                Interval UU = b.Surfaces[indexFace].Domain(0);
                Interval VV = b.Surfaces[indexFace].Domain(1);
                double u = Math.Abs(UU.Length) / count;
                double v = Math.Abs(VV.Length) / count;
                for (int i = 0; i <= count; i++)
                {
                    for (int j = 0; j <= count; j++)
                    {
                        p = b.Surfaces[0].PointAt(i * u, j * v);
                        pts.Add(p);
                    }
                }

                // Points to patch
                Plane pl;
                Plane.FitPlaneToPoints(pts, out pl);
                patchingPoints = new List<Point3d>();
                foreach (Point3d pt in pts)
                {
                    if (nakedBoundary[0].Contains(pt, pl, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance) == PointContainment.Inside) patchingPoints.Add(pt);
                }
                return true;
            }
            catch (Exception) { return false; }
        }

        public static List<Line> GetEdgesAsLines(IMesh mesh)
        {
            Point3d start, end;
            List<Line> edges = new List<Line>();
            int idxA, idxB;
            foreach (Int64 pair in mesh.Topology.GetUniqueEdges())
            {
                idxA = (Int32)(pair >> 32);
                idxB = (Int32) pair;
                start = mesh.GetVertexWithKey(idxA).RhinoPoint;
                end = mesh.GetVertexWithKey(idxB).RhinoPoint;
                edges.Add(new Line(start, end));
            }
            return edges;
        }

        public static List<Polyline> GetAllFacesAsPolylines(IMesh mesh)
        {
            List<Polyline> faces = new List<Polyline>();
            foreach (IElement e in mesh.Elements)
            {
                if (e.TopologicDimension == 2)
                {
                    Point3d[] pts = new Point3d[e.VerticesCount + 1];

                    for (int i = 0; i < e.VerticesCount; i++)
                    {
                        pts[i] = mesh.GetVertexWithKey(e.Vertices[i]).RhinoPoint;
                    }
                    pts[e.VerticesCount] = pts[0];

                    Polyline pl = new Polyline(pts);
                    faces.Add(pl);
                }
                else
                {
                    for (int i = 1; i <= e.HalfFacetsCount; i++)
                    {
                        int[] hf;
                        e.GetHalfFacetWithPrincipalNodesOnly(i, out hf);

                        Point3d[] pts = new Point3d[hf.Length+1];

                        for (int j = 0; j < hf.Length; j++)
                        {
                            pts[j] = mesh.GetVertexWithKey(hf[j]).RhinoPoint;
                        }
                        pts[pts.Length-1] = pts[0];

                        Polyline pl = new Polyline(pts);
                        faces.Add(pl);
                    }
                }
            }
            return faces;
        }

        public static List<Surface> Get2DElementsAsSurfaces(IMesh mesh)
        {
            List<Surface> faces = new List<Surface>();
            Surface f;
            foreach (IElement e in mesh.Elements)
            {
                if (e.TopologicDimension == 2)
                {
                    Point3d[] pts = new Point3d[e.VerticesCount];
                    for (int i = 0; i < e.VerticesCount; i++)
                    {
                        pts[i] = mesh.GetVertexWithKey(e.Vertices[i]).RhinoPoint;
                    }

                    if (pts.Length == 4) f = NurbsSurface.CreateFromCorners(pts[0], pts[1], pts[2], pts[3]);
                    else if (pts.Length == 3) f = NurbsSurface.CreateFromCorners(pts[0], pts[1], pts[2]);
                    else
                    {
                        f = Brep.CreateEdgeSurface(new PolylineCurve[] { new PolylineCurve(pts), new PolylineCurve(new Point3d[] { pts[pts.Length - 1], pts[0] } ) } ).Surfaces[0];
                    }


                    faces.Add(f);
                }
            }
            return faces;
        }

        public static List<Brep> Get3DElementsAsBrep(IMesh mesh)
        {
            List<Brep> solids = new List<Brep>();
            Brep b;
            Brep[] faces;


            foreach(IElement e in mesh.Elements)
            {
                if (e.TopologicDimension == 3)
                {
                    b = new Brep();
                    faces = new Brep[e.HalfFacetsCount];

                    for (int i = 1; i <= e.HalfFacetsCount; i++)
                    {
                        int[] hf;
                        e.GetHalfFacetWithPrincipalNodesOnly(i, out hf);

                        Point3d[] pts = new Point3d[hf.Length];

                        for (int j = 0; j < hf.Length; j++)
                        {
                            pts[j] = mesh.GetVertexWithKey(hf[j]).RhinoPoint;
                        }

                        if (pts.Length == 4) faces[i - 1] = Brep.CreateFromCornerPoints(pts[0], pts[1], pts[2], pts[3], RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
                        else faces[i - 1] = Brep.CreateFromCornerPoints(pts[0], pts[1], pts[2], RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
                    }

                    b = Brep.JoinBreps(faces, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)[0];

                    solids.Add(b);
                }
            }
            return solids;
        }

        public static IEnumerable<Point3d> GetVerticesAsPoints(IMesh mesh)
        {
            List<Point3d> pts = new List<Point3d>();
            foreach (ITopologicVertex v in mesh.Vertices)
            {
                pts.Add(v.RhinoPoint);
            }
            return pts;
        }

        public static Dictionary<int, List<PolylineCurve>> GetSolidsAsPoly(IMesh mesh)
        {
            Dictionary<int, List<PolylineCurve>> crv = new Dictionary<int, List<PolylineCurve>>();
            foreach (int eK in mesh.ElementsKeys)
            {
                IElement e = mesh.GetElementWithKey(eK);

                if (e.TopologicDimension == 3)
                {
                    crv.Add(eK, new List<PolylineCurve>());
                    for (int i = 1; i <= e.HalfFacetsCount; i++)
                    {
                        int[] hf;
                        e.GetHalfFacet(i, out hf);

                        //Vertices
                        Point3d[] pts = new Point3d[hf.Length + 1];
                        for (int j = 0; j < hf.Length; j++)
                        {
                            pts[j] = mesh.GetVertexWithKey(hf[j]).RhinoPoint;
                        }

                        pts[hf.Length] = pts[0];

                        crv[eK].Add(new PolylineCurve(pts));
                    }
                }
            }
            return crv;
        }

        public static Polyline[] GetPolylinesFromElement(IMesh mesh, int eKey)
        {
            List<Polyline> faces = new List<Polyline>();
            IElement e = mesh.GetElementWithKey(eKey);
            if (e.TopologicDimension == 2)
            {
                Point3d[] pts = new Point3d[e.VerticesCount + 1];

                for (int i = 0; i < e.VerticesCount; i++)
                {
                    pts[i] = mesh.GetVertexWithKey(e.Vertices[i]).RhinoPoint;
                }
                pts[e.VerticesCount] = pts[0];

                Polyline pl = new Polyline(pts);
                faces.Add(pl);
            }
            else
            {
                for (int i = 1; i <= e.HalfFacetsCount; i++)
                {
                    int[] hf;
                    e.GetHalfFacetWithPrincipalNodesOnly(i, out hf);

                    Point3d[] pts = new Point3d[hf.Length + 1];

                    for (int j = 0; j < hf.Length; j++)
                    {
                        pts[j] = mesh.GetVertexWithKey(hf[j]).RhinoPoint;
                    }
                    pts[pts.Length - 1] = pts[0];

                    Polyline pl = new Polyline(pts);
                    faces.Add(pl);
                }
            }
            return faces.ToArray();
        }

        public static Brep GetBrepFromElement(IMesh mesh, int eKey)
        {
            Brep brep = new Brep();
            IElement e = mesh.GetElementWithKey(eKey);
            if (e.TopologicDimension == 2)
            {
                Surface f;
                Point3d[] pts = new Point3d[e.VerticesCount];
                for (int i = 0; i < e.VerticesCount; i++)
                {
                    pts[i] = mesh.GetVertexWithKey(e.Vertices[i]).RhinoPoint;
                }

                if (pts.Length == 4) f = NurbsSurface.CreateFromCorners(pts[0], pts[1], pts[2], pts[3]);
                else if (pts.Length == 3) f = NurbsSurface.CreateFromCorners(pts[0], pts[1], pts[2]);
                else
                {
                    f = Brep.CreateEdgeSurface(new PolylineCurve[] { new PolylineCurve(pts), new PolylineCurve(new Point3d[] { pts[pts.Length - 1], pts[0] }) }).Surfaces[0];
                }
                brep = f.ToBrep();
            }
            else
            {
                Brep[] faces = new Brep[e.HalfFacetsCount];

                for (int i = 1; i <= e.HalfFacetsCount; i++)
                {
                    int[] hf;
                    e.GetHalfFacetWithPrincipalNodesOnly(i, out hf);

                    Point3d[] pts = new Point3d[hf.Length];

                    for (int j = 0; j < hf.Length; j++)
                    {
                        pts[j] = mesh.GetVertexWithKey(hf[j]).RhinoPoint;
                    }

                    if (pts.Length == 4) faces[i - 1] = Brep.CreateFromCornerPoints(pts[0], pts[1], pts[2], pts[3], RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
                    else faces[i - 1] = Brep.CreateFromCornerPoints(pts[0], pts[1], pts[2], RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
                }

                brep = Brep.JoinBreps(faces, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)[0];

            }

            return brep;
        }

        public static Mesh TryGetRhinoMesh(IMesh iM)
        {
            Mesh rM = new Mesh();

            Dictionary<int,int> maps = new Dictionary<int, int>();
            int idx = 0;
            foreach(ITopologicVertex v in iM.Vertices)
            {
                rM.Vertices.Add(v.RhinoPoint);
                maps.Add(v.Key, idx);
                idx++;
            }

            int vKey;
            int[] hf;
            foreach (IElement e in iM.Elements)
            {
                if (e.TopologicDimension == 2)
                {
                    if (e.VerticesCount == 3) rM.Faces.AddFace(new MeshFace(maps[e.Vertices[0]], maps[e.Vertices[1]], maps[e.Vertices[2]]));
                    else if (e.VerticesCount == 4) rM.Faces.AddFace(new MeshFace(maps[e.Vertices[0]], maps[e.Vertices[1]], maps[e.Vertices[2]], maps[e.Vertices[3]]));
                    else
                    {
                        IPoint3D pos = ISubdividor.ComputeAveragePosition(e.Vertices, iM);
                        rM.Vertices.Add(new Point3d( pos.X, pos.Y, pos.Z ));
                        vKey = rM.Vertices.Count-1;
                        maps.Add(vKey, idx);
                        idx++;
                        for (int i = 1; i <= e.HalfFacetsCount; i++)
                        {
                            e.GetHalfFacet(i, out hf);
                            rM.Faces.AddFace(new MeshFace(maps[hf[0]], maps[hf[1]], vKey));
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= e.HalfFacetsCount; i++)
                    {
                        if (e.IsNakedSiblingHalfFacet(i))
                        {
                            e.GetHalfFacet(i, out hf);
                            if(hf.Length==3) rM.Faces.AddFace(new MeshFace(maps[hf[0]], maps[hf[1]], maps[hf[2]]));
                            else rM.Faces.AddFace(new MeshFace(maps[hf[0]], maps[hf[1]], maps[hf[2]], maps[hf[3]]));
                        }
                    }
                }
            }
            //rM.Vertices.CullUnused();
            return rM;
        }
    }
}
