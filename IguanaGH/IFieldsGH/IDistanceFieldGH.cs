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
    public class IDistanceFieldGH : GH_Component
    {
        int NNodesByEdge = 20;

        /// <summary>
        /// Initializes a new instance of the IDistanceFieldGH class.
        /// </summary>
        public IDistanceFieldGH()
                  : base("iDistanceField", "iDistanceField",
                      "Compute a distance field to specify the size of mesh elements.",
                      "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Edges", "Edges", "IDs of curves in the geometric model.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Nodes", "Nodes", "IDs of points for which a boundary layer ends.", GH_ParamAccess.list);
            pManager.AddGenericParameter("FieldX","FieldX", "Field to use as x coordinate.", GH_ParamAccess.item);
            pManager.AddGenericParameter("FieldY", "FieldY", "Field to use as y coordinate.", GH_ParamAccess.item);
            pManager.AddGenericParameter("FieldZ", "FieldZ", "Field to use as z coordinate.", GH_ParamAccess.item);
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
            pManager.AddGenericParameter("iField", "iField", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IField FieldX = null, FieldY= null, FieldZ = null;
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

            IField.Distance field = new IField.Distance();
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
                return Properties.Resources.iDistanceField;
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