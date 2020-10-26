using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ICreators;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IPrimitivesGH
{
    public class IEllipticDupinCyclideGH : GH_Component
    {
        private IMesh mesh;
        /// <summary>
        /// Initializes a new instance of the AHF_EllipticDupinCyclide class.
        /// </summary>
        public IEllipticDupinCyclideGH()
          : base("iEllipticDupinCyclide", "iCyclide",
              "Creates an elliptic dupin cyclide quad-mesh.",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("U Count", "U", "Number of faces along the {x} direction.", GH_ParamAccess.item, 30);
            pManager.AddIntegerParameter("V Count", "V", "Number of faces along the {y} direction.", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Semi major", "a", "Parameter for the semi major axis.", GH_ParamAccess.item, 5.0);
            pManager.AddNumberParameter("Semi minor", "b", "Parameter for the semi minor axis.", GH_ParamAccess.item, 5.0);
            pManager.AddNumberParameter("Liner eccentricity", "c", "Linear eccentricity of the ellipse.", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Average radius", "d", "Average radius of the generating spheres.", GH_ParamAccess.item, 1.5);
            pManager.AddIntervalParameter("Domain {x}", "D1", "Domain in the {x} direction. The domain should be bounded between 0 to 2π.", GH_ParamAccess.item, new Interval(0, 2*Math.PI));
            pManager.AddIntervalParameter("Domain {y}", "D2", "Domain in the {y} direction. The domain should be bounded between 0 to 2π.", GH_ParamAccess.item, new Interval(0, 2 * Math.PI));
            pManager.AddPlaneParameter("Plane", "Pl", "Construction plane.", GH_ParamAccess.item, Plane.WorldXY);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;
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
            Plane pl = new Plane();
            int u = 30;
            int v = 10;
            double a = 5;
            double b = 5;
            double c = 1.0;
            double d = 1.5;
            Interval D1 = new Interval(0, 2 * Math.PI);
            Interval D2 = new Interval(0, 2 * Math.PI);

            //Retreive vertices and elements
            DA.GetData(0, ref u);
            DA.GetData(1, ref v);
            DA.GetData(2, ref a);
            DA.GetData(3, ref b);
            DA.GetData(4, ref c);
            DA.GetData(5, ref d);
            DA.GetData(6, ref D1);
            DA.GetData(7, ref D2);
            DA.GetData(8, ref pl);


            //Build AHF-DataStructure
            mesh = IMeshCreator.CreateEllipticDupinCyclide(u, v, a, b, c, d, D1, D2, pl);

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
            get { return new Guid("c008eb73-bcd8-4f71-b736-c146c236e3cc"); }
        }
    }
}