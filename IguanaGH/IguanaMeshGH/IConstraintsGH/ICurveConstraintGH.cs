using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IConstraintsGH
{
    public class ILineConstraintGH : GH_Component
    {
        int entityDim = 2, entityTag = -1;
        double size = 1.0;

        /// <summary>
        /// Initializes a new instance of the ILineConstraintGH class.
        /// </summary>
        public ILineConstraintGH()
          : base("iCurveConstraint", "iCrvC",
              "Curve constraint for mesh generation.",
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
            pManager.AddCurveParameter("Curve", "Crv", "Curve to use as a geometric constraint.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Size", "S", "Target global mesh element size at the constraint curve. Default value is " + size, GH_ParamAccess.item, size);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("IConstraint", "IConstraint", "Iguana constraint collector for mesh generation.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crv = null;
            DA.GetData(0, ref entityDim);
            DA.GetData(1, ref entityTag);
            DA.GetData(2, ref crv);
            DA.GetData(3, ref size);

            IguanaGmshConstraint c;
            Polyline pl;
            bool flag = crv.TryGetPolyline(out pl);
            if (flag) c = new IguanaGmshConstraint(1, pl, size, entityDim, entityTag);
            else c = new IguanaGmshConstraint(2, crv, size, entityDim, entityTag);

            DA.SetData(0, c);

            /*List<IguanaGmshConstraint> constraints = new List<IguanaGmshConstraint>();

            List<Curve> crv = new List<Curve>();
            foreach (var obj in base.Params.Input[0].VolatileData.AllData(true))
            {
                Curve c;
                obj.CastTo<Curve>(out c);
                crv.Add(c);
            }

            List<double> sizes = new List<double>();
            foreach (var obj in base.Params.Input[1].VolatileData.AllData(true))
            {
                double s;
                obj.CastTo<double>(out s);
                sizes.Add(s);
            }


            int auxcount = sizes.Count;
            int count = crv.Count;

            if (count > 0)
            {
                IguanaGmshConstraint c;
                if (count == auxcount)
                {
                    for (int i = 0; i < count; i++)
                    {
                        Polyline pl;
                        bool flag = crv[i].TryGetPolyline(out pl);
                        if (flag) c = new IguanaGmshConstraint(1, pl, sizes[i]);
                        else
                        {
                            c = new IguanaGmshConstraint(2, crv[i], sizes[i]);
                        }
                        constraints.Add(c);
                    }
                }
                else
                {
                    double s = sizes[0];
                    for (int i = 0; i < count; i++)
                    {
                        Polyline pl;
                        bool flag = crv[i].TryGetPolyline(out pl);
                        if (flag) c = new IguanaGmshConstraint(1, pl, s);
                        else
                        {
                            c = new IguanaGmshConstraint(2, crv[i], s);
                        }
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
            get { return new Guid("9f579201-4df0-41e5-bb3c-30f041e25eac"); }
        }
    }
}