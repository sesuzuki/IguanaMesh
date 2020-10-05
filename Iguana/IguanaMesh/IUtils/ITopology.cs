using Iguana.IguanaMesh.ITypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IUtils
{
    public class ITopology : ITopologyInterface
    {
        private IMesh iM;

        public ITopology(IMesh m) { iM = m; }

        public List<int> GetVertexIncidentElements(int vertexKey)
        {
            List<Int32> neighbor = null;

            if (iM.Vertices.ContainsKey(vertexKey))
            {
                Int32 eK = iM.Vertices.GetVertexWithKey(vertexKey).GetElementID();

                neighbor = new List<Int32>();

                Int64[] nK;

                while (!neighbor.Contains(eK))
                {

                    neighbor.Add(eK);
                    nK = iM.Elements.GetElementWithKey(eK).GetSiblingHalfFacets();

                    foreach (Int64 eData in nK)
                    {

                        int key = -1;
                        if (eData != 0)
                        {

                            key = (Int32)(eData >> 32);

                            IElement e = iM.Elements.GetElementWithKey(key);

                            if (e.Vertices.Contains(vertexKey) && !neighbor.Contains(key))
                            {
                                eK = key;
                            }
                        }
                    }
                }
            }
            return neighbor;
        }

        public List<int> GetVertexAdjacentVertices(int vertexKey)
        {
            List<int> vNeighbor = null;
            List<int> visited = null;

            if (iM.Vertices.ContainsKey(vertexKey))
            {
                visited = iM.Topology.GetVertexIncidentElements(vertexKey);
                vNeighbor = new List<int>();


                foreach (int key in visited)
                {
                    IElement e = iM.Elements.GetElementWithKey(key);

                    for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                    {
                        int[] hf;
                        e.GetHalfFacet(halfFacetID, out hf);

                        if (hf.Contains(vertexKey))
                        {
                            int hfIdx = hf.ToList().IndexOf(vertexKey);

                            int next = hfIdx + 1;
                            int prev = hfIdx - 1;

                            if (next > hf.Length - 1) next = 0;
                            if (prev < 0) prev = hf.Length - 1;

                            int nextV = hf[next];
                            int prevV = hf[prev];

                            if (!vNeighbor.Contains(nextV)) vNeighbor.Add(nextV);
                            if (!vNeighbor.Contains(prevV)) vNeighbor.Add(prevV);
                        }
                    }
                }
            }
            return vNeighbor;
        }

        public bool ComputeVertexNormal(int vertexKey)
        {
            if (iM.Vertices.ContainsKey(vertexKey))
            {
                ITopologicVertex vertex = iM.Vertices.GetVertexWithKey(vertexKey);

                IVector3D normal = new IVector3D();
                List<int> nV = GetVertexAdjacentVertices(vertexKey);

                int next;
                ITopologicVertex nPt1, nPt2;
                IVector3D v1, v2;
                for (int i = 0; i < nV.Count; i++)
                {
                    next = i + 1;
                    if (i == nV.Count - 1) next = 0;

                    nPt1 = iM.Vertices.GetVertexWithKey(nV[i]);
                    nPt2 = iM.Vertices.GetVertexWithKey(nV[next]);
                    v1 = nPt1 - vertex;
                    v2 = nPt2 - vertex;
                    normal += IVector3D.Cross(v1, v2, true);
                }
                normal.Norm();

                vertex.Normal = normal;

                iM.Vertices.SetVertex(vertexKey, vertex);

                return true;
            }
            else return false;
        }

        public bool ComputeAllVerticesNormals()
        {
            if (iM == null) return false;
            else
            {

                List<int> vKeys = iM.Vertices.VerticesKeys;
                Parallel.ForEach(vKeys, vertexKey =>
                {
                    ComputeVertexNormal(vertexKey);
                });
                return true;
            }
        }

        public bool CleanAllVerticesNormals()
        {
            if (iM.Vertices != null && iM.Vertices.Count > 0)
            {
                foreach (int key in iM.Vertices.VerticesKeys)
                {
                    ITopologicVertex v = iM.Vertices.GetVertexWithKey(key);
                    v.Normal = new IVector3D(0, 0, 0);

                    iM.Vertices.SetVertex(key, v);
                }
                return true;
            }
            else return false;
        }

        public List<Tuple<int, int>> GetNakedEdges()
        {
            List<Tuple<int, int>> naked = new List<Tuple<int, int>>();

            foreach (int eK in iM.Elements.ElementsKeys)
            {

                IElement e = iM.Elements.GetElementWithKey(eK);

                if (e.TopologicDimension == 2)
                {
                    Int64[] sibhf = e.GetSiblingHalfFacets();

                    for (int i = 0; i < sibhf.Length; i++)
                    {
                        Int64 hf = sibhf[i];
                        if (hf == 0)
                        {
                            int A = i;
                            int B = i + 1;
                            if (i == sibhf.Length - 1) B = 0;

                            naked.Add(Tuple.Create<int, int>(e.Vertices[A], e.Vertices[B]));
                        }
                    }
                }
            }

            return naked;
        }

        public List<int> GetNakedVertices()
        {
            List<int> naked = new List<int>();
            foreach (int vKey in iM.Vertices.VerticesKeys)
            {
                ITopologicVertex v = iM.Vertices.GetVertexWithKey(vKey);

                if (v.V2HF != 0)
                {
                    IElement e = iM.Elements.GetElementWithKey(v.GetElementID());
                    Int64 sibhe = e.GetSiblingHalfFacet(v.GetHalfFacetID());
                    if (sibhe == 0)
                    {
                        naked.Add(vKey);
                    }
                }
            }
            return naked;
        }

        public List<Int64> GetUniqueEdges()
        {
            List<Int64> edges = new List<Int64>();
            IElement element_sibling;
            int elementID_sibling, halfFacetID_sibling;
            Boolean visited;

            foreach (int elementID in iM.Elements.ElementsKeys)
            {
                IElement e = iM.Elements.GetElementWithKey(elementID);

                if (!e.Visited)
                {
                    int[] hf;
                    if (e.TopologicDimension == 2)
                    {
                        for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                        {
                            visited = e.IsSiblingHalfFacetVisited(halfFacetID);

                            if (!e.IsNakedSiblingHalfFacet(halfFacetID) && !visited)
                            {
                                while (visited == false)
                                {
                                    //Register Visit
                                    e.RegisterSiblingHalfFacetVisit(halfFacetID);

                                    //Collect information of siblings
                                    elementID_sibling = e.GetSiblingElementID(halfFacetID);
                                    halfFacetID_sibling = e.GetSiblingHalfFacetID(halfFacetID);
                                    element_sibling = iM.Elements.GetElementWithKey(elementID_sibling);

                                    visited = element_sibling.IsSiblingHalfFacetVisited(halfFacetID_sibling);

                                    halfFacetID = halfFacetID_sibling;
                                    e = element_sibling;
                                }
                                e.GetHalfFacet(halfFacetID, out hf);
                                Int64 data = (Int64)hf[0] << 32 | (Int64)hf[1];
                                edges.Add(data);
                            }
                            else if (e.IsNakedSiblingHalfFacet(halfFacetID))
                            {
                                e.GetHalfFacet(halfFacetID, out hf);
                                Int64 data = (Int64)hf[0] << 32 | (Int64)hf[1];
                                edges.Add(data);
                            }
                        }
                    }
                    else
                    {
                        int next;
                        Int64 data1, data2;
                        for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                        {
                            e.GetHalfFacet(halfFacetID, out hf);
                            for (int i = 0; i < hf.Length; i++)
                            {
                                next = i + 1;
                                if (i == hf.Length - 1) next = 0;
                                data1 = (Int64)hf[i] << 32 | (Int64)hf[next];
                                data2 = (Int64)hf[next] << 32 | (Int64)hf[i];
                                if (!edges.Contains(data1) && !edges.Contains(data2)) edges.Add(data1);
                            }
                        }
                    }
                }
            }
            iM.Elements.CleanVisits();
            return edges;
        }
    }
}
