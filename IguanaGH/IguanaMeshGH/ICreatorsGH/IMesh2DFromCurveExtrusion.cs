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
    public class IMesh2DFromCurveExtrusion : GH_Component
    {
        double[][] entitiesID;
        DrawIDs drawID = DrawIDs.HideEntities;
        bool recompute = true;
        IMesh mesh = null;
        Vector3d dir = new Vector3d(0, 0, 1);

        /// <summary>
        /// Initializes a new instance of the IMesh2DFromExtrusion class.
        /// </summary>
        public IMesh2DFromCurveExtrusion()
          : base("iExtrudeCurve", "iExtrudeCrv",
              "Create a mesh from a curve extrusion.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Base closed curve.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Direction", "Direction", "Direction.", GH_ParamAccess.item, dir);
            pManager.AddNumberParameter("Length", "Length", "Extrusion length.", GH_ParamAccess.item, 1);
            pManager.AddGenericParameter("MeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.item);
            pManager.AddGenericParameter("iSettings2D", "iSettings", "Two-dimensional meshing settings.", GH_ParamAccess.item);
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
                Curve crv = null;
                double length = 1;
                IguanaGmshField field = null;
                mesh = new IMesh();
                IguanaGmshSolver2D solverOptions = new IguanaGmshSolver2D();

                DA.GetData(0, ref crv);
                DA.GetData(1, ref dir);
                DA.GetData(2, ref length);
                DA.GetData(3, ref field);
                DA.GetData(5, ref solverOptions);


                List<IguanaGmshTransfinite> transfinite = new List<IguanaGmshTransfinite>();
                foreach (var obj in base.Params.Input[4].VolatileData.AllData(true))
                {
                    IguanaGmshTransfinite t;
                    obj.CastTo<IguanaGmshTransfinite>(out t);
                    transfinite.Add(t);
                }

                IguanaGmsh.Initialize();
                Tuple<int, int>[] dimTags;
                double size;
                dir.Unitize();
                dir *= length;

                if (crv.IsPolyline())
                {
                    Polyline poly;
                    crv.TryGetPolyline(out poly);
                    List<int> ptsTags = new List<int>();
                    PointCloud ptsCloud = new PointCloud();
                    int tag;
                    Point3d p;
                    size = solverOptions.TargetMeshSizeAtNodes[0];
                    bool flag = false;
                    if (poly.Count == solverOptions.TargetMeshSizeAtNodes.Count) flag = true;
                    
                    int[] tempTags = new int[poly.Count];
                    int idx;
                    for (int i = 0; i < poly.Count; i++)
                    {
                        p = poly[i];
                        idx = IguanaGmshFactory.EvaluatePoint(ptsCloud, p, 0.001);

                        if (idx == -1)
                        {
                            if(flag) size = solverOptions.TargetMeshSizeAtNodes[i];
                            tag = IguanaGmsh.Model.Geo.AddPoint(p.X, p.Y, p.Z, size);
                            ptsTags.Add(tag);
                            ptsCloud.Add(p);
                            idx = ptsCloud.Count - 1;
                        }

                        tempTags[i] = idx;
                    }

                    dimTags = new Tuple<int, int>[poly.SegmentCount];
                    for (int i = 0; i < poly.SegmentCount; i++)
                    {
                        tag = IguanaGmsh.Model.Geo.AddLine(ptsTags[tempTags[i]], ptsTags[tempTags[i + 1]]);
                        dimTags[i] = Tuple.Create(1, tag);
                    }
                }
                else
                {
                    size = solverOptions.TargetMeshSizeAtNodes[0];
                    int[] crvTag = IguanaGmshFactory.Geo.SplinesFromRhinoCurve(crv, size);
                    dimTags = new Tuple<int, int>[crvTag.Length];
                    for (int i = 0; i < crvTag.Length; i++) dimTags[i] = Tuple.Create(1, crvTag[i]);
                }

                Tuple<int, int>[] outDimTags;
                IguanaGmsh.Model.Geo.Extrude(dimTags, dir.X, dir.Y, dir.Z, out outDimTags);

                IguanaGmsh.Model.Geo.Synchronize();

                if (!crv.IsPolyline())
                {
                    IguanaGmsh.Model.GetEntities(out outDimTags, 2);
                    int[] temp = new int[outDimTags.Length];
                    for (int i = 0; i < temp.Length; i++) temp[i] = outDimTags[i].Item2;

                    IguanaGmsh.Model.Mesh.SetCompound(2, temp);
                }

                //Transfinite
                if (transfinite.Count > 0) IguanaGmshFactory.ApplyTransfiniteSettings(transfinite);

                // Preprocessing settings
                solverOptions.ApplySolverSettings(field);

                IguanaGmsh.Model.Mesh.Generate(2);
                mesh = IguanaGmshFactory.TryGetIMesh(2);
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
                return Properties.Resources.iExtrudeCurve;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7d121dfc-064f-4f74-a9dd-268c296d47be"); }
        }
    }
}