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
            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);

            Kernel.Model.Add("t10");

            // Let's create a simple rectangular geometry:
            double lc = .15;
            Kernel.GeometryKernel.AddPoint(0.0, 0.0, 0, lc, 1);
            Kernel.GeometryKernel.AddPoint(1, 0.0, 0, lc, 2);
            Kernel.GeometryKernel.AddPoint(1, 1, 0, lc, 3);
            Kernel.GeometryKernel.AddPoint(0, 1, 0, lc, 4);
            Kernel.GeometryKernel.AddPoint(0.2, .5, 0, lc, 5);

            Kernel.GeometryKernel.AddLine(1, 2, 1);
            Kernel.GeometryKernel.AddLine(2, 3, 2);
            Kernel.GeometryKernel.AddLine(3, 4, 3);
            Kernel.GeometryKernel.AddLine(4, 1, 4);

            int crvTag = Kernel.GeometryKernel.AddCurveLoop(new[]{ 1, 2, 3, 4 });
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ crvTag }, 6);

            Kernel.GeometryKernel.Synchronize();

            // Say we would like to obtain mesh elements with size lc/30 near curve 2 and
            // point 5, and size lc elsewhere. To achieve this, we can use two fields:
            // "Distance", and "Threshold". We first define a Distance field (`Field[1]')
            // on points 5 and on curve 2. This field returns the distance to point 5 and
            // to (100 equidistant points on) curve 2.
            Kernel.Field.AddMeshField("Distance", 1);
            Kernel.Field.SetMeshFieldOptionNumbers(1, "NodesList", new double[]{ 5});
            Kernel.Field.SetMeshFieldOptionNumber(1, "NNodesByEdge", 100);
            Kernel.Field.SetMeshFieldOptionNumbers(1, "EdgesList", new double[]{ 2});

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
            Kernel.Field.AddMeshField("Threshold", 2);
            Kernel.Field.SetMeshFieldOptionNumber(2, "IField", 1);
            Kernel.Field.SetMeshFieldOptionNumber(2, "LcMin", lc / 30);
            Kernel.Field.SetMeshFieldOptionNumber(2, "LcMax", lc);
            Kernel.Field.SetMeshFieldOptionNumber(2, "DistMin", 0.15);
            Kernel.Field.SetMeshFieldOptionNumber(2, "DistMax", 0.5);

            // Say we want to modulate the mesh element sizes using a mathematical
            // function of the spatial coordinates. We can do this with the MathEval
            // field:
            Kernel.Field.AddMeshField("MathEval", 3);
            Kernel.Field.SetMeshFieldOptionString(3, "F", "Cos(4*3.14*x) * Sin(4*3.14*y) / 10 + 0.101");

            // We could also combine MathEval with values coming from other fields. For
            // example, let's define a `Distance' field around point 1
            Kernel.Field.AddMeshField("Distance", 4);
            Kernel.Field.SetMeshFieldOptionNumbers(4, "NodesList", new double[]{ 1});

            // We can then create a `MathEval' field with a function that depends on the
            // return value of the `Distance' field 4, i.e., depending on the distance to
            // point 1 (here using a cubic law, with minimum element size = lc / 100)
            Kernel.Field.AddMeshField("MathEval", 5);
            string stream = "F4^3 + " +  lc / 100;
            Kernel.Field.SetMeshFieldOptionString(5, "F", stream);

            // We could also use a `Box' field to impose a step change in element sizes
            // inside a box
            Kernel.Field.AddMeshField("Box", 6);
            Kernel.Field.SetMeshFieldOptionNumber(6, "VIn", lc / 15);
            Kernel.Field.SetMeshFieldOptionNumber(6, "VOut", lc);
            Kernel.Field.SetMeshFieldOptionNumber(6, "XMin", 0.3);
            Kernel.Field.SetMeshFieldOptionNumber(6, "XMax", 0.6);
            Kernel.Field.SetMeshFieldOptionNumber(6, "YMin", 0.3);
            Kernel.Field.SetMeshFieldOptionNumber(6, "YMax", 0.6);

            // Many other types of fields are available: see the reference manual for a
            // complete list. You can also create fields directly in the graphical user
            // interface by selecting `Define->Fields' in the `Mesh' module.

            // Finally, let's use the minimum of all the fields as the background mesh
            // field:
            Kernel.Field.AddMeshField("Min", 7);
            Kernel.Field.SetMeshFieldOptionNumbers(7, "FieldsList", new double[]{ 2, 3, 5, 6});

            Kernel.Field.SetMeshFieldAsBackgroundMesh(7);

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

            Kernel.Option.SetNumber("Mesh.CharacteristicLengthExtendFromBoundary", 0);
            Kernel.Option.SetNumber("Mesh.CharacteristicLengthFromPoints", 0);
            Kernel.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 0);

            // This will prevent over-refinement due to small mesh sizes on the boundary.

            Kernel.MeshingKernel.Generate(2);
            //IguanaGmsh.Write("t10.msh");

            Kernel.Graphics.Run();

            Kernel.FinalizeGmsh();
        }
    }
}
