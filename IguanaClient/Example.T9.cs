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
            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);

            Kernel.Model.Add("t9");

            // Let us for example include a three-dimensional scalar view:
            Kernel.Merge("view3.pos");

            // We then set some options for the `Isosurface' plugin (which extracts an
            // isosurface from a 3D scalar view), and run it:
            Kernel.Plugin.SetNumber("Isosurface", "Value", 0.67); // Iso-value level
            Kernel.Plugin.SetNumber("Isosurface", "View", 0); // Source view is View[0]
            Kernel.Plugin.Run("Isosurface"); // Run the plugin!

            // We also set some options for the `CutPlane' plugin (which computes a
            // section of a 3D view using the plane A*x+B*y+C*z+D=0), and then run it:
            Kernel.Plugin.SetNumber("CutPlane", "A", 0);
            Kernel.Plugin.SetNumber("CutPlane", "B", 0.2);
            Kernel.Plugin.SetNumber("CutPlane", "C", 1);
            Kernel.Plugin.SetNumber("CutPlane", "D", 0);
            Kernel.Plugin.SetNumber("CutPlane", "View", 0);
            Kernel.Plugin.Run("CutPlane");

            // Add a title (By convention, for window coordinates a value greater than
            // 99999 represents the center. We could also use `General.GraphicsWidth / 2',
            // but that would only center the string for the current window size.):
            Kernel.Plugin.SetString("Annotate", "Text", "A nice title");
            Kernel.Plugin.SetNumber("Annotate", "X", 1e5);
            Kernel.Plugin.SetNumber("Annotate", "Y", 50);
            Kernel.Plugin.SetString("Annotate", "Font", "Times-BoldItalic");
            Kernel.Plugin.SetNumber("Annotate", "FontSize", 28);
            Kernel.Plugin.SetString("Annotate", "Align", "Center");
            Kernel.Plugin.SetNumber("Annotate", "View", 0);
            Kernel.Plugin.Run("Annotate");

            Kernel.Plugin.SetString("Annotate", "Text", "(and a small subtitle)");
            Kernel.Plugin.SetNumber("Annotate", "Y", 70);
            Kernel.Plugin.SetString("Annotate", "Font", "Times-Roman");
            Kernel.Plugin.SetNumber("Annotate", "FontSize", 12);
            Kernel.Plugin.Run("Annotate");

            // We finish by setting some options:
            Kernel.Option.SetNumber("View[0].Light", 1);
            Kernel.Option.SetNumber("View[0].IntervalsType", 1);
            Kernel.Option.SetNumber("View[0].NbIso", 6);
            Kernel.Option.SetNumber("View[0].SmoothNormals", 1);
            Kernel.Option.SetNumber("View[1].IntervalsType", 2);
            Kernel.Option.SetNumber("View[2].IntervalsType", 2);

            // show the GUI at the end

            Kernel.FinalizeGmsh();
            }
    }
}
