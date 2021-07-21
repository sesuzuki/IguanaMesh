/*
 * <Iguana>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Linq;

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
                copy.AddElement(e);
            });
            Vertices.ForEach(entry =>
            {
                ITopologicVertex v = (ITopologicVertex)entry.Clone();
                copy.AddVertex(v.Key, v);
            });

            copy._renderMesh = _renderMesh;
            copy._tempVertexToHalfFacets = _tempVertexToHalfFacets.ToDictionary(entry => entry.Key, entry => entry.Value);
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
                IElement e = entry.CleanCopy();
                copy.AddElement(e);
            });
            Vertices.ForEach(entry =>
            {
                ITopologicVertex v = entry.CleanCopy();
                copy.AddVertex(v.Key, v);
            });

            copy._renderMesh = _renderMesh;
            copy.elementKey = elementKey;
            copy._valid = _valid;
            return copy;
        }

    }
}
