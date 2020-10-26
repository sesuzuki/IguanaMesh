using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IDistanceFieldGH : GH_Component
    {
        int NNodesByEdge = 20;

        /// <summary>
        /// Initializes a new instance of the IDistanceFieldGH class.
        /// </summary>
        public IDistanceFieldGH()
                  : base("iDistanceField", "iDistF",
                      "Compute the distance from the nearest node in a list. It can also be used to compute the distance from curves, in which case each curve is replaced by NNodesByEdge equidistant nodes and the distance from those nodes is computed.",
                      "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Edges", "E", "IDs of curves in the geometric model.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Nodes", "N", "IDs of points for which a boundary layer ends.", GH_ParamAccess.list);
            pManager.AddGenericParameter("FieldX","FX", "Field to use as x coordinate.", GH_ParamAccess.item);
            pManager.AddGenericParameter("FieldY", "FY", "Field to use as y coordinate.", GH_ParamAccess.item);
            pManager.AddGenericParameter("FieldZ", "FZ", "Field to use as z coordinate.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Count", "Count", "Number of nodes used to discretized each curve.", GH_ParamAccess.item, NNodesByEdge);
            pManager[0].Optional = true;
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
            IguanaGmshField FieldX = null, FieldY= null, FieldZ = null;
            List<int> EdgesList = new List<int>();
            List<int> NodesList = new List<int>();
            DA.GetDataList(0, EdgesList);
            DA.GetDataList(1, NodesList);
            DA.GetData(2, ref FieldX);
            DA.GetData(3, ref FieldY);
            DA.GetData(4, ref FieldZ);
            DA.GetData(5, ref NNodesByEdge);

            double[] eList = new double[EdgesList.Count];
            for (int i = 0; i < EdgesList.Count; i++) eList[i] = EdgesList[i];
            double[] nList = new double[NodesList.Count];
            for (int i = 0; i < NodesList.Count; i++) nList[i] = NodesList[i];

            IguanaGmshField.Distance field = new IguanaGmshField.Distance();
            field.EdgesList = eList;
            field.NodesList = nList;
            field.FieldX = FieldX;
            field.FieldY = FieldY;
            field.FieldZ = FieldZ;
            field.NNodesByEdge = NNodesByEdge;

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
            get { return new Guid("924d22ec-3a58-4f87-a83a-d960d9e00b51"); }
        }
    }
}