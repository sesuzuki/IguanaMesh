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
        public static class FLTK
        {

            /// <summary>
            /// Create the FLTK graphical user interface. Can only be called in the main thread.
            /// </summary>
            public static void Initialize()
            {
                IWrappers.GmshFltkInitialize(ref _ierr);
            }

            /// <summary>
            /// Wait at most `time' seconds for user interface events and return. If `time' 
            /// < 0, wait indefinitely.First automatically create the user interface if it
            /// has not yet been initialized.Can only be called in the main thread.
            /// </summary>
            /// <param name="time"></param>
            public static void FltkWait(double time) {
                IWrappers.GmshFltkWait(time, ref _ierr);
            }

            /// <summary>
            /// Update the user interface (potentially creating new widgets and windows).
            /// First automatically create the user interface if it has not yet been
            /// initialized.Can only be called in the main thread: use `awake("update")'
            /// to trigger an update of the user interface from another thread.
            /// </summary>
            public static void Update()
            {
                IWrappers.GmshFltkUpdate(ref _ierr);
            }

            /// <summary>
            /// Awake the main user interface thread and process pending events, and
            /// optionally perform an action(currently the only `action' allowed is "update"). 
            /// </summary>
            /// <param name="action"></param>
            public static void Awake(string action) {
                IWrappers.GmshFltkAwake(action, ref _ierr);
            }

            /// <summary>
            /// Block the current thread until it can safely modify the user interface. 
            /// </summary>
            public static void Lock() {
                IWrappers.GmshFltkLock(ref _ierr);
            }

            /// <summary>
            /// Release the lock that was set using lock.
            /// </summary>
            public static void Unlock() {
                IWrappers.GmshFltkUnlock(ref _ierr);
            }

            /// <summary>
            /// Run the event loop of the graphical user interface, i.e. repeatedly call
            /// `wait()'. First automatically create the user interface if it has not yet
            /// been initialized.Can only be called in the main thread.
            /// </summary>
            public static void Run()
            {
                IWrappers.GmshFltkRun(ref _ierr);
            }

            /// <summary>
            /// Check if the user interface is available (e.g. to detect if it has been closed). 
            /// </summary>
            /// <returns></returns>
            public static int IsAvailable() {
                return IWrappers.GmshFltkIsAvailable(ref _ierr);
            }

            /// <summary>
            /// Select entities in the user interface. If `dim' is >= 0, return only the entities of the specified dimension(e.g.points if `dim' == 0). 
            /// </summary>
            /// <returns></returns>
            public static int SelectEntities(out Tuple<int,int>[] dimTags, int dim) {
                IntPtr dtP;
                long dimTags_n;
                int val = IWrappers.GmshFltkSelectEntities(out dtP, out dimTags_n, dim, ref _ierr);

                var temp = new int[dimTags_n];
                Marshal.Copy(dtP, temp, 0, (int)dimTags_n);

                dimTags = IHelpers.GraftIntTupleArray(temp);

                IguanaGmsh.Free(dtP);

                return val;
            }

            /// <summary>
            /// Select elements in the user interface.
            /// </summary>
            /// <returns></returns>
            public static int SelectElements(out long[] elementTags) {
                IntPtr etP;
                long elementTags_n;
                int val = IWrappers.GmshFltkSelectElements(out etP, out elementTags_n, ref _ierr);

                elementTags = new long[elementTags_n];
                Marshal.Copy(etP, elementTags, 0, (int)elementTags_n);

                IguanaGmsh.Free(etP);

                return val;
            }

            /// <summary>
            /// Select views in the user interface.
            /// </summary>
            /// <returns></returns>
            public static int SelectViews(out int[] viewTags) {
                IntPtr vtP;
                long viewTags_n;

                int val = IWrappers.GmshFltkSelectViews(out vtP, out viewTags_n, ref _ierr);

                viewTags = new int[viewTags_n];
                Marshal.Copy(vtP, viewTags, 0, (int)viewTags_n);

                IguanaGmsh.Free(vtP);

                return val;
            }
        }
    }
}
