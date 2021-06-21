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

using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iguana.IguanaMesh.ITypes
{
    public class ITopology
    {
        private IMesh iM;

        public ITopology(IMesh m) { iM = m; }

        public override string ToString()
        {
            return "ITopology";
        }

        public void CleanTopologicalData()
        {
            iM.CleanAllElementsTopologicalData();
            iM.CleanAllVerticesTopologicalData();
        }

        public void GetSortedVerticesID(out int[] naked, out int[] clothed, out int[] corners)
        {
            List<int> clothed_temp = new List<int>();
            List<int> naked_temp = new List<int>();
            List<int> corners_temp = new List<int>();
            foreach (int vKey in iM.VerticesKeys)
            {
                ITopologicVertex v = iM.GetVertexWithKey(vKey);

                if (!v.IsIsolated())
                {
                    if (IsClothedVertex(v.Key))
                    {
                        clothed_temp.Add(vKey);
                    }
                    else
                    {
                        if (IsCornerVertex(v.Key)) corners_temp.Add(vKey);
                        else naked_temp.Add(vKey);
                    }
                }
            }
            corners = corners_temp.ToArray();
            naked = naked_temp.ToArray();
            clothed = clothed_temp.ToArray();
        }

        public void GetSortedVertices(out ITopologicVertex[] naked, out ITopologicVertex[] clothed, out ITopologicVertex[] corners)
        {
            List<ITopologicVertex> clothed_temp = new List<ITopologicVertex>();
            List<ITopologicVertex> naked_temp = new List<ITopologicVertex>();
            List<ITopologicVertex> corners_temp = new List<ITopologicVertex>();
            foreach (ITopologicVertex v in iM.Vertices)
            {
                if (!v.IsIsolated())
                {
                    if (IsClothedVertex(v.Key))
                    {
                        clothed_temp.Add(v);
                    }
                    else
                    {
                        if (IsCornerVertex(v.Key)) corners_temp.Add(v);
                        else naked_temp.Add(v);
                    }
                }
            }
            corners = corners_temp.ToArray();
            naked = naked_temp.ToArray();
            clothed = clothed_temp.ToArray();
        }

        public int[] GetClothedVerticesID()
        {
            List<int> clothed = new List<int>();
            foreach (int vKey in iM.VerticesKeys)
            {
                ITopologicVertex v = iM.GetVertexWithKey(vKey);

                if (!v.IsIsolated())
                {
                    if (IsClothedVertex(v.Key))
                    {
                        clothed.Add(vKey);
                    }
                }
            }
            return clothed.ToArray();
        }

        public ITopologicVertex[] GetClothedVertices()
        {
            List<ITopologicVertex> clothed = new List<ITopologicVertex>();
            foreach (ITopologicVertex v in iM.Vertices)
            {
                if (v.IsIsolated())
                {
                    if (IsClothedVertex(v.Key))
                    {
                        clothed.Add(v);
                    }
                }
            }
            return clothed.ToArray();
        }

        public bool IsClothedVertex(int vKey)
        {
            ITopologicVertex v = iM.GetVertexWithKey(vKey);
            for (int i = 0; i < v.V2HF.Length; i++)
            {
                if (v.V2HF[i] != 0)
                {
                    IElement e = iM.GetElementWithKey(v.GetElementID(i));
                    Int64 sibhe = e.GetSiblingHalfFacet(v.GetHalfFacetID(i));
                    if (sibhe != 0) return true;
                }
            }
            return false;
        }

        public Boolean IsCornerVertex(int vKey)
        {
            int[] data = iM.Topology.GetVertexIncidentElements(vKey);
            if (data.Length == 1) return true;
            else return false;
        }

        public int[] GetVertexCornersID()
        {
            int[] temp = GetNakedVerticesID();
            List<int> corners = new List<int>();
            foreach(int vKey in temp)
            {
                if (IsCornerVertex(vKey)) corners.Add(vKey);
            }
            return corners.ToArray();
        }

        public ITopologicVertex[] GetVertexCorners()
        {
            ITopologicVertex[] temp = GetNakedVertices();
            List<ITopologicVertex> corners = new List<ITopologicVertex>();

            foreach (ITopologicVertex v in temp)
            {
                if (IsCornerVertex(v.Key)) corners.Add(v);
            }
            return corners.ToArray();
        }

        public int[] GetVertexIncidentElements(int vertexKey)
        {
            HashSet<Int32> neighbor = new HashSet<Int32>();
            iM.CleanElementsVisits();

            Int64[] nK;
            int key;

            ITopologicVertex v = iM.GetVertexWithKey(vertexKey);

            for (int i = 0; i < v.V2HF.Length; i++)
            {

                if (v.V2HF[i] != 0)
                {

                    HashSet<int> oldK = new HashSet<int>() { iM.GetVertexWithKey(vertexKey).GetElementID(i) };
                    HashSet<int> newK;

                    while (oldK.Count() > 0)
                    {
                        newK = new HashSet<int>();

                        foreach (int eK in oldK)
                        {
                            neighbor.Add(eK);
                            nK = iM.GetElementWithKey(eK).GetSiblingHalfFacets();

                            foreach (Int64 eData in nK)
                            {
                                if (eData != 0)
                                {
                                    key = (Int32)(eData >> 32);

                                    IElement e = iM.GetElementWithKey(key);

                                    if (e.Vertices.Contains(vertexKey) && !neighbor.Contains(key))
                                    {
                                        newK.Add(key);
                                    }
                                }
                            }
                        }

                        oldK = newK;
                    }
                }
            }
            return neighbor.ToArray();
        }

        public int[] GetVertexAdjacentVertices(int vertexKey)
        {
            HashSet<Int32> neighbor = new HashSet<Int32>();
            HashSet<Int32> vNeighbor = new HashSet<Int32>();
            iM.CleanElementsVisits();

            Int64[] nK;
            int key;
            IElement e, nE;

            ITopologicVertex v = iM.GetVertexWithKey(vertexKey);

            for (int i = 0; i < v.V2HF.Length; i++)
            {
                if (v.V2HF[i] != 0)
                {
                    HashSet<int> oldK = new HashSet<int>() { iM.GetVertexWithKey(vertexKey).GetElementID(i) };
                    HashSet<int> newK;
                    while (oldK.Count() > 0)
                    {

                        newK = new HashSet<int>();

                        foreach (int eK in oldK)
                        {
                            neighbor.Add(eK);
                            e = iM.GetElementWithKey(eK);

                            for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                            {
                                int[] hf;
                                e.GetHalfFacet(halfFacetID, out hf);

                                if (e.TopologicDimension == 1 && e.Vertices.Contains(vertexKey))
                                {
                                    if (!vNeighbor.Contains(hf[0]) && hf[0] != vertexKey) vNeighbor.Add(hf[0]);
                                }
                                else
                                {
                                    if (hf.Contains(vertexKey))
                                    {

                                        int hfIdx = hf.ToList().IndexOf(vertexKey);

                                        int next = hfIdx + 1;
                                        int prev = hfIdx - 1;

                                        if (next > hf.Length - 1) next = 0;
                                        if (prev < 0) prev = hf.Length - 1;

                                        int nextV = hf[next];
                                        int prevV = hf[prev];

                                        if (!vNeighbor.Contains(nextV) && nextV != vertexKey) vNeighbor.Add(nextV);
                                        if (!vNeighbor.Contains(prevV) && prevV != vertexKey) vNeighbor.Add(prevV);
                                    }
                                }
                            }

                            //Check for siblings
                            nK = e.GetSiblingHalfFacets();
                            foreach (Int64 eData in nK)
                            {
                                if (eData != 0)
                                {
                                    key = (Int32)(eData >> 32);

                                    nE = iM.GetElementWithKey(key);

                                    if (nE.Vertices.Contains(vertexKey) && !neighbor.Contains(key))
                                    {
                                        newK.Add(key);
                                    }
                                }
                            }
                        }

                        oldK = newK;
                    }
                }
            }
            return vNeighbor.ToArray();
        }

        public int[] GetVertexIncidentElementsSorted(int vertexKey)
        {
            int[] nKeys = iM.Topology.GetVertexIncidentElements(vertexKey);

            if (nKeys.Length > 1)
            {

                IPoint3D[] refPt = new IPoint3D[2];
                refPt[0] = ComputeAveragePosition(iM.GetElementWithKey(nKeys[0]).Vertices);
                refPt[1] = ComputeAveragePosition(iM.GetElementWithKey(nKeys[1]).Vertices);

                IVector3D refVec = refPt[1] - refPt[0];

                List<int> results = nKeys.Skip(1).OrderBy(nK =>
                    IVector3D.AngleBetween(refVec, ComputeAveragePosition(iM.GetElementWithKey(nK).Vertices) - refPt[0])
                ).ToList();

                results.Insert(0, nKeys[0]);
                nKeys = results.ToArray();
            }
            return nKeys;
        }

        public int[] GetVertexAdjacentVerticesSorted(int vertexKey)
        {
            int[] nKeys = iM.Topology.GetVertexAdjacentVertices(vertexKey);

            if (nKeys.Length > 1)
            {
                IPoint3D[] refPt = new IPoint3D[2];
                refPt[0] = iM.GetVertexWithKey(nKeys[0]).Position;
                refPt[1] = iM.GetVertexWithKey(nKeys[1]).Position;

                IVector3D refVec = refPt[1] - refPt[0];

                List<int> results = nKeys.Skip(1).OrderBy(nK =>
                    IVector3D.AngleBetween(refVec, iM.GetVertexWithKey(nK).Position - refPt[0])
                ).ToList();

                results.Insert(0, nKeys[0]);
                nKeys = results.ToArray();
            }
            return nKeys;
        }

        public ITopologicEdge[] GetNakedEdges()
        {
            List<ITopologicEdge> naked = new List<ITopologicEdge>();

            foreach (int eK in iM.ElementsKeys)
            {

                IElement e = iM.GetElementWithKey(eK);

                if (e.TopologicDimension == 2)
                {
                    Int64[] sibhf = e.GetSiblingHalfFacets();
                    int vk1, vk2;
                    for (int i = 0; i < sibhf.Length; i++)
                    {
                        Int64 hf = sibhf[i];
                        if (hf == 0)
                        {
                            int A = i;
                            int B = i + 1;
                            if (i == sibhf.Length - 1) B = 0;
                            vk1 = e.Vertices[A];
                            vk2 = e.Vertices[B];
                            naked.Add(new ITopologicEdge(iM.GetVertexWithKey(vk1), iM.GetVertexWithKey(vk2)));
                        }
                    }
                }
                else if (e.TopologicDimension == 1)
                {
                    Int64[] sibhf = e.GetSiblingHalfFacets();
                    Int64 hf = sibhf[0];
                    if (hf == 0)
                    {
                        int vk1 = e.Vertices[0];
                        int vk2 = e.Vertices[1];
                        naked.Add(new ITopologicEdge(iM.GetVertexWithKey(vk1), iM.GetVertexWithKey(vk2)));
                    }
                }
            }

            return naked.ToArray();
        }

        public int[] GetNakedVerticesID()
        {
            List<int> naked = new List<int>();
            foreach (int vKey in iM.VerticesKeys)
            {
                ITopologicVertex v = iM.GetVertexWithKey(vKey);
                if (!v.IsIsolated())
                {
                    if (IsClothedVertex(v.Key))
                    {
                        naked.Add(vKey);
                    }
                }
            }
            return naked.ToArray();
        }

        public bool IsNakedVertex(int vKey)
        {
            ITopologicVertex v = iM.GetVertexWithKey(vKey);
            if (!v.IsIsolated())
            {
                if (!IsClothedVertex(v.Key)) return true;
            }
            return false;
        }



        public ITopologicVertex[] GetNakedVertices()
        {
            List<ITopologicVertex> naked = new List<ITopologicVertex>();
            foreach (int vKey in iM.VerticesKeys)
            {
                ITopologicVertex v = iM.GetVertexWithKey(vKey);

                if (!v.IsIsolated())
                {
                    if (!IsClothedVertex(v.Key)) naked.Add(v);
                }
            }
            return naked.ToArray();
        }

        public Line[] GetUniqueEdgesAsLines()
        {
            List<Line> edges = new List<Line>();
            List<long> edgesID = new List<long>();
            long data1, data2;
            Point3d p1, p2;
            int next, count;
            int[] hf;

            foreach (int elementID in iM.ElementsKeys)
            {
                IElement e = iM.GetElementWithKey(elementID);

                for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                {
                    e.GetHalfFacet(halfFacetID, out hf);

                    count = 1;
                    if (e.TopologicDimension == 3) count = hf.Length;
                    for (int i = 0; i < count; i++)
                    {
                        next = i + 1;
                        if (i == count - 1)
                        {
                            if (count > 1) next = 0;
                            else next = 1;
                        }
                        data1 = (Int64)hf[i] << 32 | (Int64)hf[next];
                        data2 = (Int64)hf[next] << 32 | (Int64)hf[i];
                        if (!edgesID.Contains(data1) && !edgesID.Contains(data2))
                        {
                            p1 = iM.GetVertexWithKey(hf[i]).RhinoPoint;
                            p2 = iM.GetVertexWithKey(hf[next]).RhinoPoint;
                            edges.Add(new Line(p1, p2));
                            edgesID.Add(data1);
                        }
                    }
                }
            }
            return edges.ToArray();
        }

        public long[] GetUniqueEdges()
        {
            List<long> edgesID = new List<long>();
            long data1, data2;
            int next, count;
            int[] hf;

            foreach (int elementID in iM.ElementsKeys)
            {
                IElement e = iM.GetElementWithKey(elementID);

                for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                {
                    e.GetHalfFacet(halfFacetID, out hf);

                    count = 1;
                    if (e.TopologicDimension == 3) count = hf.Length;
                    for (int i = 0; i < count; i++)
                    {
                        next = i + 1;
                        if (i == count - 1)
                        {
                            if (count > 1) next = 0;
                            else next = 1;
                        }
                        data1 = (Int64)hf[i] << 32 | (Int64)hf[next];
                        data2 = (Int64)hf[next] << 32 | (Int64)hf[i];
                        if (!edgesID.Contains(data1) && !edgesID.Contains(data2))
                        {
                            edgesID.Add(data1);
                        }
                    }
                }
            }
            return edgesID.ToArray();
        }

        public ITopologicEdge[] GetTopologicEdges()
        {
            List<ITopologicEdge> edges = new List<ITopologicEdge>();
            List<long> edgesID = new List<long>();
            long data1, data2;
            ITopologicVertex v1, v2;
            int next, count;
            int[] hf;

            foreach (int elementID in iM.ElementsKeys)
            {
                IElement e = iM.GetElementWithKey(elementID);

                if (e.TopologicDimension > 1)
                {
                    for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                    {
                        e.GetHalfFacet(halfFacetID, out hf);

                        count = 1;
                        if (e.TopologicDimension == 3) count = hf.Length;
                        for (int i = 0; i < count; i++)
                        {
                            next = i + 1;
                            if (i == count - 1)
                            {
                                if (count > 1) next = 0;
                                else next = 1;
                            }
                            data1 = (Int64)hf[i] << 32 | (Int64)hf[next];
                            data2 = (Int64)hf[next] << 32 | (Int64)hf[i];
                            if (!edgesID.Contains(data1) && !edgesID.Contains(data2))
                            {
                                v1 = iM.GetVertexWithKey(hf[i]);
                                v2 = iM.GetVertexWithKey(hf[next]);
                                edges.Add(new ITopologicEdge(v1, v2));
                                edgesID.Add(data1);
                            }
                        }
                    }
                }
                else
                {
                    data1 = (Int64)e.Vertices[0] << 32 | (Int64)e.Vertices[1];
                    data2 = (Int64)e.Vertices[1] << 32 | (Int64)e.Vertices[0];
                    if (!edgesID.Contains(data1) && !edgesID.Contains(data2))
                    {
                        v1 = iM.GetVertexWithKey(e.Vertices[0]);
                        v2 = iM.GetVertexWithKey(e.Vertices[1]);
                        edges.Add(new ITopologicEdge(v1, v2));
                        edgesID.Add(data1);
                    }
                }
            }
            return edges.ToArray();
        }

        /// <summary>
        /// Return all incidents elements of an edge.
        /// </summary>
        /// <param name="vKey1"> First vertex of the edge </param>
        /// <param name="vKey2"> Second vertex of the edge </param>
        /// <returns></returns>
        public int[] GetEdgeIncidentElements(int vKey1, int vKey2)
        {
            var neighbor = new int[0];

            if (iM.ContainsVertexKey(vKey1) && iM.ContainsVertexKey(vKey2))
            {
                iM.CleanElementsVisits();
                IElement e;

                int[] vK = new int[] { vKey1, vKey2 };

                HashSet<int> result = new HashSet<int>();
                for (int i = 0; i < vK.Length; i++)
                {
                    int next = i + 1;
                    if (i == vK.Length - 1) next = 0;
                    int[] eKeys = iM.Topology.GetVertexIncidentElements(vK[i]);

                    foreach (int eK in eKeys)
                    {
                        e = iM.GetElementWithKey(eK);
                        if ((e.TopologicDimension > 1 && e.Vertices.Contains(vK[next])) || e.TopologicDimension == 1)
                        {
                            result.Add(eK);
                        }
                    }
                }

                neighbor = result.ToArray();
            }
            return neighbor;
        }

        public bool IsNakedEdge(int start, int end)
        {
            int[] n = GetEdgeIncidentElements(start, end);
            if (n.Length <= 1) return true;
            else return false;
        }

        public void ComputeElementNormals(IVector3D[] normals, IVector3D[] centers, out int[] eKeys)
        {
            IElement e, ee;
            IVector3D n;
            IVector3D pos;
            List<IVector3D> norm_List = new List<IVector3D>();
            List<IVector3D> pos_List = new List<IVector3D>();
            List<int> keys_List = new List<int>();

            foreach (int eK in iM.ElementsKeys)
            {
                e = iM.GetElementWithKey(eK);
                if (!e.Visited)
                {
                    int[] nKeys = iM.Topology.GetElementAdjacentElements(eK);
                    foreach (int nK in nKeys)
                    {
                        ee = iM.GetElementWithKey(nK);
                        if (!ee.Visited)
                        {
                            iM.Topology.ComputeTwoDimensionalElementNormal(nK, out n, out pos);
                            norm_List.Add(n);
                            pos_List.Add(pos);
                            keys_List.Add(nK);
                            ee.Visited = true;
                        }
                    }
                    e.Visited = true;
                }
            }

            normals = norm_List.ToArray();
            centers = pos_List.ToArray();
            eKeys = keys_List.ToArray();
        }

        public void ComputeElementNormals(out Vector3d[] normals, out Point3d[] centers, out int[] eKeys)
        {
            IElement e, ee;
            Vector3d n;
            Point3d pos;
            List<Vector3d> norm_List = new List<Vector3d>();
            List<Point3d> pos_List = new List<Point3d>();
            List<int> keys_List = new List<int>();

            foreach (int eK in iM.ElementsKeys)
            {
                e = iM.GetElementWithKey(eK);
                if (!e.Visited)
                {
                    int[] nKeys = iM.Topology.GetElementAdjacentElements(eK);
                    foreach (int nK in nKeys)
                    {
                        ee = iM.GetElementWithKey(nK);
                        if (!ee.Visited)
                        {
                            iM.Topology.ComputeTwoDimensionalElementNormal(nK, out n, out pos);
                            norm_List.Add(n);
                            pos_List.Add(pos);
                            keys_List.Add(nK);
                            ee.Visited = true;
                        }
                    }
                    e.Visited = true;
                }
            }

            normals = norm_List.ToArray();
            centers = pos_List.ToArray();
            eKeys = keys_List.ToArray();
        }

        public bool ComputeTwoDimensionalElementNormal(int eKey, out IVector3D normal, out IVector3D position)
        {
            IElement e = iM.GetElementWithKey(eKey);
            normal = new IVector3D();
            position = new IVector3D();
            if (e.TopologicDimension == 2)
            {
                IVector3D v1, v2;
                ITopologicVertex vv0, vv1, vv2;
                int prev_i, next_i;
                for(int i=0; i < e.VerticesCount; i++)
                {
                    prev_i = i - 1;
                    if (i == 0) prev_i = e.VerticesCount - 1;
                    next_i = i + 1;
                    if (i == e.VerticesCount - 1) next_i = 0;

                    vv0 = iM.GetVertexWithKey(e.Vertices[i]);
                    vv1 = iM.GetVertexWithKey(e.Vertices[prev_i]);
                    vv2 = iM.GetVertexWithKey(e.Vertices[next_i]);

                    v1 = vv1.Position - vv0.Position;
                    v2 = vv2.Position - vv0.Position;

                    normal += v1 * v2;
                    position += vv0.Position;
                }
                normal /= e.VerticesCount;
                position /= e.VerticesCount;
                normal.Norm();
                return true;
            }
            return false;
        }

        public bool ComputeTwoDimensionalElementNormal(int eKey, out Vector3d normal, out Point3d position)
        {
            IElement e = iM.GetElementWithKey(eKey);
            normal = new Vector3d();
            position = new Point3d();
            if (e.TopologicDimension == 2)
            {
                Vector3d v1, v2;
                ITopologicVertex vv0, vv1, vv2;
                int prev_i, next_i;
                for (int i = 0; i < e.VerticesCount; i++)
                {
                    prev_i = i - 1;
                    if (i == 0) prev_i = e.VerticesCount - 1;
                    next_i = i + 1;
                    if (i == e.VerticesCount - 1) next_i = 0;

                    vv0 = iM.GetVertexWithKey(e.Vertices[i]);
                    vv1 = iM.GetVertexWithKey(e.Vertices[prev_i]);
                    vv2 = iM.GetVertexWithKey(e.Vertices[next_i]);

                    v1 = vv1.RhinoPoint - vv0.RhinoPoint;
                    v2 = vv2.RhinoPoint - vv0.RhinoPoint;

                    normal += Vector3d.CrossProduct(v1,v2);
                    position += vv0.RhinoPoint;
                }
                normal /= e.VerticesCount;
                position /= e.VerticesCount;
                normal.Unitize();
                return true;
            }
            return false;
        }

        public Int64[] GetHalfFacetIncidentHalfFacets(Int64 hf_Key)
        {
            List<Int64> neighbor = new List<Int64>();
            iM.CleanElementsVisits();

            Int64 keyPair = hf_Key;
            Int64 nK;
            int eKey, hfKey;
            while (!neighbor.Contains(keyPair))
            {
                eKey = (Int32)(hf_Key >> 32);
                hfKey = (Int32)hf_Key;
                neighbor.Add(eKey);
                nK = iM.GetElementWithKey(eKey).GetSiblingHalfFacet(hfKey);

                if (nK != 0) keyPair = nK;
            }
            return neighbor.ToArray();
        }

        public int[] GetElementAdjacentElements(int eKey)
        {
            HashSet<Int32> result = new HashSet<Int32>();
            iM.CleanElementsVisits();

            Int64[] nK;
            int key;

            IElement e = iM.GetElementWithKey(eKey);

            foreach (int vertexKey in e.Vertices)
            {
                HashSet<Int32> neighbor = new HashSet<Int32>();

                ITopologicVertex v = iM.GetVertexWithKey(vertexKey);
                for (int i = 0; i < v.V2HF.Length; i++)
                {
                    if (v.V2HF[i] != 0)
                    {
                        HashSet<int> oldK = new HashSet<int>() { v.GetElementID(i) };
                        HashSet<int> newK;

                        while (oldK.Count() > 0)
                        {
                            newK = new HashSet<int>();

                            foreach (int eK in oldK)
                            {
                                if (eK != eKey) result.Add(eK);
                                neighbor.Add(eK);

                                nK = iM.GetElementWithKey(eK).GetSiblingHalfFacets();

                                foreach (Int64 eData in nK)
                                {
                                    if (eData != 0)
                                    {
                                        key = (Int32)(eData >> 32);

                                        IElement eN = iM.GetElementWithKey(key);

                                        if (eN.Vertices.Contains(vertexKey) && !neighbor.Contains(key))
                                        {
                                            newK.Add(key);
                                        }
                                    }
                                }
                            }

                            oldK = newK;
                        }
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Return the cosine dihedral angles among pairs of two-dimensional elements incident to the given edge. 
        /// More than one dihedral angle can be returned since the data structure supports non-manifold meshes.
        /// </summary>
        /// <param name="vKey1"></param>
        /// <param name="vKey2"></param>
        /// <returns></returns>
        public double[] ComputeEdgeCosDihedralAngle(int vKey1, int vKey2)
        {
            int[] eKey = GetEdgeIncidentElements(vKey1, vKey2);
            double[] dA = new double[0];
            if (eKey.Length > 1)
            {
                IVector3D n1, n2, p1, p2;
                double d;
                int count = eKey.Length - 1;
                int next_i;
                if (eKey.Length > 2) count = eKey.Length;
                dA = new double[count];
                for (int i = 0; i < count; i++)
                {
                    next_i = i + 1;
                    if (i == eKey.Length - 1) next_i = 0;
                    ComputeTwoDimensionalElementNormal(eKey[i], out n1, out p1);
                    ComputeTwoDimensionalElementNormal(eKey[next_i], out n2, out p2);
                    d = IVector3D.Dot(n1, n2);
                    if (d > 1) d = 1;
                    else if (d < -1) d = -1;
                    dA[i] = d;
                }
            }
            return dA;
        }

        /// <summary>
        /// Return the dihedral angles among pairs of two-dimensional elements incident to the given edge. 
        /// More than one dihedral angle can be returned since the data structure supports non-manifold meshes.
        /// </summary>
        /// <param name="vKey1"></param>
        /// <param name="vKey2"></param>
        /// <returns></returns>
        public double[] ComputeEdgeDihedralAngle(int vKey1, int vKey2)
        {
            double[] temp = ComputeEdgeCosDihedralAngle(vKey1, vKey2);
            double[] dA = new double[temp.Length];
            for(int i=0; i<temp.Length; i++)
            {
                dA[i] = Math.Acos(temp[i]);
            }
            return dA;
        }

        /// <summary>
        /// Computes the area of a two-dimensional element. 
        /// </summary>
        /// <param name="eKey"></param>
        /// <returns></returns>
        public double ComputeTwoDimensionalElementArea(int eKey)
        {
            IElement e = iM.GetElementWithKey(eKey);
            if (e.TopologicDimension != 2) return 0;

            IVector3D n, pos;
            ComputeTwoDimensionalElementNormal(eKey, out n, out pos);

            if (n.Mag() < 0.5) return 0;

            double x = Math.Abs(n.X);
            double y = Math.Abs(n.Y);
            double z = Math.Abs(n.Z);
            double area = 0;
            int coord = 3;
            if (x >= y && x >= z) coord = 1;
            else if (y >= x && y >= z) coord = 2;
            int prev_i, next_i;
            ITopologicVertex prevV, v, nextV;
            for(int i=0; i<e.VerticesCount; i++)
            {
                prev_i = i - 1;
                if (i == 0) prev_i = e.VerticesCount - 1;
                next_i = i + 1;
                if (i == e.VerticesCount - 1) next_i = 0;
                prevV = iM.GetVertexWithKey(e.Vertices[prev_i]);
                v = iM.GetVertexWithKey(e.Vertices[i]);
                nextV = iM.GetVertexWithKey(e.Vertices[next_i]);

                switch (coord)
                {
                    case 1:
                        area += v.Y * (nextV.Z - prevV.Z);
                        break;
                    case 2:
                        area += v.X * (nextV.Z - prevV.Z);
                        break;
                    case 3:
                        area += v.X * (nextV.Y - prevV.Y);
                        break;
                }
            }

            switch (coord)
            {
                case 1:
                    area *= 0.5 / x;
                    break;
                case 2:
                    area *= 0.5 / y;
                    break;
                case 3:
                    area *= 0.5 / z;
                    break;
            }
            return Math.Abs(area);
        }

        public double ComputePolygonArea(IPoint3D[] polygon)
        {
            if (polygon.Length <= 2) return 0;

            IVector3D n, pos;
            ComputePolygonNormal(polygon, out n, out pos);

            if (n.Mag() < 0.5) return 0;

            double x = Math.Abs(n.X);
            double y = Math.Abs(n.Y);
            double z = Math.Abs(n.Z);
            double area = 0;
            int coord = 3;
            if (x >= y && x >= z) coord = 1;
            else if (y >= x && y >= z) coord = 2;
            int prev_i, next_i;
            IVector3D prevV = new IVector3D();
            IVector3D v = new IVector3D();
            IVector3D nextV = new IVector3D();
            for (int i = 0; i < polygon.Length; i++)
            {
                prev_i = i - 1;
                if (i == 0) prev_i = polygon.Length - 1;
                next_i = i + 1;
                if (i == polygon.Length - 1) next_i = 0;
                prevV += polygon[prev_i];
                v += polygon[i];
                nextV += polygon[next_i];

                switch (coord)
                {
                    case 1:
                        area += v.Y * (nextV.Z - prevV.Z);
                        break;
                    case 2:
                        area += v.X * (nextV.Z - prevV.Z);
                        break;
                    case 3:
                        area += v.X * (nextV.Y - prevV.Y);
                        break;
                }
            }

            switch (coord)
            {
                case 1:
                    area *= 0.5 / x;
                    break;
                case 2:
                    area *= 0.5 / y;
                    break;
                case 3:
                    area *= 0.5 / z;
                    break;
            }
            return Math.Abs(area);
        }

        public double ComputeTwoDimensionalHalfFacetArea(int[] hf)
        {
            if (hf.Length <= 2) return 0;

            IVector3D n, pos;
            ComputeTwoDimensionalHalfFacetNormal(hf, out n, out pos);

            if (n.Mag() < 0.5) return 0;

            double x = Math.Abs(n.X);
            double y = Math.Abs(n.Y);
            double z = Math.Abs(n.Z);
            double area = 0;
            int coord = 3;
            if (x >= y && x >= z) coord = 1;
            else if (y >= x && y >= z) coord = 2;
            int prev_i, next_i;
            ITopologicVertex prevV, v, nextV;
            for (int i = 0; i < hf.Length; i++)
            {
                prev_i = i - 1;
                if (i == 0) prev_i = hf.Length - 1;
                next_i = i + 1;
                if (i == hf.Length - 1) next_i = 0;
                prevV = iM.GetVertexWithKey(hf[prev_i]);
                v = iM.GetVertexWithKey(hf[i]);
                nextV = iM.GetVertexWithKey(hf[next_i]);

                switch (coord)
                {
                    case 1:
                        area += v.Y * (nextV.Z - prevV.Z);
                        break;
                    case 2:
                        area += v.X * (nextV.Z - prevV.Z);
                        break;
                    case 3:
                        area += v.X * (nextV.Y - prevV.Y);
                        break;
                }
            }

            switch (coord)
            {
                case 1:
                    area *= 0.5 / x;
                    break;
                case 2:
                    area *= 0.5 / y;
                    break;
                case 3:
                    area *= 0.5 / z;
                    break;
            }
            return Math.Abs(area);
        }

        public double ComputeEdgeArea(int vKey1, int vKey2)
        {
            int[] eKeys = GetEdgeIncidentElements(vKey1, vKey2);
            if (eKeys.Length == 0) return 0;
            IElement e;
            double area = 0;
            int count = 0;
            eKeys.All(eK =>
            {
                e = iM.GetElementWithKey(eK);
                if (e.TopologicDimension == 2)
                {
                    area += ComputeTwoDimensionalElementArea(eK);
                    count++;
                }
                else
                {
                    int[] hf;
                    e.GetHalfFacet(e.GetHalFacetContainingVertices(new[] { vKey1, vKey2 }), out hf);
                    area += ComputeTwoDimensionalHalfFacetArea(hf);
                    count++;
                }
                return true;
            });
            area /= count;
            return area;
        }

        public bool ComputeTwoDimensionalHalfFacetNormal(int[] hf, out IVector3D normal, out IVector3D position)
        {
            normal = new IVector3D();
            position = new IVector3D();
            if (hf.Length > 2)
            {
                IVector3D v1, v2;
                ITopologicVertex vv0, vv1, vv2;
                int prev_i, next_i;
                for (int i = 0; i < hf.Length; i++)
                {
                    prev_i = i - 1;
                    if (i == 0) prev_i = hf.Length - 1;
                    next_i = i + 1;
                    if (i == hf.Length - 1) next_i = 0;

                    vv0 = iM.GetVertexWithKey(hf[i]);
                    vv1 = iM.GetVertexWithKey(hf[prev_i]);
                    vv2 = iM.GetVertexWithKey(hf[next_i]);

                    v1 = vv1.Position - vv0.Position;
                    v2 = vv2.Position - vv0.Position;

                    normal += v1 * v2;
                    position += vv0.Position;
                }
                normal /= hf.Length;
                position /= hf.Length;
                normal.Norm();
                return true;
            }
            return false;
        }

        public bool ComputePolygonNormal(IPoint3D[] polygon, out IVector3D normal, out IVector3D position)
        {
            normal = new IVector3D();
            position = new IVector3D();
            if (polygon.Length > 2)
            {
                IVector3D v1, v2;
                IVector3D vv0 = new IVector3D();
                IVector3D vv1 = new IVector3D();
                IVector3D vv2 = new IVector3D();
                int prev_i, next_i;
                for (int i = 0; i < polygon.Length; i++)
                {
                    prev_i = i - 1;
                    if (i == 0) prev_i = polygon.Length - 1;
                    next_i = i + 1;
                    if (i == polygon.Length - 1) next_i = 0;

                    vv0 += polygon[i];
                    vv1 += polygon[prev_i];
                    vv2 += polygon[next_i];

                    v1 = vv1 - vv0;
                    v2 = vv2 - vv0;

                    normal += v1 * v2;
                    position += vv0;
                }
                normal /= polygon.Length;
                position /= polygon.Length;
                normal.Norm();
                return true;
            }
            return false;
        }

        public bool ComputeEdgeNormal(int vKey1, int vKey2, out IVector3D normal, out IVector3D center)
        {
            int[] eKeys = GetEdgeIncidentElements(vKey1, vKey2);
            normal = new IVector3D();
            center = iM.GetVertexWithKey(vKey1).Position + iM.GetVertexWithKey(vKey2).Position;
            center /= 2;

            if (eKeys.Length == 0) return false;

            foreach(int eK in eKeys)
            {
                IVector3D n, p;
                ComputeTwoDimensionalElementNormal(eK, out n, out p);
                normal += n;
            }
            normal /= eKeys.Length;
            normal.Norm();

            return true;
        }

        public void ComputeEdgeNormals(out IVector3D[] normals, out IVector3D[] centers, out ITopologicEdge[] edges)
        {
            Int64[] eKeys = iM.Topology.GetUniqueEdges();
            edges = new ITopologicEdge[eKeys.Length];
            normals = new IVector3D[eKeys.Length];
            centers = new IVector3D[eKeys.Length];
            int start, end;
            for(int i=0; i<eKeys.Length; i++)
            {
                Int64 eK = eKeys[i];
                IHelpers.UnpackKey(eK, out start, out end);
                edges[i] = new ITopologicEdge(iM.GetVertexWithKey(start), iM.GetVertexWithKey(end));
                iM.Topology.ComputeEdgeNormal(start, end, out normals[i], out centers[i]);
            }
        }

        public void ComputeEdgeNormals(out Vector3d[] normals, out Point3d[] centers, out ITopologicEdge[] edges)
        {
            Int64[] eKeys = iM.Topology.GetUniqueEdges();
            edges = new ITopologicEdge[eKeys.Length];
            normals = new Vector3d[eKeys.Length];
            centers = new Point3d[eKeys.Length];
            int start, end;
            IVector3D vv,pp;
            for (int i = 0; i < eKeys.Length; i++)
            {
                Int64 eK = eKeys[i];
                IHelpers.UnpackKey(eK, out start, out end);
                edges[i] = new ITopologicEdge(iM.GetVertexWithKey(start), iM.GetVertexWithKey(end));
                
                iM.Topology.ComputeEdgeNormal(start, end, out vv, out pp);
                normals[i] = new Vector3d(vv.X, vv.Y, vv.Z);
                centers[i] = new Point3d(pp.X, pp.Y, pp.Z);
            }
        }

        public IVector3D GetHalfFacetCenter(int[] hf)
        {
            IVector3D center = new IVector3D();
            foreach (int eV in hf)
            {
                center += iM.GetVertexWithKey(eV).Position;
            }
            center /= hf.Length;
            return center;
        }

        public double ComputeVertexArea(int vKey)
        {
            double area = 0;
            int[] eKeys = GetVertexIncidentElements(vKey);
            eKeys.All(eK =>
            {
                area += ComputeTwoDimensionalElementArea(eK);
                return true;
            });
            return area;
        }

        /// <summary>
        /// Computes the barycentric area of a given vertex. 
        /// </summary>
        /// <param name="vKey"> Key of the vertex. </param>
        /// <returns></returns>
        public double ComputeBarycentricVertexArea(int vKey)
        {
            int[] nKeys = GetVertexAdjacentVertices(vKey);
            IPoint3D v = iM.GetVertexWithKey(vKey).Position;
            IPoint3D vv1, vv2;

            int next_i;
            double area = 0;
            for(int i=0; i < nKeys.Length; i++)
            {
                next_i = i + 1;
                if (i == nKeys.Length - 1) next_i = 0;

                vv1 = ComputeAveragePosition(new int[]{nKeys[i], vKey});
                vv2 = ComputeAveragePosition(new int[]{ nKeys[next_i], vKey });
                area += ComputePolygonArea(new IPoint3D[]{vv1,vv2,v});                
            }
            area /= nKeys.Length;

            return area;
        }

        /// <summary>
        /// Sum of all surface areas of two-dimensional elements incident to the given vertex.
        /// Return 0 when the vertex is not incident to a two-dimensional element.
        /// </summary>
        /// <param name="vKey"> The key of the vertex. </param>
        /// <returns></returns>
        public double ComputeSurfaceElementVertexArea(int vKey)
        {
            int[] eKeys = GetVertexIncidentElements(vKey);
            double area = 0;
            IElement e;
            for (int i = 0; i < eKeys.Length; i++)
            {
                e = iM.GetElementWithKey(eKeys[i]);
                if (e.TopologicDimension == 2)
                {
                    IPoint3D[] pts = new IPoint3D[e.VerticesCount];
                    for(int j=0; j< e.VerticesCount; j++)
                    {
                        pts[j] = iM.GetVertexWithKey(e.Vertices[j]).Position;
                    }
                    area += ComputePolygonArea(pts);
                }
            }

            return area;
        }

        internal IPoint3D ComputeAveragePosition(int[] keys)
        {
            IVector3D v = new IVector3D();
            for (int i = 0; i < keys.Length; i++) v += iM.GetVertexWithKey(keys[i]).Position;
            v /= keys.Length;
            return new IPoint3D(v.X,v.Y,v.Z);
        }

        /// <summary>
        /// Computes the normal vector at a vertex as the normalized weighted sum of the normals of incident faces.
        /// </summary>
        /// <param name="vKey"> Key of the vertex. </param>
        /// <returns></returns>
        public IVector3D ComputeVertexNormal(int vKey)
        {
            int[] eKeys = GetVertexIncidentElements(vKey);
            IVector3D normal = new IVector3D();
            IVector3D n, p;
            foreach (int eK in eKeys)
            {
                ComputeTwoDimensionalElementNormal(eK, out n, out p);
                normal += n;
            }
            normal.Norm();
            return normal;
        }

        /// <summary>
        /// Computes the normal vector at a vertex as the normalized weighted sum of the normals of incident faces,
        /// with weights proportional to the surface areas of the faces.
        /// </summary>
        /// <param name="vKey"> Key of the vertex. </param>
        /// <returns></returns>
        public IVector3D ComputeWeightedVertexNormal(int vKey)
        {
            int[] eKeys = GetVertexIncidentElements(vKey);
            IVector3D normal = new IVector3D();
            IVector3D n, p;
            double area;
            foreach (int eK in eKeys)
            {
                ComputeTwoDimensionalElementNormal(eK, out n, out p);
                area = ComputeTwoDimensionalElementArea(eK);
                n *= area;
                normal += n;
            }
            normal.Norm();
            return normal;
        }

        /// <summary>
        /// Estimates the curvature of a two-dimensional mesh.
        /// See: "Taubin, Gabriel. (1995). Estimating the tensor of curvature of a surface from a polyhedralapproximation. Computer Vision, IEEE International Conference on. 902-907. 10.1109/ICCV.1995.466840." 
        /// </summary>
        /// <param name="mesh"> Base mesh. </param>
        /// <param name="vKeys"> Keys of the vertices. </param>
        /// <param name="T1"> Maximum principal curvature directions. </param>
        /// <param name="T2"> Minimum principal curvature directions. </param>
        /// <param name="k1"> Maximum principal curvature values. </param>
        /// <param name="k2"> Minimum principal curvature values. </param>
        /// <param name="mean"> Mean curvatures. </param>
        /// <param name="gauss"> Gauss curvatures. </param>
        /// <returns></returns>
        public bool ComputeMeshCurvature(out int[] vKeys, out IVector3D[] T1, out IVector3D[] T2, out double[] k1, out double[] k2, out double[] mean, out double[] gauss)
        {
            if (!iM.IsSurfaceMesh)
            {
                vKeys = new int[0];
                T1 = new IVector3D[0];
                T2 = new IVector3D[0];
                k1 = new double[0];
                k2 = new double[0];
                mean = new double[0];
                gauss = new double[0];
                return false;
            }

            int count = iM.VerticesCount;
            vKeys = iM.VerticesKeys.ToArray();
            T1 = new IVector3D[count];
            T2 = new IVector3D[count];
            k1 = new double[count];
            k2 = new double[count];
            mean = new double[count];
            gauss = new double[count];

            for (int i=0; i<count; i++)
            {
                ComputeVertexCurvatureTensor(vKeys[i], out T1[i], out T2[i], out k1[i], out k2[i], out mean[i], out gauss[i]);
            }

            return true; 
        }

        /// <summary>
        /// Estimates the tensor of curvature at a given vertex (Only for two-diemensional meshes).
        /// See: "Taubin, Gabriel. (1995). Estimating the tensor of curvature of a surface from a polyhedralapproximation. Computer Vision, IEEE International Conference on. 902-907. 10.1109/ICCV.1995.466840." 
        /// </summary>
        /// <param name="mesh"> Base mesh. </param>
        /// <param name="vKey"> Key of the vertex. </param>
        /// <param name="T1"> Maximum principal curvature direction. </param>
        /// <param name="T2"> Minimum principal curvature direction. </param>
        /// <param name="k1"> Maximum principal curvature value. </param>
        /// <param name="k2"> Minimum principal curvature value. </param>
        /// <param name="mean"> Mean curvature. </param>
        /// <param name="gauss"> Gauss curvature. </param>
        /// <returns></returns>
        public bool ComputeVertexCurvatureTensor(int vKey, out IVector3D T1, out IVector3D T2, out double k1, out double k2, out double mean, out double gauss)
        {
            if (!iM.IsSurfaceMesh)
            {
                T1 = new IVector3D(0, 0, 0);
                T2 = new IVector3D(0, 0, 0);
                k1 = 0;
                k2 = 0;
                mean = 0;
                gauss = 0;
                return false;
            }

            int[] vertexStar = iM.Topology.GetVertexAdjacentVertices(vKey);
            IVector3D n = iM.Topology.ComputeWeightedVertexNormal(vKey);
            IMatrix I = IMatrix.Identity3x3Matrix;
            IMatrix N = new IMatrix(n);
            IMatrix Nt = N.Transpose();
            double area = 0, k;
            double[] weight = new double[vertexStar.Length];

            for (int i = 0; i < vertexStar.Length; i++)
            {
                int[] elementStar = iM.Topology.GetEdgeIncidentElements(vKey, vertexStar[i]);
                for (int j = 0; j < elementStar.Length; j++)
                {
                    weight[i] += iM.Topology.ComputeTwoDimensionalElementArea(elementStar[j]);
                }
                area += weight[i];
            }

            // Compute matrix M
            IMatrix M = new IMatrix(3, 3);
            for (int i = 0; i < vertexStar.Length; i++)
            {

                // Compute weight proportional to the sum of surface areas incident to both vertices Vi and Vj
                weight[i] /= area;

                // Get vector Vj - Vi
                IVector3D vec = iM.GetVertexWithKey(vertexStar[i]).Position - iM.GetVertexWithKey(vKey).Position;

                // T as the unit length normalized projection of the vector vj and vi over the normal plane.
                IVector3D t = new IVector3D((I - N * Nt) * vec);
                t.Norm();

                vec.Reverse();
                double[] data = (2 * Nt) * vec;
                k = data[0] / Math.Pow(vec.Mag(), 2);

                IMatrix T = new IMatrix(t);
                IMatrix Tt = T.Transpose();

                M += weight[i] * k * T * Tt;
            }

            // Compute the Householder matrix Q
            IVector3D E1 = new IVector3D(1, 0, 0);
            IVector3D w;
            if (1 - n.Mag() > 1 + n.Mag())
            {
                w = E1 - n;
            }
            else
            {
                w = E1 + n;
            }
            w.Norm();

            IMatrix W = new IMatrix(w);
            IMatrix Q = I - 2 * W * W.Transpose();
            IMatrix D = Q.Transpose() * M * Q;

            IVector3D t1 = new IVector3D(Q.GetColumn(1));
            IVector3D t2 = new IVector3D(Q.GetColumn(2));

            double m11 = D.GetData(1, 1);
            double m22 = D.GetData(2, 2);
            double m12 = D.GetData(1, 2);

            // Givens rotation
            double theta = 0.5 * Math.Atan2(2 * m11 * m12 + 2 * m12 * m22, m11 * m11 + m12 * m12 - m12 * m12 - m22 * m22);
            double c = Math.Cos(theta);
            double s = Math.Sin(theta);

            // Curvature directions
            T1 = c * t1 - s * t2;
            T2 = s * t1 + c * t2;

            // Curvature values
            k1 = 3 * m11 - m22;
            k2 = 3 * m22 - m11;
            mean = (k1 + k2) / 2;
            gauss = k1 * k2;

            return true;
        }
    }
}
