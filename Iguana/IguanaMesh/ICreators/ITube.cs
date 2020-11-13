using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;
using System;
using System.Linq;

namespace Iguana.IguanaMesh.ICreators
{
    class ITube : ICreatorInterface
    {
        private int U = 1, V = 5;
        private double innerRadius, outerRadius, height, shiftX = 0, shiftY = 0;
        private ITopologicVertex[] vertices;
        private int[][] faces;
        private Curve path;

        public ITube(int U, int V, double innerRadius, double outerRadius, double height, double shiftX, double shiftY)
        {
            this.U = U;
            this.V = V;
            this.innerRadius = innerRadius;
            this.outerRadius = outerRadius;
            this.height = height;
            this.shiftX = shiftX;
            this.shiftY = shiftY;
            path = null;
        }

        public ITube(int U, int V, double innerRadius, double outerRadius, Curve path)
        {
            this.U = U;
            this.V = V;
            this.innerRadius = innerRadius;
            this.outerRadius = outerRadius;
            this.height = path.GetLength();
            this.path = path.DuplicateCurve();
            this.path.Domain = new Interval(0, U);
        }

        public IMesh BuildMesh()
        {
            IMesh mesh = new IMesh();
            Boolean flag = BuildDataBase();
            if (flag)
            {
                mesh = new IMesh();
                mesh.AddRangeVertices(vertices.ToList());
                foreach (int[] f in faces)
                {
                    mesh.AddElement(new ISurfaceElement(f[0], f[1], f[2], f[3]));
                }

                mesh.BuildTopology();
            }

            return mesh;
        }

