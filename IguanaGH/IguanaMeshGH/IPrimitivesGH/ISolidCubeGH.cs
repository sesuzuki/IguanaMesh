using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ICreators;

namespace IguanaGH.IguanaMeshGH.IPrimitivesGH
{
    public class ISolidCubeGH : GH_Component
    {
        private IMesh mesh;

        private Box box = new Box(Plane.WorldXY, new Interval(-1, 1), new Interval(-1, 1), new Interval(-1, 1));
        private int u = 5, v = 5, w = 5;

        /// <summary>
        /// Initializes a new instance of the ISolidCubeGH class.
        /// </summary>
        public ISolidCubeGH()
          : base("iMesh SolidCube Constructor", "iSolidCube",
              "Construct a solid cube hexahedron-mesh stored via an Array-based Half-Facet (AHF) Mesh Data Structure.",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("Base Box", "B", "Base box to construct the solid cube mesh.", GH_ParamAccess.item, box);
            pManager.AddIntegerParameter("U Count", "U", "Number of faces along the {x} direction.", GH_ParamAccess.item, u);
            pManager.AddIntegerParameter("V Count", "V", "Number of faces along the {y} direction.", GH_ParamAccess.item, v);
            pManager.AddIntegerParameter("W Count", "W", "Number of faces along the {z} direction.", GH_ParamAccess.item, w);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Constructed Array-Based Half-Facet (AHF) Mesh Data Structure.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Volume", "V", "Volume of the cube mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData(0, ref box);
            DA.GetData(1, ref u);
            DA.GetData(2, ref v);
            DA.GetData(3, ref w);

            //Build AHF-DataStructure
            mesh = IMeshCreator.CreateCubeSolid(box, u, v, w);

            DA.SetData(0, mesh);
            DA.SetData(1, box.Volume);
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
            get { return new Guid("ee41bfb8-da38-486b-8599-ea6cf862b1da"); }
        }
    }
}