using System;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;
using Iguana.IguanaMesh.IWrappers.ISolver;
using System.Collections.Generic;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino;
using Iguana.IguanaMesh.IUtils;
using System.Windows.Forms;
using static Iguana.IguanaMesh.IUtils.IRhinoGeometry;
using GH_IO.Serialization;
using System.Linq;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IMeshFromSurfaceGH : GH_Component
    {
        double[][] entitiesID;
        DrawIDs drawID = DrawIDs.HideEntities;
        bool recompute = true;
        IMesh mesh = null;

        /// <summary>
        /// Initializes a new instance of the IMeshFromEdges class.
        /// </summary>
        public IMeshFromSurfaceGH()
          : base("iPatchSurface", "iPatchSrf",
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
            pManager.AddGenericParameter("iMeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iConstraints", "iConstraints", "Geometric constraints for mesh generation.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.item);
            pManager.AddGenericParameter("iSettings2D", "iSettings", "Two-dimensional meshing settings.", GH_ParamAccess.item);
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
            pManager.AddGenericParameter("iMesh", "iM", "Iguana surface mesh.", GH_ParamAccess.item);
            pManager.AddTextParameter("Info", "Info", "Log information about the meshing process.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string logInfo = "Empty mesh";

            if (recompute)
            {
                Brep b = null;
                IguanaGmshSolver2D solverOptions = new IguanaGmshSolver2D();
                IguanaGmshField field = null;

                DA.GetData(0, ref b);
                DA.GetData(1, ref field);
                DA.GetData(4, ref solverOptions);

                List<IguanaGmshConstraint> constraints = new List<IguanaGmshConstraint>();
                foreach (var obj in base.Params.Input[2].VolatileData.AllData(true))
                {
                    IguanaGmshConstraint c;
                    obj.CastTo<IguanaGmshConstraint>(out c);
                    constraints.Add(c);
                }

                List<IguanaGmshTransfinite> transfinite = new List<IguanaGmshTransfinite>();
                foreach (var obj in base.Params.Input[3].VolatileData.AllData(true))
                {
                    IguanaGmshTransfinite t;
                    obj.CastTo<IguanaGmshTransfinite>(out t);
                    transfinite.Add(t);
                }

                // Extract required data from base surface
                if (!b.IsSolid && b.Faces.Count == 1)
                {
                    Curve[] crv;
                    List<Point3d> patch;
                    IRhinoGeometry.GetBrepFaceMeshingData(b, 0, solverOptions.MinimumCurvePoints, out crv, out patch);

                    IguanaGmsh.Initialize();
                    IguanaGmsh.Logger.Start();

                    bool synchronize = true;
                    if (constraints.Count > 0) synchronize = false;

                    // Suface construction
                    int wireTag = IguanaGmshFactory.GeoOCC.CurveLoopFromRhinoCurve(crv[0], solverOptions.TargetMeshSizeAtNodes[0]);
                    int surfaceTag = IguanaGmshFactory.GeoOCC.SurfacePatch(wireTag, patch, false);
                    Tuple<int, int>[] objectDimTag = new Tuple<int, int>[] { Tuple.Create(2, surfaceTag) };

                    //Tool
                    Point3d centroid = new Point3d();
                    Interval dom = crv[0].Domain;
                    int count = 10;
                    double t = Math.Abs(dom.Length) / count;
                    for (int i = 0; i < count; i++)
                    {
                        centroid += crv[0].PointAt(dom.T0 + i * t);
                    }
                    centroid /= count;

                    double u, v;
                    b.Surfaces[0].ClosestPoint(centroid, out u, out v);
                    Vector3d n = b.Surfaces[0].NormalAt(u, v);

                    List<Tuple<int, int>> toolDimTags = new List<Tuple<int, int>>();
                    for (int i = 1; i < crv.Length; i++)
                    {
                        Curve cA = crv[i].DuplicateCurve();
                        cA.Translate(n);
                        Curve cB = crv[i].DuplicateCurve();
                        cB.Translate(-n);
                        int wireTag2 = IguanaGmshFactory.GeoOCC.CurveLoopFromRhinoCurve(cA, 10);
                        int wireTag3 = IguanaGmshFactory.GeoOCC.CurveLoopFromRhinoCurve(cB, 10);
                        Tuple<int, int>[] temp;
                        IguanaGmsh.Model.GeoOCC.AddThruSections(new[] { wireTag2, wireTag3 }, out temp, -1, true, true);
                        toolDimTags.AddRange(temp);
                    }

                    Tuple<int, int>[] dimTag;
                    IguanaGmsh.Model.GeoOCC.Cut(objectDimTag, toolDimTags.ToArray(), out dimTag, -1, true, true);

                    // Embed constraints
                    if (!synchronize) IguanaGmshFactory.GeoOCC.EmbedConstraintsOnSurface(constraints, surfaceTag, true); 
                    else IguanaGmsh.Model.GeoOCC.Synchronize();

                    //Transfinite
                    if (transfinite.Count > 0) IguanaGmshFactory.ApplyTransfiniteSettings(transfinite);

                    // Preprocessing settings
                    solverOptions.ApplySolverSettings(field);

                    // 2d mesh generation
                    IguanaGmsh.Model.Mesh.Generate(2);

                    // Iguana mesh construction
                    mesh = IguanaGmshFactory.TryGetIMesh();
                    IguanaGmshFactory.TryGetEntitiesID(out entitiesID);

                    logInfo = IguanaGmsh.Logger.Get();
                    IguanaGmsh.Logger.Stop();

                    IguanaGmsh.FinalizeGmsh();
                }
            }

            recompute = true;
            DA.SetData(0, mesh);
            DA.SetData(1, logInfo);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            base.DrawViewportMeshes(args);
            IRhinoGeometry.DrawElementsID(args, entitiesID, drawID);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("DrawIDs", (int)drawID);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            int aIndex = -1;
            if (reader.TryGetInt32("DrawIDs", ref aIndex))
            {
                drawID = (DrawIDs)aIndex;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            foreach (DrawIDs s in Enum.GetValues(typeof(DrawIDs)))
                GH_Component.Menu_AppendItem(menu, s.ToString(), DrawingIDType, true, s == this.drawID).Tag = s;
            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void DrawingIDType(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is DrawIDs)
            {
                this.drawID = (DrawIDs)item.Tag;
                recompute = false;
                ExpireSolution(true);
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iSurfacePatch;
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