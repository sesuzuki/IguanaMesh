/*
 * <Iguana>
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

using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Iguana.IguanaMesh;

namespace Iguana.IguanaMesh.ITypes
{
    public abstract class IField : IGH_Goo
    {
        private int fieldTag =-1;
        public int Tag { get => fieldTag; }

        public abstract void ApplyField();

        #region GH_methods
        public bool IsValid
        {
            get => !this.Equals(null);
        }

        public string IsValidWhyNot
        {
            get
            {
                string msg;
                if (this.IsValid) msg = string.Empty;
                else msg = string.Format("Invalid type.", this.TypeName);
                return msg;
            }
        }

        public override string ToString()
        {
            return "IguanaGmshField";
        }

        public string TypeName
        {
            get => ToString();
        }

        public string TypeDescription
        {
            get => ToString();
        }

        public IGH_Goo Duplicate()
        {
            return (IGH_Goo)this.MemberwiseClone();
        }

        public IGH_GooProxy EmitProxy()
        {
            return null;
        }

        public bool CastFrom(object source)
        {
            return false;
        }

        public object ScriptVariable()
        {
            return this;
        }

        public bool Write(GH_IWriter writer)
        {
            return true;
        }

        public bool Read(GH_IReader reader)
        {
            return true;
        }

        public bool CastTo<T>(out T target)
        {
            if (typeof(T).IsAssignableFrom(typeof(IField)))
            {
                target = (T)(object)this;
                return true;
            }

            target = default(T);
            return false;
        }
        #endregion

        /// <summary>
        /// Compute the distance from the nearest curve in a list. 
        /// Then the mesh size can be specified independently in the direction normal to the curve and in the direction parallel to the curve (Each curve is replaced by NNodesByEdge equidistant nodes and the distance from those nodes is computed.)
        /// </summary>
        public class  AttractorAnisoCurve : IField
        {
            public AttractorAnisoCurve() { 
                EdgesList = null;
                NNodesByEdge = 20;
                dMax = 0.5;
                dMin = 0.1;
                lMaxNormal = 0.5;
                lMaxTangent = 0.5;
                lMinNormal = 0.05;
                lMinTangent = 0.05;
            }

            /// <summary>
            /// Tags of curves in the geometric model.
            /// </summary>
            public double[] EdgesList { get; set; }

            /// <summary>
            /// Number of nodes used to discretized each curve
            /// </summary>
            public int NNodesByEdge { get; set; }

            /// <summary>
            /// Maxmium distance, above this distance from the curves, prescribe the maximum mesh sizes.
            /// </summary>
            public double dMax { get; set; }

            /// <summary>
            /// Minimum distance, below this distance from the curves, prescribe the minimum mesh sizes.
            /// </summary>
            public double dMin { get; set; }

            /// <summary>
            /// Maximum mesh size in the direction normal to the closest curve.
            /// </summary>
            public double lMaxNormal { get; set; }

            /// <summary>
            /// Maximum mesh size in the direction tangeant to the closest curve.
            /// </summary>
            public double lMaxTangent { get; set; }

            /// <summary>
            /// Minimum mesh size in the direction normal to the closest curve.
            /// </summary>
            public double lMinNormal { get; set; }

            /// <summary>
            /// Minimum mesh size in the direction tangeant to the closest curve.
            /// </summary>
            public double lMinTangent { get; set; }

            public override void ApplyField()
            {
                if (EdgesList != null)
                {
                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("AttractorAnisoCurve");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "EdgesList", EdgesList);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "NNodesByEdge", NNodesByEdge);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "dMax", dMax);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "dMin", dMin);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "lMaxNormal", lMaxNormal);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "lMaxTangent", lMaxTangent);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "lMinNormal", lMinNormal);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "lMinTangent", lMinTangent);
                }
            }

            public override string ToString()
            {
                return "IAttractorAnisoCurve-Field";
            }
        }

        /// <summary>
        /// Compute a mesh size field that is quite automatic Takes into account surface curvatures and closeness of objects
        /// </summary>
        public class AutomaticMeshSizeField : IField
        {
            public AutomaticMeshSizeField()
            {
                NRefine = 5;
                gradientMax = 1.4;
                hBulk = 0.1;
                nPointsPerCircle = 55;
                nPointsPerGap = 5;
            }

            /// <summary>
            /// Initial refinement level for the octree.
            /// </summary>
            public int NRefine { get; set; }

            /// <summary>
            /// Maximun gradient of the size field
            /// </summary>
            public double gradientMax { get; set; }

            /// <summary>
            /// Size everywhere no size is prescribed
            /// </summary>
            public double hBulk { get; set; }

            /// <summary>
            /// Number of points per circle(adapt to curvature of surfaces)
            /// </summary>
            public int nPointsPerCircle { get; set; }

            /// <summary>
            /// Number of points in thin layers
            /// </summary>
            public int nPointsPerGap { get; set; }

            public override void ApplyField()
            {
                fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("AutomaticMeshSizeField");
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "NRefine", NRefine);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "gradientMax", gradientMax);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "hBulk", hBulk);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "nPointsPerCircle", nPointsPerCircle);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "nPointsPerGap", nPointsPerGap);
            }

            public override string ToString()
            {
                return "AutomaticMeshSizeField";
            }
        }

        /// <summary>
        /// The value of this field is VIn inside a spherical ball, VOut outside.The ball is defined by
        /// ||dX||^2 < R^2 &&
        /// dX = (X - XC) ^ 2 + (Y - YC) ^ 2 + (Z - ZC) ^ 2
        /// If Thickness is > 0, the mesh size is interpolated between VIn and VOut in a layer around the ball of the prescribed thickness.
        /// </summary>
        public class Ball : IField
        {
            public Ball()
            {
                Radius = 1;
                Thickness = 1;
                VIn = 1;
                VOut = 1;
                XCenter = 0;
                YCenter = 0;
                ZCenter = 0;
            }
            /// <summary>
            /// Radius
            /// </summary>
            public double Radius { get; set; }

            /// <summary>
            /// Thickness of a transition layer outside the ball
            /// </summary>
            public double Thickness { get; set; }

            /// <summary>
            /// Value inside the ball
            /// </summary>
            public double VIn { get; set; }

            /// <summary>
            /// Value outside the ball
            /// </summary>
            public double VOut { get; set; }

            /// <summary>
            /// X coordinate of the ball center
            /// </summary>
            public double XCenter { get; set; }

            /// <summary>
            /// Y coordinate of the ball center
            /// </summary>
            public double YCenter { get; set; }

            /// <summary>
            /// Z coordinate of the ball center
            /// </summary>
            public double ZCenter { get; set; }

            public override void ApplyField()
            {
                fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Ball");
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Radius", Radius);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Thickness", Thickness);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "VIn", VIn);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "VOut", VOut);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "XCenter", XCenter);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "YCenter", YCenter);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "ZCenter", ZCenter);
            }

            public override string ToString()
            {
                return "IBall-Field";
            }
        }

        /// <summary>
        /// hwall* ratio^(dist/hwall)
        /// </summary>
        public class BoundaryLayer : IField
        {
            public BoundaryLayer()
            {
                AnisoMax = 1e10;
                EdgesList = new double[] { };
                ExcludedFaceList = new double[] { };
                FanNodesList = new double[] { };
                IntersectMetrics = false;
                NodesList = new double[] { };
                Quads = false;
                hfar = 1;
                hwall_n_nodes = new double[] { 0.1 };
                ratio = 1.1;
                thickness = 0.01;
            }

            /// <summary>
            /// Threshold angle for creating a mesh fan in the boundary layer
            /// </summary>
            public double AnisoMax { get; set; }

            /// <summary>
            /// Tags of curves in the geometric model for which a boundary layer is needed
            /// </summary>
            public double[] EdgesList { get; set; }

            /// <summary>
            /// Tags of surfaces in the geometric model where the boundary layer should not be applied
            /// </summary>
            public double[] ExcludedFaceList { get; set; }

            /// <summary>
            /// Tags of points in the geometric model for which a fan is created
            /// </summary>
            public double[] FanNodesList { get; set; }

            /// <summary>
            /// Intersect metrics of all faces
            /// </summary>
            public bool IntersectMetrics { get; set; }

            /// <summary>
            /// Tags of points in the geometric model for which a boundary layer ends
            /// </summary>
            public double[] NodesList { get; set; }

            /// <summary>
            /// Generate recombined elements in the boundary layer
            /// </summary>
            public bool Quads { get; set; }

            /// <summary>
            /// Element size far from the wall
            /// </summary>
            public double hfar { get; set; }

            /// <summary>
            /// Mesh Size Normal to the The Wall at nodes(overwrite hwall_n when defined)
            /// </summary>
            public double[] hwall_n_nodes { get; set; }

            /// <summary>
            /// Size Ratio Between Two Successive Layers
            /// </summary>
            public double ratio { get; set; }

            /// <summary>
            /// Maximal thickness of the boundary layer
            /// </summary>
            public double thickness { get; set; }

            public override void ApplyField()
            {
                fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("BoundaryLayer");
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "AnisoMax", AnisoMax);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "EdgesList ", EdgesList);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "ExcludedFaceList", ExcludedFaceList);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "FanNodesList", FanNodesList);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "IntersectMetrics", Convert.ToInt32(IntersectMetrics));
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "NodesList", NodesList);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Quads", Convert.ToInt32(Quads));
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "hfar", hfar);
                if(hwall_n_nodes.Length>1) IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "hwall_n_nodes", hwall_n_nodes);
                else if (hwall_n_nodes.Length == 1) IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "hwall_n", hwall_n_nodes[0]);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "ratio", ratio);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "thickness", thickness);
            }

            public override string ToString()
            {
                return "IBoundaryLayer-Field";
            }
        }

        /// <summary>
        /// The value of this field is VIn inside the box, VOut outside the box. The box is defined by
        /// Xmin <= x <= XMax &&
        /// YMin <= y <= YMax &&
        /// ZMin <= z <= ZMax
        /// If Thickness is > 0, the mesh size is interpolated between VIn and VOut in a layer around the box of the prescribed thickness.
        /// </summary>
        public class Box : IField
        {
            public Box()
            {
                Thickness = 0;
                VIn = 1;
                VOut = 1;
                XMax = 0.5;
                XMin = -0.5;
                YMax = 0.5;
                YMin = -0.5;
                ZMax = 0.5;
                ZMin = -0.5;
            }

            /// <summary>
            /// Thickness of a transition layer outside the box
            /// </summary>
            public double Thickness { get; set; }

            /// <summary>
            /// Value inside the box
            /// </summary>
            public double VIn { get; set; }

            /// <summary>
            /// Value outside the box
            /// </summary>
            public double VOut { get; set; }

            /// <summary>
            /// Maximum X coordinate of the box
            /// </summary>
            public double XMax { get; set; }

            /// <summary>
            /// Minimum X coordinate of the box
            /// </summary>
            public double XMin { get; set; }

            /// <summary>
            /// Maximum Y coordinate of the box
            /// </summary>
            public double YMax { get; set; }

            /// <summary>
            /// Minimum Y coordinate of the box
            /// </summary>
            public double YMin { get; set; }

            /// <summary>
            /// Maximum Z coordinate of the box
            /// </summary>
            public double ZMax { get; set; }

            /// <summary>
            /// Minimum Z coordinate of the box
            /// </summary>
            public double ZMin { get; set; }

            public override void ApplyField()
            {
                fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Box");
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Thickness", Thickness);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "VIn", VIn);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "VOut", VOut);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "XMax", XMax);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "XMin", XMin);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "YMax", YMax);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "YMin", YMin);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "ZMax", ZMax);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "ZMin", ZMin);
            }

            public override string ToString()
            {
                return "IBox-Field";
            }
        }

        /// <summary>
        /// Compute the curvature of Field[IField]:
        /// F = div(norm(grad(Field[IField])))
        /// </summary>
        public class Curvature : IField
        {
            public Curvature()
            {
                Delta = 0;
                IField = null;
            }
            /// <summary>
            /// Step of the finite differences
            /// </summary>
            public double Delta { get; set; }

            /// <summary>
            /// Field
            /// </summary>
            public IField IField { get; set; }

            public override void ApplyField()
            {
                if (IField != null)
                {
                    IField.ApplyField();
                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Curvature");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Delta", Delta);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "IField", IField.Tag);
                }
            }

            public override string ToString()
            {
                return "ICurvature-Field";
            }
        }

        /// <summary>
        /// The value of this field is VIn inside a frustrated cylinder, VOut outside. The cylinder is given by
        /// ||dX||^2 < R^2 && 
        /// (X-X0).A< ||A||^2
        /// dX = (X - X0) - ((X - X0).A)/(||A||^2) . A
        /// </summary>
        public class Cylinder : IField
        {
            public Cylinder()
            {
                Radius = 1;
                VIn = 1;
                VOut = 1;
                XAxis = 0;
                XCenter = 0;
                YAxis = 0;
                YCenter = 0;
                ZAxis = 1;
                ZCenter = 0;
            }

            /// <summary>
            /// Radius
            /// </summary>
            public double Radius { get; set; }

            /// <summary>
            /// Value inside the cylinder
            /// </summary>
            public double VIn { get; set; }

            /// <summary>
            /// Value outside the cylinder
            /// </summary>
            public double VOut { get; set; }

            /// <summary>
            /// X component of the cylinder axis
            /// </summary>
            public double XAxis { get; set; }

            /// <summary>
            /// X coordinate of the cylinder center
            /// </summary>
            public double XCenter { get; set; }

            /// <summary>
            /// Y component of the cylinder axis
            /// </summary>
            public double YAxis { get; set; }

            /// <summary>
            /// Y coordinate of the cylinder center
            /// </summary>
            public double YCenter { get; set; }

            /// <summary>
            /// Z component of the cylinder axis
            /// </summary>
            public double ZAxis { get; set; }

            /// <summary>
            /// Z coordinate of the cylinder center
            /// </summary>
            public double ZCenter { get; set; }

            public override void ApplyField()
            {
                fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Cylinder");
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Radius", Radius);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "VIn", VIn);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "VOut", VOut);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "XAxis", XAxis);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "XCenter", XCenter);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "YAxis", YAxis);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "YCenter", YCenter);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "ZAxis", ZAxis);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "ZCenter", ZCenter);
            }

            public override string ToString()
            {
                return "ICylinder-Field";
            }
        }

        /// <summary>
        /// Compute the distance from the nearest node in a list. It can also be used to compute the distance from curves, in which case each curve is replaced by NNodesByEdge equidistant nodes and the distance from those nodes is computed.
        /// </summary>
        public class Distance : IField
        {
            public Distance()
            {
                EdgesList = new double[] { };
                FacesList = new double[] { };
                NodesList = new double[] { };
                FieldX = null;
                FieldY = null;
                FieldZ = null;
                NNodesByEdge = 20;
            }

            /// <summary>
            /// Tags of curves in the geometric model
            /// </summary>
            public double[] EdgesList { get; set; }

            /// <summary>
            /// Tags of surfaces in the geometric model(Warning, this feature is still experimental. It might (read: will probably) give wrong results for complex surfaces)
            /// </summary>
            public double[] FacesList { get; set; }

            /// <summary>
            /// Id of the field to use as x coordinate.
            /// </summary>
            public IField FieldX { get; set; }

            /// <summary>
            /// Id of the field to use as y coordinate.
            /// </summary>
            public IField FieldY { get; set; }

            /// <summary>
            /// Id of the field to use as z coordinate.
            /// </summary>
            public IField FieldZ { get; set; }

            /// <summary>
            /// Number of nodes used to discretized each curve
            /// </summary>
            public int NNodesByEdge { get; set; }

            /// <summary>
            /// Tags of points in the geometric model
            /// </summary>
            public double[] NodesList { get; set; }

            public override void ApplyField()
            {
                bool flag = false;
                if (FieldX != null)
                {
                    FieldX.ApplyField();
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "FieldX", FieldX.Tag);
                    flag = true;
                }
                if (FieldY != null)
                {
                    FieldY.ApplyField();
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "FieldY", FieldY.Tag);
                    flag = true;
                }
                if (FieldZ != null)
                {
                    FieldZ.ApplyField();
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "FieldZ", FieldZ.Tag);
                    flag = true;
                }
                if (flag)
                {
                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Distance");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "EdgesList", EdgesList);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "NNodesByEdge", NNodesByEdge);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "NodesList", NodesList);
                }
            }

            public override string ToString()
            {
                return "IDistance-Field";
            }
        }

        /// <summary>
        /// This field is an extended cylinder with inner(i) and outer(o) radiuseson both endpoints(1 and 2). Length scale is bilinearly interpolated betweenthese locations(inner and outer radiuses, endpoints 1 and 2)The field values for a point P are given by : u = P1P.P1P2/||P1P2|| r = || P1P - u* P1P2 || Ri = (1-u)*R1i + u* R2i Ro = (1-u)*R1o + u* R2o v = (r-Ri)/(Ro-Ri) lc = (1-v)*((1-u)*v1i + u* v2i ) + v* ((1-u)*v1o + u* v2o ) where(u, v) in [0,1]
        /// x[0, 1]
        /// </summary>
        public class Frustum : IField
        {
            public Frustum()
            {
                R1_inner = 0;
                R1_outer = 1;
                R2_inner = 0;
                R2_outer = 1;
                V1_inner = 0.1;
                V1_outer = 1;
                V2_inner = 0.1;
                V2_outer = 1;
                X1 = 0;
                X2 = 0;
                Y1 = 0;
                Y2 = 0;
                Z1 = 1;
                Z2 = 1.455171629957881e-152;
            }
            /// <summary>
            /// Inner radius of Frustum at endpoint 1
            /// </summary>
            public double R1_inner { get; set; }

            /// <summary>
            /// Outer radius of Frustum at endpoint 1
            /// </summary>
            public double R1_outer { get; set; }

            /// <summary>
            /// Inner radius of Frustum at endpoint 2
            /// </summary>
            public double R2_inner { get; set; }

            /// <summary>
            /// Outer radius of Frustum at endpoint 2
            /// </summary>
            public double R2_outer { get; set; }

            /// <summary>
            /// Element size at point 1, inner radius
            /// </summary>
            public double V1_inner { get; set; }

            /// <summary>
            /// Element size at point 1, outer radius
            /// </summary>
            public double V1_outer { get; set; }

            /// <summary>
            /// Element size at point 2, inner radius
            /// </summary>
            public double V2_inner { get; set; }

            /// <summary>
            /// Element size at point 2, outer radius
            /// </summary>
            public double V2_outer { get; set; }

            /// <summary>
            /// X coordinate of endpoint 1
            /// </summary>
            public double X1 { get; set; }

            /// <summary>
            /// X coordinate of endpoint 2
            /// </summary>
            public double X2 { get; set; }

            /// <summary>
            /// Y coordinate of endpoint 1
            /// </summary>
            public double Y1 { get; set; }

            /// <summary>
            /// Y coordinate of endpoint 2
            /// </summary>
            public double Y2 { get; set; }

            /// <summary>
            /// Z coordinate of endpoint 1
            /// </summary>
            public double Z1 { get; set; }

            /// <summary>
            /// Z coordinate of endpoint 2
            /// </summary>
            public double Z2 { get; set; }

            public override void ApplyField()
            {
                fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Frustum");
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "R1_inner", R1_inner);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "R1_outer", R1_outer);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "R2_inner", R2_inner);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "R2_outer", R2_outer);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "V1_inner", V1_inner);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "V1_outer", V1_outer);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "V2_inner", V2_inner);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "V2_outer", V2_outer);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "X1", X1);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "X2", X2);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Y1", Y1);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Y2", Y2);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Z1", Z1);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Z2", Z2);
            }

            public override string ToString()
            {
                return "IFrustum-Field";
            }
        }

        /// <summary>
        /// Compute the finite difference gradient of Field[IField]:
        /// F = (Field[IField](X + Delta/2) - Field[IField] (X - Delta/2)) / Delta
        /// </summary>
        public class Gradient : IField
        {
            public Gradient()
            {
                Delta = 0;
                IField = null;
                Kind = 0;
            }

            /// <summary>
            /// Finite difference step
            /// </summary>
            public double Delta { get; set; }

            /// <summary>
            /// Field index
            /// </summary>
            public IField IField { get; set; }

            /// <summary>
            /// Component of the gradient to evaluate: 0 for X, 1 for Y, 2 for Z, 3 for the norm
            /// </summary>
            public int Kind { get; set; }

            public override void ApplyField()
            {
                if (IField != null) {
                    IField.ApplyField();
                    IKernel.IMeshingKernel.IBuilder.AddMeshField("Gradient");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Delta", Delta);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "IField", IField.Tag);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Kind", Kind);
                }
            }

            public override string ToString()
            {
                return "GradientField";
            }
        }

        /// <summary>
        /// Take the intersection of 2 anisotropic fields according to Alauzet.
        /// </summary>
        public class IntersectAniso : IField
        {
            public IntersectAniso()
            {
                FieldsList = null;
            }

            /// <summary>
            /// Fields
            /// </summary>
            public List<IField> FieldsList { get; set; }

            public override void ApplyField()
            {
                if (FieldsList != null)
                {
                    double[] fTags = new double[FieldsList.Count];
                    IField f;
                    for(int i=0; i<FieldsList.Count; i++)
                    {
                        f = FieldsList[i];
                        f.ApplyField();
                        fTags[i] = f.Tag;
                    }

                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("IntersectAniso");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "FieldsList", fTags);
                }
            }

            public override string ToString()
            {
                return "IIntersectAniso-Field";
            }
        }

        /// <summary>
        /// Compute finite difference the Laplacian of Field:
        /// F = G(x+d, y, z) + G(x-d, y, z) +
        /// G(x, y+d, z) + G(x, y-d, z) +
        /// G(x, y, z+d) + G(x, y, z-d) - 6 * G(x, y, z),
        /// where G = Field[IField] and d = Delta
        /// </summary>
        public class Laplacian :IField
        {
            public Laplacian()
            {
                Delta = 0;
                IField = null;
            }

            /// <summary>
            /// Finite difference step
            /// </summary>
            public double Delta { get; set; }

            /// <summary>
            /// Field index
            /// </summary>
            public IField IField { get; set; }

            public override void ApplyField()
            {
                if (IField != null)
                {
                    IField.ApplyField();
                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Laplacian");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Delta", Delta);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "IField", IField.Tag);
                }
            }

            public override string ToString()
            {
                return "ILaplacian-Field";
            }
        }

        /// <summary>
        /// Evaluate Field[IField] in geographic coordinates(longitude, latitude):
        /// F = Field[IField] (atan(y/x), asin(z/sqrt(x^2+y^2+z^2))
        /// </summary>
        public class LonLat : IField
        {
            public LonLat()
            {
                FromStereo = false;
                IField = null;
                RadiusStereo = 6371000;
            }

            /// <summary>
            /// if = true, the mesh is in stereographic coordinates.xi = 2Rx/(R+z), eta = 2Ry/(R+z)
            /// </summary>
            public bool FromStereo { get; set; }

            /// <summary>
            /// Index of the field to evaluate.
            /// </summary>
            public IField IField { get; set; }

            /// <summary>
            /// radius of the sphere of the stereograpic coordinates
            /// </summary>
            public double RadiusStereo { get; set; }

            public override void ApplyField()
            {
                if (IField != null)
                {
                    IField.ApplyField();
                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("LonLat");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "FromStereo", Convert.ToInt32(FromStereo));
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "IField", IField.Tag);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "RadiusStereo", RadiusStereo);
                }
            }

            public override string ToString()
            {
                return "ILonLat-Field";
            }
        }

        /// <summary>
        /// Evaluate a mathematical expression.The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions.
        /// </summary>
        public class MathEval : IField
        {
            public MathEval()
            {
                F = "";
                Fields = new List<IField>();
            }

            /// <summary>
            /// Mathematical function to evaluate.
            /// </summary>
            public string F { get; set; }

            public List<IField> Fields { get; set; }

            public override void ApplyField()
            {
                if (Fields != null)
                {
                    string temp;
                    IField f;
                    for(int i=0; i<Fields.Count; i++)
                    {
                        f = Fields[i];
                        f.ApplyField();
                        temp = F.Replace("F" + i, "F" + f.Tag);
                        F = temp;
                    }
                }

                fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("MathEval");
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionString(fieldTag, "F", F);
            }

            public override string ToString()
            {
                return "IMath-Field";
            }
        }

        /// <summary>
        /// Evaluate a metric expression.The expressions can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions.
        /// </summary>
        public class MathEvalAniso : IField
        {
            public MathEvalAniso()
            {
                m11 = "";
                m12 = "";
                m13 = "";
                m22 = "";
                m23 = "";
                m33 = "";
            }

            /// <summary>
            /// element 11 of the metric tensor.
            /// </summary>
            public string m11 { get; set; }

            /// <summary>
            /// element 12 of the metric tensor.
            /// </summary>
            public string m12 { get; set; }

            /// <summary>
            /// element 13 of the metric tensor.
            /// </summary>
            public string m13 { get; set; }

            /// <summary>
            /// element 22 of the metric tensor.
            /// </summary>
            public string m22 { get; set; }

            /// <summary>
            /// element 23 of the metric tensor.
            /// </summary>
            public string m23 { get; set; }

            /// <summary>
            /// element 33 of the metric tensor.
            /// </summary>
            public string m33 { get; set; }

            public List<IField> Fields { get; set; }

            public override void ApplyField()
            {
                if (Fields != null)
                {
                    string temp;
                    IField f;
                    for (int i = 0; i < Fields.Count; i++)
                    {
                        f = Fields[i];
                        f.ApplyField();
                        temp = m11.Replace("F" + i, "F" + f.Tag);
                        m11 = temp;
                        temp = m12.Replace("F" + i, "F" + f.Tag);
                        m12 = temp;
                        temp = m13.Replace("F" + i, "F" + f.Tag);
                        m13 = temp;
                        temp = m22.Replace("F" + i, "F" + f.Tag);
                        m22 = temp;
                        temp = m23.Replace("F" + i, "F" + f.Tag);
                        m23 = temp;
                        temp = m33.Replace("F" + i, "F" + f.Tag);
                        m33 = temp;
                    }
                }

                fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("MathEvalAniso");
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionString(fieldTag, "m11", m11);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionString(fieldTag, "m12", m12);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionString(fieldTag, "m13", m13);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionString(fieldTag, "m22", m22);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionString(fieldTag, "m23", m23);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionString(fieldTag, "m33", m33);
            }

            public override string ToString()
            {
                return "IMathEvalAniso-Field";
            }
        }

        /// <summary>
        /// Take the maximum value of a list of fields.
        /// </summary>
        public class Max : IField
        {
            public Max()
            {
                FieldsList = null;
            }

            /// <summary>
            /// Field indices
            /// </summary>
            public List<IField> FieldsList { get; set; }

            public override void ApplyField()
            {
                if (FieldsList != null)
                {
                    double[] fTags = new double[FieldsList.Count];
                    IField field;
                    for(int i=0; i<FieldsList.Count; i++)
                    {
                        field = FieldsList[i];
                        field.ApplyField();
                        fTags[i] = field.Tag;
                    }
                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Max");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "FieldsList", fTags);
                }
            }

            public override string ToString()
            {
                return "IMax-Field";
            }
        }

        /// <summary>
        /// Compute the maximum eigenvalue of the Hessian matrix of Field[IField], with the gradients evaluated by finite differences:
        /// F = max(eig(grad(grad(Field[IField]))))
        /// </summary>
        public class MaxEigenHessian : IField
        {
            public MaxEigenHessian()
            {
                Delta = 0;
                IField = null;
            }

            /// <summary>
            /// Step used for the finite differences
            /// </summary>
            public double Delta { get; set; }

            /// <summary>
            /// Field
            /// </summary>
            public IField IField { get; set; }

            public override void ApplyField()
            {
                if (IField == null)
                {
                    IField.ApplyField();
                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("MaxEigenHessian");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Delta", Delta);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "IField", IField.Tag);
                }
            }

            public override string ToString()
            {
                return "IMaxEigenHessian-Field";
            }
        }

        /// <summary>
        /// Simple smoother:
        /// F = (G(x+delta,y,z) + G(x-delta, y, z) +
        /// G(x, y+delta, z) + G(x, y-delta, z) +
        /// G(x, y, z+delta) + G(x, y, z-delta) +
        /// G(x, y, z)) / 7,
        /// where G = Field[IField]
        /// </summary>
        public class Mean : IField
        {
            public Mean()
            {
                Delta = 0.0003464101615137755;
                IField = null;
            }

            /// <summary>
            /// Distance used to compute the mean value
            /// </summary>
            public double Delta { get; set; }

            /// <summary>
            /// Field
            /// </summary>
            public IField IField { get; set; }

            public override void ApplyField()
            {
                if (IField != null)
                {
                    IField.ApplyField();
                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Mean");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Delta", Delta);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "IField", IField.Tag);
                }
            }

            public override string ToString()
            {
                return "IMean-Field";
            }
        }

        /// <summary>
        /// Take the minimum value of a list of fields.
        /// </summary>
        public class Min : IField
        {
            public Min()
            {
                FieldsList = null;
            }

            /// <summary>
            /// Field indices
            /// </summary>
            public List<IField> FieldsList { get; set; }

            public override void ApplyField()
            {
                if (FieldsList != null)
                {
                    double[] fTags = new double[FieldsList.Count];
                    IField field;
                    for(int i=0; i<FieldsList.Count; i++)
                    {
                        field= FieldsList[i];
                        field.ApplyField();
                        fTags[i] = field.Tag;
                    }

                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Min");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "FieldsList", fTags);
                }
            }

            public override string ToString()
            {
                return "IMin-Field";
            }
        }

        /// <summary>
        /// Take the intersection of a list of possibly anisotropic fields.
        /// </summary>
        public class MinAniso : IField
        {
            public MinAniso()
            {
                FieldsList = null;
            }

            /// <summary>
            /// Fields
            /// </summary>
            public List<IField> FieldsList { get; set; }

            public override void ApplyField()
            {
                if (FieldsList!=null) {
                    double[] fTags = new double[FieldsList.Count];
                    IField field;
                    for(int i=0; i<FieldsList.Count; i++)
                    {
                        field = FieldsList[i];
                        field.ApplyField();
                        fTags[i] = field.Tag;
                    }
                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("MinAniso");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "FieldsList", fTags);
                }
            }

            public override string ToString()
            {
                return "IMinAniso-Field";
            }
        }

        /// <summary>
        /// Pre compute another field on an octree to speed-up evalution 
        /// </summary>
        public class Octree : IField
        {
            public Octree()
            {
                InField = null;
            }

            /// <summary>
            /// Field to use as x coordinate.
            /// </summary>
            public IField InField { get; set; }

            public override void ApplyField()
            {
                if (InField != null)
                {
                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Octree");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "InField", InField.Tag);
                }
            }

            public override string ToString()
            {
                return "IOctree-Field";
            }
        }

        /// <summary>
        /// Evaluate Field IField in parametric coordinates:
        /// F = Field[IField] (FX, FY, FZ)
        /// See the MathEval Field help to get a description of valid FX, FY and FZ expressions.
        /// </summary>
        public class Param : IField
        {
            public Param()
            {
                FX = "";
                FY = "";
                FZ = "";
            }

            /// <summary>
            /// X component of parametric function
            /// </summary>
            public string FX { get; set; }

            /// <summary>
            /// Y component of parametric function
            /// </summary>
            public string FY { get; set; }

            /// <summary>
            /// Z component of parametric function
            /// </summary>
            public string FZ { get; set; }

            /// <summary>
            /// Field index
            /// </summary>
            public IField IField { get; set; }

            public override void ApplyField()
            {
                if (IField != null)
                {
                    IField.ApplyField();

                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Param");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "IField", IField.Tag);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionString(fieldTag, "FX", FX);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionString(fieldTag, "FY", FY);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionString(fieldTag, "FZ", FZ);
                }
            }

            public override string ToString()
            {
                return "IParam-Field";
            }
        }

        /// <summary>
        /// Evaluate the post processing view IView.
        /// </summary>
        public class PostView : IField
        {
            /// <summary>
            /// return LC_MAX instead of a negative value (this option is needed for backward compatibility with the BackgroundMesh option
            /// </summary>
            [DefaultValue(true)]
            bool CropNegativeValues { get; set; }

            /// <summary>
            /// Post-processing view index
            /// </summary>
            [DefaultValue(0)]
            int IView { get; set; }

            /// <summary>
            /// Post-processing view tag
            /// </summary>
            [DefaultValue(-1)]
            int ViewTag { get; set; }

            public override void ApplyField()
            {
                fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("PostView");
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "CropNegativeValues", Convert.ToInt32(CropNegativeValues));
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "IView", IView);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "ViewTag", ViewTag);
            }

            public override string ToString()
            {
                return "IPostView-Field";
            }
        }

        /// <summary>
        /// Restrict the application of a field to a given list of geometrical points, curves, surfaces or volumes.
        /// </summary>
        public class Restrict : IField
        {
            public Restrict()
            {
                EdgesList = new double[] { };
                FacesList = new double[] { };
                RegionsList = new double[] { };
                VerticesList = new double[] { };
                IField = null;
            }

            /// <summary>
            /// Curve tags
            /// </summary>
            public double[] EdgesList { get; set; }

            /// <summary>
            /// Surface tags
            /// </summary>
            public double[] FacesList { get; set; }

            /// <summary>
            /// Field
            /// </summary>
            public IField IField { get; set; }

            /// <summary>
            /// Volume tags
            /// </summary>
            public double[] RegionsList { get; set; }

            /// <summary>
            /// Point tags
            /// </summary>
            public double[] VerticesList { get; set; }

            public override void ApplyField()
            {
                if (IField != null)
                {
                    IField.ApplyField();
                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Restrict");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "EdgesList", EdgesList);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "FacesList", FacesList);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "IField", IField.Tag);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "RegionsList", RegionsList);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumbers(fieldTag, "VerticesList", VerticesList);
                }
            }

            public override string ToString()
            {
                return "IRestrict-Field";
            }
        }

        /// <summary>
        /// Linearly interpolate between data provided on a 3D rectangular structured grid.
        /// The format of the input file is:
        /// Ox Oy Oz
        /// Dx Dy Dz
        /// nx ny nz
        /// v(0,0,0) v(0,0,1) v(0,0,2) ...
        /// v(0,1,0) v(0,1,1) v(0,1,2) ...
        /// v(0,2,0) v(0,2,1) v(0,2,2) ...
        /// ... ... ...
        /// v(1,0,0) ... ...
        /// where O are the coordinates of the first node, D are the distances between nodes in each direction, n are the numbers of nodes in each direction, and v are the values on each node.
        /// </summary>
        public class Structured : IField
        {
            /// <summary>
            /// Name of the input file
            /// </summary>
            [DefaultValue("")]
            string FileName { get; set; }

            /// <summary>
            /// Value of the field outside the grid(only used if the "SetOutsideValue" option is true).
            /// </summary>
            [DefaultValue(0)]
            double OutsideValue { get; set; }

            /// <summary>
            /// True to use the "OutsideValue" option.If False, the last values of the grid are used.
            /// </summary>
            [DefaultValue(false)]
            bool SetOutsideValue { get; set; }

            /// <summary>
            /// True for ASCII input files, false for binary files(4 bite signed integers for n, double precision floating points for v, D and O)
            /// </summary>
            [DefaultValue(false)]
            bool TextFormat { get; set; }

            public override void ApplyField()
            {
                fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Structured");
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionString(fieldTag, "FileName", FileName);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "OutsideValue", OutsideValue);
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "SetOutsideValue", Convert.ToInt32(SetOutsideValue));
                IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "TextFormat", Convert.ToInt32(TextFormat));
            }

            public override string ToString()
            {
                return "IStructured-Field";
            }
        }

        /// <summary>
        /// F = LCMin if Field[IField] <= DistMin,
        /// F = LCMax if Field[IField] >= DistMax,
        /// F = interpolation between LcMin and LcMax if DistMin<Field[IField] < DistMax
        /// </summary>
        public class Threshold : IField
        {
            public Threshold()
            {
                DistMax = 10;
                DistMin = 1;
                IField = null;
                LcMax = 1;
                LcMin = 0.1;
                Sigmoid = false;
                StopAtDistMax = false;
            }

            /// <summary>
            /// Distance from entity after which element size will be LcMax
            /// </summary>
            public double DistMax { get; set; }

            /// <summary>
            /// Distance from entity up to which element size will be LcMin
            /// </summary>
            public double DistMin { get; set; }

            /// <summary>
            /// Field to evaluate
            /// </summary>
            public IField IField { get; set; }

            /// <summary>
            /// Element size outside DistMax
            /// </summary>
            public double LcMax { get; set; }

            /// <summary>
            /// Element size inside DistMin
            /// </summary>
            public double LcMin { get; set; }

            /// <summary>
            /// True to interpolate between LcMin and LcMax using a sigmoid, false to interpolate linearly
            /// </summary>
            public bool Sigmoid { get; set; }

            /// <summary>
            /// True to not impose element size outside DistMax(i.e., F = a very big value if Field[IField] > DistMax)
            /// </summary>
            public bool StopAtDistMax { get; set; }

            public override void ApplyField()
            {
                if (IField!=null) {
                    IField.ApplyField();

                    fieldTag = IKernel.IMeshingKernel.IBuilder.AddMeshField("Threshold");
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "DistMax", DistMax);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "DistMin", DistMin);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "IField", IField.Tag);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "LcMax", LcMax);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "LcMin", LcMin);
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "Sigmoid", Convert.ToInt32(Sigmoid));
                    IKernel.IMeshingKernel.IBuilder.SetMeshFieldOptionNumber(fieldTag, "StopAtDistMax", Convert.ToInt32(StopAtDistMax));
                }
            }
            public override string ToString()
            {
                return "IThreshold-Field";
            }
        }

    }
}
