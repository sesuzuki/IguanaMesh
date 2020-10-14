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
        public static void T13()
        {
            //  Remeshing an STL file without an underlying CAD model
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);
            IguanaGmsh.Model.Add("t13");

            try
            {
                IguanaGmsh.Merge("t13_data.stl");
            }
            catch
            {
                IguanaGmsh.Logger.Write("Could not load STL mesh: bye!");
                IguanaGmsh.FinalizeGmsh();
                return;
            }

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
            IguanaGmsh.Model.Mesh.CreateGeometry();

            Tuple<int,int>[] s;
            IguanaGmsh.Model.GetEntities(out s, 2);

            var sl = s.Select(ss => ss.Item2).ToArray();
            var l = IguanaGmsh.Model.Geo.AddSurfaceLoop(sl);

            var v = IguanaGmsh.Model.Geo.AddVolume(new[] { l });
            IguanaGmsh.Model.Geo.Synchronize();

            bool funny = true;
            int ff = IguanaGmsh.Model.MeshField.Add("MathEval");
            if (funny)
                IguanaGmsh.Model.MeshField.SetString(ff, "F", "2*Sin((x+y)/5) + 3");
            else
                IguanaGmsh.Model.MeshField.SetString(ff, "F", "4");
            IguanaGmsh.Model.MeshField.SetAsBackgroundMesh(ff);

            IguanaGmsh.Model.Mesh.Generate(3);

            IguanaGmsh.Write("t13.msh");
            IguanaGmsh.FinalizeGmsh();
        }
    }
}
