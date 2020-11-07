using System;
using System.Collections.Generic;
using System.Linq;
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
    public class IMesh3DFromLoftGH : GH_Component
    {
        double[][] entitiesID;
        DrawIDs drawID = DrawIDs.HideEntities;
        bool recompute = true;
        IMesh mesh = null;

        /// <summary>
        /// Initializes a new instance of the IMesh3DFromLoftGH class.
        /// </summary>
        public IMesh3DFromLoftGH()
          : base("iLoft3D", "iLoft3D",
              "Create a three-dimensional mesh from a loft.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Base curves to loft.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iMeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iConstraints", "iConstraints", "Geometric constraints for mesh generation.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.list);
            pManager.AddGenericParameter("iSettings3D", "iSettings", "Three-dimensional meshing settings.", GH_ParamAccess.item);
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
            pManager.AddGenericParameter("iMesh", "iM", "Iguana volume mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (recompute)
            {
                List<Curve> crv = new List<Curve>();
                IguanaGmshSolver3D solverOptions = new IguanaGmshSolver3D();
                IguanaGmshField field = null;

                DA.GetDataList(0, crv);
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
                IguanaGmsh.Initialize();

                bool synchronize = true;
                if (constraints.Count > 0) synchronize = false;

                List<int> crvTag = new List<int>();
                foreach (Curve c in crv)
                {
                    int wireTag;
                    if (!c.IsPolyline()) wireTag = IguanaGmshFactory.GeoOCC.CurveLoopFromRhinoCurve(c, solverOptions.TargetMeshSizeAtNodes[0]);
                    else
                    {
                        Polyline poly;
                        c.TryGetPolyline(out poly);
                        wireTag = IguanaGmshFactory.GeoOCC.PolylineFromRhinoCurve(poly, solverOptions.TargetMeshSizeAtNodes[0]);
                    }

                    crvTag.Add(wireTag);
                }

                Tuple<int, int>[] temp;
                IguanaGmsh.Model.GeoOCC.AddThruSections(crvTag.ToArray(), out temp, -1, true, true);

                // Embed constraints
                if (!synchronize) IguanaGmshFactory.GeoOCC.EmbedConstraintsOnBrep(constraints, true);
                else IguanaGmsh.Model.GeoOCC.Synchronize();

                //Transfinite
                if (transfinite.Count > 0) IguanaGmshFactory.ApplyTransfiniteSettings(transfinite);

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

            recompute = true;
            DA.SetData(0, mesh);
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
            get { return new Guid("dfe2d9a8-e556-4921-8878-2d0356766358"); }
        }
    }
}