using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ICreators;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IPrimitivesGH
{
    public class ICube2DGH : GH_Component
    {
        private int u = 10, v = 10, w = 10;
        private Boolean weld = false;
        private double tolerance = 0.01;
        private Box box;

        /// <summary>
        /// Initializes a new instance of the ICubeGH class.
        /// </summary>
        public ICube2DGH()
          : base("iCube2D", "iCube2D",
              "Construct a cube surface mesh.",
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
            pManager.AddGenericParameter("iMesh", "iM", "Iguana surface mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            box = new Box();

            DA.GetData(0, ref box);
            DA.GetData(1, ref u);
            DA.GetData(2, ref v);
            DA.GetData(3, ref w);
            DA.GetData(4, ref weld);
            DA.GetData(5, ref tolerance);

            //Build AHF-DataStructure
            mesh = IMeshCreator.CreateCube(box.ToBrep(), u, v, w, weld, tolerance);

            DA.SetData(0, mesh);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iCube2D;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("cdc195bc-2a8d-41a2-9b32-775f14ae1f01"); }
        }
    }
}