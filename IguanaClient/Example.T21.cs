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
    //  Gmsh C++ tutorial 21
    //
    //  Mesh partitioning
    //
    // -----------------------------------------------------------------------------
    // Gmsh can partition meshes using different algorithms, e.g. the graph
    // partitioner Metis or the `SimplePartition' plugin. For all the partitining
    // algorithms, the relationship between mesh elements and mesh partitions is
    // encoded through the creation of new (discrete) elementary entities, called
    // "partition entities".
    //
    // Partition entities behave exactly like other discrete elementary entities;
    // the only difference is that they keep track of both a mesh partition index
    // and their parent elementary entity.
    //
    // The major advantage of this approach is that it allows to maintain a full
    // boundary representation of the partition entities, which Gmsh creates
    // automatically if `Mesh.PartitionCreateTopology' is set.

    public static partial class Example
    {
        public static void T21()
        {
            Kernel.Initialize();
            Kernel.Option.SetNumber("General.Terminal", 1);

            Kernel.Model.Add("t21");

            // Let us start by creating a simple geometry with two adjacent squares
            // sharing an edge:

            Kernel.Model.Add("t21");
            Kernel.GeoOCC.AddRectangle(0, 0, 0, 1, 1, 1);
            Kernel.GeoOCC.AddRectangle(1, 0, 0, 1, 1, 2);

            Tuple<int, int>[] ov;
            Tuple<int, int>[][] ovv;
            Tuple<int, int>[] objDimTags = new Tuple<int, int>[] { Tuple.Create(2, 1) };
            Tuple<int, int>[] toolDimTags = new Tuple<int, int>[] { Tuple.Create(2, 2) };
            Kernel.GeoOCC.Fragment(objDimTags, toolDimTags, out ov, out ovv);

            Kernel.GeoOCC.Synchronize();
            Tuple<int, int>[] tmp;
            Kernel.Model.GetEntities(out tmp, 0);
            Kernel.MeshingKernel.SetSize(tmp, 0.05);

            // We create one physical group for each square, and we mesh the resulting
            // geometry:
            Kernel.Model.AddPhysicalGroup(2, new[] { 1 }, 100);
            Kernel.Model.SetPhysicalName(2, 100, "Left");
            Kernel.Model.AddPhysicalGroup(2, new[] { 2 }, 200);
            Kernel.Model.SetPhysicalName(2, 200, "Right");
            Kernel.MeshingKernel.Generate(2);

            // We now define several constants to fine-tune how the mesh will be
            // partitioned
            int partitioner = 0; // 0 for Metis, 1 for SimplePartition
            int N = 3; // Number of partitions
            int topology = 1; // Create partition topology (BRep)?
            int ghosts = 0; // Create ghost cells?
            int physicals = 0; // Create new physical groups?
            int split = 1; // Write one file per partition?

            // Should we create the boundary representation of the partition entities?
            Kernel.Option.SetNumber("Mesh.PartitionCreateTopology", topology);

            // Should we create ghost cells?
            Kernel.Option.SetNumber("Mesh.PartitionCreateGhostCells", ghosts);

            // Should we automatically create new physical groups on the partition
            // entities?
            Kernel.Option.SetNumber("Mesh.PartitionCreatePhysicals", physicals);

            // Should we keep backward compatibility with pre-Gmsh 4, e.g. to save the
            // mesh in MSH2 format?
            Kernel.Option.SetNumber("Mesh.PartitionOldStyleMsh2", 0);

            // Should we save one mesh file per partition?
            Kernel.Option.SetNumber("Mesh.PartitionSplitMeshFiles", split);

            if (partitioner == 0)
            {
                // Use Metis to create N partitions
                Kernel.MeshingKernel.Partition(N);
                // Several options can be set to control Metis: `Mesh.MetisAlgorithm' (1:
                // Recursive, 2: K-way), `Mesh.MetisObjective' (1: min. edge-cut, 2:
                // min. communication volume), `Mesh.PartitionTriWeight' (weight of
                // triangles), `Mesh.PartitionQuadWeight' (weight of quads), ...
            }
            else
            {
                // Use the `SimplePartition' plugin to create chessboard-like partitions
                Kernel.Plugin.SetNumber("SimplePartition", "NumSlicesX", N);
                Kernel.Plugin.SetNumber("SimplePartition", "NumSlicesY", 1);
                Kernel.Plugin.SetNumber("SimplePartition", "NumSlicesZ", 1);
                Kernel.Plugin.Run("SimplePartition");
            }

            // Save mesh file (or files, if `Mesh.PartitionSplitMeshFiles' is set):
            Kernel.Write("t21.msh");

            // Iterate over partitioned entities and print some info (see the first
            // extended tutorial `x1.cpp' for additional information):
            Tuple<int, int>[] entities;
            Kernel.Model.GetEntities(out entities);

            foreach (Tuple<int, int> it in entities)
            {
                int[] partitions;
                Kernel.Model.GetPartitions(it.Item1, it.Item2, out partitions);
                if (partitions.Length > 0)
                {
                    string type;
                    Kernel.Model.GetType(it.Item1, it.Item2, out type);
                    Console.WriteLine("Entity (" + it.Item1 + "," + it.Item2 + ") " + "of type " + type + "\n");
                    Console.WriteLine(" - Partition(s):");

                    for (int i = 0; i < partitions.Length; i++) Console.WriteLine(" " + partitions[i] + "\n");
                    int pdim, ptag;
                    Kernel.Model.GetParent(it.Item1, it.Item2, out pdim, out ptag);
                    Console.WriteLine(" - Parent: (" + pdim + "," + ptag + ")\n");
                    Tuple<int, int>[] bnd;
                    Kernel.Model.GetBoundary(new Tuple<int, int>[] { it }, out bnd);
                    Console.WriteLine(" - Boundary:");
                    for (int i = 0; i < bnd.Length; i++)
                    {
                        Console.WriteLine(" (" + bnd[i].Item1 + "," + bnd[i].Item2 + ")" + "\n");
                    }
                }
            }
            Kernel.FinalizeGmsh();
        }
    }
}
