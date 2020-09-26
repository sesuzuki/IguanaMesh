using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IUtilsGH
{
    public class IHexahedronSolidGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IHexahedronSolidGH class.
        /// </summary>
        public IHexahedronSolidGH()
          : base("iElement Hexahedron Constructor", "iHexahedron",
              "A three-dimensional element for volumetric meshes.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("String-sets of vertex Indexes", "S", "Formatted string-set of vertex indexes to build an AHF-IHexahedronSolid object.\nThe set structure is defined as: Q{item1; item2; item3, ...}. \n\n" +
                "NOTE: Vertices on an iElement needs to be sorted according to the CFD General Notation System.\nSee: https://cgns.github.io/CGNS_docs_current/sids/conv.html", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iHexahedron", "iE", "The three-dimensional element.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> _vertexIndexes = new List<string>();

            //Retreiving the list of formatted vertex indexes
            DA.GetDataList(0, _vertexIndexes);

            //Construction of Elements
            List<IHexahedronElement> elements = new List<IHexahedronElement>();

            for (int i = 0; i < _vertexIndexes.Count; i++)
            {
                string eText = _vertexIndexes[i];

                // Creation of AHF-IFace from 
                if (eText.Contains("Q"))
                {
                    string[] vText = eText.Split(new char[] { 'Q', '{', '}' })[2].Split(';');

                    List<int> vIdx = new List<int>();

                    //Parse string to integer
                    Boolean flag = true;
                    foreach (string s in vText)
                    {
                        int idx = -1;
                        flag = int.TryParse(s, out idx);
                        if (!vIdx.Contains(idx)) vIdx.Add(idx);
                    }

                    //Try to create an AHF_Element
                    if (flag && vIdx.Count >= 3) elements.Add(new IHexahedronElement(vIdx.ToArray()));

                }
            }

            if (elements.Contains(null)) this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Some errors on the creation of AHF-IElements were found. ");
            DA.SetDataList(0, elements);
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
            get { return new Guid("53c22774-921d-469a-b0ec-22805f8f960d"); }
        }
    }
}