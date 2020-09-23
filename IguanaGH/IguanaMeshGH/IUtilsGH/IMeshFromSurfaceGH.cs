using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IGmshWrappers;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IMeshFromSurfaceGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMeshFromEdges class.
        /// </summary>
        public IMeshFromSurfaceGH()
          : base("iMesh from Surface", "iMeshFromSurface",
              "General constructor for an Array-based Half-Facet (AHF) Mesh Data Structure.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "Srf", "Surface to mesh", GH_ParamAccess.item);
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
            Surface srf = null;
            IguanaGmshSolverOptions solverOpt = new IguanaGmshSolverOptions();
            IMesh mesh = null;

            DA.GetData(0, ref srf);
            DA.GetData(1, ref solverOpt);

            NurbsSurface nSrf = srf.ToNurbsSurface();
            NurbsSurfacePointList ctr_pts =  nSrf.Points;
            int countU = ctr_pts.CountU;
            int countV = ctr_pts.CountV;
            int idx = 0;

            Gmsh.Initialize();

            ControlPoint cP;
            int[] nTags = new int[countU * countV];
            double[] weights = new double[countU * countV];
            for (int i=0; i<countU; i++)
            {
                for(int j=0; j<countV; j++)
                {
                    cP = ctr_pts.GetControlPoint(i, j);
                    nTags[idx] = Gmsh.Model.GeoOCC.AddPoint(cP.X, cP.Y, cP.Z);
                    weights[idx] = ctr_pts.GetWeight(i, j);
                    idx++;
                }
            }

            Gmsh.Model.GeoOCC.AddBSplineSurface(nTags, countU, nSrf.Degree(0), nSrf.Degree(1), weights);

            Gmsh.Model.GeoOCC.Synchronize();

            //solver options
            Gmsh.Option.SetNumber("Mesh.Algorithm", (int)solverOpt.MeshingAlgorithm);
            Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFactor", solverOpt.CharacteristicLengthFactor);
            Gmsh.Option.SetNumber("Mesh.CharacteristicLengthMin", solverOpt.CharacteristicLengthMin);
            Gmsh.Option.SetNumber("Mesh.CharacteristicLengthMax", solverOpt.CharacteristicLengthMax);

            if (solverOpt.CharacteristicLengthFromCurvature)
            {
                //Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFromParametricPoints", 0);
                //Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 1);
            }
            else
            {
                //Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFromCurvature", 0);
                //Gmsh.Option.SetNumber("Mesh.CharacteristicLengthFromParametricPoints", 1);
            }

            Gmsh.Model.Mesh.Generate(2);

            if (solverOpt.RecombineAll)
            {
                Gmsh.Option.SetNumber("Mesh.RecombinationAlgorithm", solverOpt.RecombinationAlgorithm);
                Gmsh.Model.Mesh.Recombine();
            }

            if (solverOpt.Subdivide)
            {
                Gmsh.Option.SetNumber("Mesh.SubdivisionAlgorithm", solverOpt.SubdivisionAlgorithm);
                Gmsh.Model.Mesh.Refine();
            }

            if (solverOpt.Optimize)
            {

                Gmsh.Model.Mesh.Optimize(solverOpt.OptimizationAlgorithm, solverOpt.Smoothing);
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
            get { return new Guid("927b9c99-ecc6-4b18-b1be-9be051361169"); }
        }
    }
}