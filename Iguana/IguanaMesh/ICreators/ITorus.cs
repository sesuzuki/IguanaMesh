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

using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Iguana.IguanaMesh.ICreators
{
    class ITorus : ICreatorInterface
    {
        private List<ITopologicVertex> vertices = new List<ITopologicVertex>();
        private List<IElement> faces = new List<IElement>();
        private int U = 30, V = 10, keyElement=1;
        private double uStep, vStep, R1, R2;
        private Plane pl;
        private Interval D1 = new Interval(0, 2 * Math.PI);
        private Interval D2 = new Interval(0, 2 * Math.PI);

        public ITorus(int _U, int _V, double r1, double r2, Interval domainX, Interval domainY, Plane _pl)
        {
            this.U = _U;
            this.V = _V;
            this.R1 = r1;
            this.R2 = r2;
            this.pl = _pl;
            this.D1 = domainX;
            this.D2 = domainY;
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
                double x, y, z, u, v;
                Point3d pt;
                List<Point3d> tempV = new List<Point3d>();
                Boolean flag;
                int keyMap=1;
                Dictionary<int, int> maps = new Dictionary<int, int>();

                if (D1.T0 < 0) D1.T0 = 0;
                if (D2.T0 < 0) D2.T0 = 0;
                if (D1.T1 > 2 * Math.PI) D1.T1 = 2 * Math.PI;
                if (D2.T1 > 2 * Math.PI) D2.T1 = 2 * Math.PI;

                uStep = D1.T1 / (U - 1);
                vStep = D2.T1 / (V - 1);

                // Vertices
                for (int i = 0; i < U; i++)
                {
                    for (int j = 0; j < V; j++)
                    {

                        u = uStep * i + D1.T0;
                        v = vStep * j + D2.T0;

                        x = (R1 + R2 * Math.Cos(v)) * Math.Cos(u);
                        y = (R1 + R2 * Math.Cos(v)) * Math.Sin(u);
                        z = R2 * Math.Sin(v);
                        pt = pl.PointAt(x, y, z);

                        //Temporary list of vertices
                        tempV.Add(pt);

                        //list of unique vertices
                        flag = true;
                        vertices.ForEach(eval => {
                            if (eval.DistanceTo(pt) < 0.01) flag = false;
                        });
                        if (flag)
                        {
                            vertices.Add(new ITopologicVertex(pt.X,pt.Y,pt.Z,u,v,0,keyMap));
                            keyMap++;
                        }

                    }
                }

                //Creates mapping
                for (int i = 0; i < tempV.Count; i++)
                {
                    pt = tempV[i];
                    keyMap = -1;
                    vertices.ForEach(eval => {
                        if (eval.DistanceTo(pt) < Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance) keyMap = eval.Key;
                    });
                    maps.Add(i, keyMap);
                }

                //Quad-faces
                for (int i = 0; i < U - 1; i++)
                {
                    for (int j = 0; j < V - 1; j++)
                    {
                        int[] f = new int[4];
                        f[0] = maps[i * (V) + j];
                        f[1] = maps[i * (V) + (j + 1)];
                        f[2] = maps[(i + 1) * (V) + (j + 1)];
                        f[3] = maps[(i + 1) * (V) + j];

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
