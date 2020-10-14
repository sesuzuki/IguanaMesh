using System;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.ICollections;
using Rhino.Geometry;
using Iguana.IguanaMesh.IWrappers.ISolver;
using System.Collections.Generic;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino;
using Iguana.IguanaMesh.IUtils;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IMeshFromSurfaceGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMeshFromEdges class.
        /// </summary>
        public IMeshFromSurfaceGH()
          : base("iSurfacePatch", "iSrfPatch",
              "Create a mesh from a surface patch.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "Srf", "Base surface.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("MeshingPoints", "M", "Minimum number of points used to mesh edge-surfaces. Default value is 10.", GH_ParamAccess.item, 10);
            pManager.AddGenericParameter("iConstraints", "iC", "Constraints for mesh generation.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iSettings", "iS2D", "Two-dimensional meshing settings", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana Surface Mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep b = null;
            int minPts = 10;
            IguanaGmshSolver2D solverOptions = new IguanaGmshSolver2D();

            DA.GetData(0, ref b);
            DA.GetData(1, ref minPts);
            DA.GetData(3, ref solverOptions);

            List<IguanaGmshConstraint> constraints = new List<IguanaGmshConstraint>();
            foreach (var obj in base.Params.Input[2].VolatileData.AllData(true))
            {
                IguanaGmshConstraint c;
                obj.CastTo<IguanaGmshConstraint>(out c);
                constraints.Add(c);
            }

            IMesh mesh = null;
            solverOptions.MinimumCurvePoints = minPts;

            // Extract required data from base surface
            if (!b.IsSolid && b.Faces.Count==1)
            {
                Curve crv;
                List<Point3d> patch;
                IRhinoGeometry.GetBrepFaceMeshingData(b, 0, minPts, out crv, out patch);

                IguanaGmsh.Initialize();

                bool synchronize = true;
                if (constraints.Count > 0) synchronize = false;

                // Suface construction
                int surfaceTag = IguanaGmshFactory.GeoOCC.SurfacePatch(crv, solverOptions, patch, synchronize);

                // Embed constraints
                if (!synchronize) IguanaGmshFactory.GeoOCC.EmbedConstraintsOnSurface(constraints, surfaceTag, true);

                IguanaGmsh.Model.GeoOCC.Synchronize();

                // Preprocessing settings
                solverOptions.ApplyBasic2DSettings();
                solverOptions.ApplyAdvanced2DSettings();

                // 2d mesh generation
                IguanaGmsh.Model.Mesh.Generate(2);

                // Iguana mesh construction
                mesh = IguanaGmshFactory.TryGetIMesh();

                IguanaGmsh.FinalizeGmsh();
            }

            DA.SetData(0, mesh);
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
            get { return new Guid("927b9c99-ecc6-4b18-b1be-9be051361169"); }
        }
    }
}