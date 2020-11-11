using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using System.Xml.Serialization;
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.IModifiers;

namespace Iguana.IguanaMesh.IWrappers.IExtensions
{
    public static partial class IguanaGmshFactory
    {
        public static void ApplyTransfiniteSettings(List<IguanaGmshTransfinite> transfinite)
        {
            foreach(IguanaGmshTransfinite t in transfinite)
            {
                switch (t.Dim)
                {
                    case 1:
                        IguanaGmsh.Model.Mesh.SetTransfiniteCurve(t.Tag, t.NodesNumber, t.MethodType, t.Coef);
                        break;
                    case 2:
                        IguanaGmsh.Model.Mesh.SetTransfiniteSurface(t.Tag, t.MethodType, t.Corners);
                        break;
                    case 3:
                        IguanaGmsh.Model.Mesh.SetTransfiniteVolume(t.Tag, t.Corners);
                        break;
                }
            }
        }

        public static void TryGetEntitiesID(out double[][] entititesID, int dim=-1)
        {
            Tuple<int, int>[] dimTags;
            IguanaGmsh.Model.GetEntities(out dimTags, dim);

            entititesID = new double[dimTags.Length][];
            int tag;
            for (int i = 0; i < dimTags.Length; i++)
            {
                dim = dimTags[i].Item1;
                tag = dimTags[i].Item2;

                double[] coord;
                IguanaGmsh.Model.Mesh.GetCenter(dim, tag, out coord);

                /*switch (dim)
                {
                    case 0:
                        IguanaGmsh.Model.GetValue(dim, tag, new double[] {}, out coord);
                        break;
                    case 1:
                        IguanaGmsh.Model.GetValue(dim, tag, new double[] { 0.5 }, out coord);
                        break;
                    default:
                        double xmin, ymin, zmin, xmax, ymax, zmax;
                        IguanaGmsh.Model.GetBoundingBox(dim, tag, out xmin, out ymin, out zmin, out xmax, out ymax, out zmax);
                        coord = new double[] { (xmin+xmax)/2, (ymin+ymax)/2, (zmin+zmax)/2};
                        break;
                }*/

                entititesID[i] = new double[] { coord[0], coord[1], coord[2], dim, tag };
            }
        }

        public static IMesh TryGetIMesh(int dim = 2)
        {
            if (dim > 3) dim = 3;
            else if (dim < 2) dim = 2;

            IMesh mesh = new IMesh();
            HashSet<int> parsedNodes;
            IguanaGmsh.Model.Mesh.TryParseITopologicVertices(ref mesh);
            IguanaGmsh.Model.Mesh.TryParseIElement(ref mesh, out parsedNodes, dim);
            if (parsedNodes.Count < mesh.VerticesCount) mesh.CullUnparsedNodes(parsedNodes);
            mesh.BuildTopology(true);

            return mesh;
        }

        public static void GetConstructiveDataFromMesh(Mesh mesh, out List<long> nodes, out List<long> triangles, out List<double> xyz)
        {
            GetConstructiveDataFromMeshes(new Mesh[] { mesh }, out nodes, out triangles, out xyz);
        }

        public static void GetConstructiveDataFromMeshes(Mesh[] meshes, out List<long> nodes, out List<long> triangles, out List<double> xyz)
        {
            nodes = new List<long>();
            triangles = new List<long>();
            xyz = new List<double>();

            int count = meshes.Length;
            Mesh m;
            Point3d p;
            long key = 1;

            PointCloud cloud = new PointCloud();
            int idx;

            for (int i = 0; i < count; i++)
            {

                m = meshes[i];
                m.Faces.ConvertQuadsToTriangles();

                long[] temp = new long[m.Vertices.Count];

                for (int j = 0; j < m.Vertices.Count; j++)
                {

                    p = m.Vertices[j];
                    idx = cloud.ClosestPoint(p);

                    bool flag = false;

                    if (idx == -1) flag = true;
                    else if (cloud[idx].Location.DistanceTo(p) > RhinoDoc.ActiveDoc.ModelAbsoluteTolerance) flag = true;

                    if (flag)
                    {
                        nodes.Add(key);
                        xyz.AddRange(new double[] { p.X, p.Y, p.Z });
                        cloud.Add(p);
                        idx = nodes.Count - 1;
                        key++;
                    }
                    temp[j] = nodes[idx];
                }

                for (int j = 0; j < m.Faces.Count; j++)
                {
                    MeshFace f = m.Faces[j];
                    triangles.AddRange(new long[] { temp[f.A], temp[f.B], temp[f.C] });
                }
            }
        }

        public static void GetConstructiveDataFromIguanaSurfaceMesh(IMesh mesh, out List<long> nodes, out List<long> triangles, out List<double> xyz)
        {
            GetConstructiveDataFromIguanaSurfaceMeshes(new IMesh[] { mesh }, out nodes, out triangles, out xyz);
        }

