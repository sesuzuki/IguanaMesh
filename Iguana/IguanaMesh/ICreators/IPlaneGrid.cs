/*
 * <Iguana>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Iguana.IguanaMesh.ITypes;

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
                mesh.AddRangeVertices(vertices);
                mesh.AddRangeElements(faces);

                mesh.BuildTopology();
            }

            return mesh;
        }
    }
}
