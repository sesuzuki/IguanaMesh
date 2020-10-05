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
using Rhino;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IMesh3DFromExtrusionGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IVolumeMeshFromExtrusion class.
        /// </summary>
        public IMesh3DFromExtrusionGH()
          : base("iExtrusion3D", "iExtr3D",
              "Extrude sufaces along a vector to generate a three-dimensional mesh.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "S", "Base surface.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Direction", "D", "Direction.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "L", "Extrusion length.", GH_ParamAccess.list, 1);
            pManager.AddIntegerParameter("Division", "D", "Number of divisions per extrusion", GH_ParamAccess.list, 1);
            pManager.AddIntegerParameter("MeshingPoints", "M", "Minimum number of points used to mesh edge-surfaces. Default value is 10.", GH_ParamAccess.item, 10);
            pManager.AddGenericParameter("iConstraints", "iC", "Constraints for mesh generation.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iSettings", "iS3D", "Three-dimensional meshing settings", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana Volumetric Mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep b = null;
            Vector3d dir = new Vector3d();
            List<double> lengths = new List<double>();
            List<int> divisions = new List<int>();
            int minPts = 10;
            IguanaGmshSolver3D solverOptions = new IguanaGmshSolver3D();

            DA.GetData(0, ref b);
            DA.GetData(1, ref dir);
            DA.GetDataList(2, lengths);
            DA.GetDataList(3, divisions);
            DA.GetData(4, ref minPts);
            DA.GetData(6, ref solverOptions);

            List<IguanaGmshConstraint> constraints = new List<IguanaGmshConstraint>();
            foreach (var obj in base.Params.Input[5].VolatileData.AllData(true))
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

                // Extrude
                int[] ov;
                IguanaGmsh.Model.GeoOCC.Extrude(new[] { 2, surfaceTag }, dir.X, dir.Y, dir.Z, out ov, divisions.ToArray(), lengths.ToArray(), true);

                IguanaGmsh.Model.GeoOCC.Synchronize();

                // Preprocessing settings
                solverOptions.ApplyBasic3DSettings();
                solverOptions.ApplyAdvanced3DSettings();

                // 2d mesh generation
                IguanaGmsh.Model.Mesh.Generate(3);

                // Iguana mesh construction
                IVertexCollection vertices;
                IElementCollection elements;
                HashSet<int> parsedNodes;
                IguanaGmsh.Model.Mesh.TryGetIVertexCollection(out vertices);
                IguanaGmsh.Model.Mesh.TryGetIElementCollection(out elements, out parsedNodes, 3);
                if (parsedNodes.Count<vertices.Count) vertices.CullUnparsedNodes(parsedNodes);

                // Iguana mesh construction
                mesh = new IMesh(vertices, elements);
                mesh.BuildTopology();

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
            get { return new Guid("56d8270a-6fbe-49a2-bfd8-f7ac1b1f88b5"); }
        }
    }
}