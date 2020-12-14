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
    //  Gmsh C++ tutorial 17
    //
    //  Anisotropic background mesh
    //
    // -----------------------------------------------------------------------------

    // As seen in `t7.cpp', characteristic lengths can be specified very accurately
    // by providing a background mesh, i.e., a post-processing view that contains
    // the target mesh sizes.

    // Here, the background mesh is represented as a metric tensor field defined on
    // a square. One should use bamg as 2d mesh generator to enable anisotropic
    // meshes in 2D.

    public static partial class Example
    {
        public static void T17()
        {
            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);

            Kernel.Model.Add("t17");

            // Create a square
            Kernel.GeoOCC.AddRectangle(-1, -1, 0, 2, 2);
            Kernel.GeoOCC.Synchronize();

            // Merge a post-processing view containing the target anisotropic mesh sizes
            Kernel.Merge("t17_bgmesh.pos");

            // Apply the view as the current background mesh
            int bg_field = Kernel.Field.AddMeshField("PostView");
            Kernel.Field.SetMeshFieldAsBackgroundMesh(bg_field);

            // Use bamg
            Kernel.Option.SetNumber("Mesh.SmoothRatio", 3);
            Kernel.Option.SetNumber("Mesh.AnisoMax", 1000);
            Kernel.Option.SetNumber("Mesh.Algorithm", 7);

            Kernel.MeshingKernel.Generate(2);

            Kernel.Write("t17.msh");

            Kernel.FinalizeGmsh();
            }
    }
}
