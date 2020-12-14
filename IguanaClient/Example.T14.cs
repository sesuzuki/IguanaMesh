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
            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);

            Kernel.Model.Add("t14");

            // Create an example geometry

            double m = 0.5; // mesh characteristic length
            double h = 2; // geometry height in the z-direction

            Kernel.GeometryKernel.AddPoint(0, 0, 0, m, 1);
            Kernel.GeometryKernel.AddPoint(10, 0, 0, m, 2);
            Kernel.GeometryKernel.AddPoint(10, 10, 0, m, 3);
            Kernel.GeometryKernel.AddPoint(0, 10, 0, m, 4);

            Kernel.GeometryKernel.AddPoint(4, 4, 0, m, 5);
            Kernel.GeometryKernel.AddPoint(6, 4, 0, m, 6);
            Kernel.GeometryKernel.AddPoint(6, 6, 0, m, 7);
            Kernel.GeometryKernel.AddPoint(4, 6, 0, m, 8);

            Kernel.GeometryKernel.AddPoint(2, 0, 0, m, 9);
            Kernel.GeometryKernel.AddPoint(8, 0, 0, m, 10);
            Kernel.GeometryKernel.AddPoint(2, 10, 0, m, 11);
            Kernel.GeometryKernel.AddPoint(8, 10, 0, m, 12);

            Kernel.GeometryKernel.AddLine(1, 9, 1);
            Kernel.GeometryKernel.AddLine(9, 10, 2);
            Kernel.GeometryKernel.AddLine(10, 2, 3);

            Kernel.GeometryKernel.AddLine(2, 3, 4);
            Kernel.GeometryKernel.AddLine(3, 12, 5);
            Kernel.GeometryKernel.AddLine(12, 11, 6);

            Kernel.GeometryKernel.AddLine(11, 4, 7);
            Kernel.GeometryKernel.AddLine(4, 1, 8);
            Kernel.GeometryKernel.AddLine(5, 6, 9);

            Kernel.GeometryKernel.AddLine(6, 7, 10);
            Kernel.GeometryKernel.AddLine(7, 8, 11);
            Kernel.GeometryKernel.AddLine(8, 5, 12);

            Kernel.GeometryKernel.AddCurveLoop(new[]{ 6, 7, 8, 1, 2, 3, 4, 5}, 13);
            Kernel.GeometryKernel.AddCurveLoop(new[]{ 11, 12, 9, 10}, 14);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ 13, 14}, 15);

            Tuple<int, int>[] e;
            Tuple<int, int>[] tt = new Tuple<int, int>[] { Tuple.Create(2,15) };
            Kernel.GeometryKernel.Extrude(tt, 0, 0, h, out e);

            // Create physical groups, which are used to define the domain of the
            // (co)homology computation and the subdomain of the relative (co)homology
            // computation.

            // Whole domain
            int domain_tag = e[1].Item2;
            int domain_physical_tag = 1001;
            Kernel.Model.AddPhysicalGroup(3, new[]{ domain_tag}, domain_physical_tag);
            Kernel.Model.SetPhysicalName(3, domain_physical_tag, "Whole domain");

            // Four "terminals" of the model
            int[] terminal_tags = new[]{e[3].Item2, e[5].Item2, e[7].Item2, e[9].Item2};
            int terminals_physical_tag = 2001;
            Kernel.Model.AddPhysicalGroup(2, terminal_tags, terminals_physical_tag);
            Kernel.Model.SetPhysicalName(2, terminals_physical_tag, "Terminals");

            // Find domain boundary tags
            Tuple<int, int>[] boundary_dimtags;
            tt = new Tuple<int, int>[] { Tuple.Create(3,domain_tag) };
            Kernel.Model.GetBoundary(tt, out boundary_dimtags, false, false);

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
            Kernel.Model.AddPhysicalGroup(2, boundary_tags.ToArray(), boundary_physical_tag);
            Kernel.Model.SetPhysicalName(2, boundary_physical_tag, "Boundary");

            // Complement of the domain surface with respect to the four terminals
            int complement_physical_tag = 2003;
            Kernel.Model.AddPhysicalGroup(2, complement_tags.ToArray(), complement_physical_tag);
            Kernel.Model.SetPhysicalName(2, complement_physical_tag, "Complement");

            Kernel.GeometryKernel.Synchronize();

            // Find bases for relative homology spaces of the domain modulo the four
            // terminals
            Kernel.MeshingKernel.ComputeHomology(new[]{ domain_physical_tag}, new[]{ terminals_physical_tag}, new[]{ 0, 1, 2, 3});

            // Find homology space bases isomorphic to the previous bases: homology spaces
            // modulo the non-terminal domain surface, a.k.a the thin cuts
            Kernel.MeshingKernel.ComputeHomology(new[]{ domain_physical_tag}, new[]{ complement_physical_tag}, new[]{ 0, 1, 2, 3});

            // Find cohomology space bases isomorphic to the previous bases: cohomology
            // spaces of the domain modulo the four terminals, a.k.a the thick cuts
            Kernel.MeshingKernel.ComputeCohomology(new[]{ domain_physical_tag},new[]{ terminals_physical_tag}, new[]{ 0, 1, 2, 3});

            // More examples:
            // gmsh::model::mesh::computeHomology();
            // gmsh::model::mesh::computeHomology({domain_physical_tag});
            // gmsh::model::mesh::computeHomology({domain_physical_tag},
            //                                    {boundary_physical_tag},
            //                                     {0,1,2,3});

            // Generate the mesh and perform the requested homology computations
            Kernel.MeshingKernel.Generate(3);

            Kernel.Write("t14.msh");

            // For more information, see M. Pellikka, S. Suuriniemi, L. Kettunen and
            // C. Geuzaine. Homology and cohomology computation in finite element
            // modeling. SIAM Journal on Scientific Computing 35(5), pp. 1195-1214, 2013.

            Kernel.FinalizeGmsh();
        }
    }
}
