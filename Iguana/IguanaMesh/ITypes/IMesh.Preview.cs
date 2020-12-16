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

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IUtils;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh : IGH_PreviewData, IGH_BakeAwareData
    {
        public BoundingBox ClippingBox => new BoundingBox(IRhinoGeometry.GetVerticesAsPoints(this));

        public bool BakeGeometry(RhinoDoc doc, ObjectAttributes att, out Guid obj_guid)
        {
            obj_guid = Guid.Empty;

            if (att == null) att = doc.CreateDefaultAttributes();

            string id = Guid.NewGuid().ToString();
            int idxGr = doc.Groups.Add("IG-" + id);
            int idxLy = doc.Layers.Add("IG-" + id, Color.Aqua);
            
            ObjectAttributes att1 = att.Duplicate();
            att1.AddToGroup(idxGr);
            att1.LayerIndex = idxLy;

            Line ln;
            List<long> edgesID = new List<long>();
            long data1, data2;
            Point3d p1, p2;
            int next, count;
            int[] hf;

            foreach (int elementID in ElementsKeys)
            {
                IElement e = GetElementWithKey(elementID);

                for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                {
                    e.GetHalfFacet(halfFacetID, out hf);

                    count = 1;
                    if (e.TopologicDimension == 3) count = hf.Length;
                    for (int i = 0; i < count; i++)
                    {
                        next = i + 1;
                        if (i == count - 1)
                        {
                            if (count > 1) next = 0;
                            else next = 1;
                        }
                        data1 = (Int64)hf[i] << 32 | (Int64)hf[next];
                        data2 = (Int64)hf[next] << 32 | (Int64)hf[i];
                        if (!edgesID.Contains(data1) && !edgesID.Contains(data2))
                        {
                            p1 = GetVertexWithKey(hf[i]).RhinoPoint;
                            p2 = GetVertexWithKey(hf[next]).RhinoPoint;
                            ln = new Line(p1, p2);
                            doc.Objects.AddLine(ln, att1);
                            edgesID.Add(data1);
                        }
                    }
                }
            }
            return true;
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args) {
            args.Pipeline.DrawMeshShaded(RenderMesh, args.Material);
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (!_elementTypes.Contains(-1))
            {
                args.Pipeline.DrawMeshWires(RenderMesh, args.Color);
            }
            else
            {
                int[] hf;
                Point3d[] pts;
                foreach (IElement e in Elements)
                {
                    if (e.TopologicDimension == 2)
                    {
                        pts = IRhinoGeometry.GetPointsFromElements(e.Vertices, this);
                        args.Pipeline.DrawPolyline(pts, args.Color);
                    }
                    else
                    {
                        for (int i = 1; i <= e.HalfFacetsCount; i++)
                        {
                            e.GetHalfFacet(i, out hf);
                            pts = IRhinoGeometry.GetPointsFromElements(hf, this);
                            args.Pipeline.DrawPolyline(pts, args.Color);
                        }
                    }
                }
            }
        }
    }
}
