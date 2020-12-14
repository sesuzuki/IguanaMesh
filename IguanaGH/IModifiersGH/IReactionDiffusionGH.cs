using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IModifiers;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IModifiersGH
{
    public class IReactionDiffusionGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IReactionDiffusionGH class.
        /// </summary>
        public IReactionDiffusionGH()
          : base("iReactionDiffusion Algorithm", "iReactionDiffusion",
              "Apply a reaction-diffusion algorithm. See: http://karlsims.com/rd.html",
              "Iguana", "Modifiers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "The Iguana mesh to apply the reaction-diffusion algorithm.", GH_ParamAccess.item);
            pManager.AddNumberParameter("DiffusionRateA", "dA", "Diffusion rate of chemical A.", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("DiffusionRateA", "dB", "Diffusion rate of chemical B.", GH_ParamAccess.item, 0.3);
            pManager.AddNumberParameter("KillFactor", "k", "Kill rate.", GH_ParamAccess.item, 0.062);
            pManager.AddNumberParameter("FeedFactor", "f", "Feed rate.", GH_ParamAccess.item, 0.055);
            pManager.AddNumberParameter("NeighborContributionFactor", "nF", "Neighbor contribution factor.", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("TimeStep", "dt", "Time step.", GH_ParamAccess.item, 1.0);
            pManager.AddIntegerParameter("Iterations", "iter", "Maximum number of iterations.", GH_ParamAccess.item, 100);
            pManager.AddIntegerParameter("RandomPopulation", "rP", "Sze of the initia random population.", GH_ParamAccess.item, 10);
            pManager.AddVectorParameter("VectorField", "V", "Auxiliar vector field.", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;
            pManager[9].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "The modified Iguana Mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            double dA = 1.0, dB = 0.3, dt = 1.0, k = 0.062, f = 0.055, nF = 1.0;
            int rP = 10, iter = 100; ;
            List<Vector3d> vectorField = new List<Vector3d>();

            DA.GetData(0, ref mesh);
            DA.GetData(1, ref dA);
            DA.GetData(2, ref dB);
            DA.GetData(3, ref k);
            DA.GetData(4, ref f);
            DA.GetData(5, ref nF);
            DA.GetData(6, ref dt);
            DA.GetData(7, ref iter);
            DA.GetData(8, ref rP);
            DA.GetDataList(9, vectorField);

            IReactionDiffusion modifier = new IReactionDiffusion(dA, dB, k, f, dt, nF, iter, rP);
            modifier.AuxiliarVectorField = vectorField.ToArray();

            IMesh nM = modifier.ApplyModifier(mesh);

            DA.SetData(0, nM);
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
            get { return new Guid("10418a85-3ff7-4627-bd22-ee3ef4312bc1"); }
        }
    }
}