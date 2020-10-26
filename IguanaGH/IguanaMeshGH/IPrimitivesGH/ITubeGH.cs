using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ICreators;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IPrimitivesGH
{
    public class ITubeGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ITubeGH class.
        /// </summary>
        public ITubeGH()
          : base("iTube", "iTube",
              "Creates a tube quad-mesh.",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("U Count", "U", "Number of faces along the {x} direction.", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("V Count", "V", "Number of faces along the {y} direction.", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Inner Radius", "R1", "Inner radius of the tube.", GH_ParamAccess.item, 0.1);
            pManager.AddNumberParameter("Outer Radius", "R2", "Outer radius of the tube.", GH_ParamAccess.item, 1);
            //pManager.AddNumberParameter("Height", "H", "Height of the tube.", GH_ParamAccess.item, 10);
            pManager.AddCurveParameter("Curve Path", "P", "Curve describing the path of the tube.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Shift X", "X1", "Shift in the {x} direction.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Shift Y", "Y1", "Shift in the {y} direction.", GH_ParamAccess.item);
            pManager[5].Optional = true;
            pManager[6].Optional = true;
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
            int U = 0;
            int V = 0;
            double innerRadius = 0;
            double outerRadius = 0;
            //double height = 0;
            Curve path = null;
            double shiftX = 0;
            double shiftY = 0;

            DA.GetData(0, ref U);
            DA.GetData(1, ref V);
            DA.GetData(2, ref innerRadius);
            DA.GetData(3, ref outerRadius);
            // DA.GetData(4, ref height);
            DA.GetData(4, ref path);
            DA.GetData(5, ref shiftX);
            DA.GetData(6, ref shiftY);

            //Build AHF-DataStructure
            //IMesh mesh = IMeshCreator.CreateTube(U, V, innerRadius, outerRadius, height, shiftX, shiftY);
            IMesh mesh = IMeshCreator.CreateTube(U, V, innerRadius, outerRadius, path);

            DA.SetData(0, mesh);
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
            get { return new Guid("19296056-e02a-4edf-b331-c41ca616006c"); }
        }
    }
}