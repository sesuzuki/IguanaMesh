using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class ITetrahedronElementGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ITetrahedronElementGH class.
        /// </summary>
        public ITetrahedronElementGH()
          : base("iTetrahedronElement", "iTetrahedronElement",
              "A three-dimensional tetrahedron element.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("First", "N1", "First vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Second", "N2", "Second vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Third", "N3", "Third vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Fourth", "N4", "Fourth vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Fifth", "N5", "Fifth vertex.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iElement", "iE", "Iguana element", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int N1 = 0, N2 = 0, N3 = 0, N4 = 0, N5 = 0;
            DA.GetData(0, ref N1);
            DA.GetData(1, ref N2);
            DA.GetData(2, ref N3);
            DA.GetData(3, ref N4);
            DA.GetData(4, ref N5);

            ITetrahedronElement e = new ITetrahedronElement(N1, N2, N3, N4, N5);

            DA.SetData(0, e);
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
            get { return new Guid("7f166665-be00-4439-868a-8f16a9e6704c"); }
        }
    }
}