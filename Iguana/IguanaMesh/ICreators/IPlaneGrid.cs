using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Iguana.IguanaMesh.ITypes;

namespace Iguana.IguanaMesh.ICreators
{
    class IPlaneGrid : ICreatorInterface
    {
        private int U = 1, V = 1, keyElement=0;
        private double sizeU, sizeV;
        private List<Point3d> vertices = new List<Point3d>();
        private List<IElement> faces = new List<IElement>();
        private List<Tuple<double, double>> uv = new List<Tuple<double, double>>();
        private Plane pl;

        public IPlaneGrid(int U, int V, double sizeU, double sizeV, Plane plane)
        {
            this.U = U;
            this.V = V;
            this.sizeU = sizeU;
            this.sizeV = sizeV;
            pl = plane;
        }

        public Boolean BuildDataBase()
        {
            if (sizeU == 0 || sizeV == 0) return false;
            else
            {

                double stepU = sizeU / U;
                double stepV = sizeV / V;
                double mapU = 1.0 / U;
                double mapV = 1.0 / V;

                //Creation of vertices
                for(int i=0; i<=U; i++)
                {
                    for(int j=0; j<=V; j++)
                    {
                        Point3d pt = pl.PointAt(i*stepU - sizeU/2, j*stepV - sizeV/2, 0);
                        Tuple<double, double> uvPt = Tuple.Create(i*mapU, j*mapV);

                        vertices.Add(pt);
                        uv.Add(uvPt);
                    }
                }

                //Toplogy of quadragular faces
                for (int j = 0; j < V; j++)
                {
                    for (int i = 0; i < U; i++)
                    {
                        int[] f = new int[4];
                        f[0] = i + (U + 1) * j;
                        f[1] = i + 1 + (U + 1) * j;
                        f[2] = i + 1 + (U + 1) * (j + 1);
                        f[3] = i + (U + 1) * (j + 1);

                        IPolygonalFace iF = new IPolygonalFace(f);
                        iF.Key = keyElement;
                        faces.Add(iF);

                        keyElement++;
                    }
                }

                return true;
            }
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
    }
}
