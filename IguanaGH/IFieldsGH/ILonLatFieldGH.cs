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
    public class ILonLatFieldGH : GH_Component
    {
        double radius = 6371000;
        bool fromStereo = false;

        /// <summary>
        /// Initializes a new instance of the ILonLatFieldGH class.
        /// </summary>
        public ILonLatFieldGH()
          : base("iLonLatField", "iLonLatField",
              "Compute a field in geographic coordinates to specify the size of mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Field", "iF", "Field to evaluate.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "Radius", "Radius of the sphere of the stereograpic coordinates. Default is " + radius, GH_ParamAccess.item, radius);
            pManager.AddBooleanParameter("StereoCoords", "Stereo", "If true, the mesh is in stereographic coordinates. Default is " + fromStereo, GH_ParamAccess.item, fromStereo);
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
            IField auxfield = null;
            DA.GetData(0, ref auxfield);
            DA.GetData(1, ref radius);
            DA.GetData(2, ref fromStereo);

            IField.LonLat field = new IField.LonLat();
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
                return Properties.Resources.iLongLatField;
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