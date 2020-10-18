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
        //  Gmsh C++ tutorial 9
        //
        //  Plugins
        //
        // -----------------------------------------------------------------------------

        // Plugins can be added to Gmsh in order to extend its capabilities. For
        // example, post-processing plugins can modify views, or create new views based
        // on previously loaded views. Several default plugins are statically linked
        // with Gmsh, e.g. Isosurface, CutPlane, CutSphere, Skin, Transform or Smooth.
        //
        // Plugins can be controlled through the API functions in the `gmsh::plugin'
        // namespace, or from the graphical interface (right click on the view button,
        // then `Plugins').

        public static void T9()
        {
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t9");

            // Let us for example include a three-dimensional scalar view:
            IguanaGmsh.Merge("view3.pos");

            // We then set some options for the `Isosurface' plugin (which extracts an
            // isosurface from a 3D scalar view), and run it:
            IguanaGmsh.Plugin.SetNumber("Isosurface", "Value", 0.67); // Iso-value level
            IguanaGmsh.Plugin.SetNumber("Isosurface", "View", 0); // Source view is View[0]
            IguanaGmsh.Plugin.Run("Isosurface"); // Run the plugin!

            // We also set some options for the `CutPlane' plugin (which computes a
            // section of a 3D view using the plane A*x+B*y+C*z+D=0), and then run it:
            IguanaGmsh.Plugin.SetNumber("CutPlane", "A", 0);
            IguanaGmsh.Plugin.SetNumber("CutPlane", "B", 0.2);
            IguanaGmsh.Plugin.SetNumber("CutPlane", "C", 1);
            IguanaGmsh.Plugin.SetNumber("CutPlane", "D", 0);
            IguanaGmsh.Plugin.SetNumber("CutPlane", "View", 0);
            IguanaGmsh.Plugin.Run("CutPlane");

            // Add a title (By convention, for window coordinates a value greater than
            // 99999 represents the center. We could also use `General.GraphicsWidth / 2',
            // but that would only center the string for the current window size.):
            IguanaGmsh.Plugin.SetString("Annotate", "Text", "A nice title");
            IguanaGmsh.Plugin.SetNumber("Annotate", "X", 1e5);
            IguanaGmsh.Plugin.SetNumber("Annotate", "Y", 50);
            IguanaGmsh.Plugin.SetString("Annotate", "Font", "Times-BoldItalic");
            IguanaGmsh.Plugin.SetNumber("Annotate", "FontSize", 28);
            IguanaGmsh.Plugin.SetString("Annotate", "Align", "Center");
            IguanaGmsh.Plugin.SetNumber("Annotate", "View", 0);
            IguanaGmsh.Plugin.Run("Annotate");

            IguanaGmsh.Plugin.SetString("Annotate", "Text", "(and a small subtitle)");
            IguanaGmsh.Plugin.SetNumber("Annotate", "Y", 70);
            IguanaGmsh.Plugin.SetString("Annotate", "Font", "Times-Roman");
            IguanaGmsh.Plugin.SetNumber("Annotate", "FontSize", 12);
            IguanaGmsh.Plugin.Run("Annotate");

            // We finish by setting some options:
            IguanaGmsh.Option.SetNumber("View[0].Light", 1);
            IguanaGmsh.Option.SetNumber("View[0].IntervalsType", 1);
            IguanaGmsh.Option.SetNumber("View[0].NbIso", 6);
            IguanaGmsh.Option.SetNumber("View[0].SmoothNormals", 1);
            IguanaGmsh.Option.SetNumber("View[1].IntervalsType", 2);
            IguanaGmsh.Option.SetNumber("View[2].IntervalsType", 2);

            // show the GUI at the end

            IguanaGmsh.FinalizeGmsh();
            }
    }
}
