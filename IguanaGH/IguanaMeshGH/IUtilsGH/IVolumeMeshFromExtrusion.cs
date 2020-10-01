using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.ICollections;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Grasshopper.Kernel.Data;
using Iguana.IguanaMesh.IWrappers.IExtensions;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IVolumeMeshFromExtrusion : GH_Component
    {
        IMesh mesh;
        /// <summary>
        /// Initializes a new instance of the IVolumeMeshFromExtrusion class.
        /// </summary>
        public IVolumeMeshFromExtrusion()
          : base("iVolumeMeshFromExtrusion", "iVolumeMeshExtrusion",
              "General constructor for an Array-based Half-Facet (AHF) Mesh Data Structure.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Closed curve to patch", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "P", "Points to patch", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Count", "N", "Number of control points to rebuild curve", GH_ParamAccess.item);
            pManager.AddGenericParameter("IConstraints", "IConstraints", "Constraints for mesh generation.", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Meshing Settings", "ISettings", "Meshing settings", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
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
            Curve crv = null;
            List<Point3d> pts_patch = new List<Point3d>();
            int crvRes = 0;
            IguanaGmshSolver2D solverOptions = new IguanaGmshSolver2D();
            //GH_Structure<IguanaGmshConstraint> constraints = new GH_Structure<IguanaGmshConstraint>();

            DA.GetData(0, ref crv);
            DA.GetDataList(1, pts_patch);
            DA.GetData(2, ref crvRes);

            //DA.GetDataTree(3, out constraints);

            DA.GetData(4, ref solverOptions);

            mesh = null;
            //IGH_StructureEnumerator allConstraints = constraints.AllData(true);

            if (crv.IsClosed)
            {
                IVertexCollection vertices = new IVertexCollection();
                IElementCollection elements = new IElementCollection();

                NurbsCurve nCrv = crv.ToNurbsCurve();
                if (crvRes > 0 && nCrv.Points.Count < crvRes) nCrv = nCrv.Rebuild(crvRes, nCrv.Degree, true);

                IguanaGmsh.Initialize();

                int surfacetag = IguanaGmshFactory.OCCSurfacePatch(nCrv, pts_patch);

                int[] ov;
                IguanaGmsh.Model.GeoOCC.Extrude(new[] { 2, surfacetag }, 0, 0, 1, out ov, new int[] { 8, 2 }, new double[] { 0.5, 1 }, true);

                IguanaGmsh.Model.GeoOCC.Synchronize();

                solverOptions.ApplyBasicPreProcessing2D();

                // 2d mesh generation
                IguanaGmsh.Model.Mesh.Generate(3);

                // Postprocessing settings
                solverOptions.ApplyBasicPostProcessing2D();

                // Iguana mesh construction
                IguanaGmsh.Model.Mesh.TryGetIVertexCollection(ref vertices, 3);
                IguanaGmsh.Model.Mesh.TryGetIElementCollection(ref elements, 3);
                mesh = new IMesh(vertices, elements);
                mesh.BuildTopology();

                IguanaGmsh.FinalizeGmsh();
            }

            DA.SetData(0, mesh);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if(mesh!=null) IRhinoGeometry.DrawIMeshAsWires(args, mesh);
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
            get { return new Guid("56d8270a-6fbe-49a2-bfd8-f7ac1b1f88b5"); }
        }
    }
}