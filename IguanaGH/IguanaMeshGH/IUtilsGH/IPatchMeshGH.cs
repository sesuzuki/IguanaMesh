using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IGmshWrappers;
using Iguana.IguanaMesh.ITypes;
using Rhino.Display;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IPatchMeshGH : GH_Component
    {
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
            pManager.AddIntegerParameter("Count", "N", "Number of control points to rebuild curve", GH_ParamAccess.item);       
            pManager.AddGenericParameter("IConstraints", "IConstraints", "Point constraint", GH_ParamAccess.item);            
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
            IguanaGmshSolverOptions solverOpt = new IguanaGmshSolverOptions();
            IguanaGmshConstraintCollector iCol = null;

            DA.GetData(0, ref crv);
            DA.GetDataList(1, pts_patch);
            DA.GetData(2, ref crvRes);

            DA.GetData(3, ref iCol);

            DA.GetData(4, ref solverOpt);

            IMesh mesh = null;

            if (crv.IsClosed)
            {
                NurbsCurve nCrv = crv.ToNurbsCurve();
                if (crvRes > 0 && nCrv.Points.Count < crvRes) nCrv = nCrv.Rebuild(crvRes, nCrv.Degree, true);
                nCrv.MakePiecewiseBezier(true);
                NurbsCurvePointList nPts = nCrv.Points;
                NurbsCurveKnotList nKnots = nCrv.Knots;

                Gmsh.Initialize();

                int[] nTags = new int[nPts.Count];
                double[] nW = new double[nPts.Count];
                Point3d[] pts = new Point3d[nPts.Count];

                ControlPoint pt;
                for (int i = 0; i < nPts.Count-1; i++)
                {
                    pt = nPts[i];
                    nTags[i] = Gmsh.Model.GeoOCC.AddPoint(pt.X, pt.Y, pt.Z, 0.1);
                    pts[i] = nPts[i].Location;
                    nW[i] = nPts.GetWeight(i);
                }
                nW[nPts.Count-1]= nW[0];
                nTags[nPts.Count - 1] = nTags[0];

                //Points to patch
                Point3d pp;
                int[] patchPts = new int[pts_patch.Count];
                for (int i = 0; i < pts_patch.Count; i++)
                {
                    pp = pts_patch[i];
                    patchPts[i] = Gmsh.Model.GeoOCC.AddPoint(pp.X, pp.Y, pp.Z, 1);
                }

                int splineTag = Gmsh.Model.GeoOCC.AddBSpline(nTags, nCrv.Degree, nW);
                int wireTag = Gmsh.Model.GeoOCC.AddWire(new int[] { splineTag });

                int surfaceTag = Gmsh.Model.GeoOCC.AddSurfaceFilling(wireTag, patchPts);
               
                //////////////////////////////////////////// EMBED
                if (iCol != null)
                {
                    int count;
                    //Point constraint
                    if (iCol.HasPointConstraints())
                    {
                        count = iCol.GetPointConstraintCount();
                        int[] embedPts = new int[count];
                        Tuple<Point3d, double> data;
                        Point3d p;
                        double d;
                        for (int i = 0; i < count; i++)
                        {
                            data = iCol.GetPointConstraint(i);
                            p = data.Item1;
                            d = data.Item2;

                            embedPts[i] = Gmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z, d);
                        }

                        Gmsh.Model.GeoOCC.Synchronize();

                        Gmsh.Model.Mesh.Embed(0, embedPts, 2, surfaceTag);
                    }

                    //Curve constraint
                    if (iCol.HasCurveConstraints())
                    {
                        //TODO
                        Curve auxC;
                        count = iCol.GetCurveConstraintCount();
                        Tuple<Curve, double> data;
                        double d;
                        for (int i = 0; i < count; i++)
                        {
                            data = iCol.GetCurveConstraint(i);
                            auxC = data.Item1;
                            d = data.Item2;

                            if (auxC.IsLinear())
                            {
                               
                            }
                        }
                    }
                }

                solverOpt.ApplyBasicPreProcessing2D();

                Gmsh.Model.Mesh.Generate(2);

                solverOpt.ApplyBasicPostProcessing2D();

                // Iguana mesh construction
                IVertexCollection vertices = Gmsh.Model.Mesh.TryGetIVertexCollection();
                IElementCollection elements = Gmsh.Model.Mesh.TryGetIElementCollection();
                mesh = new IMesh(vertices, elements);
                mesh.BuildTopology();

                Gmsh.FinalizeGmsh();
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
            get { return new Guid("46f44095-6a5b-47d2-848b-4663facc1845"); }
        }
    }
}