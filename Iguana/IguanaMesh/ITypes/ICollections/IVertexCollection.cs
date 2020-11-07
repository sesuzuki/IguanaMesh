using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes.ICollections
{
    public class IVertexCollection
    {
        private Dictionary<int, ITopologicVertex> _vertices = new Dictionary<int, ITopologicVertex>();

        public IVertexCollection ShallowCopy()
        {
            return (IVertexCollection)this.MemberwiseClone();
        }

        public IVertexCollection DeepCopy()
        {
            IVertexCollection copy = ShallowCopy();
            copy._vertices = new Dictionary<int, ITopologicVertex>(_vertices);
            return copy;
        }

        /// <summary>
        /// Copy without topologic data.
        /// </summary>
        public IVertexCollection CleanCopy()
        {
           IVertexCollection copy = new IVertexCollection();
           _vertices.Values.All(v =>
           {
               copy.AddVertex(v.Key, v.CleanCopy());
               return true;
           });
           return copy;
        }

        public void CleanTopologicalData()
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
            cullNodes.All(vKey => DeleteVertex(vKey));
        }

        public void AddVertex(int key, ITopologicVertex vertex)
        {
            if (key <= 0) key = FindNextKey();
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

        public int FindNextKey()
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

        public void SetVertexPosition(int key, Point3d point)
        {
            ITopologicVertex v = _vertices[key];
            v.Position = new IVector3D(point.X, point.Y, point.Z);
            _vertices[key] = v;
        }

        public void SetVertexPosition(int key, IVector3D position)
        {
            ITopologicVertex v = _vertices[key];
            v.Position = position;
            _vertices[key] = v;
        }


        public void SetVertexPosition(int key, double x, double y, double z)
        {
            ITopologicVertex v = _vertices[key];
            v.Position = new IVector3D(x, y, z);
            _vertices[key] = v;
        }

        public void SetVertexTextureCoordinates(int key, double u, double v, double w)
        {
            ITopologicVertex vertex = _vertices[key];
            vertex.TextureCoordinates = new double[] { u, v, w };
            _vertices[key] = vertex;
        }

        public void Clean()
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

        internal bool DeleteVertex(int key)
        {
            return _vertices.Remove(key);
        }

        internal bool DeleteVertex(ITopologicVertex vertex)
        {
            return _vertices.Remove(vertex.Key);
        }

        public bool ContainsKey(int key)
        {
            return _vertices.ContainsKey(key);
        }

        public ITopologicVertex GetVertexWithKey(int key)
        {
            return _vertices[key];
        }

        public void DeleteTextureCoordinates(int key)
        {
            ITopologicVertex v = _vertices[key];
            v.TextureCoordinates = new double[] { 0, 0, 0 };
            SetVertex(key, v);
        }

        public void DeleteAllTextureCoordinates()
        {
            foreach(int key in _vertices.Keys)
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

        public List<ITopologicVertex> VerticesValues
        {
            get => _vertices.Values.ToList();
        }

        public int Count
        {
            get => _vertices.Count;
        }

        public override string ToString()
        {
            return "IVertexCollection{"+Count+"}";
        }
    }
}
