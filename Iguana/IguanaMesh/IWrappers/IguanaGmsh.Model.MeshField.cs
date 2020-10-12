using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers
{
    public static partial class IguanaGmsh
    {
        public static partial class Model
        {
            public static partial class MeshField
            {
                /// <summary>
                /// Add a new mesh size field of type `fieldType'. 
                /// If `tag' is positive, assign the tag explicitly; otherwise a new tag is assigned automatically.Return the field tag.
                /// </summary>
                /// <param name="fieldType"></param>
                /// <param name="tag"></param>
                /// <returns></returns>
                public static int Add(string fieldType, int tag = -1)
                {
                    return IWrappers.GmshModelMeshFieldAdd(fieldType, tag, ref _ierr);
                }

                /// <summary>
                /// Remove the field with tag `tag'.
                /// </summary>
                /// <param name="tag"></param>
                public static void Remove(int tag)
                {
                    IWrappers.GmshModelMeshFieldRemove(tag, ref _ierr);
                }

                /// <summary>
                /// Set the numerical option `option' to value `value' for field `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="option"></param>
                /// <param name="value"></param>
                public static void SetNumber(int tag, string option, double value)
                {
                    IWrappers.GmshModelMeshFieldSetNumber(tag, option, value, ref _ierr);
                }

                /// <summary>
                /// Set the string option `option' to value `value' for field `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="option"></param>
                /// <param name="value"></param>
                public static void SetString(int tag, string option, string value)
                {
                    IWrappers.GmshModelMeshFieldSetString(tag, option, value, ref _ierr);
                }

                /// <summary>
                /// Set the numerical list option `option' to value `value' for field `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="option"></param>
                /// <param name="value"></param>
                public static void SetNumbers(int tag, string option, double[] value)
                {
                    IWrappers.GmshModelMeshFieldSetNumbers(tag, option, value, value.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set the field `tag' as the background mesh size field. 
                /// </summary>
                /// <param name="tag"></param>
                public static void SetAsBackgroundMesh(int tag)
                {
                    IWrappers.GmshModelMeshFieldSetAsBackgroundMesh(tag, ref _ierr);
                }

                /// <summary>
                /// Set the field `tag' as a boundary layer size field.
                /// </summary>
                /// <param name="tag"></param>
                public static void SetAsBoundaryLayer(int tag)
                {
                    IWrappers.GmshModelMeshFieldSetAsBoundaryLayer(tag, ref _ierr);
                }
            }
        }
    }
}
