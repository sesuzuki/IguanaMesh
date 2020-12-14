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
    //  Gmsh C++ tutorial 3
    //
    //  Extruded meshes, options
    //
    // -----------------------------------------------------------------------------

    public static partial class Example
    {
        public static void T3()
        {
            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);

            Kernel.Model.Add("t3");

            // Copied from t1.cpp...
            double lc = 1e-2;
            Kernel.GeometryKernel.AddPoint(0, 0, 0, lc, 1);
            Kernel.GeometryKernel.AddPoint(.1, 0, 0, lc, 2);
            Kernel.GeometryKernel.AddPoint(.1, .3, 0, lc, 3);
            Kernel.GeometryKernel.AddPoint(0, .3, 0, lc, 4);
            Kernel.GeometryKernel.AddLine(1, 2, 1);
            Kernel.GeometryKernel.AddLine(3, 2, 2);
            Kernel.GeometryKernel.AddLine(3, 4, 3);
            Kernel.GeometryKernel.AddLine(4, 1, 4);
            Kernel.GeometryKernel.AddCurveLoop(new[]{ 4, 1, -2, 3}, 1);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ 1}, 1);
            Kernel.Model.AddPhysicalGroup(1, new[]{ 1, 2, 4}, 5);
            int ps = Kernel.Model.AddPhysicalGroup(2,new[]{ 1});
            Kernel.Model.SetPhysicalName(2, ps, "My surface");

            // As in `t2.cpp', we plan to perform an extrusion along the z axis.  But
            // here, instead of only extruding the geometry, we also want to extrude the
            // 2D mesh. This is done with the same `extrude()' function, but by specifying
            // element 'Layers' (2 layers in this case, the first one with 8 subdivisions
            // and the second one with 2 subdivisions, both with a height of h/2). The
            // number of elements for each layer and the (end) height of each layer are
            // specified in two vectors:

            double h = 0.1, angle = 90;
            Tuple<int, int>[] ov;
            Tuple<int, int>[] temp = new Tuple<int, int>[] { Tuple.Create(2,1) };
            Kernel.GeometryKernel.Extrude(temp, 0, 0, h, out ov, new[]{ 8, 2}, new[]{ 0.5, 1});

            // The extrusion can also be performed with a rotation instead of a
            // translation, and the resulting mesh can be recombined into prisms (we use
            // only one layer here, with 7 subdivisions). All rotations are specified by
            // an an axis point (-0.1, 0, 0.1), an axis direction (0, 1, 0), and a
            // rotation angle (-Pi/2):
            temp = new Tuple<int, int>[] { Tuple.Create(2, 28) };
            Kernel.GeometryKernel.Revolve(temp, -0.1, 0, 0.1, 0, 1, 0, -Math.PI / 2, out ov, new[]{ 7});

            // Using the built-in geometry kernel, only rotations with angles < Pi are
            // supported. To do a full turn, you will thus need to apply at least 3
            // rotations. The OpenCASCADE geometry kernel does not have this limitation.

            // A translation (-2*h, 0, 0) and a rotation ((0,0.15,0.25), (1,0,0), Pi/2)
            // can also be combined to form a "twist".  The last (optional) argument for
            // the extrude() and twist() functions specifies whether the extruded mesh
            // should be recombined or not.
            temp = new Tuple<int, int>[] { Tuple.Create(2, 50) };
            Kernel.GeometryKernel.Twist(temp, 0, 0.15, 0.25, -2 * h, 0, 0, 1, 0, 0, angle* Math.PI / 180, out ov, new[]{ 10}, new double[]{}, true);

            Kernel.GeometryKernel.Synchronize();

            // All the extrusion functions return a vector of extruded entities: the "top"
            // of the extruded surface (in `ov[0]'), the newly created volume (in `ov[1]')
            // and the tags of the lateral surfaces (in `ov[2]', `ov[3]', ...).

            // We can then define a new physical volume (with tag 101) to group all the
            // elementary volumes:
            Kernel.Model.AddPhysicalGroup(3, new int[]{ 1, 2, ov[1].Item2}, 101);

            Kernel.MeshingKernel.Generate(3);
            //IguanaGmsh.Write("t3.msh");

            Kernel.Graphics.Run();

            // When the GUI is launched, you can use the `Help->Current Options and
            // Workspace' menu to see the current values of all options. To save the
            // options in a file, use `File->Export->Gmsh Options', or through the api:

            // gmsh::write("t3.opt");

            Kernel.FinalizeGmsh();

        }
    }
}
