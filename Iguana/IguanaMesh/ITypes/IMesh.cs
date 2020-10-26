using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.ITypes.ICollections;
using Iguana.IguanaMesh.ITypes.IElements;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh : ICloneable
    {
        //Guides
        private string message = "IMesh not initialized";

        private int dim = -1;

        //Half-Facet Data Structure
        public IVertexCollection Vertices { get; set; }
        public IElementCollection Elements { get; set; }
        public ITopology Topology { get; }

        //Temporary data structures for construction
        private Dictionary<Int32, List<Int64>> _tempVertexToHalfFacets;
        private Dictionary<Int64, List<Int32>> _tempVertexToAdjacentHalfFacets;

        /// <summary>
        /// <para> General constructor of a AHF-Mesh Data Structure. </para>
        /// </summary>
        public IMesh() { Topology = new ITopology(this); initData(); }

        /// <summary>
        /// <para> Constructor of a AHF-Mesh Data Structure. </para>
        /// <para><paramref name="vertices"/> : A sorted collection of vertices. </para>
        /// <para><paramref name="elements"/> : A collection of AHF_Elements. </para>
        /// </summary>
        public IMesh(List<ITopologicVertex> vertices, List<IElement> elements)
        {
            Topology = new ITopology(this);
            initData();

            Vertices.AddRangeVertices(vertices);
            Elements.AddRangeElements(elements);

            BuildTopology();
        }

        public IMesh(IVertexCollection _vertices, IElementCollection elements)
        {
            Topology = new ITopology(this);
            initData();

            Vertices = _vertices;
            Elements = elements;

            BuildTopology();
        }

        public IMesh(List<Point3d> vertices, List<IElement> elements)
        {
            Topology = new ITopology(this);
            initData();

            Vertices.AddRangeVertices(vertices);
            Elements.AddRangeElements(elements);

            BuildTopology();
        }

        /// <summary>
        /// <para> Constructor of a AHF-Mesh Data Structure. </para>
        /// <para><paramref name="mesh"/> : Rhino mesh. </para>
        /// </summary>
        public IMesh(Mesh mesh)
        {
            Topology = new ITopology(this);
            initData();

            // Initialize vertices
            for(int vK=0; vK<mesh.Vertices.Count; vK++)
            {
                Vertices.AddVertex(vK, new ITopologicVertex(mesh.Vertices[vK]));
            }

            //Initialize Elements
            for(int eK=0; eK<mesh.Faces.Count; eK++)
            {
                MeshFace f = mesh.Faces[eK];
                int[] vList = new int[]{ f.A, f.B, f.C };
                if (f.IsQuad) vList = new int[]{ f.A, f.B, f.C, f.D };

                this.Elements.AddElement(eK, new ISurfaceElement(vList));
            }

            this.BuildTopology();
        }

        private void initData()
        {
            Vertices = new IVertexCollection();
            Elements = new IElementCollection();

            _tempVertexToHalfFacets = new Dictionary<Int32, List<Int64>>();
            _tempVertexToAdjacentHalfFacets = new Dictionary<Int64, List<Int32>>();
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
            int keyVertex = this.Vertices.FindNextKey();
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                ITopologicVertex pt = mesh.Vertices.GetVertexWithKey(i);

                if (weld)
                {
                    flag = false;

                    Parallel.ForEach<int>(this.Vertices.VerticesKeys, oldKey =>
                    {
                        ITopologicVertex oldVertex = this.Vertices.GetVertexWithKey(oldKey);

                        if (oldVertex.DistanceTo(pt) < tolerance)
                        {
                            maps.Add(i, oldKey);
                            flag = true;
                        }
                    });

                    if (flag == false)
                    {
                        maps.Add(i, keyVertex);
                        this.Vertices.AddVertex(keyVertex, pt);
                        keyVertex++;
                    }
                }

                else
                {
                    maps.Add(i, keyVertex);
                    this.Vertices.AddVertex(keyVertex, pt);
                    keyVertex++;
                }
            }

            //Add Elements
            int keyElement = Elements.FindNextKey();
            foreach (IElement e in mesh.Elements.ElementsValues)
            {
                int[] vList = new int[e.VerticesCount];
                for(int i=0; i<e.VerticesCount; i++)
                {
                    int oldKey = e.Vertices[i];
                    vList[i] = maps[oldKey];
                }
                e.Vertices = vList;

                this.Elements.AddElement(keyElement, e);
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
            int keyVertex = this.Vertices.FindNextKey();
            for (int mapKey=0; mapKey<mesh.Vertices.Count; mapKey++)
            {
                Point3d pt = mesh.Vertices[mapKey];

                if (weld)
                {
                    flag = false;

                    Parallel.ForEach<int>(this.Vertices.VerticesKeys, oldKey =>
                    {
                        ITopologicVertex oldVertex = this.Vertices.GetVertexWithKey(oldKey);
                        if (oldVertex.DistanceTo(pt) < tolerance)
                        {
                            maps.Add(mapKey, oldKey);
                            flag = true;
                        }
                    });

                    if (flag == false)
                    {
                        maps.Add(mapKey, keyVertex);
                        this.Vertices.AddVertex(keyVertex, new ITopologicVertex(pt));
                        keyVertex++;
                    }
                }
                else
                {
                    maps.Add(mapKey, keyVertex);
                    this.Vertices.AddVertex(keyVertex, new ITopologicVertex(pt));
                    keyVertex++;
                }
            }

            //Add Elements
            foreach (MeshFace f in mesh.Faces)
            {
                int[] vList = new int[] { maps[f.A], maps[f.B], maps[f.C] };
                if (f.IsQuad) vList = new int[]{ maps[f.A], maps[f.B], maps[f.C], maps[f.D] };

                ISurfaceElement e = new ISurfaceElement(vList);
                this.Elements.AddElement(e);
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
            foreach(IElement e in Elements.ElementsValues)
            {
                int dim = e.TopologicDimension;
                if (!topo.Contains(dim)) topo.Add(dim);
            }

            if (topo.Count == 1)
            {
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

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public bool IsSurfaceMesh
        {
            get
            {
                if (dim == 2) return true;
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
