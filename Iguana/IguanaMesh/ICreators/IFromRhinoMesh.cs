using System;
using System.Collections.Generic;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ICreators
{
    public class IFromRhinoMesh : ICreatorInterface
    {
        private Mesh rhinoMesh;
        private List<ITopologicVertex> vertices;
        private List<IElement> faces;
        private int keyElement;

        public IFromRhinoMesh(Mesh _rhinoMesh)
        {
            rhinoMesh = _rhinoMesh;
            vertices = new List<ITopologicVertex>();
            faces = new List<IElement>();

        }

        public IMesh BuildMesh()
        {
            IMesh mesh = new IMesh();
            Boolean flag = BuildDataBase();
            if (flag)
            {
                mesh.Vertices.AddRangeVertices(vertices);
                mesh.Elements.AddRangeElements(faces);

                mesh.BuildTopology();
            }
            return mesh;
        }

        public bool BuildDataBase()
        {
            if (vertices!=null)
            {
                Boolean flag = false;
                if (rhinoMesh.TextureCoordinates.Count == rhinoMesh.Vertices.Count) flag = true;

                Point2f uvw = new Point2f(0,0);
                for (int i = 0; i < rhinoMesh.Vertices.Count; i++)
                {
                    if (flag) uvw = rhinoMesh.TextureCoordinates[i];

                    ITopologicVertex v = new ITopologicVertex(rhinoMesh.Vertices[i]);
                    v.Key = i;
                    v.TextureCoordinates = new double[] { uvw.X, uvw.Y, 0 };          

                    vertices.Add(v);
                }

                foreach (MeshFace f in rhinoMesh.Faces)
                {
                    ISurfaceElement iF = new ISurfaceElement(f.A, f.B, f.C);
                    if (f.IsQuad) iF = new ISurfaceElement(f.A, f.B, f.C, f.D);

                    iF.Key = keyElement;
                    faces.Add(iF);
                    keyElement++;
                }
                return true;
            }

            else return false;
        }
    }
}
