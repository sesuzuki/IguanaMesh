﻿/*
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

using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;

namespace Iguana.IguanaMesh.ITypes
{
    public class IBarElement : IElement
    {

     
        /// <summary>
        /// <para> General constructor for a polygonal face. </para>
        /// Element Type Reference: -1 por poligonal faces with more thant 4 edges per node, 2 for triangular faces and 3 for quadrangular faces.
        /// <para><paramref name="vertices"/> : A collection of vertex identifiers. </para>
        /// </summary>
        ///
        public IBarElement(int[] vertices) : base(vertices, 2, 1, -1) 
        {
            this.SetElementType(1);         
        }

        //public IBarElement(int A, int B) : base(new int[] {A, B}, 2, 1, 1) { }


        #region
        /*

         /// <summary>
         /// Constructor for a quadrangular face.
         /// Element Type Reference: 3
         /// <para><paramref name="A"/> : First vertex identifier. </para>
         /// <para><paramref name="B"/> : Second vertex identifier. </para>
         /// <para><paramref name="C"/> : Third vertex identifier. </para>
         /// <para><paramref name="D"/> : Fourth vertex identifier. </para>
         /// </summary>
         ///
         public IBarElement(int A, int B, int C, int D) : base(new int[] { A, B, C, D}, 4, 2, 3) { }

         /// <summary>
         /// Constructor for a triangle face.
         /// Element Type Reference: 2
         /// <para><paramref name="A"/> : First vertex identifier. </para>
         /// <para><paramref name="B"/> : Second vertex identifier. </para>
         /// <para><paramref name="C"/> : Third vertex identifier. </para>
         /// </summary>
         public IBarElement(int A, int B, int C) : base(new int[] { A, B, C }, 3, 2, 2) { }

         */
        #endregion

        public override IElement CleanCopy()
        {
            IElement e = new IBarElement(Vertices);
            e.Key = Key;
            return e;
        }

        /// <summary>
        /// <para> Element´s description . </para>
        /// </summary>
        ///
        public override string ToString()
        {
            string msg = "IBar{";
            for (int i = 0; i < 2; i++)
            {
                int idx = Vertices[i];
                if (i < VerticesCount - 1) msg += idx + ";";
                else msg += idx + "}";
            }
            return msg;
        }

        /// <summary>
        /// <para> Returns a Half-Facet of type Half-Vertex. A Half-Vertex is the zero-dimensional sub-entity of an edge. </para>
        /// <paramref name="index"/> : The local index of the Half-Facet to search within the element.
        /// <paramref name="halfFacet"/> : A sub-list to store the vertex identifiers representing the Half-Facet.
        /// </summary>
        ///
        public override bool GetHalfFacet(int index, out int[] halfFacet)
        {
            halfFacet = null;
            if (index > 0 && index <= HalfFacetsCount)
            {
                if (index == 1)
                {
                    halfFacet = new int[] { Vertices[0] };
                }
                else
                {
                    halfFacet = new int[] { Vertices[1] };
                }
                return true;
            }
            else return false;
        }

        public override bool AddVertex(int vertexKey)
        {
            {
                if (!Vertices.Contains(vertexKey))
                {
                    //Add vertex
                    int[] tempV = new int[VerticesCount + 1];
                    Array.Copy(Vertices, tempV, VerticesCount);
                    tempV[VerticesCount] = vertexKey;
                    return true;
                }
                else return false;
            }
        }

        public override bool RemoveVertex(int vertexKey)
        {
            {
                if (!Vertices.Contains(vertexKey))
                {
                    int[] tempV = new int[VerticesCount-1];
                    int idx=0, evalKey;
                    for(int i=0; i<VerticesCount; i++)
                    {
                        evalKey = Vertices[i];
                        if (evalKey != vertexKey)
                        {
                            tempV[idx] = evalKey;
                            idx++;
                        }
                    }

                    int eT = -1;
                    if (tempV.Length == 3) eT = 2;
                    else if (tempV.Length == 4) eT = 3;
                    init(tempV, tempV.Length, 2, eT);

                    return true;
                }
                else return false;
            }
        }

        public override bool GetHalfFacetWithPrincipalNodesOnly(int index, out int[] halfFacets)
        {
            return GetHalfFacet(index, out halfFacets);
        }

        public override int[] GetGmshFormattedVertices()
        {
            return Vertices;
        }
    }
}
