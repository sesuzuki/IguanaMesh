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
    //  Gmsh C++ tutorial 20
    //
    //  STEP import and manipulation, geometry partitioning
    //
    // -----------------------------------------------------------------------------

    // The OpenCASCADE geometry kernel allows to import STEP files and to modify
    // them. In this tutorial we will load a STEP geometry and partition it into
    // slices.

    public static partial class Example
    {
        public static void T20()
        {
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t20");

            // Load a STEP file (using `importShapes' instead of `merge' allows to
            // directly retrieve the tags of the highest dimensional imported entities):
            Tuple<int, int>[] v;
            IguanaGmsh.Model.GeoOCC.ImportShapes("t20_data.step", out v);


            // If we had specified
            //
            // gmsh::option::setString("OCCTargetUnit", "M");
            //
            // before merging the STEP file, OpenCASCADE would have converted the units to
            // meters (instead of the default, which is millimeters).

            // Get the bounding box of the volume:
            IguanaGmsh.Model.GeoOCC.Synchronize();
            double xmin, ymin, zmin, xmax, ymax, zmax;
            IguanaGmsh.Model.GetBoundingBox(v[0].Item1, v[0].Item2, out xmin, out ymin, out zmin, out xmax, out ymax, out zmax);

            // Note that the synchronization step can be avoided in Gmsh >= 4.6 by using
            // `gmsh::model::occ::getBoundingBox()' instead of
            // `gmsh::model::getBoundingBox()'.

            // We want to slice the model into N slices, and either keep the volume slices
            // or just the surfaces obtained by the cutting:

            int N = 5; // Number of slices
            string dir = "X"; // Direction: "X", "Y" or "Z"
            bool surf = false; // Keep only surfaces?

            double dx = (xmax - xmin);
            double dy = (ymax - ymin);
            double dz = (zmax - zmin);
            double L = (dir == "X") ? dz : dx;
            double H = (dir == "Y") ? dz : dy;

            // Create the first cutting plane
            List<Tuple<int, int>> s = new List<Tuple<int,int>>();
            s.Add(Tuple.Create( 2, IguanaGmsh.Model.GeoOCC.AddRectangle(xmin, ymin, zmin, L, H)));
            if (dir == "X")
            {
                IguanaGmsh.Model.GeoOCC.Rotate(new[]{ s[0]}, xmin, ymin, zmin, 0, 1, 0, -Math.PI / 2);
            }
            else if (dir == "Y")
            {
                IguanaGmsh.Model.GeoOCC.Rotate(new[]{ s[0]}, xmin, ymin, zmin, 1, 0, 0, Math.PI / 2);
            }
            double tx = (dir == "X") ? dx / N : 0;
            double ty = (dir == "Y") ? dy / N : 0;
            double tz = (dir == "Z") ? dz / N : 0;
            IguanaGmsh.Model.GeoOCC.Translate(new[]{ s[0]}, tx, ty, tz);

            // Create the other cutting planes:
            Tuple<int, int>[] tmp;
            for (int i = 1; i < N - 1; i++)
            {
                IguanaGmsh.Model.GeoOCC.Copy(new[]{ s[0]}, out tmp);
                s.Add(tmp[0]);
                IguanaGmsh.Model.GeoOCC.Translate(new[]{ s[s.Count-1]}, i* tx, i *ty, i* tz);
            }

            // Fragment (i.e. intersect) the volume with all the cutting planes:
            Tuple<int, int>[] ov;
            Tuple<int, int>[][] ovv;
            IguanaGmsh.Model.GeoOCC.Fragment(v, s.ToArray(), out ov, out ovv);

            // Now remove all the surfaces (and their bounding entities) that are not on
            // the boundary of a volume, i.e. the parts of the cutting planes that "stick
            // out" of the volume:
            IguanaGmsh.Model.GeoOCC.Synchronize();
            IguanaGmsh.Model.GetEntities(out tmp, 2);
            IguanaGmsh.Model.GeoOCC.Remove(tmp, true);

            // The previous synchronization step can be avoided in Gmsh >= 4.6 by using
            // `gmsh::model::occ::getEntities()' instead of `gmsh::model::getEntities()'.

            IguanaGmsh.Model.GeoOCC.Synchronize();

            if (surf)
            {
                // If we want to only keep the surfaces, retrieve the surfaces in bounding
                // boxes around the cutting planes...
                double eps = 1e-4;
                s = new List<Tuple<int, int>>();
                for (int i = 1; i < N; i++)
                {
                    double xx = (dir == "X") ? xmin : xmax;
                    double yy = (dir == "Y") ? ymin : ymax;
                    double zz = (dir == "Z") ? zmin : zmax;
                    Tuple<int, int>[] e;
                    IguanaGmsh.Model.GetEntitiesInBoundingBox(
                      xmin - eps + i * tx, ymin - eps + i * ty, zmin - eps + i * tz,
                      xx + eps + i * tx, yy + eps + i * ty, zz + eps + i * tz, out e, 2);
                    s.InsertRange(s.Count - 1, e);
                }
                // ...and remove all the other entities (here directly in the model, as we
                // won't modify any OpenCASCADE entities later on):
                Tuple<int, int>[] dels;
                IguanaGmsh.Model.GetEntities(out dels, 2);
                var list = dels.ToList();
                foreach (Tuple<int,int> it in s)
                {
                    var it2 = list.Contains(it);
                    if (it2) list.Remove(it);
                }
                dels = list.ToArray();
                IguanaGmsh.Model.GetEntities(out tmp, 3);
                IguanaGmsh.Model.RemoveEntities(tmp);
                IguanaGmsh.Model.RemoveEntities(dels);
                IguanaGmsh.Model.GetEntities(out tmp, 1);
                IguanaGmsh.Model.RemoveEntities(tmp);
                IguanaGmsh.Model.GetEntities(out tmp, 0);
                IguanaGmsh.Model.RemoveEntities(tmp);
            }

            // Finally, let's specify a global mesh size and mesh the partitioned model:
            IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthMin", 3);
            IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthMax", 3);
            IguanaGmsh.Model.Mesh.Generate(3);
            IguanaGmsh.Write("t20.msh");

            // Show the result:

            IguanaGmsh.FinalizeGmsh();
        }
    }
}
