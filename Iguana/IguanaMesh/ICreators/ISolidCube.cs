using System;
using System.Collections.Generic;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ICreators
{
    class ISolidCube : ICreatorInterface
    {
        private Box box;
        private int u, v, w, keyElement=0;
        private List<Point3d> vertices = new List<Point3d>();
        private List<IElement> solids = new List<IElement>();

        public ISolidCube(Box _box, int _u, int _v, int _w)
        {
            box = _box;
            u = _u;
            v = _v;
            w = _w;
        }

        public IMesh BuildMesh()
        {
            IMesh mesh = new IMesh();
            Boolean flag = BuildDataBase();
            if (flag)
            {
                mesh = new IMesh();
                mesh.Vertices.AddRangeVertices(vertices);
                mesh.Elements.AddRangeElements(solids);

                mesh.BuildTopology();
            }

            return mesh;
        }

        public bool BuildDataBase()
        {
            if (box.IsValid)
            {
                Interval domU = box.X;
                Interval domV = box.Y;
                Interval domW = box.Z;

                double uStep = domU.Length / (u - 1);
                double vStep = domV.Length / (v - 1);
                double wStep = domW.Length / (w - 1);

                //Vertices
                for (int i = 0; i < u; i++)
                {
                    for (int j = 0; j < v; j++)
                    {
                        for (int k = 0; k < w; k++)
                        {
                            double x = domU.T0 + uStep * i;
                            double y = domV.T0 + vStep * j;
                            double z = domW.T0 + wStep * k;

                            Point3d pt = box.Plane.PointAt(x, y, z);

                            vertices.Add(pt);
                        }
                    }
                }

                //Faces
                int A, B, C, D, E, F, G, H;
                for (int i = 0; i < u - 1; i++)
                {
                    for (int j = 0; j < v - 1; j++)
                    {
                        for (int k = 0; k < w - 1; k++)
                        {

                            A = i * (v * w) + j * w + k;
                            B = i * (v * w) + (j + 1) * w + k;
                            C = (i + 1) * (v * w) + (j + 1) * w + k;
                            D = (i + 1) * (v * w) + j * w + k;
                            E = i * (v * w) + j * w + (k + 1);
                            F = i * (v * w) + (j + 1) * w + k + 1;
                            G = (i + 1) * (v * w) + (j + 1) * w + (k + 1);
                            H = (i + 1) * (v * w) + j * w + k + 1;

                            int[] f = new int[8] { A,B,C,D,E,F,G,H };

                            IHexahedronSolid iS = new IHexahedronSolid(f);
                            iS.Key = keyElement;
                            solids.Add(iS);

                            keyElement++;
                        }
                    }
                }
                return true;
            }
            else return false;
        }
    }
}
