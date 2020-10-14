using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IWrappers;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Iguana.IguanaMesh.IWrappers.ISolver;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IMesh3DFromBrepGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMesh3DFromBrepGH class.
        /// </summary>
        public IMesh3DFromBrepGH()
          : base("iBrepVolume", "iBVol",
              "Create a three-dimensional mesh from a brep.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Brep to mesh.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iSettings", "iS2D", "Two-dimensional meshing settings", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana Surface Mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep b = null;
            IguanaGmshSolver2D solverOptions = new IguanaGmshSolver2D();

            DA.GetData(0, ref b);
            DA.GetData(1, ref solverOptions);

            IMesh mesh = new IMesh();
            if (b.IsSolid)
            {
                // Extract required data from base surface
                IguanaGmsh.Initialize();

                // Brep construction
                IguanaGmshFactory.Geo.GmshVolumeFromBrep(b, true);

                // Preprocessing settings
                //solverOptions.ApplyBasic2DSettings();
                //solverOptions.ApplyAdvanced2DSettings();

                //Tuple<int, int>[] nodes;
                //IguanaGmsh.Model.GetEntities(out nodes, 0);
                //IguanaGmsh.Model.Mesh.SetSize(nodes, solverOptions.TargetMeshSizeAtNodes[0]);

                // 3d mesh generation
                IguanaGmsh.Model.Mesh.Generate(3);

                // Iguana mesh construction
                mesh = IguanaGmshFactory.TryGetIMesh(3);

                IguanaGmsh.FinalizeGmsh();
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
            get { return new Guid("b8a6a3ba-64f2-45a8-a928-f85f7da256a0"); }
        }
    }
}