        public static void GetConstructiveDataFromIguanaSurfaceMeshes(IMesh[] meshes, out List<long> nodes, out List<long> triangles, out List<double> xyz)
        {
            nodes = new List<long>();
            triangles = new List<long>();
            xyz = new List<double>();

            int count = meshes.Length;
            IMesh m;
            Point3d p;
            long key = 1;

            PointCloud cloud = new PointCloud();
            int idx;

            for (int i = 0; i < count; i++)
            {
                if (meshes[i].IsSurfaceMesh)
                {
                    m = IModifier.Triangulate2DElements(meshes[i]);

                    Dictionary<long,long> temp = new Dictionary<long, long>();

                    foreach(ITopologicVertex v in m.Vertices)
                    {
                        p = v.RhinoPoint;
                        key = v.Key + 1;
                        idx = cloud.ClosestPoint(p);

                        bool flag = false;

                        if (idx == -1) flag = true;
                        else if (cloud[idx].Location.DistanceTo(p) > RhinoDoc.ActiveDoc.ModelAbsoluteTolerance) flag = true;

                        if (flag)
                        {
                            nodes.Add(key);
                            xyz.AddRange(new double[] { p.X, p.Y, p.Z });
                            cloud.Add(p);
                            idx = nodes.Count - 1;
                        }
                        temp.Add(v.Key,nodes[idx]);
                    }

                    foreach(IElement e in m.Elements)
                    {
                        triangles.AddRange(new long[] { temp[e.Vertices[0]], temp[e.Vertices[1]], temp[e.Vertices[2]] });
                    }
                }
            }
        }

        public static void GetConstructiveDataFromBrep(Brep b, out List<long> nodes, out List<long> triangles, out List<double> xyz)
        {
            Mesh[] meshes = Mesh.CreateFromBrep(b, MeshingParameters.Default);
            GetConstructiveDataFromMeshes(meshes, out nodes, out triangles, out xyz);
        }


        public static int EvaluatePoint(PointCloud pts, Point3d p, double t)
        {
            int idx = pts.ClosestPoint(p);
            if (idx != -1 && p.DistanceTo(pts[idx].Location) > t) idx = -1;
            return idx;
        }


        public static void GetElementDataFromIguanaMesh(IMesh mesh, out int[] elementTypes, out long[][] elementTags, out long[][] elementNodes)
        {
            Dictionary<int, List<long>> eTags = new Dictionary<int, List<long>>();
            Dictionary<int, List<long>> eNodes = new Dictionary<int, List<long>>();
            foreach (IElement e in mesh.Elements)
            {

                if (!eTags.ContainsKey(e.ElementType))
                {
                    eTags.Add(e.ElementType, new List<long>());
                    eNodes.Add(e.ElementType, new List<long>());
                }

                eTags[e.ElementType].Add(e.Key);
                int[] vertices = e.GetGmshFormattedVertices();
                foreach (int vk in vertices) eNodes[e.ElementType].Add((long)vk);
            }

            elementTypes = eTags.Keys.ToArray();
            elementTags = new long[eTags.Count][];
            elementNodes = new long[eTags.Count][];
            for (int i = 0; i < elementTypes.Length; i++)
            {
                int eType = elementTypes[i];
                elementTags[i] = eTags[eType].ToArray();
                elementNodes[i] = eNodes[eType].ToArray();
            }
        }

        public static void GetNodeDataFromIguanaMesh(IMesh mesh, out long[] nodeTags, out double[] position)
        {

            int count = mesh.Vertices.Count;
            nodeTags = new long[count];
            position = new double[count * 3];
            int i = 0;
            foreach (ITopologicVertex v in mesh.Vertices)
            {
                nodeTags[i] = v.Key;
                position[i * 3] = v.X;
                position[i * 3 + 1] = v.Y;
                position[i * 3 + 2] = v.Z;
                i++;
            }
        }

        public static void GetTriangulatedDataFromIguanaMesh(IMesh mesh, out long[] nodeTags, out double[] position, out int[] elementTypes, out long[][] elementTags, out long[][] elementNodes)
        {
            List<long> vertexKeys = new List<long>();
            List<double> vertexPos = new List<double>();
            foreach (ITopologicVertex v in mesh.Vertices)
            {
                vertexKeys.Add(v.Key);
                vertexPos.AddRange(new double[] {v.X,v.Y,v.Z });
            }

            // Elements (only triangular surface elements)
            List<long> eTags = new List<long>();
            List<long> eNodes = new List<long>();
            int vkey = mesh.FindNextVertexKey();
            int eKey = 1;
            foreach (IElement e in mesh.Elements)
            {
                if (e.TopologicDimension == 2)
                {
                    if (e.VerticesCount == 3)
                    {
                        eNodes.AddRange(new long[] { (long)e.Vertices[0], (long)e.Vertices[1], (long)e.Vertices[2] });
                        eTags.Add(eKey);
                        eKey++;
                    }
                    else if (e.VerticesCount == 4)
                    {
                        eNodes.AddRange(new long[] { (long)e.Vertices[0], (long)e.Vertices[1], (long)e.Vertices[3] });
                        eTags.Add(eKey);
                        eKey++;
                        eNodes.AddRange(new long[] { (long)e.Vertices[3], (long)e.Vertices[1], (long)e.Vertices[2] });
                        eTags.Add(eKey);
                        eKey++;
                    }
                    else
                    {
                        IPoint3D pos = ISubdividor.ComputeAveragePosition(e.Vertices, mesh);
                        vertexPos.AddRange(new double[] { pos.X, pos.Y, pos.Z });
                        vertexKeys.Add(vkey);
                        for (int i = 1; i <= e.HalfFacetsCount; i++)
                        {
                            int[] hf;
                            e.GetHalfFacet(i, out hf);
                            eNodes.AddRange(new long[] { hf[0], hf[1], vkey });
                            eTags.Add(eKey);
                            eKey++;
                        }
                        vkey++;
                    }
                }
            }

            //To arrays
            nodeTags = vertexKeys.ToArray();
            position = vertexPos.ToArray();
            elementTypes = new int[] { 2 };
            elementTags = new long[1][] { eTags.ToArray() };
            elementNodes = new long[1][] { eNodes.ToArray() };
        }
    }
}
