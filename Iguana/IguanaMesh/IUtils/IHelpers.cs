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
        public static int[] ToIntArray(long[] data)
        {
            int[] arr = new int[data.Length];
            for(int i=0; i<data.Length; i++)
            {
                arr[i] = (int)data[i];
            }

            return arr;
        }

        public static void UnpackKeyPair(long keyPair, out int firstPart, out int secondPart)
        {
            firstPart = (int)(keyPair >> 32);
            secondPart = (int)keyPair;
        }


        public static long PackKeyPair(int firstPart, int secondPart)
        {
            return (long)(firstPart) << 32 | (long)(secondPart);
        }

        public static int UnpackFirst32BitsOnSignedKeyPair(long keyPair)
        {
            return (int)(keyPair >> 32);
        }

        public static int UnpackSecond32BitsOnSignedKeyPair(long keyPair)
        {
            return (int)(keyPair);
        }

        /// <summary>
        /// Pack parent element, child element (for 3D elements) and half facet.
        /// The parent element is stored within 32 bites (max count : 4.294.967.295),
        /// the child element within 16 bites (max count : 65,535)
        /// and the half-facet within 16 bites (max count : 65,535).  
        /// </summary>
        /// <param name="part1"></param>
        /// <param name="part2"></param>
        /// <param name="part3"></param>
        /// <returns></returns>
        public static long PackTripleKey(int part1, int part2, int part3)
        {
            long sibData = (long)part1;
            sibData = (sibData << 16) | (long)part2;
            sibData = (sibData << 16) | (long)part3;
            return sibData;
        }

        /// <summary>
        /// Unpack Pack triple key.
        /// </summary>
        /// <param name="keyTriple"></param>
        /// <param name="part1"> Key of the parent element stored within 32 bites (max count : 4.294.967.295)</param>
        /// <param name="part2"> Key of the child element stored within 16 bites (max count : 65,535)</param>
        /// <param name="part3"> Key of the half-facet stored within 16 bites (max count : 65,535)</param>
        public static void UnpackTripleKey(long keyTriple, out int part1, out int part2, out int part3)
        {
            part3 = Convert.ToInt32(keyTriple & 0xFFFF);
            keyTriple = keyTriple >> 16;
            part2 = Convert.ToInt32(keyTriple & 0xFFFF);
            keyTriple = keyTriple >> 16;
            part1 = Convert.ToInt32(keyTriple & 0xFFFFFFFF);
        }

        public static int UnpackFirst32BitsOnTripleKey(long tripleKey)
        {
            tripleKey = tripleKey >> 32;
            return Convert.ToInt32(tripleKey & 0xFFFFFFFF);
        }

        public static int UnpackSecond16BitsOnTripleKey(long tripleKey)
        {
            tripleKey = tripleKey >> 16;
            return Convert.ToInt32(tripleKey & 0xFFFF);
        }

        public static int UnpackThird16BitsOnTripleKey(long tripleKey)
        {
            return Convert.ToInt32(tripleKey & 0xFFFF);
        }


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

        public static long[] FlattenLongArray(long[][] data)
        {
            List<long> list = new List<long>();
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    list.Add(data[i][j]);
                }
            }
            return list.ToArray();
        }

        public static ulong[] FlattenULongArray(long[][] data)
        {
            List<ulong> list = new List<ulong>();
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    list.Add((ulong)data[i][j]);
                }
            }
            return list.ToArray();
        }

        public static void ParseRhinoVertices(MeshVertexList vertices, out double[] coord, out long[] nodeTags)
        {
            coord = new double[vertices.Count*3];
            nodeTags = new long[vertices.Count];
            Point3d p;
            for (int i = 0; i < vertices.Count; i++)
            {
                p = vertices[i];
                nodeTags[i] = i;
                coord[i*3] = p.X;
                coord[i*3+1] = p.Y;
                coord[i*3+1] = p.Z;
            }
        }

        public static void ParseRhinoTextures(MeshTextureCoordinateList textures, out double[] parametricCoord)
        {
            parametricCoord = new double[textures.Count * 2];
            Point2f uv;
            for (int i = 0; i < textures.Count; i++)
            {
                uv = textures[i];
                parametricCoord[i * 2] = uv.X;
                parametricCoord[i * 2] = uv.Y;
            }

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
