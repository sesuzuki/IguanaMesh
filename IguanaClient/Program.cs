using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iguana.IguanaMesh.ICreators;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;
using Iguana.IguanaMesh.IGmshWrappers;
using Iguana.IguanaMesh.ITypes.ICollections;

namespace IguanaClient
{
    class Program
    {
        static void Main(string[] args)
        {
            const double lc = 0.5;

            Gmsh.Initialize();

            Gmsh.Option.SetNumber("General.Terminal", 1);

            Gmsh.Model.Add("t1");

            Gmsh.Model.Geo.AddPoint(0, 0, 0, lc, 1);
            Gmsh.Model.Geo.AddPoint(1, 0, 0, lc, 2);
            Gmsh.Model.Geo.AddPoint(1, 3, 0, lc, 3);
            Gmsh.Model.Geo.AddPoint(0, 3, 0, lc, 4);

            Gmsh.Model.Geo.AddLine(1, 2, 1);
            Gmsh.Model.Geo.AddLine(3, 2, 2);
            Gmsh.Model.Geo.AddLine(3, 4, 3);
            Gmsh.Model.Geo.AddLine(4, 1, 4);

            Gmsh.Model.Geo.AddCurveLoop(new int[] { 4, 1, -2, 3 }, 1);

            Gmsh.Model.Geo.AddPlaneSurface(new int[] { 1 }, 1);

            Gmsh.Model.AddPhysicalGroup(2, new int[] { 1 }, 1);

            Gmsh.Model.SetPhysicalName(2, 1, "My surface");

            Gmsh.Model.Geo.Synchronize();

            Gmsh.Model.Mesh.Generate(2);

            Gmsh.Model.Mesh.TryGetIElementCollection();

            Gmsh.FinalizeGmsh();

            /*Gmsh.Initialize();
            Gmsh.Option.SetNumber("General.Terminal", 1);

            Gmsh.Model.Add("t3");

            Gmsh.Model.Geo.AddPoint(0, 0, 0, lc, 1);
            Gmsh.Model.Geo.AddPoint(1, 0, 0, lc, 2);
            Gmsh.Model.Geo.AddPoint(1, 3, 0, lc, 3);
            Gmsh.Model.Geo.AddPoint(0, 3, 0, lc, 4);

            Gmsh.Model.Geo.AddLine(1, 2, 1);
            Gmsh.Model.Geo.AddLine(3, 2, 2);
            Gmsh.Model.Geo.AddLine(3, 4, 3);
            Gmsh.Model.Geo.AddLine(4, 1, 4);

            Gmsh.Model.Geo.AddCurveLoop(new int[] { 4, 1, -2, 3 }, 1);

            Gmsh.Model.Geo.AddPlaneSurface(new int[] { 1 }, 1);

            Gmsh.Model.AddPhysicalGroup(1, new int[] { 1,2,3,4 }, 5);

            int ps = Gmsh.Model.AddPhysicalGroup(2, new int[] { 1 });

            Gmsh.Model.SetPhysicalName(2, ps, "My surface");

            // As in `t2.cpp', we plan to perform an extrusion along the z axis.  But
            // here, instead of only extruding the geometry, we also want to extrude the
            // 2D mesh. This is done with the same `extrude()' function, but by specifying
            // element 'Layers' (2 layers in this case, the first one with 8 subdivisions
            // and the second one with 2 subdivisions, both with a height of h/2). The
            // number of elements for each layer and the (end) height of each layer are
            // specified in two vectors:

            double h = 0.1, angle = 90;
            int[] ov;
            Gmsh.Model.Geo.Extrude(new[] { 2, 1 }, 0, 0, h, out ov, new int[]{8,2}, new double[]{ 0.5, 1}, true);

            // The extrusion can also be performed with a rotation instead of a
            // translation, and the resulting mesh can be recombined into prisms (we use
            // only one layer here, with 7 subdivisions). All rotations are specified by
            // an an axis point (-0.1, 0, 0.1), an axis direction (0, 1, 0), and a
            // rotation angle (-Pi/2):
            Gmsh.Model.Geo.Revolve(new[] {2, 28}, -0.1, 0, 0.1, 0, 1, 0, -Math.PI / 2, out ov, new int[]{7});

            // Using the built-in geometry kernel, only rotations with angles < Pi are
            // supported. To do a full turn, you will thus need to apply at least 3
            // rotations. The OpenCASCADE geometry kernel does not have this limitation.

            // A translation (-2*h, 0, 0) and a rotation ((0,0.15,0.25), (1,0,0), Pi/2)
            // can also be combined to form a "twist".  The last (optional) argument for
            // the extrude() and twist() functions specifies whether the extruded mesh
            // should be recombined or not.
            Gmsh.Model.Geo.Twist(new[]{2, 50}, 0, 0.15, 0.25, -2 * h, 0, 0, 1, 0, 0, angle* Math.PI / 180, out ov, new int[]{10}, default , true);

            Gmsh.Model.Geo.Synchronize();

            // All the extrusion functions return a vector of extruded entities: the "top"
            // of the extruded surface (in `ov[0]'), the newly created volume (in `ov[1]')
            // and the tags of the lateral surfaces (in `ov[2]', `ov[3]', ...).

            // We can then define a new physical volume (with tag 101) to group all the
            // elementary volumes:
            Gmsh.Model.AddPhysicalGroup(3, new int[] { 1, 2, ov[3]}, 101);

            Gmsh.Model.Mesh.Generate(3);
            //Gmsh.Write("t3.msh");

            var element = Gmsh.Model.Mesh.TryGetIElementCollection();


            Gmsh.FinalizeGmsh();*/


            Console.ReadLine();
        }
    }
}
