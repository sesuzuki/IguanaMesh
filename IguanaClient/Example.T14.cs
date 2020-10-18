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
    //  Gmsh C++ tutorial 14
    //
    //  Homology and cohomology computation
    //
    // -----------------------------------------------------------------------------

    // Homology computation in Gmsh finds representative chains of (relative)
    // (co)homology space bases using a mesh of a model.  The representative basis
    // chains are stored in the mesh as physical groups of Gmsh, one for each chain.
    public static partial class Example
    {
        public static void T14()
        {
            IguanaGmsh.Initialize();
            IguanaGmsh.Option.SetNumber("General.Terminal", 1);

            IguanaGmsh.Model.Add("t14");

            // Create an example geometry

            double m = 0.5; // mesh characteristic length
            double h = 2; // geometry height in the z-direction

            IguanaGmsh.Model.Geo.AddPoint(0, 0, 0, m, 1);
            IguanaGmsh.Model.Geo.AddPoint(10, 0, 0, m, 2);
            IguanaGmsh.Model.Geo.AddPoint(10, 10, 0, m, 3);
            IguanaGmsh.Model.Geo.AddPoint(0, 10, 0, m, 4);

            IguanaGmsh.Model.Geo.AddPoint(4, 4, 0, m, 5);
            IguanaGmsh.Model.Geo.AddPoint(6, 4, 0, m, 6);
            IguanaGmsh.Model.Geo.AddPoint(6, 6, 0, m, 7);
            IguanaGmsh.Model.Geo.AddPoint(4, 6, 0, m, 8);

            IguanaGmsh.Model.Geo.AddPoint(2, 0, 0, m, 9);
            IguanaGmsh.Model.Geo.AddPoint(8, 0, 0, m, 10);
            IguanaGmsh.Model.Geo.AddPoint(2, 10, 0, m, 11);
            IguanaGmsh.Model.Geo.AddPoint(8, 10, 0, m, 12);

            IguanaGmsh.Model.Geo.AddLine(1, 9, 1);
            IguanaGmsh.Model.Geo.AddLine(9, 10, 2);
            IguanaGmsh.Model.Geo.AddLine(10, 2, 3);

            IguanaGmsh.Model.Geo.AddLine(2, 3, 4);
            IguanaGmsh.Model.Geo.AddLine(3, 12, 5);
            IguanaGmsh.Model.Geo.AddLine(12, 11, 6);

            IguanaGmsh.Model.Geo.AddLine(11, 4, 7);
            IguanaGmsh.Model.Geo.AddLine(4, 1, 8);
            IguanaGmsh.Model.Geo.AddLine(5, 6, 9);

            IguanaGmsh.Model.Geo.AddLine(6, 7, 10);
            IguanaGmsh.Model.Geo.AddLine(7, 8, 11);
            IguanaGmsh.Model.Geo.AddLine(8, 5, 12);

            IguanaGmsh.Model.Geo.AddCurveLoop(new[]{ 6, 7, 8, 1, 2, 3, 4, 5}, 13);
            IguanaGmsh.Model.Geo.AddCurveLoop(new[]{ 11, 12, 9, 10}, 14);
            IguanaGmsh.Model.Geo.AddPlaneSurface(new[]{ 13, 14}, 15);

            Tuple<int, int>[] e;
            Tuple<int, int>[] tt = new Tuple<int, int>[] { Tuple.Create(2,15) };
            IguanaGmsh.Model.Geo.Extrude(tt, 0, 0, h, out e);

            // Create physical groups, which are used to define the domain of the
            // (co)homology computation and the subdomain of the relative (co)homology
            // computation.

            // Whole domain
            int domain_tag = e[1].Item2;
            int domain_physical_tag = 1001;
            IguanaGmsh.Model.AddPhysicalGroup(3, new[]{ domain_tag}, domain_physical_tag);
            IguanaGmsh.Model.SetPhysicalName(3, domain_physical_tag, "Whole domain");

            // Four "terminals" of the model
            int[] terminal_tags = new[]{e[3].Item2, e[5].Item2, e[7].Item2, e[9].Item2};
            int terminals_physical_tag = 2001;
            IguanaGmsh.Model.AddPhysicalGroup(2, terminal_tags, terminals_physical_tag);
            IguanaGmsh.Model.SetPhysicalName(2, terminals_physical_tag, "Terminals");

            // Find domain boundary tags
            Tuple<int, int>[] boundary_dimtags;
            tt = new Tuple<int, int>[] { Tuple.Create(3,domain_tag) };
            IguanaGmsh.Model.GetBoundary(tt, out boundary_dimtags, false, false);

            List<int> boundary_tags = new List<int>();
            List<int> complement_tags = new List<int>();
            foreach (var it in boundary_dimtags)
            {
                complement_tags.Add(it.Item2);
                boundary_tags.Add(it.Item2);
            }

            foreach(var it in terminal_tags)
            {
                var it2 = complement_tags.Contains(it);
                if (it2) complement_tags.Remove(it);
            }

            // Whole domain surface
            int boundary_physical_tag = 2002;
            IguanaGmsh.Model.AddPhysicalGroup(2, boundary_tags.ToArray(), boundary_physical_tag);
            IguanaGmsh.Model.SetPhysicalName(2, boundary_physical_tag, "Boundary");

            // Complement of the domain surface with respect to the four terminals
            int complement_physical_tag = 2003;
            IguanaGmsh.Model.AddPhysicalGroup(2, complement_tags.ToArray(), complement_physical_tag);
            IguanaGmsh.Model.SetPhysicalName(2, complement_physical_tag, "Complement");

            IguanaGmsh.Model.Geo.Synchronize();

            // Find bases for relative homology spaces of the domain modulo the four
            // terminals
            IguanaGmsh.Model.Mesh.ComputeHomology(new[]{ domain_physical_tag}, new[]{ terminals_physical_tag}, new[]{ 0, 1, 2, 3});

            // Find homology space bases isomorphic to the previous bases: homology spaces
            // modulo the non-terminal domain surface, a.k.a the thin cuts
            IguanaGmsh.Model.Mesh.ComputeHomology(new[]{ domain_physical_tag}, new[]{ complement_physical_tag}, new[]{ 0, 1, 2, 3});

            // Find cohomology space bases isomorphic to the previous bases: cohomology
            // spaces of the domain modulo the four terminals, a.k.a the thick cuts
            IguanaGmsh.Model.Mesh.ComputeCohomology(new[]{ domain_physical_tag},new[]{ terminals_physical_tag}, new[]{ 0, 1, 2, 3});

            // More examples:
            // gmsh::model::mesh::computeHomology();
            // gmsh::model::mesh::computeHomology({domain_physical_tag});
            // gmsh::model::mesh::computeHomology({domain_physical_tag},
            //                                    {boundary_physical_tag},
            //                                     {0,1,2,3});

            // Generate the mesh and perform the requested homology computations
            IguanaGmsh.Model.Mesh.Generate(3);

            IguanaGmsh.Write("t14.msh");

            // For more information, see M. Pellikka, S. Suuriniemi, L. Kettunen and
            // C. Geuzaine. Homology and cohomology computation in finite element
            // modeling. SIAM Journal on Scientific Computing 35(5), pp. 1195-1214, 2013.

            IguanaGmsh.FinalizeGmsh();
        }
    }
}
