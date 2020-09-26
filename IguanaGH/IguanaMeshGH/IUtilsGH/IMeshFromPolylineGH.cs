using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Iguana.IguanaMesh.IGmshWrappers;
using Iguana.IguanaMesh.ITypes;
using IguanaGH.IguanaMeshGH.ITopologyGH;
using Iguana.IguanaMesh.ITypes.ICollections;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IMeshFromPolylineGH : GH_Component
    {
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
            IguanaGmshSolverOptions solverOpt = new IguanaGmshSolverOptions();

            //Retrieve vertices and elements
            DA.GetData(0, ref _outer);
            DA.GetDataList(1, _inner);
            DA.GetData(2, ref solverOpt);

            IMesh mesh = null;

            Gmsh.Initialize();

            int[] crv_tags = new int[_inner.Count + 1];
            crv_tags[0] = TryBuildGmshCurveLoop(_outer, solverOpt);

            for (int i = 0; i < _inner.Count; i++)
            {
                crv_tags[i + 1] = TryBuildGmshCurveLoop(_inner[i], solverOpt);
            }

            Gmsh.Model.Geo.AddPlaneSurface(crv_tags, 1);

            Gmsh.Model.Geo.Synchronize();

            //solver options
            solverOpt.ApplyBasicPreProcessing2D();

            Gmsh.Model.Mesh.Generate(2);

            solverOpt.ApplyBasicPostProcessing2D();

            // Iguana mesh construction
            IVertexCollection vertices = Gmsh.Model.Mesh.TryGetIVertexCollection();
            IElementCollection elements = Gmsh.Model.Mesh.TryGetIElementCollection();
            mesh = new IMesh(vertices, elements);
            mesh.BuildTopology();

            Gmsh.FinalizeGmsh();

            DA.SetData(0, mesh);
        }

        public int TryBuildGmshCurveLoop(Curve crv, IguanaGmshSolverOptions solverOpt)
        {
            Polyline poly;
            crv.TryGetPolyline(out poly);

            if (!poly.IsClosed) poly.Add(poly[0]);

            int[] pt_tags = new int[poly.Count - 1];
            double size = 0.1;
            for (int i = 0; i < poly.Count - 1; i++)
            {
                Point3d pt = poly[i];
                size = solverOpt.TargetMeshSize[0];
                if (solverOpt.TargetMeshSize.Count == poly.Count - 1) size = solverOpt.TargetMeshSize[i];
                pt_tags[i] = Gmsh.Model.Geo.AddPoint(pt.X, pt.Y, pt.Z, size);
            }

            int[] ln_tags = new int[pt_tags.Length];
            for (int i = 0; i < pt_tags.Length; i++)
            {
                int start = pt_tags[i];
                int end = pt_tags[0];
                if (i < pt_tags.Length - 1) end = pt_tags[i + 1];

                ln_tags[i] = Gmsh.Model.Geo.AddLine(start, end);
            }

            return Gmsh.Model.Geo.AddCurveLoop(ln_tags);
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