using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IGmshWrappers;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.ICollections;
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

            solverOpt.ApplyBasicPreProcessing2D();

            Gmsh.Model.Mesh.Generate(2);

            solverOpt.ApplyBasicPostProcessing2D();

            // Iguana mesh construction
            IVertexCollection vertices = Gmsh.Model.Mesh.TryGetIVertexCollection();
            IElementCollection elements = Gmsh.Model.Mesh.TryGetIElementCollection();
            mesh = new IMesh(vertices, elements);
            mesh.BuildTopology();

            Gmsh.FinalizeGmsh();

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