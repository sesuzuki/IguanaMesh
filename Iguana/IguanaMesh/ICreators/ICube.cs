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

using System;
using System.Collections.Generic;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ICreators
{
    class ICube : ICreatorInterface
    {
        private Mesh _rM;
        private Boolean _weld;
        private double _tolerance;

        public ICube(Box box, int u, int v, int w, Boolean weld, double tolerance)
        {
            _weld = weld;
            _tolerance = tolerance;
            _rM = Mesh.CreateFromBox(box, u, v, w);
        }

        public IMesh BuildMesh()
        {
            IMesh m = new IMesh();
            Boolean flag = BuildDataBase();

            if (flag)
            {
                m.AddRhinoMesh(_rM, _weld, _tolerance);
            }
            return m;
        }

        public bool BuildDataBase()
        {
            if (_rM == null || !_rM.IsValid) return false;
            else return true;
        }
    }
}
