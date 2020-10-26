using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH
{
    public class ISurfaceConstraint : GH_Component
    {
        int entityDim = 2, entityTag = -1;
        double size = 1.0;

        /// <summary>
        /// Initializes a new instance of the ISurfaceConstraint class.
        /// </summary>
        public ISurfaceConstraint()
          : base("iSurfaceConstraint", "iSrfC",
              "Surface constraint for mesh generation.",
              "Iguana", "Constraints")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("EntityDimension", "eDim", "Dimension (2 or 3) of the entity to embed the constraint. In most of the cases the entity is automatically detected but must be explicitly set for breps. Default is " + entityDim, GH_ParamAccess.item, entityDim);
            pManager.AddIntegerParameter("EntityID", "ID", "eID of the entity entity to embed the constraint. In most of the cases the entity is automatically detected but must be explicitly set for breps. Default is " + entityTag, GH_ParamAccess.item, entityTag);
            pManager.AddBrepParameter("Surface", "Srf", "Surface to use as a geometric constraint.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Size", "S", "Target global mesh element size at the constraint surface. Default value is " + size, GH_ParamAccess.item, size);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iConstraints", "iConstraint", "Iguana constraint for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep b = null;
            DA.GetData(0, ref entityDim);
            DA.GetData(1, ref entityTag);
            DA.GetData(2, ref b);
            DA.GetData(3, ref size);

            IguanaGmshConstraint c = new IguanaGmshConstraint(3, b, size, entityDim, entityTag);

            DA.SetData(0, c);
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
            get { return new Guid("48059746-fc06-4964-b976-6e348118daa1"); }
        }
    }
}