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

using System;
using System.Runtime.InteropServices;
using Iguana.IguanaMesh.IUtils;

namespace Iguana.IguanaMesh
{
    public static partial class IKernel
    {
        internal static partial class IModel
        {
            /// <summary>
            /// Add a new model, with name `name', and set it as the current model.
            /// </summary>
            /// <param name="name"> Name of the model </param>
            public static void Add(string name)
            {
                IWrap.GmshModelAdd(name, ref _ierr);
            }

            /// <summary>
            /// Remove Model
            /// </summary>
            public static void Remove()
            {
                IWrap.GmshModelRemove(ref _ierr);
            }

            /// <summary>
            /// Add a physical group of dimension `dim', grouping the model entities with tags `tags'. 
            /// Return the tag of the physical group, equal to `tag' if `tag' is positive, or a new tag if `tag' < 0.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tags"></param>
            /// <param name="tag"></param>
            /// <returns></returns>
            public static int AddPhysicalGroup(int dim, int[] tags, int tag = -1)
            {
                return IWrap.GmshModelAddPhysicalGroup(dim, tags, tags.Length, tag, ref _ierr);
            }

            /// <summary>
            /// Set the name of the physical group of dimension `dim' and tag `tag'.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="name"></param>
            public static void SetPhysicalName(int dim, int tag, string name)
            {
                IWrap.GmshModelSetPhysicalName(dim, tag, name, ref _ierr);
            }

            /// <summary>
            /// Get all the entities in the current model. If `dim' is >= 0, return only 
            /// the entities of the specified dimension(e.g.points if `dim' == 0). The
            /// entities are returned as a vector of (dim, tag) integer pairs.
            /// </summary>
            /// <param name="dimTags"></param>
            /// <param name="dim"></param>
            public static void GetEntities(out Tuple<int, int>[] dimTags, int dim = -1)
            {
                IntPtr dimTags_parse;
                long dimTags_n;
                IWrap.GmshModelGetEntities(out dimTags_parse, out dimTags_n, dim, ref _ierr);

                dimTags = null;

                // Tags
                if (dimTags_n > 0)
                {
                    var temp = new int[dimTags_n];
                    Marshal.Copy(dimTags_parse, temp, 0, (int)dimTags_n);

                    dimTags = IHelpers.GraftIntTupleArray(temp);
                }

                // Delete unmanaged allocated memory
                IWrap.GmshFree(dimTags_parse);
            }


            /// <summary>
            /// Get the boundary of the model entities `dimTags'. Return in `outDimTags' 
            /// the boundary of the individual entities(if `combined' is false) or the
            /// boundary of the combined geometrical shape formed by all input entities (if
            /// `combined' is true). Return tags multiplied by the sign of the boundary
            /// entity if `oriented' is true. Apply the boundary operator recursively down
            /// to dimension 0 (i.e.to points) if `recursive' is true. 
            /// </summary>
            public static void GetBoundary(Tuple<int, int>[] dimTags, out Tuple<int, int>[] outDimTags, bool combined = false, bool oriented = false, bool recursive = false)
            {
                int[] dimTags_flatten = IHelpers.FlattenIntTupleArray(dimTags);
                IntPtr outDimTags_parse;
                long outDimTags_n;
                IWrap.GmshModelGetBoundary(dimTags_flatten, dimTags_flatten.LongLength, out outDimTags_parse, out outDimTags_n, Convert.ToInt32(combined), Convert.ToInt32(oriented), Convert.ToInt32(recursive), ref _ierr);

                outDimTags = new Tuple<int, int>[0];
                if (outDimTags_n > 0)
                {
                    var temp = new int[outDimTags_n];
                    Marshal.Copy(outDimTags_parse, temp, 0, (int)outDimTags_n);

                    outDimTags = IHelpers.GraftIntTupleArray(temp);
                }

                IWrap.GmshFree(outDimTags_parse);
            }

