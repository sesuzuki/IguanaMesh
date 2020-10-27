using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iguana.IguanaMesh.ITypes.ICollections
{
    public class IVertexCollection : IVertexCollectionInterface
    {
        private Dictionary<int, ITopologicVertex> _vertices = new Dictionary<int, ITopologicVertex>();

        public void CullUnparsedNodes(IEnumerable<int> parsedNodes)
        {
            var cullNodes = VerticesKeys.Except(parsedNodes);
            if (cullNodes.Count() > 0) DeleteVertices(cullNodes);
        }

        public bool AddVertex(int key, ITopologicVertex vertex)
        {
            try
            {
                vertex.Key = key;
                _vertices.Add(key, vertex);
                return true;
            }
            catch (Exception) { return false; }
        }

        public bool AddVertex(int key, Point3d point)
        {
            return AddVertex(key, new ITopologicVertex(point.X, point.Y, point.Z));
        }

        public bool AddVertex(int key, double x, double y, double z)
        {
            return AddVertex(key, new ITopologicVertex(x, y, z));
        }

        public bool AddVertex(int key, double x, double y, double z, double u, double v, double w)
        {
            return AddVertex(key, new ITopologicVertex(x, y, z, u, v, w));
        }

        public int FindNextKey()
        {
            List<int> keys = VerticesKeys;
            keys.Sort();
            if (VerticesKeys.Count == 0) return 0;
            else return keys[keys.Count - 1] + 1;
        }

        public bool SetVertex(int key, ITopologicVertex vertex)
        {
            try {
                vertex.Key = key;
                _vertices[key] = vertex;
                return true;
            }
            catch (Exception) { return false; }
        }

        public bool SetVertexPosition(int key, Point3d point)
        {
            if (_vertices.ContainsKey(key))
            {
                ITopologicVertex v = _vertices[key];
                v.Position = new IVector3D( point.X, point.Y, point.Z );
                _vertices[key] = v;
                return true;
            }
            else return false;
        }


        public bool SetVertexPosition(int key, double x, double y, double z)
        {
            if (_vertices.ContainsKey(key))
            {
                ITopologicVertex v = _vertices[key];
                v.Position = new IVector3D(x, y, z);
                _vertices[key] = v;
                return true;
            }
            else return false;
        }

        public bool SetVertexTextureCoordinates(int key, double u, double v, double w)
        {
            if (_vertices.ContainsKey(key))
            {
                ITopologicVertex vertex = _vertices[key];
                vertex.TextureCoordinates = new double[] { u,v,w };
                _vertices[key] = vertex;
                return true;
            }
            else return false;
        }

        public void Clean()
        {
            _vertices = new Dictionary<int, ITopologicVertex>();
        }

        public bool AddRangeVertices(IEnumerable<ITopologicVertex> vertices)
        {
            try { 
                foreach(ITopologicVertex v in vertices)
                {
                    AddVertex(v.Key, v);
                }
                return true;
            }
            catch (Exception) { return false; }
        }

        public bool AddRangeVertices(IEnumerable<int> keys, IEnumerable<ITopologicVertex> vertices)
        {
            try
            {
                if (keys.Count() == vertices.Count())
                {
                    for (int i = 0; i < keys.Count(); i++)
                    {
                        AddVertex(keys.ElementAt(i), vertices.ElementAt(i));
                    }
                    return true;
                }
                else return false;
            }
            catch (Exception) { return false; }
        }

        public bool AddRangeVertices(IEnumerable<Point3d> points)
        {
            try
            {
                int lastKey = FindNextKey();
                foreach (Point3d pt in points)
                {
                    AddVertex(lastKey, new ITopologicVertex(pt));
                    lastKey++;
                }
                return true;
            }catch(Exception) { return false; }
        }

        public bool SetRangeVertices(IEnumerable<int> keys, IEnumerable<ITopologicVertex> vertices)
        {
            try
            {
                if (keys.Count() == vertices.Count())
                {
                    for (int i = 0; i < keys.Count(); i++)
                    {
                        SetVertex(keys.ElementAt(i), vertices.ElementAt(i));
                    }
                    return true;
                }
                else return false;
            }
            catch (Exception) { return false; }
        }

        public bool SetRangeVertices(IEnumerable<int> keys, IEnumerable<Point3d> points)
        {
            try
            {
                if (keys.Count() == points.Count())
                {
                    for (int i = 0; i < keys.Count(); i++)
                    {
                        SetVertexPosition(keys.ElementAt(i), points.ElementAt(i));
                    }
                    return true;
                }
                else return false;
            }
            catch (Exception) { return false; }
        }

        public bool DeleteVertex(int key)
        {
            try
            {
                return _vertices.Remove(key);
            }
            catch (Exception) { return false; }
        }

        public bool DeleteVertex(ITopologicVertex vertex)
        {
            try
            {
                return _vertices.Remove(vertex.Key);
            }
            catch (Exception) { return false; }
        }

        public void DeleteVertices(IEnumerable<int> keys)
        {
            try
            {
                foreach (int k in keys)
                {
                    DeleteVertex(k);
                }
            }
            catch (Exception) { }
        }

        public void DeleteVertices(IEnumerable<ITopologicVertex> vertices)
        {
            try
            {
                foreach (ITopologicVertex v in vertices)
                {
                    DeleteVertex(v);
                }
            }
            catch (Exception) { }
        }

        public bool ContainsKey(int key)
        {
            return _vertices.ContainsKey(key);
        }

        public ITopologicVertex GetVertexWithKey(int key)
        {
            return _vertices[key];
        }

        public bool DeleteTextureCoordinates(int key)
        {
            if (_vertices.ContainsKey(key))
            {
                ITopologicVertex v = _vertices[key];
                v.TextureCoordinates = new double[] { 0,0,0 };
                SetVertex(key, v);
                return true;
            }
            else return false;
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
    }
}
