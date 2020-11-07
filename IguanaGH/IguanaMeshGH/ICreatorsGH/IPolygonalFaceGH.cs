using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IPolygonalFaceGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public IPolygonalFaceGH()
          : base("iElement Polygonal Face Constructor", "iPolygonalFace",
              "A two-dimensional element for surface meshes.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("String-sets of vertex Indexes", "F", "Formatted string-set of vertex indexes to a build AHF-ISurfaceElement object.\nThe set structure is defined as: Q{item1; item2; item3, ...}. \n" +
                "MeshFace objects from Rhino are automatically converted into AHF-IFace objects.\n\n" +
                "NOTE: Vertices on an iElement needs to be sorted according to the CFD General Notation System.\nSee: https://cgns.github.io/CGNS_docs_current/sids/conv.html", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("AHF-ISurfaceElement", "iF", "Constructed AHF-ISurfaceElement", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> _vertexIndexes = new List<string>();

            //Retreiving the list of formatted vertex indexes
            DA.GetDataList(0, _vertexIndexes);

            //Construction of Elements
            List<ISurfaceElement> elements = new List<ISurfaceElement>();

            for(int i=0; i<_vertexIndexes.Count; i++)
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
                    if (flag && vIdx.Count>=3) elements.Add(new ISurfaceElement(vIdx.ToArray()));

                }
            }

            if(elements.Contains(null)) this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Some errors on the creation of AHF-IElements were found. ");
            DA.SetDataList(0, elements);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.AHF_IFace;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2677cda1-ada0-4c8d-b3b4-df61f886b242"); }
        }
    }
}