            /// <summary>
            /// Add a discrete model entity (defined by a mesh) of dimension `dim' in the
            /// current model.Return the tag of the new discrete entity, equal to `tag' if
            /// `tag' is positive, or a new tag if `tag' < 0. `boundary' specifies the tags
            /// of the entities on the boundary of the discrete entity, if any.Specifying
            /// `boundary' allows Gmsh to construct the topology of the overall model.
            /// </summary>
            public static int AddDiscreteEntity(int dim, int tag, int[] boundary = default)
            {
                if (boundary == default) boundary = new int[0];
                return IWrap.GmshModelAddDiscreteEntity(dim, tag, boundary, boundary.LongLength, ref _ierr);
            }

            /// <summary>
            /// Remove the entities `dimTags' of the current model. If `recursive' is true,
            /// remove all the entities on their boundaries, down to dimension 0.
            /// </summary>
            public static void RemoveEntities(Tuple<int, int>[] dimTags, bool recursive=false)
            {
                var arr = IHelpers.FlattenIntTupleArray(dimTags);
                IWrap.GmshModelRemoveEntities(arr, arr.LongLength, Convert.ToInt32(recursive), ref _ierr);
            }

            /// <summary>
            /// Set the `x', `y', `z' coordinates of a geometrical point.
            /// </summary>
            public static void SetCoordinates(int tag, double x, double y, double z)
            {
                IWrap.GmshModelSetCoordinates(tag, x, y, z, ref _ierr);
            }


            /// <summary>
            /// Evaluate the parametrization of the entity of dimension `dim' and tag `tag'
            /// at the parametric coordinates `parametricCoord'. Only valid for `dim' equal
            /// to 0 (with empty `parametricCoord'), 1 (with `parametricCoord' containing
            /// parametric coordinates on the curve) or 2 (with `parametricCoord'
            /// containing pairs of u, v parametric coordinates on the surface,
            /// concatenated: [p1u, p1v, p2u, ...]). Return triplets of x, y, z coordinates
            /// in `coord', concatenated: [p1x, p1y, p1z, p2x, ...].
            /// </summary>
            public static void GetValue(int dim, int tag, double[] parametricCoord, out double[] coord)
            {
                IntPtr coord_parse;
                long coord_n;
                IWrap.GmshModelGetValue(dim, tag, parametricCoord, parametricCoord.LongLength, out coord_parse, out coord_n, ref _ierr);

                coord = new double[0];
                if (coord_n > 0)
                {
                    coord = new double[coord_n];
                    Marshal.Copy(coord_parse, coord, 0, (int) coord_n);
                }

                Free(coord_parse);
            }

            /// <summary>
            /// Evaluate the derivative of the parametrization of the entity of dimension
            /// `dim' and tag `tag' at the parametric coordinates `parametricCoord'. Only
            /// valid for `dim' equal to 1 (with `parametricCoord' containing parametric
            /// coordinates on the curve) or 2 (with `parametricCoord' containing pairs of
            /// u, v parametric coordinates on the surface, concatenated: [p1u, p1v, p2u,
            /// ...]). For `dim' equal to 1 return the x, y, z components of the derivative
            /// with respect to u[d1ux, d1uy, d1uz, d2ux, ...]; for `dim' equal to 2
            /// return the x, y, z components of the derivate with respect to u and v:
            /// [d1ux, d1uy, d1uz, d1vx, d1vy, d1vz, d2ux, ...].
            /// </summary>
            public static void GetDerivatives(int dim, int tag, double[] parametricCoord, out double[] derivatives)
            {
                IntPtr dP;
                long derivatives_n;

                IWrap.GmshModelGetDerivative(dim, tag, parametricCoord, parametricCoord.LongLength, out dP, out derivatives_n, ref _ierr);

                derivatives = new double[derivatives_n];
                Marshal.Copy(dP, derivatives, 0, (int)derivatives_n);

                Free(dP);
            }

