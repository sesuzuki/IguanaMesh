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
        //  Gmsh C++ tutorial 8
        //
        //  Post-processing and animations
        //
        // -----------------------------------------------------------------------------
        // In addition to creating geometries and meshes, the C++ API can also be used
        // to manipulate post-processing datasets (called "views" in Gmsh).
        public static void T8()
        {
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t8");

            // We first create a simple geometry
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

            // We merge some post-processing views to work on
            IguanaGmsh.Merge("view1.pos");
            IguanaGmsh.Merge("view1.pos");
            IguanaGmsh.Merge("view4.pos"); // contains 2 views inside

            // Gmsh can read post-processing views in various formats. Here the
            // `view1.pos' and `view4.pos' files are in the Gmsh "parsed" format, which is
            // interpreted by the GEO script parser. The parsed format should only be used
            // for relatively small datasets of course: for larger datasets using e.g. MSH
            // files is much more efficient. Post-processing views can also be created
            // directly from the C++ API.

            // We then set some general options:
            IguanaGmsh.Option.SetNumber("General.Trackball", 0);
            IguanaGmsh.Option.SetNumber("General.RotationX", 0);
            IguanaGmsh.Option.SetNumber("General.RotationY", 0);
            IguanaGmsh.Option.SetNumber("General.RotationZ", 0);

            int[] white = new[] { 255, 255, 255 };
            int[] black = new[] { 0, 0, 0 };
            IguanaGmsh.Option.SetColor("General.Background", white[0], white[1], white[2]);
            IguanaGmsh.Option.SetColor("General.Foreground", black[0], black[1], black[2]);
            IguanaGmsh.Option.SetColor("General.Text", black[0], black[1], black[2]);

            IguanaGmsh.Option.SetNumber("General.Orthographic", 0);
            IguanaGmsh.Option.SetNumber("General.Axes", 0);
            IguanaGmsh.Option.SetNumber("General.SmallAxes", 0);

            // Show the GUI
            IguanaGmsh.FLTK.Initialize();

            // We also set some options for each post-processing view:
            IguanaGmsh.Option.SetNumber("View[0].IntervalsType", 2);
            IguanaGmsh.Option.SetNumber("View[0].OffsetZ", 0.05);
            IguanaGmsh.Option.SetNumber("View[0].RaiseZ", 0);
            IguanaGmsh.Option.SetNumber("View[0].Light", 1);
            IguanaGmsh.Option.SetNumber("View[0].ShowScale", 0);
            IguanaGmsh.Option.SetNumber("View[0].SmoothNormals", 1);

            IguanaGmsh.Option.SetNumber("View[1].IntervalsType", 1);
            // We can't yet set the ColorTable through the API
            // gmsh::option::setColorTable("View[1].ColorTable", "{ Green, Blue }");
            IguanaGmsh.Option.SetNumber("View[1].NbIso", 10);
            IguanaGmsh.Option.SetNumber("View[1].ShowScale", 0);

            IguanaGmsh.Option.SetString("View[2].Name", "Test...");
            IguanaGmsh.Option.SetNumber("View[2].Axes", 1);
            IguanaGmsh.Option.SetNumber("View[2].IntervalsType", 2);
            IguanaGmsh.Option.SetNumber("View[2].Type", 2);
            IguanaGmsh.Option.SetNumber("View[2].IntervalsType", 2);
            IguanaGmsh.Option.SetNumber("View[2].AutoPosition", 0);
            IguanaGmsh.Option.SetNumber("View[2].PositionX", 85);
            IguanaGmsh.Option.SetNumber("View[2].PositionY", 50);
            IguanaGmsh.Option.SetNumber("View[2].Width", 200);
            IguanaGmsh.Option.SetNumber("View[2].Height", 130);

            IguanaGmsh.Option.SetNumber("View[3].Visible", 0);

            // You can save an MPEG movie directly by selecting `File->Export' in the
            // GUI. Several predefined animations are setup, for looping on all the time
            // steps in views, or for looping between views.

            // But the API can be used to build much more complex animations, by changing
            // options at run-time and re-rendering the graphics. Each frame can then be
            // saved to disk as an image, and multiple frames can be encoded to form a
            // movie. Below is an example of such a custom animation.

            int t = 0; // Initial step

            for (int num = 1; num <= 3; num++)
            {
                double nbt;
                IguanaGmsh.Option.GetNumber("View[0].NbTimeStep", out nbt);
                t = (t < nbt - 1) ? t + 1 : 0;

                // Set time step
                IguanaGmsh.Option.SetNumber("View[0].TimeStep", t);
                IguanaGmsh.Option.SetNumber("View[1].TimeStep", t);
                IguanaGmsh.Option.SetNumber("View[2].TimeStep", t);
                IguanaGmsh.Option.SetNumber("View[3].TimeStep", t);

                double max;
                IguanaGmsh.Option.GetNumber("View[0].Max", out max);
                IguanaGmsh.Option.SetNumber("View[0].RaiseZ", 0.01 / max * t);

                if (num == 3)
                {
                    double mw;
                    IguanaGmsh.Option.GetNumber("General.MenuWidth", out mw);
                    IguanaGmsh.Option.SetNumber("General.GraphicsWidth", mw + 640);
                    IguanaGmsh.Option.SetNumber("General.GraphicsHeight", 480);
                }

                int frames = 50;
                for (int num2 = 1; num2 <= frames; num2++)
                {
                    // Incrementally rotate the scene
                    double rotx;
                    IguanaGmsh.Option.GetNumber("General.RotationX", out rotx);
                    IguanaGmsh.Option.SetNumber("General.RotationX", rotx + 10);
                    IguanaGmsh.Option.SetNumber("General.RotationY", (rotx + 10) / 3.0);
                    double rotz;
                    IguanaGmsh.Option.GetNumber("General.RotationZ", out rotz);
                    IguanaGmsh.Option.SetNumber("General.RotationZ", rotz + 0.1);

                    // Draw the scene
                    IguanaGmsh.GraphicsDraw();

                    if (num == 3)
                    {
                        // Uncomment the following lines to save each frame to an image file
                        // gmsh::write("t2-" + std::to_string(num2) + ".gif");
                        // gmsh::write("t2-" + std::to_string(num2) + ".ppm");
                        // gmsh::write("t2-" + std::to_string(num2) + ".jpg");
                    }
                }

                if (num == 3)
                {
                    // Here we could make a system call to generate a movie...
                }
            }

            IguanaGmsh.FLTK.Run();
            IguanaGmsh.FinalizeGmsh();

        }
    }
}
