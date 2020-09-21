using Iguana.IguanaMesh.ITypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IModifiers
{
    interface IModifier
    {
        IMesh ApplyModifier(IMesh mesh);
    }
}
