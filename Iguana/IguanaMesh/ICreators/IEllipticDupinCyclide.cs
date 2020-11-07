using System;
using System.Collections.Generic;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ICreators
{
    class IEllipticDupinCyclide : ICreatorInterface
    {
        private List<ITopologicVertex> vertices = new List<ITopologicVertex>();
        private List<IElement> faces = new List<IElement>();
        private int U = 30, V = 10, keyElement = 1;
        private double a=5.0, b=5.0, c=1.0, d=1.5, uStep, vStep;
        private Plane pl;
        private Interval D1 = new Interval(0, 2 * Math.PI);
        private Interval D2 = new Interval(0, 2 * Math.PI);

        public IEllipticDupinCyclide(int _U, int _V, double _a, double _b, double _c, double _d, Interval domainX, Interval domainY, Plane _pl)
        {
            this.U = _U;
            this.V = _V;
            this.a = _a;
            this.b = _b;
            this.c = _c;
            this.d = _d;
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
                mesh.Vertices.AddRangeVertices(vertices);
                mesh.Elements.AddRangeElements(faces);

                mesh.BuildTopology();
            }

            return mesh;
        }

        public bool BuildDataBase()
        {
            if (U == 0 || V == 0) return false;
            else
            {
                double x, y, z, u, v, cosU, cosV, sinU, sinV;
                Point3d pt;
                List<Point3d> tempV = new List<Point3d>();
                Boolean flag;
                int keyMap = 1;
                Dictionary<int, int> maps = new Dictionary<int, int>();

                if (D1.T0 < 0) D1.T0 = 0;
                if (D2.T0 < 0) D2.T0 = 0;
                if (D1.T1 > 2*Math.PI) D1.T1 = 2 * Math.PI;
                if (D2.T1 > 2 * Math.PI) D2.T1 = 2 * Math.PI;

                uStep = D1.Length / (U - 1);
                vStep = D2.Length / (V - 1);

                // Vertices
                for (int i = 0; i < U ; i++)
                {
                    for (int j = 0; j < V; j++)
                    {

                        u = uStep * i;
                        v = vStep * j;

                        cosU = Math.Cos(u);
                        cosV = Math.Cos(v);
                        sinU = Math.Sin(u);
                        sinV = Math.Sin(v);

                        x = (d * (c - a * cosU * cosV) + Math.Pow(b, 2) * cosU) / (a - c * cosU * cosV);
                        y = (b * sinU * (a - d * cosV)) / (a - c * cosU * cosV);
                        z = (b * sinV * (c * cosU - d)) / (a - c * cosU * cosV);
                        pt = pl.PointAt(x, y, z);

                        //Temporary list of vertices
                        tempV.Add(pt);

                        //list of unique vertices
                        flag = true;
                        vertices.ForEach(eval => {
                            if (eval.DistanceTo(pt) < Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance) flag = false;
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
                        if (eval.DistanceTo(pt) < 0.01) keyMap = eval.Key;
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

                        ISurfaceElement pF = new ISurfaceElement(f);
                        pF.Key = keyElement;
                        faces.Add(pF);

                        keyElement++;
                    }
                }
                return true;
            }
        }
    }
}
