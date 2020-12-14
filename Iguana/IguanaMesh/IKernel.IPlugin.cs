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

namespace Iguana.IguanaMesh
{
    public static partial class IKernel
    {
        internal static partial class IPlugin
        {
            /// <summary>
            /// Set the numerical option `option' to the value `value' for plugin `name'. 
            /// </summary>
            public static void SetNumber(string name, string option, double value) {
                IWrap.GmshPluginSetNumber(name, option, value, ref _ierr);
            }

            /// <summary>
            /// Set the string option `option' to the value `value' for plugin `name'.
            /// </summary>
            public static void SetString(string name, string option, string value)
            {
                IWrap.GmshPluginSetString(name, option, value, ref _ierr);
            }

            /// <summary>
            /// Run the plugin `name'.
            /// </summary>
            public static void Run(string name)
            {
                IWrap.GmshPluginRun(name, ref _ierr);
            }
        }
    }
}
