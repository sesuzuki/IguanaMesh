using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IModifiers
{
    public static partial class IModifier
    {
        public static IMesh Triangulate2DElements(IMesh mesh)
        {
            IMesh triangulated = new IMesh();
            foreach (ITopologicVertex v in mesh.Vertices)
            {
                triangulated.AddVertex(v.Key, new ITopologicVertex(v));
            }

            ISurfaceElement face;
            int key = mesh.FindNextVertexKey();
            foreach (IElement e in mesh.Elements)
            {
                if (e.TopologicDimension == 2)
                {
                    if (e.VerticesCount == 3)
                    {
                        face = new ISurfaceElement(e.Vertices[0], e.Vertices[1], e.Vertices[2]);
                        triangulated.AddElement(face);
                    }
                    else if (e.VerticesCount == 4)
                    {
                        face = new ISurfaceElement(e.Vertices[0], e.Vertices[1], e.Vertices[3]);
                        triangulated.AddElement(face);
                        face = new ISurfaceElement(e.Vertices[3], e.Vertices[1], e.Vertices[2]);
                        triangulated.AddElement(face);
                    }
                    else
                    {
                        IPoint3D pos = ISubdividor.ComputeAveragePosition(e.Vertices, mesh);
                        ITopologicVertex v = new ITopologicVertex(pos.X, pos.Y, pos.Z, key);
                        triangulated.AddVertex(key, v);
                        for (int i = 1; i <= e.HalfFacetsCount; i++)
                        {
                            int[] hf;
                            e.GetFirstLevelHalfFacet(i, out hf);
                            face = new ISurfaceElement(hf[0], hf[1], key);
                            triangulated.AddElement(face);
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

            // Face center points
            foreach (IElement e in mesh.Elements)
            {
                nM.AddVertex(e.Key, new ITopologicVertex(ISubdividor.ComputeAveragePosition(e.Vertices, mesh)));
            }

            int[] data1, data2;
            PointCloud cloud = new PointCloud();
            List<int> global = new List<int>();

            int vertexKey = nM.FindNextVertexKey();
            foreach (ITopologicVertex v in mesh.Vertices)
            {
                data1 = mesh.Topology.GetVertexIncidentElementsSorted(v.Key);
                if (!mesh.Topology.IsNakedVertex(v.Key))
                {
                    nM.AddElement(new ISurfaceElement(data1));
                }
                else
                {
                    List<int> local = new List<int>();

                    data2 = mesh.Topology.GetVertexAdjacentVerticesSorted(v.Key);
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
                                nM.AddVertex(vertexKey, new ITopologicVertex(p));
                                global.Add(vertexKey);
                                local.Add(vertexKey);
                                vertexKey++;
                            }
                            else local.Add(global[idx]);
                        }
                    }

                    nM.AddVertex(vertexKey, new ITopologicVertex(v));
                    local.Insert(1, vertexKey);
                    local.AddRange(data1.Reverse());
                    vertexKey++;

                    nM.AddElement(new ISurfaceElement(local.ToArray()));
                }
            }

            nM.BuildTopology();

            return nM;
        }

        public static IMesh ExtrudeTwoDimensionalElements(IMesh mesh, IEnumerable<int> eKeys, double length)
        {
            IMesh dM = mesh.CleanCopy();
            ITopologicVertex v, vv;
            IVector3D n, pos;
            IElement e, nE;
            int next_vKey = mesh.FindNextVertexKey();

            foreach (int eK in eKeys)
            {
                e = mesh.GetElementWithKey(eK);
                if (e.TopologicDimension == 2)
                {
                    dM.DeleteElement(eK, false);
                    nE = null;
                    mesh.Topology.ComputeTwoDimensionalElementNormal(eK, out n, out pos);
                    n *= length;
                    List<int> vertices = new List<int>(e.Vertices);

                    foreach (int vK in e.Vertices)
                    {
                        v = mesh.GetVertexWithKey(vK);
                        vv = new ITopologicVertex(v.Position + n, next_vKey);
                        dM.AddVertex(next_vKey, vv);
                        vertices.Add(next_vKey);
                        next_vKey++;
                    }

                    if (vertices.Count == 8) nE = new IHexahedronElement(vertices.ToArray());
                    else if (vertices.Count == 6) nE = new IPrismElement(vertices.ToArray());
                    if (nE != null)
                    {
                        dM.AddElement(nE);
                    }
                }
            }

            dM.BuildTopology(true);
            return dM;
        }

        public static IMesh ExtrudeTwoDimensionalElementsEdges(IMesh mesh, IEnumerable<int> eKeys, double length)
        {
            IMesh dM = mesh.CleanCopy();
            ITopologicVertex v, vv;
            IVector3D n;
            IElement e, nE;
            int next_vKey = mesh.FindNextVertexKey();

            foreach (int eK in eKeys)
            {
                e = mesh.GetElementWithKey(eK);
                if (e.TopologicDimension == 2)
                {
                    List<int> vertices = new List<int>();
                    foreach (int vK in e.Vertices)
                    {
                        v = mesh.GetVertexWithKey(vK);
                        n = mesh.Topology.ComputeVertexNormal(vK);
                        vv = new ITopologicVertex(v.Position + n, next_vKey);
                        dM.AddVertex(next_vKey, vv);
                        vertices.Add(next_vKey);
                        next_vKey++;
                    }

                    int[] hf;
                    List<int> temp;
                    int next_i, prev_i;
                    for (int i = 1; i <= e.HalfFacetsCount; i++)
                    {
                        e.GetFirstLevelHalfFacet(i, out hf);
                        temp = new List<int>(hf);
                        next_i = i;
                        if (i == e.HalfFacetsCount) next_i = 0;
                        prev_i = i - 1;
                        temp.Add(vertices[next_i]);
                        temp.Add(vertices[prev_i]);

                        nE = new ISurfaceElement(temp.ToArray());
                        dM.AddElement(nE);
                    }

                }
            }

            dM.BuildTopology(true);
            return dM;
        }

        public static IMesh LaplacianSmoother(IMesh mesh, int smoothingSteps = 1, bool keepNaked = true, IEnumerable<int> exclude = default)
        {
            IMesh dM = mesh.DeepCopy();
            ITopologicVertex v, nV;
            IVector3D pos;
            int[] nKeys;
            int count = 0;
            if (exclude == default) exclude = new List<int>();

            while (count != smoothingSteps)
            {
                foreach (int vK in dM.VerticesKeys)
                {
                    if (!dM.Topology.IsNakedVertex(vK) && !exclude.Contains(vK))
                    {
                        v = dM.GetVertexWithKey(vK);
                        nKeys = dM.Topology.GetVertexAdjacentVertices(vK);
                        pos = new IVector3D();

                        foreach (int nK in nKeys)
                        {
                            nV = dM.GetVertexWithKey(nK);
                            pos += nV.Position;
                        }
                        pos /= nKeys.Length;

                        dM.SetVertexPosition(vK, new IPoint3D(pos.X, pos.Y, pos.Z));
                    }
                }
                count++;
            }

            return dM;
        }

        public static IMesh Stretch(IMesh mesh, IPlane plane = default, IVector3D direction = default, double stretchFactor = 1, double compressionFactor = 1)
        {
            if (plane == default) plane = IPlane.WorldXY;
            if (direction == default) direction = plane.Normal;

            IMesh dM = mesh.DeepCopy();
            IPoint3D p;
            IVector3D vec;
            ITopologicVertex v;
            foreach (int vK in mesh.VerticesKeys)
            {
                v = dM.GetVertexWithKey(vK);

                double d = plane.DistanceToPlane(v.Position);
                if (d > 1e-9)
                {
                    p = plane.GetClosestPoint(v.Position);
                    vec = v.Position - p;
                    vec *= stretchFactor;
                    vec += p;

                    p = GetClosestPointToFictiousLine(v.Position, plane.Origin, plane.Origin+direction);
                    vec -= p;
                    vec *= 1 / compressionFactor;
                    vec += p;

                    dM.SetVertexPosition(vK, vec.X,vec.Y,vec.Z);
                }
            }
            return dM;
        }

        public static IMesh Twist(IMesh mesh, Line axis, double angleFactor)
        {
            IMesh dM = mesh.DeepCopy();
            IPoint3D pt;
            double d;
            ITransformX T;
            IPoint3D origin = new IPoint3D(axis.FromX, axis.FromY, axis.FromZ);
            IVector3D direction = IVector3D.CreateVector(axis.To, axis.From);
            foreach (int vK in dM.VerticesKeys)
            {
                pt = dM.GetVertexWithKey(vK).Position;
                d = axis.DistanceTo(pt.RhinoPoint, true);
                T = new ITransformX();
                T.RotateAboutAxis(d*angleFactor, origin, direction);
                pt.Transform(T);

                dM.SetVertexPosition(vK, pt);
            }
            return dM;
        }

        public static IMesh Skew(IMesh mesh, IPlane plane=default, IVector3D skewDirection=default, double skewFactor=1.0)
        {
            IMesh dM = mesh.DeepCopy();
            if (plane == default) plane = IPlane.WorldXY;
            if (skewDirection == default) skewDirection = IVector3D.UnitZ;
            IPoint3D pt;
            double d;
            foreach(int vK in dM.VerticesKeys)
            {
                pt = dM.GetVertexWithKey(vK).Position;
                d = plane.DistanceToPlane(pt);
                if (d > 0) pt += IVector3D.Mult(skewDirection, d * skewFactor);
                dM.SetVertexPosition(vK, pt);
            }
            return dM;
        }

        internal static IPoint3D GetClosestPointToFictiousLine(IPoint3D p, IPoint3D lineStart, IPoint3D lineEnd)
        {
            IVector3D direction = lineEnd - lineStart;
            IVector3D projection = lineStart-p;
            IPoint3D pos;
            double t = IVector3D.Dot(projection,direction);
            if (t <= 0) return lineStart;
            else
            {
                double eval = Math.Pow(lineStart.DistanceTo(lineEnd),2);
                if (t >= eval) return lineEnd;
                else
                {
                    t = t / eval;
                    pos = new IPoint3D();
                    direction.Mult(t);
                    pos = lineStart+direction;
                    return pos;
                }
            }
        }
    }
}
