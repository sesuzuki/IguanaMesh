using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.ICollections;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Iguana.IguanaMesh.IUtils;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IMeshFromPolylineGH : GH_Component
    {
        IMesh mesh;

        /// <summary>
        /// Initializes a new instance of the IMeshFromPolyline class.
        /// </summary>
        public IMeshFromPolylineGH()
          : base("iMesh Planar", "iMeshBoundary",
              "Create a planar mesh from a closed boundary polyline and a collection of internal boundraies polylines.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Outer boundary", "Outer", "External boundary", GH_ParamAccess.item);
            pManager.AddCurveParameter("Inner boundaries", "Inner", "Holes as polylines", GH_ParamAccess.list);
            pManager.AddGenericParameter("Meshing Settings", "Settings", "Meshing settings", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
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
            Curve _outer = null;
            List<Curve> _inner = new List<Curve>();
            IguanaGmshSolver2D solverOpt = new IguanaGmshSolver2D();

            //Retrieve vertices and elements
            DA.GetData(0, ref _outer);
            DA.GetDataList(1, _inner);
            DA.GetData(2, ref solverOpt);

            mesh = null;
            IVertexCollection vertices = new IVertexCollection();
            IElementCollection elements = new IElementCollection();

            IguanaGmsh.Initialize();

            int[] crv_tags = new int[_inner.Count + 1];
            crv_tags[0] = IguanaGmshConstructors.GmshCurveLoop(_outer, solverOpt);

            for (int i = 0; i < _inner.Count; i++)
            {
                crv_tags[i + 1] = IguanaGmshConstructors.GmshCurveLoop(_inner[i], solverOpt);
            }

            IguanaGmsh.Model.Geo.AddPlaneSurface(crv_tags);

            IguanaGmsh.Model.Geo.Synchronize();

            //solver options
            solverOpt.ApplyBasicPreProcessing2D();

            IguanaGmsh.Model.Mesh.Generate(2);

            solverOpt.ApplyBasicPostProcessing2D();

            // Iguana mesh construction
            IguanaGmsh.Model.Mesh.TryGetIVertexCollection(ref vertices, 2);
            IguanaGmsh.Model.Mesh.TryGetIElementCollection(ref elements, 2);

            mesh = new IMesh(vertices, elements);
            mesh.BuildTopology();

            IguanaGmsh.FinalizeGmsh();

            DA.SetData(0, mesh);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (mesh != null) IRhinoGeometry.DrawIMeshAsWires(args, mesh);
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
            get { return new Guid("40271dbd-73f2-4d37-bfd0-0d5ba5066999"); }
        }
    }
}