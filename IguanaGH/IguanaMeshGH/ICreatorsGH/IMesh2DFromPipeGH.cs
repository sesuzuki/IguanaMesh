using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino.Geometry;
using static Iguana.IguanaMesh.IUtils.IRhinoGeometry;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IMesh2DFromPipeGH : GH_Component
    {
        double[][] entitiesID;
        DrawIDs drawID = DrawIDs.HideEntities;
        bool recompute = true;
        IMesh mesh = null;

        /// <summary>
        /// Initializes a new instance of the IMesh2DFromPipeGH class.
        /// </summary>
        public IMesh2DFromPipeGH()
          : base("iPipe2D", "iPipe2D",
              "Create a mesh by extruding a curve along another curve.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Profile Curve", "Profile", "Profile curve.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Extrusion Curve", "Extrusion", "Extrusion curve.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iMeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iConstraints", "iConstraints", "Geometric constraints for mesh generation.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.list);
            pManager.AddGenericParameter("iSettings2D", "iSettings", "Two-dimensional meshing settings.", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana surface mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (recompute)
            {
                IguanaGmshSolver2D solverOptions = new IguanaGmshSolver2D();
                IguanaGmshField field = null;
                Curve profCrv = null, extrCrv = null;

                DA.GetData(0, ref profCrv);
                DA.GetData(1, ref extrCrv);
                DA.GetData(2, ref solverOptions);

                DA.GetData(0, ref profCrv);
                DA.GetData(1, ref extrCrv);
                DA.GetData(2, ref field);
                DA.GetData(5, ref solverOptions);

                List<IguanaGmshConstraint> constraints = new List<IguanaGmshConstraint>();
                foreach (var obj in base.Params.Input[3].VolatileData.AllData(true))
                {
                    IguanaGmshConstraint c;
                    obj.CastTo<IguanaGmshConstraint>(out c);
                    constraints.Add(c);
                }

                List<IguanaGmshTransfinite> transfinite = new List<IguanaGmshTransfinite>();
                foreach (var obj in base.Params.Input[4].VolatileData.AllData(true))
                {
                    IguanaGmshTransfinite t;
                    obj.CastTo<IguanaGmshTransfinite>(out t);
                    transfinite.Add(t);
                }

                bool synchronize = true;
                if (constraints.Count > 0) synchronize = false;

                IguanaGmsh.Initialize();

                int wireTag = IguanaGmshFactory.GeoOCC.CurveLoopFromRhinoCurve(extrCrv, 1);
                int wireTag2;
                if (profCrv.IsCircle())
                {
                    Circle c;
                    profCrv.TryGetCircle(out c);
                    Point3d ce = c.Center;
                    int crvTag = IguanaGmsh.Model.GeoOCC.AddCircle(ce.X, ce.Y, ce.Z, c.Radius);
                    wireTag2 = IguanaGmsh.Model.GeoOCC.AddWire(new int[] { crvTag });
                }
                else if (profCrv.IsPolyline())
                {
                    Polyline pl;
                    profCrv.TryGetPolyline(out pl);
                    wireTag2 = IguanaGmshFactory.GeoOCC.PolylineFromRhinoCurve(pl, 1);
                }
                else wireTag2 = IguanaGmshFactory.GeoOCC.CurveLoopFromRhinoCurve(profCrv, 1);
                int srfTag = IguanaGmshFactory.GeoOCC.SurfacePatch(wireTag2, new List<Point3d>(), false);

                Tuple<int, int>[] dimTags;
                IguanaGmsh.Model.GeoOCC.AddPipe(new Tuple<int, int>[] { Tuple.Create(2, srfTag) }, wireTag, out dimTags);
                IguanaGmsh.Model.GeoOCC.Synchronize();

                // Embed constraints
                if (!synchronize) IguanaGmshFactory.Geo.EmbedConstraintsOnBrep(constraints, true);

                //Transfinite
                if (transfinite.Count > 0) IguanaGmshFactory.ApplyTransfiniteSettings(transfinite);

                // Preprocessing settings
                solverOptions.ApplySolverSettings(field);

                IguanaGmsh.Model.Mesh.Generate(3);

                mesh = IguanaGmshFactory.TryGetIMesh(3);
                IguanaGmshFactory.TryGetEntitiesID(out entitiesID);

                IguanaGmsh.FinalizeGmsh();
            }

            recompute = true;
            DA.SetData(0, mesh);
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
                return Properties.Resources.iPipe;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a878cd2b-e67e-47d8-9336-2a2765d073c0"); }
        }
    }
}