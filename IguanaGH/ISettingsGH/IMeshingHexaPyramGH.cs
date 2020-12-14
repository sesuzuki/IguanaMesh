/*
 * <IguanaMesh>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using Grasshopper.Kernel;
using Iguana.IguanaMesh;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.ISettings
{
    public class IMeshingHexaPyramGH : GH_Component
    {
        ISolver3D solverOpt;
        MeshSolvers3D solver = MeshSolvers3D.Delaunay;
        double sizeFactor = 1.0, qualityThreshold = 0.3, size = 1.0;
        int recombine = 2, optimization = 0, qualityType = 2, minElemPerTwoPi = 6, subD;
        bool adaptive = false;

        /// <summary>
        /// Initializes a new instance of the IMeshingHexahedronGH class.
        /// </summary>
        public IMeshingHexaPyramGH()
          : base("iLinearHexaPyra", "iLinearHexaPyra",
              "Solver configuration for linear hexa-pyra mesh generation.",
              "Iguana", "Settings")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Size Factor", "SizeFactor", "Factor applied to all mesh element sizes. Default value is " + sizeFactor, GH_ParamAccess.item, sizeFactor);
            pManager.AddNumberParameter("Size", "Size", "Target size of mesh element. Default value is " + size, GH_ParamAccess.item, size);
            pManager.AddNumberParameter("Minimum Size", "MinSize", "Minimum mesh element size.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Maximun Size", "MaxSize", "Maximum mesh element size.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Curvature Adapt", "Adaptive", "Automatically compute mesh element sizes from curvature. It overrides the target global mesh element size at input nodes. Default value is " + adaptive.ToString(), GH_ParamAccess.item, adaptive);
            pManager.AddIntegerParameter("Minimum Elements", "MinElements", "Minimum number of elements per 2PI. Default value is " + minElemPerTwoPi, GH_ParamAccess.item, minElemPerTwoPi);
            pManager.AddIntegerParameter("Optimization", "Optimization", "Optimization method (0:Standard, 1: NetGen). Default value is " + optimization, GH_ParamAccess.item, optimization);
            pManager.AddIntegerParameter("Quality Type", "Quality", "Type of quality measure for element optimization (0: SICN => signed inverse condition number, 1: SIGE => signed inverse gradient error, 2: gamma => vol/sum_face/max_edge, 3: Disto => minJ/maxJ). Default value is " + qualityType, GH_ParamAccess.item, qualityType);
            pManager.AddNumberParameter("Quality Threshold", "Threshold", "Quality threshold for element optimization. Default value is " + qualityThreshold, GH_ParamAccess.item, qualityThreshold);
            pManager.AddIntegerParameter("Recombination Method", "Method", "Method to combine triangles into quadrangles (1: Blossom, 2: Simple full-quad, 3: Blossom full-quad). Default value is " + recombine, GH_ParamAccess.item, recombine);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iSettings", "iSettings", "Solver configuration for linear hexa-pyra mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            solverOpt = new ISolver3D();
            double maxSize = 1, minSize = 1;

            DA.GetData(0, ref sizeFactor);
            DA.GetData(1, ref size);
            DA.GetData(2, ref minSize);
            DA.GetData(3, ref maxSize);
            DA.GetData(4, ref adaptive);
            DA.GetData(5, ref minElemPerTwoPi);
            DA.GetData(6, ref optimization);
            DA.GetData(7, ref qualityType);
            DA.GetData(8, ref qualityThreshold);
            DA.GetData(9, ref recombine);

            solverOpt.MeshingAlgorithm = solver;
            solverOpt.CharacteristicLengthFactor = sizeFactor;
            solverOpt.CharacteristicLengthMin = minSize;
            solverOpt.CharacteristicLengthMax = maxSize;
            solverOpt.CharacteristicLengthFromCurvature = adaptive;
            solverOpt.OptimizeThreshold = qualityThreshold;

            if (optimization != 0 || optimization != 1) optimization = 0;
            solverOpt.OptimizationMethod = optimization;

            string method = Enum.GetName(typeof(ElementQualityType), qualityType);
            if (method != null) qualityType = 2;
            solverOpt.QualityType = qualityType;

            method = Enum.GetName(typeof(RecombinationAlgorithm), recombine);
            if (method == null) recombine = 2;
            solverOpt.RecombinationAlgorithm = recombine;
            solverOpt.RecombineAll = true;

            method = Enum.GetName(typeof(SubdivisionAlgorithm), subD);
            if (method == null) subD = 1;
            solverOpt.SubdivisionAlgorithm = subD;
            solverOpt.Subdivide = true;

            DA.SetData(0, solverOpt);

            this.Message = "8Hexa+5Pyra";
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
                return Properties.Resources.iHexahedronSettings;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("be4e1f2c-8d6c-4da2-8282-fa1e75b16aad"); }
        }
    }
}