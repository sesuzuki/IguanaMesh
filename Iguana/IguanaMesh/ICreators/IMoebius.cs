using System;
using System.Collections.Generic;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ICreators
{
    class IMoebius : ICreatorInterface
    {
        private double x, y, z, u, v, r1=1, r2=1, h=1;
        private int U = 5, V = 5, keyElement = 1, keyNode=1;
        private List<ITopologicVertex> vertices = new List<ITopologicVertex>();
        private List<IElement> faces = new List<IElement>();
        private Plane pl;

        public IMoebius(int _U, int _V, Plane _pl)
        {
            this.U = _U;
            this.V = _V;
            this.pl = _pl;
        }

        public IMoebius(int _U, int _V, double _r1, double _r2, double _h, Plane _pl)
        {
            this.U = _U;
            this.V = _V;
            this.pl = _pl;
            this.r1 = _r1;
            this.r2 = _r2;
            this.h = _h;
        }

        public IMesh BuildMesh()
        {
            IMesh mesh = new IMesh();
            Boolean flag = BuildDataBase();
            if (flag)
            {
                mesh = new IMesh();
                mesh.AddRangeVertices(vertices);
                mesh.AddRangeElements(faces);

                mesh.BuildTopology();
            }

            return mesh;
        }

        public bool BuildDataBase()
        {
            if (U == 0 || V == 0) return false;
            else
            {
                double stepU = (2 * Math.PI) / (U - 1);
                double stepV = 1.0 / (V - 1);
                Point3d pt;

                // Vertices
                for (int i = 0; i < U - 1; i++)
                {
                    for (int j = 0; j < V; j++)
                    {

                        u = stepU * i;
                        v = stepV * j - 0.5;

                        x = r1 * ( (1 + (v / 2) * Math.Cos(u / 2)) * Math.Cos(u));
                        y = r2 * ( (1 + (v / 2) * Math.Cos(u / 2)) * Math.Sin(u) );
                        z = h * ( (v / 2) * Math.Sin(u / 2) );
                        pt = pl.PointAt(x, y, z);

                        vertices.Add(new ITopologicVertex(pt.X,pt.Y,pt.Z,u,v,0,keyNode));
                        keyNode++;
                    }
                }

                //Quad-faces
                for (int i = 0; i < U - 1; i++)
                {
                    for (int j = 0; j < V - 1; j++)
                    {
                        int[] f = new int[4];
                        f[0] = (i * V + j)+1;
                        f[1] = (i * V + (j + 1))+ 1;
                        f[2] = ((i + 1) * V + (j + 1))+ 1;
                        f[3] = ((i + 1) * V + j)+ 1;

                        if (i == U - 2)
                        {
                            f[0] = (i * V + j)+ 1;
                            f[1] = (i * V + (j + 1))+ 1;
                            f[2] = ((V - 1) - (j + 1))+ 1;
                            f[3] = ((V - 1) - (j))+ 1;
                        }

                        ISurfaceElement iF = new ISurfaceElement(f);
                        iF.Key = keyElement;
                        faces.Add(iF);

                        keyElement++;
                    }
                }

                return true;
            }
        }


    }
}
