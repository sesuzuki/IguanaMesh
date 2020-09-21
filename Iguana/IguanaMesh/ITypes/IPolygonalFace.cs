using System;
using System.Linq;

namespace Iguana.IguanaMesh.ITypes
{
    public class IPolygonalFace : IElement
    {
        /// <summary>
        /// <para> General constructor for a polygonal face. </para>
        /// <para><paramref name="vertices"/> : A collection of vertex identifiers. </para>
        /// </summary>
        ///
        public IPolygonalFace(int[] vertices) : base(vertices, vertices.Length, 2) {}

        /// <summary>
        /// <para> Specific constructor for a triangular face. </para>
        /// <para><paramref name="A"/> : First vertex identifier. </para>
        /// <para><paramref name="B"/> : Second vertex identifier. </para>
        /// <para><paramref name="C"/> : Third vertex identifier. </para>
        /// </summary>
        ///
        public IPolygonalFace(int A, int B, int C) : base(new int[]{ A, B, C }, 3, 2) { }

        /// <summary>
        /// <para> Specific constructor for a quadrangular face. </para>
        /// <para><paramref name="A"/> : First vertex identifier. </para>
        /// <para><paramref name="B"/> : Second vertex identifier. </para>
        /// <para><paramref name="C"/> : Third vertex identifier. </para>
        /// <para><paramref name="D"/> : Fourth vertex identifier. </para>
        /// </summary>
        ///
        public IPolygonalFace(int A, int B, int C, int D) : base(new int[] { A, B, C, D}, 4, 2) { }

        /// <summary>
        /// <para> Element´s description . </para>
        /// </summary>
        ///
        public override string ToString()
        {
            string msg = "AHF-IFace{";
            for (int i = 0; i < VerticesCount; i++)
            {
                int idx = VerticesKeys[i];
                if (i < VerticesCount - 1) msg += idx + ";";
                else msg += idx + "}";
            }
            return msg;
        }

        /// <summary>
        /// <para> Returns a Half-Facet of type Half-Edge. A Half-Edge is the one-dimensional sub-entity of a face. </para>
        /// <paramref name="index"/> : The local index of the Half-Facet to search within the element.
        /// <paramref name="halfFacet"/> : A sub-list to store all the vertex identifiers representing the Half-Facet.
        /// </summary>
        ///
        public override bool GetHalfFacet(int index, out int[] halfFacet)
        {
            halfFacet = null;
            if (index > 0 && index <= HalfFacetsCount)
            {
                if (index < HalfFacetsCount)
                {
                    halfFacet = new int[] { VerticesKeys[index-1], VerticesKeys[index] };
                }
                else
                {
                    halfFacet = new int[] { VerticesKeys[index-1], VerticesKeys[0] };
                }
                return true;
            }
            else return false;
        }

        public override bool AddVertex(int vertexKey)
        {
            {
                if (!VerticesKeys.Contains(vertexKey))
                {
                    //Add vertex
                    int[] tempV = new int[VerticesCount + 1];
                    Array.Copy(VerticesKeys, tempV, VerticesCount);
                    tempV[VerticesCount] = vertexKey;
                    


                    return true;
                }
                else return false;
            }
        }

        public override bool RemoveVertex(int vertexKey)
        {
            {
                if (!VerticesKeys.Contains(vertexKey))
                {
                    int[] tempV = new int[VerticesCount-1];
                    int idx=0, evalKey;
                    for(int i=0; i<VerticesCount; i++)
                    {
                        evalKey = VerticesKeys[i];
                        if (evalKey != vertexKey)
                        {
                            tempV[idx] = evalKey;
                            idx++;
                        }
                    }

                    init(tempV, tempV.Length, 2);

                    return true;
                }
                else return false;
            }
        }
    }
}
