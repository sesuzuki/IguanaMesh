using Grasshopper.Kernel;
using Iguana.IguanaMesh.IUtils;
using Rhino;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Point3d start, end;
            int idxA, idxB;
            Line ln;

            ObjectAttributes att1 = att.Duplicate();
            att1.AddToGroup(idxGr);
            att1.LayerIndex = idxLy;
            foreach (Int64 pair in this.Topology.GetUniqueEdges())
            {
                idxA = (Int32)(pair >> 32);
                idxB = (Int32)pair;
                start = GetVertexWithKey(idxA).RhinoPoint;
                end = GetVertexWithKey(idxB).RhinoPoint;
                ln =  new Line(start, end);
                doc.Objects.AddLine(ln, att1);
            }
            return true;
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args){ }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            Point3d start, end;
            int idxA, idxB;
            foreach (Int64 pair in this.Topology.GetUniqueEdges())
            {
                idxA = (Int32)(pair >> 32);
                idxB = (Int32) pair;
                start = this.GetVertexWithKey(idxA).RhinoPoint;
                end = this.GetVertexWithKey(idxB).RhinoPoint;
                args.Pipeline.DrawLine(start, end, args.Color, args.Thickness);
            }
        }
    }
}
