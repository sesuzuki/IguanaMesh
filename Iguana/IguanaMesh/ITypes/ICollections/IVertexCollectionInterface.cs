using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Iguana.IguanaMesh.ITypes.ICollections
{
    interface IVertexCollectionInterface
    {
        Boolean AddVertex(int key, ITopologicVertex vertex);

        Boolean AddVertex(int key, double[] vertex);

        Boolean AddVertex(int key, double x, double y, double z);

        Boolean AddVertex(ITopologicVertex vertex);

        Boolean AddVertexWithTextureCoordinates(int key, double x, double y, double z, double u, double v, double w);

        Boolean AddVertexWithTextureCoordinates(int key, double[] vertex);

        Boolean SetOrAddVertex(ITopologicVertex vertex);

        Boolean SetOrAddVertex(int key, ITopologicVertex vertex);

        Boolean SetOrAddVertex(int key, double[] vertex);

        Boolean SetOrAddVertex(int key, double x, double y, double z);

        Boolean SetOrAddVertexWithTextureCoordinates(int key, double x, double y, double z, double u, double v, double w);

        Boolean SetOrAddVertexWithTextureCoordinates(int key, double[] vertex);

        Boolean AddRangeVertices(List<ITopologicVertex> vertices);

        Boolean AddRangeVertices(List<int> keys, List<ITopologicVertex> vertices);

        Boolean AddRangeVertices(List<Point3d> vertices);

        Boolean AddRangeVertices(double[][] vertices);

        Boolean AddRangeVerticesWithTextureCoordinates(double[][] vertices);

        Boolean AddRangeVerticesWithTextureCoordinates(List<Point3d> vertices, List<Point2f> uvw);

        Boolean SetRangeVertices(List<int> keys, List<ITopologicVertex> vertices);

        Boolean SetRangeVertices(List<int> keys, List<Point3d> vertices);

        Boolean SetRangeVertices(List<int> keys, double[][] vertices);

        Boolean SetRangeTextureCoordinates(List<int> keys, double[][] uvw);

        Boolean SetOrAddRangeVertices(List<ITopologicVertex> vertices);

        Boolean SetOrAddRangeVertices(List<int> keys, List<ITopologicVertex> vertices);

        Boolean SetOrAddRangeVertices(List<int> keys, List<Point3d> vertices);

        Boolean SetOrAddRangeVertices(List<int> keys, double[][] vertices);

        Boolean SetOrAddRangeVerticesWithTextureCoordinates(List<int> keys, double[][] vertices);

        Boolean SetOrAddRangeVerticesWithTextureCoordinates(List<int> keys, List<Point3d> vertices, List<Point2f> uvw);

        Boolean SetVertex(int key, ITopologicVertex vertex);

        Boolean SetVertex(int key, double[] vertex);

        Boolean SetVertex(int key, double x, double y, double z);

        Boolean SetVertexWithTextureCoordinates(int key, double x, double y, double z, double u, double v, double w);

        Boolean SetVertexWithTextureCoordinates(int key, double[] vertex);

        Boolean SetVertex(ITopologicVertex vertex);

        Boolean SetTextureCoordinates(int key, double u, double v, double w);

        Boolean SetTextureCoordinates(int key, double[] vertex);

        Boolean DeleteVertex(int key);

        Boolean DeleteVertex(ITopologicVertex vertex);

        void DeleteVertices(IEnumerable<ITopologicVertex> vertex);

        void DeleteVertices(IEnumerable<int> keys);

        Boolean DeleteTextureCoordinates(int key);

        void Clean();

        void DeleteAllTextureCoordinates();

        Boolean ContainsKey(int key);

        ITopologicVertex GetVertexWithKey(int key);
    }
}
