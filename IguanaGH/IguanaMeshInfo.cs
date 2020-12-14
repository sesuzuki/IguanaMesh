using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace IguanaMeshGH
{
    public class IguanaMeshInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "IguanaMesh";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                return "An open source finite element mesh library built on a half-facet data structure supporting non-manifold meshes and mixed dimensionalities.";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("ced19cee-20ee-4687-a7a5-8fbf5ea9c38f");
            }
        }

        public override string AuthorName
        {
            get
            {
                return "Seiichi Suzuki";
            }
        }
        public override string AuthorContact
        {
            get
            {
                return "sesuzuki@hotmail.com";
            }
        }
    }
}
