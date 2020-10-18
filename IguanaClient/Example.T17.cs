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
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t17");

            // Create a square
            IguanaGmsh.Model.GeoOCC.AddRectangle(-1, -1, 0, 2, 2);
            IguanaGmsh.Model.GeoOCC.Synchronize();

            // Merge a post-processing view containing the target anisotropic mesh sizes
                IguanaGmsh.Merge("t17_bgmesh.pos");

            // Apply the view as the current background mesh
            int bg_field = IguanaGmsh.Model.MeshField.Add("PostView");
            IguanaGmsh.Model.MeshField.SetAsBackgroundMesh(bg_field);

            // Use bamg
            IguanaGmsh.Option.SetNumber("Mesh.SmoothRatio", 3);
            IguanaGmsh.Option.SetNumber("Mesh.AnisoMax", 1000);
            IguanaGmsh.Option.SetNumber("Mesh.Algorithm", 7);

            IguanaGmsh.Model.Mesh.Generate(2);

            IguanaGmsh.Write("t17.msh");

            IguanaGmsh.FinalizeGmsh();
            }
    }
}
