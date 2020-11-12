﻿using Iguana.IguanaMesh.ITypes;
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
        internal static IPoint3D ComputeLoopEdgeVertexPosition(int[] keys, IMesh m)
        {
            IVector3D v = new IVector3D();
            if (!m.Topology.IsNakedEdge(keys[0], keys[1]))
            {
                // Edge ends average
                foreach(int vK in keys) v += m.GetVertexWithKey(vK).Position;

                //Face centers average
                int[] incidentE = m.Topology.GetEdgeIncidentElements(keys[0], keys[1]);
                IElement e;
                foreach (int eKey in incidentE)
                {
                    e = m.GetElementWithKey(eKey);
                    v += (ComputeAveragePosition(e.Vertices, m)*3);
                }
                v /= 8;
            }
            else
            {
                for (int i = 0; i < keys.Length; i++) v += m.GetVertexWithKey(keys[i]).Position;
                v /= keys.Length;

            }

            return new IPoint3D(v.X,v.Y,v.Z);
        }

        internal static IPoint3D ComputeLoopVertexPosition(IMesh m, int vKey)
        {
            ITopologicVertex v = m.GetVertexWithKey(vKey);
            int[] vN = m.Topology.GetVertexAdjacentVertices(vKey);
            IPoint3D pos = v.Position;
            IVector3D vec = new IVector3D();
            if (!m.Topology.IsNakedVertex(vKey))
            {
                int n = vN.Length;

                double coef = (3.0 / 16.0)*n;
                if (n > 3) coef = (3.0 / (8.0 * n))*n;

                vec = pos * (1 - coef);
                vec += (ComputeAveragePosition(vN, m)*coef);
            }
            else
            {
                int count = 0;
                foreach (int nKey in vN)
                {
                    if (m.Topology.IsNakedEdge(nKey, vKey))
                    {
                        vec += ComputeAveragePosition(ComputeAveragePosition(new[] { vKey, nKey }, m), v.Position);
                        count++;
                    }
                }
                vec /= count;
            }
            return new IPoint3D(vec.X,vec.Y,vec.Z);
        }

        public static IMesh Loop(IMesh mesh)
        {
            IMesh triMesh = IModifier.Triangulate2DElements(mesh);

            IMesh sMesh = new IMesh();

            //Old vertices
            foreach (int vK in triMesh.VerticesKeys)
            {
                sMesh.AddVertex(vK, new ITopologicVertex(ComputeLoopVertexPosition(triMesh, vK)));
            }

            // Subidvision
            int key = triMesh.FindNextVertexKey();
            int[] hf;
            IElement element_sibling;
            int elementID_sibling, halfFacetID_sibling;
            Boolean visited;
            Dictionary<int, int[]> eVertex = new Dictionary<int, int[]>();

            // Vertices
            foreach (int elementID in triMesh.ElementsKeys)
            {
                IElement e = triMesh.GetElementWithKey(elementID);

                if (!e.Visited)
                {
                    if (e.TopologicDimension == 2)
                    {
                        if (!eVertex.ContainsKey(elementID)) eVertex.Add(elementID, new int[e.HalfFacetsCount]);

                        for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                        {
                            e.GetFirstLevelHalfFacet(halfFacetID, out hf);
                            visited = e.IsHalfFacetVisited(halfFacetID);

                            if (!visited)
                            {
                                e.RegisterHalfFacetVisit(halfFacetID);
                                e.GetFirstLevelHalfFacet(halfFacetID, out hf);
                                sMesh.AddVertex(key, new ITopologicVertex(ComputeAveragePosition(hf, triMesh)));
                                eVertex[elementID][halfFacetID - 1] = key;

                                if (!e.IsNakedSiblingHalfFacet(halfFacetID))
                                {
                                    while (!visited)
                                    {
                                        e.RegisterHalfFacetVisit(halfFacetID);

                                        //Collect information of siblings
                                        elementID_sibling = e.GetSiblingElementID(halfFacetID);
                                        halfFacetID_sibling = e.GetParentSiblingHalfFacetID(halfFacetID);
                                        element_sibling = triMesh.GetElementWithKey(elementID_sibling);

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
            triMesh.CleanElementsVisits();

            //Faces
            int prev;
            foreach (int elementID in triMesh.ElementsKeys)
            {
                IElement e = triMesh.GetElementWithKey(elementID);

                if (e.TopologicDimension == 2)
                {
                    int[] eV = eVertex[elementID];
                    for (int i = 0; i < e.Vertices.Length; i++)
                    {
                        prev = i - 1;
                        if (prev < 0) prev = e.Vertices.Length - 1;

                        sMesh.AddElement(new ISurfaceElement(new[] { eV[prev], e.Vertices[i], eV[i] }));
                    }
                    sMesh.AddElement(new ISurfaceElement(new[] { eV[0], eV[1], eV[2] }));
                }
            }

            // Edge Vertex
            foreach (int eK in eVertex.Keys)
            {
                IElement e = triMesh.GetElementWithKey(eK);
                for (int i = 1; i <= e.HalfFacetsCount; i++)
                {
                    e.GetFirstLevelHalfFacet(i, out hf);
                    int vK = eVertex[eK][i - 1];
                    sMesh.SetVertexPosition(vK, ComputeLoopEdgeVertexPosition(hf, triMesh));
                }
            }

            //Build Mesh
            sMesh.BuildTopology();

            return sMesh;
        }
    }
}
