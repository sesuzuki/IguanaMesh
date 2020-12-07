using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Iguana.IguanaMesh.ICreators;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IWrappers;

namespace IguanaClient
{
    class Program
    {
        static void Main(string[] args)
        {

            Example.T1();


            /*IguanaGmsh.Initialize();

            var sw = new StringWriter();
            Console.SetOut(sw);
            Console.SetError(sw);

            //IguanaGmsh.Option.SetNumber("General.Terminal", 1);
            IguanaGmsh.Logger.Start();

            Tuple<int, int>[] dimTags;
            IguanaGmsh.Model.GeoOCC.ImportShapes("C:/Users/sesuz/Desktop/IG-6dfc0fcd-0e16-4a8f-b29e-a8285f413e49.step", out dimTags);
            IguanaGmsh.Model.GeoOCC.Synchronize();

            IguanaGmsh.Option.SetNumber("Mesh.MeshSizeMax", 0.3);
            IguanaGmsh.Option.SetNumber("Mesh.MeshSizeMin", 1);

            //2D mesh algorithm (1: MeshAdapt, 2: Automatic, 3: Initial mesh only, 5: Delaunay, 6: Frontal-Delaunay
            //7: BAMG, 8: Frontal - Delaunay for Quads, 9: Packing of Parallelograms)
            //3D mesh algorithm (1: Delaunay, 3: Initial mesh only, 4: Frontal, 7: MMG3D, 9: R-tree, 10: HXT)
            // Equilateral trias
            //IguanaGmsh.Option.SetNumber("Mesh.Algorithm3D", 1);

            // Righ-angle trias
            //IguanaGmsh.Option.SetNumber("Mesh.Algorithm", 9);

            // Quad-dominant
            //IguanaGmsh.Option.SetNumber("Mesh.Algorithm3D", 1);
            //IguanaGmsh.Option.SetNumber("Mesh.RecombineAll", 1);
            //IguanaGmsh.Option.SetNumber("Mesh.RecombinationAlgorithm", 1);

            // Mixed trias and quads
            //IguanaGmsh.Option.SetNumber("Mesh.Algorithm3D", 2);
            //IguanaGmsh.Option.SetNumber("Mesh.RecombineAll", 1);
            //IguanaGmsh.Option.SetNumber("Mesh.RecombinationAlgorithm", 1);
            //IguanaGmsh.Option.SetNumber("Mesh.ElementOrder", 2);

            // Quad-Only
            IguanaGmsh.Option.SetNumber("Mesh.MeshSizeFromPoints", 1);
            //IguanaGmsh.Option.SetNumber("Mesh.MeshSizeFromParametricPoints", 1);
            //IguanaGmsh.Model.Mesh.SetAutomaticTransfinite(dimTags);
            IguanaGmsh.Option.SetNumber("Mesh.Algorithm", 1);
            IguanaGmsh.Option.SetNumber("Mesh.RecombinationAlgorithm", 1);
            IguanaGmsh.Option.SetNumber("Mesh.RecombineAll", 1);
            //IguanaGmsh.Option.SetNumber("Mesh.AngleSmoothNormals", 45);
            //IguanaGmsh.Option.SetNumber("Mesh.SubdivisionAlgorithm", 1);
            //IguanaGmsh.Option.SetNumber("Mesh.OptimizeThreshold", 1);

            IguanaGmsh.Model.Mesh.Generate(3);

            Tuple<int, int>[] temp;
            IguanaGmsh.Model.GetEntities(out temp, 0);
            IguanaGmsh.Model.Mesh.SetSize(temp, 1);

            IguanaGmsh.FLTK.Run();

            string result = sw.ToString();

            Console.WriteLine(result);

            IguanaGmsh.FinalizeGmsh();*/

            Console.ReadLine();
        }     
    }
}
