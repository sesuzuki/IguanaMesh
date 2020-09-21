using System;
using System.Collections.Generic;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ICreators
{
    class ICube : ICreatorInterface
    {
        private Brep brep;
        private int u, v, w;
        private Mesh[] rM;
        private Boolean weld;
        private double tolerance;

        public ICube(Brep _brep, int _u, int _v, int _w, Boolean _weld, double _tolerance)
        {
            brep = _brep;
            u = _u;
            v = _v;
            w = _w;
            weld = _weld;
            tolerance = _tolerance;
            rM = new Mesh[6];
        }

        public IMesh BuildMesh()
        {
            IMesh m = new IMesh();
            Boolean flag = BuildDataBase();

            if (flag)
            {
                foreach (Mesh rhinoMesh in rM)
                {
                    m.AddRhinoMesh(rhinoMesh, weld, tolerance);
                }
            }
            return m;
        }

        public bool BuildDataBase()
        {
            if (brep != null)
            {
                rM[0] = CreateRhinoMesh(brep.Surfaces[0], u, w);
                rM[1] = CreateRhinoMesh(brep.Surfaces[1], v, w);
                rM[2] = CreateRhinoMesh(brep.Surfaces[2], u, w);
                rM[3] = CreateRhinoMesh(brep.Surfaces[3], v, w);
                rM[4] = CreateRhinoMesh(brep.Surfaces[4], v, u);
                rM[5] = CreateRhinoMesh(brep.Surfaces[5], u, v);
                return true;
            }
            else return false;
        }

        private Mesh CreateRhinoMesh(Surface srf, int u, int v)
        {
            Mesh m = new Mesh();
            List<Point3d> pts = new List<Point3d>();

            srf.SetDomain(0, new Interval(0, 1));
            srf.SetDomain(1, new Interval(0, 1));
            double t1 = 1.0 / u;
            double t2 = 1.0 / v;
            for (int i = 0; i <= u; i++)
            {
                for (int j = 0; j <= v; j++)
                {
                    Point3d pt = srf.PointAt(t1 * i, t2 * j);
                    pts.Add(pt);
                }
            }
            m.Vertices.AddVertices(pts);


            for (int i = 0; i < u; i++)
            {
                for (int j = 0; j < v; j++)
                {
                    MeshFace f = new MeshFace(i * (v + 1) + j, i * (v + 1) + j + 1, (i + 1) * (v + 1) + j + 1, (i + 1) * (v + 1) + j);
                    m.Faces.AddFace(f);
                }
            }

            return m;
        }
    }
}
