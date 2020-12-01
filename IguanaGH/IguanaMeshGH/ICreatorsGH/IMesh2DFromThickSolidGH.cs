using System;
using System.Linq;
using System.IO;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using System.Collections.Generic;
using static Iguana.IguanaMesh.IUtils.IRhinoGeometry;
using GH_IO.Serialization;
using System.Windows.Forms;
using Iguana.IguanaMesh.IUtils;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IMesh2DFromThickSolidGH : GH_Component
    {
        double[][] entitiesID;
        DrawIDs drawID = DrawIDs.HideEntities;
        bool recompute = true;
        IMesh mesh = null;
        double offset = 0.2;

        /// <summary>
        /// Initializes a new instance of the IMesh2DFromHollowedVolumesGH class.
        /// </summary>
        public IMesh2DFromThickSolidGH()
          : base("iThickMesh2D", "iThickMesh2D",
              "Create a two-dimensional mesh from a brep.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "Brep", "Base brep.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Offset", "Offset", "Offset distance", GH_ParamAccess.item, offset);
            pManager.AddIntegerParameter("Exclude", "Exclude", "IDs of two-dimensional entities to exclude form the meshing process.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iMeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iConstraints", "iConstraints", "Geometric constraints for mesh generation.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.list);
            pManager.AddGenericParameter("iSettings2D", "iSettings", "Two-dimensional meshing settings.", GH_ParamAccess.item);
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
                Brep geom = null;
                IguanaGmshSolver2D solverOptions = new IguanaGmshSolver2D();
                List<int> excludeSrfTag = new List<int>();
                IguanaGmshField field = null;

                DA.GetData(0, ref geom);
                DA.GetData(1, ref offset);
                DA.GetDataList(2, excludeSrfTag);
                DA.GetData(3, ref field);
                DA.GetData(6, ref solverOptions);

                List<IguanaGmshConstraint> constraints = new List<IguanaGmshConstraint>();
                foreach (var obj in base.Params.Input[4].VolatileData.AllData(true))
                {
                    IguanaGmshConstraint c;
                    obj.CastTo<IguanaGmshConstraint>(out c);
                    constraints.Add(c);
                }

                List<IguanaGmshTransfinite> transfinite = new List<IguanaGmshTransfinite>();
                foreach (var obj in base.Params.Input[5].VolatileData.AllData(true))
                {
                    IguanaGmshTransfinite t;
                    obj.CastTo<IguanaGmshTransfinite>(out t);
                    transfinite.Add(t);
                }

                bool synchronize = true;
                if (constraints.Count > 0) synchronize = false;

                var doc = RhinoDoc.ActiveDoc;
                if (geom.IsSolid)
                {

                    //var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    //var filename = Path.Combine(path, "IG-" + Guid.NewGuid().ToString() + ".step");
                    var filename = Path.ChangeExtension(Path.GetTempFileName(), ".stp");

                    Guid id = doc.Objects.AddBrep(geom);
                    ObjRef obj = new ObjRef(id);
                    var tmpObj = doc.Objects.Select(obj);
                    RhinoApp.RunScript("_-Export " + "\"" + filename + "\" _Enter", false);
                    doc.Objects.Delete(obj, true);

                    IguanaGmsh.Initialize();

                    Tuple<int, int>[] v;
                    IguanaGmsh.Model.GeoOCC.ImportShapes(filename, out v);

                    int volTag = v.First(keypair => keypair.Item1 == 3).Item2;

                    Tuple<int, int>[] dimTags;
                    IguanaGmsh.Model.GeoOCC.AddThickSolid(volTag, excludeSrfTag.ToArray(), offset, out dimTags);
                    IguanaGmsh.Model.GeoOCC.Synchronize();

                    // Embed constraints
                    if (!synchronize) IguanaGmshFactory.Geo.EmbedConstraintsOnBrep(constraints, true);
                    
                    //Transfinite
                    if (transfinite.Count > 0) IguanaGmshFactory.ApplyTransfiniteSettings(transfinite);

                    // Preprocessing settings
                    solverOptions.ApplySolverSettings(field);

                    IguanaGmsh.Model.Mesh.Generate(2);

                    mesh = IguanaGmshFactory.TryGetIMesh();
                    IguanaGmshFactory.TryGetEntitiesID(out entitiesID);

                    IguanaGmsh.FinalizeGmsh();

                    if (File.Exists(filename)) File.Delete(filename);
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
                return Properties.Resources.iThick2D;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f64cb3e5-33d4-4cd2-9956-63fa923f2599"); }
        }
    }
}