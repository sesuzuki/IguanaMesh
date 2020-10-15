using Iguana.IguanaMesh.IWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IguanaClient
{
    public static partial class Example
    {
        public static void T4()
        {
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t4");

            double cm = 1e-02;
            double e1 = 4.5 * cm, e2 = 6 * cm / 2, e3 = 5 * cm / 2;
            double h1 = 5 * cm, h2 = 10 * cm, h3 = 5 * cm, h4 = 2 * cm, h5 = 4.5 * cm;
            double R1 = 1 * cm, R2 = 1.5 * cm, r = 1 * cm;
            double Lc1 = 0.01;
            double Lc2 = 0.003;

            double ccos = (-h5 * R1 + e2 * hypoth(h5, hypoth(e2, R1))) / (h5 * h5 + e2 * e2);
            double ssin = Math.Sqrt(1 - ccos * ccos);

            // We start by defining some points and some lines. To make the code shorter
            IguanaGmsh.Model.Geo.AddPoint(-e1 - e2, 0, 0, Lc1, 1);
            IguanaGmsh.Model.Geo.AddPoint(-e1 - e2, h1, 0, Lc1, 2);
            IguanaGmsh.Model.Geo.AddPoint(-e3 - r, h1, 0, Lc2, 3);
            IguanaGmsh.Model.Geo.AddPoint(-e3 - r, h1 + r, 0, Lc2, 4);
            IguanaGmsh.Model.Geo.AddPoint(-e3, h1 + r, 0, Lc2, 5);
            IguanaGmsh.Model.Geo.AddPoint(-e3, h1 + h2, 0, Lc1, 6);
            IguanaGmsh.Model.Geo.AddPoint(e3, h1 + h2, 0, Lc1, 7);
            IguanaGmsh.Model.Geo.AddPoint(e3, h1 + r, 0, Lc2, 8);
            IguanaGmsh.Model.Geo.AddPoint(e3 + r, h1 + r, 0, Lc2, 9);
            IguanaGmsh.Model.Geo.AddPoint(e3 + r, h1, 0, Lc2, 10);
            IguanaGmsh.Model.Geo.AddPoint(e1 + e2, h1, 0, Lc1, 11);
            IguanaGmsh.Model.Geo.AddPoint(e1 + e2, 0, 0, Lc1, 12);
            IguanaGmsh.Model.Geo.AddPoint(e2, 0, 0, Lc1, 13);

            IguanaGmsh.Model.Geo.AddPoint(R1 / ssin, h5 + R1 * ccos, 0, Lc2, 14);
            IguanaGmsh.Model.Geo.AddPoint(0, h5, 0, Lc2, 15);
            IguanaGmsh.Model.Geo.AddPoint(-R1 / ssin, h5 + R1 * ccos, 0, Lc2, 16);
            IguanaGmsh.Model.Geo.AddPoint(-e2, 0.0, 0, Lc1, 17);

            IguanaGmsh.Model.Geo.AddPoint(-R2, h1 + h3, 0, Lc2, 18);
            IguanaGmsh.Model.Geo.AddPoint(-R2, h1 + h3 + h4, 0, Lc2, 19);
            IguanaGmsh.Model.Geo.AddPoint(0, h1 + h3 + h4, 0, Lc2, 20);
            IguanaGmsh.Model.Geo.AddPoint(R2, h1 + h3 + h4, 0, Lc2, 21);
            IguanaGmsh.Model.Geo.AddPoint(R2, h1 + h3, 0, Lc2, 22);
            IguanaGmsh.Model.Geo.AddPoint(0, h1 + h3, 0, Lc2, 23);

            IguanaGmsh.Model.Geo.AddPoint(0, h1 + h3 + h4 + R2, 0, Lc2, 24);
            IguanaGmsh.Model.Geo.AddPoint(0, h1 + h3 - R2, 0, Lc2, 25);

            IguanaGmsh.Model.Geo.AddLine(1, 17, 1);
            IguanaGmsh.Model.Geo.AddLine(17, 16, 2);

            // Gmsh provides other curve primitives than straight lines: splines,
            // B-splines, circle arcs, ellipse arcs, etc. Here we define a new circle arc,
            // starting at point 14 and ending at point 16, with the circle's center being
            // the point 15:
            IguanaGmsh.Model.Geo.AddCircleArc(14, 15, 16, 3);

            // Note that, in Gmsh, circle arcs should always be smaller than Pi. The
            // OpenCASCADE geometry kernel does not have this limitation.

            // We can then define additional lines and circles, as well as a new surface:
            IguanaGmsh.Model.Geo.AddLine(14, 13, 4);
            IguanaGmsh.Model.Geo.AddLine(13, 12, 5);
            IguanaGmsh.Model.Geo.AddLine(12, 11, 6);
            IguanaGmsh.Model.Geo.AddLine(11, 10, 7);
            IguanaGmsh.Model.Geo.AddCircleArc(8, 9, 10, 8);
            IguanaGmsh.Model.Geo.AddLine(8, 7, 9);
            IguanaGmsh.Model.Geo.AddLine(7, 6, 10);
            IguanaGmsh.Model.Geo.AddLine(6, 5, 11);
            IguanaGmsh.Model.Geo.AddCircleArc(3, 4, 5, 12);
            IguanaGmsh.Model.Geo.AddLine(3, 2, 13);
            IguanaGmsh.Model.Geo.AddLine(2, 1, 14);
            IguanaGmsh.Model.Geo.AddLine(18, 19, 15);
            IguanaGmsh.Model.Geo.AddCircleArc(21, 20, 24, 16);
            IguanaGmsh.Model.Geo.AddCircleArc(24, 20, 19, 17);
            IguanaGmsh.Model.Geo.AddCircleArc(18, 23, 25, 18);
            IguanaGmsh.Model.Geo.AddCircleArc(25, 23, 22, 19);
            IguanaGmsh.Model.Geo.AddLine(21, 22, 20);

            IguanaGmsh.Model.Geo.AddCurveLoop(new[] { 17, -15, 18, 19, -20, 16 }, 21);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new[] { 21 }, 22);

            // But we still need to define the exterior surface. Since this surface has a
            // hole, its definition now requires two curves loops:
            IguanaGmsh.Model.Geo.AddCurveLoop(new[] { 11, -12, 13, 14, 1, 2, -3, 4, 5, 6, 7, -8, 9, 10 }, 23);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new[] { 23, 21 }, 24);

            // As a general rule, if a surface has N holes, it is defined by N+1 curve
            // loops: the first loop defines the exterior boundary; the other loops define
            // the boundaries of the holes.

            IguanaGmsh.Model.Geo.Synchronize();

            // We can also change the color of some entities:
            Tuple<int, int>[] tmp = new Tuple<int, int>[] { Tuple.Create(2, 22) };
            IguanaGmsh.Model.SetColor(tmp, 127, 127, 127); // Gray50
            tmp = new Tuple<int, int>[] { Tuple.Create(2, 24) };
            IguanaGmsh.Model.SetColor(tmp, 160, 32, 240); // Purple

            for (int i = 1; i <= 14; i++) {
                tmp = new Tuple<int, int>[] { Tuple.Create(2, i) };
                IguanaGmsh.Model.SetColor(tmp, 255, 0, 0); // Red
            }

            for (int i = 15; i <= 20; i++)
            {
                tmp = new Tuple<int, int>[] { Tuple.Create(1, i) };
                IguanaGmsh.Model.SetColor(tmp, 255, 255, 0); // Yellow
            }

            IguanaGmsh.Model.Mesh.Generate(2);

            IguanaGmsh.Write("t4.msh");

            IguanaGmsh.FinalizeGmsh();

        }

        public static double hypoth(double a, double b) 
        { 
            return Math.Sqrt(a * a + b * b); 
        }
    }
}
