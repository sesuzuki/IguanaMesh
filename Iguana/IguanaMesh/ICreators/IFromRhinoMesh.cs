using System;
using System.Collections.Generic;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ICreators
{
    class IFromRhinoMesh : ICreatorInterface
    {
        private Mesh rhinoMesh;
        private List<Point3d> vertices;
        private List<IElement> faces;
        private List<Point2f> uvw;
        private int keyElement;

        public IFromRhinoMesh(Mesh _rhinoMesh)
        {
            rhinoMesh = _rhinoMesh;
            vertices = new List<Point3d>();
            faces = new List<IElement>();
            uvw = new List<Point2f>();

        }

        public IMesh BuildMesh()
        {
            IMesh mesh = new IMesh();
            Boolean flag = BuildDataBase();
            if (flag)
            {
                mesh.Vertices.AddRangeVerticesWithTextureCoordinates(vertices, uvw);
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

                for (int i = 0; i < rhinoMesh.Vertices.Count; i++)
                {
                    vertices.Add(rhinoMesh.Vertices[i]);

                    if (flag) uvw.Add(rhinoMesh.TextureCoordinates[i]);
                    else uvw.Add(new Point2f(0, 0));
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
