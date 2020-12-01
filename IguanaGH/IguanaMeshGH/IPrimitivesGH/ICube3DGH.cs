using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IPrimitivesGH
{
    public class ICube3DGH : GH_Component
    {
        private int u = 5, v = 5, w = 5;
        private Boolean weld = false;
        private double tolerance = 0.01;

        /// <summary>
        /// Initializes a new instance of the ICube3DGH class.
        /// </summary>
        public ICube3DGH()
          : base("iCube3D", "iCube3D",
              "Construct a cube volume mesh",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("Base Box", "B", "Base box to construct the cube mesh.", GH_ParamAccess.item, new Box(Plane.WorldXY, new Interval(-1, 1), new Interval(-1, 1), new Interval(-1, 1)));
            pManager.AddIntegerParameter("U Count", "U", "Number of faces along the {x} direction.", GH_ParamAccess.item, u);
            pManager.AddIntegerParameter("V Count", "V", "Number of faces along the {y} direction.", GH_ParamAccess.item, v);
            pManager.AddIntegerParameter("W Count", "W", "Number of faces along the {z} direction.", GH_ParamAccess.item, w);
            pManager.AddBooleanParameter("Weld", "We", "Weld creases in the mesh.", GH_ParamAccess.item, weld);
            pManager.AddNumberParameter("Tolerance", "t", "Welding tolerance (Vertices smaller than this tolerance will be merged)", GH_ParamAccess.item, tolerance);
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
            IMesh mesh = new IMesh();
            Box box = new Box();

            DA.GetData(0, ref box);
            DA.GetData(1, ref u);
            DA.GetData(2, ref v);
            DA.GetData(3, ref w);
            DA.GetData(4, ref weld);
            DA.GetData(5, ref tolerance);

            IguanaGmsh.Initialize();

            Point3d p1 = box.PointAt(0, 0, 0);
            Point3d p2 = box.PointAt(1, 0, 0);
            Point3d p3 = box.PointAt(1, 1, 0);
            Point3d p4 = box.PointAt(0, 1, 0);

            IguanaGmsh.Model.GeoOCC.AddPoint(p1.X, p1.Y, p1.Z, 1);
            IguanaGmsh.Model.GeoOCC.AddPoint(p2.X, p2.Y, p2.Z, 2);
            IguanaGmsh.Model.GeoOCC.AddPoint(p3.X, p3.Y, p3.Z, 3);
            IguanaGmsh.Model.GeoOCC.AddPoint(p4.X, p4.Y, p4.Z, 4);
            int l1 = IguanaGmsh.Model.GeoOCC.AddLine(1, 2, 1);
            int l2 = IguanaGmsh.Model.GeoOCC.AddLine(2, 3, 2);
            int l3 = IguanaGmsh.Model.GeoOCC.AddLine(3, 4, 3);
            int l4 = IguanaGmsh.Model.GeoOCC.AddLine(4, 1, 4);
            int wireTag = IguanaGmsh.Model.GeoOCC.AddCurveLoop(new[] { l1, l2, l3, l4 });
            int surfaceTag = IguanaGmsh.Model.GeoOCC.AddPlaneSurface(new[] { wireTag });
            IguanaGmsh.Model.GeoOCC.Synchronize();

            IguanaGmsh.Model.Mesh.SetTransfiniteCurve(l1, u);
            IguanaGmsh.Model.Mesh.SetTransfiniteCurve(l2, v);
            IguanaGmsh.Model.Mesh.SetTransfiniteCurve(l3, u);
            IguanaGmsh.Model.Mesh.SetTransfiniteCurve(l4, v);

            Point3d p5 = box.PointAt(0, 0, 1);
            Tuple<int, int>[] ov;
            Tuple<int, int>[] temp = new Tuple<int, int>[] { Tuple.Create(2, surfaceTag) };
            IguanaGmsh.Model.GeoOCC.Extrude(temp, 0,0, p5.Z*2, out ov, new[] { w }, new[] { p5.Z }, true);     

            IguanaGmsh.Model.GeoOCC.Synchronize();

            IguanaGmsh.Model.Mesh.Generate(3);
            mesh = IguanaGmshFactory.TryGetIMesh(3);

            IguanaGmsh.FinalizeGmsh();

            DA.SetData(0, mesh);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iCube3d;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9bff3195-49d8-421a-88e4-d9133a62fc59"); }
        }
    }
}