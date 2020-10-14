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
using Rhino;

namespace Iguana.IguanaMesh.IWrappers.IExtensions
{
    public static partial class IguanaGmshFactory
    {
        public static IMesh TryGetIMesh(int dim=2)
        {
            if (dim > 3) dim = 3;
            else if (dim < 2) dim = 2;

            // Iguana mesh construction
            IVertexCollection vertices;
            IElementCollection elements;
            HashSet<int> parsedNodes;
            IguanaGmsh.Model.Mesh.TryGetIVertexCollection(out vertices);
            IguanaGmsh.Model.Mesh.TryGetIElementCollection(out elements, out parsedNodes, dim);
            if (parsedNodes.Count < vertices.Count) vertices.CullUnparsedNodes(parsedNodes);

            // Iguana mesh construction
            IMesh mesh = new IMesh(vertices, elements);
            mesh.BuildTopology();

            return mesh;
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

        public static void GetConstructiveDataFromBrep(Brep b, out List<long> nodes, out List<long> triangles, out List<double> xyz)
        {
            Mesh[] meshes = Mesh.CreateFromBrep(b, MeshingParameters.Default);
            GetConstructiveDataFromMeshes(meshes, out nodes, out triangles, out xyz);
        }


            internal static int EvaluatePoint(PointCloud pts, Point3d p, double t)
        {
            int idx = pts.ClosestPoint(p);
            if (idx != -1 && p.DistanceTo(pts[idx].Location) > t) idx = -1;
            return idx;
        }
    }
}
