using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.ICollections;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Collections.Generic;

namespace Iguana.IguanaMesh.IGmshWrappers
{
    public static class IguanaGmshConstructors
    {
        public static int OCCSurfacePatch(NurbsCurve crv, List<Point3d> patchs=default, bool synchronize=false)
        {
            crv.MakePiecewiseBezier(true);
            NurbsCurvePointList pts = crv.Points;
            NurbsCurveKnotList knots = crv.Knots;

            int[] curvePts = new int[pts.Count];
            double[] weightPts = new double[pts.Count];

            // 1._ Build Points
            ControlPoint p1;
            for (int i = 0; i < pts.Count - 1; i++)
            {
                p1 = pts[i];
                curvePts[i] = IguanaGmsh.Model.GeoOCC.AddPoint(p1.X, p1.Y, p1.Z);
                weightPts[i] = pts.GetWeight(i);
            }
            weightPts[pts.Count - 1] = weightPts[0];
            curvePts[pts.Count - 1] = curvePts[0];

            // 2._ Check points to patch
            int[] patchPts;
            if (patchs == default) patchPts = new int[0];
            else patchPts = new int[patchs.Count];

            Point3d p2;             
            for (int i = 0; i < patchs.Count; i++)
            {
                p2 = patchs[i];
                patchPts[i] = IguanaGmsh.Model.GeoOCC.AddPoint(p2.X, p2.Y, p2.Z);
            }

            // 3._ Build OCC Geometry
            int occCurve = IguanaGmsh.Model.GeoOCC.AddBSpline(curvePts, crv.Degree, weightPts);
            int occWire = IguanaGmsh.Model.GeoOCC.AddWire(new int[] { occCurve });
            int surfaceTag = IguanaGmsh.Model.GeoOCC.AddSurfaceFilling(occWire, patchPts);

            // 5._ Synchronize model
            if(synchronize) IguanaGmsh.Model.GeoOCC.Synchronize();

            return surfaceTag;
        }

        public static void OCCEmbedConstraintsOnSurface(IguanaGmshConstraintCollector constraints, int surfaceTag, ref IVertexCollection vertices, bool synchronize=false)
        {
            int count;

            if (constraints.HasConstraints())
            {
                count = constraints.GetPointConstraintCount();
                int[] embedPts = new int[count];
                Tuple<Point3d, double> data;
                Point3d p;
                for (int i = 0; i < count; i++)
                {
                    data = constraints.GetConstraint(i);
                    p = data.Item1;
                    embedPts[i] = IguanaGmsh.Model.GeoOCC.AddPoint(p.X, p.Y, p.Z, data.Item2);
                    vertices.AddVertex(embedPts[i], new ITopologicVertex(p.X, p.Y, p.Z));
                }

                if(synchronize) IguanaGmsh.Model.GeoOCC.Synchronize();

                IguanaGmsh.Model.Mesh.Embed(0, embedPts, 2, surfaceTag);
            }
        }
    }
}
