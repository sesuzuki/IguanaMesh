using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;
using static Iguana.IguanaMesh.IWrappers.IExtensions.IguanaGmshField;

namespace IguanaGH.IguanaMeshGH.ISettingsGH
{
    public class IAnisoCurveFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IAnisoCurveFieldGH class.
        /// </summary>
        public IAnisoCurveFieldGH()
          : base("iAnisoCurveField", "iAnisoF",
              "Attractor anisotropic curve field to specify the size of the mesh elements.",
              "Iguana", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Nodes", "N", "Number of nodes used to discretized a curve. Default value is 20.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Maximum Distance", "MaxDist", "Maxmium distance, above this distance from the curves, prescribe the maximum mesh sizes. Default value is 0.5.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Minimum Distance", "MinDist", "Minimum distance, below this distance from the curves, prescribe the minimum mesh sizes. Default value is 0.1.", GH_ParamAccess.item, 0.5);
            pManager.AddNumberParameter("Maximum Normal", "MaxNormal", "Maximum mesh size in the direction normal to the closest curve. Default value is 0.5.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Minimum Normal", "lMinNormal", "Minimum mesh size in the direction normal to the closest curve. Default value is 0.05.", GH_ParamAccess.item, 0.5);
            pManager.AddNumberParameter("Maximum Tangent", "MaxTangent", "Maximum mesh size in the direction tangeant to the closest curve. Default value is 0.5.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Minimum Tangent", "MinTangent", "Minimum mesh size in the direction tangeant to the closest curve. Default value is 0.05.", GH_ParamAccess.item, 0.5);
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
            int nnodesByEdge = 1;
            double dMax = 1;
            double dMin = 0.5;
            double lMaxNormal = 1;
            double lMaxTangent = 1;
            double lMinNormal = 0.5;
            double lMinTangent = 0.5;

            DA.GetData(0, ref nnodesByEdge);
            DA.GetData(1, ref dMax);
            DA.GetData(2, ref dMin);
            DA.GetData(3, ref lMaxNormal);
            DA.GetData(4, ref lMinNormal);
            DA.GetData(5, ref lMaxTangent);
            DA.GetData(6, ref lMinTangent);

            AttractorAnisoCurve field = new AttractorAnisoCurve();
            field.EdgesList = new double[0];
            field.NNodesByEdge = nnodesByEdge;
            field.dMax = dMax;
            field.dMin = dMin;
            field.lMaxNormal = lMaxNormal;
            field.lMaxTangent = lMaxTangent;
            field.lMinNormal = lMinNormal;
            field.lMinTangent = lMinTangent;

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
            get { return new Guid("7c5f846f-05d7-417b-bed6-42d5b4a5d6df"); }
        }
    }
}