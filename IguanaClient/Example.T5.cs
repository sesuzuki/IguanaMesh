using Iguana.IguanaMesh.ITypes.ICollections;
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
        public static void T5()
        {

            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            double lcar1 = 1;
            double lcar2 = 1;
            double lcar3 = 1;

            // If we wanted to change these mesh sizes globally (without changing the
            // above definitions), we could give a global scaling factor for all
            // characteristic lengths with e.g.
            //
            // gmsh::option::setNumber("Mesh.CharacteristicLengthFactor", 0.1);
            //
            // Since we pass `argc' and `argv' to `gmsh::initialize()', we can also give
            // the option on the command line with the `-clscale' switch. For example,
            // with:
            //
            // > ./t5.exe -clscale 1
            //
            // this tutorial produces a mesh of approximately 3000 nodes and 14,000
            // tetrahedra. With
            //
            // > ./t5.exe -clscale 0.2
            //
            // the mesh counts approximately 231,000 nodes and 1,360,000 tetrahedra. You
            // can check mesh statistics in the graphical user interface
            // (gmsh::fltk::run()) with the `Tools->Statistics' menu.
            //
            // See `t10.cpp' for more information about mesh sizes.

            // We proceed by defining some elementary entities describing a truncated
            // cube:

            IguanaGmsh.Model.Geo.AddPoint(0.5, 0.5, 0.5, lcar2, 1);
            IguanaGmsh.Model.Geo.AddPoint(0.5, 0.5, 0, lcar1, 2);
            IguanaGmsh.Model.Geo.AddPoint(0, 0.5, 0.5, lcar1, 3);
            IguanaGmsh.Model.Geo.AddPoint(0, 0, 0.5, lcar1, 4);
            IguanaGmsh.Model.Geo.AddPoint(0.5, 0, 0.5, lcar1, 5);
            IguanaGmsh.Model.Geo.AddPoint(0.5, 0, 0, lcar1, 6);
            IguanaGmsh.Model.Geo.AddPoint(0, 0.5, 0, lcar1, 7);
            IguanaGmsh.Model.Geo.AddPoint(0, 1, 0, lcar1, 8);
            IguanaGmsh.Model.Geo.AddPoint(1, 1, 0, lcar1, 9);
            IguanaGmsh.Model.Geo.AddPoint(0, 0, 1, lcar1, 10);
            IguanaGmsh.Model.Geo.AddPoint(0, 1, 1, lcar1, 11);
            IguanaGmsh.Model.Geo.AddPoint(1, 1, 1, lcar1, 12);
            IguanaGmsh.Model.Geo.AddPoint(1, 0, 1, lcar1, 13);
            IguanaGmsh.Model.Geo.AddPoint(1, 0, 0, lcar1, 14);

            IguanaGmsh.Model.Geo.AddLine(8, 9, 1);
            IguanaGmsh.Model.Geo.AddLine(9, 12, 2);
            IguanaGmsh.Model.Geo.AddLine(12, 11, 3);
            IguanaGmsh.Model.Geo.AddLine(11, 8, 4);
            IguanaGmsh.Model.Geo.AddLine(9, 14, 5);
            IguanaGmsh.Model.Geo.AddLine(14, 13, 6);
            IguanaGmsh.Model.Geo.AddLine(13, 12, 7);
            IguanaGmsh.Model.Geo.AddLine(11, 10, 8);
            IguanaGmsh.Model.Geo.AddLine(10, 13, 9);
            IguanaGmsh.Model.Geo.AddLine(10, 4, 10);
            IguanaGmsh.Model.Geo.AddLine(4, 5, 11);
            IguanaGmsh.Model.Geo.AddLine(5, 6, 12);
            IguanaGmsh.Model.Geo.AddLine(6, 2, 13);
            IguanaGmsh.Model.Geo.AddLine(2, 1, 14);
            IguanaGmsh.Model.Geo.AddLine(1, 3, 15);
            IguanaGmsh.Model.Geo.AddLine(3, 7, 16);
            IguanaGmsh.Model.Geo.AddLine(7, 2, 17);
            IguanaGmsh.Model.Geo.AddLine(3, 4, 18);
            IguanaGmsh.Model.Geo.AddLine(5, 1, 19);
            IguanaGmsh.Model.Geo.AddLine(7, 8, 20);
            IguanaGmsh.Model.Geo.AddLine(6, 14, 21);

            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -11, -19, -15, -18 }, 22);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new int[] { 22 }, 23);
            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { 16, 17, 14, 15 }, 24);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new int[] { 24 }, 25);
            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -17, 20, 1, 5, -21, 13 }, 26);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new int[] { 26 }, 27);
            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -4, -1, -2, -3 }, 28);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new int[] { 28 }, 29);
            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -7, 2, -5, -6 }, 30);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new int[] { 30 }, 31);
            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { 6, -9, 10, 11, 12, 21 }, 32);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new int[] { 32 }, 33);
            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { 7, 3, 8, 9 }, 34);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new int[] { 34 }, 35);
            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -10, 18, -16, -20, 4, -8 }, 36);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new int[] { 36 }, 37);
            IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -14, -13, -12, 19 }, 38);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new int[] { 38 }, 39);

            List<int> shells = new List<int>();
            List<int> volumes = new List<int>();

            int sl = IguanaGmsh.Model.Geo.AddSurfaceLoop(new int[] { 35, 31, 29, 37, 33, 23, 39, 25, 27 });
            shells.Add(sl);

            // We create five holes in the cube:
            double x = 0, y = 0.75, z = 0, r = 0.09;
            for (int t = 1; t <= 5; t++)
            {
                x += 0.166;
                z += 0.166;
                CheeseHole(x, y, z, r, lcar3, ref shells, ref volumes);
                IguanaGmsh.Model.AddPhysicalGroup(3, new int[] { volumes[volumes.Count - 1] }, t);
            }

            // The volume of the cube, without the 5 holes, is defined by 6 surface loops:
            // the first surface loop defines the exterior surface; the surface loops
            // other than the first one define holes:
            int ve = IguanaGmsh.Model.Geo.AddVolume(shells.ToArray());

            // Note that using solid modelling with the OpenCASCADE geometry kernel, the
            // same geometry could be built quite differently: see `t16.cpp'.

            // We finally define a physical volume for the elements discretizing the cube,
            // without the holes (for which physical groups were already defined in the
            // `cheeseHole()' function):
            IguanaGmsh.Model.AddPhysicalGroup(3, new int[] { ve }, 10);

            IguanaGmsh.Model.Geo.Synchronize();

            // We could make only part of the model visible to only mesh this subset:
            // std::vector<std::pair<int, int> > ent;
            // gmsh::model::getEntities(ent);
            // gmsh::model::setVisibility(ent, false);
            // gmsh::model::setVisibility({{3, 5}}, true, true);
            // gmsh::option::setNumber("Mesh.MeshOnlyVisible", 1);

            // Meshing algorithms can changed globally using options:
            IguanaGmsh.Option.SetNumber("Mesh.Algorithm", 6); // Frontal-Delaunay for 2D meshes

            // They can also be set for individual surfaces, e.g. for using `MeshAdapt' on
            // surface 1:
            IguanaGmsh.Model.Mesh.SetAlgorithm(2, 33, 1);

            // To generate a curvilinear mesh and optimize it to produce provably valid
            // curved elements (see A. Johnen, J.-F. Remacle and C. Geuzaine. Geometric
            // validity of curvilinear finite elements. Journal of Computational Physics
            // 233, pp. 359-372, 2013; and T. Toulorge, C. Geuzaine, J.-F. Remacle,
            // J. Lambrechts. Robust untangling of curvilinear meshes. Journal of
            // Computational Physics 254, pp. 8-26, 2013), you can uncomment the following
            // lines:
            //
            // gmsh::option::setNumber("Mesh.ElementOrder", 2);
            // gmsh::option::setNumber("Mesh.HighOrderOptimize", 2);

            IguanaGmsh.Model.Mesh.Generate(3);

            // Iguana mesh construction
            IVertexCollection vertices;
            IElementCollection elements;
            HashSet<int> parsedNodes;
            IguanaGmsh.Model.Mesh.TryGetIVertexCollection(out vertices);
            IguanaGmsh.Model.Mesh.TryGetIElementCollection(out elements, out parsedNodes, 3);
            //if (parsedNodes.Count < vertices.Count) vertices.CullUnparsedNodes(parsedNodes);

            Console.WriteLine(elements.Count);

            IguanaGmsh.Write("t5.msh");

            IguanaGmsh.FinalizeGmsh();

        }


        internal static void CheeseHole(double x, double y, double z, double r, double lc, ref List<int> shells, ref List<int> volumes)
        {
            // This function will create a spherical hole in a volume. We don't specify
            // tags manually, and let the functions return them automatically:

            int p1 = IguanaGmsh.Model.Geo.AddPoint(x, y, z, lc);
            int p2 = IguanaGmsh.Model.Geo.AddPoint(x + r, y, z, lc);
            int p3 = IguanaGmsh.Model.Geo.AddPoint(x, y + r, z, lc);
            int p4 = IguanaGmsh.Model.Geo.AddPoint(x, y, z + r, lc);
            int p5 = IguanaGmsh.Model.Geo.AddPoint(x - r, y, z, lc);
            int p6 = IguanaGmsh.Model.Geo.AddPoint(x, y - r, z, lc);
            int p7 = IguanaGmsh.Model.Geo.AddPoint(x, y, z - r, lc);

            int c1 = IguanaGmsh.Model.Geo.AddCircleArc(p2, p1, p7);
            int c2 = IguanaGmsh.Model.Geo.AddCircleArc(p7, p1, p5);
            int c3 = IguanaGmsh.Model.Geo.AddCircleArc(p5, p1, p4);
            int c4 = IguanaGmsh.Model.Geo.AddCircleArc(p4, p1, p2);
            int c5 = IguanaGmsh.Model.Geo.AddCircleArc(p2, p1, p3);
            int c6 = IguanaGmsh.Model.Geo.AddCircleArc(p3, p1, p5);
            int c7 = IguanaGmsh.Model.Geo.AddCircleArc(p5, p1, p6);
            int c8 = IguanaGmsh.Model.Geo.AddCircleArc(p6, p1, p2);
            int c9 = IguanaGmsh.Model.Geo.AddCircleArc(p7, p1, p3);
            int c10 = IguanaGmsh.Model.Geo.AddCircleArc(p3, p1, p4);
            int c11 = IguanaGmsh.Model.Geo.AddCircleArc(p4, p1, p6);
            int c12 = IguanaGmsh.Model.Geo.AddCircleArc(p6, p1, p7);

            int l1 = IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { c5, c10, c4 });
            int l2 = IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { c9, -c5, c1 });
            int l3 = IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { c12, -c8, -c1 });
            int l4 = IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { c8, -c4, c11 });
            int l5 = IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -c10, c6, c3 });
            int l6 = IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -c11, -c3, c7 });
            int l7 = IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -c2, -c7, -c12 });
            int l8 = IguanaGmsh.Model.Geo.AddCurveLoop(new int[] { -c6, -c9, c2 });

            // We need non-plane surfaces to define the spherical holes. Here we use the
            // `gmsh::model::geo::addSurfaceFilling()' function, which can be used for
            // surfaces with 3 or 4 curves on their boundary. With the he built-in kernel,
            // if the curves are circle arcs, ruled surfaces are created; otherwise
            // transfinite interpolation is used.
            //
            // With the OpenCASCADE kernel, `gmsh::model::occ::addSurfaceFilling()' uses a
            // much more general generic surface filling algorithm, creating a BSpline
            // surface passing through an arbitrary number of boundary curves. The
            // `gmsh::model::geo::addThruSections()' allows to create ruled surfaces (see
            // `t19.cpp').

            int s1 = IguanaGmsh.Model.Geo.AddSurfaceFilling(l1);
            int s2 = IguanaGmsh.Model.Geo.AddSurfaceFilling(l2);
            int s3 = IguanaGmsh.Model.Geo.AddSurfaceFilling(l3);
            int s4 = IguanaGmsh.Model.Geo.AddSurfaceFilling(l4);
            int s5 = IguanaGmsh.Model.Geo.AddSurfaceFilling(l5);
            int s6 = IguanaGmsh.Model.Geo.AddSurfaceFilling(l6);
            int s7 = IguanaGmsh.Model.Geo.AddSurfaceFilling(l7);
            int s8 = IguanaGmsh.Model.Geo.AddSurfaceFilling(l8);

            int sl = IguanaGmsh.Model.Geo.AddSurfaceLoop(new int[] { s1, s2, s3, s4, s5, s6, s7, s8 });
            int v = IguanaGmsh.Model.Geo.AddVolume(new int[] { sl });
            shells.Add(sl);
            volumes.Add(v);
        }
    }
}