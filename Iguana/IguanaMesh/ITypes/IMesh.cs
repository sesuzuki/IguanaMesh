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
using System.Deployment.Internal;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh
    {
        //Guides
        private string message = "IMesh not initialized";
        private int dim = -1;
        private ITopology _topology;

        //Half-Facet Data Structure
        public IVertexCollection Vertices { get; set; }
        public IElementCollection Elements { get; set; }
        public ITopology Topology { get => _topology; }

        /// <summary>
        /// <para> General constructor of a AHF-Mesh Data Structure. </para>
        /// </summary>
        public IMesh() { _topology = new ITopology(this); initData(); }

        public IMesh(IVertexCollection vertices, IElementCollection elements)
        {
            initData();

            Vertices = vertices;
            Elements = elements;

            BuildTopology();
        }

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
                Vertices.AddVertex(vK, new ITopologicVertex(mesh.Vertices[vK-1]));
            }

            //Initialize Elements
            for(int eK=1; eK<=mesh.Faces.Count; eK++)
            {
                MeshFace f = mesh.Faces[eK-1];
                int[] vList = new int[]{ f.A+1, f.B+1, f.C+1 };
                if (f.IsQuad) vList = new int[]{ f.A+1, f.B+1, f.C+1, f.D+1 };

                this.Elements.AddElement(eK, new ISurfaceElement(vList));
            }

            this.BuildTopology();
        }

        private void initData()
        {
            Vertices = new IVertexCollection();
            Elements = new IElementCollection();
            _topology = new ITopology(this);
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
            foreach(int i in mesh.Vertices.VerticesKeys)
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

                    Parallel.ForEach<ITopologicVertex>(this.Vertices.VerticesValues, v =>
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
            int keyElement = this.Elements.FindNextKey();
            foreach (MeshFace f in mesh.Faces)
            {
                int[] vList = new int[] { maps[f.A], maps[f.B], maps[f.C] };
                if (f.IsQuad) vList = new int[]{ maps[f.A], maps[f.B], maps[f.C], maps[f.D] };

                ISurfaceElement e = new ISurfaceElement(vList);
                this.Elements.AddElement(keyElement, e);
                keyElement++;
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

        public IMesh ShallowCopy()
        {
            return (IMesh) this.MemberwiseClone();
        }

        public IMesh DeepCopy()
        {
            IMesh copy = new IMesh();
            copy.Vertices = Vertices.DeepCopy();
            copy.Elements = Elements.DeepCopy();
            return copy;
        }

        /// <summary>
        /// Copy without topological data.
        /// </summary>
        public IMesh CleanCopy()
        {
            IMesh copy = new IMesh();
            copy.Vertices = Vertices.CleanCopy();
            copy.Elements = Elements.CleanCopy();
            copy.dim = dim;
            copy.message = "IMesh not initialized";
            //copy._tempVertexToAdjacentHalfFacets = new Dictionary<long, List<int>>();
            //copy._tempVertexToHalfFacets = new Dictionary<int, List<long>>();
            copy._topology = new ITopology(this);
            return copy;
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

        /// <summary>
        /// Second Step: Construction of mapping from vertex to an incident half-facet (v2hf).
        /// From element´s connectivity and cyclic mapping of sibblings half-V (input), it returns a collection of maps from vertices to incident half-facet (output).
        /// </summary>
        private Boolean BuildVertexToHalfFacet()
        {
            Boolean flag = true;
            try
            {
                foreach (Int32 elementID in Elements.ElementsKeys)
                {
                    IElement e = Elements.GetElementWithKey(elementID);
                    ITopologicVertex v;

                    //Give border V higher priorities
                    for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                    {
                        //Vertices of the facet
                        int[] hf;
                        e.GetHalfFacet(halfFacetID, out hf);

                        foreach (int vKey in hf)
                        {
                            v = Vertices.GetVertexWithKey(vKey);

                            if (v.V2HF == 0)
                            {
                                v.SetV2HF(elementID, halfFacetID);
                                Vertices.SetVertex(vKey, v);
                            }

                            if (e.GetSiblingHalfFacet(halfFacetID) == 0)
                            {
                                if (v.GetElementID() != elementID || v.GetHalfFacetID() != halfFacetID)
                                {
                                    v.SetV2HF(elementID, halfFacetID);
                                    Vertices.SetVertex(vKey, v);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { flag = false; }

            return flag;
        }

        private void UpdateVertexToHalfFacet(int elementID, int halfFacetID)
        {
            //Vertices of the facet
            int[] hf;
            IElement e = Elements.GetElementWithKey(elementID);
            e.GetHalfFacet(halfFacetID, out hf);
            ITopologicVertex v;

            foreach (int vKey in hf)
            {
                v = Vertices.GetVertexWithKey(vKey);
                v.SetV2HF(elementID, halfFacetID);
                Vertices.SetVertex(vKey, v);

                if (e.GetSiblingHalfFacet(halfFacetID) == 0)
                {
                    if (v.GetElementID() != elementID || v.GetHalfFacetID() != halfFacetID)
                    {
                        v.SetV2HF(elementID, halfFacetID);
                        Vertices.SetVertex(vKey, v);
                    }
                }
            }
        }

        /// <summary>
        /// Build topologic relationships.
        /// </summary>
        internal void BuildTopology()
        {
            //BuildSiblingHalfFacets
            Boolean flag1 = Elements.BuildTopologicalData();
            Boolean flag2 = BuildVertexToHalfFacet();

            string type = this.GetMeshTypeDescription();
            message = "IMesh (Vertices: " + Vertices.Count + "; Elements: " + Elements.Count + " ; Type: " + type + ")";

            if (!flag1 || !flag2)
            {
                message += "\n\n||||| Invalid Data-Structure |||||\nSibling Half-Facets (sibhfs): ";
                if (flag1) message += " Built;\n";
                else if (!flag1) message += " Errors Found;\n";

                message += "Vertex to Half-Facet (v2hf): ";
                if (flag2) message += " Built;\n";
                else if (!flag2) message += " Errors Found;\n";
            }
        }
    }
}
