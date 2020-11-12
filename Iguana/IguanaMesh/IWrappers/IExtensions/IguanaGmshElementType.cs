using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iguana.IguanaMesh.IWrappers.IExtensions
{
    public static class IguanaGmshElementType
    {
        internal static int[] implementedElements = new int[]{2,3,9,16,20,22,24,11,4,19,7,6,18,5,17};

        internal static bool IsElementImplemented(int elementType)
        {
            return implementedElements.Contains(elementType);
        }

        public static HashSet<int> TryParseToIguanaElement(int elementType, long[] nodes, int nodes_per_element, int number_of_elements, ref IMesh mesh)
        {
            HashSet<int> parsedNodes = new HashSet<int>();
            if (IsElementImplemented(elementType))
            {
                for (int j = 0; j < number_of_elements; j++)
                {
                    int[] eD = new int[nodes_per_element];

                    IElement e = null;

                    for (int k = 0; k < nodes_per_element; k++)
                    {
                        eD[k] = (int)nodes[j * nodes_per_element + k];
                        parsedNodes.Add(eD[k]);
                    }

                    switch (elementType)
                    {
                        //1st-order Triangle Face
                        case 2:
                            e = new ISurfaceElement(eD);
                            break;

                        //1st-order Quadrangle Face
                        case 3:
                            e = new ISurfaceElement(eD);
                            break;

                        //2nd-order 6-node triangle 
                        case 9:
                            e = new ISurfaceElement.HighOrder.ITriangle6(eD);
                            break;

                        //2nd-order 8-node quadrangle
                        case 16:
                            e = new ISurfaceElement.HighOrder.IQuadrangle8(eD);
                            break;

                        //3rd-order 9-node incomplete triangle 
                        case 20:
                            e = new ISurfaceElement.HighOrder.ITriangle9(eD);
                            break;

                        //4th-order 12-node incomplete triangle
                        case 22:
                            e = new ISurfaceElement.HighOrder.ITriangle12(eD);
                            break;

                        //5t-order 15-node incomplete triangle 
                        case 24:
                            e = new ISurfaceElement.HighOrder.ITriangle15(eD);
                            break;

                        //2nd-order 10-node tetrahedron
                        case 11:
                            e = new ITetrahedronElement.HighOrder.ITetrahedron10(eD);
                            break;

                        //1s-order 4-node tetrahedron element
                        case 4:
                            e = new ITetrahedronElement(eD);
                            break;

                        //2n-order 13-node pyramid
                        case 19:
                            e = new IPyramidElement.HighOrder.IPyramid13(eD);
                            break;

                        //1st-order 5-node pyramid element
                        case 7:
                            e = new IPyramidElement(eD);
                            break;

                        //1st-order 6-node prism element
                        case 6:
                            e = new IPrismElement(eD);
                            break;

                        //2nd-order 15-node prism
                        case 18:
                            e = new IPrismElement.HighOrder.IPrism15(eD);
                            break;

                        //1st-order 8-node hexahedron element
                        case 5:
                            e = new IHexahedronElement(eD);
                            break;

                        //2nd-order 20-node hexahedron
                        case 17:
                            e = new IHexahedronElement.HighOrder.IHexahedron20(eD);
                            break;
                    }

                    if (e != null) mesh.AddElement(e);
                }
            }
            return parsedNodes;
        }
    }
}
