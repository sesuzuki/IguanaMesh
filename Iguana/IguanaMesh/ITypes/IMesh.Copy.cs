using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh
    {
        public IMesh ShallowCopy()
        {
            return (IMesh) this.MemberwiseClone();
        }

        public IMesh DeepCopy()
        {
            IMesh copy = new IMesh();
            Elements.ForEach(entry => 
            { 
                IElement e = (IElement) entry.Clone();
                copy._elements.Add(e.Key, e);
            });
            Vertices.ForEach(entry =>
            {
                ITopologicVertex v = (ITopologicVertex)entry.Clone();
                copy._vertices.Add(v.Key, v);
            });
            copy._tempVertexToHalfFacets = _tempVertexToHalfFacets.ToDictionary(entry => entry.Key, entry => entry.Value);
            copy.dim = dim;
            copy.message = message;
            copy.elementKey = elementKey;
            copy._valid = _valid;
            return copy;
        }
        public IMesh CleanCopy()
        {
            IMesh copy = new IMesh();
            Elements.ForEach(entry =>
            {
                IElement e = (IElement)entry.CleanCopy();
                copy._elements.Add(e.Key, e);
            });
            Vertices.ForEach(entry =>
            {
                ITopologicVertex v = (ITopologicVertex)entry.CleanCopy();
                copy._vertices.Add(v.Key, v);
            });
            copy.elementKey = elementKey;
            copy._valid = _valid;
            return copy;
        }

    }
}
