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
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh
    {
        //Guides
        private string message = "IMesh not initialized";
        private int dim = -1;
        private int elementKey = 1;
        private ITopology _topology;
        private bool _valid = false;

        private Mesh _renderMesh;
        private HashSet<int> _elementTypes;
        public Mesh RenderMesh => _renderMesh;
        public HashSet<int> ElementTypes => _elementTypes;
        private Dictionary<int, IElement> _elements;
        private Dictionary<int, ITopologicVertex> _vertices;

        //Temporary data structures for construction
        private Dictionary<Int32, HashSet<Int64>> _tempVertexToHalfFacets;

        //Half-Facet Data Structure
        public ITopology Topology { get => _topology; }

        /// <summary>
        /// <para> General constructor of a AHF-Mesh Data Structure. </para>
        /// </summary>
        public IMesh() { _topology = new ITopology(this); initData(); }

        /// <summary>
        /// <para> Constructor of a AHF-Mesh Data Structure. </para>
        /// <para><paramref name="mesh"/> : Rhino mesh. </para>
        /// </summary>
        public IMesh(Mesh mesh)
        {
            initData();

            // Initialize vertices
            for(int vK=1; vK<=mesh.Vertices.Count; vK++)
            {
                AddVertex(vK, new ITopologicVertex(mesh.Vertices[vK-1]));
            }

            //Initialize Elements
            for(int eK=1; eK<=mesh.Faces.Count; eK++)
            {
                MeshFace f = mesh.Faces[eK-1];
                int[] vList = new int[]{ f.A+1, f.B+1, f.C+1 };
                if (f.IsQuad) vList = new int[]{ f.A+1, f.B+1, f.C+1, f.D+1 };

                this.AddElement(new ISurfaceElement(vList));
            }

            this.BuildTopology();
        }

        private void initData()
        {
            _vertices = new Dictionary<int, ITopologicVertex>();
            _elements = new Dictionary<int, IElement>();
            _tempVertexToHalfFacets = new Dictionary<Int32, HashSet<Int64>>();
            _topology = new ITopology(this);
            _elementTypes = new HashSet<int>();
        }

        /// <summary>
        /// Add a AHF_Mesh. 
        /// <para><paramref name="mesh"/> : The AHF_mesh to be added. </para>
        /// <para><paramref name="weld"/> : Weld creases in a mesh. </para>
        /// <para><paramref name="tolerance"/> : Vertices smaller than this tolerance will be merged. </para>
        /// </summary>
        public void AddMesh(IMesh mesh, Boolean weld, double tolerance)
        {
            Dictionary<int, int> maps = new Dictionary<int, int>();
            Boolean flag;

            //Add Vertices
            int keyVertex = FindNextVertexKey();
            foreach(int i in mesh.VerticesKeys)
            {
                ITopologicVertex pt = mesh.GetVertexWithKey(i);

                if (weld)
                {
                    flag = false;

                    Parallel.ForEach<int>(this.VerticesKeys, oldKey =>
                    {
                        ITopologicVertex oldVertex = this.GetVertexWithKey(oldKey);

                        if (oldVertex.DistanceTo(pt) < tolerance)
                        {
                            maps.Add(i, oldKey);
                            flag = true;
                        }
                    });

                    if (flag == false)
                    {
                        maps.Add(i, keyVertex);
                        this.AddVertex(keyVertex, pt);
                        keyVertex++;
                    }
                }

                else
                {
                    maps.Add(i, keyVertex);
                    this.AddVertex(keyVertex, pt);
                    keyVertex++;
                }
            }

            //Add Elements
            int keyElement = FindNextElementKey();
            foreach (IElement e in mesh.Elements)
            {
                int[] vList = new int[e.VerticesCount];
                for(int i=0; i<e.VerticesCount; i++)
                {
                    int oldKey = e.Vertices[i];
                    vList[i] = maps[oldKey];
                }
                e.Vertices = vList;

                this.AddElement(e);
            }

            this.BuildTopology();
        }

        /// <summary>
        /// Add a AHF_Mesh. 
        /// <para><paramref name="mesh"/> : The AHF_mesh to be added. </para>
        /// <para><paramref name="weld"/> : Weld creases in a mesh. </para>
        /// <para><paramref name="tolerance"/> : Vertices smaller than this tolerance will be merged. </para>
        /// </summary>
        public void AddRhinoMesh(Mesh mesh, Boolean weld, double tolerance)
        {

            Dictionary<int, int> maps = new Dictionary<int, int>();
            Boolean flag;

            //Add Vertices
            int keyVertex = FindNextVertexKey();
            for (int mapKey=0; mapKey<mesh.Vertices.Count; mapKey++)
            {
                Point3d pt = mesh.Vertices[mapKey];

                if (weld)
                {
                    flag = false;

                    Parallel.ForEach<ITopologicVertex>(Vertices, v =>
                    {
                        if (v.DistanceTo(pt) < tolerance)
                        {
                            maps.Add(mapKey, v.Key);
                            flag = true;
                        }
                    });

                    if (!flag)
                    {
                        maps.Add(mapKey, keyVertex);
                        AddVertex(keyVertex, new ITopologicVertex(pt));
                        keyVertex++;
                    }
                }
                else
                {
                    maps.Add(mapKey, keyVertex);
                    AddVertex(keyVertex, new ITopologicVertex(pt));
                    keyVertex++;
                }
            }

            //Add Elements
            foreach (MeshFace f in mesh.Faces)
            {
                int[] vList = new int[] { maps[f.A], maps[f.B], maps[f.C] };
                if (f.IsQuad) vList = new int[]{ maps[f.A], maps[f.B], maps[f.C], maps[f.D] };

                ISurfaceElement e = new ISurfaceElement(vList);
                this.AddElement(e);
            }

            this.BuildTopology();
        }

        public override string ToString()
        {
            return message;
        }

        public string GetMeshTypeDescription()
        {
            List<int> topo = new List<int>();
            string msg = "Undefined Dimensionality";
            foreach(IElement e in Elements)
            {
                int dim = e.TopologicDimension;
                if (!topo.Contains(dim)) topo.Add(dim);
            }

            if (topo.Count == 1)
            {
                if (topo[0] == 1)
                {
                    msg = "Bar Mesh (1D)";
                    dim = 1;
                }

                if (topo[0] == 2)
                {
                    msg = "Surface Mesh (2D)";
                    dim = 2;
                }
                if (topo[0] == 3)
                {
                    msg = "Volume Mesh (3D)";
                    dim = 3;
                }
            }

            if (topo.Count == 2)
            {
                msg = "Multi-dimensional Mesh (2D+3D)";
                dim = 4;
            }

            return msg;
        }

        public bool IsSurfaceMesh
        {
            get
            {
                if (dim == 2) return true;
                else return false;
            }
        }

        public bool IsBarMesh
        {
            get
            {
                if (dim == 1) return true;
                else return false;
            }
        }

        public bool IsVolumeMesh
        {
            get
            {
                if (dim == 3) return true;
                else return false;
            }
        }

        public bool IsValidMesh
        {
            get
            {
                if (dim != -1) return true;
                else return false;
            }
        }

        public bool IsMultidimensionalMesh
        {
            get
            {
                if (dim == 4) return true;
                else return false;
            }
        }
    }
}
