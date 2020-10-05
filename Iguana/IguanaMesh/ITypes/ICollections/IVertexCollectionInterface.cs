using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Iguana.IguanaMesh.ITypes.ICollections
{
    interface IVertexCollectionInterface
    {
        bool AddVertex(int key, ITopologicVertex vertex);

        bool AddVertex(int key, Point3d point);

        bool AddVertex(int key, double x, double y, double z);

        bool AddVertex(int key, double x, double y, double z, double u, double v, double w);

        bool AddRangeVertices(IEnumerable<ITopologicVertex> vertices);

        bool AddRangeVertices(IEnumerable<int> keys, IEnumerable<ITopologicVertex> vertices);

        bool AddRangeVertices(IEnumerable<Point3d> points);

        bool SetRangeVertices(IEnumerable<int> keys, IEnumerable<ITopologicVertex> vertices);

        bool SetRangeVertices(IEnumerable<int> keys, IEnumerable<Point3d> points);

        bool SetVertex(int key, ITopologicVertex vertex);

        bool SetVertexPosition(int key, Point3d point);

        bool SetVertexPosition(int key, double x, double y, double z);

        bool SetVertexTextureCoordinates(int key, double u, double v, double w);

        bool DeleteVertex(int key);

        bool DeleteVertex(ITopologicVertex vertex);

        void DeleteVertices(IEnumerable<ITopologicVertex> vertices);

        void DeleteVertices(IEnumerable<int> keys);

        bool DeleteTextureCoordinates(int key);

        void Clean();

        void DeleteAllTextureCoordinates();

        Boolean ContainsKey(int key);

        ITopologicVertex GetVertexWithKey(int key);

        int FindNextKey();

        void CullUnparsedNodes(IEnumerable<int> parsedNodes);
    }
}
