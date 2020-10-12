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
        public static class Logger
        {
            /// <summary>
            /// Write a `message'. `level' can be "info", "warning" or "error".
            /// </summary>
            /// <param name="message"></param>
            /// <param name="levelr"></param>
            public static void Write(string message, string level="info")
            {
                IWrappers.GmshLoggerWrite(message, level, ref _ierr);
            }

            /// <summary>
            /// Start logging messages.
            /// </summary>
            public static void Start()
            {
                IWrappers.GmshLoggerStart(ref _ierr);
            }

            /// <summary>
            /// Get logged messages.
            /// </summary>
            /// <returns></returns>
            public static char[] Get()
            {
                IntPtr log;
                long log_n;

                IWrappers.GmshLoggerGet(out log, out log_n, ref _ierr);

                char[] msg = new char[log_n]; ;
                Marshal.Copy(log, msg, 0, (int) log_n);

                return msg;
            }

            /// <summary>
            /// Stop logging messages.
            /// </summary>
            public static void Stop() {
                IWrappers.GmshLoggerStop(ref _ierr);
            }

            /// <summary>
            /// Return wall clock time.
            /// </summary>
            public static double GetWallTime() {
                return IWrappers.GmshLoggerGetWallTime(ref _ierr);
            }

            /// <summary>
            /// Return CPU time.
            /// </summary>
            /// <returns></returns>
            public static double GetCpuTime() {
                return IWrappers.GmshLoggerGetCpuTime(ref _ierr);
            }
        }
    }
}
