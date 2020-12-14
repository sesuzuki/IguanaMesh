/*
 * <Iguana>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Collections.Generic;

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

        public static void UnpackKey(Int64 keyPair, out Int32 firstPart, out Int32 secondPart)
        {
            firstPart = (Int32)(keyPair >> 32);
            secondPart = (Int32)keyPair;
        }

        public static Int64 PackKeyPair(Int32 firstPart, Int32 secondPart)
        {
            return (Int64)firstPart << 32 | (Int64)secondPart;
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
