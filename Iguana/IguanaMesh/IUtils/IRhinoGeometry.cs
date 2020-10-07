using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.IUtils
{
    public static class IRhinoGeometry
    {
        public static Curve GetBrepFaceNakedBoundary(Brep b, int indexFace)
        {
            BrepFace face = b.Faces[indexFace];
            int[] indexEdges = face.AdjacentEdges();
            Curve[] edges = new Curve[indexEdges.Length];
            for (int i = 0; i < indexEdges.Length; i++)
            {
                edges[i] = b.Edges[indexEdges[i]];
            }

            Curve c = Curve.JoinCurves(edges)[0];

            return c;
        }

        public static bool GetBrepFaceMeshingData(Brep b, int indexFace, int count, out Curve nakedBoundary, out List<Point3d> patchingPoints)
        {
            nakedBoundary = null;
            patchingPoints = new List<Point3d>();

            try
            {
                nakedBoundary = GetBrepFaceNakedBoundary(b, indexFace);

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
                    if (nakedBoundary.Contains(pt, pl, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance) == PointContainment.Inside) patchingPoints.Add(pt);
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
                start = mesh.Vertices.GetVertexWithKey(idxA).RhinoPoint;
                end = mesh.Vertices.GetVertexWithKey(idxB).RhinoPoint;
                edges.Add(new Line(start, end));
            }
            return edges;
        }

        public static List<Polyline> GetAllFacesAsPolylines(IMesh mesh)
        {
            List<Polyline> faces = new List<Polyline>();
            foreach (IElement e in mesh.Elements.ElementsValues)
            {
                if (e.TopologicDimension == 2)
                {
                    Point3d[] pts = new Point3d[e.VerticesCount + 1];

                    for (int i = 0; i < e.VerticesCount; i++)
                    {
                        pts[i] = mesh.Vertices.GetVertexWithKey(e.Vertices[i]).RhinoPoint;
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
                            pts[j] = mesh.Vertices.GetVertexWithKey(hf[j]).RhinoPoint;
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
            foreach (IElement e in mesh.Elements.ElementsValues)
            {
                if (e.TopologicDimension == 2)
                {
                    Point3d[] pts = new Point3d[e.VerticesCount];
                    for (int i = 0; i < e.VerticesCount; i++)
                    {
                        pts[i] = mesh.Vertices.GetVertexWithKey(e.Vertices[i]).RhinoPoint;
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


            foreach(IElement e in mesh.Elements.ElementsValues)
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
                            pts[j] = mesh.Vertices.GetVertexWithKey(hf[j]).RhinoPoint;
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
            foreach (ITopologicVertex v in mesh.Vertices.VerticesValues)
            {
                pts.Add(v.RhinoPoint);
            }
            return pts;
        }

        public static Dictionary<int, List<PolylineCurve>> GetSolidsAsPoly(IMesh mesh)
        {
            Dictionary<int, List<PolylineCurve>> crv = new Dictionary<int, List<PolylineCurve>>();
            foreach (int eK in mesh.Elements.ElementsKeys)
            {
                IElement e = mesh.Elements.GetElementWithKey(eK);

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
                            pts[j] = mesh.Vertices.GetVertexWithKey(hf[j]).RhinoPoint;
                        }

                        pts[hf.Length] = pts[0];

                        crv[eK].Add(new PolylineCurve(pts));
                    }
                }
            }
            return crv;
        }
    }
}
