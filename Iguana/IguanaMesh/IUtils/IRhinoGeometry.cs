using System;
using System.Collections.Generic;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.IUtils
{
    public static class IRhinoGeometry
    { 
        public static List<Line> GetEdgesAsLines(IMesh mesh)
        {
            Point3d start, end;
            List<Line> edges = new List<Line>();
            foreach(int[] pair in mesh.Topology.GetUniqueEdges())
            {
                start = mesh.Vertices.GetVertexWithKey(pair[0]).RhinoPoint;
                end = mesh.Vertices.GetVertexWithKey(pair[1]).RhinoPoint;
                edges.Add(new Line(start, end));
            }
            return edges;
        }

        public static List<Polyline> GetFacesAsPolylines(IMesh mesh)
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
            }
            return faces;
        }

        public static List<Surface> GetFacesAsSurfaces(IMesh mesh)
        {
            List<Surface> faces = new List<Surface>();
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

                    PolylineCurve pl = new PolylineCurve(pts);
                    faces.Add(Brep.CreatePatch(new PolylineCurve[]{pl}, null, 0.001).Surfaces[0]);
                }
            }
            return faces;
        }

        public static List<Brep> GetSolidsAsBrep(IMesh mesh)
        {
            List<Brep> solids = new List<Brep>();
            foreach (IElement e in mesh.Elements.ElementsValues)
            {
                if (e.TopologicDimension == 3)
                {
                    Brep b = new Brep();
                    for (int i = 0; i < e.HalfFacetsCount; i++)
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

                        PolylineCurve pl = new PolylineCurve(pts);

                        Surface face = Brep.CreatePatch(new PolylineCurve[] { pl }, null, 0.001).Surfaces[0];
                        b.AddSurface(face);
                    }

                    solids.Add(b);
                }
            }
            return solids;
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
                    for (int i = 0; i < e.HalfFacetsCount; i++)
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
