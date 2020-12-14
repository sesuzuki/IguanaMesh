/*
 * <IguanaMesh>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.IFields
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
            : base("iBoundaryField", "iBoundaryField",
                   "Compute a boundary field to specify the size of mesh elements.",
                   "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Maximum Anisotropy", "AnisoMax", "Threshold angle for creating a mesh fan in the boundary layer", GH_ParamAccess.item, AnisoMax);
            pManager.AddNumberParameter("SizeWall", "SizeWall", "Element size far from the wall", GH_ParamAccess.item, hfar);
            pManager.AddNumberParameter("SizeNormal", "SizeNormal", "Mesh size normal to the the wall at nodes", GH_ParamAccess.list, hwall_n_nodes);
            pManager.AddNumberParameter("SizeRatio", "SizeRatio", "Size ratio between two successive layers", GH_ParamAccess.item, ratio);
            pManager.AddNumberParameter("MaxThickness", "Thickness", "Maximal thickness of the boundary layer", GH_ParamAccess.item, thickness);
            pManager.AddIntegerParameter("Edges", "Edges", "IDs of curves for which a boundary layer is needed", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Surfaces","Surfaces", "IDs of surfaces where the boundary layer should not be applied", GH_ParamAccess.list);
            pManager.AddIntegerParameter("FanNodes", "FanNodes", "IDs of points for which a fan is created", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Nodes", "Nodes", "IDs of points for which a boundary layer ends", GH_ParamAccess.list);
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
            pManager.AddGenericParameter("iField", "iField", "Field for mesh generation.", GH_ParamAccess.item);
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

            IField.BoundaryLayer field = new IField.BoundaryLayer();
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
                return Properties.Resources.iBoundaryField;
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