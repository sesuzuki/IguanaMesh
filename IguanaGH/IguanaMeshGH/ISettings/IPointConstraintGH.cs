using System;
using System.Collections.Generic;
using System.Linq;
using GH_IO.Types;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IConstraints;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ISettings
{
    public class IPointConstraintGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IConstraintCollectorGH class.
        /// </summary>
        public IPointConstraintGH()
          : base("iPointConstraint", "iPtC",
              "Point constraint for mesh generation.",
              "Iguana", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point Constraints", "P", "List of point constraints.", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Contraint Size", "S1", "Target global mesh element size at constraint points. If the number of size values is not equal to the number of points, the first item of the list is assigned to all points. Default value is 1.0.", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("IConstraints", "IConstraints", "Iguana constraint collector for mesh generation.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {           
            List<IguanaGmshConstraint> constraints = new List<IguanaGmshConstraint>();

            List<Point3d> pts = new List<Point3d>();
            foreach (var obj in base.Params.Input[0].VolatileData.AllData(true))
            {
                Point3d p;
                obj.CastTo<Point3d>(out p);
                pts.Add(p);
            }

            List<double> sizes = new List<double>();
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
                        IguanaGmshConstraint c = new IguanaGmshConstraint(0, pt, s);
                        constraints.Add(c);
                    }
                }
                else
                {
                    s = sizes[0];
                    for (int i = 0; i < count; i++)
                    {
                        pt = pts[i];
                        IguanaGmshConstraint c = new IguanaGmshConstraint(0, pt, s);
                        constraints.Add(c);
                    }
                }
            }

            DA.SetDataList(0, constraints);
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
            get { return new Guid("6b8276c2-fd3e-4a2a-939c-3644b8dfb953"); }
        }
    }
}