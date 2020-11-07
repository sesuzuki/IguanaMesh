using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IModifiers
{
    public static partial class ISubdividor
    {
        internal static IVector3D ComputeLoopEdgeVertexPosition(int[] keys, IMesh m)
        {
            IVector3D v = new IVector3D();
            if (!m.Topology.IsNakedEdge(keys[0], keys[1]))
            {
                // Edge ends average
                foreach(int vK in keys) v += m.Vertices.GetVertexWithKey(vK).Position;

                //Face centers average
                int[] incidentE = m.Topology.GetEdgeIncidentElements(keys[0], keys[1]);
                IElement e;
                foreach (int eKey in incidentE)
                {
                    e = m.Elements.GetElementWithKey(eKey);
                    v += (ComputeAveragePosition(e.Vertices, m)*3);
                }
                v /= 8;
            }
            else
            {
                for (int i = 0; i < keys.Length; i++) v += m.Vertices.GetVertexWithKey(keys[i]).Position;
                v /= keys.Length;
            }

            return v;
        }

        internal static IVector3D ComputeLoopVertexPosition(IMesh m, int vKey)
        {
            ITopologicVertex v = m.Vertices.GetVertexWithKey(vKey);
            int[] vN = m.Topology.GetVertexAdjacentVertices(vKey);
            IVector3D pos = new IVector3D();
            if (!m.Topology.IsNakedVertex(vKey))
            {
                int n = vN.Length;

                double coef = (3 / 16) * n;
                if (n > 3) coef = (3 / (8 * n)) * n;

                pos = ComputeAveragePosition(vN, m);
                pos *= (1 - coef);
                pos += (v.Position * coef);
            }
            else
            {
                pos = new IVector3D();
                int count = 0;
                foreach (int nKey in vN)
                {
                    if (m.Topology.IsNakedEdge(nKey, vKey))
                    {
                        pos += ComputeAveragePosition(ComputeAveragePosition(new[] { vKey, nKey }, m), v.Position);
                        count++;
                    }
                }
                pos /= count;
            }
            return pos;
        }

        public static IMesh Loop(IMesh mesh)
        {
            IMesh triMesh = IModifier.Triangulate2DElements(mesh);

            IMesh sMesh = new IMesh();

            //Old vertices
            foreach (int vK in triMesh.Vertices.VerticesKeys)
            {
                sMesh.Vertices.AddVertex(vK, new ITopologicVertex(ComputeLoopVertexPosition(triMesh, vK)));
            }

            // Subidvision
            int key = triMesh.Vertices.FindNextKey();
            int[] hf;
            IElement element_sibling;
            int elementID_sibling, halfFacetID_sibling;
            Boolean visited;
            Dictionary<int, int[]> eVertex = new Dictionary<int, int[]>();

            // Vertices
            foreach (int elementID in triMesh.Elements.ElementsKeys)
            {
                IElement e = triMesh.Elements.GetElementWithKey(elementID);

                if (!e.Visited)
                {
                    if (e.TopologicDimension == 2)
                    {
                        if (!eVertex.ContainsKey(elementID)) eVertex.Add(elementID, new int[e.HalfFacetsCount]);

                        for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                        {
                            e.GetHalfFacet(halfFacetID, out hf);
                            visited = e.IsHalfFacetVisited(halfFacetID);

                            if (!visited)
                            {
                                e.RegisterHalfFacetVisit(halfFacetID);
                                e.GetHalfFacet(halfFacetID, out hf);
                                sMesh.Vertices.AddVertex(key, new ITopologicVertex(ComputeAveragePosition(hf, triMesh)));
                                eVertex[elementID][halfFacetID - 1] = key;

                                if (!e.IsNakedSiblingHalfFacet(halfFacetID))
                                {
                                    while (!visited)
                                    {
                                        e.RegisterHalfFacetVisit(halfFacetID);

                                        //Collect information of siblings
                                        elementID_sibling = e.GetSiblingElementID(halfFacetID);
                                        halfFacetID_sibling = e.GetSiblingHalfFacetID(halfFacetID);
                                        element_sibling = triMesh.Elements.GetElementWithKey(elementID_sibling);

                                        visited = element_sibling.IsHalfFacetVisited(halfFacetID_sibling);

                                        halfFacetID = halfFacetID_sibling;
                                        e = element_sibling;

                                        if (!eVertex.ContainsKey(elementID_sibling)) eVertex.Add(elementID_sibling, new int[e.HalfFacetsCount]);
                                        eVertex[elementID_sibling][halfFacetID - 1] = key;
                                    }
                                }

                                key++;
                            }
                        }
                    }
                }
            }
            triMesh.Elements.CleanVisits();

            //Faces
            int prev;
            int elementKey = sMesh.Elements.FindNextKey();
            foreach (int elementID in triMesh.Elements.ElementsKeys)
            {
                IElement e = triMesh.Elements.GetElementWithKey(elementID);

                if (e.TopologicDimension == 2)
                {
                    int[] eV = eVertex[elementID];
                    for (int i = 0; i < e.Vertices.Length; i++)
                    {
                        prev = i - 1;
                        if (prev < 0) prev = e.Vertices.Length - 1;

                        sMesh.Elements.AddElement(elementKey, new ISurfaceElement(new[] { eV[prev], e.Vertices[i], eV[i] }));
                        elementKey++;
                    }
                    sMesh.Elements.AddElement(elementKey, new ISurfaceElement(new[] { eV[0], eV[1], eV[2] }));
                    elementKey++;
                }
            }

            // Edge Vertex
            foreach (int eK in eVertex.Keys)
            {
                IElement e = triMesh.Elements.GetElementWithKey(eK);
                for (int i = 1; i <= e.HalfFacetsCount; i++)
                {
                    e.GetHalfFacet(i, out hf);
                    int vK = eVertex[eK][i - 1];
                    sMesh.Vertices.SetVertexPosition(vK, ComputeLoopEdgeVertexPosition(hf, triMesh));
                }
            }

            //Build Mesh
            sMesh.BuildTopology();

            return sMesh;
        }
    }
}
