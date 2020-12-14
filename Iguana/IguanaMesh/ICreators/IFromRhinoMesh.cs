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

using System;
using System.Collections.Generic;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ICreators
{
    public class IFromRhinoMesh : ICreatorInterface
    {
        private Mesh rhinoMesh;
        private List<ITopologicVertex> vertices;
        private List<IElement> faces;
        private int keyElement = 1;

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
                mesh.AddRangeVertices(vertices);
                mesh.AddRangeElements(faces);

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
                    v.Key = i+1;
                    v.TextureCoordinates = new double[] { uvw.X, uvw.Y, 0 };          

                    vertices.Add(v);
                }

                foreach (MeshFace f in rhinoMesh.Faces)
                {
                    ISurfaceElement iF = new ISurfaceElement(vertices[f.A].Key, vertices[f.B].Key, vertices[f.C].Key);
                    if (f.IsQuad) iF = new ISurfaceElement(vertices[f.A].Key, vertices[f.B].Key, vertices[f.C].Key, vertices[f.D].Key);

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
