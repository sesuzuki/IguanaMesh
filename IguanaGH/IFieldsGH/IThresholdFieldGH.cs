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
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.IFields
{
    public class IThresholdFieldGH : GH_Component
    {
        double distMax = 10, distMin = 1, lcMax = 1, lcMin = 0.1;
        bool sigmoid = false, stopAtDistMax = false;

        /// <summary>
        /// Initializes a new instance of the IThresholdFieldGH class.
        /// </summary>
        public IThresholdFieldGH()
          : base("iThresholdField", "iThresholdField",
              "Threshold field to specify the size of the mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iField", "iF", "Field to evaluate.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Maximum Distance", "MaxDist", "Distance from entity after which element size will be MaxSize. Default is " + distMax, GH_ParamAccess.item, distMax);
            pManager.AddNumberParameter("Minimum Distance", "MinDist", "Distance from entity up to which element size will be MinSize. Default is " + distMin, GH_ParamAccess.item, distMin);
            pManager.AddNumberParameter("Maximum Size", "MaxSize", "Element size outside MaxDist. Default is " + lcMax, GH_ParamAccess.item, lcMax);
            pManager.AddNumberParameter("Minimum Size", "MinSize", "Element size inside MinDist. Default is " + lcMin, GH_ParamAccess.item, lcMin);
            pManager.AddBooleanParameter("Sigmoid", "Sigmoid", "True for interpolating between MinSize and MaxSize using a sigmoid or false to interpolate linearly. Default is " + sigmoid, GH_ParamAccess.item, sigmoid);
            pManager.AddBooleanParameter("Stop", "Stop", "Impose element size outside dMax. Default is " + stopAtDistMax, GH_ParamAccess.item, stopAtDistMax);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iField", "iField", "Field for mesh generation.", GH_ParamAccess.item);

        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IField auxfield = null;
            DA.GetData(0, ref auxfield);
            DA.GetData(1, ref distMax);
            DA.GetData(2, ref distMin);
            DA.GetData(3, ref lcMax);
            DA.GetData(4, ref lcMin);
            DA.GetData(5, ref sigmoid);
            DA.GetData(6, ref stopAtDistMax);

            IField.Threshold field = new IField.Threshold();
            field.IField = auxfield;
            field.DistMax = distMax;
            field.DistMin = distMin;
            field.LcMax = lcMax;
            field.LcMin = lcMin;
            field.Sigmoid = sigmoid;
            field.StopAtDistMax = stopAtDistMax;

            DA.SetData(0, field);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iThesholdField;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fe68339b-be68-415a-937c-09e286ae4e23"); }
        }
    }
}