            /// <summary>
            /// Evaluate the (maximum) curvature of the entity of dimension `dim' and tag
            /// `tag' at the parametric coordinates `parametricCoord'. Only valid for `dim'
            /// equal to 1 (with `parametricCoord' containing parametric coordinates on the
            /// curve) or 2 (with `parametricCoord' containing pairs of u, v parametric
            /// coordinates on the surface, concatenated: [p1u, p1v, p2u, ...]).
            /// </summary>
            public static void GetCurvature(int dim, int tag, double[] parametricCoord, out double[] curvatures)
            {
                IntPtr cP;
                long curvatures_n;
                IWrap.GmshModelGetCurvature(dim, tag, parametricCoord, parametricCoord.LongLength, out cP, out curvatures_n, ref _ierr);

                curvatures = new double[curvatures_n];
                Marshal.Copy(cP, curvatures, 0, (int)curvatures_n);

                Free(cP);
            }

            /// <summary>
            /// Evaluate the principal curvatures of the surface with tag `tag' at the
            /// parametric coordinates `parametricCoord', as well as their respective
            /// directions. `parametricCoord' are given by pair of u and v coordinates,
            /// concatenated: [p1u, p1v, p2u, ...].
            /// </summary>
            public static void GetPrincipalCurvatures(int tag, double[] parametricCoord, out double[] curvatureMax, out double[] curvatureMin, out double[] directionMax, out double[] directionMin)
            {
                IntPtr cMax, cMin, dMax, dMin;
                long directionMin_n, directionMax_n, curvatureMin_n, curvatureMax_n;

                IWrap.GmshModelGetPrincipalCurvatures(tag, parametricCoord, parametricCoord.LongLength, out cMax, out curvatureMax_n, out cMin, out curvatureMin_n, out dMax, out directionMax_n, out dMin, out directionMin_n, ref _ierr);

                curvatureMax = new double[curvatureMax_n];
                curvatureMin = new double[curvatureMin_n];
                directionMax = new double[directionMax_n];
                directionMin = new double[directionMin_n];

                Marshal.Copy(cMax, curvatureMax, 0, (int)curvatureMax_n);
                Marshal.Copy(cMin, curvatureMin, 0, (int)curvatureMin_n);
                Marshal.Copy(dMax, directionMax, 0, (int)directionMax_n);
                Marshal.Copy(dMin, directionMin, 0, (int)directionMin_n);

                Free(cMax);
                Free(cMin);
                Free(dMax);
                Free(dMin);

            }

            /// <summary>
            /// Get the normal to the surface with tag `tag' at the parametric coordinates
            /// `parametricCoord'. `parametricCoord' are given by pairs of u and v
            /// coordinates, concatenated: [p1u, p1v, p2u, ...]. `normals' are returned as
            /// triplets of x, y, z components, concatenated: [n1x, n1y, n1z, n2x, ...].
            /// </summary>
            public static void GetNormal(int tag, double[] parametricCoord, out double[] normals)
            {
                IntPtr nP;
                long normals_n;
                IWrap.GmshModelGetNormal(tag, parametricCoord, parametricCoord.LongLength, out nP, out normals_n, ref _ierr);

                normals = new double[0];
                if (normals_n > 0)
                {
                    normals = new double[normals_n];
                    Marshal.Copy(nP, normals, 0, (int)normals_n);
                }

                Free(nP);
            }

            /// <summary>
            /// Set the color of the model entities `dimTags' to the RGBA value (`r', `g',
            /// `b', `a'), where `r', `g', `b' and `a' should be integers between 0 and
            /// 255. Apply the color setting recursively if `recursive' is true.
            /// </summary>
            public static void SetColor(Tuple<int, int>[] dimTags, int r, int g, int b, int a = 255, bool recursive = false)
            {
                var arr = IHelpers.FlattenIntTupleArray(dimTags);
                IWrap.GmshModelSetColor(arr, arr.LongLength, r, g, b, a, Convert.ToInt32(recursive), ref _ierr);
            }

            /// <summary>
            /// Get the color of the model entity of dimension `dim' and tag `tag'.
            /// </summary>
            public static void GetColor(int dim, int tag, out int r, out int g, out int b, out int a)
            {
                IWrap.GmshModelGetColor(dim, tag, out r, out g, out b, out a, ref _ierr);
            }

