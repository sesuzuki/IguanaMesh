using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iguana.IguanaMesh.ITypes.ICollections
{
    public class IVertexCollection : IVertexCollectionInterface
    {
        private Dictionary<int, ITopologicVertex> _vertices = new Dictionary<int, ITopologicVertex>();

        public bool AddVertex(int key, ITopologicVertex vertex)
        {
            if (key < 0) key = FindNextKey();
            if (!_vertices.ContainsKey(key) && key>=0)
            {
                vertex.Key = key;
                _vertices.Add(key, vertex);

                return true;
            }
            else return false;
        }

        public bool AddVertex(ITopologicVertex vertex)
        {
            return AddVertex(vertex.Key, vertex);
        }

        public bool AddVertex(int key, double x, double y, double z)
        {
            return AddVertex(key, new ITopologicVertex(x, y, z));
        }

        public bool AddVertex(int key, double[] _vertex)
        {
            if (_vertex.Length == 3)
            {
                return AddVertex(key, new ITopologicVertex(_vertex[0], _vertex[1], _vertex[2]));
            }
            else return false;
        }

        public bool AddVertexWithTextureCoordinates(int key, double x, double y, double z, double u, double v, double w)
        {
            return AddVertex(key, new ITopologicVertex(x, y, z, u, v, w));
        }

        public bool AddVertexWithTextureCoordinates(int key, double[] _vertex)
        {
            if (_vertex.Length == 6)
            {
                return AddVertex(key, new ITopologicVertex(_vertex[0], _vertex[1], _vertex[2], _vertex[3], _vertex[4], _vertex[5]));
            }
            else return false;
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
            if (_vertices.ContainsKey(key) && key>=0)
            {
                vertex.Key = key;
                _vertices[key] = vertex;
                return true;
            }
            else return false;
        }

        public bool SetVertex(int key, double[] value)
        {
            if (value.Length == 3)
            {
                return SetVertex(key, new ITopologicVertex(value[0], value[1], value[2]));
            }
            else return false;
        }

        public bool SetVertex(int key, double x, double y, double z)
        {
            return SetVertex(key, new ITopologicVertex(x, y, z));
        }

        public bool SetVertexWithTextureCoordinates(int key, double x, double y, double z, double u, double v, double w)
        {
            return SetVertex(key, new ITopologicVertex(x, y, z, u, v, w));
        }

        public bool SetVertexWithTextureCoordinates(int key, double[] value)
        {
            if (value.Length == 6)
            {
                return SetVertex(key, new ITopologicVertex(value[0], value[1], value[2], value[3], value[4], value[5]));
            }
            else return false;
        }

        public bool SetVertex(ITopologicVertex vertex)
        {
            return SetVertex(vertex.Key, vertex);
        }

        public void Clean()
        {
            _vertices = new Dictionary<int, ITopologicVertex>();
        }

        public bool SetOrAddVertex(int key, ITopologicVertex vertex)
        {
            if (_vertices.ContainsKey(key)) return SetVertex(key, vertex);
            else return AddVertex(key, vertex);
        }

        public bool SetOrAddVertex(int key, double[] vertex)
        {
            if (vertex.Length == 3) return SetOrAddVertex(key, new ITopologicVertex(vertex[0], vertex[1], vertex[2]));
            else return false;
        }

        public bool SetOrAddVertex(int key, double x, double y, double z)
        {
            return SetOrAddVertex(key, new ITopologicVertex(x,y,z));
        }

        public bool SetOrAddVertexWithTextureCoordinates(int key, double x, double y, double z, double u, double v, double w)
        {
            return SetOrAddVertex(key, new ITopologicVertex(x, y, z, u, v, w));
        }

        public bool SetOrAddVertexWithTextureCoordinates(int key, double[] vertex)
        {
            if (vertex.Length == 6) return SetOrAddVertex(key, new ITopologicVertex(vertex[0], vertex[1], vertex[2], vertex[3], vertex[4], vertex[5]));
            else return false;
        }

        public bool SetOrAddVertex(ITopologicVertex vertex)
        {
            return SetOrAddVertex(vertex.Key, vertex);
        }

        public bool AddRangeVertices(List<ITopologicVertex> vertices)
        {
            if (vertices.Count > 0 && vertices != null)
            {
                int lastKey = FindNextKey();
                foreach (ITopologicVertex v in vertices)
                {
                    AddVertex(lastKey, v);
                    lastKey++;
                }
                return true;
            }
            else return false;
        }

        public bool AddRangeVerticesWithKeys(ITopologicVertex[] vertices, int[] keys)
        {
            if (vertices.Length > 0 && vertices.Length==keys.Length)
            {
                for(int i=0; i<vertices.Length; i++)
                {
                    AddVertex(keys[i], vertices[i]);
                }
                return true;
            }
            else return false;
        }

        public bool AddRangeVertices(List<int> keys, List<ITopologicVertex> vertices)
        {
            if (keys.Count == vertices.Count && keys.Count>0)
            {
                for(int i=0; i<keys.Count; i++)
                {
                    AddVertex(keys[i], vertices[i]);
                }
                return true;
            }
            else return false;
        }

        public bool AddRangeVertices(List<Point3d> points)
        {
            if (points.Count > 0 && points != null)
            {
                int lastKey = FindNextKey();
                foreach (Point3d pt in points)
                {
                    AddVertex(lastKey, new ITopologicVertex(pt));
                    lastKey++;
                }
                return true;
            }
            else return false;
        }

        public bool AddRangeVertices(double[][] values)
        {
            if (values.Length>0 && values != null)
            {
                int key = FindNextKey();
                foreach (double[] v in values)
                {
                    if (v.Length == 3) AddVertex(key, new ITopologicVertex(v[0], v[1], v[2]));
                    key++;
                }
                return true;
            }
            else return false;
        }

        public bool AddRangeVerticesWithTextureCoordinates(double[][] values)
        {
            if (values.Length > 0 && values != null)
            {
                int key = FindNextKey();
                foreach (double[] v in values)
                {
                    if (v.Length == 6) AddVertex(key, new ITopologicVertex(v[0], v[1], v[2], v[3], v[4], v[5]));
                    key++;
                }
                return true;
            }
            else return false;
        }

        public bool AddRangeVerticesWithTextureCoordinates(List<Point3d> points, List<Point2f> uvw)
        {
            if (points.Count > 0 && points.Count==uvw.Count)
            {
                int lastKey = FindNextKey();
                Point3d pt;
                Point2f coord;
                for(int i=0;i<points.Count; i++)
                {
                    pt = points[i];
                    coord = uvw[i];
                    AddVertex(lastKey, new ITopologicVertex(pt.X, pt.Y, pt.Z, coord.X, coord.Y, 0));
                    lastKey++;
                }
                return true;
            }
            else return false;
        }

        public bool SetRangeVertices(List<int> keys, List<ITopologicVertex> vertices)
        {
            if (keys.Count == vertices.Count && keys.Count>0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    SetVertex(keys[i], vertices[i]);
                }
                return true;
            }
            else return false;
        }

        public bool SetRangeVertices(List<int> keys, List<Point3d> vertices)
        {
            if (keys.Count == vertices.Count && keys.Count > 0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    SetVertex(keys[i], new ITopologicVertex(vertices[i]));
                }
                return true;
            }
            else return false;
        }

        public bool SetRangeVertices(List<int> keys, double[][] vertices)
        {
            if (keys.Count == vertices.Length && keys.Count > 0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    SetVertex(keys[i], vertices[i]);
                }
                return true;
            }
            else return false;
        }

        public bool SetOrAddRangeVertices(List<ITopologicVertex> vertices)
        {
            if (vertices!=null && vertices.Count > 0)
            {
                int lastKey = FindNextKey(), key;
                for (int i = 0; i < vertices.Count; i++)
                {
                    key = vertices[i].Key;
                    if (_vertices.ContainsKey(key)) SetVertex(key, vertices[i]);
                    else
                    {
                        AddVertex(lastKey, vertices[i]);
                        lastKey++;
                    }
                }
                return true;
            }
            else return false;
        }

        public bool SetOrAddRangeVertices(List<int> keys, List<ITopologicVertex> vertices)
        {
            if (keys.Count == vertices.Count && keys.Count > 0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    if(_vertices.ContainsKey(keys[i])) SetVertex(keys[i], vertices[i]);
                    else AddVertex(keys[i], vertices[i]);
                }
                return true;
            }
            else return false;
        }

        public bool SetOrAddRangeVertices(List<int> keys, List<Point3d> vertices)
        {
            if (keys.Count == vertices.Count && keys.Count > 0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    if (_vertices.ContainsKey(keys[i])) SetVertex(keys[i], new ITopologicVertex(vertices[i]));
                    else AddVertex(keys[i], new ITopologicVertex(vertices[i]));
                }
                return true;
            }
            else return false;
        }

        public bool SetOrAddRangeVertices(List<int> keys, double[][] vertices)
        {
            if (keys.Count == vertices.Length && keys.Count > 0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    if (_vertices.ContainsKey(keys[i])) SetVertex(keys[i], vertices[i]);
                    else AddVertex(keys[i], vertices[i]);
                }
                return true;
            }
            else return false;
        }

        public bool DeleteVertex(int key)
        {
            return _vertices.Remove(key);
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
            foreach (int k in keys)
            {
                DeleteVertex(k);
            }
        }

        public void DeleteVertices(IEnumerable<ITopologicVertex> vertices)
        {
            foreach (ITopologicVertex v in vertices)
            {
                DeleteVertex(v);
            }
        }

        public bool ContainsKey(int key)
        {
            return _vertices.ContainsKey(key);
        }

        public ITopologicVertex GetVertexWithKey(int key)
        {
            return _vertices[key];
        }

        public bool SetRangeTextureCoordinates(List<int> keys, double[][] uvw)
        {
            if (keys.Count == uvw.Length && keys.Count > 0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    SetTextureCoordinates(keys[i], uvw[i]);
                }
                return true;
            }
            else return false;
        }

        public bool SetOrAddRangeVerticesWithTextureCoordinates(List<int> keys, double[][] vertices)
        {
            if (keys.Count == vertices.Length && keys.Count > 0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    if (_vertices.ContainsKey(keys[i])) SetOrAddVertexWithTextureCoordinates(keys[i], vertices[i]);
                    else AddVertex(keys[i], vertices[i]);
                }
                return true;
            }
            else return false;
        }

        public bool SetOrAddRangeVerticesWithTextureCoordinates(List<int> keys, List<Point3d> vertices, List<Point2f> uvw)
        {
            if (keys.Count == vertices.Count && uvw.Count==keys.Count && keys.Count > 0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    Point3d pt = vertices[i];
                    Point2f coord = uvw[i];
                    if (_vertices.ContainsKey(keys[i])) SetVertexWithTextureCoordinates(keys[i], pt.X, pt.Y, pt.Z, coord.X, coord.Y, 0);
                    else AddVertexWithTextureCoordinates(keys[i], pt.X, pt.Y, pt.Z, coord.X, coord.Y, 0);
                }
                return true;
            }
            else return false;
        }

        public bool SetTextureCoordinates(int key, double u, double v, double w)
        {
            if (_vertices.ContainsKey(key))
            {
                ITopologicVertex vertex = _vertices[key];
                vertex.X = u;
                vertex.Y = v;
                vertex.Z = w;
                return SetVertex(key, vertex);
            }
            else return false;
        }

        public bool SetTextureCoordinates(int key, double[] uvw)
        {
            if (_vertices.ContainsKey(key))
            {
                ITopologicVertex v = _vertices[key];
                v.TextureCoordinates = uvw;
                return SetVertex(key, v);
            }
            else return false;
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
                DeleteTextureCoordinates(key);
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
