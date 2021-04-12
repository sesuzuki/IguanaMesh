/*
 * <Iguana>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Rhino.Geometry;
using System;
using System.Runtime.InteropServices;

namespace Iguana.IguanaMesh
{
    public static partial class IKernel
    {
        internal static int _ierr = 0;

        internal static int EvaluatePoint(PointCloud pts, Point3d p, double t)
        {
            int idx = pts.ClosestPoint(p);
            if (idx != -1 && p.DistanceTo(pts[idx].Location) > t) idx = -1;
            return idx;
        }

        internal static string LogInfo
        {
            get
            {
                string msg = "\n---------------------------------------------\n\n" +
                             "                 IguanaMesh\n\n" +
                             "---------------------------------------------\n" +
                             "Version       : 1.0\n" +
                             "License       : GNU General Public License\n" +
                             "Gmsh version  : 4.6.0\n" +
                             "OCC version   : 7.4.0\n" +
                             "---------------------------------------------\n\n\n";
                return msg;
            }
        }

        /// <summary>
        /// Initialize IguanaGmsh. This must be called before any call to the other functions in the API.
        /// </summary>
        public static string Initialize()
        {
            IntPtr argv = IntPtr.Zero;
            IWrap.GmshInitialize(0, ref argv, Convert.ToInt32(true), ref _ierr);
            Marshal.FreeCoTaskMem(argv);
            return LogInfo;
        }

        /// <summary>
        /// Free a gmsh pointer.
        /// </summary>
        /// <param name="ptr"> Pointer. </param>
        internal static void Free(IntPtr ptr)
        {
            IWrap.GmshFree(ptr);
        }

        /// <summary>
        /// Finalize gmsh.
        /// </summary>
        public static void End()
        {
            IWrap.GmshFinalize(ref _ierr);
        }

        /// <summary>
        /// Open a file. Handling of the file depends on its extension and/or its contents: opening a file with model data will create a new model.
        /// </summary>
        /// <param name="fileName"></param>
        internal static void Open(string fileName)
        {
            IWrap.GmshOpen(fileName, ref _ierr);
        }

        /// <summary>
        /// Write a file. The export format is determined by the file extension.
        /// </summary>
        /// <param name="fileName"> Name of the file </param>
        internal static void Write(string fileName)
        {
            IWrap.GmshWrite(fileName, ref _ierr);
        }

        /// <summary>
        /// Clear all loaded models and post-processing data, and add a new empty model.
        /// </summary>
        internal static void Clear()
        {
            IWrap.GmshClear(ref _ierr);
        }

        /// <summary>
        /// Merge a file. Equivalent to the `File->Merge' menu in the Gmsh app.
        /// Handling of the file depends on its extension and/or its contents.Merging
        /// a file with model data will add the data to the current model.
        /// </summary>
        /// <param name="fileName"></param>
        internal static void Merge(string fileName)
        {
            IWrap.GmshMerge(fileName, ref _ierr);
        }

        /// <summary>
        /// Draw all the OpenGL scenes.
        /// </summary>
        internal static void GraphicsDraw()
        {
            IWrap.GmshGraphicsDraw(ref _ierr);
        }

        /// <summary>
        /// Set a numerical option to `value'. `name' is of the form "category.option" or "category[num].option". 
        /// Available categories and options are listed in the IguanaGmsh reference manual.
        /// By default IguanaGmsh will not print out any messages: in order to output messages on the terminal, just set the "General.Terminal" option to 1:
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetOptionNumber(string name, double value)
        {
            IWrap.GmshOptionSetNumber(name, value, ref _ierr);
        }

        /// <summary>
        /// Get the `value' of a numerical option. 
        /// </summary>
        /// <param name="name"> `name' is of the form "category.option" or "category[num].option". Available categories and options are listed in the IguanaGmsh reference manual. </param>
        /// <returns></returns>
        internal static void GetOptionNumber(string name, out double number)
        {
            IWrap.GmshOptionGetNumber(name, out number, ref _ierr);
        }

        /// <summary>
        /// Set a string option to `value'. `name' is of the form "category.option" or
        /// "category[num].option". Available categories and options are listed in the Gmsh reference manual.
        /// </summary>
        public static void SetOptionString(string name, string value)
        {
            IWrap.GmshOptionSetString(name, value, ref _ierr);
        }

        /// <summary>
        /// Get the `value' of a string option. `name' is of the form "category.option"
        /// or "category[num].option". Available categories and options are listed in
        /// the Gmsh reference manual.
        /// </summary>
        internal static void GetOptionString(string name, out string value)
        {
            IntPtr vP;
            IWrap.GmshOptionGetString(name, out vP, ref _ierr);
            value = Marshal.PtrToStringAnsi(vP);

            Free(vP);
        }

        /// <summary>
        ///  Set a color option to the RGBA value (`r', `g', `b', `a'), where where `r',
        ///  `g', `b' and `a' should be integers between 0 and 255. `name' is of the
        ///  form "category.option" or "category[num].option". Available categories and
        ///  options are listed in the Gmsh reference manual, with the "Color." middle string removed.
        /// </summary>
        internal static void SetOptionColor(string name, int r, int g, int b, int a = 255)
        {
            IWrap.GmshOptionSetColor(name, r, g, b, a, ref _ierr);
        }

        /// <summary>
        /// Get the `r', `g', `b', `a' value of a color option. `name' is of the form
        /// "category.option" or "category[num].option". Available categories and
        /// options are listed in the Gmsh reference manual, with the "Color." middle
        /// string removed.
        /// </summary>
        internal static void GetOptionColor(string name, out int r, out int g, out int b, out int a)
        {
            IWrap.GmshOptionGetColor(name, out r, out g, out b, out a, ref _ierr);
        }

        /// <summary>
        /// Write a `message'. `level' can be "info", "warning" or "error".
        /// </summary>
        /// <param name="message"></param>
        /// <param name="levelr"></param>
        internal static void WriteLogger(string message, string level = "info")
        {
            IWrap.GmshLoggerWrite(message, level, ref _ierr);
        }

        /// <summary>
        /// Start logging messages.
        /// </summary>
        internal static void StartLogger()
        {
            IWrap.GmshLoggerStart(ref _ierr);
        }

        /// <summary>
        /// Get logged messages.
        /// </summary>
        /// <returns></returns>
        internal static string GetLogger()
        {
            IntPtr bulk = IntPtr.Zero;
            long log_n;

            IWrap.GmshLoggerGet(out bulk, out log_n, ref _ierr);

            IntPtr[] tempPtr = new IntPtr[log_n];
            Marshal.Copy(bulk, tempPtr, 0, (int)log_n);

            string log = "";
            foreach (IntPtr ptr in tempPtr)
            {
                log += Marshal.PtrToStringAnsi(ptr) + "\n";
            }

            for (int i = 0; i < tempPtr.Length; i++)
            {
                Free(tempPtr[i]);
            }
            Free(bulk);

            return log;
        }

        /// <summary>
        /// Stop logging messages.
        /// </summary>
        internal static void StopLogger()
        {
            IWrap.GmshLoggerStop(ref _ierr);
        }

        /// <summary>
        /// Return wall clock time.
        /// </summary>
        internal static double GetWallTimeLogger()
        {
            return IWrap.GmshLoggerGetWallTime(ref _ierr);
        }

        /// <summary>
        /// Return CPU time.
        /// </summary>
        /// <returns></returns>
        internal static double GetCpuTimeLogger()
        {
            return IWrap.GmshLoggerGetCpuTime(ref _ierr);
        }
    }
}
