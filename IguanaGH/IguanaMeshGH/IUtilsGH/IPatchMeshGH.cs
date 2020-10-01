using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.ICollections;
using Rhino.Geometry;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Grasshopper.Kernel.Data;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IPatchMeshGH : GH_Component
    {
        int minCrvPts = 10;
        IMesh mesh;

        /// <summary>
        /// Initializes a new instance of the IPatchMeshGH class.
        /// </summary>
        public IPatchMeshGH()
          : base("iMesh from Patch", "iMeshFromPatch",
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
            pManager.AddIntegerParameter("Count", "N", "Minimum number of points used to mesh curves. Default is 10.", GH_ParamAccess.item, minCrvPts);       
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

            DA.GetData(0, ref crv);
            DA.GetDataList(1, pts_patch);
            DA.GetData(2, ref crvRes);
            DA.GetData(4, ref solverOptions);

            List<IguanaGmshConstraint> constraints = new List<IguanaGmshConstraint>();
            foreach (var obj in base.Params.Input[3].VolatileData.AllData(true))
            {
                IguanaGmshConstraint c;
                obj.CastTo<IguanaGmshConstraint>(out c);
                constraints.Add(c);
            }

            mesh = null;
            solverOptions.MinimumCurvePoints = crvRes;

            if (crv.IsClosed)
            {
                IVertexCollection vertices = new IVertexCollection();
                IElementCollection elements = new IElementCollection();

                NurbsCurve nCrv = crv.ToNurbsCurve();

                IguanaGmsh.Initialize();

                // Suface construction
                int surfaceTag = IguanaGmshFactory.OCCSurfacePatch(nCrv, pts_patch, true);

                // Embed constraints
                if(constraints.Count>0) IguanaGmshFactory.OCCEmbedConstraintsOnSurface(constraints, surfaceTag, true);

                // Preprocessing settings
                solverOptions.ApplyBasicPreProcessing2D();

                // 2d mesh generation
                IguanaGmsh.Model.Mesh.Generate(2);

                // Postprocessing settings
                solverOptions.ApplyBasicPostProcessing2D();

                // Iguana mesh construction
                IguanaGmsh.Model.Mesh.TryGetIVertexCollection(ref vertices);
                IguanaGmsh.Model.Mesh.TryGetIElementCollection(ref elements, 2);

                // Iguana mesh construction
                mesh = new IMesh(vertices, elements);
                mesh.BuildTopology();

                IguanaGmsh.FinalizeGmsh();
            }

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
            get { return new Guid("46f44095-6a5b-47d2-848b-4663facc1845"); }
        }
    }
}