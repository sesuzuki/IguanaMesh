using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IMinFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMinFieldGH class.
        /// </summary>
        public IMinFieldGH()
          : base("iMinField", "iMinF",
              "Min field to specify the size of the mesh elements. Take the minimum value of a list of fields.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Fields", "iF", "Fields to evaluate.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshField", "iF", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IguanaGmshField> fieldList = new List<IguanaGmshField>();
            DA.GetDataList(0, fieldList);

            IguanaGmshField.Min field = new IguanaGmshField.Min();
            field.FieldsList = fieldList;

            DA.SetData(0, field);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iMinField;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("77fd1b08-90f7-4da2-9409-4154f3e4385f"); }
        }
    }
}