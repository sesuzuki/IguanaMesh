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
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t7");

            // Create a simple rectangular geometry
            double lc = 1e-2;
            IguanaGmsh.Model.Geo.AddPoint(0, 0, 0, lc, 1);
            IguanaGmsh.Model.Geo.AddPoint(.1, 0, 0, lc, 2);
            IguanaGmsh.Model.Geo.AddPoint(.1, .3, 0, lc, 3);
            IguanaGmsh.Model.Geo.AddPoint(0, .3, 0, lc, 4);
            IguanaGmsh.Model.Geo.AddLine(1, 2, 1);
            IguanaGmsh.Model.Geo.AddLine(3, 2, 2);
            IguanaGmsh.Model.Geo.AddLine(3, 4, 3);
            IguanaGmsh.Model.Geo.AddLine(4, 1, 4);
            IguanaGmsh.Model.Geo.AddCurveLoop(new[] { 4, 1, -2, 3 }, 1);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new[] { 1 }, 1);

            IguanaGmsh.Model.Geo.Synchronize();

            // Merge a post-processing view containing the target mesh sizes
            IguanaGmsh.Merge("t7_bgmesh.pos");

            // Add the post-processing view as a new size field
            int bg_field = IguanaGmsh.Model.MeshField.Add("PostView");

            // Apply the view as the current background mesh
            IguanaGmsh.Model.MeshField.SetAsBackgroundMesh(bg_field);

            // Background meshes are one particular case of general mesh size fields: see
            // `t10.cpp' for more mesh size field examples.

            IguanaGmsh.Model.Mesh.Generate(2);
            IguanaGmsh.Write("t7.msh");

            // Show the mesh
            // gmsh::fltk::run();

            IguanaGmsh.FinalizeGmsh();
        }
    }
}
