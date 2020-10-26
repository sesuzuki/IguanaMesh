using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IRestrictFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IRestrictFieldGH class.
        /// </summary>
        public IRestrictFieldGH()
                  : base("iRestrictField", "iRestrictF",
                      "Restrict the application of a field to a given list of geometrical points, curves, surfaces or volumes.",
                      "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Field", "iF", "Field to evaluate.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Nodes","N", "IDs of points in the geometric model.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Edges", "E", "IDs of curves in the geometric model.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Surfaces", "S", "IDs of surfaces in the geometric model.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Volumes", "V", "IDs of volumes in the geometric model.", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
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
            List<double> EdgesList = new List<double>();
            List<double> FacesList = new List<double>();
            List<double> RegionsList = new List<double>();
            List<double> NodesList = new List<double>();
            IguanaGmshField auxField = null;

            DA.GetData(0, ref auxField);
            DA.GetDataList(1, NodesList);
            DA.GetDataList(2, EdgesList);
            DA.GetDataList(3, FacesList);
            DA.GetDataList(4, RegionsList);

            IguanaGmshField.Restrict field = new IguanaGmshField.Restrict();
            field.VerticesList = NodesList.ToArray();
            field.EdgesList = EdgesList.ToArray();
            field.FacesList = FacesList.ToArray();
            field.RegionsList = RegionsList.ToArray();
            field.IField = auxField;

            DA.SetData(0, field);

        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
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
            get { return new Guid("1a22e591-c6bb-4749-a5ca-5f04b1602f68"); }
        }
    }
}