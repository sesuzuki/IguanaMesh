using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Rhino;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITopologyGH
{
    public class IFindElementGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IGetElementGH class.
        /// </summary>
        public IFindElementGH()
          : base("iFindElement", "iFindElement",
              "Find the element associated with the given key",
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element", "e-Key", "Element Key.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iElement", "iElement", "Iguana element.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            int eKey = -1;
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref eKey);

            IElement e = mesh.GetElementWithKey(eKey);
            e.BuildRhinoGeometry(mesh);

            DA.SetData(0, e);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iElementFind;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9cea2c3d-5838-4114-bd91-89cc239a8382"); }
        }
    }
}