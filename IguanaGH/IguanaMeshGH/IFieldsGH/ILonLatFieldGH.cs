using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ISettingsGH
{
    public class ILonLatFieldGH : GH_Component
    {
        double radius = 6371000;
        bool fromStereo = false;

        /// <summary>
        /// Initializes a new instance of the ILonLatFieldGH class.
        /// </summary>
        public ILonLatFieldGH()
          : base("iLonLatField", "iLonLatF",
              "Compute a Field in geographic coordinates(longitude, latitude)\nF = Field (atan(y/x), asin(z/sqrt(x^2+y^2+z^2))",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Field", "iF", "Field to evaluate.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "R", "Radius of the sphere of the stereograpic coordinates. Default is " + radius, GH_ParamAccess.item, radius);
            pManager.AddBooleanParameter("StereoCoords", "St", "If = true, the mesh is in stereographic coordinates (xi = 2Rx/(R+z), eta = 2Ry/(R+z)). Default is " + fromStereo, GH_ParamAccess.item, fromStereo);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshField", "iF", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IguanaGmshField auxfield = null;
            DA.GetData(0, ref auxfield);
            DA.GetData(1, ref radius);
            DA.GetData(2, ref fromStereo);

            IguanaGmshField.LonLat field = new IguanaGmshField.LonLat();
            field.IField = auxfield;
            field.RadiusStereo = radius;
            field.FromStereo = fromStereo;

            DA.SetData(0, field);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
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
            get { return new Guid("38aab679-4bd2-401f-b4ff-3077ad7586bd"); }
        }
    }
}