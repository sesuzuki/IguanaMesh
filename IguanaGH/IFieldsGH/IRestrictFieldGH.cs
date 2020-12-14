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
    public class IRestrictFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IRestrictFieldGH class.
        /// </summary>
        public IRestrictFieldGH()
                  : base("iRestrictField", "iRestrictField",
                      "Restrict the application of a field to a given list of geometrical points, curves, surfaces or volumes.",
                      "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iField", "iF", "Field to evaluate.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Nodes","Nodes", "IDs of points of the underlying model.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Edges", "Edges", "IDs of curves of the underlying model.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Surfaces", "Surfaces", "IDs of surfaces of the underlying model.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Volumes", "Volumes", "IDs of volumes of the underlying model.", GH_ParamAccess.list);
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
            List<double> EdgesList = new List<double>();
            List<double> FacesList = new List<double>();
            List<double> RegionsList = new List<double>();
            List<double> NodesList = new List<double>();
            IField auxField = null;

            DA.GetData(0, ref auxField);
            DA.GetDataList(1, NodesList);
            DA.GetDataList(2, EdgesList);
            DA.GetDataList(3, FacesList);
            DA.GetDataList(4, RegionsList);

            IField.Restrict field = new IField.Restrict();
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
                return Properties.Resources.iRestrictField;
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