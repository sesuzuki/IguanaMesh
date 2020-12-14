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
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaMeshGH.ITransform
{
    public class ISkewGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ISkewGH class.
        /// </summary>
        public ISkewGH()
          : base("ISkew", "iSkew",
              "Skew a mesh",
              "Iguana", "Transform")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pl", "Base plane.", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddVectorParameter("Vector", "Direction", "Stretching direction.", GH_ParamAccess.item, Vector3d.ZAxis);
            pManager.AddNumberParameter("SkewFactor", "SkewFactor", "Skew factor.", GH_ParamAccess.item, 1.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            Vector3d vec = Vector3d.ZAxis;
            Plane plane = Plane.WorldXY;
            double sCoef = 1;

            DA.GetData(0, ref mesh);
            DA.GetData(1, ref plane);
            DA.GetData(2, ref vec);
            DA.GetData(3, ref sCoef);

            IPlane pl = new IPlane(plane.OriginX, plane.OriginY, plane.OriginZ, plane.Normal.X, plane.Normal.Y, plane.Normal.Z);
            IVector3D dir = new IVector3D(vec.X, vec.Y, vec.Z);
            IMesh dM = IModifier.Skew(mesh, pl, dir, sCoef);

            DA.SetData(0, dM);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iSkew;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3a666f66-3fa4-4d4f-a8df-c392cc893083"); }
        }
    }
}