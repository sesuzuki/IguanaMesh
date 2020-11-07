using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITransformGH
{
    public class IMoveElementGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMoveElementGH class.
        /// </summary>
        public IMoveElementGH()
          : base("iMoveElement", "iMoveElement",
              "Move element",
              "Iguana", "Transform")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element", "e-Key", "Vertex key.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Vector", "T", "Translation vector.", GH_ParamAccess.item);
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
            Vector3d vec = new Vector3d();
            int eKey = 0;

            DA.GetData(0, ref mesh);
            DA.GetData(1, ref eKey);
            DA.GetData(2, ref vec);

            IMesh dM = mesh.DeepCopy();

            ITopologicVertex v;
            IElement e = dM.Elements.GetElementWithKey(eKey);
            foreach(int vK in e.Vertices)
            {
                v = dM.Vertices.GetVertexWithKey(vK);
                dM.Vertices.SetVertexPosition(vK, v.Position + new IVector3D(vec.X, vec.Y, vec.Z));
            }            

            DA.SetData(0, dM);
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
            get { return new Guid("c446ce1e-026b-4e75-a8a8-69b3b051d419"); }
        }
    }
}