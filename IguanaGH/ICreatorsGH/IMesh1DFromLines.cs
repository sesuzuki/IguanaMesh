using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace IguanaMeshGH.ICreatorsGH
{
    public class IMesh1DFromLines : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMesh1DFromLines class.
        /// </summary>
        public IMesh1DFromLines()
          : base("iLineGraph", "iLineGraph",
              "Create a one-dimensional mesh from a collection of lines.",
              "Iguana", "Creators")
        { 
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("Lines", "Ln", "Collection of lines.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana surface mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Line> lines = new List<Line>();
            DA.GetDataList(0, lines);

            PointCloud cloud = new PointCloud();
            List<IElement> elements = new List<IElement>();
            List<ITopologicVertex> vertices = new List<ITopologicVertex>();

            foreach (Line ln in lines)
            {

                Point3d p1 = ln.From;
                Point3d p2 = ln.To;

                int idx1 = cloud.ClosestPoint(p1);
                if (idx1 == -1)
                {
                    cloud.Add(p1);
                    idx1 = 1;
                    vertices.Add(new ITopologicVertex(p1.X,p1.Y,p1.Z, idx1));
                }
                else
                {
                    if (p1.DistanceTo(cloud[idx1].Location) > 0.01)
                    {
                        cloud.Add(p1);
                        idx1 = cloud.Count;
                        vertices.Add(new ITopologicVertex(p1.X, p1.Y, p1.Z, idx1));
                    }
                    else idx1 ++;
                }

                int idx2 = cloud.ClosestPoint(p2);
                if (p2.DistanceTo(cloud[idx2].Location) > 0.01)
                {
                    cloud.Add(p2);
                    idx2 = cloud.Count;
                    vertices.Add(new ITopologicVertex(p2.X, p2.Y, p2.Z, idx2));
                }
                else idx2 ++;

                elements.Add(new IBarElement(idx1, idx2));
            }

            IMesh mesh = new IMesh();
            mesh.AddRangeVertices(vertices);
            mesh.AddRangeElements(elements);
            mesh.BuildTopology();

            DA.SetData(0, mesh);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0fc3100f-315a-47bd-8753-a60726798349"); }
        }
    }
}