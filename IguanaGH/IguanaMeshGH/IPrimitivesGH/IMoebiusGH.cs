using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ICreators;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IPrimitivesGH
{
    public class IMoebiusGH : GH_Component
    {
        private IMesh mesh;

        /// <summary>
        /// Initializes a new instance of the IMoebiusGH class.
        /// </summary>
        public IMoebiusGH()
          : base("iMoebiusStrip", "iMoebius",
              "Creates a Moebius strip quad-mesh.",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("U Count", "U", "Number of faces along the {x} direction.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("V Count", "V", "Number of faces along the {y} direction.", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Radius 1", "R1", "Radius in the plane {x} direction.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Radius 2", "R2", "Radius in the plane {y} direction.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Height", "H", "Moebius strip height.", GH_ParamAccess.item, 1);
            pManager.AddPlaneParameter("Plane", "Pl", "Construction plane.", GH_ParamAccess.item, Plane.WorldXY);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana surface mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane pl = Plane.WorldXY;
            int u = 10;
            int v = 5;
            double r1 = 1;
            double r2 = 1;
            double h = 1;

            //Retreive vertices and elements
            DA.GetData(0, ref u);
            DA.GetData(1, ref v);
            DA.GetData(2, ref r1);
            DA.GetData(3, ref r2);
            DA.GetData(4, ref h);
            DA.GetData(5, ref pl);

            u++;
            v++;


            //Build AHF-DataStructure
            mesh = IMeshCreator.CreateMoebiusStrip(u, v, r1, r2, h, pl);

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
            get { return new Guid("381e6ebf-0cfd-4eed-9c1a-df49b9b34671"); }
        }
    }
}