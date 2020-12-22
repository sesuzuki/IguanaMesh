using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace IguanaMeshGH.IParameters
{
    public class IMeshParameterGH : GH_PersistentGeometryParam<IMesh>, IGH_PreviewObject
    {
        public IMeshParameterGH()
          : base(new GH_InstanceDescription("iMesh", "iMesh", "Contains a collection of iMeshes.", "Params", "Geometry"))
        {
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iMeshParam;
            }
        }
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.tertiary;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("ee30da59-f7e4-45b1-8fac-45cf6a057bed"); }
        }

        protected override GH_GetterResult Prompt_Plural(ref List<IMesh> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref IMesh value)
        {
            return GH_GetterResult.cancel;
        }

        #region preview methods
        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            IGH_StructureEnumerator data = this.VolatileData.AllData(true);
            Color c = args.WireColour;
            if (Attributes.Selected) c = args.WireColour_Selected;

            foreach (IMesh m in data)
            {
                if (!m.ElementTypes.Contains(-1))
                {
                    args.Display.DrawMeshWires(m.RenderMesh, c);
                }
                else
                {
                    int[] hf;
                    Point3d[] pts;
                    foreach (IElement e in m.Elements)
                    {
                        if (e.TopologicDimension == 2)
                        {
                            pts = IRhinoGeometry.GetPointsFromElements(e.Vertices, m);
                            args.Display.DrawPolyline(pts, c);
                        }
                        else
                        {
                            if (!m.IsMultidimensionalMesh)
                            {
                                for (int i = 1; i <= e.HalfFacetsCount; i++)
                                {
                                    if (e.IsNakedSiblingHalfFacet(i))
                                    {
                                        e.GetHalfFacet(i, out hf);
                                        pts = IRhinoGeometry.GetPointsFromElements(hf, m);
                                        args.Display.DrawPolyline(pts, c);
                                    }
                                }
                            }
                            else
                            {
                                if (e.IsBoundaryElement())
                                {
                                    for (int i = 1; i <= e.HalfFacetsCount; i++)
                                    {
                                        e.GetHalfFacet(i, out hf);
                                        pts = IRhinoGeometry.GetPointsFromElements(hf, m);
                                        args.Display.DrawPolyline(pts, c);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            IGH_StructureEnumerator data = this.VolatileData.AllData(true);
            Rhino.Display.DisplayMaterial mat = args.ShadeMaterial;
            if (Attributes.Selected) mat = args.ShadeMaterial_Selected;

            foreach (IMesh m in data)
            {
                args.Display.DrawMeshShaded(m.RenderMesh, mat);
            }
        }

        private bool m_hidden = false;
        public bool Hidden
        {
            get { return m_hidden; }
            set { m_hidden = value; }
        }

        public bool IsPreviewCapable
        {
            get { return true; }
        }
        #endregion
    }
}
