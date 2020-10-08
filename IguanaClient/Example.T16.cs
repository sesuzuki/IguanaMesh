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
        public static void T16()
        {
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t16");

            // Let's build the same model as in `t5.cpp', but using constructive solid
            // geometry.

            // We first create two cubes:
            IguanaGmsh.Model.GeoOCC.AddBox(0, 0, 0, 1, 1, 1, 1);
            IguanaGmsh.Model.GeoOCC.AddBox(0, 0, 0, 0.5, 0.5, 0.5, 2);

            // We apply a boolean difference to create the "cube minus one eigth" shape:
            Tuple<int, int>[] ov;
            IguanaGmsh.Model.GeoOCC.Cut(new Tuple<int, int>[] { Tuple.Create(3, 1) }, new Tuple<int, int>[] { Tuple.Create(3, 2) }, out ov, 3);

            // Boolean operations with OpenCASCADE always create new entities. By default
            // the extra arguments `removeObject' and `removeTool' in `cut()' are set to
            // `true', which will delete the original entities.

            // We then create the five spheres:
            double x = 0, y = 0.75, z = 0, r = 0.09;
            Tuple<int, int>[] holes = new Tuple<int, int>[5];
            for (int t = 1; t <= 5; t++)
            {
                x += 0.166;
                z += 0.166;
                IguanaGmsh.Model.GeoOCC.AddSphere(x, y, z, r, -Math.PI / 2, Math.PI / 2, 2 * Math.PI, 3 + t);
                holes[t - 1] = Tuple.Create(3, 3 + t);
            }

            // If we had wanted five empty holes we would have used `cut()' again. Here we
            // want five spherical inclusions, whose mesh should be conformal with the
            // mesh of the cube: we thus use `fragment()', which intersects all volumes in
            // a conformal manner (without creating duplicate interfaces):
            IguanaGmsh.Model.GeoOCC.Fragment(new Tuple<int, int>[] { Tuple.Create(3, 3) }, holes, out ov);

            IguanaGmsh.Model.GeoOCC.Synchronize();

            // When the boolean operation leads to simple modifications of entities, and
            // if one deletes the original entities, Gmsh tries to assign the same tag to
            // the new entities. (This behavior is governed by the
            // `Geometry.OCCBooleanPreserveNumbering' option.)

            // Here the `Physical Volume' definitions can thus be made for the 5 spheres
            // directly, as the five spheres (volumes 4, 5, 6, 7 and 8), which will be
            // deleted by the fragment operations, will be recreated identically (albeit
            // with new surfaces) with the same tags:
            for (int i = 1; i <= 5; i++) IguanaGmsh.Model.AddPhysicalGroup(3, new int[] { 3 + i }, i);

            // The tag of the cube will change though, so we need to access it
            // programmatically:
            IguanaGmsh.Model.AddPhysicalGroup(3, new int[] { ov[ov.Length - 1].Item2 }, 10);

            // Creating entities using constructive solid geometry is very powerful, but
            // can lead to practical issues for e.g. setting mesh sizes at points, or
            // identifying boundaries.

            // To identify points or other bounding entities you can take advantage of the
            // `getEntities()', `getBoundary()' and `getEntitiesInBoundingBox()'
            // functions:

            double lcar1 = 0.1;
            double lcar2 = 0.0005;
            double lcar3 = 0.055;

            // Assign a mesh size to all the points:
            IguanaGmsh.Model.GetEntities(out ov, 0);
            IguanaGmsh.Model.Mesh.SetSize(ov, lcar1);

            // Override this constraint on the points of the five spheres:
            //IguanaGmsh.Model.GetBoundary(holes, out ov, false, false, true);
            IguanaGmsh.Model.Mesh.SetSize(ov, lcar3);

            IguanaGmsh.Model.Mesh.SetSize(ov, lcar2);

            IguanaGmsh.Model.Mesh.Generate(3);

            IguanaGmsh.Write("t16.msh");

            IguanaGmsh.FinalizeGmsh();

        }
    }
}
