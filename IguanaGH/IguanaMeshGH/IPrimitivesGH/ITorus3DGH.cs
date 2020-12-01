using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IPrimitivesGH
{
    public class ITorus3DGH : GH_Component
    {
        Point3d pt = new Point3d();
        double r1 = 1, r2 = 0.5, ang = 360;

        /// <summary>
        /// Initializes a new instance of the ITorus3DGH class.
        /// </summary>
        public ITorus3DGH()
          : base("iTorus3D", "iTorus3D",
              "Construct a torus volume mesh",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Center Point", "Pt", "Center Point.", GH_ParamAccess.item, pt);
            pManager.AddNumberParameter("Radius1", "R1", "Parameter for the semi major axis.", GH_ParamAccess.item, r1);
            pManager.AddNumberParameter("Radius2", "R2", "Parameter for the semi minor axis.", GH_ParamAccess.item, r2);
            pManager.AddNumberParameter("Angle", "Angle", "Defines the angular opening in degrees (from 0 to 360).", GH_ParamAccess.item, ang);
            pManager.AddGenericParameter("iSettings3D", "iSettings", "Three-dimensional meshing settings.", GH_ParamAccess.item);
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
            IguanaGmshSolver3D solverOptions = new IguanaGmshSolver3D();

            DA.GetData(0, ref pt);
            DA.GetData(1, ref r1);
            DA.GetData(2, ref r2);
            DA.GetData(3, ref ang);
            DA.GetData(4, ref solverOptions);

            IguanaGmsh.Initialize();

            int tag = IguanaGmsh.Model.GeoOCC.AddTorus(pt.X,pt.Y,pt.Z, r1, r2, ang * Math.PI / 180);
            IguanaGmsh.Model.GeoOCC.Synchronize();

            solverOptions.ApplySolverSettings();

            IguanaGmsh.Model.Mesh.Generate(3);
            IMesh mesh = IguanaGmshFactory.TryGetIMesh(3);

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
                return Properties.Resources.iTorus3d;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9617528e-1e99-47b0-a6a8-57b0f0e486aa"); }
        }
    }
}