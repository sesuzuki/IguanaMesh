using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using Rhino.Render;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers.IExtensions
{
    public abstract class IguanaGmshField : IGH_Goo
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
            if (typeof(T).IsAssignableFrom(typeof(IguanaGmshField)))
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
        public class  AttractorAnisoCurve : IguanaGmshField
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
                    fieldTag = IguanaGmsh.Model.MeshField.Add("AttractorAnisoCurve");
                    IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "EdgesList", EdgesList);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "NNodesByEdge", NNodesByEdge);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "dMax", dMax);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "dMin", dMin);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "lMaxNormal", lMaxNormal);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "lMaxTangent", lMaxTangent);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "lMinNormal", lMinNormal);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "lMinTangent", lMinTangent);
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
        public class AutomaticMeshSizeField : IguanaGmshField
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
                fieldTag = IguanaGmsh.Model.MeshField.Add("AutomaticMeshSizeField");
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "NRefine", NRefine);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "gradientMax", gradientMax);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "hBulk", hBulk);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "nPointsPerCircle", nPointsPerCircle);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "nPointsPerGap", nPointsPerGap);
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
        public class Ball : IguanaGmshField
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
                fieldTag = IguanaGmsh.Model.MeshField.Add("Ball");
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Radius", Radius);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Thickness", Thickness);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "VIn", VIn);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "VOut", VOut);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "XCenter", XCenter);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "YCenter", YCenter);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "ZCenter", ZCenter);
            }

            public override string ToString()
            {
                return "IBall-Field";
            }
        }

        /// <summary>
        /// hwall* ratio^(dist/hwall)
        /// </summary>
        public class BoundaryLayer : IguanaGmshField
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
                fieldTag = IguanaGmsh.Model.MeshField.Add("BoundaryLayer");
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "AnisoMax", AnisoMax);
                IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "EdgesList ", EdgesList); 
                IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "ExcludedFaceList", ExcludedFaceList);
                IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "FanNodesList", FanNodesList);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "IntersectMetrics", Convert.ToInt32(IntersectMetrics));
                IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "NodesList", NodesList);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Quads", Convert.ToInt32(Quads));
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "hfar", hfar);
                if(hwall_n_nodes.Length>1) IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "hwall_n_nodes", hwall_n_nodes);
                else if (hwall_n_nodes.Length == 1) IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "hwall_n", hwall_n_nodes[0]);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "ratio", ratio);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "thickness", thickness);
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
        public class Box : IguanaGmshField
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
                fieldTag = IguanaGmsh.Model.MeshField.Add("Box");
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Thickness", Thickness);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "VIn", VIn);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "VOut", VOut);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "XMax", XMax);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "XMin", XMin);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "YMax", YMax);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "YMin", YMin);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "ZMax", ZMax);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "ZMin", ZMin);
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
        public class Curvature : IguanaGmshField
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
            public IguanaGmshField IField { get; set; }

            public override void ApplyField()
            {
                if (IField != null)
                {
                    IField.ApplyField();
                    fieldTag = IguanaGmsh.Model.MeshField.Add("Curvature");
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Delta", Delta);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "IField", IField.Tag);
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
        public class Cylinder : IguanaGmshField
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
                fieldTag = IguanaGmsh.Model.MeshField.Add("Cylinder");
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Radius", Radius);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "VIn", VIn);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "VOut", VOut);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "XAxis", XAxis);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "XCenter", XCenter);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "YAxis", YAxis);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "YCenter", YCenter);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "ZAxis", ZAxis);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "ZCenter", ZCenter);
            }

            public override string ToString()
            {
                return "ICylinder-Field";
            }
        }

        /// <summary>
        /// Compute the distance from the nearest node in a list. It can also be used to compute the distance from curves, in which case each curve is replaced by NNodesByEdge equidistant nodes and the distance from those nodes is computed.
        /// </summary>
        public class Distance : IguanaGmshField
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
            public IguanaGmshField FieldX { get; set; }

            /// <summary>
            /// Id of the field to use as y coordinate.
            /// </summary>
            public IguanaGmshField FieldY { get; set; }

            /// <summary>
            /// Id of the field to use as z coordinate.
            /// </summary>
            public IguanaGmshField FieldZ { get; set; }

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
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "FieldX", FieldX.Tag);
                    flag = true;
                }
                if (FieldY != null)
                {
                    FieldY.ApplyField();
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "FieldY", FieldY.Tag);
                    flag = true;
                }
                if (FieldZ != null)
                {
                    FieldZ.ApplyField();
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "FieldZ", FieldZ.Tag);
                    flag = true;
                }
                if (flag)
                {
                    fieldTag = IguanaGmsh.Model.MeshField.Add("Distance");
                    IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "EdgesList", EdgesList);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "NNodesByEdge", NNodesByEdge);
                    IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "NodesList", NodesList);
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
        public class Frustum : IguanaGmshField
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
                fieldTag = IguanaGmsh.Model.MeshField.Add("Frustum");
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "R1_inner", R1_inner);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "R1_outer", R1_outer);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "R2_inner", R2_inner);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "R2_outer", R2_outer);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "V1_inner", V1_inner);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "V1_outer", V1_outer);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "V2_inner", V2_inner);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "V2_outer", V2_outer);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "X1", X1);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "X2", X2);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Y1", Y1);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Y2", Y2);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Z1", Z1);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Z2", Z2);
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
        public class Gradient : IguanaGmshField
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
            public IguanaGmshField IField { get; set; }

            /// <summary>
            /// Component of the gradient to evaluate: 0 for X, 1 for Y, 2 for Z, 3 for the norm
            /// </summary>
            public int Kind { get; set; }

            public override void ApplyField()
            {
                if (IField != null) {
                    IField.ApplyField();
                    fieldTag = IguanaGmsh.Model.MeshField.Add("Gradient");
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Delta", Delta);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "IField", IField.Tag);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Kind", Kind);
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
        public class IntersectAniso : IguanaGmshField
        {
            public IntersectAniso()
            {
                FieldsList = null;
            }

            /// <summary>
            /// Fields
            /// </summary>
            public List<IguanaGmshField> FieldsList { get; set; }

            public override void ApplyField()
            {
                if (FieldsList != null)
                {
                    double[] fTags = new double[FieldsList.Count];
                    IguanaGmshField f;
                    for(int i=0; i<FieldsList.Count; i++)
                    {
                        f = FieldsList[i];
                        f.ApplyField();
                        fTags[i] = f.Tag;
                    }

                    fieldTag = IguanaGmsh.Model.MeshField.Add("IntersectAniso");
                    IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "FieldsList", fTags);
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
        public class Laplacian :IguanaGmshField
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
            public IguanaGmshField IField { get; set; }

            public override void ApplyField()
            {
                if (IField != null)
                {
                    IField.ApplyField();
                    fieldTag = IguanaGmsh.Model.MeshField.Add("Laplacian");
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Delta", Delta);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "IField", IField.Tag);
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
        public class LonLat : IguanaGmshField
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
            public IguanaGmshField IField { get; set; }

            /// <summary>
            /// radius of the sphere of the stereograpic coordinates
            /// </summary>
            public double RadiusStereo { get; set; }

            public override void ApplyField()
            {
                if (IField != null)
                {
                    IField.ApplyField();
                    fieldTag = IguanaGmsh.Model.MeshField.Add("LonLat");
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "FromStereo", Convert.ToInt32(FromStereo));
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "IField", IField.Tag);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "RadiusStereo", RadiusStereo);
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
        public class MathEval : IguanaGmshField
        {
            public MathEval()
            {
                F = "";
                Fields = new List<IguanaGmshField>();
            }

            /// <summary>
            /// Mathematical function to evaluate.
            /// </summary>
            public string F { get; set; }

            public List<IguanaGmshField> Fields { get; set; }

            public override void ApplyField()
            {
                if (Fields != null)
                {
                    string temp;
                    IguanaGmshField f;
                    for(int i=0; i<Fields.Count; i++)
                    {
                        f = Fields[i];
                        f.ApplyField();
                        temp = F.Replace("F" + i, "F" + f.Tag);
                        F = temp;
                    }
                }

                fieldTag = IguanaGmsh.Model.MeshField.Add("MathEval");
                IguanaGmsh.Model.MeshField.SetString(fieldTag, "F", F);
            }

            public override string ToString()
            {
                return "IMath-Field";
            }
        }

        /// <summary>
        /// Evaluate a metric expression.The expressions can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and and mathematical functions.
        /// </summary>
        public class MathEvalAniso : IguanaGmshField
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

            public List<IguanaGmshField> Fields { get; set; }

            public override void ApplyField()
            {
                if (Fields != null)
                {
                    string temp;
                    IguanaGmshField f;
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

                fieldTag = IguanaGmsh.Model.MeshField.Add("MathEvalAniso");
                IguanaGmsh.Model.MeshField.SetString(fieldTag, "m11", m11);
                IguanaGmsh.Model.MeshField.SetString(fieldTag, "m12", m12);
                IguanaGmsh.Model.MeshField.SetString(fieldTag, "m13", m13);
                IguanaGmsh.Model.MeshField.SetString(fieldTag, "m22", m22);
                IguanaGmsh.Model.MeshField.SetString(fieldTag, "m23", m23);
                IguanaGmsh.Model.MeshField.SetString(fieldTag, "m33", m33);
            }

            public override string ToString()
            {
                return "IMathEvalAniso-Field";
            }
        }

        /// <summary>
        /// Take the maximum value of a list of fields.
        /// </summary>
        public class Max : IguanaGmshField
        {
            public Max()
            {
                FieldsList = null;
            }

            /// <summary>
            /// Field indices
            /// </summary>
            public List<IguanaGmshField> FieldsList { get; set; }

            public override void ApplyField()
            {
                if (FieldsList != null)
                {
                    double[] fTags = new double[FieldsList.Count];
                    IguanaGmshField field;
                    for(int i=0; i<FieldsList.Count; i++)
                    {
                        field = FieldsList[i];
                        field.ApplyField();
                        fTags[i] = field.Tag;
                    }
                    fieldTag = IguanaGmsh.Model.MeshField.Add("Max");
                    IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "FieldsList", fTags);
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
        public class MaxEigenHessian : IguanaGmshField
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
            public IguanaGmshField IField { get; set; }

            public override void ApplyField()
            {
                if (IField == null)
                {
                    IField.ApplyField();
                    fieldTag = IguanaGmsh.Model.MeshField.Add("MaxEigenHessian");
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Delta", Delta);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "IField", IField.Tag);
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
        public class Mean : IguanaGmshField
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
            public IguanaGmshField IField { get; set; }

            public override void ApplyField()
            {
                if (IField != null)
                {
                    IField.ApplyField();
                    fieldTag = IguanaGmsh.Model.MeshField.Add("Mean");
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Delta", Delta);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "IField", IField.Tag);
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
        public class Min : IguanaGmshField
        {
            public Min()
            {
                FieldsList = null;
            }

            /// <summary>
            /// Field indices
            /// </summary>
            public List<IguanaGmshField> FieldsList { get; set; }

            public override void ApplyField()
            {
                if (FieldsList != null)
                {
                    double[] fTags = new double[FieldsList.Count];
                    IguanaGmshField field;
                    for(int i=0; i<FieldsList.Count; i++)
                    {
                        field= FieldsList[i];
                        field.ApplyField();
                        fTags[i] = field.Tag;
                    }

                    fieldTag = IguanaGmsh.Model.MeshField.Add("Min");
                    IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "FieldsList", fTags);
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
        public class MinAniso : IguanaGmshField
        {
            public MinAniso()
            {
                FieldsList = null;
            }

            /// <summary>
            /// Fields
            /// </summary>
            public List<IguanaGmshField> FieldsList { get; set; }

            public override void ApplyField()
            {
                if (FieldsList!=null) {
                    double[] fTags = new double[FieldsList.Count];
                    IguanaGmshField field;
                    for(int i=0; i<FieldsList.Count; i++)
                    {
                        field = FieldsList[i];
                        field.ApplyField();
                        fTags[i] = field.Tag;
                    }
                    fieldTag = IguanaGmsh.Model.MeshField.Add("MinAniso");
                    IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "FieldsList", fTags);
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
        public class Octree : IguanaGmshField
        {
            public Octree()
            {
                InField = null;
            }

            /// <summary>
            /// Field to use as x coordinate.
            /// </summary>
            public IguanaGmshField InField { get; set; }

            public override void ApplyField()
            {
                if (InField != null)
                {
                    fieldTag = IguanaGmsh.Model.MeshField.Add("Octree");
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "InField", InField.Tag);
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
        public class Param : IguanaGmshField
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
            public IguanaGmshField IField { get; set; }

            public override void ApplyField()
            {
                if (IField != null)
                {
                    IField.ApplyField();

                    fieldTag = IguanaGmsh.Model.MeshField.Add("Param");
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "IField", IField.Tag);
                    IguanaGmsh.Model.MeshField.SetString(fieldTag, "FX", FX);
                    IguanaGmsh.Model.MeshField.SetString(fieldTag, "FY", FY);
                    IguanaGmsh.Model.MeshField.SetString(fieldTag, "FZ", FZ);
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
        public class PostView : IguanaGmshField
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
                fieldTag = IguanaGmsh.Model.MeshField.Add("PostView");
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "CropNegativeValues", Convert.ToInt32(CropNegativeValues));
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "IView", IView);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "ViewTag", ViewTag);
            }

            public override string ToString()
            {
                return "IPostView-Field";
            }
        }

        /// <summary>
        /// Restrict the application of a field to a given list of geometrical points, curves, surfaces or volumes.
        /// </summary>
        public class Restrict : IguanaGmshField
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
            public IguanaGmshField IField { get; set; }

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
                    fieldTag = IguanaGmsh.Model.MeshField.Add("Restrict");
                    IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "EdgesList", EdgesList);
                    IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "FacesList", FacesList);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "IField", IField.Tag);
                    IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "RegionsList", RegionsList);
                    IguanaGmsh.Model.MeshField.SetNumbers(fieldTag, "VerticesList", VerticesList);
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
        public class Structured : IguanaGmshField
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
                fieldTag = IguanaGmsh.Model.MeshField.Add("Structured");
                IguanaGmsh.Model.MeshField.SetString(fieldTag, "FileName", FileName);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "OutsideValue", OutsideValue);
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "SetOutsideValue", Convert.ToInt32(SetOutsideValue));
                IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "TextFormat", Convert.ToInt32(TextFormat));
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
        public class Threshold : IguanaGmshField
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
            public IguanaGmshField IField { get; set; }

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

                    fieldTag = IguanaGmsh.Model.MeshField.Add("Threshold");
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "DistMax", DistMax);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "DistMin", DistMin);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "IField", IField.Tag);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "LcMax", LcMax);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "LcMin", LcMin);
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "Sigmoid", Convert.ToInt32(Sigmoid));
                    IguanaGmsh.Model.MeshField.SetNumber(fieldTag, "StopAtDistMax", Convert.ToInt32(StopAtDistMax));
                }
            }
            public override string ToString()
            {
                return "IThreshold-Field";
            }
        }

    }
}
