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

using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

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

        public static List<PolylineCurve> Get1DElementsAsLines(IMesh mesh)
        {
            List<PolylineCurve> lines = new List<PolylineCurve>();
            PolylineCurve c;
            foreach (IElement e in mesh.Elements)
            {
                if (e.TopologicDimension == 1)
                {
                    Point3d[] pts = new Point3d[e.VerticesCount];
                    for (int i = 0; i < e.VerticesCount; i++)
                    {
                        pts[i] = mesh.GetVertexWithKey(e.Vertices[i]).RhinoPoint;
                    }
                    c = new PolylineCurve(pts);
                    lines.Add(c);
                }
            }
            return lines;
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

        public static GeometryBase GetGeometryFromElement(IMesh mesh, int eKey)
        {
            GeometryBase geom = null;

            if (mesh.ContainsElementKey(eKey))
            {
                IElement e = mesh.GetElementWithKey(eKey);

                if (e.TopologicDimension == 1)
                {
                    Point3d p1 = mesh.GetVertexWithKey(e.Vertices[0]).RhinoPoint;
                    Point3d p2 = mesh.GetVertexWithKey(e.Vertices[1]).RhinoPoint;
                    geom = new LineCurve(p1, p2);
                }
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
                    geom = f.ToBrep();
                }
                else if (e.TopologicDimension == 3)
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

                    geom = Brep.JoinBreps(faces, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)[0];
                }
            }

            return geom;
        }

        public static Mesh TryGetRhinoMesh(IMesh iM, bool trianglesOnly=false)
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

            int vKey = iM.FindNextVertexKey();
            int[] hf;
            foreach (IElement e in iM.Elements)
            {
                if (e.TopologicDimension == 1)
                {
                    rM.Faces.AddFace(new MeshFace(maps[e.Vertices[0]], maps[e.Vertices[1]], maps[e.Vertices[0]]));
                }
                else if (e.TopologicDimension == 2)
                {
                    if (e.VerticesCount == 3) rM.Faces.AddFace(new MeshFace(maps[e.Vertices[0]], maps[e.Vertices[1]], maps[e.Vertices[2]]));
                    else if (e.VerticesCount == 4)
                    {
                        if (trianglesOnly)
                        {
                            rM.Faces.AddFace(new MeshFace(maps[e.Vertices[0]], maps[e.Vertices[1]], maps[e.Vertices[3]]));
                            rM.Faces.AddFace(new MeshFace(maps[e.Vertices[3]], maps[e.Vertices[1]], maps[e.Vertices[2]]));
                        }
                        else
                        {
                            rM.Faces.AddFace(new MeshFace(maps[e.Vertices[0]], maps[e.Vertices[1]], maps[e.Vertices[2]], maps[e.Vertices[3]]));
                        }
                    }
                    else
                    {
                        IPoint3D pos = ISubdividor.ComputeAveragePosition(e.Vertices, iM);
                        rM.Vertices.Add(new Point3d(pos.X, pos.Y, pos.Z));
                        maps.Add(vKey, idx);
                        for (int i = 1; i <= e.HalfFacetsCount; i++)
                        {
                            e.GetHalfFacet(i, out hf);
                            rM.Faces.AddFace(new MeshFace(maps[hf[0]], maps[hf[1]], maps[vKey]));
                        }
                        vKey++;
                        idx++;
                    }
                }
                else if (e.TopologicDimension == 3)
                {
                    if (!iM.IsMultidimensionalMesh)
                    {
                        for (int i = 1; i <= e.HalfFacetsCount; i++)
                        {
                            if (e.IsNakedSiblingHalfFacet(i))
                            {
                                e.GetHalfFacet(i, out hf);
                                if (hf.Length == 3) rM.Faces.AddFace(new MeshFace(maps[hf[0]], maps[hf[1]], maps[hf[2]]));
                                else rM.Faces.AddFace(new MeshFace(maps[hf[0]], maps[hf[1]], maps[hf[2]], maps[hf[3]]));
                            }
                        }
                    }
                    else
                    {
                        if (e.IsBoundaryElement())
                        {
                            for (int i = 1; i <= e.HalfFacetsCount; i++)
                            {
                                e.GetHalfFacet(i, out hf);
                                if (hf.Length == 3) rM.Faces.AddFace(new MeshFace(maps[hf[0]], maps[hf[1]], maps[hf[2]]));
                                else rM.Faces.AddFace(new MeshFace(maps[hf[0]], maps[hf[1]], maps[hf[2]], maps[hf[3]]));
                            }
                        }
                    }
                }
            }

            rM.UnifyNormals();
            return rM;
        }

        public static Point3d[] GetPointsFromElements(int[] vKeys, IMesh iM)
        {
            Point3d[] pts = new Point3d[vKeys.Length+1]; 
            for(int i=0; i < vKeys.Length; i++)
            {
                pts[i] = iM.GetVertexWithKey(vKeys[i]).RhinoPoint;
            }
            pts[vKeys.Length] = pts[0];
            return pts;
        }
    }
}
