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
using GH_IO.Serialization;
using System.Windows.Forms;
using static Iguana.IguanaMesh.IUtils.IRhinoGeometry;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IMesh3DFromSurfaceExtrusionGH : GH_Component
    {
        double[][] entitiesID;
        DrawIDs drawID = DrawIDs.HideEntities;
        bool recompute = true;
        IMesh mesh = null;

        /// <summary>
        /// Initializes a new instance of the IVolumeMeshFromExtrusion class.
        /// </summary>
        public IMesh3DFromSurfaceExtrusionGH()
          : base("iExtrusionSurface", "iExtrusionSrf",
              "Extrude a suface along a vector to generate a three-dimensional mesh.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "Srf", "Base surface.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Direction", "Direction", "Direction.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "Length", "Extrusion length.", GH_ParamAccess.list, 1);
            pManager.AddIntegerParameter("Divisions", "Divisions", "Number of divisions per extrusion", GH_ParamAccess.list, 1);
            pManager.AddGenericParameter("iMeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iConstraints", "iConstraints", "Geometric constraints for mesh generation.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.list);
            pManager.AddGenericParameter("iSettings3D", "iSettings", "Three-dimensional meshing settings.", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
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
            if (recompute)
            {
                Brep b = null;
                Vector3d dir = new Vector3d();
                List<double> lengths = new List<double>();
                List<int> divisions = new List<int>();
                IguanaGmshField field = null;
                IguanaGmshSolver3D solverOptions = new IguanaGmshSolver3D();

                DA.GetData(0, ref b);
                DA.GetData(1, ref dir);
                DA.GetDataList(2, lengths);
                DA.GetDataList(3, divisions);
                DA.GetData(4, ref field);
                DA.GetData(7, ref solverOptions);

                List<IguanaGmshConstraint> constraints = new List<IguanaGmshConstraint>();
                foreach (var obj in base.Params.Input[5].VolatileData.AllData(true))
                {
                    IguanaGmshConstraint c;
                    obj.CastTo<IguanaGmshConstraint>(out c);
                    constraints.Add(c);
                }

                List<IguanaGmshTransfinite> transfinite = new List<IguanaGmshTransfinite>();
                foreach (var obj in base.Params.Input[6].VolatileData.AllData(true))
                {
                    IguanaGmshTransfinite t;
                    obj.CastTo<IguanaGmshTransfinite>(out t);
                    transfinite.Add(t);
                }


                // Extract required data from base surface
                if (!b.IsSolid && b.Faces.Count == 1)
                {
                    Curve crv;
                    List<Point3d> patch;
                    IRhinoGeometry.GetBrepFaceMeshingData(b, 0, solverOptions.MinimumCurvePoints, out crv, out patch);

                    IguanaGmsh.Initialize();

                    bool synchronize = true;
                    if (constraints.Count > 0) synchronize = false;

                    // Suface construction
                    int wireTag = IguanaGmshFactory.GeoOCC.CurveLoopFromRhinoCurve(crv, solverOptions.TargetMeshSizeAtNodes[0]);
                    int surfaceTag = IguanaGmshFactory.GeoOCC.SurfacePatch(wireTag, patch, synchronize);

                    // Embed constraints
                    if (!synchronize) IguanaGmshFactory.GeoOCC.EmbedConstraintsOnSurface(constraints, surfaceTag, true);

                    //Transfinite
                    if (transfinite.Count > 0) IguanaGmshFactory.ApplyTransfiniteSettings(transfinite);

                    // Extrude
                    Tuple<int, int>[] ov;
                    Tuple<int, int>[] temp = new Tuple<int, int>[] { Tuple.Create(2, surfaceTag) }; ;
                    IguanaGmsh.Model.GeoOCC.Extrude(temp, dir.X, dir.Y, dir.Z, out ov, divisions.ToArray(), lengths.ToArray(), true);

                    IguanaGmsh.Model.GeoOCC.Synchronize();

                    // Preprocessing settings
                    solverOptions.ApplyBasic3DSettings();
                    solverOptions.ApplyAdvanced3DSettings();

                    // 2d mesh generation
                    IguanaGmsh.Model.Mesh.Generate(3);

                    // Iguana mesh construction
                    mesh = IguanaGmshFactory.TryGetIMesh(3);
                    IguanaGmshFactory.TryGetEntitiesID(out entitiesID);

                    IguanaGmsh.FinalizeGmsh();
                }
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