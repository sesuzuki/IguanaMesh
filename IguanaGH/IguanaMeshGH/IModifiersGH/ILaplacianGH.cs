using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IModifiers;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IModifiersGH
{
    public class ILaplacianGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ILaplacianGH class.
        /// </summary>
        public ILaplacianGH()
          : base("iLaplacianSmooth", "iLaplacianSmooth",
              "Apply Laplacian smoothing.",
              "Iguana", "Modifiers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana Mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Steps", "Steps", "Subdivision steps.", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Naked", "Naked", "Smooth naked vertices.", GH_ParamAccess.item, true);
            pManager.AddIntegerParameter("Vertices", "v-Keys", "Vertices to exclude.", GH_ParamAccess.list);
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
            IMesh old = new IMesh();
            int step = 1;
            bool naked = true;
            List<int> exclude = new List<int>();
            DA.GetData(0, ref old);
            DA.GetData(1, ref step);
            DA.GetData(2, ref naked);
            DA.GetData(3, ref exclude);

            IMesh mesh = IModifier.LaplacianSmoother(old, step, naked, exclude);

            DA.SetData(0, mesh);
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
            get { return new Guid("e4a63414-5a78-4300-85f1-619f69445f6a"); }
        }
    }
}