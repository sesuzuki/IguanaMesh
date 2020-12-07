using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security;

namespace Iguana.IguanaMesh.IWrappers
{
    public static partial class IguanaGmsh
    {
        private const string gmsh_dll = "gmsh-4.6.dll";

        internal static class IWrappers
        {

            static IWrappers()
            {
                IguanaLoader.ExtractEmbeddedDlls(gmsh_dll, Properties.Resources.gmsh_4_6);
            }

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
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFree")]
            internal static extern void GmshFree2(ref IntPtr ptr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshInitialize")]
            internal static extern void GmshInitialize(int argc, ref IntPtr argv, int readConfigFiles, ref int ierr);

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

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshOptionGetNumber")]
            internal static extern void GmshOptionGetNumber([In, Out] string name, out double value, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshMerge")]
            internal static extern void GmshMerge([In] string fileName, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshOptionSetString")]
            internal static extern void GmshOptionSetString([In] string name, [In] string value, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshOptionGetString")]
            internal static extern void GmshOptionGetString([In] string name, out IntPtr value, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshOptionSetColor")]
            internal static extern void GmshOptionSetColor([In] string name, int r, int g, int b, int a, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshOptionGetColor")]
            internal static extern void GmshOptionGetColor([In] string name, out int r, out int g, out int b, out int a, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshGraphicsDraw")]
            internal static extern void GmshGraphicsDraw(ref int ierr);

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


            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetEntities")]
            internal static extern void GmshModelGetEntities(out IntPtr dimTags, out long dimTags_n, int dim, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetBoundary")]
            internal static extern void GmshModelGetBoundary(int[] dimTags, long dimTags_n, out IntPtr outDimTags, out long outDimTags_n, int combined, int oriented, int recursive, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelAddDiscreteEntity")]
            internal static extern int GmshModelAddDiscreteEntity(int dim, int tag, [In] int[] boundary, long boundary_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelRemoveEntities")]
            internal static extern void GmshModelRemoveEntities([In] int[] dimTags, long dimTags_n, int recursive, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelSetCoordinates")]
            internal static extern void GmshModelSetCoordinates(int tag, double x, double y, double z, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetValue")]
            internal static extern void GmshModelGetValue(int dim, int tag, [In] double[] parametricCoord, long parametricCoord_n, out IntPtr coord, out long coord_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetDerivative")]
            internal static extern void GmshModelGetDerivative(int dim, int tag, [In] double[] parametricCoord, long parametricCoord_n, out IntPtr derivatives, out long derivatives_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetCurvature")]
            internal static extern void GmshModelGetCurvature(int dim, int tag, [In] double[] parametricCoord, long parametricCoord_n, out IntPtr curvatures, out long curvatures_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetPrincipalCurvatures")]
            internal static extern void GmshModelGetPrincipalCurvatures(int tag, [In] double[] parametricCoord, long parametricCoord_n, out IntPtr curvatureMax, out long curvatureMax_n, out IntPtr curvatureMin, out long curvatureMin_n, out IntPtr directionMax, out long directionMax_n, out IntPtr directionMin, out long directionMin_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetNormal")]
            internal static extern void GmshModelGetNormal(int tag, [In] double[] parametricCoord, long parametricCoord_n, out IntPtr normals, out long normals_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelSetColor")]
            internal static extern void GmshModelSetColor([In] int[] dimTags, long dimTags_n, int r, int g, int b, int a, int recursive, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetColor")]
            internal static extern void GmshModelGetColor(int dim, int tag, out int r, out int g, out int b, out int a, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetCurrent")]
            internal static extern void GmshModelGetCurrent(out IntPtr name, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelSetCurrent")]
            internal static extern void GmshModelSetCurrent([In] string name, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelSetEntityName")]
            internal static extern void GmshModelSetEntityName(int dim, int tag, [In] string name, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetEntityName")]
            internal static extern void GmshModelGetEntityName(int dim, int tag, out IntPtr name, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetPhysicalGroups")]
            internal static extern void GmshModelGetPhysicalGroups(out IntPtr dimTags, out long dimTags_n, int dim, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetEntitiesForPhysicalGroup")]
            internal static extern void GmshModelGetEntitiesForPhysicalGroup(int dim, int tag, out IntPtr tags, out long tags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetPhysicalGroupsForEntity")]
            internal static extern void GmshModelGetPhysicalGroupsForEntity(int dim, int tag, out IntPtr physicalTags, out long physicalTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetBoundingBox")]
            internal static extern void GmshModelGetBoundingBox(int dim, int tag, out double xmin, out double ymin, out double zmin, out double xmax, out double ymax, out double zmax, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetDimension")]
            internal static extern int GmshModelGetDimension(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetPhysicalName")]
            internal static extern void GmshModelGetPhysicalName(int dim, int tag, out IntPtr name, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetEntitiesInBoundingBox")]
            internal static extern void GmshModelGetEntitiesInBoundingBox(double xmin, double ymin, double zmin, double xmax, double ymax, double zmax, out IntPtr tags, out long tags_n, int dim, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelRemoveEntityName")]
            internal static extern void GmshModelRemoveEntityName([In] string name, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelRemovePhysicalGroups")]
            internal static extern void GmshModelRemovePhysicalGroups([In] int[] dimTags, long dimTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelRemovePhysicalName")]
            internal static extern void GmshModelRemovePhysicalName([In] string name, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetType")]
            internal static extern void GmshModelGetType(int dim, int tag, out IntPtr entityType, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetPartitions")]
            internal static extern void GmshModelGetPartitions(int dim, int tag, out IntPtr partitions, out long partitions_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGetParent")]
            internal static extern void GmshModelGetParent(int dim, int tag, out int parentDim, out int parentTag, ref int ierr);
            
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

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoTranslate")]
            internal static extern void GmshModelGeoTranslate([In] int[] dimTags, long dimTags_n, double dx, double dy, double dz, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoRotate")]
            internal static extern void GmshModelGeoRotate([In] int[] dimTags, long dimTags_n, double x, double y, double z, double ax, double ay, double az, double angle, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoCopy")]
            internal static extern void GmshModelGeoCopy([In] int[] dimTags, long dimTags_n, out IntPtr outDimTags, out long outDimTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoDilate")]
            internal static extern void GmshModelGeoDilate([In] int[] dimTags, long dimTags_n, double x, double y, double z, double a, double b, double c, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoMirror")]
            internal static extern void GmshModelGeoMirror([In] int[] dimTags, long dimTags_n, double a, double b, double c, double d, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoSymmetrize")]
            internal static extern void GmshModelGeoSymmetrize([In] int[] dimTags, long dimTags_n, double a, double b, double c, double d, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoRemove")]
            internal static extern void GmshModelGeoRemove([In] int[] dimTags, long dimTags_n, int recursive, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoRemoveAllDuplicates")]
            internal static extern void GmshModelGeoRemoveAllDuplicates(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoSplitCurve")]
            internal static extern void GmshModelGeoSplitCurve(int tag, [In] int[] pointTags, long pointTags_n, out IntPtr curveTags, out long curveTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelGeoMeshSetSize")]
            internal static extern void GmshModelGeoMeshSetSize([In] int[] dimTags, long dimTags_n, double size, ref int ierr);
            
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

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccTranslate")]
            internal static extern void GmshModelOccTranslate([In] int[] dimTags, long dimTags_n, double dx, double dy, double dz, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccRotate")]
            internal static extern void GmshModelOccRotate([In] int[] dimTags, long dimTags_n, double x, double y, double z, double ax, double ay, double az, double angle, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccDilate")]
            internal static extern void GmshModelOccDilate([In] int[] dimTags, long dimTags_n, double x, double y, double z, double a, double b, double c, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccMirror")]
            internal static extern void GmshModelOccMirror([In] int[] dimTags, long dimTags_n, double a, double b, double c, double d, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccSymmetrize")]
            internal static extern void GmshModelOccSymmetrize([In] int[] dimTags, long dimTags_n, double a, double b, double c, double d, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccAffineTransform")]
            internal static extern void GmshModelOccAffineTransform([In] int[] dimTags, long dimTags_n, out IntPtr a, out long a_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccCopy")]
            internal static extern void GmshModelOccCopy([In] int[] dimTags, long dimTags_n, out IntPtr outDimTags, out long outDimTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccRemove")]
            internal static extern void GmshModelOccRemove([In] int[] dimTags, long dimTags_n, int recursive, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccHealShapes")]
            internal static extern void GmshModelOccHealShapes(out IntPtr outDimTags, out long outDimTags_n, [In] int[] dimTags, long dimTags_n, double tolerance, int fixDegenerated, int fixSmallEdges, int fixSmallFaces, int sewFaces, int makeSolids, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccGetEntities")]
            internal static extern void GmshModelOccGetEntities(out IntPtr dimTags, out long dimTags_n, int dim, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccGetEntitiesInBoundingBox")]
            internal static extern void GmshModelOccGetEntitiesInBoundingBox(double xmin, double ymin, double zmin, double xmax, double ymax, double zmax, out IntPtr tags, out long tags_n, int dim, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccGetBoundingBox")]
            internal static extern void GmshModelOccGetBoundingBox(int dim, int tag, out double xmin, out double ymin, out double zmin, out double xmax, out double ymax, out double zmax, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccMeshSetSize")]
            internal static extern void GmshModelOccMeshSetSize([In] int[] dimTags, long dimTags_n, double size, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccGetMass")]
            internal static extern void GmshModelOccGetMass(int dim, int tag, out double mass, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccGetMatrixOfInertia")]
            internal static extern void GmshModelOccGetMatrixOfInertia(int dim, int tag, out IntPtr mat, out long mat_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelOccImportShapes")]
            internal static extern void GmshModelOccImportShapes([In] string fileName, out IntPtr outDimTags, out long outDimTags_n, int highestDimOnly, [In] string format, ref int ierr);

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
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetTransfiniteAutomatic")]
            internal static extern void GmshModelMeshSetTransfiniteAutomatic([In] int[] dimTags, long dimTags_n, double cornerAngle, int recombine, ref int ierr);

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
            internal static extern void GmshModelMeshCreateGeometry([In,Out] long[] dimTags, long dimTags_n, ref int ierr);

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

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshReclassifyNodes")]
            internal static extern void GmshModelMeshReclassifyNodes(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshAddNodes")]
            internal static extern void GmshModelMeshAddNodes(int dim, int tag, [In] long[] nodeTags, long nodeTags_n, [In] double[] coord, long coord_n, [In] double[] parametricCoord, long parametricCoord_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshAddElements")]
            internal static extern void GmshModelMeshAddElements(int dim, int tag, int[] elementTypes, long elementTypes_n, [In] long[] elementTags, long elementTags_n, long elementTags_nn, [In] long[] nodeTags, long nodeTags_n, long nodeTags_nn, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshAddElementsByType")]
            internal static extern void GmshModelMeshAddElementsByType(int tag, int elementType, [In] long[] elementTags, long elementTags_n, [In] long[] nodeTags, long nodeTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshPartition")]
            internal static extern void GmshModelMeshPartition(int numPart, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshUnpartition")]
            internal static extern void GmshModelMeshUnpartition(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetOrder")]
            internal static extern void GmshModelMeshSetOrder(int order, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshClear")]
            internal static extern void GmshModelMeshClear([In] int[] dimTags, long dimTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetNodesByElementType")]
            internal static extern void GmshModelMeshGetNodesByElementType(int elementType, out IntPtr nodeTags, out long nodeTags_n, out IntPtr coord, out long coord_n, out IntPtr parametricCoord, out long parametricCoord_n, int tag, int returnParametricCoord, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetNode")]
            internal static extern void GmshModelMeshGetNode(long nodeTag, out IntPtr coord, out long coord_n, out IntPtr parametricCoord, out long parametricCoord_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshSetNode")]
            internal static extern void GmshModelMeshSetNode(long nodeTag, [In] double[] coord, long coord_n, [In] double[] parametricCoord, long parametricCoord_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshRebuildNodeCache")]
            internal static extern void GmshModelMeshRebuildNodeCache(int onlyIfNecessary, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshRebuildElementCache")]
            internal static extern void GmshModelMeshRebuildElementCache(int onlyIfNecessary, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetNodesForPhysicalGroup")]
            internal static extern void GmshModelMeshGetNodesForPhysicalGroup(int dim, int tag, out IntPtr nodeTags, out long nodeTags_n, out IntPtr coord, out long coord_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshRelocateNodes")]
            internal static extern void GmshModelMeshRelocateNodes(int dim, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetElement")]
            internal static extern void GmshModelMeshGetElement(long elementTag, out int elementType, out IntPtr nodeTags, out long nodeTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetElementByCoordinates")]
            internal static extern void GmshModelMeshGetElementByCoordinates(double x, double y, double z, out long elementTag, out int elementType, out IntPtr nodeTags, out long nodeTags_n, out double u, out double v, out double w, int dim, int strict, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetElementsByCoordinates")]
            internal static extern void GmshModelMeshGetElementsByCoordinates(double x, double y, double z, out IntPtr elementTags, out long elementTags_n, int dim, int strict, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetLocalCoordinatesInElement")]
            internal static extern void GmshModelMeshGetLocalCoordinatesInElement(long elementTag, double x, double y, double z, out double u, out double v, out double w, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetElementTypes")]
            internal static extern void GmshModelMeshGetElementTypes(out IntPtr elementTypes, out long elementTypes_n, int dim, int tag, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetElementType")]
            internal static extern void GmshModelMeshGetElementType([In] string familyName, int order, int serendip, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetElementProperties")]
            internal static extern void GmshModelMeshGetElementProperties(int elementType, [In] string elementName, out int dim, out int order, out int numNodes, out IntPtr localNodeCoord, out long localNodeCoord_n, out int numPrimaryNodes, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetElementsByType")]
            internal static extern void GmshModelMeshGetElementsByType(int elementType, out IntPtr elementTags, out long elementTags_n, out IntPtr nodeTags, out long nodeTags_n, int tag, long task, long numTasks, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetJacobians")]
            internal static extern void GmshModelMeshGetJacobians(int elementType, [In] double[] localCoord, long localCoord_n, out IntPtr jacobians, out long jacobians_n, out IntPtr determinants, out long determinants_n, out IntPtr coord, out long coord_n, int tag, long task, long numTasks, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetJacobian")]
            internal static extern void GmshModelMeshGetJacobian(long elementTag, [In] double[] localCoord, long localCoord_n, out IntPtr jacobians, out long jacobians_n, out IntPtr determinants, out long determinants_n, out IntPtr coord, out long coord_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetInformationForElements")]
            internal static extern void GmshModelMeshGetInformationForElements([In] int[] keys, long keys_n, int elementType, [In] string functionSpaceType, out IntPtr infoKeys, out long infoKeys_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetBarycenters")]
            internal static extern void GmshModelMeshGetBarycenters(int elementType, int tag, int fast, int primary, out IntPtr barycenters, out long barycenters_n, long task, long numTasks, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetElementEdgeNodes")]
            internal static extern void GmshModelMeshGetElementEdgeNodes(int elementType, out IntPtr nodeTags, out long nodeTags_n, int tag, int primary, long task, long numTasks, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetElementFaceNodes")]
            internal static extern void GmshModelMeshGetElementFaceNodes(int elementType, int faceType, out IntPtr nodeTags, out long nodeTags_n, int tag, int primary, long task, long numTasks, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshReorderElements")]
            internal static extern void GmshModelMeshReorderElements(int elementType, int tag, [In] long[] ordering, long ordering_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetBasisFunctions")]
            internal static extern void GmshModelMeshGetBasisFunctions(int elementType, [In] double[] localCoord, long localCoord_n, [In] string functionSpaceType, out int numComponents, out IntPtr basisFunctions, out long basisFunctions_n, out int numOrientations, [In] int[] wantedOrientations, long wantedOrientations_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetBasisFunctionsOrientationForElements")]
            internal static extern void GmshModelMeshGetBasisFunctionsOrientationForElements(int elementType, [In] string functionSpaceType, out IntPtr basisFunctionsOrientation, out long basisFunctionsOrientation_n, int tag, long task, long numTasks, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetBasisFunctionsOrientationForElement")]
            internal static extern void GmshModelMeshGetBasisFunctionsOrientationForElement(long elementTag, [In] string functionSpaceType, out int basisFunctionsOrientation, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshModelMeshGetNumberOfOrientations")]
            internal static extern void GmshModelMeshGetNumberOfOrientations(int elementType, [In] string functionSpaceType, ref int ierr);
            
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

            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            ////// LOGGER
            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////

            #region Logger

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshLoggerWrite")]
            internal static extern void GmshLoggerWrite([In]string message, [In] string level, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshLoggerStart")]
            internal static extern void GmshLoggerStart(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshLoggerGet")]
            internal static extern void GmshLoggerGet(out IntPtr log, out long log_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshLoggerStop")]
            internal static extern void GmshLoggerStop(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshLoggerGetWallTime")]
            internal static extern double GmshLoggerGetWallTime(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshLoggerGetCpuTime")]
            internal static extern double GmshLoggerGetCpuTime(ref int ierr);

            #endregion

            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            ////// Plugin
            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////

            #region Plugin

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshPluginSetNumber")]
            internal static extern void GmshPluginSetNumber([In] string name, [In] string option, double value, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshPluginSetString")]
            internal static extern void GmshPluginSetString([In] string name, [In] string option, [In] string value, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshPluginRun")]
            internal static extern void GmshPluginRun([In] string name, ref int ierr);

            #endregion

            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            ////// FLTK
            /////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////

            #region FLTK

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFltkInitialize")]
            internal static extern void GmshFltkInitialize(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFltkWait")]
            internal static extern void GmshFltkWait(double time, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFltkUpdate")]
            internal static extern void GmshFltkUpdate(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFltkAwake")]
            internal static extern void GmshFltkAwake([In] string action, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFltkLock")]
            internal static extern void GmshFltkLock(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFltkUnlock")]
            internal static extern void GmshFltkUnlock(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFltkRun")]
            internal static extern void GmshFltkRun(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFltkIsAvailable")]
            internal static extern int GmshFltkIsAvailable(ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFltkSelectEntities")]
            internal static extern int GmshFltkSelectEntities(out IntPtr dimTags, out long dimTags_n, int dim, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mshFltkSelectElements")]
            internal static extern int GmshFltkSelectElements(out IntPtr elementTags, out long elementTags_n, ref int ierr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(gmsh_dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gmshFltkSelectViews")]
            internal static extern int GmshFltkSelectViews(out IntPtr viewTags, out long viewTags_n, ref int ierr);

            #endregion
        }
    }
}
