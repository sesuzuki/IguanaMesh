using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IUtils
{
    public static class IHelpers
    {
        public static Tuple<int, int>[] GraftIntTupleArray(int[] data)
        {
            Tuple<int, int>[] list = new Tuple<int, int>[data.Length / 2];
            for (int i = 0; i < data.Length / 2; i++)
            {
                list[i] = Tuple.Create(data[i * 2], data[i * 2 + 1]);
            }
            return list;
        }

        public static Tuple<int, int>[] GraftIntTupleArray(long[] data)
        {
            Tuple<int, int>[] list = new Tuple<int, int>[data.Length / 2];
            for (int i = 0; i < data.Length / 2; i++)
            {
                list[i] = Tuple.Create((int) data[i * 2], (int) data[i * 2 + 1]);
            }
            return list;
        }

        public static int[] FlattenIntTupleArray(Tuple<int, int>[] data)
        {
            int[] list = new int[data.Length * 2];
            int idx = 0;
            foreach (var pair in data)
            {
                list[idx] = pair.Item1;
                list[idx + 1] = pair.Item2;
                idx += 2;
            }
            return list;
        }

        public static double[] FlattenDoubleArray(double[][] data)
        {
            List<double> list = new List<double>();
            for(int i=0;i<data.Length; i++)
            {
                for(int j=0; j< data[i].Length; j++)
                {
                    list.Add(data[i][j]);
                }
            }
            return list.ToArray();
        }

        public static int[] FlattenIntArray(int[][] data)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    list.Add(data[i][j]);
                }
            }
            return list.ToArray();
        }

        public static void ParseRhinoVertices(MeshVertexList vertices, out double[][] coord, out int[] nodeTags)
        {
            coord = new double[vertices.Count][];
            nodeTags = new int[vertices.Count];
            Point3d p;
            for (int i = 0; i < vertices.Count; i++)
            {
                p = vertices[i];
                coord[i] = new double[] { p.X, p.Y, p.Z };
                nodeTags[i] = i;
            }
        }

        public static double[][] ParseRhinoTextures(MeshTextureCoordinateList textures)
        {
            double[][] list = new double[textures.Count][];
            Point2f uv;
            for (int i = 0; i < textures.Count; i++)
            {
                uv = textures[i];
                list[i] = new double[] { uv.X, uv.Y };
            }
            return list;
        }

        public static string HighOrder2DElementsToString(string element_type, int[] vertices, int order=2)
        {
            string aux1 = "Node-Vertex{ ";
            string aux2 = "Node-Edge{ ";
            for (int i = 0; i < vertices.Length / order; i++)
            {
                // Nodes-Vertices
                aux1 += vertices[i*order] + " ";

                // Nodes-Edges
                for (int j = 1; j < order; j++)
                {
                    aux2 += vertices[i*order + j] + " ";
                }
            }

            string msg = element_type + " || " + aux1 + "} || " + aux2 + "}";

            return msg;
        }

        public static string HighOrder3DElementsToString(string element_type, int[] vertices, int low_order_nodes_count = 4)
        {
            string aux1 = "Node-Vertex{ ";
            string aux2 = "Node-Edge{ ";
            for (int i = 0; i < vertices.Length; i++)
            {
                // Nodes-Vertices
                if(i<low_order_nodes_count) aux1 += vertices[i] + " ";
                else aux2 += vertices[i] + " ";
            }

            string msg = element_type + " || " + aux1 + "} || " + aux2 + "}";

            return msg;
        }
    }
}
