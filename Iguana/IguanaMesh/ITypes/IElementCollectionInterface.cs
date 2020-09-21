using System;
using System.Collections.Generic;

namespace Iguana.IguanaMesh.ITypes
{
    interface IElementCollectionInterface
    {
        Boolean AddElement(IElement element);

        Boolean AddElement(int idx, IElement element);

        Boolean AddRangeElements(List<IElement> elements);

        Boolean DeleteElement(int key);

        Boolean DeleteRangeElements(List<int> keys);

        void Clean();

        void CleanVisits();

        Boolean ContainsKey(int key);

        IElement GetElementWithKey(int key);
    }
}
