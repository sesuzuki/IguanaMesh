using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers
{
    /// <summary>
    /// A class used by managed classes to managed unmanaged DLLs. This will extract and load DLLs from embedded binary resources. 
    /// This class only uses pinvoke. When the DLLs are extracted, the %PATH% environment variable is updated to point to the temporary folder.
    /// Add all of the DLLs as binary file resources to the project Propeties. 
    /// In a static constructor of a class, call IguanaLoader.ExtractEmbeddedDlls() for each DLL that is needed.
    /// Example: IguanaLoader.ExtractEmbeddedDlls("libFrontPanel-pinv.dll", Properties.Resources.libFrontPanel_pinv);
    /// Continue using standard Pinvoke methods for the desired functions in the DLL
    /// </summary>
    public class IguanaLoader
    {
        private static string tempFolder = "";

        /// <summary>
        /// Extract DLLs from resources to temporary folder
        /// </summary>
        /// <param name="dllName">name of DLL file to create (including dll suffix)</param>
        /// <param name="resourceBytes">The resource name (fully qualified)</param>
        public static void ExtractEmbeddedDlls(string dllName, byte[] resourceBytes)
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            //string[] names = assem.GetManifestResourceNames();
            AssemblyName an = assem.GetName();

            // The temporary folder holds one or more of the temporary DLLs
            // It is made "unique" to avoid different versions of the DLL or architectures.
            tempFolder = String.Format("{0}.{1}.{2}", an.Name, an.ProcessorArchitecture, an.Version);

            string dirName = Path.Combine(Path.GetTempPath(), tempFolder);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            // Add the temporary dirName to the PATH environment variable (at the head!)
            string path = Environment.GetEnvironmentVariable("PATH");
            string[] pathPieces = path.Split(';');
            bool found = false;
            foreach (string pathPiece in pathPieces)
            {
                if (pathPiece == dirName)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Environment.SetEnvironmentVariable("PATH", dirName + ";" + path);
            }

            // See if the file exists, avoid rewriting it if not necessary
            string dllPath = Path.Combine(dirName, dllName);
            bool rewrite = true;
            if (File.Exists(dllPath))
            {
                byte[] existing = File.ReadAllBytes(dllPath);
                if (resourceBytes.SequenceEqual(existing))
                {
                    rewrite = false;
                }
            }
            if (rewrite)
            {
                File.WriteAllBytes(dllPath, resourceBytes);
            }
        }
    }
}
