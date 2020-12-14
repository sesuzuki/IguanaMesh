using Iguana.IguanaMesh.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IguanaClient
{
    // -----------------------------------------------------------------------------
    //
    //  Gmsh C++ tutorial 19
    //
    //  Thrusections, fillets, pipes, mesh size from curvature
    //
    // -----------------------------------------------------------------------------

    // The OpenCASCADE geometry kernel supports several useful features for solid
    // modelling.

    public static partial class Example
    {
        public static void T19()
        {
            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);

            Kernel.Model.Add("t19");

            // Volumes can be constructed from (closed) curve loops thanks to the
            // `addThruSections()' function
            Kernel.GeoOCC.AddCircle(0, 0, 0, 0.5, 1);
            int w1 = Kernel.GeoOCC.AddCurveLoop(new[]{ 1 }, 1);
            Kernel.GeoOCC.AddCircle(0.1, 0.05, 1, 0.1, 2);
            int w2 = Kernel.GeoOCC.AddCurveLoop(new[]{2}, 2);
            Kernel.GeoOCC.AddCircle(-0.1, -0.1, 2, 0.3, 3);
            int w3 = Kernel.GeoOCC.AddCurveLoop(new[]{ 3}, 3);
            Tuple<int, int>[] ov;
            Kernel.GeoOCC.AddThruSections(new []{ 1, 2, 3}, out ov, 1, true);
            Kernel.GeoOCC.Synchronize();

            // We can also force the creation of ruled surfaces:
            Kernel.GeoOCC.AddCircle(2 + 0, 0, 0, 0.5, 11);
            Kernel.GeoOCC.AddCurveLoop(new[]{ 11}, 11);
            Kernel.GeoOCC.AddCircle(2 + 0.1, 0.05, 1, 0.1, 12);
            Kernel.GeoOCC.AddCurveLoop(new[]{ 12}, 12);
            Kernel.GeoOCC.AddCircle(2 - 0.1, -0.1, 2, 0.3, 13);
            Kernel.GeoOCC.AddCurveLoop(new[]{ 13}, 13);
            Kernel.GeoOCC.AddThruSections(new[]{ 11, 12, 13}, out ov, 11, true, true);
            Kernel.GeoOCC.Synchronize();

            // We copy the first volume, and fillet all its edges:
            Tuple<int, int>[] temp = new Tuple<int, int>[] { Tuple.Create(3,1) };
            Kernel.GeoOCC.Copy(temp, out ov);
            Kernel.GeoOCC.Translate(ov, 4, 0, 0);
            Kernel.GeoOCC.Synchronize();
            Tuple<int, int>[] f;
            Kernel.Model.GetBoundary(ov, out f);
            Tuple<int, int>[] e;
            Kernel.Model.GetBoundary(f, out e, false);
            List<int> c = new List<int>();
            for (int i = 0; i < e.Length; i++) c.Add(Math.Abs(e[i].Item2));

            Kernel.GeoOCC.Fillet(new[]{ov[0].Item2}, c.ToArray(), new double[]{ 0.1}, out ov);
            Kernel.GeoOCC.Synchronize();

            // OpenCASCADE also allows general extrusions along a smooth path. Let's first
            // define a spline curve:
            double nturns = 1;
            int npts = 20;
            double r = 1;
            double h = 1 * nturns;
            List<int> p = new List<int>();
            for (int i = 0; i < npts; i++)
            {
                double theta = i * 2 * Math.PI * nturns / npts;
                Kernel.GeoOCC.AddPoint(r * Math.Cos(theta), r * Math.Sin(theta), i * h / npts, 1, 1000 + i);
                p.Add(1000 + i);
            }
            Kernel.GeoOCC.AddSpline(p.ToArray(), 1000);

            // A wire is like a curve loop, but open:
            Kernel.GeoOCC.AddWire(new[]{ 1000}, false, 1000);

            // We define the shape we would like to extrude along the spline (a disk):
            Kernel.GeoOCC.AddDisk(1, 0, 0, 0.2, 0.2, 1000);
            Tuple<int, int>[] tt = new Tuple<int, int>[] { Tuple.Create(2,1000) };
            Kernel.GeoOCC.Rotate(tt, 0, 0, 0, 1, 0, 0, Math.PI / 2);

            // We extrude the disk along the spline to create a pipe:
            Kernel.GeoOCC.AddPipe(tt, 1000, out ov);

            // We delete the source surface, and increase the number of sub-edges for a
            // nicer display of the geometry:
            Kernel.GeoOCC.Remove(tt);
            Kernel.Option.SetNumber("Geometry.NumSubEdges", 1000);

            Kernel.GeoOCC.Synchronize();

            // We can activate the calculation of mesh element sizes based on curvature:
            Kernel.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 1);

            // And we set the minimum number of elements per 2*Pi radians:
            Kernel.Option.SetNumber("Mesh.MinimumElementsPerTwoPi", 20);

            // We can constraint the min and max element sizes to stay within reasonnable
            // values (see `t10.cpp' for more details):
            Kernel.Option.SetNumber("Mesh.CharacteristicLengthMin", 0.001);
            Kernel.Option.SetNumber("Mesh.CharacteristicLengthMax", 0.3);

            Kernel.MeshingKernel.Generate(3);
            Kernel.Write("t19.msh");

            Kernel.FinalizeGmsh();
        }
    }
}
