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
            
            int idxGr = doc.Groups.Add("IMesh_"+DateTime.Today.ToShortDateString().Replace("/", "_"));
            int idxLy = doc.Layers.Add("IMesh_" + DateTime.Today.ToShortDateString().Replace("/", "_"), Color.Aqua);

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

        public void DrawViewportMeshes(GH_PreviewMeshArgs args){ }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
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
                            args.Pipeline.DrawLine(new Line(p1, p2), args.Color);
                            edgesID.Add(data1);
                        }
                    }
                }
            }
        }
    }
}
