using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security;

namespace Iguana.IguanaMesh.IGmshWrappers
{
    public static partial class Gmsh
    {
        private const string gmsh_dll = "gmsh-4.6.dll";

        internal static class GmshWrappers
        {

            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            ////// SETUP
            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////

            #region Setup

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFree")]
            internal static extern void GmshFree(IntPtr ptr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshInitialize")]
            internal static extern void GmshInitialize(int argc, ref IntPtr argv, int readConfigFiles, ref int ierr);
            //GMSH_API void gmshInitialize(int argc, char** argv, const int readConfigFiles, int* ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFinalize")]
            internal static extern void GmshFinalize(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshOpen")]
            internal static extern void GmshOpen([MarshalAs(UnmanagedType.LPStr)] string fileName, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshWrite")]
            internal static extern void GmshWrite([MarshalAs(UnmanagedType.LPStr)] string fileName, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshClear")]
            internal static extern void GmshClear(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshOptionSetNumber")]
            internal static extern void GmshOptionSetNumber([In, Out] string name, double value, ref int ierr);
            // GMSH_API void gmshOptionSetNumber(const char* name, const double value, int* ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshOptionGetNumber")]
            internal static extern void GmshOptionGetNumber([In, Out] string name, IntPtr value, ref int ierr);

            #endregion

            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            ////// MODEL
            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////

            #region Model
            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelAdd")]
            internal static extern void GmshModelAdd([In, Out] string name, ref int ierr);
            // GMSH_API void gmshModelAdd(const char* name, int* ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelRemove")]
            internal static extern void GmshModelRemove(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetEntities")]
            internal static extern void GmshModelGetEntities(out IntPtr dimTags, out IntPtr dimTags_n, int dim, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelAddPhysicalGroup")]
            internal static extern int GmshModelAddPhysicalGroup(int dim, [In, Out] int[] tags, long tags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelSetPhysicalName")]
            internal static extern void GmshModelSetPhysicalName(int dim, int tag, [MarshalAs(UnmanagedType.LPStr)] string name, ref int ierr);

            #endregion

            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            ////// GEO
            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////

            #region Geo

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddPoint")]
            internal static extern int GmshModelGeoAddPoint(double x, double y, double z, double meshSize, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddLine")]
            internal static extern int GmshModelGeoAddLine(int startTag, int endTag, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddCircleArc")]
            internal static extern int GmshModelGeoAddCircleArc(int startTag, int centerTag, int endTag, int tag, double nx, double ny, double nz, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddEllipseArc")]
            internal static extern int GmshModelGeoAddEllipseArc(int startTag, int centerTag, int majorTag, int endTag, int tag, double nx, double ny, double nz, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddCurveLoop")]
            internal static extern int GmshModelGeoAddCurveLoop([In, Out] int[] curveTags, int curveTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddSpline")]
            internal static extern int GmshModelGeoAddSpline([In, Out] int[] pointTags, long pointTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddBSpline")]
            internal static extern int GmshModelGeoAddBSpline([In, Out] int[] pointTags, long pointTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddBezier")]
            internal static extern int GmshModelGeoAddBezier([In, Out] int[] pointTags, long pointTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddCompoundSpline")]
            internal static extern int GmshModelGeoAddCompoundSpline([In, Out] int[] curveTags, long curveTags_n, int numIntervals, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddCompoundBSpline")]
            internal static extern int GmshModelGeoAddCompoundBSpline([In, Out] int[] curveTags, long curveTags_n, int numIntervals, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddPlaneSurface")]
            internal static extern int GmshModelGeoAddPlaneSurface([In, Out] int[] wireTags, long wireTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddSurfaceFilling")]
            internal static extern int GmshModelGeoAddSurfaceFilling([In, Out] int[] wireTags, long wireTags_n, int tag, int sphereCenterTag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddSurfaceLoop")]
            internal static extern int GmshModelGeoAddSurfaceLoop([In, Out] int[] surfaceTags, long surfaceTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoAddVolume")]
            internal static extern int GmshModelGeoAddVolume([In, Out] int[] shellTags, long shellTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoExtrude")]
            internal static extern void GmshModelGeoExtrude([In, Out] int[] dimTags, long dimTags_n, double dx, double dy, double dz, out IntPtr outDimTags, out long outDimTags_n, [In, Out] int[] numElements, long numElements_n, [In,Out] double[] heights, long heights_n, int recombine, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoRevolve")]
            internal static extern void GmshModelGeoRevolve([In,Out] int[] dimTags, long dimTags_n, double x, double y, double z, double ax, double ay, double az, double angle, out IntPtr outDimTags, out long outDimTags_n, [In,Out] int[] numElements, long numElements_n, [In,Out] double[] heights, long heights_n, int recombine, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoTwist")]
            internal static extern void GmshModelGeoTwist([In,Out] int[] dimTags, long dimTags_n, double x, double y, double z, double dx, double dy, double dz, double ax, double ay, double az, double angle, out IntPtr outDimTags, out long outDimTags_n, [In,Out] int[] numElements, long numElements_n, [In,Out] double[] heights, long heights_n, int recombine, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoMeshSetTransfiniteCurve")]
            internal static extern void GmshModelGeoMeshSetTransfiniteCurve(int tag, int nPoints, string meshType, double coef, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoMeshSetTransfiniteSurface")]
            internal static extern void GmshModelGeoMeshSetTransfiniteSurface(int tag, string arrangement, int[] cornerTags, long cornerTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoMeshSetTransfiniteVolume")]
            internal static extern void GmshModelGeoMeshSetTransfiniteVolume(int tag, int[] cornerTags, long cornerTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoMeshSetRecombine(")]
            internal static extern void GmshModelGeoMeshSetRecombine(int dim, int tag, double angle, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoMeshSetSmoothing")]
            internal static extern void GmshModelGeoMeshSetSmoothing(int dim, int tag, int val, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoMeshSetReverse")]
            internal static extern void GmshModelGeoMeshSetReverse(int dim, int tag, int val, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoMeshSetAlgorithm")]
            internal static extern void GmshModelGeoMeshSetAlgorithm(int dim, int tag, int val, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoMeshSetSizeFromBoundary")]
            internal static extern void GmshModelGeoMeshSetSizeFromBoundary(int dim, int tag, int val, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoSynchronize")]
            internal static extern void GmshModelGeoSynchronize(ref int ierr);

            #endregion

            #region Geo-OpenCASCADE

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddPoint")]
            internal static extern int GmshModelOccAddPoint(double x, double y, double z, double meshSize, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddLine")]
            internal static extern int GmshModelOccAddLine(int startTag, int endTag, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddCircleArc")]
            internal static extern int GmshModelOccAddCircleArc(int startTag, int centerTag, int endTag, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddCircle")]
            internal static extern int gmshModelOccAddCircle(double x, double y, double z, double r, int tag, double angle1, double angle2, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddEllipseArc")]
            internal static extern int GmshModelOccAddEllipseArc(int startTag, int centerTag, int majorTag, int endTag, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddEllipse")]
            internal static extern int GmshModelOccAddEllipse(double x, double y, double z, double r1, double r2, int tag, double angle1, double angle2, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddSpline")]
            internal static extern int gmshModelOccAddSpline([In, Out] int[] pointTags, long pointTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddBSpline")]
            internal static extern int GmshModelOccAddBSpline([In, Out] int[] pointTags, long pointTags_n, int tag, int degree, [In, Out] double[] weights, long weights_n, [In, Out] double[] knots, long knots_n, [In, Out] int[] multiplicities, long multiplicities_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddBezier")]
            internal static extern int GmshModelOccAddBezier([In, Out] int[] pointTags, long pointTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddWire")]
            internal static extern int GmshModelOccAddWire([In, Out] int[] curveTags, long curveTags_n, int tag, int checkClosed, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddCurveLoop")]
            internal static extern int GmshModelOccAddCurveLoop([In, Out] int[] curveTags, long curveTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddRectangle")]
            internal static extern int GmshModelOccAddRectangle(double x, double y, double z, double dx, double dy, int tag, double roundedRadius, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddDisk")]
            internal static extern int GmshModelOccAddDisk(double xc, double yc, double zc, double rx, double ry, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddPlaneSurface")]
            internal static extern int GmshModelOccAddPlaneSurface([In, Out] int[] wireTags, long wireTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddSurfaceFilling")]
            internal static extern int GmshModelOccAddSurfaceFilling(int wireTag, int tag, [In, Out] int[] pointTags, long pointTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddBSplineFilling")]
            internal static extern int GmshModelOccAddBSplineFilling(int wireTag, int tag, [In] string type, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddBezierFilling")]
            internal static extern int GmshModelOccAddBezierFilling(int wireTag, int tag, [In] string type, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddBSplineSurface")]
            internal static extern int GmshModelOccAddBSplineSurface([In, Out] int[] pointTags, long pointTags_n, int numPointsU, int tag, int degreeU, int degreeV, [In, Out] double[] weights, long weights_n, [In, Out] double[] knotsU, long knotsU_n, [In, Out] double[] knotsV, long knotsV_n, [In, Out] int[] multiplicitiesU, long multiplicitiesU_n, [In,Out] int[] multiplicitiesV, long multiplicitiesV_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddBezierSurface")]
            internal static extern int GmshModelOccAddBezierSurface([In, Out] int[] pointTags, long pointTags_n, int numPointsU, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddSurfaceLoop")]
            internal static extern int GmshModelOccAddSurfaceLoop([In, Out] int[] surfaceTags, long surfaceTags_n, int tag, int sewing, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddVolume")]
            internal static extern int GmshModelOccAddVolume([In, Out] int[] shellTags, long shellTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddSphere")]
            internal static extern int GmshModelOccAddSphere(double xc, double yc, double zc, double radius, int tag, double angle1, double angle2, double angle3, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddBox")]
            internal static extern int GmshModelOccAddBox(double x, double y, double z, double dx, double dy, double dz, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddCylinder")]
            internal static extern int GmshModelOccAddCylinder(double x, double y, double z, double dx, double dy, double dz, double r, int tag, double angle, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddCone")]
            internal static extern int GmshModelOccAddCone(double x, double y, double z, double dx, double dy, double dz, double r1, double r2, int tag, double angle, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddWedge")]
            internal static extern int GmshModelOccAddWedge(double x, double y, double z, double dx, double dy, double dz, int tag, double ltx, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddTorus")]
            internal static extern int GmshModelOccAddTorus(double x, double y, double z, double r1, double r2, int tag, double angle, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddThruSections")]
            internal static extern void GmshModelOccAddThruSections([In,Out] int[] wireTags, long wireTags_n, out IntPtr outDimTags, out long outDimTags_n, int tag, int makeSolid, int makeRuled, int maxDegree, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddThickSolid")]
            internal static extern void GmshModelOccAddThickSolid(int volumeTag, [In,Out] int[] excludeSurfaceTags, long excludeSurfaceTags_n, double offset, out IntPtr outDimTags, out long outDimTags_n, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccExtrude")]
            internal static extern void GmshModelOccExtrude([In, Out] int[] dimTags, long dimTags_n, double dx, double dy, double dz, out IntPtr outDimTags, out long outDimTags_n, [In,Out] int[] numElements, long numElements_n, [In,Out] double[] heights, long heights_n, int recombine, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccRemoveAllDuplicates")]
            internal static extern void GmshModelOccRemoveAllDuplicates(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccRevolve")]
            internal static extern void GmshModelOccRevolve([In, Out] int[] dimTags, long dimTags_n, double x, double y, double z, double ax, double ay, double az, double angle, out IntPtr outDimTags, out long outDimTags_n, [In, Out] int[] numElements, long numElements_n, [In,Out] double[] heights, long heights_n, int recombine, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAddPipe")]
            internal static extern void GmshModelOccAddPipe([In,Out] int[] dimTags, long dimTags_n, int wireTag, out IntPtr outDimTags, out long outDimTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccFillet")]
            internal static extern void GmshModelOccFillet([In, Out] int[] volumeTags, long volumeTags_n, [In, Out] int[] curveTags, long curveTags_n, [In, Out] double[] radii, long radii_n, out IntPtr outDimTags, out long outDimTags_n, int removeVolume, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccChamfer")]
            internal static extern void GmshModelOccChamfer([In, Out] int[] volumeTags, long volumeTags_n, [In, Out] int[] curveTags, long curveTags_n, [In, Out] int[] surfaceTags, long surfaceTags_n, [In,Out] double[] distances, long distances_n, out IntPtr outDimTags, out long outDimTags_n, int removeVolume, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccGetCenterOfMass")]
            internal static extern void GmshModelOccGetCenterOfMass(int dim, int tag, out double x, out double y, out double z, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccSynchronize")]
            internal static extern void GmshModelOccSynchronize(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccFuse")]
            internal static extern void GmshModelOccFuse([In,Out] int[] objectDimTags, long objectDimTags_n, [In, Out] int[] toolDimTags, long toolDimTags_n, out IntPtr outDimTags, out long outDimTags_n, out IntPtr outDimTagsMap, out IntPtr outDimTagsMap_n, out long outDimTagsMap_nn, int tag, int removeObject, int removeTool, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccIntersect")]
            internal static extern void GmshModelOccIntersect([In, Out] int[] objectDimTags, long objectDimTags_n, [In, Out] int[] toolDimTags, long toolDimTags_n, out IntPtr outDimTags, out long outDimTags_n, out IntPtr outDimTagsMap, out IntPtr outDimTagsMap_n, out long outDimTagsMap_nn, int tag, int removeObject, int removeTool, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccCut")]
            internal static extern void GmshModelOccCut([In, Out] int[] objectDimTags, long objectDimTags_n, [In, Out] int[] toolDimTags, long toolDimTags_n, out IntPtr outDimTags, out long outDimTags_n, out IntPtr outDimTagsMap, out IntPtr outDimTagsMap_n, out long outDimTagsMap_nn, int tag, int removeObject, int removeTool, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccFragment")]
            internal static extern void GmshModelOccFragment([In, Out] int[] objectDimTags, long objectDimTags_n, [In, Out] int[] toolDimTags, long toolDimTags_n, out IntPtr outDimTags, out long outDimTags_n, out IntPtr outDimTagsMap, out IntPtr outDimTagsMap_n, out long outDimTagsMap_nn, int tag, int removeObject, int removeTool, ref int ierr);

            #endregion

            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            ////// MESH
            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////

            #region Mesh

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGenerate")]
            internal static extern void GmshModelMeshGenerate(int dim, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetNodes")]
            internal static extern void GmshModelMeshGetNodes(out IntPtr nodeTags, out long nodeTags_n, out IntPtr coord, out long coord_n, out IntPtr parametricCoord, out long parametricCoord_n, int dim, int tag, int includeBoundary, int returnParametricCoord, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetElements")]
            internal static extern void GmshModelMeshGetElements(out IntPtr elementTypes, out long elementTypes_n, out IntPtr elementTags, out IntPtr elementTags_n, out long elementTags_nn, out IntPtr nodeTags, out IntPtr nodeTags_n, out long nodeTags_nn, int dim, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshOptimize")]
            internal static extern void GmshModelMeshOptimize([In, Out] string method, int force, int niter, [In, Out] int[] dimTags, IntPtr dimTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshRemoveDuplicateNodes")]
            internal static extern void GmshModelMeshRemoveDuplicateNodes(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSplitQuadrangles")]
            internal static extern void GmshModelMeshSplitQuadrangles(double quality, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetSize")]
            internal static extern void GmshModelMeshSetSize([In,Out] int[] dimTags, long dimTags_n, double size, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetSizeAtParametricPoints")]
            internal static extern void GmshModelMeshSetSizeAtParametricPoints(int dim, int tag, [In, Out] double[] parametricCoord, long parametricCoord_n, [In,Out] double[] sizes, long sizes_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetTransfiniteCurve")]
            internal static extern void GmshModelMeshSetTransfiniteCurve(int tag, int numNodes, [In] string meshType, double coef, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetTransfiniteSurface")]
            internal static extern void GmshModelMeshSetTransfiniteSurface(int tag, [In] string arrangement, [In,Out] int[] cornerTags, long cornerTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetTransfiniteVolume")]
            internal static extern void GmshModelMeshSetTransfiniteVolume(int tag, [In,Out] int[] cornerTags, long cornerTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetRecombine")]
            internal static extern void GmshModelMeshSetRecombine(int dim, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetSmoothing")]
            internal static extern void GmshModelMeshSetSmoothing(int dim, int tag, int val, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetReverse")]
            internal static extern void GmshModelMeshSetReverse(int dim, int tag, int val, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetAlgorithm")]
            internal static extern void GmshModelMeshSetAlgorithm(int dim, int tag, int val, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetSizeFromBoundary")]
            internal static extern void GmshModelMeshSetSizeFromBoundary(int dim, int tag, int val, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetCompound")]
            internal static extern void GmshModelMeshSetCompound(int dim, [In,Out] int[] tags, long tags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetOutwardOrientation")]
            internal static extern void GmshModelMeshSetOutwardOrientation(int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshEmbed")]
            internal static extern void GmshModelMeshEmbed(int dim, [In,Out] int[] tags, long tags_n, int inDim, int inTag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshRemoveEmbedded")]
            internal static extern void GmshModelMeshRemoveEmbedded([In,Out] int[] dimTags, long dimTags_n, int dim, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshRenumberNodes")]
            internal static extern void GmshModelMeshRenumberNodes(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshRenumberElements")]
            internal static extern void GmshModelMeshRenumberElements(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetPeriodic")]
            internal static extern void GmshModelMeshSetPeriodic(int dim, [In, Out] int[] tags, long tags_n, [In,Out] int[] tagsMaster, long tagsMaster_n, [In,Out] double[] affineTransform, long affineTransform_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshClassifySurfaces")]
            internal static extern void GmshModelMeshClassifySurfaces(double angle, int boundary, int forReparametrization, double curveAngle, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshCreateGeometry")]
            internal static extern void GmshModelMeshCreateGeometry([In,Out] int[] dimTags, long dimTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshCreateTopology")]
            internal static extern void GmshModelMeshCreateTopology(int makeSimplyConnected, int exportDiscrete, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshComputeHomology")]
            internal static extern void GmshModelMeshComputeHomology([In, Out] int[] domainTags, long domainTags_n, [In, Out] int[] subdomainTags, long subdomainTags_n, [In, Out] int[] dims, long dims_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshComputeCohomology")]
            internal static extern void GmshModelMeshComputeCohomology([In, Out] int[] domainTags, long domainTags_n, [In, Out] int[] subdomainTags, long subdomainTags_n, [In,Out] int[] dims, long dims_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshComputeCrossField")]
            internal static extern void GmshModelMeshComputeCrossField(out IntPtr viewTags, out long viewTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshFieldAdd")]
            internal static extern int GmshModelMeshFieldAdd([In] string fieldType, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshFieldRemove")]
            internal static extern void GmshModelMeshFieldRemove(int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshFieldSetNumber")]
            internal static extern void GmshModelMeshFieldSetNumber(int tag, [In] string option, double value, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshFieldSetString")]
            internal static extern void GmshModelMeshFieldSetString(int tag, [In] string option, [In]string value, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshFieldSetNumbers")]
            internal static extern void GmshModelMeshFieldSetNumbers(int tag, [In] string option, [In,Out] double[] value, long value_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshFieldSetAsBackgroundMesh")]
            internal static extern void GmshModelMeshFieldSetAsBackgroundMesh(int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshFieldSetAsBoundaryLayer")]
            internal static extern void GmshModelMeshFieldSetAsBoundaryLayer(int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshRefine")]
            internal static extern void GmshModelMeshRefine(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshRecombine")]
            internal static extern void GmshModelMeshRecombine(ref int ierr);

            #endregion

            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            ////// HELPERS
            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////

            #region Helpers
            public static double IntPtrToDouble(IntPtr ptr)
            {
                if (ptr != IntPtr.Zero)
                {
                    byte[] ba = new byte[sizeof(double)];
                    for (int i = 0; i < ba.Length; i++) ba[i] = Marshal.ReadByte(ptr, i);
                    return BitConverter.ToDouble(ba, 0);
                }
                return 0;
            }
            #endregion

        }
    }
}
