using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IDeconstructMeshGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AHF_DeconstructDataStructure class.
        /// </summary>
        public IDeconstructMeshGH()
          : base("iDeconstructMesh", "iDeconstructMesh",
              "Deconstruct Iguana mesh.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "The Iguana mesh to be deconstructed.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iVertices", "Vertices", "Topologic vertices as points", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("VertexKeys", "v-Key", "Vertex keys", GH_ParamAccess.tree);
            pManager.AddGenericParameter("iElements", "Elements", "Elements", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("iKeysE", "e-Key", "Element Keys", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Sibling Half-Facets", "sibhf", "Cyclic mappings of sibling half-facets.", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Vertex to Half-Facet", "v2hf", "Mappings from vertices to incident half-facets.", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            DA.GetData(0, ref mesh);

            GH_Structure<GH_String> sibHalfFacets = new GH_Structure<GH_String>();
            GH_Structure<GH_String> elements = new GH_Structure<GH_String>();
            GH_Structure<GH_Number> eKeys = new GH_Structure<GH_Number>();
            IElement e;
            Int64 sibData;
            GH_Path path;
            GH_Path oPath = new GH_Path(0);
            foreach (int eK in mesh.ElementsKeys)
            {
                path = new GH_Path(eK);
                e = mesh.GetElementWithKey(eK);

                elements.Append(new GH_String(e.ToString()), oPath);
                eKeys.Append(new GH_Number(eK), oPath);

                for (int i = 1; i <= e.HalfFacetsCount; i++)
                {
                    GH_String msg = new GH_String("");
                    sibData = e.GetSiblingHalfFacet(i);

                    if (sibData != 0)
                    {
                        if (e.TopologicDimension == 2) msg = new GH_String("Face Element ID: " + e.GetSiblingElementID(i) + " :: Half-Edge ID: " + e.GetSiblingHalfFacetID(i));
                        if (e.TopologicDimension == 3) msg = new GH_String("Solid Element ID: " + e.GetSiblingElementID(i) + " :: Half-Face ID: " + e.GetSiblingHalfFacetID(i));
                    }
                    else
                    {
                        if (e.TopologicDimension == 2) msg = new GH_String("Naked Half-Edge");
                        if (e.TopologicDimension == 3) msg = new GH_String("Naked Half-Face");
                    }

                    sibHalfFacets.Append(msg, path);
                }
            }

            //Vertex to half-edge
            GH_Structure<GH_String> vertexToHalfFacet = new GH_Structure<GH_String>();
            GH_Structure<GH_Point> vertices = new GH_Structure<GH_Point>();
            GH_Structure<GH_Number> vKeys = new GH_Structure<GH_Number>();

            foreach (ITopologicVertex v in mesh.Vertices)
            {
                GH_String msg = new GH_String("Empty");
                if (v.V2HF > 0) msg.Value = v.SiblingHalfFacetDataToString();

                vertexToHalfFacet.Append(msg, oPath);
                vertices.Append(new GH_Point(v.RhinoPoint), oPath);
                vKeys.Append(new GH_Number(v.Key),oPath);
            }


            DA.SetDataTree(0, vertices);
            DA.SetDataTree(1, vKeys);
            DA.SetDataTree(2, elements);
            DA.SetDataTree(3, eKeys);
            DA.SetDataTree(4, sibHalfFacets);
            DA.SetDataTree(5, vertexToHalfFacet);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.AHF_DeconstructMesh;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("66dc6adc-d33b-41a7-a8ec-73a7779c7ed3"); }
        }
    }
}