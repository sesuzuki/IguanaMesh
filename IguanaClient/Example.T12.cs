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
        //  Gmsh C++ tutorial 12
        //
        //  Cross-patch meshing with compounds
        //
        // -----------------------------------------------------------------------------
        // "Compound" meshing constraints allow to generate meshes across surface
        // boundaries, which can be useful e.g. for imported CAD models (e.g. STEP) with
        // undesired small features.

        // When a `setCompound()' meshing constraint is given, at mesh generation time
        // Gmsh
        //  1. meshes the underlying elementary geometrical entities, individually
        //  2. creates a discrete entity that combines all the individual meshes
        //  3. computes a discrete parametrization (i.e. a piece-wise linear mapping)
        //     on this discrete entity
        //  4. meshes the discrete entity using this discrete parametrization instead
        //     of the underlying geometrical description of the underlying elementary
        //     entities making up the compound
        //  5. optionally, reclassifies the mesh elements and nodes on the original
        //     entities

        // Step 3. above can only be performed if the mesh resulting from the
        // combination of the individual meshes can be reparametrized, i.e. if the shape
        // is "simple enough". If the shape is not amenable to reparametrization, you
        // should create a full mesh of the geometry and first re-classify it to
        // generate patches amenable to reparametrization (see `t13.cpp').

        // The mesh of the individual entities performed in Step 1. should usually be
        // finer than the desired final mesh; this can be controlled with the
        // `Mesh.CompoundCharacteristicLengthFactor' option.

        // The optional reclassification on the underlying elementary entities in Step
        // 5. is governed by the `Mesh.CompoundClassify' option.
        public static void T12()
        {

            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);

            Kernel.Model.Add("t12");

            double lc = 0.1;

            Kernel.GeometryKernel.AddPoint(0, 0, 0, lc, 1);
            Kernel.GeometryKernel.AddPoint(1, 0, 0, lc, 2);
            Kernel.GeometryKernel.AddPoint(1, 1, 0.5, lc, 3);
            Kernel.GeometryKernel.AddPoint(0, 1, 0.4, lc, 4);
            Kernel.GeometryKernel.AddPoint(0.3, 0.2, 0, lc, 5);
            Kernel.GeometryKernel.AddPoint(0, 0.01, 0.01, lc, 6);
            Kernel.GeometryKernel.AddPoint(0, 0.02, 0.02, lc, 7);
            Kernel.GeometryKernel.AddPoint(1, 0.05, 0.02, lc, 8);
            Kernel.GeometryKernel.AddPoint(1, 0.32, 0.02, lc, 9);

            Kernel.GeometryKernel.AddLine(1, 2, 1);
            Kernel.GeometryKernel.AddLine(2, 8, 2);
            Kernel.GeometryKernel.AddLine(8, 9, 3);
            Kernel.GeometryKernel.AddLine(9, 3, 4);
            Kernel.GeometryKernel.AddLine(3, 4, 5);
            Kernel.GeometryKernel.AddLine(4, 7, 6);
            Kernel.GeometryKernel.AddLine(7, 6, 7);
            Kernel.GeometryKernel.AddLine(6, 1, 8);
            Kernel.GeometryKernel.AddSpline(new int[] { 7, 5, 9 }, 9);
            Kernel.GeometryKernel.AddLine(6, 8, 10);

            Kernel.GeometryKernel.AddCurveLoop(new int[] { 5, 6, 9, 4 }, 11);
            Kernel.GeometryKernel.AddSurfaceFilling(11, -1, 1);

            Kernel.GeometryKernel.AddCurveLoop(new int[] { -9, 3, 10, 7 }, 13);
            Kernel.GeometryKernel.AddSurfaceFilling(13, -1, 5);

            Kernel.GeometryKernel.AddCurveLoop(new int[] { -10, 2, 1, 8 }, 15);
            Kernel.GeometryKernel.AddSurfaceFilling(15, -1, 10);

            Kernel.GeometryKernel.Synchronize();

            // Treat curves 2, 3 and 4 as a single curve when meshing (i.e. mesh across
            // points 6 and 7)
            Kernel.MeshingKernel.SetCompound(1, new int[] { 2, 3, 4 });

            // Idem with curves 6, 7 and 8
            Kernel.MeshingKernel.SetCompound(1, new int[] { 6, 7, 8 });

            // Treat surfaces 1, 5 and 10 as a single surface when meshing (i.e. mesh
            // across curves 9 and 10)
            Kernel.MeshingKernel.SetCompound(2, new int[] { 1, 5, 10 });

            Kernel.MeshingKernel.Generate(2);

            Kernel.FinalizeGmsh();

        }
    }
}
