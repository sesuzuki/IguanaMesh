using System;
using Iguana.IguanaMesh.ITypes;

namespace Iguana.IguanaMesh.ICreators
{
    interface ICreatorInterface
    {
        Boolean BuildDataBase();
        IMesh BuildMesh();
    }
}
