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
    public class IMesh : ICloneable, IGH_Goo
    {
        //Guides
        private string message = "AHF-Mesh not initialized";

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

        public void BuildTopology()
        {
            Boolean flag1 = BuildSiblingHalfFacets();
            Boolean flag2 = BuildVertexToHalfFacet();

            string type = this.GetMeshTypeDescription();
            message = "AHF-Mesh (Vertices: " + Vertices.Count + "; Elements: " + Elements.Count + " ; Type: " + type + ")";



            if (!flag1 || !flag2)
            {
                message += "\n\n||||| Invalid Data-Structure |||||\nSibling Half-Facets (sibhfs): ";
                if (flag1) message += " Built;\n";
                else if(!flag1) message += " Errors Found;\n";

                message += "Vertex to Half-Facet (v2hf): ";
                if (flag2) message += " Built;\n";
                else if (!flag2) message += " Errors Found;\n";
            }
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

        /// <summary>
        /// <para> First Step: Construction of Sibling Half-Facets (sibhfs). </para>
        /// <para> From element´s connectivity (input), it returns a cyclic mapping of sibbling half-V (output). </para>
        /// </summary>
        private Boolean BuildSiblingHalfFacets()
        {
            Boolean flag = true;
            try
            {
                //Part 1
                foreach (Int32 elementID in Elements.ElementsKeys)
                {

                    IElement e = Elements.GetElementWithKey(elementID);

                    //Half-facets from element
                    for (Int32 halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                    {
                        //Adjacent vertices
                        Int32[] us;
                        e.GetHalfFacet(halfFacetID, out us);

                        //Find vertex with larger ID
                        Int32 v = us[0];
                        us.ToList().ForEach(idx => { if (v < idx) v = idx; });

                        Int64 sibData = (Int64) elementID << 32 | (Int64)halfFacetID;

                        if (!_tempVertexToHalfFacets.ContainsKey(v)) _tempVertexToHalfFacets.Add(v, new List<Int64> {  });
                        _tempVertexToHalfFacets[v].Add(sibData);

                        if (!_tempVertexToAdjacentHalfFacets.ContainsKey(sibData)) _tempVertexToAdjacentHalfFacets.Add(sibData, new List<int>());
                        _tempVertexToAdjacentHalfFacets[sibData].AddRange(us);
                    }
                }

                //Part 2
                //local half-facet indexing start with 1 and not 0; 
                foreach (Int32 elementID in Elements.ElementsKeys)
                {
                    IElement e = Elements.GetElementWithKey(elementID);

                    //Half-Facets from element e (Faces to edges)
                    for (Int32 halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                    {
                        if (e.GetSiblingHalfFacet(halfFacetID) == 0)
                        {
                            //Adjacent vertices
                            int[] hf;
                            e.GetHalfFacet(halfFacetID, out hf);

                            //Find vertex with larger ID
                            int v = hf[0];
                            hf.ToList().ForEach(idx => { if (v < idx) v = idx; });

                            Int64 sibData = (Int64) elementID << 32 | (Int64) halfFacetID;

                            // Adjacent vertices
                            List<Int32> us = _tempVertexToAdjacentHalfFacets[sibData];

                            //Step : Find half-V in _tempVertexToHalEdges(v) subject to v2adj(v,.)=us;
                            // Half-V in _tempVertexToHalEdges associated with v;
                            List<Int64> hfs_V = _tempVertexToHalfFacets[v];

                            foreach (Int64 hfs_v_f in hfs_V)
                            {
                                List<Int32> hfs_us = _tempVertexToAdjacentHalfFacets[hfs_v_f];

                                if (!hfs_v_f.Equals(sibData) && hfs_us.All(el => us.Contains(el)))
                                {
                                    e.SetSiblingHalfFacet(halfFacetID, hfs_v_f);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { flag = false; }

            return flag;
        }

        /// <summary>
        /// <para> Second Step: Construction of mapping from vertex to an incident half-facet (v2hf). </para>
        /// <para> From element´s connectivity and cyclic mapping of sibblings half-V (input), it returns a collection of maps from vertices to incident half-facet (output). </para>
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

                    for (Int32 vertexID = 0; vertexID < e.VerticesCount; vertexID++)
                    {

                        Int32 vKey = e.Vertices[vertexID];      
                        v = Vertices.GetVertexWithKey(vKey);

                        if (v.V2HF == 0)
                        {
                            v.SetV2HF(elementID, vertexID);

                            Vertices.SetVertex(vKey, v);
                        }
                    }


                    //Give border V higher priorities
                    for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                    {
                        if (e.GetSiblingHalfFacet(halfFacetID) == 0)
                        {
                            //Vertices of the facet
                            int[] hf;
                            e.GetHalfFacet(halfFacetID, out hf);

                            foreach (int vKey in hf)
                            {
                                v = Vertices.GetVertexWithKey(vKey);

                                if(v.GetElementID()!=elementID || v.GetHalfFacetID()!=halfFacetID)
                                {
                                    v.SetV2HF(elementID, halfFacetID);
                                    Vertices.SetVertex(vKey, v);
                                }
                            }
                        }
                    }

                    /*for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                    {
                        //Vertices of the facet
                        int[] hf;
                        e.GetHalfFacet(halfFacetID, out hf);
                        Int64 sibData = (Int64)elementID << 32 | (Int64)halfFacetID;

                        foreach (int vKey in hf)
                        {
                            v = Vertices.GetVertexWithKey(vKey);

                            if (v.V2HF == 0 || e.GetSiblingHalfFacet(halfFacetID) == 0)
                            {
                                v.SetV2HF(sibData);
                                Vertices.SetVertex(vKey, v);
                            }
                        }

                    }*/
                }
            }
            catch (Exception) { flag = false; }

            return flag;
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
                if (topo[0] == 2) msg = "Surface Mesh (2D)";
                if (topo[0] == 3) msg = "Volume Mesh (3D)";
            }

            if (topo.Count == 2)
            {
                msg = "Multi-dimensional Mesh (2D+3D)";
            }

            return msg;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #region IGH_Goo methods
        public bool IsValid
        {
            get => this.Equals(null);
        }

        public string IsValidWhyNot
        {
            get
            {
                string msg;
                if (this.IsValid) msg = string.Empty;
                else msg = string.Format("Invalid type.", this.TypeName);
                return msg;
            }
        }

        public string TypeName
        {
            get => "AHF-IDataStructure";
        }

        public string TypeDescription
        {
            get => ToString();
        }

        public IGH_Goo Duplicate()
        {
            return (IGH_Goo)this.MemberwiseClone();
        }

        public IGH_GooProxy EmitProxy()
        {
            return null;
        }

        public bool CastFrom(object source)
        {
            return false;
        }

        public bool CastTo<T>(out T target)
        {
            if (typeof(T).IsAssignableFrom(typeof(IMesh)))
            {
                if (this == null)
                    target = default(T);
                else
                    target = (T)(object)this;
                return true;
            }

            target = default(T);
            return false;
        }

        public object ScriptVariable()
        {
            return this;
        }

        public bool Write(GH_IWriter writer)
        {
            return true;
        }

        public bool Read(GH_IReader reader)
        {
            return true;
        }
        #endregion
    }
}
