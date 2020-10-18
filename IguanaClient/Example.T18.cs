using Iguana.IguanaMesh.IWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IguanaClient
{
    // -----------------------------------------------------------------------------
    //
    //  Gmsh C++ tutorial 18
    //
    //  Periodic meshes
    //
    // -----------------------------------------------------------------------------

    // Periodic meshing constraints can be imposed on surfaces and curves.

    public static partial class Example
    {
        public static void T18()
        {
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t18");

            // Let's use the OpenCASCADE geometry kernel to build two geometries.

            // The first geometry is very simple: a unit cube with a non-uniform mesh size
            // constraint (set on purpose to be able to verify visually that the
            // periodicity constraint works!):

            IguanaGmsh.Model.GeoOCC.AddBox(0, 0, 0, 1, 1, 1, 1);
            IguanaGmsh.Model.GeoOCC.Synchronize();

            Tuple<int, int>[] ov;
            IguanaGmsh.Model.GetEntities(out ov, 0);
            IguanaGmsh.Model.Mesh.SetSize(ov, 0.1);
            Tuple<int, int>[] tt = new Tuple<int, int>[] { Tuple.Create( 0,1 )};
            IguanaGmsh.Model.Mesh.SetSize(tt, 0.02);

            // To impose that the mesh on surface 2 (the right side of the cube) should
            // match the mesh from surface 1 (the left side), the following periodicity
            // constraint is set:
            List<double> translation = new List<double>(){ 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1};
            IguanaGmsh.Model.Mesh.SetPeriodic(2, new[]{ 2}, new[]{ 1}, translation.ToArray());

            // The periodicity transform is provided as a 4x4 affine transformation
            // matrix, given by row.

            // During mesh generation, the mesh on surface 2 will be created by copying
            // the mesh from surface 1.

            // Multiple periodicities can be imposed in the same way:
            IguanaGmsh.Model.Mesh.SetPeriodic( 2, new[]{ 6}, new[]{ 5}, new double[]{ 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1});
            IguanaGmsh.Model.Mesh.SetPeriodic(2, new[]{ 4}, new[]{ 3}, new double[]{ 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1});

            // For more complicated cases, finding the corresponding surfaces by hand can
            // be tedious, especially when geometries are created through solid
            // modelling. Let's construct a slightly more complicated geometry.

            // We start with a cube and some spheres:
            IguanaGmsh.Model.GeoOCC.AddBox(2, 0, 0, 1, 1, 1, 10);
            double x = 2 - 0.3, y = 0, z = 0;
            IguanaGmsh.Model.GeoOCC.AddSphere(x, y, z, 0.35, 11);
            IguanaGmsh.Model.GeoOCC.AddSphere(x + 1, y, z, 0.35, 12);
            IguanaGmsh.Model.GeoOCC.AddSphere(x, y + 1, z, 0.35, 13);
            IguanaGmsh.Model.GeoOCC.AddSphere(x, y, z + 1, 0.35, 14);
            IguanaGmsh.Model.GeoOCC.AddSphere(x + 1, y + 1, z, 0.35, 15);
            IguanaGmsh.Model.GeoOCC.AddSphere(x, y + 1, z + 1, 0.35, 16);
            IguanaGmsh.Model.GeoOCC.AddSphere(x + 1, y, z + 1, 0.35, 17);
            IguanaGmsh.Model.GeoOCC.AddSphere(x + 1, y + 1, z + 1, 0.35, 18);

            // We first fragment all the volumes, which will leave parts of spheres
            // protruding outside the cube:
            Tuple<int, int>[][] out_map;
            List<Tuple<int, int>> sph = new List<Tuple<int, int>>();
            for (int i = 11; i <= 18; i++) sph.Add(Tuple.Create(3, i));

            tt = new Tuple<int, int>[] { Tuple.Create(3, 10) };
            IguanaGmsh.Model.GeoOCC.Fragment(tt, sph.ToArray(), out ov, out out_map);
            IguanaGmsh.Model.GeoOCC.Synchronize();

            // Ask OpenCASCADE to compute more accurate bounding boxes of entities using
            // the STL mesh:
            IguanaGmsh.Option.SetNumber("Geometry.OCCBoundsUseStl", 1);

            // We then retrieve all the volumes in the bounding box of the original cube,
            // and delete all the parts outside it:
            double eps = 1e-3;
            Tuple<int, int>[] inT;
            IguanaGmsh.Model.GetEntitiesInBoundingBox(2 - eps, -eps, -eps, 2 + 1 + eps, 1 + eps, 1 + eps, out inT, 3);
            List<Tuple<int, int>> listTemp = ov.ToList();
            foreach (Tuple<int,int> it in inT) {
                var it2 = ov.Contains(it);
                if (it2) listTemp.Remove(it);
            }
            ov = listTemp.ToArray();

            IguanaGmsh.Model.RemoveEntities(ov, true); // Delete outside parts recursively

            // We now set a non-uniform mesh size constraint (again to check results
            // visually):
            Tuple<int, int>[] p;
            IguanaGmsh.Model.GetBoundary(inT, out p, false, false, true); // Get all points
            IguanaGmsh.Model.Mesh.SetSize(p, 0.1);
            IguanaGmsh.Model.GetEntitiesInBoundingBox(2 - eps, -eps, -eps, 2 + eps, eps, eps, out p, 0);
            IguanaGmsh.Model.Mesh.SetSize(p, 0.001);

            // We now identify corresponding surfaces on the left and right sides of the
            // geometry automatically.

            // First we get all surfaces on the left:
            Tuple<int, int>[] sxmin;
            IguanaGmsh.Model.GetEntitiesInBoundingBox(2 - eps, -eps, -eps, 2 + eps, 1 + eps, 1 + eps, out sxmin, 2);
            for (int i = 0; i < sxmin.Length; i++)
            {
                // Then we get the bounding box of each left surface
                double xmin, ymin, zmin, xmax, ymax, zmax;
                IguanaGmsh.Model.GetBoundingBox(sxmin[i].Item1, sxmin[i].Item2, out xmin, out ymin, out zmin, out xmax, out ymax, out zmax);
                // We translate the bounding box to the right and look for surfaces inside
                // it:
                Tuple<int, int>[] sxmax;
                IguanaGmsh.Model.GetEntitiesInBoundingBox(xmin - eps + 1, ymin - eps, zmin - eps, xmax + eps + 1, ymax + eps, zmax + eps, out sxmax, 2);
                // For all the matches, we compare the corresponding bounding boxes...
                for (int j = 0; j < sxmax.Length; j++)
                {
                    double xmin2, ymin2, zmin2, xmax2, ymax2, zmax2;
                    IguanaGmsh.Model.GetBoundingBox(sxmax[j].Item1, sxmax[j].Item2, out xmin2, out ymin2, out zmin2, out xmax2, out ymax2, out zmax2);
                    xmin2 -= 1;
                    xmax2 -= 1;
                    // ...and if they match, we apply the periodicity constraint
                    if (Math.Abs(xmin2 - xmin) < eps && Math.Abs(xmax2 - xmax) < eps &&
                       Math.Abs(ymin2 - ymin) < eps && Math.Abs(ymax2 - ymax) < eps &&
                       Math.Abs(zmin2 - zmin) < eps && Math.Abs(zmax2 - zmax) < eps)
                    {
                        IguanaGmsh.Model.Mesh.SetPeriodic(2, new[] { sxmax[j].Item2 }, new[] { sxmin[i].Item2 }, translation.ToArray());
                    }
                }
            }

            IguanaGmsh.Model.Mesh.Generate(3);
            IguanaGmsh.Write("t18.msh");


            IguanaGmsh.FinalizeGmsh();
        }
    }
}