            /// <summary>
            /// Get the name of the current model.
            /// </summary>
            public static void GetCurrent(out string name)
            {
                IntPtr nP;
                IWrap.GmshModelGetCurrent(out nP, ref _ierr);
                name = Marshal.PtrToStringAnsi(nP);

                Free(nP);
            }

            /// <summary>
            /// Set the current model to the model with name `name'. If several models have
            /// the same name, select the one that was added first.
            /// </summary>
            public static void SetCurrent(string name)
            {
                IWrap.GmshModelSetCurrent(name, ref _ierr);
            }

            /// <summary>
            /// Set the name of the entity of dimension `dim' and tag `tag'.
            /// </summary>
            public static void SetEntityName(int dim, int tag, string name)
            {
                IWrap.GmshModelSetEntityName(dim, tag, name, ref _ierr);
            }

            /// <summary>
            /// Get the name of the entity of dimension `dim' and tag `tag'.
            /// </summary>
            public static void GetEntityName(int dim, int tag, out string name)
            {
                IntPtr nP;
                IWrap.GmshModelGetEntityName(dim, tag, out nP, ref _ierr);

                name = Marshal.PtrToStringAnsi(nP);

                Free(nP);
            }

            /// <summary>
            /// Get all the physical groups in the current model. If `dim' is >= 0, return
            /// only the entities of the specified dimension(e.g.physical points if `dim'
            /// == 0). The entities are returned as a vector of(dim, tag) integer pairs.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="dimTags"></param>
            public static void GetPhysicalGroups(int dim, out Tuple<int, int>[] dimTags)
            {
                IntPtr dtP;
                long dimTags_n;
                IWrap.GmshModelGetPhysicalGroups(out dtP, out dimTags_n, dim, ref _ierr);

                dimTags = new Tuple<int, int>[0];
                if (dimTags_n > 0)
                {
                    var temp = new int[dimTags_n];
                    ; Marshal.Copy(dtP, temp, 0, (int)dimTags_n);
                    dimTags = IHelpers.GraftIntTupleArray(temp);
                }

                Free(dtP);
            }

            /// <summary>
            /// Get the tags of the model entities making up the physical group of dimension `dim' and tag `tag'.
            /// </summary>
            public static void GetEntitiesForPhysicalGroup(int dim, int tag, out int[] tags)
            {
                IntPtr tP;
                long tags_n;
                IWrap.GmshModelGetEntitiesForPhysicalGroup(dim, tag, out tP, out tags_n, ref _ierr);

                tags = new int[0];
                if (tags_n > 0)
                {
                    tags = new int[tags_n];
                    Marshal.Copy(tP, tags, 0, (int)tags_n);
                }

                Free(tP);
            }

            /// <summary>
            /// Get the tags of the physical groups (if any) to which the model entity of dimension `dim' and tag `tag' belongs.
            /// </summary>
            public static void GetPhysicalGroupsForEntity(int dim, int tag, out int[] physicalTags)
            {
                IntPtr ptP;
                long physicalTags_n;
                IWrap.GmshModelGetPhysicalGroupsForEntity(dim, tag, out ptP, out physicalTags_n, ref _ierr);

                physicalTags = new int[0];
                if (physicalTags_n > 0)
                {
                    physicalTags = new int[physicalTags_n];
                    Marshal.Copy(ptP, physicalTags, 0, (int)physicalTags_n);
                }

                Free(ptP);
            }

            /// <summary>
            /// Get the bounding box (`xmin', `ymin', `zmin'), (`xmax', `ymax', `zmax') of
            /// the model entity of dimension `dim' and tag `tag'. If `dim' and `tag' are
            /// negative, get the bounding box of the whole model.
            /// </summary>
            public static void GetBoundingBox(int dim, int tag, out double xmin, out double ymin, out double zmin, out double xmax, out double ymax, out double zmax)
            {
                IWrap.GmshModelGetBoundingBox(dim, tag, out xmin, out ymin, out zmin, out xmax, out ymax, out zmax, ref _ierr);
            }

