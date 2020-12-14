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
    //  Gmsh C++ tutorial 6
    //
    //  Transfinite meshes
    //
    // -----------------------------------------------------------------------------

    public static partial class Example
    {
        public static void T6()
        {
            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);

            Kernel.Model.Add("t2");

            // Copied from t1.cpp...
            double lc = 1e-2;
            Kernel.GeometryKernel.AddPoint(0, 0, 0, lc, 1);
            Kernel.GeometryKernel.AddPoint(0.1, 0, 0, lc, 2);
            Kernel.GeometryKernel.AddPoint(0.1, 0.3, 0, lc, 3);
            Kernel.GeometryKernel.AddPoint(0, 0.3, 0, lc, 4);
            Kernel.GeometryKernel.AddLine(1, 2, 1);
            Kernel.GeometryKernel.AddLine(3, 2, 2);
            Kernel.GeometryKernel.AddLine(3, 4, 3);
            Kernel.GeometryKernel.AddLine(4, 1, 4);
            Kernel.GeometryKernel.AddCurveLoop(new[]{ 4, 1, -2, 3}, 1);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ 1}, 1);

            // Delete the surface and the left line, and replace the line with 3 new ones:
            Tuple<int, int>[] temp = new Tuple<int, int>[] { Tuple.Create(2,1), Tuple.Create(1,4) };
            Kernel.GeometryKernel.Remove(temp);

            int p1 = Kernel.GeometryKernel.AddPoint(-0.05, 0.05, 0, lc);
            int p2 = Kernel.GeometryKernel.AddPoint(-0.05, 0.1, 0, lc);
            int l1 = Kernel.GeometryKernel.AddLine(1, p1);
            int l2 = Kernel.GeometryKernel.AddLine(p1, p2);
            int l3 = Kernel.GeometryKernel.AddLine(p2, 4);

            // Create surface:
            Kernel.GeometryKernel.AddCurveLoop(new[]{ 2, -1, l1, l2, l3, -3}, 2);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ -2}, 1);
            Kernel.GeometryKernel.Synchronize();

            // The `setTransfiniteCurve()' meshing constraints explicitly specifies the
            // location of the nodes on the curve. For example, the following command
            // forces 20 uniformly placed nodes on curve 2 (including the nodes on the two
            // end points):
            Kernel.MeshingKernel.SetTransfiniteCurve(2, 20);

            // Let's put 20 points total on combination of curves `l1', `l2' and `l3'
            // (beware that the points `p1' and `p2' are shared by the curves, so we do
            // not create 6 + 6 + 10 = 22 nodes, but 20!)
            Kernel.MeshingKernel.SetTransfiniteCurve(l1, 6);
            Kernel.MeshingKernel.SetTransfiniteCurve(l2, 6);
            Kernel.MeshingKernel.SetTransfiniteCurve(l3, 10);

            // Finally, we put 30 nodes following a geometric progression on curve 1
            // (reversed) and on curve 3: Put 30 points following a geometric progression
            Kernel.MeshingKernel.SetTransfiniteCurve(1, 30, "Progression", -1.2);
            Kernel.MeshingKernel.SetTransfiniteCurve(3, 30, "Progression", 1.2);

            // The `setTransfiniteSurface()' meshing constraint uses a transfinite
            // interpolation algorithm in the parametric plane of the surface to connect
            // the nodes on the boundary using a structured grid. If the surface has more
            // than 4 corner points, the corners of the transfinite interpolation have to
            // be specified by hand:
            Kernel.MeshingKernel.SetTransfiniteSurface(1, "Left", new[]{ 1, 2, 3, 4});

            // To create quadrangles instead of triangles, one can use the `setRecombine'
            // constraint:
            Kernel.MeshingKernel.SetRecombine(2, 1);

            // When the surface has only 3 or 4 points on its boundary the list of corners
            // can be omitted in the `setTransfiniteSurface()' call:
            Kernel.GeometryKernel.AddPoint(0.2, 0.2, 0, 1.0, 7);
            Kernel.GeometryKernel.AddPoint(0.2, 0.1, 0, 1.0, 8);
            Kernel.GeometryKernel.AddPoint(0, 0.3, 0, 1.0, 9);
            Kernel.GeometryKernel.AddPoint(0.25, 0.2, 0, 1.0, 10);
            Kernel.GeometryKernel.AddPoint(0.3, 0.1, 0, 1.0, 11);
            Kernel.GeometryKernel.AddLine(8, 11, 10);
            Kernel.GeometryKernel.AddLine(11, 10, 11);
            Kernel.GeometryKernel.AddLine(10, 7, 12);
            Kernel.GeometryKernel.AddLine(7, 8, 13);
            Kernel.GeometryKernel.AddCurveLoop(new[]{ 13, 10, 11, 12}, 14);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ 14}, 15);
            Kernel.GeometryKernel.Synchronize();

            for (int i = 10; i <= 13; i++) Kernel.MeshingKernel.SetTransfiniteCurve(i, 10);
            Kernel.MeshingKernel.SetTransfiniteSurface(15);

            // The way triangles are generated can be controlled by specifying "Left",
            // "Right" or "Alternate" in `setTransfiniteSurface()' command. Try e.g.
            //
            // gmsh::model::geo::mesh::setTransfiniteSurface(15, "Alternate");

            // Finally we apply an elliptic smoother to the grid to have a more regular
            // mesh:
            Kernel.Option.SetNumber("Mesh.Smoothing", 100);

            //IguanaGmsh.Model.Geo.Synchronize();
            Kernel.MeshingKernel.Generate(2);
            //IguanaGmsh.Write("t6.msh");

            Kernel.Graphics.Run();

            Kernel.FinalizeGmsh();

        }
    }
}
