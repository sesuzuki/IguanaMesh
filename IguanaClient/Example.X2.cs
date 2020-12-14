using Iguana.IguanaMesh.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IguanaClient
{
    public static partial class Example
    {
        // -----------------------------------------------------------------------------
        //
        //  Gmsh C++ extended tutorial 2
        //
        //  Mesh import, discrete entities, hybrid models, terrain meshing
        //
        // -----------------------------------------------------------------------------

        internal static long N = 100;

        public static void X2()
        {

            /*The API can be used to import a mesh without reading it from a file, by
            creating nodes and elements on the fly and storing them in model
            entities. These model entities can be existing CAD entities, or can be
            discrete entities, entirely defined by the mesh.

            Discrete entities can be reparametrized (see `t13.py') so that they can be
            remeshed later on; and they can also be combined with CAD entities to produce
            hybrid models.

            We combine all these features in this tutorial to perform terrain meshing,
            where the terrain is described by a discrete surface (that we then
            reparametrize) combined with a CAD representation of the underground.*/

            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);
            Kernel.Model.Add("x2");

            var nodes = new List<long>();
            var tris = new List<long>();
            var lin = new List<long>[4];
            var coords = new List<double>();

            for (long i = 0; i < N + 1; i++)
            {
                for (long j = 0; j < N + 1; j++)
                {
                    nodes.Add(Tag(i, j));

                    coords.AddRange(new[] { (double) i / N, (double) j / N, (double) (0.05 * Math.Sin(10 * (i + j) / N)) });

                    if (i > 0 && j > 0)
                    {
                        tris.AddRange(new[] { Tag(i - 1, j - 1), Tag(i, j - 1), Tag(i - 1, j) });
                        tris.AddRange(new[] { Tag(i, j - 1), Tag(i, j), Tag(i - 1, j) });
                    }

                    if ((i == 0 || i == N) && j > 0)
                    {
                        var s = i == 0 ? 3 : 1;
                        if (lin[s] == null) lin[s] = new List<long>();
                        lin[s].AddRange(new[] { Tag(i, j - 1), Tag(i, j) });
                    }

                    if ((j == 0 || j == N) && i > 0)
                    {
                        var s = (j == 0) ? 0 : 2;
                        if (lin[s] == null) lin[s] = new List<long>();
                        lin[s].AddRange(new[] { Tag(i - 1, j), Tag(i, j) });
                    }
                }
            }
            var pnt = new long[] { Tag(0, 0), Tag(N, 0), Tag(N, N), Tag(0, N) };


            // Create 4 discrete points for the 4 corners of the terrain surface:
            for (int i = 0; i < 4; i++)
            {
                int tag1 = Kernel.Model.AddDiscreteEntity(0, i + 1);
            }
            Kernel.Model.SetCoordinates(1, 0, 0, coords[Convert.ToInt32(3 * Tag(0, 0) - 1)]);
            Kernel.Model.SetCoordinates(2, 1, 0, coords[Convert.ToInt32(3 * Tag(N, 0) - 1)]);
            Kernel.Model.SetCoordinates(3, 1, 1, coords[Convert.ToInt32(3 * Tag(N, N) - 1)]);
            Kernel.Model.SetCoordinates(4, 0, 1, coords[Convert.ToInt32(3 * Tag(0, N) - 1)]);

            // Create 4 discrete bounding curves, with their boundary points:
            for (int i = 1; i <= 4; i++)
            {
                int next = i + 1;
                if (i == 4) next = 1;
                int tag2 = Kernel.Model.AddDiscreteEntity(1, i, new int[] { i,next });
            }

            // Create one discrete surface, with its bounding curves:
            int tag3 = Kernel.Model.AddDiscreteEntity(2, 1, new int[] { 1, 2, -3, -4 });

            // Add all the nodes on the surface (for simplicity... see below):
            Kernel.MeshingKernel.AddNodes(2, 1, nodes.ToArray(), coords.ToArray());

            // Add point elements on the 4 points, line elements on the 4 curves, and
            // triangle elements on the surface:
            for (int i = 0; i < 4; i++)
            {
                Kernel.MeshingKernel.AddElementsByType(i + 1, 15, new long[]{}, new long[] { pnt[i] });
                Kernel.MeshingKernel.AddElementsByType(i + 1, 1, new long[] { }, lin[i].ToArray()); ;
            }
            Kernel.MeshingKernel.AddElementsByType(1, 2, new long[] { }, tris.ToArray());

            // Reclassify the nodes on the curves and the points (since we put them all on
            // the surface before with `addNodes' for simplicity)
            Kernel.MeshingKernel.ReclassifyNodes();

            // Create a geometry for the discrete curves and surfaces, so that we can
            // remesh them later on:
            Kernel.MeshingKernel.CreateGeometry();

            // Note that for more complicated meshes, e.g. for on input unstructured STL
            // mesh, we could use `classifySurfaces()' to automatically create the
            // discrete entities and the topology; but we would then have to extract the
            // boundaries afterwards.

            // Create other CAD entities to form one volume below the terrain surface:
            int p1 = Kernel.GeometryKernel.AddPoint(0, 0, -0.5);
            int p2 = Kernel.GeometryKernel.AddPoint(1, 0, -0.5);
            int p3 = Kernel.GeometryKernel.AddPoint(1, 1, -0.5);
            int p4 = Kernel.GeometryKernel.AddPoint(0, 1, -0.5);
            int c1 = Kernel.GeometryKernel.AddLine(p1, p2);
            int c2 = Kernel.GeometryKernel.AddLine(p2, p3);
            int c3 = Kernel.GeometryKernel.AddLine(p3, p4);
            int c4 = Kernel.GeometryKernel.AddLine(p4, p1);
            int c10 = Kernel.GeometryKernel.AddLine(p1, 1);
            int c11 = Kernel.GeometryKernel.AddLine(p2, 2);
            int c12 = Kernel.GeometryKernel.AddLine(p3, 3);
            int c13 = Kernel.GeometryKernel.AddLine(p4, 4);
            int ll1 = Kernel.GeometryKernel.AddCurveLoop(new[] { c1, c2, c3, c4 });
            int s1 = Kernel.GeometryKernel.AddPlaneSurface(new[] { ll1 });
            int ll3 = Kernel.GeometryKernel.AddCurveLoop(new[] { c1, c11, -1, -c10 });
            int s3 = Kernel.GeometryKernel.AddPlaneSurface(new[] { ll3 });
            int ll4 = Kernel.GeometryKernel.AddCurveLoop(new[] { c2, c12, -2, -c11 });
            int s4 = Kernel.GeometryKernel.AddPlaneSurface(new[] { ll4 });
            int ll5 = Kernel.GeometryKernel.AddCurveLoop(new[] { c3, c13, 3, -c12 });
            int s5 = Kernel.GeometryKernel.AddPlaneSurface(new[] { ll5 });
            int ll6 = Kernel.GeometryKernel.AddCurveLoop(new[] { c4, c10, 4, -c13 });
            int s6 = Kernel.GeometryKernel.AddPlaneSurface(new[] { ll6 });
            int sl1 = Kernel.GeometryKernel.AddSurfaceLoop(new[] { s1, s3, s4, s5, s6, 1 });
            int v1 = Kernel.GeometryKernel.AddVolume(new[] { sl1 });

            Kernel.GeometryKernel.Synchronize();

            // Set this to true to build a fully hex mesh:
            bool transfinite = false;

            if (transfinite)
            {
                int NN = 30;
                Tuple<int, int>[] tmp;
                Kernel.Model.GetEntities(out tmp, 1);
                for (int i = 0; i < tmp.Length; i++)
                {
                    Kernel.MeshingKernel.SetTransfiniteCurve(tmp[i].Item2, NN);
                }
                
                Kernel.Model.GetEntities(out tmp, 2);
                for (int i = 0; i < tmp.Length; i++)
                {
                    Kernel.MeshingKernel.SetTransfiniteSurface(tmp[i].Item2);
                    Kernel.MeshingKernel.SetRecombine(tmp[i].Item1, tmp[i].Item2);
                    Kernel.MeshingKernel.SetSmoothing(tmp[i].Item1, tmp[i].Item2, 100);
                }
                Kernel.MeshingKernel.SetTransfiniteVolume(v1);
            }
            else
            {
                Kernel.Option.SetNumber("Mesh.CharacteristicLengthMin", 0.05);
                Kernel.Option.SetNumber("Mesh.CharacteristicLengthMax", 0.05);
            }

            Kernel.MeshingKernel.Generate(3);
            Kernel.Write("x2.msh");
            Kernel.FinalizeGmsh();
        }

        public static long Tag(long i, long j)
        {
            return (N + 1) * i + j + 1;
        }
    }
}
