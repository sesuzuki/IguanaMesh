using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.IWrappers.IExtensions;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.IFieldsGH
{
    public class IMathFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMathFieldGH class.
        /// </summary>
        public IMathFieldGH()
          : base("iMathField", "iMathF",
              "Math field to specify the size of the mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Expression", "E", "Mathematical expression to evaluate.The expression can contain x, y, z for spatial coordinates, F0, F1, ... for field values, and mathematical functions.\nThe expression is used to modulate the mesh element sizes. Default expression is Cos(x) * Sin(y)", GH_ParamAccess.item, "Cos(x) * Sin(y)");
            pManager.AddGenericParameter("Fields", "F", "List of fields to evaluate.", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMeshField", "iMF", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string evalF = "Cos(x) * Sin(y)";
            DA.GetData(0, ref evalF);

            List<IguanaGmshField> fields = new List<IguanaGmshField>();
            foreach (var obj in base.Params.Input[1].VolatileData.AllData(true))
            {
                IguanaGmshField f;
                obj.CastTo<IguanaGmshField>(out f);
                if(f!=null) fields.Add(f);
            }

            IguanaGmshField.MathEval field = new IguanaGmshField.MathEval();
            field.F = evalF;
            field.Fields = fields;

            DA.SetData(0, field);
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
            get { return new Guid("c1284250-0b71-45c4-91b1-1ef7f94d0c3d"); }
        }
    }
}