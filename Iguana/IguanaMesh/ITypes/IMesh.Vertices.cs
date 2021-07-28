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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh
    {
        public void CleanAllVerticesTopologicalData()
        {
            ITopologicVertex v;
            foreach (int vK in VerticesKeys)
            {
                v = _vertices[vK];
                v.CleanTopologicalData();
                _vertices[vK] = v;
            }
        }

        public void CullUnparsedNodes(IEnumerable<int> parsedNodes)
        {
            var cullNodes = VerticesKeys.Except(parsedNodes);
            cullNodes.All(vKey => _vertices.Remove(vKey));
        }

        public void AddVertex(int key, ITopologicVertex vertex)
        {
            if (key < 0) key = FindNextVertexKey();
            vertex.Key = key;
            _vertices.Add(key, vertex);
        }

        public void AddVertex(int key, Point3d point)
        {
            AddVertex(key, new ITopologicVertex(point.X, point.Y, point.Z));
        }

        public void AddVertex(int key, double x, double y, double z, double u, double v, double w)
        {
            AddVertex(key, new ITopologicVertex(x, y, z, u, v, w));
        }

        public int FindNextVertexKey()
        {
            List<int> keys = VerticesKeys;
            keys.Sort();
            if (VerticesKeys.Count == 0) return 1;
            else return keys[keys.Count - 1] + 1;
        }

        public void SetVertex(int key, ITopologicVertex vertex)
        {
            _vertices[key] = vertex;
        }

        public void SetVertexPosition(int key, Point3d point, bool updateGraphics = false)
        {
            ITopologicVertex v = _vertices[key];
            v.Position = new IPoint3D(point.X, point.Y, point.Z);
            _vertices[key] = v;
            if (updateGraphics) UpdateGraphics();
        }

        public void SetVertexPosition(int key, IPoint3D position, bool updateGraphics = false)
        {
            ITopologicVertex v = _vertices[key];
            v.Position = position;
            _vertices[key] = v;
            if (updateGraphics) UpdateGraphics();
        }


        public void SetVertexPosition(int key, double x, double y, double z, bool updateGraphics = false)
        {
            ITopologicVertex v = _vertices[key];
            v.Position = new IPoint3D(x, y, z);
            _vertices[key] = v;
            if(updateGraphics) UpdateGraphics();
        }

        public void SetVertexTextureCoordinates(int key, double u, double v, double w)
        {
            ITopologicVertex vertex = _vertices[key];
            vertex.TextureCoordinates = new double[] { u, v, w };
            _vertices[key] = vertex;
        }

        public void CleanVertices()
        {
            _vertices = new Dictionary<int, ITopologicVertex>();
        }

        public void AddRangeVertices(IEnumerable<ITopologicVertex> vertices)
        {
            foreach (ITopologicVertex v in vertices)
            {
                AddVertex(v.Key, v);
            }
        }

        public void DeleteVertex(int vKey, bool updateTopology = true)
        {
            int[] eKeys = Topology.GetVertexIncidentElements(vKey);
            foreach (int eK in eKeys)
            {
                DeleteElement(eK, false);
            }

            _vertices.Remove(vKey);

            if (updateTopology) BuildTopology(true);
        }

        public void DeleteVertices(IEnumerable<int> vKeys, bool updateTopology = true)
        {
            HashSet<int> deleteV = new HashSet<int>();

            foreach (int vK in vKeys)
            {
                int[] eKeys = Topology.GetVertexIncidentElements(vK);
                foreach(int eK in eKeys)
                {
                    DeleteElement(eK, false);
                }
                deleteV.Add(vK);
            }

            foreach (int vK in deleteV)         
                _vertices.Remove(vK);

            if (updateTopology) BuildTopology(true);
        }

        public bool ContainsVertexKey(int key)
        {
            return _vertices.ContainsKey(key);
        }

        public ITopologicVertex GetVertexWithKey(int key)
        {
            return _vertices[key];
        }

        public void DeleteVertexTextureCoordinates(int key)
        {
            ITopologicVertex v = _vertices[key];
            v.TextureCoordinates = new double[] { 0, 0, 0 };
            SetVertex(key, v);
        }

        public void DeleteAllVertexTextureCoordinates()
        {
            foreach (int key in _vertices.Keys)
            {
                ITopologicVertex v = _vertices[key];
                v.TextureCoordinates = new double[] { 0, 0, 0 };
                SetVertex(key, v);
            }
        }

        public List<int> VerticesKeys
        {
            get => _vertices.Keys.ToList();
        }

        public List<ITopologicVertex> Vertices
        {
            get => _vertices.Values.ToList();
        }

        public int VerticesCount
        {
            get => _vertices.Count;
        }
    }
}
