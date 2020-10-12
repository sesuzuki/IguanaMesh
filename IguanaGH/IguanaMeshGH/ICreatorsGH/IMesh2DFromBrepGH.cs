using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.ICollections;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IMesh2DFromBrepGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMesh2DFromBrep class.
        /// </summary>
        public IMesh2DFromBrepGH()
          : base("iOpenBrep", "iOBrep",
              "Create a mesh from a curve patch.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Brep to mesh.", GH_ParamAccess.item);
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
            if (!b.IsSolid)
            {
                // Boundary curve
                Curve crv = b.DuplicateNakedEdgeCurves(true, false)[0];

                // Surface points
                int count = minPts;
                Point3d p;
                List<Point3d> pts = new List<Point3d>();
                Interval UU = b.Surfaces[0].Domain(0);
                Interval VV = b.Surfaces[0].Domain(1);
                double u = Math.Abs(UU.Length) / count;
                double v = Math.Abs(VV.Length) / count;
                for (int i = 0; i <= count; i++)
                {
                    for (int j = 0; j <= count; j++)
                    {
                        p = b.Surfaces[0].PointAt(i * u, j * v);
                        pts.Add(p);
                    }
                }

                // Points to patch
                Plane pl;
                Plane.FitPlaneToPoints(pts, out pl);
                List<Point3d> patch = new List<Point3d>();
                foreach (Point3d pt in pts)
                {
                    if (crv.Contains(pt, pl, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance) == PointContainment.Inside) patch.Add(pt);
                }

                /////////////////
                NurbsCurve nCrv = crv.ToNurbsCurve();

                IguanaGmsh.Initialize();

                bool synchronize = true;
                if (constraints.Count > 0) synchronize = false;

                // Suface construction
                int surfaceTag = IguanaGmshFactory.OCCSurfacePatch(nCrv, patch, synchronize);

                // Embed constraints
                if (!synchronize) IguanaGmshFactory.OCCEmbedConstraintsOnSurface(constraints, surfaceTag, true);

                IguanaGmsh.Model.GeoOCC.Synchronize();

                // Preprocessing settings
                solverOptions.ApplyBasic2DSettings();
                solverOptions.ApplyAdvanced2DSettings();

                // 2d mesh generation
                IguanaGmsh.Model.Mesh.Generate(2);

                // Iguana mesh construction
                IVertexCollection vertices;
                IElementCollection elements;
                HashSet<int> parsedNodes;
                IguanaGmsh.Model.Mesh.TryGetIVertexCollection(out vertices);
                IguanaGmsh.Model.Mesh.TryGetIElementCollection(out elements, out parsedNodes, 2);
                if (parsedNodes.Count < vertices.Count) vertices.CullUnparsedNodes(parsedNodes);

                // Iguana mesh construction
                mesh = new IMesh(vertices, elements);
                mesh.BuildTopology();

                IguanaGmsh.FinalizeGmsh();
            }
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
            get { return new Guid("8b5f141b-2d8d-485d-b353-12120803ef31"); }
        }
    }
}