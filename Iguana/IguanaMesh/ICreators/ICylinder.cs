using System;
using System.Collections.Generic;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ICreators
{
    class ICylinder : ICreatorInterface
    {
        private Plane pl = new Plane();
        private double r1 = 1, r2 = 1, h = 1;
        private int u = 30, v = 20, keyVertex = 0, keyElement = 0;
        private List<ITopologicVertex> vertices = new List<ITopologicVertex>();
        private List<IElement> faces = new List<IElement>();

        public ICylinder(int U, int V, double lowerRadius, double upperRadius, double height, Plane plane)
        {
            pl = plane;
            u = U;
            v = V;
            r1 = lowerRadius;
            r2 = upperRadius;
            h = height;
        }

        public IMesh BuildMesh()
        {
            IMesh mesh = new IMesh();
            Boolean flag = BuildDataBase();
            if (flag)
            {
                mesh = new IMesh();
                mesh.Vertices.AddRangeVertices(vertices);
                mesh.Elements.AddRangeElements(faces);

                mesh.BuildTopology();
            }

            return mesh;
        }

        public bool BuildDataBase()
        {
            if (u == 0 || v == 0) return false;
            else
            {
                Circle c1 = new Circle(pl, r1);
                Circle c2 = new Circle(pl, r2);
                c2.Translate(pl.Normal * h);

                double t1 = (2 * Math.PI) / u;
                double t2 = 1.0 / v;

                //Construct Vertices
                for (int i = 0; i < u; i++)
                {
                    Line ln = new Line(c1.PointAt(t1 * i), c2.PointAt(t1 * i));
                    for (int j = 0; j <= v; j++)
                    {
                        ITopologicVertex p = new ITopologicVertex(ln.PointAt(t2 * j));
                        p.Key = keyVertex;

                        vertices.Add(p);
                        keyVertex++;
                    }
                }

                //Construct Faces
                for (int i = 0; i < u; i++)
                {
                    int idxA = i;
                    int idxB = i + 1;
                    if (idxA == (u - 1)) idxB = 0;
                    for (int j = 0; j < v; j++)
                    {
                        ISurfaceElement f = new ISurfaceElement(idxA * (v + 1) + j, idxA * (v + 1) + j + 1, idxB * (v + 1) + j + 1, idxB * (v + 1) + j);
                        f.Key = keyElement;
                        faces.Add(f);

                        keyElement++;
                    }
                }
                return true;
            }
        }
    }
}
