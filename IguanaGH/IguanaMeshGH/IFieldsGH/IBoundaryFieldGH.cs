using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IBoundaryFieldGH : GH_Component
    {
        double AnisoMax = 1e10, hfar = 1, ratio = 1.1, thickness = 0.01;
        bool IntersectMetrics = false, quads = false;
        List<double> hwall_n_nodes = new List<double>() { 1.0 };

        /// <summary>
        /// Initializes a new instance of the IBoundaryFieldGH class.
        /// </summary>
        public IBoundaryFieldGH()
                  : base("iBoundaryField", "iBoundF",
                      "Boundary layer field to specify the size of the mesh elements.",
                      "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("AnisoMax", "Aniso", "Threshold angle for creating a mesh fan in the boundary layer", GH_ParamAccess.item, AnisoMax);
            pManager.AddNumberParameter("SizeWall", "SWall", "Element size far from the wall", GH_ParamAccess.item, hfar);
            pManager.AddNumberParameter("SizeNormal", "SNormal", "Mesh size normal to the the wall at nodes", GH_ParamAccess.list, hwall_n_nodes);
            pManager.AddNumberParameter("SizeRatio", "SRatio", "Size ratio between two successive layers", GH_ParamAccess.item, ratio);
            pManager.AddNumberParameter("MaxThickness", "Thickness", "Maximal thickness of the boundary layer", GH_ParamAccess.item, thickness);
            pManager.AddIntegerParameter("Edges", "E", "IDs of curves for which a boundary layer is needed", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Surfaces","S", "IDs of surfaces where the boundary layer should not be applied", GH_ParamAccess.list);
            pManager.AddIntegerParameter("FanNodes", "FN", "IDs of points for which a fan is created", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Nodes", "N", "IDs of points for which a boundary layer ends", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Intersect","Intersect", "Intersect metrics of all faces", GH_ParamAccess.item, IntersectMetrics);
            pManager.AddBooleanParameter("Quads", "Quads", "Generate recombined elements in the boundary layer", GH_ParamAccess.item, quads);
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
            pManager.AddGenericParameter("iMeshField", "iF", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<int> EdgesList = new List<int>();
            List<int> ExcludedFaceList = new List<int>();
            List<int> FanNodesList = new List<int>();
            List<int> NodesList = new List<int>();

            DA.GetData(0, ref AnisoMax);
            DA.GetData(1, ref hfar);
            DA.GetDataList(2, hwall_n_nodes);
            DA.GetData(3, ref ratio);
            DA.GetData(4, ref thickness);
            DA.GetDataList(5, EdgesList);
            DA.GetDataList(6, ExcludedFaceList);
            DA.GetDataList(7, FanNodesList);
            DA.GetDataList(8, NodesList);
            DA.GetData(9, ref IntersectMetrics);
            DA.GetData(10, ref quads);

            double[] eList = new double[EdgesList.Count];
            for (int i = 0; i < EdgesList.Count; i++) eList[i] = EdgesList[i];
            double[] nList = new double[NodesList.Count];
            for (int i = 0; i < NodesList.Count; i++) nList[i] = NodesList[i];
            double[] fList = new double[FanNodesList.Count];
            for (int i = 0; i < FanNodesList.Count; i++) fList[i] = FanNodesList[i];
            double[] exList = new double[ExcludedFaceList.Count];
            for (int i = 0; i < ExcludedFaceList.Count; i++) exList[i] = ExcludedFaceList[i];

            IguanaGmshField.BoundaryLayer field = new IguanaGmshField.BoundaryLayer();
            field.AnisoMax = AnisoMax;
            field.hfar = hfar;
            field.hwall_n_nodes = hwall_n_nodes.ToArray();
            field.ratio = ratio;
            field.thickness = thickness;
            field.EdgesList = eList;
            field.ExcludedFaceList = exList;
            field.FanNodesList = fList;
            field.NodesList = nList;
            field.IntersectMetrics = IntersectMetrics;
            field.Quads = quads;

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
            get { return new Guid("a028b130-983e-42b7-9e9a-bcfbfd155395"); }
        }
    }
}