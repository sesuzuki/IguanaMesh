using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IThresholdFieldGH : GH_Component
    {
        double distMax = 10, distMin = 1, lcMax = 1, lcMin = 0.1;
        bool sigmoid = false, stopAtDistMax = false;

        /// <summary>
        /// Initializes a new instance of the IThresholdFieldGH class.
        /// </summary>
        public IThresholdFieldGH()
          : base("iThresholdField", "iThreshF",
              "Threshold field to specify the size of the mesh elements. Expresion to evaluate is:\nF = LCMin if Field[IField] <= DistMin,\nF = LCMax if Field[IField] >= DistMax,\nF = interpolation between LcMin and LcMax if DistMin<Field[IField] < DistMax",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Field", "iF", "Field to evaluate.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "dMax", "Distance from entity after which element size will be LcMax. Default is " + distMax, GH_ParamAccess.item, distMax);
            pManager.AddNumberParameter("Min Distance", "dMin", "Distance from entity up to which element size will be LcMin. Default is " + distMin, GH_ParamAccess.item, distMin);
            pManager.AddNumberParameter("Max Size", "sMax", "Element size outside DistMax. Default is " + lcMax, GH_ParamAccess.item, lcMax);
            pManager.AddNumberParameter("Min Size", "sMin", "Element size inside DistMin. Default is " + lcMin, GH_ParamAccess.item, lcMin);
            pManager.AddBooleanParameter("Sigmoid", "Sig", "True for interpolating between LcMin and LcMax using a sigmoid or false to interpolate linearly. Default is " + sigmoid, GH_ParamAccess.item, sigmoid);
            pManager.AddBooleanParameter("Stop at dMax", "St", "Impose element size outside dMax. Default is " + stopAtDistMax, GH_ParamAccess.item, stopAtDistMax);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshField", "iF", "Field for mesh generation.", GH_ParamAccess.item);

        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IguanaGmshField auxfield = null;
            DA.GetData(0, ref auxfield);
            DA.GetData(1, ref distMax);
            DA.GetData(2, ref distMin);
            DA.GetData(3, ref lcMax);
            DA.GetData(4, ref lcMin);
            DA.GetData(5, ref sigmoid);
            DA.GetData(6, ref stopAtDistMax);

            IguanaGmshField.Threshold field = new IguanaGmshField.Threshold();
            field.IField = auxfield;
            field.DistMax = distMax;
            field.DistMin = distMin;
            field.LcMax = lcMax;
            field.LcMin = lcMin;
            field.Sigmoid = sigmoid;
            field.StopAtDistMax = stopAtDistMax;

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
            get { return new Guid("fe68339b-be68-415a-937c-09e286ae4e23"); }
        }
    }
}