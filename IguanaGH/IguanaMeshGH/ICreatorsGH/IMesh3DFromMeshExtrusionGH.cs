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
    public class IMesh3DFromMeshExtrusionGH : GH_Component
    {
        double[][] entitiesID;
        DrawIDs drawID = DrawIDs.HideEntities;
        bool recompute = true;
        IMesh mesh = null;

        /// <summary>
        /// Initializes a new instance of the IExtrudeGH class.
        /// </summary>
        public IMesh3DFromMeshExtrusionGH()
          : base("IExtrudeMesh", "iExtMesh",
              "Extrude Rhino mesh along a vector to generate a three-dimensional mesh.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Base Rhino mesh.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Direction", "D", "Direction.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "L", "Extrusion length.", GH_ParamAccess.list, 1);
            pManager.AddIntegerParameter("Division", "D", "Number of divisions per extrusion", GH_ParamAccess.list, 1);
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
                Mesh bMesh = null;
                Vector3d dir = new Vector3d();
                List<double> lengths = new List<double>();
                List<int> divisions = new List<int>();
                IguanaGmshField field = null;
                IguanaGmshSolver3D solverOptions = new IguanaGmshSolver3D();

                DA.GetData(0, ref bMesh);
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
                if (!bMesh.IsClosed)
                {
                    IguanaGmsh.Initialize();

                    Tuple<int, int>[] dimTags;
                    IguanaGmshFactory.Geo.GmshSurfaceFromMesh(bMesh, out dimTags);
                    Tuple<int, int>[] temp;
                    IguanaGmsh.Model.GetEntities(out temp, 2);

                    bool synchronize = true;
                    if (constraints.Count > 0) synchronize = false;

                    // Embed constraints
                    if (!synchronize) IguanaGmshFactory.Geo.EmbedConstraintsOnSurface(constraints, temp[0].Item2, true);

                    //Transfinite
                    if (transfinite.Count > 0) IguanaGmshFactory.ApplyTransfiniteSettings(transfinite);

                    // Extrude
                    Tuple<int, int>[] ov;
                    IguanaGmsh.Model.Geo.Extrude(temp, dir.X, dir.Y, dir.Z, out ov, divisions.ToArray(), lengths.ToArray(), true);

                    IguanaGmsh.Model.Geo.Synchronize();

                    // Preprocessing settings
                    //solverOptions.ApplyBasic3DSettings();
                    //solverOptions.ApplyAdvanced3DSettings();

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
            get { return new Guid("276e0241-1a1b-4d68-91d6-7142354cb501"); }
        }
    }
}