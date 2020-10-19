using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ISettingsGH
{
    public class IAutomaticFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IAutomaticField class.
        /// </summary>
        public IAutomaticFieldGH()
          : base("iAutomaticField", "iAutoF",
              "Automatic mesh size field to specify the size of the mesh elements.",
              "Iguana", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("NRefine", "N", "Initial refinement level for the octree. Default value is 5.", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Maximum Gradient", "MaxGrad", "Maximun gradient of the size field. Default value is 1.4.", GH_ParamAccess.item, 1.4);
            pManager.AddNumberParameter("Bulk", "B", "Size everywhere no size is prescribed. Default value is 0.5.", GH_ParamAccess.item, 0.5);
            pManager.AddIntegerParameter("PointsPerCircle", "C", "Number of points per circle (adapt to curvature of surfaces). Default value is 55.", GH_ParamAccess.item, 55);
            pManager.AddIntegerParameter("PointsPerGap", "G", "Number of points in thin layers. Default value is 5.", GH_ParamAccess.item, 5);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshField", "iMF", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int NRefine = 5;
            double gradientMax = 1.4;
            double hBulk = 0.5;
            int nPointsPerCircle = 55;
            int nPointsPerGap = 5;
            DA.GetData(0, ref NRefine);
            DA.GetData(1, ref gradientMax);
            DA.GetData(2, ref hBulk);
            DA.GetData(3, ref nPointsPerCircle);
            DA.GetData(4, ref nPointsPerGap);

            IguanaGmshField.AutomaticMeshSizeField field = new IguanaGmshField.AutomaticMeshSizeField();
            field.NRefine = NRefine;
            field.gradientMax = gradientMax;
            field.hBulk = hBulk;
            field.nPointsPerCircle = nPointsPerCircle;
            field.nPointsPerGap = nPointsPerGap;

            DA.SetData(0, field);
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
            get { return new Guid("19ecb98f-05f7-4be8-b1cf-ebe484a3dc1b"); }
        }
    }
}