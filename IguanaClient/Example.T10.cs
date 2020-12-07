using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
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
        //  Gmsh C++ tutorial 10
        //
        //  Mesh size fields
        //
        // -----------------------------------------------------------------------------

        // In addition to specifying target mesh sizes at the points of the geometry
        // (see `t1.cpp') or using a background mesh (see `t7.cpp'), you can use general
        // mesh size "Fields".
        public static void T10()
        {
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t10");

            // Let's create a simple rectangular geometry:
            double lc = .15;
            IguanaGmsh.Model.Geo.AddPoint(0.0, 0.0, 0, lc, 1);
            IguanaGmsh.Model.Geo.AddPoint(1, 0.0, 0, lc, 2);
            IguanaGmsh.Model.Geo.AddPoint(1, 1, 0, lc, 3);
            IguanaGmsh.Model.Geo.AddPoint(0, 1, 0, lc, 4);
            IguanaGmsh.Model.Geo.AddPoint(0.2, .5, 0, lc, 5);

            IguanaGmsh.Model.Geo.AddLine(1, 2, 1);
            IguanaGmsh.Model.Geo.AddLine(2, 3, 2);
            IguanaGmsh.Model.Geo.AddLine(3, 4, 3);
            IguanaGmsh.Model.Geo.AddLine(4, 1, 4);

            int crvTag = IguanaGmsh.Model.Geo.AddCurveLoop(new[]{ 1, 2, 3, 4 });
            IguanaGmsh.Model.Geo.AddPlaneSurface(new[]{ crvTag }, 6);

            IguanaGmsh.Model.Geo.Synchronize();

            // Say we would like to obtain mesh elements with size lc/30 near curve 2 and
            // point 5, and size lc elsewhere. To achieve this, we can use two fields:
            // "Distance", and "Threshold". We first define a Distance field (`Field[1]')
            // on points 5 and on curve 2. This field returns the distance to point 5 and
            // to (100 equidistant points on) curve 2.
            IguanaGmsh.Model.MeshField.Add("Distance", 1);
            IguanaGmsh.Model.MeshField.SetNumbers(1, "NodesList", new double[]{ 5});
            IguanaGmsh.Model.MeshField.SetNumber(1, "NNodesByEdge", 100);
            IguanaGmsh.Model.MeshField.SetNumbers(1, "EdgesList", new double[]{ 2});

            // We then define a `Threshold' field, which uses the return value of the
            // `Distance' field 1 in order to define a simple change in element size
            // depending on the computed distances
            //
            // LcMax -                         /------------------
            //                               /
            //                             /
            //                           /
            // LcMin -o----------------/
            //        |                |       |
            //      Point           DistMin DistMax
            IguanaGmsh.Model.MeshField.Add("Threshold", 2);
            IguanaGmsh.Model.MeshField.SetNumber(2, "IField", 1);
            IguanaGmsh.Model.MeshField.SetNumber(2, "LcMin", lc / 30);
            IguanaGmsh.Model.MeshField.SetNumber(2, "LcMax", lc);
            IguanaGmsh.Model.MeshField.SetNumber(2, "DistMin", 0.15);
            IguanaGmsh.Model.MeshField.SetNumber(2, "DistMax", 0.5);

            // Say we want to modulate the mesh element sizes using a mathematical
            // function of the spatial coordinates. We can do this with the MathEval
            // field:
            IguanaGmsh.Model.MeshField.Add("MathEval", 3);
            IguanaGmsh.Model.MeshField.SetString(3, "F", "Cos(4*3.14*x) * Sin(4*3.14*y) / 10 + 0.101");

            // We could also combine MathEval with values coming from other fields. For
            // example, let's define a `Distance' field around point 1
            IguanaGmsh.Model.MeshField.Add("Distance", 4);
            IguanaGmsh.Model.MeshField.SetNumbers(4, "NodesList", new double[]{ 1});

            // We can then create a `MathEval' field with a function that depends on the
            // return value of the `Distance' field 4, i.e., depending on the distance to
            // point 1 (here using a cubic law, with minimum element size = lc / 100)
            IguanaGmsh.Model.MeshField.Add("MathEval", 5);
            string stream = "F4^3 + " +  lc / 100;
            IguanaGmsh.Model.MeshField.SetString(5, "F", stream);

            // We could also use a `Box' field to impose a step change in element sizes
            // inside a box
            IguanaGmsh.Model.MeshField.Add("Box", 6);
            IguanaGmsh.Model.MeshField.SetNumber(6, "VIn", lc / 15);
            IguanaGmsh.Model.MeshField.SetNumber(6, "VOut", lc);
            IguanaGmsh.Model.MeshField.SetNumber(6, "XMin", 0.3);
            IguanaGmsh.Model.MeshField.SetNumber(6, "XMax", 0.6);
            IguanaGmsh.Model.MeshField.SetNumber(6, "YMin", 0.3);
            IguanaGmsh.Model.MeshField.SetNumber(6, "YMax", 0.6);

            // Many other types of fields are available: see the reference manual for a
            // complete list. You can also create fields directly in the graphical user
            // interface by selecting `Define->Fields' in the `Mesh' module.

            // Finally, let's use the minimum of all the fields as the background mesh
            // field:
            IguanaGmsh.Model.MeshField.Add("Min", 7);
            IguanaGmsh.Model.MeshField.SetNumbers(7, "FieldsList", new double[]{ 2, 3, 5, 6});

            IguanaGmsh.Model.MeshField.SetAsBackgroundMesh(7);

            // To determine the size of mesh elements, Gmsh locally computes the minimum
            // of
            //
            // 1) the size of the model bounding box;
            // 2) if `Mesh.CharacteristicLengthFromPoints' is set, the mesh size specified
            //    at geometrical points;
            // 3) if `Mesh.CharacteristicLengthFromCurvature' is set, the mesh size based
            //    on the curvature and `Mesh.MinimumElementsPerTwoPi';
            // 4) the background mesh field;
            // 5) any per-entity mesh size constraint.
            //
            // This value is then constrained in the interval
            // [`Mesh.CharacteristicLengthMin', `MeshCharacteristicLengthMax'] and
            // multiplied by `Mesh.CharacteristicLengthFactor'.  In addition, boundary
            // mesh sizes (on curves or surfaces) are interpolated inside the enclosed
            // entity (surface or volume, respectively) if the option
            // `Mesh.CharacteristicLengthExtendFromBoundary' is set (which is the case by
            // default).
            //
            // When the element size is fully specified by a background mesh (as it is in
            // this example), it is thus often desirable to set

            IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthExtendFromBoundary", 0);
            IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromPoints", 0);
            IguanaGmsh.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 0);

            // This will prevent over-refinement due to small mesh sizes on the boundary.

            IguanaGmsh.Model.Mesh.Generate(2);
            //IguanaGmsh.Write("t10.msh");

            IguanaGmsh.FLTK.Run();

            IguanaGmsh.FinalizeGmsh();
        }
    }
}
