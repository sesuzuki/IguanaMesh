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
        //  Gmsh C++ tutorial 7
        //
        //  Background meshes
        //
        // -----------------------------------------------------------------------------
        // Mesh sizes can be specified very accurately by providing a background mesh,
        // i.e., a post-processing view that contains the target characteristic lengths.
        public static void T7()
        {
            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);

            Kernel.Model.Add("t7");

            // Create a simple rectangular geometry
            double lc = 1e-2;
            Kernel.GeometryKernel.AddPoint(0, 0, 0, lc, 1);
            Kernel.GeometryKernel.AddPoint(.1, 0, 0, lc, 2);
            Kernel.GeometryKernel.AddPoint(.1, .3, 0, lc, 3);
            Kernel.GeometryKernel.AddPoint(0, .3, 0, lc, 4);
            Kernel.GeometryKernel.AddLine(1, 2, 1);
            Kernel.GeometryKernel.AddLine(3, 2, 2);
            Kernel.GeometryKernel.AddLine(3, 4, 3);
            Kernel.GeometryKernel.AddLine(4, 1, 4);
            Kernel.GeometryKernel.AddCurveLoop(new[] { 4, 1, -2, 3 }, 1);
            Kernel.GeometryKernel.AddPlaneSurface(new[] { 1 }, 1);

            Kernel.GeometryKernel.Synchronize();

            // Merge a post-processing view containing the target mesh sizes
            Kernel.Merge("t7_bgmesh.pos");

            // Add the post-processing view as a new size field
            int bg_field = Kernel.Field.AddMeshField("PostView");

            // Apply the view as the current background mesh
            Kernel.Field.SetMeshFieldAsBackgroundMesh(bg_field);

            // Background meshes are one particular case of general mesh size fields: see
            // `t10.cpp' for more mesh size field examples.

            Kernel.MeshingKernel.Generate(2);
            Kernel.Write("t7.msh");

            // Show the mesh
            // gmsh::fltk::run();

            Kernel.FinalizeGmsh();
        }
    }
}
