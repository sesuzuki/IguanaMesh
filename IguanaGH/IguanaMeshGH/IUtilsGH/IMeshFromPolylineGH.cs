using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Iguana.IguanaGmshWrappers;
using Iguana.IguanaMesh.ITypes;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IMeshFromPolylineGH : GH_Component
    {
        IGmshSolverOptions solverOpt;
        /// <summary>
        /// Initializes a new instance of the IMeshFromPolyline class.
        /// </summary>
        public IMeshFromPolylineGH()
          : base("iMesh from closed polyline", "iMeshFromPolyline",
              "General constructor for an Array-based Half-Facet (AHF) Mesh Data Structure.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Outer boundary", "Outer", "External boundary", GH_ParamAccess.item);
            pManager.AddCurveParameter("Inner boundaries", "Inner", "Holes as polylines", GH_ParamAccess.list);
            pManager.AddGenericParameter("Meshing Settings", "Settings", "Meshing settings", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Constructed Array-Based Half-Facet (AHF) Mesh Data Structure.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve _outer = null;
            List<Curve> _inner = new List<Curve>();

            //Retreive vertices and elements
            DA.GetData(0, ref _outer);
            DA.GetDataList(1, _inner);
            DA.GetData(2, ref solverOpt);

            IMesh mesh = null;

            if (_outer.IsClosed)
            {
                Gmsh.Initialize();

                Polyline poly;
                _outer.TryGetPolyline(out poly);

                int[] pt_tags = new int[poly.Count - 1];
                double size = 0.1;
                for (int i = 0; i < poly.Count - 1; i++)
                {
                    Point3d pt = poly[i];
                    size=solverOpt.TargetMeshSize[0];
                    if(solverOpt.TargetMeshSize.Count == poly.Count - 1) size = solverOpt.TargetMeshSize[i];
                    pt_tags[i] = Gmsh.Model.Geo.AddPoint(pt.X, pt.Y, pt.Z, size);
                }

                int[] ln_tags = new int[pt_tags.Length];
                for (int i = 0; i < pt_tags.Length; i++)
                {
                    int start = pt_tags[i];
                    int end = pt_tags[0];
                    if (i < pt_tags.Length - 1) end = pt_tags[i + 1];

                    ln_tags[i] = Gmsh.Model.Geo.AddLine(start, end);
                }

                //TODO Include inner boundaries
                int pl_tag = Gmsh.Model.Geo.AddCurveLoop(ln_tags, 1);

                Gmsh.Model.Geo.AddPlaneSurface(new int[] { pl_tag }, 1);

                Gmsh.Model.Geo.Synchronize();

                //solver options
                Gmsh.Option.SetNumber("Mesh.Algorithm", (int) solverOpt.MeshingAlgorithm);
                Gmsh.Option.SetNumber("Mesh.AllowSwapAngle", solverOpt.AllowSwapAngle);
                Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFactor", solverOpt.CharacteristicLengthFactor);
                Gmsh.Option.SetNumber("Mesh.CharacteristicLengthMin", solverOpt.CharacteristicLengthMin);
                Gmsh.Option.SetNumber("Mesh.CharacteristicLengthMax", solverOpt.CharacteristicLengthMax);
                Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", Convert.ToDouble(solverOpt.CharacteristicLengthFromCurvature));
                //Gmsh.Option.SetNumber("Mesh.RandomSeed", solverOpt.RandomSeed);
                Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFromParametricPoints", 1);
                //Gmsh.Option.SetNumber("Mesh.RecombinationAlgorithm", solverOpt.RecombinationAlgorithm);
                //Gmsh.Option.SetNumber("Mesh.RecombineOptimizeTopology", solverOpt.RecombineOptimizeTopology);

                Gmsh.Model.Mesh.Generate(2);

                if (solverOpt.Optimize)
                {
                    Gmsh.Option.SetNumber("Mesh.Optimize", 1);
                    //Gmsh.Option.SetNumber("Mesh.RecombineOptimizeTopology", solverOpt.RecombineOptimizeTopology);
                    Gmsh.Model.Mesh.Optimize(Gmsh.Model.Mesh.OptimizationMethod.Laplace2D, 5);
                }


                int[] nodesTag;
                double[][] coords, uvw;
                Gmsh.Model.Mesh.GetNodes(out nodesTag, out coords, out uvw, 2);

                int[][][] elementTags;
                Gmsh.Model.Mesh.GetElements(out elementTags, 2);
                Gmsh.FinalizeGmsh();

                ITopologicVertex[] nodes = new ITopologicVertex[nodesTag.Length];
                for (int i = 0; i < nodesTag.Length; i++)
                {
                    nodes[i] = new ITopologicVertex(coords[i][0], coords[i][1], coords[i][2]);
                }

                List<IElement> elements = new List<IElement>();
                for (int i = 0; i < elementTags.Length; i++)
                {
                    for (int j = 0; j < elementTags[i].Length; j++)
                    {
                        IPolygonalFace face = new IPolygonalFace(elementTags[i][j]);
                        elements.Add(face);
                    }
                }

                mesh = new IMesh(nodes, nodesTag, elements);
                mesh.BuildTopology();
            }

            DA.SetData(0, mesh);
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
            get { return new Guid("40271dbd-73f2-4d37-bfd0-0d5ba5066999"); }
        }
    }
}