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
    public class IBoxFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IBoxFieldGH class.
        /// </summary>
        public IBoxFieldGH()
          : base("iBoxField", "iBoxField",
              "Compute a box field to specify the size of mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("Box", "B", "Base box.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness", "Thickness", "Thickness of a transition layer outside the Box. Default value is 1.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Size Inside", "SizeIn", "Element sizes inside the Box. Default value is 1.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Size Outside", "SizeOut", "Element sizes outside the Box. Default value is 1.", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iField", "iField", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Box b= new Box();
            double Thickness = 1;
            double VIn = 1;
            double VOut = 1;

            DA.GetData(0, ref b);
            DA.GetData(1, ref Thickness);
            DA.GetData(2, ref VIn);
            DA.GetData(3, ref VOut);

            Point3d[] pts = b.GetCorners();
            Point3d min = pts[0];
            Point3d max = pts[6];

            IField.Box field = new IField.Box();
            field.Thickness= Thickness;
            field.VIn = VIn;
            field.VOut = VOut;
            field.XMax = max.X;
            field.XMin = min.X;
            field.YMax = max.Y;
            field.YMin = min.Y;
            field.ZMax = max.Z;
            field.ZMin = min.Z;

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
                return Properties.Resources.iBoxField;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("85019ee5-c915-4364-8676-cfa54b5b8af4"); }
        }
    }
}