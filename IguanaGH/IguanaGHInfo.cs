using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace IguanaMeshGH
{
    public class IguanaGHInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "IguanaGH";
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
                //Return a short string describing the purpose of this GHA library.
                return "";
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
                //Return a string identifying you or your company.
                return "Seiichi Suzuki";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "sesuzuki@hotmail.com";
            }
        }
    }
}
