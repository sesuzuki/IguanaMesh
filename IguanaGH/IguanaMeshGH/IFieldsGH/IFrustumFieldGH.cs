using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IFrustumFieldGH : GH_Component
    {
        double R1_inner = 0, R1_outer = 1, R2_inner = 0, R2_outer = 1, 
            V1_inner = 0.1, V1_outer = 1, V2_inner = 0.1, V2_outer = 1;
        Point3d p1 = new Point3d(), p2 = new Point3d(0,0,1);

        /// <summary>
        /// Initializes a new instance of the IFrustumField class.
        /// </summary>
        public IFrustumFieldGH()
          : base("iFrustumField", "iFrustF", "Frustum field to specify the size of the mesh elements. This field is an extended cylinder with inner(i) and outer(o) radiuses on both endpoints(1 and 2).\nLength scale is bilinearly interpolated betweenthese locations (inner and outer radiuses, endpoints 1 and 2).\nThe field values for a point P are given by : u = P1P.P1P2/||P1P2|| r = || P1P - u* P1P2 || Ri = (1-u)*R1i + u* R2i Ro = (1-u)*R1o + u* R2o v = (r-Ri)/(Ro-Ri) lc = (1-v)*((1-u)*v1i + u* v2i ) + v* ((1-u)*v1o + u* v2o ) where(u, v) in [0, 1]",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point1", "P1", "Endpoint 1.", GH_ParamAccess.item, p1);
            pManager.AddPointParameter("Point2", "P2", "Endpoint 2.", GH_ParamAccess.item, p2);
            pManager.AddNumberParameter("InnerRadius1", "R1In", "Inner radius of Frustum at endpoint 1. Default is " + R1_inner, GH_ParamAccess.item, R1_inner);
            pManager.AddNumberParameter("OuterRadius1", "R1Out", "Outer radius of Frustum at endpoint 1. Default is " + R1_outer, GH_ParamAccess.item, R1_outer);
            pManager.AddNumberParameter("InnerRadius2", "R2In", "Inner radius of Frustum at endpoint 2. Default is " + R2_inner, GH_ParamAccess.item, R2_inner);
            pManager.AddNumberParameter("OuterRadius2", "R2Out", "Outer radius of Frustum at endpoint 2. Default is " + R2_outer, GH_ParamAccess.item, R2_outer);
            pManager.AddNumberParameter("InnerSize1", "S1In", "Element size at point 1, inner radius. Default is " + V1_inner, GH_ParamAccess.item, V1_inner);
            pManager.AddNumberParameter("OuterSize1", "S1Out", "Element size at point 1, outer radius. Default is " + V1_outer, GH_ParamAccess.item, V1_outer);
            pManager.AddNumberParameter("InnerSize2", "S2In", "Element size at point 2, inner radius. Default is " + V2_inner, GH_ParamAccess.item, V2_inner);
            pManager.AddNumberParameter("OuterSize2", "S2Out", "Element size at point 2, outer radius. Default is " + V2_outer, GH_ParamAccess.item, V2_outer);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshField", "iF", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData(0, ref p1);
            DA.GetData(1, ref p2);
            DA.GetData(2, ref R1_inner);
            DA.GetData(3, ref R1_outer);
            DA.GetData(4, ref R2_inner);
            DA.GetData(5, ref R2_outer);
            DA.GetData(6, ref V1_inner);
            DA.GetData(7, ref V1_outer);
            DA.GetData(8, ref V2_inner);
            DA.GetData(9, ref V2_outer);

            IguanaGmshField.Frustum field = new IguanaGmshField.Frustum();
            field.R1_inner = R1_inner;
            field.R1_outer = R1_outer;
            field.R2_inner = R2_inner;
            field.R2_outer = R2_outer;
            field.V1_inner = V1_inner;
            field.V1_outer = V1_outer;
            field.V2_inner = V2_inner;
            field.V2_outer = V2_outer;
            field.X1 = p1.X;
            field.Y1 = p1.Y;
            field.Z1 = p1.Z;
            field.X2 = p2.X;
            field.Y2 = p2.Y;
            field.Z2 = p2.Z;

            DA.SetData(0, field);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
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
            get { return new Guid("6f375420-8c88-40f3-ae43-ca43c2fefcc8"); }
        }
    }
}