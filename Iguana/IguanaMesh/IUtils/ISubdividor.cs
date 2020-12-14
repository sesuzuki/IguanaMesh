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

using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.IUtils
{
    public static partial class ISubdividor
    {
        public static IPoint3D ComputeAveragePosition(int[] keys, IMesh m)
        {
            IVector3D v = new IVector3D();
            for (int i = 0; i < keys.Length; i++) v += m.GetVertexWithKey(keys[i]).Position;
            v /= keys.Length;
            return new IPoint3D(v.X,v.Y,v.Z);
        }

        public static Point3d ComputeAveragePoint(int[] keys, IMesh m)
        {
            IVector3D v = new IVector3D();
            for (int i = 0; i < keys.Length; i++) v += m.GetVertexWithKey(keys[i]).Position;
            v /= keys.Length;
            return new Point3d(v.X,v.Y,v.Z);
        }

        public static IPoint3D ComputeAveragePosition(IPoint3D A, IPoint3D B)
        {
            return new IPoint3D((A.X+B.X)/2, (A.Y + B.Y) / 2, (A.Z + B.Z) / 2);
        }
    }
}
