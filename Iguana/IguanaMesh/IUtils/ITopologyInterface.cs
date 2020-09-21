using System;
using System.Collections.Generic;

namespace Iguana.IguanaMesh.IUtils
{
    interface ITopologyInterface
    {
        List<int> GetVertexIncidentElements(int vertexKey);

        List<int> GetVertexAdjacentVertices(int vertexKey);

        Boolean ComputeVertexNormal(int vertexKey);

        Boolean ComputeAllVerticesNormals();

        Boolean CleanAllVerticesNormals();

        List<Tuple<int, int>> GetNakedEdges();

        List<int> GetNakedVertices();

        List<int[]> GetUniqueEdges();
    }
}