            /// <summary>
            /// Get the geometrical dimension of the current model.
            /// </summary>
            /// <returns></returns>
            public static int GetDimension()
            {
                return IWrap.GmshModelGetDimension(ref _ierr);
            }

            /// <summary>
            /// Get the name of the physical group of dimension `dim' and tag `tag'.
            /// </summary>
            public static void GetPhysicalName(int dim, int tag, out string name)
            {
                IntPtr nP;
                IWrap.GmshModelGetPhysicalName(dim, tag, out nP, ref _ierr);

                name = Marshal.PtrToStringAnsi(nP);

                Free(nP);
            }

            /// <summary>
            /// Get the model entities in the bounding box defined by the two points
            /// (`xmin', `ymin', `zmin') and (`xmax', `ymax', `zmax'). If `dim' is >= 0,
            /// return only the entities of the specified dimension(e.g.points if `dim'== 0).
            /// </summary>
            public static void GetEntitiesInBoundingBox(double xmin, double ymin, double zmin, double xmax, double ymax, double zmax, out Tuple<int,int>[] tags, int dim = 0)
            {
                IntPtr tP;
                long tags_n;
                IWrap.GmshModelGetEntitiesInBoundingBox(xmin, ymin, zmin, xmax, ymax, zmax, out tP, out tags_n, dim, ref _ierr);

                tags = new Tuple<int, int>[0];
                if (tags_n > 0)
                {
                    var temp = new int[tags_n];
                    Marshal.Copy(tP, temp, 0, (int)tags_n);

                    tags = IHelpers.GraftIntTupleArray(temp);
                }

                Free(tP);
            }

            /// <summary>
            /// Remove the entity name `name' from the current model.
            /// </summary>
            public static void RemoveEntityName(string name)
            {
                IWrap.GmshModelRemoveEntityName(name, ref _ierr);
            }

            /// <summary>
            /// Remove the physical groups `dimTags' of the current model. If `dimTags' is empty, remove all groups.
            /// </summary>
            public static void RemovePhysicalGroups(Tuple<int,int>[] dimTags) {
                var arr = IHelpers.FlattenIntTupleArray(dimTags);
                IWrap.GmshModelRemovePhysicalGroups(arr, arr.LongLength, ref _ierr);
            }

            /// <summary>
            /// Remove the physical name `name' from the current model.
            /// </summary>
            public static void RemovePhysicalName(string name) {
                IWrap.GmshModelRemovePhysicalName(name, ref _ierr);
            }

            /// <summary>
            /// Get the type of the entity of dimension `dim' and tag `tag'.
            /// </summary>
            public static void GetType(int dim, int tag, out string entityType) {
                IntPtr etP;
                IWrap.GmshModelGetType(dim, tag, out etP, ref _ierr);

                entityType = Marshal.PtrToStringAnsi(etP);

                Free(etP);
            }

            /// <summary>
            /// In a partitioned model, return the tags of the partition(s) to which the entity belongs.
            /// </summary>
            public static void GetPartitions(int dim, int tag, out int[] partitions) {
                IntPtr p;
                long partitions_n;
                IWrap.GmshModelGetPartitions(dim, tag, out p, out partitions_n, ref _ierr);

                partitions = new int[0];
                if (partitions_n>0)
                {
                    partitions = new int[partitions_n];
                    Marshal.Copy(p, partitions, 0, (int) partitions_n);
                }

                Free(p);
            }

            /// <summary>
            /// In a partitioned model, get the parent of the entity of dimension `dim' and
            /// tag `tag', i.e. from which the entity is a part of, if any. `parentDim' and
            /// `parentTag' are set to -1 if the entity has no parent. 
            /// </summary>
            public static void GetParent(int dim, int tag, out int parentDim, out int parentTag) {
                IWrap.GmshModelGetParent(dim, tag, out parentDim, out parentTag, ref _ierr);
            }
        }
    }
}
