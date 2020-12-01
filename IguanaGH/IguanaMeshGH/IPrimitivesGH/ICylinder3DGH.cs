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
    public class ICylinder3DGH : GH_Component
    {
        Plane plane = Plane.WorldXY;
        double r = 1, ang = 360;
        /// <summary>
        /// Initializes a new instance of the ICylinder3DGH class.
        /// </summary>
        public ICylinder3DGH()
          : base("iCylinder3D", "iCylinder3D",
              "Construct a cylinder volume mesh",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Base plane", "Pl", "Base plane.", GH_ParamAccess.item, plane);
            pManager.AddNumberParameter("Radius", "R", "Radius.", GH_ParamAccess.item, r);
            pManager.AddNumberParameter("Angle", "Angle", "Defines the angular opening in degrees (from 0 to 360).", GH_ParamAccess.item, ang);
            pManager.AddGenericParameter("iSettings3D", "iSettings", "Three-dimensional meshing settings.", GH_ParamAccess.item);
            pManager[3].Optional = true;
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

            DA.GetData(0, ref plane);
            DA.GetData(1, ref r);
            DA.GetData(2, ref ang);
            DA.GetData(3, ref solverOptions);

            IguanaGmsh.Initialize();

            Vector3d n = plane.Normal;

            int tag = IguanaGmsh.Model.GeoOCC.AddCylinder(plane.OriginX, plane.OriginY, plane.OriginZ,n.X, n.Y, n.Z, r, ang * Math.PI / 180);
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
                return Properties.Resources.iCylinder3d;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("193b3a43-f9b0-4396-93f8-899eba962a40"); }
        }
    }
}