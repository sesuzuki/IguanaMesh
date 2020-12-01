using System;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes.IElements;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IHexahedronElementGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IHexadronElementGH class.
        /// </summary>
        public IHexahedronElementGH()
          : base("iHexahedronElement", "iHexahedronElement",
              "A three-dimensional hexahedron element.",
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
            pManager.AddIntegerParameter("Sixth", "N6", "Sixth vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Seventh", "N7", "Seventh vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Eighth", "N8", "Eighth vertex.", GH_ParamAccess.item);
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
            int N1 = 0, N2 = 0, N3 = 0, N4 = 0, N5=0, N6=0, N7=0, N8=8;
            DA.GetData(0, ref N1);
            DA.GetData(1, ref N2);
            DA.GetData(2, ref N3);
            DA.GetData(3, ref N4);
            DA.GetData(4, ref N5);
            DA.GetData(5, ref N6);
            DA.GetData(6, ref N7);
            DA.GetData(7, ref N8);

            IHexahedronElement e = new IHexahedronElement(N1,N2,N3,N4,N5,N6,N7,N8);

            DA.SetData(0, e);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iHexahedron;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("cd98ecbe-a964-4153-8ea7-a4ea5d88d57e"); }
        }
    }
}