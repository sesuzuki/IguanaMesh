using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITransformGH
{
    public class IExpandMeshGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IAddVertexGH class.
        /// </summary>
        public IExpandMeshGH()
          : base("iExpandMesh", "iExpandMesh",
              "Expand a mesh data structure.",
              "Iguana", "Transform")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Vertices", "iV", "Vertices.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Elements", "iE", "Elements.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            List<ITopologicVertex> vertices = new List<ITopologicVertex>();
            List<IElement> elements = new List<IElement>();

            DA.GetData(0, ref mesh);
            DA.GetDataList(1, vertices);
            DA.GetDataList(2, elements);

            IMesh dM = mesh.CleanCopy();
            dM.AddRangeVertices(vertices);
            dM.AddRangeElements(elements);
            dM.BuildTopology(true);

            DA.SetData(0, dM);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("78f96c80-b1f2-4cee-8b4f-4985d920b19d"); }
        }
    }
}