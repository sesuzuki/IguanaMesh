using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class ICylinderFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ICylinderGH class.
        /// </summary>
        public ICylinderFieldGH()
          : base("iCylinderField", "iCylF",
              "Cylinder field to specify the size of the mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Cylinder", "C", "Base cylinder.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness", "T", "Thickness of a transition layer outside the ball. Default value is 1.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Size Inside", "SI", "Element sizes inside the sphere. Default value is 1.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Size Outside", "SE", "Element sizes outside the sphere. Default value is 1.", GH_ParamAccess.item, 1);
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
            Surface srf = null;
            double Thickness = 1;
            double VIn = 1;
            double VOut = 1;

            DA.GetData(0, ref srf);
            DA.GetData(1, ref Thickness);
            DA.GetData(2, ref VIn);
            DA.GetData(3, ref VOut);

            if (srf.IsCylinder())
            {
                Cylinder c;
                srf.TryGetCylinder(out c);
                IguanaGmshField.Cylinder field = new IguanaGmshField.Cylinder();
                field.Radius = c.Radius;
                field.VIn = VIn;
                field.VOut = VOut;
                field.XCenter = c.Center.X;
                field.YCenter = c.Center.Y;
                field.ZCenter = c.Center.Z;
                field.XAxis = c.Axis.X;
                field.YAxis = c.Axis.Y;
                field.ZAxis = c.Axis.Z;

                DA.SetData(0, field);
            }
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
            get { return new Guid("055c86da-6d86-49bc-9d18-c35425774670"); }
        }
    }
}