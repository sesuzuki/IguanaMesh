using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IModifiers;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITransformGH
{
    public class IExtrudeElementGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IExtrudeElementGH class.
        /// </summary>
        public IExtrudeElementGH()
          : base("iExtrudeElement", "iExtrudeElement",
              "Extrude a two-dimensional element in the direction of its normal to build a three-dimensional element.",
              "Iguana", "Transform")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element", "e-Key", "Vertex key.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Length", "Length", "Extrusion length.", GH_ParamAccess.item, 1.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            double length = 1;
            List<int> eKeys = new List<int>();

            DA.GetData(0, ref mesh);
            DA.GetDataList(1, eKeys);
            DA.GetData(2, ref length);

            IMesh dM = IModifier.ExtrudeTwoDimensionalElements(mesh, eKeys, length);

            DA.SetData(0, dM);
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
            get { return new Guid("a8ec26bd-9f32-43e0-9bf6-ec2a1f6005bc"); }
        }
    }
}