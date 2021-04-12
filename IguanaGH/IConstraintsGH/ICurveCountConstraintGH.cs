using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaMeshGH.IConstraintsGH
{
    public class ICurveCountConstraintGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ICurveCountConstraintGH class.
        /// </summary>
        public ICurveCountConstraintGH()
          : base("iCurveCountConstraint", "iCurveCountConstraint",
              "Embed a curve divided by count to constraint mesh generation.",
              "Iguana", "Constraints")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Curve to use as a geometric constraint.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Size", "Size", "Target global mesh element size at the constraint curve.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Count", "Count", "Number of sampling nodes.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iConstraint", "iConstraint", "Constraint for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crv = null;
            double size = 0;
            int count = 0;
            DA.GetData(0, ref crv);
            DA.GetData(1, ref size);
            DA.GetData(2, ref count);

            IConstraint constraints = new IConstraint(2, crv, size, -1, -1, count);

            DA.SetData(0, constraints);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iCurveCountConstraints;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("22f69df2-a8eb-4113-b52d-252ce6534eeb"); }
        }
    }
}