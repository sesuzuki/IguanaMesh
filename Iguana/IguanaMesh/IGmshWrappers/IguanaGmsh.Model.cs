using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Iguana.IguanaMesh.IGmshWrappers
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
                Wrappers.GmshModelAdd(name, ref _ierr);
            }

            /// <summary>
            /// Remove Model
            /// </summary>
            public static void Remove()
            {
                Wrappers.GmshModelRemove(ref _ierr);
            }

            /// <summary>
            /// Get all the entities in the current model. 
            /// </summary>
            /// <param name="dimTags"></param>
            /// <param name="dim"> If `dim' is >= 0, return only the entities of the specified dimension(e.g.points if `dim' == 0). The entities are returned as a vector of (dim, tag) integer pairs.</param>
            public static Tuple<int, int>[] GetEntities(int dim)
            {
                IntPtr arr_ptr, arr_size;

                Wrappers.GmshModelGetEntities(out arr_ptr, out arr_size, 0, ref _ierr);

                int length = arr_size.ToInt32();

                Tuple<int, int>[] ps = null;

                if (length > 0)
                {
                    var arr_out = new int[length];
                    Marshal.Copy(arr_ptr, arr_out, 0, length);

                    ps = new Tuple<int, int>[length / 2];
                    for (int i = 0; i < length / 2; i++)
                    {
                        ps[i] = Tuple.Create(arr_out[i * 2], arr_out[i * 2 + 1]);
                    }
                }

                Wrappers.GmshFree(arr_ptr);
                Wrappers.GmshFree(arr_size);
                return ps;
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
                return Wrappers.GmshModelAddPhysicalGroup(dim, tags, tags.Length, tag, ref _ierr);
            }

            /// <summary>
            /// Set the name of the physical group of dimension `dim' and tag `tag'.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="name"></param>
            public static void SetPhysicalName(int dim, int tag, string name)
            {
                Wrappers.GmshModelSetPhysicalName(dim, tag, name, ref _ierr);
            }
        }
    }
}
