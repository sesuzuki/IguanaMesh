using Grasshopper.Kernel;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.ITypes
{
    public partial class IMesh : IGH_PreviewData
    {
        public BoundingBox ClippingBox => new BoundingBox(IRhinoGeometry.GetVerticesAsPoints(this));

        public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            Point3d start, end;
            int idxA, idxB;
            foreach (Int64 pair in this.Topology.GetUniqueEdges())
            {
                idxA = (Int32)(pair >> 32);
                idxB = (Int32)pair;
                start = this.Vertices.GetVertexWithKey(idxA).RhinoPoint;
                end = this.Vertices.GetVertexWithKey(idxB).RhinoPoint;
                args.Pipeline.DrawLine(start, end, args.Color, args.Thickness);
            }
        }
    }
}
