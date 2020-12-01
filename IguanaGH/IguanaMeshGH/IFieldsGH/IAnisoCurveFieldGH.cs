using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;
using static Iguana.IguanaMesh.IWrappers.IExtensions.IguanaGmshField;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IAnisoCurveFieldGH : GH_Component
    {
        int nnodesByEdge = 1;
        double dMax = 1, dMin = 0.5, 
            maxNormal = 1, maxTangent = 1,
            minNormal = 0.5, minTangent = 0.5;

        /// <summary>
        /// Initializes a new instance of the IAnisoCurveFieldGH class.
        /// </summary>
        public IAnisoCurveFieldGH()
          : base("iAnisoCurveField", "iAnisoF",
              "Attractor anisotropic curve field to specify the size of the mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Edges", "E", "Indexes of the curves in the geometric model.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Nodes", "N", "Number of nodes used to discretized a curve. Default value is " + nnodesByEdge, GH_ParamAccess.item, nnodesByEdge);
            pManager.AddNumberParameter("Maximum Distance", "MaxDist", "Maxmium distance, above this distance from the curves, prescribe the maximum mesh sizes. Default value is " + dMax, GH_ParamAccess.item, dMax);
            pManager.AddNumberParameter("Minimum Distance", "MinDist", "Minimum distance, below this distance from the curves, prescribe the minimum mesh sizes. Default value is " + dMin, GH_ParamAccess.item, dMin);
            pManager.AddNumberParameter("Maximum Normal", "MaxNorm", "Maximum mesh size in the direction normal to the closest curve. Default value is " + maxNormal, GH_ParamAccess.item, maxNormal);
            pManager.AddNumberParameter("Minimum Normal", "MinNorm", "Minimum mesh size in the direction normal to the closest curve. Default value is " + minNormal, GH_ParamAccess.item, minNormal);
            pManager.AddNumberParameter("Maximum Tangent", "MaxTang", "Maximum mesh size in the direction tangeant to the closest curve. Default value is " + maxTangent, GH_ParamAccess.item, maxTangent);
            pManager.AddNumberParameter("Minimum Tangent", "MinTang", "Minimum mesh size in the direction tangeant to the closest curve. Default value is " + minTangent, GH_ParamAccess.item, minTangent);
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
            List<int> edges = new List<int>();
            DA.GetDataList(0, edges);
            DA.GetData(1, ref nnodesByEdge);
            DA.GetData(2, ref dMax);
            DA.GetData(3, ref dMin);
            DA.GetData(4, ref maxNormal);
            DA.GetData(5, ref minNormal);
            DA.GetData(6, ref maxTangent);
            DA.GetData(7, ref minTangent);

            double[] eList = new double[edges.Count];
            for (int i = 0; i < edges.Count; i++) eList[i] = edges[i];

            AttractorAnisoCurve field = new AttractorAnisoCurve();
            field.EdgesList = eList;
            field.NNodesByEdge = nnodesByEdge;
            field.dMax = dMax;
            field.dMin = dMin;
            field.lMaxNormal = maxNormal;
            field.lMaxTangent = maxTangent;
            field.lMinNormal = minNormal;
            field.lMinTangent = minTangent;

            DA.SetData(0, field);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iAnisoCrvField;
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