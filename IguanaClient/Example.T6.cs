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
    //  Gmsh C++ tutorial 6
    //
    //  Transfinite meshes
    //
    // -----------------------------------------------------------------------------

    public static partial class Example
    {
        public static void T6()
        {
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t2");

            // Copied from t1.cpp...
            double lc = 1e-2;
            IguanaGmsh.Model.Geo.AddPoint(0, 0, 0, lc, 1);
            IguanaGmsh.Model.Geo.AddPoint(0.1, 0, 0, lc, 2);
            IguanaGmsh.Model.Geo.AddPoint(0.1, 0.3, 0, lc, 3);
            IguanaGmsh.Model.Geo.AddPoint(0, 0.3, 0, lc, 4);
            IguanaGmsh.Model.Geo.AddLine(1, 2, 1);
            IguanaGmsh.Model.Geo.AddLine(3, 2, 2);
            IguanaGmsh.Model.Geo.AddLine(3, 4, 3);
            IguanaGmsh.Model.Geo.AddLine(4, 1, 4);
            IguanaGmsh.Model.Geo.AddCurveLoop(new[]{ 4, 1, -2, 3}, 1);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new[]{ 1}, 1);

            // Delete the surface and the left line, and replace the line with 3 new ones:
            Tuple<int, int>[] temp = new Tuple<int, int>[] { Tuple.Create(2,1), Tuple.Create(1,4) };
            IguanaGmsh.Model.Geo.Remove(temp);

            int p1 = IguanaGmsh.Model.Geo.AddPoint(-0.05, 0.05, 0, lc);
            int p2 = IguanaGmsh.Model.Geo.AddPoint(-0.05, 0.1, 0, lc);
            int l1 = IguanaGmsh.Model.Geo.AddLine(1, p1);
            int l2 = IguanaGmsh.Model.Geo.AddLine(p1, p2);
            int l3 = IguanaGmsh.Model.Geo.AddLine(p2, 4);

            // Create surface:
            IguanaGmsh.Model.Geo.AddCurveLoop(new[]{ 2, -1, l1, l2, l3, -3}, 2);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new[]{ -2}, 1);
            IguanaGmsh.Model.Geo.Synchronize();

            // The `setTransfiniteCurve()' meshing constraints explicitly specifies the
            // location of the nodes on the curve. For example, the following command
            // forces 20 uniformly placed nodes on curve 2 (including the nodes on the two
            // end points):
            IguanaGmsh.Model.Mesh.SetTransfiniteCurve(2, 20);

            // Let's put 20 points total on combination of curves `l1', `l2' and `l3'
            // (beware that the points `p1' and `p2' are shared by the curves, so we do
            // not create 6 + 6 + 10 = 22 nodes, but 20!)
            IguanaGmsh.Model.Mesh.SetTransfiniteCurve(l1, 6);
            IguanaGmsh.Model.Mesh.SetTransfiniteCurve(l2, 6);
            IguanaGmsh.Model.Mesh.SetTransfiniteCurve(l3, 10);

            // Finally, we put 30 nodes following a geometric progression on curve 1
            // (reversed) and on curve 3: Put 30 points following a geometric progression
            IguanaGmsh.Model.Mesh.SetTransfiniteCurve(1, 30, "Progression", -1.2);
            IguanaGmsh.Model.Mesh.SetTransfiniteCurve(3, 30, "Progression", 1.2);

            // The `setTransfiniteSurface()' meshing constraint uses a transfinite
            // interpolation algorithm in the parametric plane of the surface to connect
            // the nodes on the boundary using a structured grid. If the surface has more
            // than 4 corner points, the corners of the transfinite interpolation have to
            // be specified by hand:
            IguanaGmsh.Model.Mesh.SetTransfiniteSurface(1, "Left", new[]{ 1, 2, 3, 4});

            // To create quadrangles instead of triangles, one can use the `setRecombine'
            // constraint:
            IguanaGmsh.Model.Mesh.SetRecombine(2, 1);

            // When the surface has only 3 or 4 points on its boundary the list of corners
            // can be omitted in the `setTransfiniteSurface()' call:
            IguanaGmsh.Model.Geo.AddPoint(0.2, 0.2, 0, 1.0, 7);
            IguanaGmsh.Model.Geo.AddPoint(0.2, 0.1, 0, 1.0, 8);
            IguanaGmsh.Model.Geo.AddPoint(0, 0.3, 0, 1.0, 9);
            IguanaGmsh.Model.Geo.AddPoint(0.25, 0.2, 0, 1.0, 10);
            IguanaGmsh.Model.Geo.AddPoint(0.3, 0.1, 0, 1.0, 11);
            IguanaGmsh.Model.Geo.AddLine(8, 11, 10);
            IguanaGmsh.Model.Geo.AddLine(11, 10, 11);
            IguanaGmsh.Model.Geo.AddLine(10, 7, 12);
            IguanaGmsh.Model.Geo.AddLine(7, 8, 13);
            IguanaGmsh.Model.Geo.AddCurveLoop(new[]{ 13, 10, 11, 12}, 14);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new[]{ 14}, 15);
            IguanaGmsh.Model.Geo.Synchronize();

            for (int i = 10; i <= 13; i++) IguanaGmsh.Model.Mesh.SetTransfiniteCurve(i, 10);
            IguanaGmsh.Model.Mesh.SetTransfiniteSurface(15);

            // The way triangles are generated can be controlled by specifying "Left",
            // "Right" or "Alternate" in `setTransfiniteSurface()' command. Try e.g.
            //
            // gmsh::model::geo::mesh::setTransfiniteSurface(15, "Alternate");

            // Finally we apply an elliptic smoother to the grid to have a more regular
            // mesh:
            IguanaGmsh.Option.SetNumber("Mesh.Smoothing", 100);

            //IguanaGmsh.Model.Geo.Synchronize();
            IguanaGmsh.Model.Mesh.Generate(2);
            IguanaGmsh.Write("t6.msh");

            IguanaGmsh.FinalizeGmsh();

        }
    }
}
