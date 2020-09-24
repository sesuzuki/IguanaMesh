using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iguana.IguanaMesh.ICreators;
using Iguana.IguanaMesh.ITypes;
using Iguana.IStandard;
using Rhino.Geometry;
using Iguana.IguanaMesh.IGmshWrappers;


namespace IguanaClient
{
    class Program
    {
        static void Main(string[] args)
        {
            const double lc = 0.5;

            Gmsh.Initialize();

            Gmsh.Option.SetNumber("General.Terminal", 1);

            Gmsh.Model.Add("t1");

            Gmsh.Model.Geo.AddPoint(0, 0, 0, lc, 1);
            Gmsh.Model.Geo.AddPoint(1, 0, 0, lc, 2);
            Gmsh.Model.Geo.AddPoint(1, 3, 0, lc, 3);
            Gmsh.Model.Geo.AddPoint(0, 3, 0, lc, 4);

            Gmsh.Model.Geo.AddLine(1, 2, 1);
            Gmsh.Model.Geo.AddLine(3, 2, 2);
            Gmsh.Model.Geo.AddLine(3, 4, 3);
            Gmsh.Model.Geo.AddLine(4, 1, 4);

            Gmsh.Model.Geo.AddCurveLoop(new int[] { 4, 1, -2, 3 }, 1);

            Gmsh.Model.Geo.AddPlaneSurface(new int[] { 1 }, 1);

            Gmsh.Model.AddPhysicalGroup(2, new int[] { 1 }, 1);

            Gmsh.Model.SetPhysicalName(2, 1, "My surface");

            Gmsh.Model.Geo.Synchronize();

            Gmsh.Model.Mesh.Generate(2);

            Gmsh.Write("Mesh1.msh");


            IVertexCollection vertices = Gmsh.Model.Mesh.TryGetIVertexCollection();

            Console.WriteLine(vertices);
            /*int[] nodesTag;
            double[][] coords, uvw;
            Gmsh.Model.Mesh.GetNodes(out nodesTag, out coords, out uvw, 2);
            
            int[][][] elements;

            Gmsh.Model.Mesh.GetElements(out elements, 2);
            foreach(int[][] eType in elements)
            {
                foreach(int[] eData in eType)
                {
                    Console.WriteLine("New Element");
                    Console.WriteLine(":: " + eData[0] + " :: " + eData[1] + " :: " + eData[2] + " ::");
                }
            }*/

            Gmsh.FinalizeGmsh();

            Console.ReadLine();
        }
    }
}
