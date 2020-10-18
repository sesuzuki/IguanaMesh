﻿using Iguana.IguanaMesh.IWrappers;
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
        //  Gmsh C++ tutorial 11
        //
        //  Unstructured quadrangular meshes
        //
        // -----------------------------------------------------------------------------

        public static void T11()
        {
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t11");

            // We have seen in tutorials `t3.cpp' and `t6.cpp' that extruded and
            // transfinite meshes can be "recombined" into quads, prisms or
            // hexahedra. Unstructured meshes can be recombined in the same way. Let's
            // define a simple geometry with an analytical mesh size field:

            int p1 = IguanaGmsh.Model.Geo.AddPoint(-1.25, -.5, 0);
            int p2 = IguanaGmsh.Model.Geo.AddPoint(1.25, -.5, 0);
            int p3 = IguanaGmsh.Model.Geo.AddPoint(1.25, 1.25, 0);
            int p4 = IguanaGmsh.Model.Geo.AddPoint(-1.25, 1.25, 0);

            int l1 = IguanaGmsh.Model.Geo.AddLine(p1, p2);
            int l2 = IguanaGmsh.Model.Geo.AddLine(p2, p3);
            int l3 = IguanaGmsh.Model.Geo.AddLine(p3, p4);
            int l4 = IguanaGmsh.Model.Geo.AddLine(p4, p1);

            int cl = IguanaGmsh.Model.Geo.AddCurveLoop(new[]{ l1, l2, l3, l4});
            int pl = IguanaGmsh.Model.Geo.AddPlaneSurface(new[]{ cl});

            IguanaGmsh.Model.Geo.Synchronize();

            IguanaGmsh.Model.MeshField.Add("MathEval", 1);
            IguanaGmsh.Model.MeshField.SetString(1, "F", "0.01*(1.0+30.*(y-x*x)*(y-x*x) + (1-x)*(1-x))");
            IguanaGmsh.Model.MeshField.SetAsBackgroundMesh(1);

            // To generate quadrangles instead of triangles, we can simply add
            IguanaGmsh.Model.Mesh.SetRecombine(2, pl);

            // If we'd had several surfaces, we could have used the global option
            // "Mesh.RecombineAll":
            //
            // gmsh::option::setNumber("Mesh.RecombineAll", 1);

            // The default recombination algorithm is called "Blossom": it uses a minimum
            // cost perfect matching algorithm to generate fully quadrilateral meshes from
            // triangulations. More details about the algorithm can be found in the
            // following paper: J.-F. Remacle, J. Lambrechts, B. Seny, E. Marchandise,
            // A. Johnen and C. Geuzaine, "Blossom-Quad: a non-uniform quadrilateral mesh
            // generator using a minimum cost perfect matching algorithm", International
            // Journal for Numerical Methods in Engineering 89, pp. 1102-1119, 2012.

            // For even better 2D (planar) quadrilateral meshes, you can try the
            // experimental "Frontal-Delaunay for quads" meshing algorithm, which is a
            // triangulation algorithm that enables to create right triangles almost
            // everywhere: J.-F. Remacle, F. Henrotte, T. Carrier-Baudouin, E. Bechet,
            // E. Marchandise, C. Geuzaine and T. Mouton. A frontal Delaunay quad mesh
            // generator using the L^inf norm. International Journal for Numerical Methods
            // in Engineering, 94, pp. 494-512, 2013. Uncomment the following line to try
            // the Frontal-Delaunay algorithms for quads:
            //
            // gmsh::option::setNumber("Mesh.Algorithm", 8);

            // The default recombination algorithm might leave some triangles in the mesh,
            // if recombining all the triangles leads to badly shaped quads. In such
            // cases, to generate full-quad meshes, you can either subdivide the resulting
            // hybrid mesh (with `Mesh.SubdivisionAlgorithm' set to 1), or use the
            // full-quad recombination algorithm, which will automatically perform a
            // coarser mesh followed by recombination, smoothing and
            // subdivision. Uncomment the following line to try the full-quad algorithm:
            //
            // gmsh::option::setNumber("Mesh.RecombinationAlgorithm", 2); // or 3

            // You can also set the subdivision step alone, with
            //
            // gmsh::option::setNumber("Mesh.SubdivisionAlgorithm", 1);

            IguanaGmsh.Model.Mesh.Generate(2);

            // Note that you could also apply the recombination algorithm and/or the
            // subdivision step explicitly after meshing, as follows:
            //
            // gmsh::model::mesh::generate(2);
            // gmsh::model::mesh::recombine();
            // gmsh::option::setNumber("Mesh.SubdivisionAlgorithm", 1);
            // gmsh::model::mesh::refine();

            IguanaGmsh.Write("T11.msh");


            IguanaGmsh.FinalizeGmsh();

        }
    }
}
