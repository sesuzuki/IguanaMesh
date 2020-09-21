using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.ITypes;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IConstructMeshGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMesh class.
        /// </summary>
        public IConstructMeshGH()
          : base("iMesh Constructor", "iMesh",
              "General constructor for an Array-based Half-Facet (AHF) Mesh Data Structure.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Vertices", "V", "Vertices as a list of 3-D points", GH_ParamAccess.list);
            pManager.AddGenericParameter("IElements", "iE", "List of IElements", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Constructed Array-Based Half-Facet (AHF) Mesh Data Structure.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> _vertices = new List<Point3d>();
            List<IElement> _elements = new List<IElement>();

            //Retreive vertices and elements
            DA.GetDataList(0, _vertices);
            DA.GetDataList(1, _elements);

            //Build AHF-DataStructure
            //IMesh mesh = new IMesh(_vertices, _elements);
            IMesh mesh = new IMesh(_vertices, _elements);
            mesh.BuildTopology();

            DA.SetData(0, mesh);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.AHF_ConstructMesh;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2b1e3e0d-7b5c-49d9-835f-eda2a20050b9"); }
        }
    }
}