using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IModifiers
{
    public static partial class IModifier
    {
        public static IMesh Triangulate2DElements(IMesh mesh)
        {
            IMesh triangulated = new IMesh();
            foreach(ITopologicVertex v in mesh.Vertices.VerticesValues)
            {
                triangulated.Vertices.AddVertex(v.Key, new ITopologicVertex(v));
            }

            ISurfaceElement face;
            int key = mesh.Vertices.FindNextKey();
            int elementKey = triangulated.Vertices.FindNextKey();
            foreach (IElement e in mesh.Elements.ElementsValues)
            {
                if (e.TopologicDimension == 2)
                {
                    if (e.VerticesCount == 3)
                    {
                        face = new ISurfaceElement(e.Vertices[0], e.Vertices[1], e.Vertices[2]);
                        triangulated.Elements.AddElement(elementKey, face);
                        elementKey++;
                    }
                    else if (e.VerticesCount == 4)
                    {
                        face = new ISurfaceElement(e.Vertices[0], e.Vertices[1], e.Vertices[3]);
                        triangulated.Elements.AddElement(elementKey, face);
                        elementKey++;
                        face = new ISurfaceElement(e.Vertices[3], e.Vertices[1], e.Vertices[2]);
                        triangulated.Elements.AddElement(elementKey, face);
                        elementKey++;
                    }
                    else
                    {
                        IVector3D pos = ISubdividor.ComputeAveragePosition(e.Vertices, mesh);
                        ITopologicVertex v = new ITopologicVertex(pos.X,pos.Y,pos.Z,key);
                        triangulated.Vertices.AddVertex(key,v);
                        for(int i=1; i<=e.HalfFacetsCount; i++)
                        {
                            int[] hf;
                            e.GetHalfFacet(i, out hf);
                            face = new ISurfaceElement(hf[0],hf[1],key);
                            triangulated.Elements.AddElement(elementKey, face);
                            elementKey++;
                        }
                        key++;
                    }
                }
            }

            triangulated.BuildTopology();

            return triangulated;
        }

        public static IMesh DualMesh(IMesh mesh)
        {
            IMesh nM = new IMesh();
            foreach(IElement e in mesh.Elements.ElementsValues)
            {
                nM.Vertices.AddVertex(e.Key, new ITopologicVertex(ISubdividor.ComputeAveragePosition(e.Vertices, mesh)));
            }

            int[] data1, data2;
            PointCloud cloud = new PointCloud();
            List<int> global = new List<int>();
            int key = nM.Vertices.FindNextKey();
            int elementKey = nM.Elements.FindNextKey();
            foreach (ITopologicVertex v in mesh.Vertices.VerticesValues)
            {
                data1 = mesh.Topology.GetVertexIncidentElements(v.Key);
                if (!mesh.Topology.IsNakedVertex(v.Key))
                {
                    nM.Elements.AddElement(elementKey, new ISurfaceElement(data1));
                    elementKey++;
                }
                else
                {
                    List<int> local = new List<int>();

                    data2 = mesh.Topology.GetVertexAdjacentVertices(v.Key);
                    bool flag = false;

                    foreach (int vv in data2)
                    {
                        if (mesh.Topology.IsNakedEdge(vv, v.Key))
                        {
                            Point3d p = ISubdividor.ComputeAveragePoint(new[] { vv, v.Key }, mesh);
                            int idx = cloud.ClosestPoint(p);

                            if (idx == -1) flag = true;
                            else
                            {
                                if (p.DistanceTo(cloud[idx].Location) > 0.01) flag = true;
                                else flag = false;
                            }

                            if (flag)
                            {
                                cloud.Add(p);
                                nM.Vertices.AddVertex(key, new ITopologicVertex(p));
                                global.Add(key);
                                local.Add(key);
                                key++;
                            }
                            else local.Add(global[idx]);
                        }
                    }

                    nM.Vertices.AddVertex(key, new ITopologicVertex(v));
                    local.Insert(1, key);
                    local.AddRange(data1.Reverse());
                    key++;

                    nM.Elements.AddElement(elementKey, new ISurfaceElement(local.ToArray()));
                    elementKey++;
                }
            }

            nM.BuildTopology();

            return nM;
        }
    }
}
