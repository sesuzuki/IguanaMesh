using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;

namespace Iguana.IguanaMesh.ICreators
{
    class IPlaneGrid : ICreatorInterface
    {
        private int U = 1, V = 1, keyElement=1;
        private double sizeU, sizeV;
        private List<ITopologicVertex> vertices = new List<ITopologicVertex>();
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
                int keyNode = 1;

                //Creation of vertices
                for(int i=0; i<=U; i++)
                {
                    for(int j=0; j<=V; j++)
                    {
                        Point3d pt = pl.PointAt(i*stepU - sizeU/2, j*stepV - sizeV/2, 0);
                        Tuple<double, double> uvPt = Tuple.Create(i*mapU, j*mapV);

                        vertices.Add(new ITopologicVertex(pt.X,pt.Y,pt.Z, i * mapU, j * mapV, 0, keyNode));
                        keyNode++;
                    }
                }

                //Toplogy of quadragular faces
                for (int i = 0; i < U; i++)
                {
                    for (int j = 0; j < V; j++)
                    {
                        int[] f = new int[4];
                        f[0] = ((i * (V + 1)) + j) +1;
                        f[1] = ((i * (V + 1)) + (j + 1))+1;
                        f[2] = (((i + 1) * (V + 1)) + (j + 1)) +1;
                        f[3] = (((i + 1) * (V + 1)) + j) +1;

                        ISurfaceElement iF = new ISurfaceElement(f);
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
