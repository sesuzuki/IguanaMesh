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

using Iguana.IguanaMesh.IUtils;
using System;
using System.Runtime.InteropServices;

namespace Iguana.IguanaMesh
{
    public static partial class IKernel
    {
        internal static class IGraphics
        {
            /// <summary>
            /// Create the FLTK graphical user interface. Can only be called in the main thread.
            /// </summary>
            public static void FltkInitialize()
            {
                IWrap.GmshFltkInitialize(ref _ierr);
            }

            /// <summary>
            /// Wait at most `time' seconds for user interface events and return. If `time' 
            /// < 0, wait indefinitely.First automatically create the user interface if it
            /// has not yet been initialized.Can only be called in the main thread.
            /// </summary>
            /// <param name="time"></param>
            public static void FltkWait(double time) {
                IWrap.GmshFltkWait(time, ref _ierr);
            }

            /// <summary>
            /// Update the user interface (potentially creating new widgets and windows).
            /// First automatically create the user interface if it has not yet been
            /// initialized.Can only be called in the main thread: use `awake("update")'
            /// to trigger an update of the user interface from another thread.
            /// </summary>
            public static void FltkUpdate()
            {
                IWrap.GmshFltkUpdate(ref _ierr);
            }

            /// <summary>
            /// Awake the main user interface thread and process pending events, and
            /// optionally perform an action(currently the only `action' allowed is "update"). 
            /// </summary>
            /// <param name="action"></param>
            public static void FltkAwake(string action) {
                IWrap.GmshFltkAwake(action, ref _ierr);
            }

            /// <summary>
            /// Block the current thread until it can safely modify the user interface. 
            /// </summary>
            public static void FltkLock() {
                IWrap.GmshFltkLock(ref _ierr);
            }

            /// <summary>
            /// Release the lock that was set using lock.
            /// </summary>
            public static void FltkUnlock() {
                IWrap.GmshFltkUnlock(ref _ierr);
            }

            /// <summary>
            /// Run the event loop of the graphical user interface, i.e. repeatedly call
            /// `wait()'. First automatically create the user interface if it has not yet
            /// been initialized.Can only be called in the main thread.
            /// </summary>
            public static void FltkRun()
            {
                IWrap.GmshFltkRun(ref _ierr);
            }

            /// <summary>
            /// Check if the user interface is available (e.g. to detect if it has been closed). 
            /// </summary>
            /// <returns></returns>
            public static int FltkIsAvailable() {
                return IWrap.GmshFltkIsAvailable(ref _ierr);
            }

            /// <summary>
            /// Select entities in the user interface. If `dim' is >= 0, return only the entities of the specified dimension(e.g.points if `dim' == 0). 
            /// </summary>
            /// <returns></returns>
            public static int FltkSelectEntities(out Tuple<int,int>[] dimTags, int dim) {
                IntPtr dtP;
                long dimTags_n;
                int val = IWrap.GmshFltkSelectEntities(out dtP, out dimTags_n, dim, ref _ierr);

                var temp = new int[dimTags_n];
                Marshal.Copy(dtP, temp, 0, (int)dimTags_n);

                dimTags = IHelpers.GraftIntTupleArray(temp);

                Free(dtP);

                return val;
            }

            /// <summary>
            /// Select elements in the user interface.
            /// </summary>
            /// <returns></returns>
            public static int FltkSelectElements(out long[] elementTags) {
                IntPtr etP;
                long elementTags_n;
                int val = IWrap.GmshFltkSelectElements(out etP, out elementTags_n, ref _ierr);

                elementTags = new long[elementTags_n];
                Marshal.Copy(etP, elementTags, 0, (int)elementTags_n);

                Free(etP);

                return val;
            }

            /// <summary>
            /// Select views in the user interface.
            /// </summary>
            /// <returns></returns>
            public static int FltkSelectViews(out int[] viewTags) {
                IntPtr vtP;
                long viewTags_n;

                int val = IWrap.GmshFltkSelectViews(out vtP, out viewTags_n, ref _ierr);

                viewTags = new int[viewTags_n];
                Marshal.Copy(vtP, viewTags, 0, (int)viewTags_n);

                Free(vtP);

                return val;
            }
        }
    }
}
