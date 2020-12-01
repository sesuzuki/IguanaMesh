using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class ITriangularFaceGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ITriangularFaceGH()
          : base("iTriangleElement", "iTriangleElement",
              "A two-dimensional triangular element.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)       
        {
            pManager.AddIntegerParameter("First","N1","First vertex.",GH_ParamAccess.item);
            pManager.AddIntegerParameter("Second", "N2", "Second vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Third", "N3", "Third vertex.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iElement", "iE", "Iguana element.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int A=0, B=0, C=0;
            DA.GetData(0, ref A);
            DA.GetData(1, ref B);
            DA.GetData(2, ref C);

            ISurfaceElement e = new ISurfaceElement(A, B, C);

            DA.SetData(0, e);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iTrias;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2677cda1-ada0-4c8d-b3b4-df61f886b242"); }
        }
    }
}
