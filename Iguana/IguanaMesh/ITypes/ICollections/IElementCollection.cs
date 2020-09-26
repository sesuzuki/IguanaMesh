using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes.ICollections
{
    public class IElementCollection : IElementCollectionInterface
    {
        private Dictionary<int, IElement> _elements = new Dictionary<int, IElement>();
        private string msg;

        public bool AddElement(IElement element)
        {
            int key = element.Key;
            if (key < 0) key = FindNextKey();

            if (!_elements.ContainsKey(key))
            {
                element.Key = key;
                _elements.Add(key, element);
                msg = "||||| Element Added |||||\nPlease update topologic relationships.";
                return true;
            }
            else return false;
        }

        public bool AddElement(int key, IElement element)
        {
            if (!_elements.ContainsKey(key) && key>=0)
            {
                element.Key = key;
                _elements.Add(key, element);
                msg = "||||| Element Added |||||\nPlease update topologic relationships.";
                return true;
            }
            else return false;
        }

        public bool AddRangeElements(List<IElement> elements)
        {
            if (elements != null && elements.Count > 0)
            {
                foreach (IElement e in elements)
                {
                    AddElement(e);
                }
                return true;
            }
            else return false;
        }

        public void Clean()
        {
            _elements = new Dictionary<int, IElement>();
        }

        public void CleanVisits()
        {
            Parallel.ForEach(_elements.Values, e =>
            {
                e.Visited = false;
            });
        }

        public bool ContainsKey(int key)
        {
            return _elements.ContainsKey(key);
        }

        public bool DeleteElement(int key)
        {
            if (ContainsKey(key))
            {
                _elements.Remove(key);
                msg = "||||| Element Removed |||||\nPlease update topologic relationships.";
                return true;
            }
            else return false;
        }

        public bool DeleteRangeElements(List<int> keys)
        {
            if (keys != null && keys.Count > 0)
            {
                foreach (int k in keys)
                {
                    DeleteElement(k);
                }
                return true;
            }
            else return false;
        }

        public IElement GetElementWithKey(int key)
        {
            return _elements[key];
        }

        public int FindNextKey()
        {
            List<int> keys = ElementsKeys;
            keys.Sort();
            if (ElementsKeys.Count == 0) return 0;
            else return keys[keys.Count - 1] + 1;
        }

        public List<int> ElementsKeys
        {
            get => _elements.Keys.ToList();
        }

        public List<IElement> ElementsValues
        {
            get => _elements.Values.ToList();
        }

        public int Count
        {
            get => _elements.Count;
        }

        public override string ToString()
        {
            return msg;
        }
    }
}
