﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ICreators;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IFromRhinoMeshGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IFromRhinoMeshGH class.
        /// </summary>
        public IFromRhinoMeshGH()
          : base("iFromRhinoMesh", "iRhinoMesh",
              "Create an Iguana mesh from a Rhino mesh.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("iMesh", "iM", "Iguana surface mesh.", GH_ParamAccess.item);
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
            Mesh m = new Mesh();
            DA.GetData(0, ref m);

            //Build AHF-DataStructure
            IMesh mesh = IMeshCreator.CreateFromRhinoMesh(m);

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
                return Properties.Resources.iFromRhinoMesh;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9681bf46-7b70-4b22-88f9-c6bb78966235"); }
        }
    }
}