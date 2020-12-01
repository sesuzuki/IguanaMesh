using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IModifiers;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IModifiersGH
{
    public class ITriangulateGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ITriangulateGH class.
        /// </summary>
        public ITriangulateGH()
          : base("iTriangulate", "iTriangulate",
              "Triangulate surface elements.",
              "Iguana", "Modifiers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana Mesh.", GH_ParamAccess.item);
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
            DA.GetData(0, ref old);

            IMesh mesh = IModifier.Triangulate2DElements(old);

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
                return Properties.Resources.iTriangulate;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("78ae4c59-62a9-435e-b783-846910e7d0b7"); }
        }
    }
}