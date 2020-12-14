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
    //  Gmsh C++ tutorial 13
    //
    //  Remeshing an STL file without an underlying CAD model
    //
    // -----------------------------------------------------------------------------
    public static partial class Example
    {
        public static void T13()
        {
            //  Remeshing an STL file without an underlying CAD model
            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);
            Kernel.Model.Add("t13");

            try
            {
                Kernel.Merge("t13_data.stl");
            }
            catch
            {
                Kernel.Logger.WriteLogger("Could not load STL mesh: bye!");
                Kernel.FinalizeGmsh();
                return;
            }

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

            Kernel.MeshingKernel.ClassifySurfaces(angle * Math.PI / 180, includeBoundary, forceParametrizablePatches, curveAngle * Math.PI / 180);
            Kernel.MeshingKernel.CreateGeometry();

            Tuple<int,int>[] s;
            Kernel.Model.GetEntities(out s, 2);

            var sl = s.Select(ss => ss.Item2).ToArray();
            var l = Kernel.GeometryKernel.AddSurfaceLoop(sl);

            var v = Kernel.GeometryKernel.AddVolume(new[] { l });
            Kernel.GeometryKernel.Synchronize();

            bool funny = true;
            int ff = Kernel.Field.AddMeshField("MathEval");
            if (funny)
                Kernel.Field.SetMeshFieldOptionString(ff, "F", "2*Sin((x+y)/5) + 3");
            else
                Kernel.Field.SetMeshFieldOptionString(ff, "F", "4");
            Kernel.Field.SetMeshFieldAsBackgroundMesh(ff);

            Kernel.MeshingKernel.Generate(3);

            Kernel.Write("t13.msh");
            Kernel.FinalizeGmsh();
        }
    }
}
