using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ICreators;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IPrimitivesGH
{
    public class AHF_CylinderGH : GH_Component
    {
        private double r1 = 1, r2 = 1, h = 1;
        private int u = 30, v = 20;
        private IMesh mesh;

        /// <summary>
        /// Initializes a new instance of the AHF_CylinderGH class.
        /// </summary>
        public AHF_CylinderGH()
          : base("iMesh Cylinder Constructor", "iCylinder",
              "Construct a cylinder quad-mesh stored via an Array-based Half-Facet (AHF) Mesh Data Structure.",
              "Iguana", "Primitives")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Base Plane", "B", "Base plane to construct the cylinder.", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddIntegerParameter("U Count", "U", "Number of faces along the {x} direction.", GH_ParamAccess.item, u);
            pManager.AddIntegerParameter("V Count", "V", "Number of faces along the {y} direction.", GH_ParamAccess.item, v);
            pManager.AddNumberParameter("Lower Radius", "R1", "Lower radius of the cylinder.", GH_ParamAccess.item, r1);
            pManager.AddNumberParameter("Upper Radius", "R2", "Upper radius of the cylinder.", GH_ParamAccess.item, r2);
            pManager.AddNumberParameter("Height", "H", "Height of the tube.", GH_ParamAccess.item, h);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Constructed Array-Based Half-Facet (AHF) Mesh Data Structure.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane pl = new Plane();
            mesh = new IMesh();

            DA.GetData(0, ref pl);
            DA.GetData(1, ref u);
            DA.GetData(2, ref v);
            DA.GetData(3, ref r1);
            DA.GetData(4, ref r2);
            DA.GetData(5, ref h);

            //Build AHF-DataStructure
            mesh = IMeshCreator.CreateCylinder(u, v, r1, r2, h, pl);

            DA.SetData(0, mesh);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.AHF_CylinderMesh;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2add74aa-66d2-4a41-b1d9-8d112646c31c"); }
        }
    }
}