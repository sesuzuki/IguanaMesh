/*
 * <IguanaMesh>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaMeshGH.IFields
{
    public class IFrustumFieldGH : GH_Component
    {
        double R1_inner = 0, R1_outer = 1, R2_inner = 0, R2_outer = 1, 
            V1_inner = 0.1, V1_outer = 1, V2_inner = 0.1, V2_outer = 1;

        /// <summary>
        /// Initializes a new instance of the IFrustumField class.
        /// </summary>
        public IFrustumFieldGH()
          : base("iFrustumField", "iFrustumField", "Compute a frustum field to specify the size of mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("Line", "Ln", "Base projection axis.", GH_ParamAccess.item);
            pManager.AddNumberParameter("InnerRadius1", "RadiusIn1", "Inner radius of Frustum at start. Default is " + R1_inner, GH_ParamAccess.item, R1_inner);
            pManager.AddNumberParameter("OuterRadius1", "RadiusOut1", "Outer radius of Frustum at start. Default is " + R1_outer, GH_ParamAccess.item, R1_outer);
            pManager.AddNumberParameter("InnerRadius2", "RadiusIn2", "Inner radius of Frustum at end. Default is " + R2_inner, GH_ParamAccess.item, R2_inner);
            pManager.AddNumberParameter("OuterRadius2", "RadiusOut2", "Outer radius of Frustum at end. Default is " + R2_outer, GH_ParamAccess.item, R2_outer);
            pManager.AddNumberParameter("InnerSize1", "SizeIn1", "Element size at start inner radius. Default is " + V1_inner, GH_ParamAccess.item, V1_inner);
            pManager.AddNumberParameter("OuterSize1", "SizeOut1", "Element size at start outer radius. Default is " + V1_outer, GH_ParamAccess.item, V1_outer);
            pManager.AddNumberParameter("InnerSize2", "SizeIn2", "Element size at end inner radius. Default is " + V2_inner, GH_ParamAccess.item, V2_inner);
            pManager.AddNumberParameter("OuterSize2", "SizeOut2", "Element size at end outer radius. Default is " + V2_outer, GH_ParamAccess.item, V2_outer);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshField", "iF", "Field for mesh generation.", GH_ParamAccess.item);
            pManager.AddCircleParameter("Reference", "Geom", "Reference geometry representing the frustum.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Line ln = new Line();
            DA.GetData(0, ref ln);
            DA.GetData(1, ref R1_inner);
            DA.GetData(2, ref R1_outer);
            DA.GetData(3, ref R2_inner);
            DA.GetData(4, ref R2_outer);
            DA.GetData(5, ref V1_inner);
            DA.GetData(6, ref V1_outer);
            DA.GetData(7, ref V2_inner);
            DA.GetData(8, ref V2_outer);

            IField.Frustum field = new IField.Frustum();
            field.R1_inner = R1_inner;
            field.R1_outer = R1_outer;
            field.R2_inner = R2_inner;
            field.R2_outer = R2_outer;
            field.V1_inner = V1_inner;
            field.V1_outer = V1_outer;
            field.V2_inner = V2_inner;
            field.V2_outer = V2_outer;
            field.X1 = ln.FromX;
            field.Y1 = ln.FromY;
            field.Z1 = ln.FromZ;
            field.X2 = ln.ToX;
            field.Y2 = ln.ToY;
            field.Z2 = ln.ToZ;

            Vector3d norm = ln.To - ln.From;
            Plane pl1 = new Plane(ln.From, norm);
            norm.Reverse();
            Plane pl2 = new Plane(ln.To, norm);

            Circle[] geom = new Circle[4];
            geom[0] = new Circle(pl1, R1_inner);
            geom[1] = new Circle(pl1, R1_outer);
            geom[2] = new Circle(pl2, R2_inner);
            geom[3] = new Circle(pl2, R2_outer);

            DA.SetData(0, field);
            DA.SetDataList(1, geom);
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
                return Properties.Resources.iFrustumField;
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