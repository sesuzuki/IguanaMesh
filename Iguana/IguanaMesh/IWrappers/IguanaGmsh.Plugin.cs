using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers
{
    public static partial class IguanaGmsh
    {
        public static partial class Plugin
        {
            /// <summary>
            /// Set the numerical option `option' to the value `value' for plugin `name'. 
            /// </summary>
            public static void SetNumber(string name, string option, double value) {
                IWrappers.GmshPluginSetNumber(name, option, value, ref _ierr);
            }

            /// <summary>
            /// Set the string option `option' to the value `value' for plugin `name'.
            /// </summary>
            public static void SetString(string name, string option, string value)
            {
                IWrappers.GmshPluginSetString(name, option, value, ref _ierr);
            }

            /// <summary>
            /// Run the plugin `name'.
            /// </summary>
            public static void Run(string name)
            {
                IWrappers.GmshPluginRun(name, ref _ierr);
            }
        }
    }
}
