using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using static Iguana.IguanaMesh.IUtils.IRhinoGeometry;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IMesh3DFromThickSolidGH : GH_Component
    {
        double[][] entitiesID;
        DrawIDs drawID = DrawIDs.HideEntities;
        bool recompute = true;
        IMesh mesh = null;
        double offset = 0.2;

        /// <summary>
        /// Initializes a new instance of the IMesh3DFromThickSolidGH class.
        /// </summary>
        public IMesh3DFromThickSolidGH()
          : base("iThickMesh3D", "iThickMesh3D",
              "Create a three-dimensional mesh from a brep.",
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
            pManager.AddGenericParameter("iSettings3D", "iSettings", "Three-dimensional meshing settings.", GH_ParamAccess.item);
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
                Brep geom = null;
                IguanaGmshSolver3D solverOptions = new IguanaGmshSolver3D();
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
                    var filename = Path.ChangeExtension(Path.GetTempFileName(), ".stp");

                    Guid id = doc.Objects.AddBrep(geom);
                    ObjRef obj = new ObjRef(id);
                    var tmpObj = doc.Objects.Select(obj);
                    RhinoApp.RunScript("_-Export " + "\"" + filename + "\" _Enter", false);
                    doc.Objects.Delete(obj, true);

                    IguanaGmsh.Initialize();
                    IguanaGmsh.Logger.Start();

                    Tuple<int, int>[] v;
                    IguanaGmsh.Model.GeoOCC.ImportShapes(filename, out v);
                    File.Delete(filename);

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
            get { return GH_Exposure.tertiary; }
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
                return Properties.Resources.iThick3D;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4a835a42-66bb-432a-b444-5f7c3de6796b"); }
        }
    }
}