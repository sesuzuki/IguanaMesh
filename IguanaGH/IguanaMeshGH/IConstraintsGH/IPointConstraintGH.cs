using System;
using System.Collections.Generic;
using System.Linq;
using GH_IO.Types;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IConstraintsGH
{
    public class IPointConstraintGH : GH_Component
    {
        int entityDim = 2, entityTag = -1;
        double size = 1.0;

        /// <summary>
        /// Initializes a new instance of the IConstraintCollectorGH class.
        /// </summary>
        public IPointConstraintGH()
          : base("iPointConstraint", "iPtC",
              "Point constraint for mesh generation.",
              "Iguana", "Constraints")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("EntityDimension","eDim", "Dimension (2 or 3) of the entity to embed the constraint. In most of the cases the entity is automatically detected but must be explicitly set for breps. Default is " + entityDim, GH_ParamAccess.item, entityDim);
            pManager.AddIntegerParameter("EntityID","ID", "eID of the entity entity to embed the constraint. In most of the cases the entity is automatically detected but must be explicitly set for breps. Default is " + entityTag, GH_ParamAccess.item, entityTag);
            pManager.AddPointParameter("Point", "Pt", "Point to use as a geometric constraint.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Size", "S", "Target global mesh element size at the constraint point. Default value is " + size, GH_ParamAccess.item, size);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iConstraint", "iConstraint", "Iguana constraint for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d pt = new Point3d();
            DA.GetData(0, ref entityDim);
            DA.GetData(1, ref entityTag);
            DA.GetData(2, ref pt);
            DA.GetData(3, ref size);

            IguanaGmshConstraint c = new IguanaGmshConstraint(0, pt, size, entityDim, entityTag);

            DA.SetData(0, c);

            /*
            List<IguanaGmshConstraint> constraints = new List<IguanaGmshConstraint>();

            List<Point3d> pts = new List<Point3d>();
            foreach (var obj in base.Params.Input[0].VolatileData.AllData(true))
            {
                Point3d p;
                obj.CastTo<Point3d>(out p);
                if(p!=null) pts.Add(p);
            }

            sizes = new List<double>();
            foreach (var obj in base.Params.Input[1].VolatileData.AllData(true))
            {
                double s;
                obj.CastTo<double>(out s);
                sizes.Add(s);
            }


            int count = pts.Count;
            int auxcount = sizes.Count;

            if (count > 0)
            {
                Point3d pt;
                double s;
                if (count == auxcount)
                {
                    for (int i = 0; i < count; i++)
                    {
                        pt = pts[i];
                        s = sizes[i];
                        IguanaGmshConstraint c = new IguanaGmshConstraint(0, pt, s, entityDim, entityTag);
                        constraints.Add(c);
                    }
                }
                else
                {
                    s = sizes[0];
                    for (int i = 0; i < count; i++)
                    {
                        pt = pts[i];
                        IguanaGmshConstraint c = new IguanaGmshConstraint(0, pt, s, entityDim, entityTag);
                        constraints.Add(c);
                    }
                }
            }

            DA.SetDataList(0, constraints);*/
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iPointConstraints;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6b8276c2-fd3e-4a2a-939c-3644b8dfb953"); }
        }
    }
}