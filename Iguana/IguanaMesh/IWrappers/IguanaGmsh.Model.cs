using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Iguana.IguanaMesh.IUtils;

namespace Iguana.IguanaMesh.IWrappers
{
    public static partial class IguanaGmsh
    {
        public static partial class Model
        {
            /// <summary>
            /// Add a new model, with name `name', and set it as the current model.
            /// </summary>
            /// <param name="name"> Name of the model </param>
            public static void Add(string name)
            {
                IWrappers.GmshModelAdd(name, ref _ierr);
            }

            /// <summary>
            /// Remove Model
            /// </summary>
            public static void Remove()
            {
                IWrappers.GmshModelRemove(ref _ierr);
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
                return IWrappers.GmshModelAddPhysicalGroup(dim, tags, tags.Length, tag, ref _ierr);
            }

            /// <summary>
            /// Set the name of the physical group of dimension `dim' and tag `tag'.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="name"></param>
            public static void SetPhysicalName(int dim, int tag, string name)
            {
                IWrappers.GmshModelSetPhysicalName(dim, tag, name, ref _ierr);
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
                IWrappers.GmshModelGetEntities(out dimTags_parse, out dimTags_n, dim, ref _ierr);

                dimTags = null;

                // Tags
                if (dimTags_n > 0)
                {
                    var temp = new int[dimTags_n];
                    Marshal.Copy(dimTags_parse, temp, 0, (int)dimTags_n);

                    dimTags = IHelpers.GraftIntTupleArray(temp);
                }

                // Delete unmanaged allocated memory
                IWrappers.GmshFree(dimTags_parse);
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
                IWrappers.GmshModelGetBoundary(dimTags_flatten, dimTags_flatten.LongLength, out outDimTags_parse, out outDimTags_n, Convert.ToInt32(combined), Convert.ToInt32(oriented), Convert.ToInt32(recursive), ref _ierr);

                outDimTags = null;
                if (outDimTags_n > 0)
                {
                    var temp = new int[outDimTags_n];
                    Marshal.Copy(outDimTags_parse, temp, 0, (int)outDimTags_n);
                    outDimTags = IHelpers.GraftIntTupleArray(temp);
                }

                IWrappers.GmshFree(outDimTags_parse);
            }

            /// <summary>
            /// Add a discrete model entity (defined by a mesh) of dimension `dim' in the
            /// current model.Return the tag of the new discrete entity, equal to `tag' if
            /// `tag' is positive, or a new tag if `tag' < 0. `boundary' specifies the tags
            /// of the entities on the boundary of the discrete entity, if any.Specifying
            /// `boundary' allows Gmsh to construct the topology of the overall model.
            /// </summary>
            public static int AddDiscreteEntity(int dim, int tag, long[] boundary = default)
            {
                if (boundary == default) boundary = new long[0];
                return IWrappers.GmshModelAddDiscreteEntity(dim, tag, boundary, boundary.LongLength, ref _ierr);
            }

            /// <summary>
            /// Remove the entities `dimTags' of the current model. If `recursive' is true,
            /// remove all the entities on their boundaries, down to dimension 0.
            /// </summary>
            public static void RemoveEntities(long[] dimTags, bool recursive)
            {
                IWrappers.GmshModelRemoveEntities(dimTags, dimTags.LongLength, Convert.ToInt32(recursive), ref _ierr);
            }

            /// <summary>
            /// Set the `x', `y', `z' coordinates of a geometrical point.
            /// </summary>
            public static void SetCoordinates(int tag, double x, double y, double z)
            {
                IWrappers.GmshModelSetCoordinates(tag, x, y, z, ref _ierr);
            }
        }
    }
}
