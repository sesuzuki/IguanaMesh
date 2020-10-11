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
        //  Remeshing an STL file without an underlying CAD model
        public static void T13()
        {
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t13");

            // Let's merge an STL mesh that we would like to remesh (from the parent
            // directory):

            long N = 100;

            var nodes = new List<long>();
            var tris = new List<long>();
            var lin = new List<long>[4];
            var coords = new List<double>();
            var pnt = new long[] { Tag(0, 0, N), Tag(N, 0, N), Tag(N, N, N), Tag(0, N, N) };

            for (int i = 0; i < N + 1; i++)
            {
                for (int j = 0; j < N + 1; j++)
                {
                    nodes.Add(Tag(i, j, N));
                    coords.AddRange(new[] { (double)i / N, (double)j / N, 0.05 * Math.Sin(10 * (double)(i + j) / N) });
                    if (i > 0 && j > 0)
                    {
                        tris.AddRange(new[] { Tag(i - 1, j - 1, N), Tag(i, j - 1, N), Tag(i - 1, j, N) });
                        tris.AddRange(new[] { Tag(i, j - 1, N), Tag(i, j, N), Tag(i - 1, j, N) });
                    }
                    if ((i == 0 || i == N) && j > 0)
                    {
                        var s = i == 0 ? 3 : 1;
                        lin[s] = new List<long>(new[] { Tag(i, j - 1, N), Tag(i, j, N) });
                    }
                    if ((j == 0 || j == N) && i > 0)
                    {
                        var s = (j == 0) ? 0 : 2;
                        lin[s] = new List<long>(new[] { Tag(i - 1, j, N), Tag(i, j, N) });
                    }
                }
            }

            // Create 4 discrete points for the 4 corners of the terrain surface:
            for (int i = 0; i < 4; i++)
            {
                IguanaGmsh.Model.AddDiscreteEntity(0, i + 1);
            }
            IguanaGmsh.Model.SetCoordinates(1, 0, 0, coords[Convert.ToInt32(3 * Tag(0, 0, N) - 1)]);
            IguanaGmsh.Model.SetCoordinates(2, 1, 0, coords[Convert.ToInt32(3 * Tag(N, 0, N) - 1)]);
            IguanaGmsh.Model.SetCoordinates(3, 1, 1, coords[Convert.ToInt32(3 * Tag(N, N, N) - 1)]);
            IguanaGmsh.Model.SetCoordinates(4, 0, 1, coords[Convert.ToInt32(3 * Tag(0, N, N) - 1)]);

            // Create 4 discrete bounding curves, with their boundary points:
            for (int i = 0; i < 4; i++)
            {
                IguanaGmsh.Model.AddDiscreteEntity(1, i + 1, new long[] { i + 1, (i < 3) ? (i + 2) : 1 });
            }

            // Create one discrete surface, with its bounding curves:
            IguanaGmsh.Model.AddDiscreteEntity(2, 1, new long[] { 1, 2, -3, -4 });

            // Add all the nodes on the surface (for simplicity... see below):
            IguanaGmsh.Model.Mesh.AddNodes(2, 1, nodes.ToArray(), coords.ToArray());

            // Add point elements on the 4 points, line elements on the 4 curves, and
            // triangle elements on the surface:
            for (int i = 0; i < 4; i++)
            {
                IguanaGmsh.Model.Mesh.AddElementsByType(i + 1, 15, new long[] { }, new long[] { pnt[i] });
                IguanaGmsh.Model.Mesh.AddElementsByType(i + 1, 1, new long[] { }, lin[i].ToArray());
            }
            IguanaGmsh.Model.Mesh.AddElementsByType(1, 2, new long[] { }, tris.ToArray());

            // Reclassify the nodes on the curves and the points (since we put them all on
            // the surface before with `addNodes' for simplicity)
            IguanaGmsh.Model.Mesh.ReclassifyNodes();

            // Create a geometry for the discrete curves and surfaces, so that we can
            // remesh them later on:
            IguanaGmsh.Model.Mesh.CreateGeometry();

            // We first classify ("color") the surfaces by splitting the original surface
            // along sharp geometrical features. This will create new discrete surfaces,
            // curves and points.


            // Angle between two triangles above which an edge is considered as sharp:
            double angle = 40;

            // For complex geometries, patches can be too complex, too elongated or too
            // large to be parametrized; setting the following option will force the
            // creation of patches that are amenable to reparametrization:
            bool forceParametrizablePatches = false;

            // For open surfaces include the boundary edges in the classification process:
            bool includeBoundary = true;

            // Force curves to be split on given angle:
            double curveAngle = 180;

            IguanaGmsh.Model.Mesh.ClassifySurfaces(angle * Math.PI / 180, includeBoundary, forceParametrizablePatches, curveAngle * Math.PI / 180);

            // Create a geometry for all the discrete curves and surfaces in the mesh, by
            // computing a parametrization for each one
            IguanaGmsh.Model.Mesh.CreateGeometry();

            // Create a volume from all the surfaces
            Tuple<int, int>[] ss;
            IguanaGmsh.Model.GetEntities(out ss, 2);
            List<int> sl = new List<int>();
            foreach(var surf in ss) sl.Add(surf.Item2);
            int l = IguanaGmsh.Model.Geo.AddSurfaceLoop(sl.ToArray());
            IguanaGmsh.Model.Geo.AddVolume(new int[]{ l});

            IguanaGmsh.Model.Geo.Synchronize();

            // We specify element sizes imposed by a size field, just because we can :-)
            /*bool funny = true; // false;
            int f = gmsh::model::mesh::field::add("MathEval");
            if (funny)
                gmsh::model::mesh::field::setString(f, "F", "2*Sin((x+y)/5) + 3");
            else
                gmsh::model::mesh::field::setString(f, "F", "4");
            gmsh::model::mesh::field::setAsBackgroundMesh(f);*/

            IguanaGmsh.Model.Mesh.Generate(3);

            // Launch the GUI to see the results:

            IguanaGmsh.Write("T13.msh");

            IguanaGmsh.FinalizeGmsh();

            }
        }
}
