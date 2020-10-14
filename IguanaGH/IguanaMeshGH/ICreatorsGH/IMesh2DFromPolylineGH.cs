using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.ICollections;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.IWrappers.IExtensions;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IMesh2DFromPolylineGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMeshFromPolyline class.
        /// </summary>
        public IMesh2DFromPolylineGH()
          : base("iPolylinePatch", "iPoly",
              "Create a two-dimensional mesh from a closed polyline.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Outer boundary", "Outer", "External boundary", GH_ParamAccess.item);
            pManager.AddCurveParameter("Inner boundaries", "Inner", "Holes as polylines", GH_ParamAccess.list);
            pManager.AddIntegerParameter("MeshingPoints", "M", "Minimum number of points used to mesh edge-surfaces. Default value is 10.", GH_ParamAccess.item, 10);
            pManager.AddGenericParameter("iConstraints", "iC", "Constraints for mesh generation.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iSettings", "iS2D", "Two-dimensional meshing settings.", GH_ParamAccess.item);
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
            Curve _outer = null;
            List<Curve> _inner = new List<Curve>();
            IguanaGmshSolver2D solverOptions = new IguanaGmshSolver2D();
            int minPts = 10;
            //Retrieve vertices and elements
            DA.GetData(0, ref _outer);
            DA.GetDataList(1, _inner);
            DA.GetData(2, ref minPts);
            DA.GetData(4, ref solverOptions);

            List<IguanaGmshConstraint> constraints = new List<IguanaGmshConstraint>();
            foreach (var obj in base.Params.Input[3].VolatileData.AllData(true))
            {
                IguanaGmshConstraint c;
                obj.CastTo<IguanaGmshConstraint>(out c);
                constraints.Add(c);
            }

            IguanaGmsh.Initialize();

            bool synchronize = true;
            if (constraints.Count > 0) synchronize = false;

            int surfaceTag = IguanaGmshFactory.Geo.GmshPlaneSurface(_outer, _inner, solverOptions);

            // Embed constraints
            if (!synchronize) IguanaGmshFactory.Geo.EmbedConstraintsOnSurface(constraints, surfaceTag, true);
            else IguanaGmsh.Model.Geo.Synchronize();

            //solver options
            solverOptions.ApplyBasic2DSettings();
            solverOptions.ApplyAdvanced2DSettings();

            IguanaGmsh.Model.Mesh.Generate(2);

            // Iguana mesh construction
            IMesh mesh = IguanaGmshFactory.TryGetIMesh();

            IguanaGmsh.FinalizeGmsh();

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
            get { return new Guid("40271dbd-73f2-4d37-bfd0-0d5ba5066999"); }
        }
    }
}