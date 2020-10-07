using Iguana.IguanaMesh.IUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers
{
    public static partial class IguanaGmsh
    {
        public static partial class Model
        {
            public static partial class GeoOCC
            {
                /// <summary>
                /// Add a geometrical point in the OpenCASCADE CAD representation, at coordinates(`x', `y', `z').
                /// Return the tag of the point. 
                /// (Note that the point will be added in the current model only after `synchronize' is called. This behavior holds for all the entities added in the occ module.)
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="meshSize"> If `meshSize' is > 0, add a meshing constraint at that point. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddPoint(double x, double y, double z, double meshSize = -1, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddPoint(x, y, z, meshSize, tag, ref _ierr);
                }

                /// <summary>
                /// Add a straight line segment between the two points with tags `startTag' and `endTag'.
                /// </summary>
                /// <param name="startTag"> Start point tag </param>
                /// <param name="endTag"> End point tag </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddLine(int startTag, int endTag, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddLine(startTag, endTag, tag, ref _ierr);

                }

                /// <summary>
                /// Add a circle arc between the two points with tags `startTag' and `endTag', with center `centerTag'.
                /// Return the tag of the circle arc.
                /// </summary>
                /// <param name="startTag"> Start point tag </param>
                /// <param name="centerTag"> Center point tag </param>
                /// <param name="endTag"> End point tag </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddCircleArc(int startTag, int centerTag, int endTag, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddCircleArc(startTag, centerTag, endTag, tag, ref _ierr);
                }

                /// <summary>
                /// Add a circle of center (`x', `y', `z') and radius `r'. 
                /// If `angle1' and `angle2' are specified, create a circle arc between the two angles. 
                /// Return the tag of the circle.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="r"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="angle1"></param>
                /// <param name="angle2"></param>
                /// <returns></returns>
                public static int AddCircleArc(double x, double y, double z, double r, double angle1 = -1, double angle2 = -1, int tag = -1)
                {
                    return IWrappers.gmshModelOccAddCircle(x, y, z, r, tag, angle1, angle2, ref _ierr);
                }

                /// <summary>
                /// Add an ellipse arc between the two points `startTag' and `endTag', with center `centerTag' and major axis point `majorTag'. 
                /// Return the tag of the ellipse arc. 
                /// (Note that OpenCASCADE does not allow creating ellipse arcs with the major radius smaller than the minor radius)
                /// </summary>
                /// <param name="startTag"></param>
                /// <param name="centerTag"></param>
                /// <param name="majorTag"></param>
                /// <param name="endTag"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddEllipseArc(int startTag, int centerTag, int majorTag, int endTag, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddEllipseArc(startTag, centerTag, majorTag, endTag, tag, ref _ierr);
                }

                /// <summary>
                /// Add an ellipse of center (`x', `y', `z') and radii `r1' and `r2' along the x- and y-axes respectively.
                /// If `angle1' and `angle2' are specified, create an ellipse arc between the two angles.
                /// Return the tag of the ellipse.
                /// (Note that OpenCASCADE does not allow creating ellipses with the major radius (along the x-axis) smaller than or equal to the minor radius (along the y-axis): rotate the shape or use `addCircle' in such cases)
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="r1"></param>
                /// <param name="r2"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <param name="angle1"></param>
                /// <param name="angle2"></param>
                /// <returns></returns>
                public static int AddEllipse(double x, double y, double z, double r1, double r2, double angle1 = -1, double angle2 = -1, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddEllipse(x, y, z, r1, r2, tag, angle1, angle2, ref _ierr);
                }

                /// <summary>
                /// Add a spline (C2 b-spline) curve going through the points `pointTags'.
                /// Create a periodic curve if the first and last points are the same.
                /// Return the tag of the spline curve.
                /// </summary>
                /// <param name="pointTags"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddSpline(int[] pointTags, int tag = -1)
                {
                    return IWrappers.gmshModelOccAddSpline(pointTags, pointTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a b-spline curve of degree `degree' with `pointTags' control points.
                /// If `weights', `knots' or `multiplicities' are not provided, default parameters are computed automatically.
                /// Create a periodic curve if the first and last points are the same. 
                /// Return the tag of the b-spline
                /// </summary>
                /// <param name="pointTags"></param>
                /// <param name="degree"></param>
                /// <param name="weights"></param>
                /// <param name="knots"></param>
                /// <param name="multiplicities"> </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddBSpline(int[] pointTags, int degree, double[] weights = default, double[] knots = default, int[] multiplicities = default, int tag = -1)
                {
                    if (weights == default) weights = new double[0];
                    if (knots == default) knots = new double[0];
                    if (multiplicities == default) multiplicities = new int[0];
                    return IWrappers.GmshModelOccAddBSpline(pointTags, pointTags.LongLength, tag, degree, weights, weights.LongLength, knots, knots.LongLength, multiplicities, multiplicities.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add a Bezier curve with `pointTags' control points.
                /// Return the tag of the Bezier curve.
                /// </summary>
                /// <param name="pointTags"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddBezier(int[] pointTags, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddBezier(pointTags, pointTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a wire (open or closed) formed by the curves `curveTags'.
                /// Return the tag of the wire.
                /// Note that an OpenCASCADE wire can be made of curves that share geometrically identical(but topologically different) points.
                /// </summary>
                /// <param name="curveTags"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="checkClosed"></param>
                /// <returns></returns>
                public static int AddWire(int[] curveTags, bool checkClosed = false, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddWire(curveTags, curveTags.LongLength, tag, Convert.ToInt32(checkClosed), ref _ierr);
                }

                /// <summary>
                /// Add a curve loop (a closed wire) formed by the curves `curveTags'.
                /// Note that an OpenCASCADE curve loop can be made of curves that share geometrically identical(but topologically different) points.
                /// Return the tag of the curve loop.
                /// </summary>
                /// <param name="curveTags"> `curveTags' should contain tags of curves forming a closed loop. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <returns></returns>
                public static int AddCurveLoop(int[] curveTags, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddCurveLoop(curveTags, curveTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a rectangle with lower left corner at (`x', `y', `z') and upper right corner at (`x' + `dx', `y' + `dy', `z'). 
                /// Return the tag of the rectangle.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="tag"></param>
                /// <param name="roundedRadius"> Round the corners if `roundedRadius' is nonzero .</param>
                /// <returns></returns>
                public static int AddRectangle(double x, double y, double z, double dx, double dy, int tag = -1, double roundedRadius = 0)
                {
                    return IWrappers.GmshModelOccAddRectangle(x, y, z, dx, dy, tag, roundedRadius, ref _ierr);
                }

                /// <summary>
                /// Add a disk with center (`xc', `yc', `zc') and radius `rx' along the x-axis and `ry' along the y-axis. 
                /// Return the tag of the disk.
                /// </summary>
                /// <param name="xc"></param>
                /// <param name="yc"></param>
                /// <param name="zc"></param>
                /// <param name="rx"></param>
                /// <param name="ry"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <returns></returns>
                public static int AddDisk(double xc, double yc, double zc, double rx, double ry, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddDisk(xc, yc, zc, rx, ry, tag, ref _ierr);
                }


                /// <summary>
                /// Add a plane surface defined by one or more curve loops (or closed wires) `wireTags'.  
                /// Return the tag of the surface.
                /// </summary>
                /// <param name="wireTags"> The first curve loop defines the exterior contour; additional curve loop define holes. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <returns></returns>
                public static int AddPlaneSurface(int[] wireTags, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddPlaneSurface(wireTags, wireTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a surface filling the curve loop `wireTag'. 
                /// Return the tag of the surface.
                /// </summary>
                /// <param name="wireTag"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <param name="pointTags"> If `pointTags' are provided, force the surface to pass through the given points. </param>
                /// <returns></returns>
                public static int AddSurfaceFilling(int wireTag, int[] pointTags = default, int tag = -1)
                {
                    if (pointTags == default) pointTags = new int[0];
                    return IWrappers.GmshModelOccAddSurfaceFilling(wireTag, tag, pointTags, pointTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add a BSpline surface filling the curve loop `wireTag'. 
                /// The curve loop should be made of 2, 3 or 4 BSpline curves. 
                /// Return the tag of the surface.
                /// </summary>
                /// <param name="wireTag"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="type"> The optional `type' argument specifies the type of filling: "Stretch" creates the flattest patch, "Curved" (the default) creates the most rounded patch, and "Coons" creates a rounded patch with less depth than "Curved".  </param>
                /// <returns></returns>
                public static int AddBSplineFilling(int wireTag, string type = "Curved", int tag=-1)
                {
                    return IWrappers.GmshModelOccAddBSplineFilling(wireTag, tag, type, ref _ierr);
                }

                /// <summary>
                /// Add a Bezier surface filling the curve loop `wireTag'. 
                /// The curve loop should be made of 2, 3 or 4 BSpline curves. 
                /// Return the tag of the surface.
                /// </summary>
                /// <param name="wireTag"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="type"> The optional `type' argument specifies the type of filling: "Stretch" creates the flattest patch, "Curved" (the default) creates the most rounded patch, and "Coons" creates a rounded patch with less depth than "Curved".  </param>
                /// <returns></returns>
                public static int AddBezierFilling(int wireTag, string type = "Curved", int tag = -1)
                {
                    return IWrappers.GmshModelOccAddBezierFilling(wireTag, tag, type, ref _ierr);
                }

                /// <summary>
                /// Add a b-spline surface of degree `degreeU' x `degreeV'
                /// If `weights', `knotsU', `knotsV', `multiplicitiesU' or `multiplicitiesV' are not provided, default parameters are computed automatically. 
                /// Return the tag of the b-spline surface.
                /// </summary>
                /// <param name="pointTags"> `pointTags' are control points given as a single vector [Pu1v1, ... Pu`numPointsU'v1, Pu1v2, ...]. </param>
                /// <param name="numPointsU"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <param name="degreeU"></param>
                /// <param name="degreeV"></param>
                /// <param name="weights"></param>
                /// <param name="knotsU"></param>
                /// <param name="knotsV"></param>
                /// <param name="multiplicitiesU"></param>
                /// <param name="multiplicitiesV"></param>
                /// <returns></returns>
                public static int AddBSplineSurface(int[] pointTags, int numPointsU, int degreeU, int degreeV, double[] weights = default, double[] knotsU = default, double[] knotsV = default, int[] multiplicitiesU = default, int[] multiplicitiesV = default, int tag = -1)
                {
                    if (weights == default) weights = new double[0];
                    if (knotsU == default) knotsU = new double[0];
                    if (knotsV == default) knotsV = new double[0];
                    if (multiplicitiesU == default) multiplicitiesU = new int[0];
                    if (multiplicitiesV == default) multiplicitiesV = new int[0];
                    return IWrappers.GmshModelOccAddBSplineSurface(pointTags, pointTags.LongLength, numPointsU, tag, degreeU, degreeV, weights, weights.LongLength, knotsU, knotsU.LongLength, knotsV, knotsV.LongLength, multiplicitiesU, multiplicitiesU.LongLength, multiplicitiesV, multiplicitiesV.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add a Bezier surface.
                /// Return the tag of the b-spline surface.
                /// </summary>
                /// <param name="pointTags"> `pointTags' control points given as a single vector [Pu1v1, ... Pu`numPointsU'v1, Pu1v2, ...].  </param>
                /// <param name="numPointsU"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddBezierSurface(int[] pointTags, int numPointsU, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddBezierSurface(pointTags, pointTags.LongLength, numPointsU, tag, ref _ierr);
                }

                /// <summary>
                /// Add a surface loop (a closed shell) formed by `surfaceTags'.  
                /// Return the tag of the surface loop.
                /// </summary>
                /// <param name="surfaceTags"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <param name="sewing"> Setting `sewing' allows to build a shell made of surfaces that share geometrically identical (but topologically different) curves. </param>
                /// <returns></returns>
                public static int AddSurfaceLoop(int[] surfaceTags, bool sewing = false, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddSurfaceLoop(surfaceTags, surfaceTags.LongLength, tag, Convert.ToInt32(sewing), ref _ierr);
                }

                /// <summary>
                /// Add a volume (a region) defined by one or more surface loops `shellTags'. 
                /// Return the tag of the volume.
                /// </summary>
                /// <param name="shellTags"> The first surface loop defines the exterior boundary; additional surface loop define holes. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwisea new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddVolume(int[] shellTags, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddVolume(shellTags, shellTags.LongLength, tag, ref _ierr);
                }

                /// <summary>
                /// Add a sphere of center (`xc', `yc', `zc') and radius `r'. 
                /// The optional `angle1' and `angle2' arguments define the polar angle opening(from -Pi/2 to Pi/2). 
                /// The optional `angle3' argument defines the azimuthal opening (from 0 to 2*Pi).  
                /// Return the tag of the sphere.
                /// </summary>
                /// <param name="xc"></param>
                /// <param name="yc"></param>
                /// <param name="zc"></param>
                /// <param name="radius"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="angle1"></param>
                /// <param name="angle2"></param>
                /// <param name="angle3"></param>
                /// <returns></returns>
                public static int AddSphere(double xc, double yc, double zc, double radius, double angle1 = -Math.PI/2, double angle2 = Math.PI/2, double angle3 = 2*Math.PI, int tag=-1)
                {
                    return IWrappers.GmshModelOccAddSphere(xc, yc, zc, radius, tag, angle1, angle2, angle3, ref _ierr);
                }


                /// <summary>
                /// Add a parallelepipedic box defined by a point (`x', `y', `z') and the extents along the x-, y- and z-axes. 
                /// Return the tag of the box.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.  </param>
                /// <returns></returns>
                public static int AddBox(double x, double y, double z, double dx, double dy, double dz, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddBox(x, y, z, dx, dy, dz, tag, ref _ierr);
                }

                /// <summary>
                /// Add a cylinder, defined by the center (`x', `y', `z') of its first circular face, 
                /// the 3 components(`dx', `dy', `dz') of the vector defining its axis and its radius `r'. 
                /// The optional `angle' argument defines the angular opening (from 0 to 2*Pi). 
                /// Return the tag of the cylinder.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="r"></param>
                /// <param name="tag"></param>
                /// <param name="angle"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <returns></returns>
                public static int AddCylinder(double x, double y, double z, double dx, double dy, double dz, double r, double angle = -1, int tag =-1)
                {
                    return IWrappers.GmshModelOccAddCylinder(x, y, z, dx, dy, dz, r, tag, angle, ref _ierr);
                }

                /// <summary>
                /// Add a cone, defined by the center (`x', `y', `z') of its first circular face, 
                /// the 3 components of the vector(`dx', `dy', `dz') defining its axis
                /// and the two radii `r1' and `r2' of the faces (these radii can be zero). 
                /// Return the tag of the cone.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="r1"></param>
                /// <param name="r2"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="angle"> `angle' defines the optional angular opening (from 0 to 2*Pi).  </param>
                /// <returns></returns>
                public static int AddCone(double x, double y, double z, double dx, double dy, double dz, double r1, double r2, double angle = -1, int tag =-1)
                {
                    return IWrappers.GmshModelOccAddCone(x, y, z, dx, dy, dz, r1, r2, tag, angle, ref _ierr);
                }

                /// <summary>
                /// Add a right angular wedge, defined by the right-angle point (`x', `y', `z')
                /// and the 3 extends along the x-, y- and z-axes(`dx', `dy', `dz'). 
                /// Return the tag of the wedge.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                /// <param name="ltx"> The optional argument `ltx' defines the top extent along the x-axis. </param>
                /// <returns></returns>
                public static int AddWedge(double x, double y, double z, double dx, double dy, double dz, double ltx = -1, int tag =-1)
                {
                    return IWrappers.GmshModelOccAddWedge(x, y, z, dx, dy, dz, tag, ltx, ref _ierr);
                }

                /// <summary>
                /// Add a torus, defined by its center (`x', `y', `z') and its 2 radii `r' and `r2'. 
                /// Return the tag of the wedge.
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="r1"></param>
                /// <param name="r2"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.</param>
                /// <param name="angle"> The optional argument `angle' defines the angular opening(from 0 to 2*Pi)</param>
                /// <returns></returns>
                public static int AddTorus(double x, double y, double z, double r1, double r2, double angle = -1, int tag = -1)
                {
                    return IWrappers.GmshModelOccAddTorus(x, y, z, r1, r2, tag, angle, ref _ierr);
                }

                /* Add a volume (if the optional argument `makeSolid' is set) or surfaces
                 * defined through the open or closed wires `wireTags'.
                 *  */

                /// <summary>
                /// Add a volume (if the optional argument `makeSolid' is set) or surfaces defined through the open or closed wires `wireTags'.
                /// </summary>
                /// <param name="wireTags"></param>
                /// <param name="outDimTags"> The new entities are returned in `outDimTags'. </param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically.</param>
                /// <param name="makeSolid"></param>
                /// <param name="makeRuled"> If the optional argument `makeRuled' is set, the surfaces created on the boundary are forced to be ruled surfaces.</param>
                /// <param name="maxDegree"> If `maxDegree' is positive, set the maximal degree of resulting surface.</param>
                /// <returns></returns>
                public static void AddThruSections(int[] wireTags, out int[] outDimTags, bool makeSolid = false, bool makeRuled = true, int maxDegree = -1, int tag = -1)
                {
                    IntPtr dimTags;
                    long outDimTags_n;
                    IWrappers.GmshModelOccAddThruSections(wireTags, wireTags.LongLength, out dimTags, out outDimTags_n, tag, Convert.ToInt32(makeSolid), Convert.ToInt32(makeRuled), maxDegree, ref _ierr);

                    outDimTags = null;
                    if (outDimTags_n > 0)
                    {
                        outDimTags = new int[outDimTags_n];
                        Marshal.Copy(dimTags, outDimTags, 0, (int)outDimTags_n);
                    }

                    IWrappers.GmshFree(dimTags);
                }

                /// <summary>
                /// Add a hollowed volume built from an initial volume `volumeTag' and a set of faces from this volume `excludeSurfaceTags', which are to be removed. 
                /// The remaining faces of the volume become the walls of the hollowed solid, with thickness `offset'. 
                /// </summary>
                /// <param name="volumeTag"></param>
                /// <param name="excludeSurfaceTags"></param>
                /// <param name="offset"></param>
                /// <param name="outDimTags"></param>
                /// <param name="tag"> If `tag' is positive, set the tag explicitly; otherwise a new tag is selected automatically. </param>
                public static void AddThickSolid(int volumeTag, int[] excludeSurfaceTags, double offset, out int[] outDimTags, int tag = -1)
                {
                    IntPtr dimTags;
                    long outDimTags_n;
                    IWrappers.GmshModelOccAddThickSolid(volumeTag, excludeSurfaceTags, excludeSurfaceTags.LongLength, offset, out dimTags, out outDimTags_n, tag, ref _ierr);

                    outDimTags = null;
                    if (outDimTags_n > 0)
                    {
                        outDimTags = new int[outDimTags_n];
                        Marshal.Copy(dimTags, outDimTags, 0, (int)outDimTags_n);
                    }
                    IWrappers.GmshFree(dimTags);
                }

                /// <summary>
                /// Extrude the model entities `dimTags' by translation along (`dx', `dy', `dz'). 
                /// Return extruded entities in `outDimTags'.
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="dx"></param>
                /// <param name="dy"></param>
                /// <param name="dz"></param>
                /// <param name="outDimTags"></param>
                /// <param name="numElements"> If `numElements' is not empty, also extrude the mesh: the entries in `numElements' give the number of elements in each layer. </param>
                /// <param name="heights"> If `height' is not empty, it provides the (cumulative) height of the different layers, normalized to 1.</param>
                /// <param name="recombine"></param>
                public static void Extrude(int[] dimTags, double dx, double dy, double dz, out int[] outDimTags, int[] numElements=default, double[] heights = default, bool recombine = false)
                {
                    IntPtr out_DimTags;
                    long outDimTags_n;
                    if (numElements == default) numElements = new int[0];
                    if (heights == default) heights = new double[0];


                    IWrappers.GmshModelOccExtrude(dimTags, dimTags.LongLength, dx, dy, dz, out out_DimTags, out outDimTags_n, numElements, numElements.LongLength, heights, heights.LongLength, Convert.ToInt32(recombine), ref _ierr);
                    outDimTags = null;
                    if (outDimTags_n > 0)
                    {
                        outDimTags = new int[outDimTags_n];
                        Marshal.Copy(out_DimTags, outDimTags, 0, (int)outDimTags_n);
                    }

                    IWrappers.GmshFree(out_DimTags);
                }

                /// <summary>
                /// Remove all duplicate entities (different entities at the same geometrical location) after intersecting(using boolean fragments) all highest dimensional entities.
                /// </summary>
                public static void RemoveAllDuplicates()
                {
                    IWrappers.GmshModelOccRemoveAllDuplicates(ref _ierr);
                }


                /* Extrude the model entities `dimTags' by rotation of `angle' radians around
                 * the axis of revolution defined by the point (`x', `y', `z') and the
                 * direction (`ax', `ay', `az'). Return extruded entities in `outDimTags'. If
                 * `numElements' is not empty, also extrude the mesh: the entries in
                 * `numElements' give the number of elements in each layer.  */
                /// <summary>
                /// 
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                /// <param name="ax"></param>
                /// <param name="ay"></param>
                /// <param name="az"></param>
                /// <param name="angle">  When the mesh is extruded the angle should be strictly smaller than 2*Pi. </param>
                /// <param name="outDimTags"></param>
                /// <param name="numElements"></param>
                /// <param name="heights"> If `height' is not empty, it provides the(cumulative) height of the different layers, normalized to 1. </param>
                /// <param name="recombine"></param>
                public static void Revolve(int[] dimTags, double x, double y, double z, double ax, double ay, double az, double angle, out int[] outDimTags, int[] numElements, double[] heights = null, bool recombine = true)
                {
                    IntPtr out_DimTags;
                    long outDimTags_n;
                    if (numElements == default) numElements = new int[0];
                    if (heights == default) heights = new double[0];

                    IWrappers.GmshModelOccRevolve(dimTags, dimTags.LongLength, x, y, z, ax, ay, az, angle, out out_DimTags, out outDimTags_n, numElements, numElements.LongLength, heights, heights.LongLength, Convert.ToInt32(recombine), ref _ierr);
                    outDimTags = null;
                    if (outDimTags_n > 0)
                    {
                        outDimTags = new int[outDimTags_n];
                        Marshal.Copy(out_DimTags, outDimTags, 0, (int)outDimTags_n);
                    }

                    IWrappers.GmshFree(out_DimTags);
                }

                /// <summary>
                /// Add a pipe by extruding the entities `dimTags' along the wire `wireTag'.
                /// Return the pipe in `outDimTags'.
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="wireTag"></param>
                /// <param name="outDimTags"></param>
                public static void AddPipe(int[] dimTags, int wireTag, out int[] outDimTags)
                {
                    IntPtr out_dimTags;
                    long outDimTags_n;
                    IWrappers.GmshModelOccAddPipe(dimTags, dimTags.LongLength, wireTag, out out_dimTags, out outDimTags_n, ref _ierr);
                    outDimTags = null;
                    if (outDimTags_n > 0)
                    {
                        outDimTags = new int[outDimTags_n];
                        Marshal.Copy(out_dimTags, outDimTags, 0, (int)outDimTags_n);
                    }

                    IWrappers.GmshFree(out_dimTags);
                }
                /* Fillet the volumes `volumeTags' on the curves `curveTags' with radii
                 * `radii'. The `radii' vector can either contain a single radius, as many
                 * radii as `curveTags', or twice as many as `curveTags' (in which case
                 * different radii are provided for the begin and end points of the curves).
                 * Return the filleted entities in `outDimTags'. Remove the original volume if
                 * `removeVolume' is set. */
                public static void Fillet(int[] volumeTags, int[] curveTags, long curveTags_n, double[] radii, out int[] outDimTags, int removeVolume)
                {
                    IntPtr out_dimTags;
                    long outDimTags_n;
                    IWrappers.GmshModelOccFillet(volumeTags, volumeTags.LongLength, curveTags, curveTags.LongLength, radii, radii.LongLength, out out_dimTags, out outDimTags_n, removeVolume, ref _ierr);
                    outDimTags = null;
                    if (outDimTags_n > 0)
                    {
                        outDimTags = new int[outDimTags_n];
                        Marshal.Copy(out_dimTags, outDimTags, 0, (int)outDimTags_n);
                    }

                    IWrappers.GmshFree(out_dimTags);
                }

                /// <summary>
                /// Chamfer the volumes `volumeTags' on the curves `curveTags' with distances `distances' measured on surfaces `surfaceTags'. 
                /// The `distances' vector can either contain a single distance, as many distances as `curveTags' and `surfaceTags', 
                /// or twice as many as `curveTags' and `surfaceTags' (in which case the first in each pair is measured on the corresponding surface in `surfaceTags', 
                /// the other on the other adjacent surface). 
                /// Return the chamfered entities in `outDimTags'. 
                /// </summary>
                /// <param name="volumeTags"></param>
                /// <param name="curveTags"></param>
                /// <param name="surfaceTags"></param>
                /// <param name="distances"></param>
                /// <param name="outDimTags"></param>
                /// <param name="removeVolume"> Remove the original volume if `removeVolume' is set. </param>
                public static void Chamfer(int[] volumeTags, int[] curveTags, int[] surfaceTags, double[] distances, out int[] outDimTags, bool removeVolume = false)
                {
                    IntPtr out_dimTags;
                    long outDimTags_n;
                    IWrappers.GmshModelOccChamfer(volumeTags, volumeTags.LongLength, curveTags, curveTags.LongLength, surfaceTags, surfaceTags.LongLength, distances, distances.LongLength, out out_dimTags, out outDimTags_n, Convert.ToInt32(removeVolume), ref _ierr);

                    outDimTags = null;
                    if (outDimTags_n > 0)
                    {
                        outDimTags = new int[outDimTags_n];
                        Marshal.Copy(out_dimTags, outDimTags, 0, (int)outDimTags_n);
                    }

                    IWrappers.GmshFree(out_dimTags);
                }

                /// <summary>
                /// Get the center of mass of the OpenCASCADE entity of dimension `dim' and tag `tag'
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="z"></param>
                public static void GetCenterOfMass(int dim, int tag, out double x, out double y, out double z)
                {
                    IWrappers.GmshModelOccGetCenterOfMass(dim, tag, out x, out y, out z, ref _ierr);
                }

                /// <summary>
                /// Synchronize the OpenCASCADE CAD representation with the current IguanaGmsh model.
                /// This can be called at any time, but since it involves a non trivial amount of processing, the number of synchronization points should normally be minimized.
                /// </summary>
                public static void Synchronize() {
                    IWrappers.GmshModelOccSynchronize(ref _ierr);
                }

                /// <summary>
                /// Compute the boolean union (the fusion) of the entities `objectDimTags' and
                /// `toolDimTags'. Return the resulting entities in `outDimTags'. If `tag' is
                /// positive, try to set the tag explicitly(only valid if the boolean
                /// operation results in a single entity). Remove the object if `removeObject'
                /// is set.Remove the tool if `removeTool' is set. 
                /// </summary>
                /// <param name="objectDimTags"></param>
                /// <param name="toolDimTags"></param>
                /// <param name="outDimTags_out"></param>
                /// <param name="tag"></param>
                /// <param name="removeObject"></param>
                /// <param name="removeTool"></param>
                public static void Fuse(int[] objectDimTags, int[] toolDimTags, out int[] outDimTags_out, int tag=-1, bool removeObject=true, bool removeTool=true)
                {
                    IntPtr outDimTags, outDimTagsMap, outDimTagsMap_n;
                    long outDimTags_n, outDimTagsMap_nn;
                    IWrappers.GmshModelOccFuse(objectDimTags, objectDimTags.LongLength, toolDimTags, toolDimTags.LongLength, out outDimTags, out outDimTags_n, out outDimTagsMap, out outDimTagsMap_n, out outDimTagsMap_nn, tag, Convert.ToInt32(removeObject), Convert.ToInt32(removeTool), ref _ierr);

                    outDimTags_out = null;
                    if (outDimTags_n > 0)
                    {
                        outDimTags_out = new int[outDimTags_n];
                        Marshal.Copy(outDimTags, outDimTags_out, 0, (int)outDimTags_n);
                    }

                    IWrappers.GmshFree(outDimTags);
                    IWrappers.GmshFree(outDimTagsMap);
                    IWrappers.GmshFree(outDimTagsMap_n);
                }

                /// <summary>
                /// Compute the boolean intersection (the common parts) of the entities
                /// `objectDimTags' and `toolDimTags'. Return the resulting entities in
                /// `outDimTags'. If `tag' is positive, try to set the tag explicitly(only
                /// valid if the boolean operation results in a single entity). Remove the
                /// object if `removeObject' is set. Remove the tool if `removeTool' is set.
                /// </summary>
                /// <param name="objectDimTags"></param>
                /// <param name="toolDimTags"></param>
                /// <param name="outDimTags_out"></param>
                /// <param name="tag"></param>
                /// <param name="removeObject"></param>
                /// <param name="removeTool"></param>
                public static void Intersect(int[] objectDimTags, int[] toolDimTags, out int[] outDimTags_out, int tag = -1, bool removeObject = true, bool removeTool = true)
                {
                    IntPtr outDimTags, outDimTagsMap, outDimTagsMap_n;
                    long outDimTags_n, outDimTagsMap_nn;
                    IWrappers.GmshModelOccIntersect(objectDimTags, objectDimTags.LongLength, toolDimTags, toolDimTags.LongLength, out outDimTags, out outDimTags_n, out outDimTagsMap, out outDimTagsMap_n, out outDimTagsMap_nn, tag, Convert.ToInt32(removeObject), Convert.ToInt32(removeTool), ref _ierr);
                    outDimTags_out = null;
                    if (outDimTags_n > 0)
                    {
                        outDimTags_out = new int[outDimTags_n];
                        Marshal.Copy(outDimTags, outDimTags_out, 0, (int)outDimTags_n);
                    }

                    IWrappers.GmshFree(outDimTags);
                    IWrappers.GmshFree(outDimTagsMap);
                    IWrappers.GmshFree(outDimTagsMap_n);
                }

                /// <summary>
                /// Compute the boolean difference between the entities `objectDimTags' and
                /// `toolDimTags'. Return the resulting entities in `outDimTags'. If `tag' is
                /// positive, try to set the tag explicitly(only valid if the boolean
                /// operation results in a single entity). Remove the object if `removeObject'
                /// is set.Remove the tool if `removeTool' is set.
                /// </summary>
                /// <param name="objectDimTags"></param>
                /// <param name="toolDimTags"></param>
                /// <param name="outDimTags_out"></param>
                /// <param name="tag"></param>
                /// <param name="removeObject"></param>
                /// <param name="removeTool"></param>
                public static void Cut(Tuple<int, int>[] objectDimTags, Tuple<int, int>[] toolDimTags, out Tuple<int,int>[] dimTags, int tag = -1, bool removeObject = true, bool removeTool = true)
                {
                    int[] objectDimTags_flatten = IHelpers.ToIntArray(objectDimTags);
                    int[] toolDimTags_flatten = IHelpers.ToIntArray(toolDimTags);
                    IntPtr outDimTags, outDimTagsMap, outDimTagsMap_n;
                    long outDimTags_n, outDimTagsMap_nn;
                    IWrappers.GmshModelOccCut(objectDimTags_flatten, objectDimTags_flatten.LongLength, toolDimTags_flatten, toolDimTags_flatten.LongLength, out outDimTags, out outDimTags_n, out outDimTagsMap, out outDimTagsMap_n, out outDimTagsMap_nn, tag, Convert.ToInt32(removeObject), Convert.ToInt32(removeTool), ref _ierr);
                    dimTags = null;
                    if (outDimTags_n > 0)
                    {
                        var temp = new long[outDimTags_n];
                        Marshal.Copy(outDimTags, temp, 0, (int)outDimTags_n);
                        dimTags = IHelpers.ToIntPair(temp);
                    }         

                    IWrappers.GmshFree(outDimTags);
                    IWrappers.GmshFree(outDimTagsMap);
                    IWrappers.GmshFree(outDimTagsMap_n);
                }

                /// <summary>
                /// Compute the boolean fragments (general fuse) of the entities
                /// `objectDimTags' and `toolDimTags'. Return the resulting entities in
                /// `outDimTags'. If `tag' is positive, try to set the tag explicitly(only
                /// valid if the boolean operation results in a single entity). Remove the
                /// object if `removeObject' is set. Remove the tool if `removeTool' is set.
                /// </summary>
                /// <param name="objectDimTags"></param>
                /// <param name="toolDimTags"></param>
                /// <param name="outDimTags_out"></param>
                /// <param name="tag"></param>
                /// <param name="removeObject"></param>
                /// <param name="removeTool"></param>
                public static void Fragment(Tuple<int, int>[] objectDimTags, Tuple<int, int>[] toolDimTags, out Tuple<int, int>[] dimTags, int tag = -1, bool removeObject = true, bool removeTool = true)
                {
                    int[] objectDimTags_flatten = IHelpers.ToIntArray(objectDimTags);
                    int[] toolDimTags_flatten = IHelpers.ToIntArray(toolDimTags);
                    IntPtr outDimTags, outDimTagsMap, outDimTagsMap_n;
                    long outDimTags_n, outDimTagsMap_nn;
                    IWrappers.GmshModelOccFragment(objectDimTags_flatten, objectDimTags_flatten.LongLength, toolDimTags_flatten, toolDimTags_flatten.LongLength, out outDimTags, out outDimTags_n, out outDimTagsMap, out outDimTagsMap_n, out outDimTagsMap_nn, tag, Convert.ToInt32(removeObject), Convert.ToInt32(removeTool), ref _ierr);
                    dimTags = null;
                    if (outDimTags_n > 0)
                    {
                        var temp = new int[outDimTags_n];
                        Marshal.Copy(outDimTags, temp, 0, (int)outDimTags_n);
                        dimTags = IHelpers.ToIntPair(temp);
                    }

                    IWrappers.GmshFree(outDimTags);
                    IWrappers.GmshFree(outDimTagsMap);
                    IWrappers.GmshFree(outDimTagsMap_n);
                }
            }
        }
    }
}
