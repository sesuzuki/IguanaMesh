﻿using System;
using System.Collections.Generic;
using GH_IO.Types;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IDeconstructVertexGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IDeconstructVertexGH class.
        /// </summary>
        public IDeconstructVertexGH()
          : base("IDeconstructVertex", "iDeconstructVertex",
              "Deconstruct Iguana vertex.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iVertex", "iVertex", "Base Iguana vertex.", GH_ParamAccess.item);        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Vertex", "v-Key", "´Vertex key.", GH_ParamAccess.item);
            pManager.AddPointParameter("Position", "Position", "Position.", GH_ParamAccess.item);
            pManager.AddPointParameter("TextureCoordinates", "Texture", "Texture coordinate.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element", "e-Key", "Element key to which this vertex is associated.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Half-facet", "hf-Key", "Half-facet key to which this vertex is associated.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ITopologicVertex vertex = new ITopologicVertex();
            DA.GetData(0, ref vertex);

            Point3d position = new Point3d(vertex.X, vertex.Y, vertex.Z);
            Point3d uvw = new Point3d(vertex.U, vertex.V, vertex.W );

            DA.SetData(0, vertex.Key);
            DA.SetData(1, position);
            DA.SetData(2, uvw);
            DA.SetData(3, vertex.GetElementID());
            DA.SetData(4, vertex.GetParentHalfFacetID());
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
            get { return new Guid("d901acf2-71b5-4d32-91d4-e58653ceca86"); }
        }
    }
}