using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;

namespace Iguana.IguanaMesh.ITypes.IElements
{
    public partial class ISurfaceElement : IElement
    {
        /// <summary>
        /// <para> General constructor for a polygonal face. </para>
        /// Element Type Reference: -1 por poligonal faces with more thant 4 edges per node, 2 for triangular faces and 3 for quadrangular faces.
        /// <para><paramref name="vertices"/> : A collection of vertex identifiers. </para>
        /// </summary>
        ///
        public ISurfaceElement(int[] vertices) : base(vertices, vertices.Length, 2, -1) 
        {
            if (vertices.Length == 3) this.SetElementType(2);
            else if (vertices.Length == 4) this.SetElementType(3);
        }

        /// <summary>
        /// Constructor for a quadrangular face.
        /// Element Type Reference: 3
        /// <para><paramref name="A"/> : First vertex identifier. </para>
        /// <para><paramref name="B"/> : Second vertex identifier. </para>
        /// <para><paramref name="C"/> : Third vertex identifier. </para>
        /// <para><paramref name="D"/> : Fourth vertex identifier. </para>
        /// </summary>
        ///
        public ISurfaceElement(int A, int B, int C, int D) : base(new int[] { A, B, C, D}, 4, 2, 3) { }

        /// <summary>
        /// Constructor for a triangle face.
        /// Element Type Reference: 2
        /// <para><paramref name="A"/> : First vertex identifier. </para>
        /// <para><paramref name="B"/> : Second vertex identifier. </para>
        /// <para><paramref name="C"/> : Third vertex identifier. </para>
        /// </summary>
        public ISurfaceElement(int A, int B, int C) : base(new int[] { A, B, C }, 3, 2, 2) { }

        public override IElement CleanCopy()
        {
            return new ISurfaceElement(Vertices);
        }

        /// <summary>
        /// <para> Element´s description . </para>
        /// </summary>
        ///
        public override string ToString()
        {
            string msg = "ISurfaceElement{";
            if(VerticesCount==3) msg = "ITriangleElement{";
            else if(VerticesCount==4) msg = "IQuadrangleElement{";
            for (int i = 0; i < VerticesCount; i++)
            {
                int idx = Vertices[i];
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
                    halfFacet = new int[] { Vertices[index-1], Vertices[index] };
                }
                else
                {
                    halfFacet = new int[] { Vertices[index-1], Vertices[0] };
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