        public Boolean BuildDataBase()
        {
            if (U == 0 || V == 0) return false;
            else
            {

                vertices = new ITopologicVertex[((U + 1) * V) * 2];
                int keyNode = 1;
                // outer vertices
                //Plane old = new Plane();
                for (int i = 0; i <= U; i++)
                {
                    double z = (i * height) / U;

                    if (path != null)
                    {
                        Point3d origin = path.PointAt(i);
                        int idx = i + 1;
                        if (i == U) idx = i - 1;
                        Point3d next = path.PointAt(idx);
                        Vector3d vec = next - origin;
                        if(i == U) vec.Reverse();

                        Plane pl = new Plane(origin, vec);
                        /*if (i == 0) old = pl;
                        else 
                        {
                            pl.Transform(Transform.PlaneToPlane(pl, old));
                            old = pl;
                        }*/
                        

                        for (int j = 0; j < V; j++)
                        {
                            double x = outerRadius * Math.Cos(((2 * Math.PI) / V) * j);
                            double y = outerRadius * Math.Sin(((2 * Math.PI) / V) * j);
                            Point3d pt = pl.PointAt(x, y);

                            vertices[j + (i * V)] = new ITopologicVertex(pt.X,pt.Y,pt.Z,keyNode);
                            keyNode++;
                        }

                    }
                    else
                    {
                        for (int j = 0; j < V; j++)
                        {
                            vertices[j + (i * V)].X = outerRadius * Math.Cos(((2 * Math.PI) / V) * j);
                            vertices[j + (i * V)].Y = outerRadius * Math.Sin(((2 * Math.PI) / V) * j);
                            vertices[j + (i * V)].Z = z;
                        }
                    }
                }


                // inner vertices
                for (int i = U; i >= 0; i--)
                {
                    double z = (i * height) / U;

                    if (path != null)
                    {
                        Point3d origin = path.PointAt(i);
                        int idx = i - 1;
                        if (i == 0) idx = i + 1;
                        Point3d next = path.PointAt(idx);
                        Vector3d vec = origin - next;
                        if (i == 0) vec.Reverse();
                        Plane pl = new Plane(origin, vec);

                        for (int j = 0; j < V; j++)
                        {
                            double x = shiftX + (innerRadius * Math.Cos(((2 * Math.PI) / V) * j));
                            double y = shiftY + (innerRadius * Math.Sin(((2 * Math.PI) / V) * j));
                            Point3d pt = pl.PointAt(x, y);

                            vertices[j + (i * V) + ((U + 1) * V)] = new ITopologicVertex(pt.X,pt.Y,pt.Z, keyNode);
                            keyNode++;
                        }

                    }
                    else
                    {
                        for (int j = 0; j < V; j++)
                        {
                            vertices[(j + (i * V) + ((U + 1) * V))].X = shiftX + (innerRadius * Math.Cos(((2 * Math.PI) / V) * j));
                            vertices[(j + (i * V) + ((U + 1) * V))].Y = shiftY + (innerRadius * Math.Sin(((2 * Math.PI) / V) * j));
                            vertices[(j + (i * V) + ((U + 1) * V))].Z = z;
                        }
                    }
                }

                faces = new int[(U * V * 2) + (V * 2)][];
                for (int j = 0; j < V; j++)
                {
                    // outer faces
                    for (int i = 0; i < U; i++)
                    {
                        faces[j + (i * V)] = new int[4];
                        faces[j + (i * V)][0] = vertices[((j + 1) % V) + (i * V)].Key;
                        faces[j + (i * V)][1] = vertices[((j + 1) % V) + V + (i * V)].Key;
                        faces[j + (i * V)][2] = vertices[j + (i * V) + V].Key;
                        faces[j + (i * V)][3] = vertices[j + (i * V)].Key;
                    }
                    // top faces
                    if (j != (V - 1))
                    {
                        faces[j + (U * V)] = new int[4];
                        faces[j + (U * V)][0] = vertices[j + (U * V) + V].Key;
                        faces[j + (U * V)][1] = vertices[j + 1 + (U * V) + V].Key;
                        faces[j + (U * V)][2] = vertices[j + 1].Key;
                        faces[j + (U * V)][3] = vertices[j].Key;
                    }
                    else
                    {
                        faces[j + (U * V)] = new int[4];
                        faces[j + (U * V)][0] = vertices[j + (U * V) + V].Key;
                        faces[j + (U * V)][1] = vertices[(U * V) + V].Key;
                        faces[j + (U * V)][2] = vertices[0].Key;
                        faces[j + (U * V)][3] = vertices[j].Key;
                    }
                    // inner faces
                    for (int i = 0; i < U; i++)
                    {
                        faces[(j + (i * V)) + (U * V) + V] = new int[4];
                        faces[(j + (i * V)) + (U * V) + V][0] = vertices[(j + (i * V)) + (U * V) + V].Key;
                        faces[(j + (i * V)) + (U * V) + V][1] = vertices[(j + (i * V) + V) + (U * V) + V].Key;
                        faces[(j + (i * V)) + (U * V) + V][2] = vertices[(((j + 1) % V) + V + (i * V)) + (U * V) + V].Key;
                        faces[(j + (i * V)) + (U * V) + V][3] = vertices[(((j + 1) % V) + (i * V)) + (U * V) + V].Key;
                    }
                    // bottom faces
                    if (j != (V - 1))
                    {
                        faces[j + (2 * (U * V)) + V] = new int[4];
                        faces[j + (2 * (U * V)) + V][0] = vertices[j + (U * V)].Key;
                        faces[j + (2 * (U * V)) + V][1] = vertices[j + 1 + (U * V)].Key;
                        faces[j + (2 * (U * V)) + V][2] = vertices[j + 1 + (U * V) + V + (U * V)].Key;
                        faces[j + (2 * (U * V)) + V][3] = vertices[j + (U * V) + V + (U * V)].Key;
                    }
                    else
                    {
                        faces[j + (2 * (U * V)) + V] = new int[4];
                        faces[j + (2 * (U * V)) + V][0] = vertices[j + (U * V)].Key;
                        faces[j + (2 * (U * V)) + V][1] = vertices[(V * U)].Key;
                        faces[j + (2 * (U * V)) + V][2] = vertices[vertices.Length - 1 - j].Key;
                        faces[j + (2 * (U * V)) + V][3] = vertices[vertices.Length-1].Key;
                    }
                }

                return true;
            }

        }
    }
}
