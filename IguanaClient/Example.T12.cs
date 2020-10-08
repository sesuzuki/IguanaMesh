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
        public static void T12()
        {

            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t12");

            double lc = 0.1;

            IguanaGmsh.Model.Geo.AddPoint(0, 0, 0, lc, 1);
            IguanaGmsh.Model.Geo.AddPoint(1, 0, 0, lc, 2);
            IguanaGmsh.Model.Geo.AddPoint(1, 1, 0.5, lc, 3);
            IguanaGmsh.Model.Geo.AddPoint(0, 1, 0.4, lc, 4);
            IguanaGmsh.Model.Geo.AddPoint(0.3, 0.2, 0, lc, 5);
            IguanaGmsh.Model.Geo.AddPoint(0, 0.01, 0.01, lc, 6);
            IguanaGmsh.Model.Geo.AddPoint(0, 0.02, 0.02, lc, 7);
            IguanaGmsh.Model.Geo.AddPoint(1, 0.05, 0.02, lc, 8);
            IguanaGmsh.Model.Geo.AddPoint(1, 0.32, 0.02, lc, 9);

            IguanaGmsh.Model.Geo.AddLine(1, 2, 1);
            IguanaGmsh.Model.Geo.AddLine(2, 8, 2);
            IguanaGmsh.Model.Geo.AddLine(8, 9, 3);
            IguanaGmsh.Model.Geo.AddLine(9, 3, 4);
            IguanaGmsh.Model.Geo.AddLine(3, 4, 5);
            IguanaGmsh.Model.Geo.AddLine(4, 7, 6);
            IguanaGmsh.Model.Geo.AddLine(7, 6, 7);
            IguanaGmsh.Model.Geo.AddLine(6, 1, 8);
            IguanaGmsh.Model.Geo.AddSpline(new int[] { 7, 5, 9 }, 9);
            IguanaGmsh.Model.Geo.AddLine(6, 8, 10);

            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { 5, 6, 9, 4 }, 11);
            IguanaGmsh.Model.Geo.AddSurfaceFilling(11, -1, 1);

            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -9, 3, 10, 7 }, 13);
            IguanaGmsh.Model.Geo.AddSurfaceFilling(13, -1, 5);

            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -10, 2, 1, 8 }, 15);
            IguanaGmsh.Model.Geo.AddSurfaceFilling(15, -1, 10);

            IguanaGmsh.Model.Geo.Synchronize();

            // Treat curves 2, 3 and 4 as a single curve when meshing (i.e. mesh across
            // points 6 and 7)
            IguanaGmsh.Model.Mesh.SetCompound(1, new int[] { 2, 3, 4 });

            // Idem with curves 6, 7 and 8
            IguanaGmsh.Model.Mesh.SetCompound(1, new int[] { 6, 7, 8 });

            // Treat surfaces 1, 5 and 10 as a single surface when meshing (i.e. mesh
            // across curves 9 and 10)
            IguanaGmsh.Model.Mesh.SetCompound(2, new int[] { 1, 5, 10 });

            IguanaGmsh.Model.Mesh.Generate(2);

            IguanaGmsh.FinalizeGmsh();

        }
    }
}
