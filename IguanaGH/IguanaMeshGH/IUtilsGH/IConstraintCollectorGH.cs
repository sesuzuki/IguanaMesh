using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IGmshWrappers;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IConstraintCollectorGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IConstraintCollectorGH class.
        /// </summary>
        public IConstraintCollectorGH()
          : base("IConstraint Collector", "iContraintCollector",
              "Constraint collector for mesh generation.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point Constraints", "P", "List of point constraints.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Contraint Size", "S1", "Target global mesh element size at constraint points. If the number of size values is not equal to the number of points, the first item of the list is assigned to all points. Default value is 1.0.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve Constraints", "C", "List of curve constraints.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Contraint Size", "S2", "Target global mesh element size at constraint curve. If the number of size values is not equal to the number of curve points, the first item of the list is assigned to all curve points. Default value is 1.0.", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("IConstraints", "IConstraints", "Iguana constraint collector for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> pts = new List<Point3d>();
            List<Curve> crv = new List<Curve>();
            List<double> ptsS = new List<double>(){};
            List<double> crvS = new List<double>(){};

            DA.GetDataList(0, pts);
            DA.GetDataList(1, ptsS);
            DA.GetDataList(2, crv);
            DA.GetDataList(3, crvS);

            IguanaGmshConstraintCollector icollector;

            if (crv.Count > 0)
            {
                List<Polyline> poly = new List<Polyline>();
                Polyline pl;
                Curve c;
                for(int i=0; i<poly.Count; i++)
                {
                    c = crv[i];
                    if (c.IsPolyline())
                    {
                        c.TryGetPolyline(out pl);
                        poly.Add(pl);
                    }
                }                
                icollector = new IguanaGmshConstraintCollector(pts, ptsS);
            }
            else
            {
                icollector = new IguanaGmshConstraintCollector(pts, ptsS);
            }

            DA.SetData(0, icollector);
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