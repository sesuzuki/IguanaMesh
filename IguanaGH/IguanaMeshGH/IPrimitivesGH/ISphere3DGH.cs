using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IPrimitivesGH
{
    public class ISphere3DGH : GH_Component
    {
        Point3d p = new Point3d();
        double r = 1;
        /// <summary>
        /// Initializes a new instance of the ISphere3DGH class.
        /// </summary>
        public ISphere3DGH()
          : base("iSphere3D", "iSphere3D",
              "Construct a sphere volume mesh",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Pt", "Base center point of the sphere", GH_ParamAccess.item, p);
            pManager.AddNumberParameter("Radius","Radius","Radius of the sphere",GH_ParamAccess.item, r);
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

            DA.GetData(0, ref p);
            DA.GetData(1, ref r);

            IguanaGmsh.Initialize();

            int sTag = IguanaGmsh.Model.GeoOCC.AddSphere(p.X, p.Y,p.Z,r);
            IguanaGmsh.Model.Mesh.SetRecombine(2, sTag);

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
                return Properties.Resources.iSphere3d;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("de7bfdef-0962-4b0d-a045-771574628564"); }
        }
    }
}