/*
 * <Iguana>
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

using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using Rhino.DocObjects;
using System.IO;
using Rhino;
using Grasshopper.Kernel.Data;
using Rhino.FileIO;
using Rhino.Display;
using System.Drawing;

namespace Iguana.IguanaMesh
{
    public static partial class IKernel
    {
        public static class IMeshingKernel
        {
            public enum MeshFormats { msh = 0, stl = 1 }

            #region Iguana Methods

            /// <summary>
            /// Expor IMesh.
            /// </summary>
            /// <param name="mesh"> Base IMesh. </param>
            /// <param name="filePath"> File path. </param>
            /// <returns></returns>
            public static void ExportToFile(IMesh mesh, string filePath)
            {
                Initialize();

                int dim = 2;
                if (mesh.IsVolumeMesh) dim = 3;
                int tag = 1;

                ImportIMesh(mesh, dim, tag);

                Write(filePath);

                End();
            }

            /// <summary>
            /// Import a mesh file and convert data into an IMesh.
            /// </summary>
            /// <param name="filePath"> File path. </param>
            /// <param name="mesh"> Iguana mesh. </param>
            /// <returns> Success of the operation. </returns>
            public static bool ImportFromFile(String filePath, out IMesh mesh)
            {
                bool flag = false;
                string ext = Path.GetExtension(filePath);

                foreach (string format in Enum.GetNames(typeof(MeshFormats)))
                {
                    string eval = format.Insert(0, ".");
                    if (eval.Equals(ext))
                    {
                        flag = true;
                        break;
                    }
                }

                mesh = new IMesh();
                if (flag)
                {
                    Initialize();
                    Merge(filePath);

                    Tuple<int, int>[] dimTags;
                    IKernel.IModel.GetEntities(out dimTags);
                    mesh = TryCreateIMesh(dimTags[0].Item1);

                    End();
                }

                return flag;
            }

            /// <summary>
            /// Import a rhino mesh.
            /// </summary>
            /// <param name="mesh"> Rhino mesh. </param>
            /// <param name="doc"> Rhino document. </param>
            /// <param name="synchronize"> Synchronize the model. </param>
            /// <returns></returns>
            public static Tuple<int, int>[] ImportRhinoMesh(Mesh mesh, RhinoDoc doc, bool synchronize = true)
            {
                ImportMesh(mesh, 2, 1);

                Tuple<int, int>[] dimTags;
                IModel.GetEntities(out dimTags, -1);

                return dimTags;
            }

            /// <summary>
            /// Import rhino geometry.
            /// </summary>
            /// <param name="geom"> Rhino geometry. Default is the current active Rhino doc. </param>
            /// <param name="doc"> Rhino document. </param>
            /// <param name="synchronize"> Synchronize the model. </param>
            /// <returns></returns>
            public static Tuple<int, int>[] ImportRhinoGeometry(GeometryBase geom, bool synchronize = true)
            {
                var filename = Path.ChangeExtension(Path.GetTempFileName(), ".step");

                RhinoDoc doc = RhinoDoc.CreateHeadless(null);

                Guid id = doc.Objects.Add(geom);

                Tuple<int, int>[] dimTags = new Tuple<int, int>[] { };
                if (id != Guid.Empty)
                {
                    FileStp.Write(filename, doc, new FileStpWriteOptions());
                    doc.Dispose();

                    IGeometryOCCKernel.IBuilder.ImportShapes(filename, out dimTags);
                    SetOptionString("OCCTargetUnit", "M");

                    File.Delete(filename);
                    if (synchronize) IGeometryOCCKernel.IBuilder.Synchronize();
                }

                return dimTags;
            }

            /// <summary>
            /// Import a collection of rhino geometries.
            /// </summary>
            /// <param name="geom"> Rhino geometry. Default is the current active Rhino doc. </param>
            /// <param name="doc"> Rhino document. </param>
            /// <param name="synchronize"> Synchronize the model. </param>
            /// <returns></returns>
            public static Tuple<int, int>[] ImportRhinoGeometry(IEnumerable<GeometryBase> geom, bool synchronize = true)
            {
                var filename = Path.ChangeExtension(Path.GetTempFileName(), ".step");

                RhinoDoc doc = RhinoDoc.CreateHeadless(null);

                int count = geom.Count();
                Guid[] id = new Guid[count];
                ObjRef[] obj = new ObjRef[count];
                for (int i = 0; i < count; i++)
                {
                    id[i] = doc.Objects.Add(geom.ElementAt(i));
                    obj[i] = new ObjRef(id[i]);
                }

                FileStp.Write(filename, doc, new FileStpWriteOptions());
                doc.Dispose();

                Tuple<int, int>[] dimTags = new Tuple<int, int>[] { };
                IGeometryOCCKernel.IBuilder.ImportShapes(filename, out dimTags);
                SetOptionString("OCCTargetUnit", "M");

                File.Delete(filename);
                if (synchronize) IGeometryOCCKernel.IBuilder.Synchronize();
                
                return dimTags;
            }

            /// <summary>
            /// Set the size of elements before meshing.
            /// </summary>
            /// <param name="size"> Target element size. </param>
            internal static void SetMeshSize(double size)
            {
                Tuple<int, int>[] dimTags;
                IKernel.IModel.GetEntities(out dimTags, 0);
                IBuilder.SetSize(dimTags, size);
            }

            /// <summary>
            /// Apply transfinite constraints along the meshing process.
            /// </summary>
            /// <param name="transfinite"> List of transfinite constraints. </param>
            internal static void ApplyTransfiniteSettings(List<ITransfinite> transfinite)
            {
                if (transfinite.Count == 0) return;
                if (transfinite == default) return;

                foreach (ITransfinite t in transfinite)
                {
                    switch (t.Dim)
                    {
                        case 1:
                            IBuilder.SetTransfiniteCurve(t.Tag, t.NodesNumber, t.MethodType, t.Coef);
                            break;
                        case 2:
                            IBuilder.SetTransfiniteSurface(t.Tag, t.MethodType, t.Corners);
                            break;
                        case 3:
                            // TransfiniteVolume in gmsh_4.6 is not producing any result
                            // Alternative solution

                            Tuple<int, int>[] dimTags;
                            IModel.GetBoundary(new Tuple<int, int>[] { Tuple.Create(3, t.Tag) }, out dimTags);
                            foreach (Tuple<int, int> keyPair in dimTags)
                            {
                                IBuilder.SetTransfiniteSurface(keyPair.Item2, t.MethodType);
                            }
                            break;
                    }
                }
            }

            /// <summary>
            /// Get the underlying information (dimension and tag) of the entities created during meshing. 
            /// </summary>
            /// <returns> A data tree of entities information. </returns>
            internal static GH_Structure<IEntityInfo> GetUnderlyingEntitiesInformation()
            {
                Tuple<int, int>[] dimTags;
                IKernel.IModel.GetEntities(out dimTags, -1);

                GH_Structure<IEntityInfo> infoTree = new GH_Structure<IEntityInfo>();
                GH_Path path;
                IEntityInfo info;

                for (int i = 0; i < dimTags.Length; i++)
                {
                    int dim = dimTags[i].Item1;
                    int tag = dimTags[i].Item2;

                    path = new GH_Path(dim);

                    double[] coord;
                    switch (dim)
                    {
                        case 0:
                            IModel.GetValue(dim, tag, new double[] { }, out coord);
                            info = new IEntityInfo(dim, tag, new Point3d(coord[0], coord[1], coord[2]));
                            break;
                        case 1:
                            IBuilder.GetCenter(dim, tag, out coord);
                            info = new IEntityInfo(dim, tag, new Point3d(coord[0], coord[1], coord[2]));
                            break;
                        default:
                            double xmin, ymin, zmin, xmax, ymax, zmax;
                            IModel.GetBoundingBox(dim, tag, out xmin, out ymin, out zmin, out xmax, out ymax, out zmax);
                            coord = new double[] { (xmin + xmax) / 2, (ymin + ymax) / 2, (zmin + zmax) / 2 };
                            info = new IEntityInfo(dim, tag, new Point3d(coord[0], coord[1], coord[2]));
                            info.SetBoundingBoxParameters(xmin, ymin, zmin, xmax, ymax, zmax);
                            break;
                    }
                    infoTree.Append(info, path);
                }
                return infoTree;
            }

            /// <summary>
            /// Try to create the Iguana mesh.
            /// </summary>
            /// <param name="dim"> Dimension of the mesh. </param>
            /// <returns> Iguana Mesh </returns>
            public static IMesh TryCreateIMesh(int dim = 2)
            {
                if (dim > 3) dim = 3;
                else if (dim < 2) dim = 2;

                IMesh mesh = new IMesh();
                HashSet<int> parsedNodes;
                TryCreateITopologicVertices(ref mesh);
                TryCreateIElements(ref mesh, out parsedNodes, dim);
                if (parsedNodes.Count < mesh.VerticesCount) mesh.CullUnparsedNodes(parsedNodes);
                mesh.BuildTopology(true);

                return mesh;
            }

            /// <summary>
            /// Try to create the set of ITopologicVertices from the meshing process.
            /// </summary>
            /// <param name="mesh"> Iguana mesh. </param>
            /// <param name="dim"> Dimension of meshing proces. </param>
            /// <param name="tag"> Tag of the underlying model to work with. If tag is -1 (default), nodes on all models are going to be searched. </param>
            /// <returns></returns>
            private static bool TryCreateITopologicVertices(ref IMesh mesh, int dim = -1, int tag = -1)
            {
                IntPtr nodeTags, coord, parametricCoord;
                long nodeTags_Number, coord_Number, parametricCoord_Number;
                IWrap.GmshModelMeshGetNodes(out nodeTags, out nodeTags_Number, out coord, out coord_Number, out parametricCoord, out parametricCoord_Number, dim, tag, Convert.ToInt32(true), Convert.ToInt32(true), ref _ierr);
                mesh.CleanVertices();

                if (nodeTags_Number > 0)
                {
                    // Coordinates
                    var xyz = new double[coord_Number];
                    Marshal.Copy(coord, xyz, 0, (int)coord_Number);
                    // Keys
                    var keys = new long[nodeTags_Number];
                    Marshal.Copy(nodeTags, keys, 0, (int)nodeTags_Number);
                    // uvw
                    var uvw = new double[parametricCoord_Number];
                    Marshal.Copy(parametricCoord, uvw, 0, (int)parametricCoord_Number);

                    for (int i = 0; i < nodeTags_Number; i++)
                    {
                        ITopologicVertex v = new ITopologicVertex(xyz[i * 3], xyz[i * 3 + 1], xyz[i * 3 + 2]);

                        if (!mesh.ContainsVertexKey((int)keys[i])) mesh.AddVertex((int)keys[i], v);
                    }
                }

                // Delete unmanaged allocated memory
                IWrap.GmshFree(nodeTags);
                IWrap.GmshFree(coord);
                IWrap.GmshFree(parametricCoord);

                return true;
            }

            /// <summary>
            /// Try to create the set of IElements from the meshing process.
            /// </summary>
            /// <param name="dim"> 2 for surface element, 3 for volume elements and -1 for all elements. Default is -1. </param>
            /// <returns></returns>
            private static bool TryCreateIElements(ref IMesh mesh, out HashSet<int> parsedNodes, int dim = -1)
            {
                parsedNodes = new HashSet<int>();
                mesh.CleanElements();
                try
                {
                    IntPtr elementTypes, elementTags, nodeTags, elementTags_n, nodeTags_n;
                    long elementTypes_Number, elementTags_NNumber, nodeTags_NNumber;

                    IWrap.GmshModelMeshGetElements(out elementTypes, out elementTypes_Number, out elementTags, out elementTags_n, out elementTags_NNumber, out nodeTags, out nodeTags_n, out nodeTags_NNumber, dim, -1, ref _ierr);

                    var eTypes = new int[elementTypes_Number];
                    var eTags_n = new long[elementTags_NNumber];
                    var nTags_n = new long[nodeTags_NNumber];

                    Marshal.Copy(elementTypes, eTypes, 0, (int)elementTypes_Number);
                    Marshal.Copy(elementTags_n, eTags_n, 0, (int)elementTags_NNumber);
                    Marshal.Copy(nodeTags_n, nTags_n, 0, (int)nodeTags_NNumber);

                    var nTags_ptr = new IntPtr[nodeTags_NNumber];
                    var eTags_ptr = new IntPtr[elementTags_NNumber];

                    Marshal.Copy(nodeTags, nTags_ptr, 0, (int)nodeTags_NNumber);
                    Marshal.Copy(elementTags, eTags_ptr, 0, (int)elementTags_NNumber);

                    for (int i = 0; i < elementTags_NNumber; i++)
                    {
                        // Initializing containers
                        var eTags_val = new long[eTags_n[i]];
                        var nTags_val = new long[nTags_n[i]];

                        // Marshalling
                        Marshal.Copy(eTags_ptr[i], eTags_val, 0, (int)eTags_n[i]);
                        Marshal.Copy(nTags_ptr[i], nTags_val, 0, (int)nTags_n[i]);

                        // Building elements
                        int nodes_per_element = (int)(nTags_n[i] / eTags_n[i]);
                        //int number_of_elements = nTags_val.Length / nodes_per_element;

                        IKernel.IElementParser.TryParseToIguanaElement(eTypes[i], nTags_val, nodes_per_element, (int)eTags_n[i], ref parsedNodes, ref mesh);
                    }

                    // Delete unmanaged allocated memory
                    IWrap.GmshFree(elementTypes);
                    IWrap.GmshFree(elementTags);
                    IWrap.GmshFree(nodeTags);
                    IWrap.GmshFree(elementTags_n);
                    IWrap.GmshFree(nodeTags_n);

                    for (int i = 0; i < nTags_ptr.Length; i++)
                    {
                        IWrap.GmshFree(nTags_ptr[i]);
                    }
                    for (int i = 0; i < eTags_ptr.Length; i++)
                    {
                        IWrap.GmshFree(eTags_ptr[i]);
                    }

                    return true;
                }
                catch (Exception) { return false; }
            }

            /// <summary>
            /// Import an Iguana mesh.
            /// </summary>
            /// <param name="mesh"> Base Iguana mesh. </param>
            /// <param name="dim"> Underlying discrete mode dimension. </param>
            /// <param name="tag"> Underlying discrete model tag. </param>
            /// <param name="triangulate"> Return triangulated data from the given Iguana mesh </param>
            /// <param name="angle"></param>
            /// <param name="curveAngle"></param>
            /// <param name="forceParametrizablePatches"> For complex geometries, patches can be too complex, too elongated or too large to be parametrized; setting the following option will force the creation of patches that are amenable to reparametrization:</param>
            /// <param name="includeBoundary"> For open surfaces include the boundary edges in the classification process: </param>
            public static void ImportIMesh(IMesh mesh, int dim, int tag, bool triangulate = false, double angle = 0, double curveAngle = 180, bool forceParametrizablePatches = true, bool includeBoundary = true)
            {
                int[] elementTypes;
                long[][] elementTags, elementNodes;
                long[] nodeTags;
                double[] position;
                if (!triangulate)
                {
                    ParseIElementData(mesh, out elementTypes, out elementTags, out elementNodes);
                    ParseITopologicVertexData(mesh, out nodeTags, out position);
                }
                else ParseIMeshData(mesh, out nodeTags, out position, out elementTypes, out elementTags, out elementNodes);

                IModel.AddDiscreteEntity(dim, tag, new int[] { });
                IBuilder.AddNodes(dim, tag, nodeTags, position);

                for (int i = 0; i < elementTypes.Length; i++)
                {
                    IBuilder.AddElementsByType(tag, elementTypes[i], elementTags[i], elementNodes[i]);
                }

                IBuilder.ClassifySurfaces(angle * Math.PI / 180, includeBoundary, forceParametrizablePatches, curveAngle * Math.PI / 180);
                IBuilder.CreateGeometry();

                IGeometryKernel.IBuilder.Synchronize();
            }

            /// <summary>
            /// Import an Iguana mesh.
            /// </summary>
            /// <param name="mesh"> Base Iguana mesh. </param>
            /// <param name="dim"> Underlying discrete mode dimension. </param>
            /// <param name="tag"> Underlying discrete model tag. </param>
            /// <param name="angle"></param>
            /// <param name="curveAngle"></param>
            /// <param name="forceParametrizablePatches"> For complex geometries, patches can be too complex, too elongated or too large to be parametrized; setting the following option will force the creation of patches that are amenable to reparametrization:</param>
            /// <param name="includeBoundary"> For open surfaces include the boundary edges in the classification process: </param>
            public static void ImportMesh(Mesh mesh, int dim, int tag, double angle = 0, double curveAngle = 180, bool forceParametrizablePatches = true, bool includeBoundary = true)
            {
                int[] elementTypes;
                long[][] elementTags, elementNodes;
                long[] nodeTags;
                double[] position;

                ParseRhinoFaceData(mesh, out elementTypes, out elementTags, out elementNodes);
                ParseRhinoVertexData(mesh, out nodeTags, out position);

                IModel.AddDiscreteEntity(dim, tag, new int[] { });
                IBuilder.AddNodes(dim, tag, nodeTags, position);

                for (int i = 0; i < elementTypes.Length; i++)
                {
                    IBuilder.AddElementsByType(tag, elementTypes[i], elementTags[i], elementNodes[i]);
                }

                IBuilder.ClassifySurfaces(angle * Math.PI / 180, includeBoundary, forceParametrizablePatches, curveAngle * Math.PI / 180);
                IBuilder.CreateGeometry();

                IGeometryKernel.IBuilder.Synchronize();
            }

            /// <summary>
            /// Create a shell mesh from a brep. 
            /// </summary>
            /// <param name="brep"> Base brep. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromBrep(Brep brep, ISolver2D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                logInfo = Initialize();
                StartLogger();

                ImportRhinoGeometry(brep);

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryOCCKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                IBuilder.Generate(2);

                IMesh mesh = TryCreateIMesh();
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a shell mesh from a curve extrusion.
            /// </summary>
            /// <param name="crv"> Base curve. </param>
            /// <param name="direction"> Extrusion direction. </param>
            /// <param name="magnitude"> Length of the extrusion. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromCurveExtrusion(Curve crv, Vector3d direction, double magnitude, ISolver2D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                logInfo = Initialize();
                StartLogger();

                int[] crvTags;
                direction.Unitize();
                direction *= magnitude;

                if (crv.IsPolyline())
                {
                    crvTags = IGeometryKernel.CreateUnderlyingLinesFromCurve(crv, solver.Size);
                }
                else
                {
                    crvTags = IGeometryKernel.CreateUnderlyingSplinesFromCurve(crv, solver.Size);
                }

                Tuple<int, int>[] dimTags = new Tuple<int, int>[crvTags.Length];
                for (int i = 0; i < crvTags.Length; i++)
                {
                    dimTags[i] = Tuple.Create(1, crvTags[i]);
                }

                Tuple<int, int>[] outDimTags;
                IGeometryKernel.IBuilder.Extrude(dimTags, direction.X, direction.Y, direction.Z, out outDimTags);

                IGeometryKernel.IBuilder.Synchronize();

                if (!crv.IsPolyline())
                {
                    IKernel.IModel.GetEntities(out outDimTags, 2);
                    int[] temp = new int[outDimTags.Length];
                    for (int i = 0; i < temp.Length; i++) temp[i] = outDimTags[i].Item2;

                    IBuilder.SetCompound(2, temp);
                }

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                IBuilder.Generate(2);
                IMesh mesh = TryCreateIMesh(2);
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a shell mesh from a closed curve. 
            /// </summary>
            /// <param name="crv"> Base closed curve. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromClosedCurve(Curve crv, ISolver2D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                logInfo = Initialize();
                StartLogger();

                Tuple<int, int>[] v = ImportRhinoGeometry(crv, false);

                // Surface filling
                var crvTags = v.Where(keyPair => keyPair.Item1 == 1).Select(keyPair => keyPair.Item2).ToArray();
                int wireTag = IGeometryOCCKernel.IBuilder.AddWire(crvTags);
                int surfaceTag = IGeometryOCCKernel.IBuilder.AddSurfaceFilling(wireTag);
                IGeometryOCCKernel.IBuilder.Synchronize();

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryOCCKernel.EmbedConstraints(constraints, 2, surfaceTag);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                // 2d mesh generation
                IBuilder.Generate(2);

                // Iguana mesh construction
                IMesh mesh = TryCreateIMesh();
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a shell mesh from a loft.
            /// </summary>
            /// <param name="curves"> Base curves for lofting. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromCurveLoft(List<Curve> curves, ISolver2D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                // Extract required data from base surface
                logInfo = Initialize();
                StartLogger();

                List<int> crvTag = new List<int>();
                foreach (Curve c in curves)
                {
                    if (c.IsClosed)
                    {
                        int wireTag;
                        if (!c.IsPolyline()) wireTag = IGeometryOCCKernel.CreateUnderlyingCurveLoopFromCurve(c, solver.Size);
                        else
                        {
                            Polyline poly;
                            c.TryGetPolyline(out poly);
                            wireTag = IGeometryOCCKernel.CreateUnderlyingPolylineFromPolyline(poly, solver.Size);
                        }

                        crvTag.Add(wireTag);
                    }
                }

                Tuple<int, int>[] temp;
                IGeometryOCCKernel.IBuilder.AddThruSections(crvTag.ToArray(), out temp, -1, true, true);
                IGeometryOCCKernel.IBuilder.Synchronize();

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryOCCKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                // 2d mesh generation
                IBuilder.Generate(2);

                // Iguana mesh construction
                IMesh mesh = TryCreateIMesh();
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a shell mesh from a pipe.
            /// </summary>
            /// <param name="profile"> Profile curve. </param>
            /// <param name="path"> Extrusion curve. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromPipe(Curve profile, Curve path, ISolver2D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                logInfo = Initialize();
                StartLogger();

                int wireTag = IGeometryOCCKernel.CreateUnderlyingCurveLoopFromCurve(path, 1);
                int wireTag2;
                if (profile.IsCircle())
                {
                    Circle c;
                    profile.TryGetCircle(out c);
                    int crvTag = IGeometryOCCKernel.IBuilder.AddCircle(c.Center.X, c.Center.Y, c.Center.Z, c.Radius);
                    wireTag2 = IGeometryOCCKernel.IBuilder.AddWire(new int[] { crvTag });
                }
                else if (profile.IsPolyline())
                {
                    Polyline pl;
                    profile.TryGetPolyline(out pl);
                    wireTag2 = IGeometryOCCKernel.CreateUnderlyingPolylineFromPolyline(pl, 1);
                }
                else wireTag2 = IGeometryOCCKernel.CreateUnderlyingCurveLoopFromCurve(profile, 1);

                int srfTag = IGeometryOCCKernel.CreateUnderlyingSurface(wireTag2, new List<Point3d>(), false);

                Tuple<int, int>[] dimTags;
                IGeometryOCCKernel.IBuilder.AddPipe(new Tuple<int, int>[] { Tuple.Create(2, srfTag) }, wireTag, out dimTags);
                IGeometryOCCKernel.IBuilder.Synchronize();

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryOCCKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                IBuilder.Generate(2);

                IMesh mesh = TryCreateIMesh(2);
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a plane surface from a collection of polylines. 
            /// </summary>
            /// <param name="outerboundary"> Outer boundary. </param>
            /// <param name="holes"> Internal holes. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromPolylines(Curve outerboundary, List<Curve> holes, ISolver2D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                logInfo = Initialize();
                StartLogger();

                int surfaceTag = IGeometryKernel.CreateUnderlyingPlaneSurface(outerboundary, holes, solver.Size);
                IGeometryKernel.IBuilder.Synchronize();

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryKernel.EmbedConstraints(constraints, 2, surfaceTag);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                //solver options
                solver.ApplySolverSettings(field);

                IBuilder.Generate(2);

                // Iguana mesh construction
                IMesh mesh = TryCreateIMesh();
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a shell mesh from a trimmed/untrimmed surface.
            /// </summary>
            /// <param name="surface"> Base surface. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromSurface(Surface surface, ISolver2D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                logInfo = Initialize();
                StartLogger();

                ImportRhinoGeometry(surface);

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryOCCKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                // 2d mesh generation
                IBuilder.Generate(2);

                // Iguana mesh construction
                IMesh mesh = TryCreateIMesh();
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a shell mesh from a thick solid. 
            /// </summary>
            /// <param name="brep"> Base brep. </param>
            /// <param name="excludeSrfTag"> Underlying surfaces to exclude. </param>
            /// <param name="offset"> Distance offset. </param>
            /// <param name="cut"> Boolean difference. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromThickSolid(Brep brep, List<int> excludeSrfTag, double offset, bool cut, ISolver2D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                logInfo = Initialize();
                StartLogger();

                Tuple<int, int>[] v = ImportRhinoGeometry(brep, false);

                int volTag = v.First(keypair => keypair.Item1 == 3).Item2;

                Tuple<int, int>[] dimTags, outDimTags;
                IGeometryOCCKernel.IBuilder.AddThickSolid(volTag, excludeSrfTag.ToArray(), offset, out dimTags);
                if (cut) IGeometryOCCKernel.IBuilder.Cut(dimTags, new Tuple<int, int>[] { Tuple.Create(3, volTag) }, out outDimTags);
                IGeometryOCCKernel.IBuilder.Synchronize();

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryOCCKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                IBuilder.Generate(2);

                IMesh mesh = TryCreateIMesh();
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a volume mesh from a brep.
            /// </summary>
            /// <param name="brep"> Base brep. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateVolumeMeshFromBrep(Brep brep, ISolver3D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                logInfo = Initialize();
                StartLogger();

                ImportRhinoGeometry(brep);

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryOCCKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                IBuilder.Generate(3);

                IMesh mesh = TryCreateIMesh(3);
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a volume mesh from a loft.
            /// </summary>
            /// <param name="curves"> Base curves for lofting. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateVolumeMeshFromCurveLoft(List<Curve> curves, ISolver3D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                // Extract required data from base surface
                logInfo = Initialize();
                StartLogger();

                List<int> crvTag = new List<int>();
                foreach (Curve c in curves)
                {
                    if (c.IsClosed)
                    {
                        int wireTag;
                        if (!c.IsPolyline()) wireTag = IGeometryOCCKernel.CreateUnderlyingCurveLoopFromCurve(c, solver.Size);
                        else
                        {
                            Polyline poly;
                            c.TryGetPolyline(out poly);
                            wireTag = IGeometryOCCKernel.CreateUnderlyingPolylineFromPolyline(poly, solver.Size);
                        }

                        crvTag.Add(wireTag);
                    }
                }

                Tuple<int, int>[] temp;
                IGeometryOCCKernel.IBuilder.AddThruSections(crvTag.ToArray(), out temp, -1, true, true);
                IGeometryOCCKernel.IBuilder.Synchronize();

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryOCCKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                // 2d mesh generation
                IBuilder.Generate(3);

                // Iguana mesh construction
                IMesh mesh = TryCreateIMesh(3);
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a volume mesh from a pipe.
            /// </summary>
            /// <param name="profile"> Profile curve. </param>
            /// <param name="path"> Extrusion curve. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateVolumeMeshFromPipe(Curve profile, Curve path, ISolver3D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                logInfo = Initialize();
                StartLogger();

                int wireTag = IGeometryOCCKernel.CreateUnderlyingCurveLoopFromCurve(path, 1);
                int wireTag2;
                if (profile.IsCircle())
                {
                    Circle c;
                    profile.TryGetCircle(out c);
                    int crvTag = IGeometryOCCKernel.IBuilder.AddCircle(c.Center.X, c.Center.Y, c.Center.Z, c.Radius);
                    wireTag2 = IGeometryOCCKernel.IBuilder.AddWire(new int[] { crvTag });
                }
                else if (profile.IsPolyline())
                {
                    Polyline pl;
                    profile.TryGetPolyline(out pl);
                    wireTag2 = IGeometryOCCKernel.CreateUnderlyingPolylineFromPolyline(pl, 1);
                }
                else wireTag2 = IGeometryOCCKernel.CreateUnderlyingCurveLoopFromCurve(profile, 1);

                int srfTag = IGeometryOCCKernel.CreateUnderlyingSurface(wireTag2, new List<Point3d>(), false);

                Tuple<int, int>[] dimTags;
                IGeometryOCCKernel.IBuilder.AddPipe(new Tuple<int, int>[] { Tuple.Create(2, srfTag) }, wireTag, out dimTags);
                IGeometryOCCKernel.IBuilder.Synchronize();

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryOCCKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                IBuilder.Generate(3);

                IMesh mesh = TryCreateIMesh(3);
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a volume mesh from surface extrusion.
            /// </summary>
            /// <param name="surface"> Base surface. </param>
            /// <param name="direction"> Extrusion direction. </param>
            /// <param name="divisions"> Subdivisions along the extrusion. </param>
            /// <param name="lengths"> Lengths of the subdivision. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateVolumeMeshFromSurfaceExtrusion(Brep surface, Vector3d direction, List<int> divisions, List<double> lengths, ISolver3D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                if (surface.IsSolid || surface.Faces.Count > 1)
                {
                    logInfo = "";
                    entities = new GH_Structure<IEntityInfo>();
                    return new IMesh();
                }

                Curve[] crv;
                List<Point3d> patch;
                IRhinoGeometry.GetBrepFaceMeshingData(surface, 0, solver.MinimumCurvePoints, out crv, out patch);

                logInfo = Initialize();
                StartLogger();

                // Suface construction
                int wireTag = IGeometryOCCKernel.CreateUnderlyingCurveLoopFromCurve(crv[0], solver.Size);
                int surfaceTag = IGeometryOCCKernel.CreateUnderlyingSurface(wireTag, patch, false);
                Tuple<int, int>[] objectDimTag = new Tuple<int, int>[] { Tuple.Create(2, surfaceTag) };

                //Tool
                if (crv.Length > 1)
                {
                    Point3d centroid = new Point3d();
                    Interval dom = crv[0].Domain;
                    int count = 10;
                    double t = Math.Abs(dom.Length) / count;
                    for (int i = 0; i < count; i++)
                    {
                        centroid += crv[0].PointAt(dom.T0 + i * t);
                    }
                    centroid /= count;

                    double u, v;
                    surface.Surfaces[0].ClosestPoint(centroid, out u, out v);
                    Vector3d n = surface.Surfaces[0].NormalAt(u, v);

                    List<Tuple<int, int>> toolDimTags = new List<Tuple<int, int>>();
                    Tuple<int, int>[] temp;
                    for (int i = 1; i < crv.Length; i++)
                    {
                        Curve cA = crv[i].DuplicateCurve();
                        cA.Translate(n);
                        Curve cB = crv[i].DuplicateCurve();
                        cB.Translate(-n);
                        int wireTag2 = IGeometryOCCKernel.CreateUnderlyingCurveLoopFromCurve(cA, 10);
                        int wireTag3 = IGeometryOCCKernel.CreateUnderlyingCurveLoopFromCurve(cB, 10);
                        IGeometryOCCKernel.IBuilder.AddThruSections(new[] { wireTag2, wireTag3 }, out temp, -1, true, true);
                        toolDimTags.AddRange(temp);
                    }

                    Tuple<int, int>[] dimTag;
                    IGeometryOCCKernel.IBuilder.Cut(objectDimTag, toolDimTags.ToArray(), out dimTag, -1, true, true);
                }

                IGeometryOCCKernel.IBuilder.Synchronize();

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryOCCKernel.EmbedConstraints(constraints, 3, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Extrude
                Tuple<int, int>[] ov;
                var geom = new Tuple<int, int>[] { Tuple.Create(2, surfaceTag) }; ;
                IGeometryOCCKernel.IBuilder.Extrude(geom, direction.X, direction.Y, direction.Z, out ov, divisions.ToArray(), lengths.ToArray(), true);

                IGeometryOCCKernel.IBuilder.Synchronize();

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                // 2d mesh generation
                IBuilder.Generate(3);

                // Iguana mesh construction
                IMesh mesh = TryCreateIMesh(3);
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Create a volume mesh from a thick solid. 
            /// </summary>
            /// <param name="brep"> Base brep. </param>
            /// <param name="excludeSrfTag"> Underlying surfaces to exclude. </param>
            /// <param name="offset"> Distance offset. </param>
            /// <param name="cut"> Boolean difference. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateVolumeMeshFromThickSolid(Brep brep, List<int> excludeSrfTag, double offset, bool cut, ISolver3D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                logInfo = Initialize();
                StartLogger();

                Tuple<int, int>[] v = ImportRhinoGeometry(brep, false);

                int volTag = v.First(keypair => keypair.Item1 == 3).Item2;

                Tuple<int, int>[] dimTags, outDimTags;
                IGeometryOCCKernel.IBuilder.AddThickSolid(volTag, excludeSrfTag.ToArray(), offset, out dimTags);
                if (cut) IGeometryOCCKernel.IBuilder.Cut(dimTags, new Tuple<int, int>[] { Tuple.Create(3, volTag) }, out outDimTags);
                IGeometryOCCKernel.IBuilder.Synchronize();

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryOCCKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                IBuilder.Generate(3);

                IMesh mesh = TryCreateIMesh(3);
                entities = GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Remesh an Iguana shell mesh.
            /// </summary>
            /// <param name="imesh"> Base IMesh. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh RemeshShellMesh(IMesh imesh, ISolver2D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                Mesh rM = IRhinoGeometry.TryGetRhinoMesh(imesh, true);

                Initialize();
                StartLogger();

                Tuple<int, int>[] dimTags = ImportRhinoMesh(rM, RhinoDoc.ActiveDoc, false);

                var srfTag = dimTags.Where(keyPair => keyPair.Item1 == 2).Select(keyPair => keyPair.Item2).ToArray();
                IGeometryKernel.IBuilder.AddSurfaceLoop(srfTag);

                IGeometryKernel.IBuilder.Synchronize();

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                IBuilder.Generate(2);

                IMesh mesh = TryCreateIMesh(2);
                entities = GetUnderlyingEntitiesInformation();

                logInfo = GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Remesh an Iguana volume mesh.
            /// </summary>
            /// <param name="imesh"> Base IMesh. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh RemeshVolumeMesh(IMesh imesh, ISolver3D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                Mesh rM = IRhinoGeometry.TryGetRhinoMesh(imesh, true);

                Initialize();
                StartLogger();

                Tuple<int, int>[] dimTags = ImportRhinoMesh(rM, RhinoDoc.ActiveDoc, false);

                var srfTag = dimTags.Where(keyPair => keyPair.Item1 == 2).Select(keyPair => keyPair.Item2).ToArray();
                int shelltag = IGeometryKernel.IBuilder.AddSurfaceLoop(srfTag);
                int volTag = IGeometryKernel.IBuilder.AddVolume(new []{ shelltag });

                IGeometryKernel.IBuilder.Synchronize();

                // Set mesh size
                SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryKernel.EmbedConstraints(constraints, 2, -1);

                //Transfinite
                ApplyTransfiniteSettings(transfinites);

                // Preprocessing settings
                solver.ApplySolverSettings(field);

                IBuilder.Generate(3);

                IMesh mesh = TryCreateIMesh(3);
                entities = GetUnderlyingEntitiesInformation();

                logInfo = GetLogger();
                StopLogger();

                End();

                return mesh;
            }

            /// <summary>
            /// Parse IElements into element data for the meshing kernel.  
            /// </summary>
            /// <param name="mesh"> Base Iguana mesh. </param>
            /// <param name="elementTypes"> Types of elements. </param>
            /// <param name="elementTags"> Tags of elements per type.. </param>
            /// <param name="elementNodes"> Tags of nodes per element per type. </param>
            internal static void ParseIElementData(IMesh mesh, out int[] elementTypes, out long[][] elementTags, out long[][] elementNodes)
            {
                Dictionary<int, List<long>> eTags = new Dictionary<int, List<long>>();
                Dictionary<int, List<long>> eNodes = new Dictionary<int, List<long>>();
                foreach (IElement e in mesh.Elements)
                {

                    if (!eTags.ContainsKey(e.ElementType))
                    {
                        eTags.Add(e.ElementType, new List<long>());
                        eNodes.Add(e.ElementType, new List<long>());
                    }

                    eTags[e.ElementType].Add(e.Key);
                    int[] vertices = e.GetGmshFormattedVertices();
                    foreach (int vk in vertices) eNodes[e.ElementType].Add((long)vk);
                }

                elementTypes = eTags.Keys.ToArray();
                elementTags = new long[eTags.Count][];
                elementNodes = new long[eTags.Count][];
                for (int i = 0; i < elementTypes.Length; i++)
                {
                    int eType = elementTypes[i];
                    elementTags[i] = eTags[eType].ToArray();
                    elementNodes[i] = eNodes[eType].ToArray();
                }
            }

            /// <summary>
            /// Parse rhino face elements for the meshing kernel.  
            /// </summary>
            /// <param name="mesh"> Base Rhino mesh. </param>
            /// <param name="elementTypes"> Types of elements. </param>
            /// <param name="elementTags"> Tags of elements per type.. </param>
            /// <param name="elementNodes"> Tags of nodes per element per type. </param>
            internal static void ParseRhinoFaceData(Mesh mesh, out int[] elementTypes, out long[][] elementTags, out long[][] elementNodes)
            {
                Dictionary<int, List<long>> eTags = new Dictionary<int, List<long>>();
                Dictionary<int, List<long>> eNodes = new Dictionary<int, List<long>>();

                int eKey = 1;
                foreach (MeshFace e in mesh.Faces)
                {
                    int eType = 2;
                    if (e.IsQuad) eType = 4;

                    if (!eTags.ContainsKey(eType))
                    {
                        eTags.Add(eType, new List<long>());
                        eNodes.Add(eType, new List<long>());
                    }

                    eTags[eType].Add(eKey);
                    
                    if(eType==2) eNodes[eType].AddRange(new long[] { (long)e.A+1, (long)e.B+1, (long)e.C+1 });
                    else eNodes[eType].AddRange(new long[] { (long)e.A+1, (long)e.B+1, (long)e.C+1, (long) e.D+1 });

                    eKey++;
                }

                elementTypes = eTags.Keys.ToArray();
                elementTags = new long[eTags.Count][];
                elementNodes = new long[eTags.Count][];
                for (int i = 0; i < elementTypes.Length; i++)
                {
                    int eType = elementTypes[i];
                    elementTags[i] = eTags[eType].ToArray();
                    elementNodes[i] = eNodes[eType].ToArray();
                }
            }

            /// <summary>
            /// Parse ITopologicVertex into node data for the meshing kernel.  
            /// </summary>
            /// <param name="mesh"> Base Iguana mesh. </param>
            /// <param name="nodeTags"> Tags of nodes. </param>
            /// <param name="position"> Nodes position. </param>
            internal static void ParseITopologicVertexData(IMesh mesh, out long[] nodeTags, out double[] position)
            {

                int count = mesh.Vertices.Count;
                nodeTags = new long[count];
                position = new double[count * 3];
                int i = 0;
                foreach (ITopologicVertex v in mesh.Vertices)
                {
                    nodeTags[i] = v.Key;
                    position[i * 3] = v.X;
                    position[i * 3 + 1] = v.Y;
                    position[i * 3 + 2] = v.Z;
                    i++;
                }
            }

            /// <summary>
            /// Parse Rhino Vertex into node data for the meshing kernel.  
            /// </summary>
            /// <param name="mesh"> Base Iguana mesh. </param>
            /// <param name="nodeTags"> Tags of nodes. </param>
            /// <param name="position"> Nodes position. </param>
            internal static void ParseRhinoVertexData(Mesh mesh, out long[] nodeTags, out double[] position)
            {

                int count = mesh.Vertices.Count;
                nodeTags = new long[count];
                position = new double[count * 3];
                int i = 0;
                foreach (Point3d v in mesh.Vertices)
                {
                    nodeTags[i] = i+1;
                    position[i * 3] = v.X;
                    position[i * 3 + 1] = v.Y;
                    position[i * 3 + 2] = v.Z;
                    i++;
                }
            }

            /// <summary>
            /// Parse IMesh into mesh data for the meshing kernel. 
            /// </summary>
            /// <param name="mesh"> Base IMesh. </param>
            /// <param name="nodeTags"> Tags of nodes. </param>
            /// <param name="position"> Nodes position. </param>
            /// <param name="elementTypes"> Types of elements. </param>
            /// <param name="elementTags"> Tags of elements per type.. </param>
            /// <param name="elementNodes"> Tags of nodes per element per type. </param>
            internal static void ParseIMeshData(IMesh mesh, out long[] nodeTags, out double[] position, out int[] elementTypes, out long[][] elementTags, out long[][] elementNodes)
            {
                List<long> vertexKeys = new List<long>();
                List<double> vertexPos = new List<double>();
                foreach (ITopologicVertex v in mesh.Vertices)
                {
                    vertexKeys.Add(v.Key);
                    vertexPos.AddRange(new double[] { v.X, v.Y, v.Z });
                }

                // Elements (only triangular surface elements)
                List<long> eTags = new List<long>();
                List<long> eNodes = new List<long>();
                int vkey = mesh.FindNextVertexKey();
                int eKey = 1;
                foreach (IElement e in mesh.Elements)
                {
                    if (e.TopologicDimension == 2)
                    {
                        if (e.VerticesCount == 3)
                        {
                            eNodes.AddRange(new long[] { (long)e.Vertices[0], (long)e.Vertices[1], (long)e.Vertices[2] });
                            eTags.Add(eKey);
                            eKey++;
                        }
                        else if (e.VerticesCount == 4)
                        {
                            eNodes.AddRange(new long[] { (long)e.Vertices[0], (long)e.Vertices[1], (long)e.Vertices[3] });
                            eTags.Add(eKey);
                            eKey++;
                            eNodes.AddRange(new long[] { (long)e.Vertices[3], (long)e.Vertices[1], (long)e.Vertices[2] });
                            eTags.Add(eKey);
                            eKey++;
                        }
                        else
                        {
                            IPoint3D pos = ISubdividor.ComputeAveragePosition(e.Vertices, mesh);
                            vertexPos.AddRange(new double[] { pos.X, pos.Y, pos.Z });
                            vertexKeys.Add(vkey);
                            for (int i = 1; i <= e.HalfFacetsCount; i++)
                            {
                                int[] hf;
                                e.GetHalfFacet(i, out hf);
                                eNodes.AddRange(new long[] { hf[0], hf[1], vkey });
                                eTags.Add(eKey);
                                eKey++;
                            }
                            vkey++;
                        }
                    }
                }

                //To arrays
                nodeTags = vertexKeys.ToArray();
                position = vertexPos.ToArray();
                elementTypes = new int[] { 2 };
                elementTags = new long[1][] { eTags.ToArray() };
                elementNodes = new long[1][] { eNodes.ToArray() };
            }

            /// <summary>
            /// Create a volume mesh from a cone.
            /// </summary>
            /// <param name="plane"> Base plane. </param>
            /// <param name="radius1"> Radius of the lower face. </param>
            /// <param name="radius2"> Radius of the upper face. </param>
            /// <param name="angle"> Angular opening in degrees. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateVolumeMeshFromCone(Plane plane, double radius1, double radius2, double angle, ISolver3D solver)
            {
                Initialize();

                Vector3d n = plane.Normal;
                int tag = IGeometryOCCKernel.IBuilder.AddCone(plane.OriginX, plane.OriginY, plane.OriginZ, n.X, n.Y, n.Z, radius1, radius2, angle * Math.PI / 180);
                IGeometryOCCKernel.IBuilder.Synchronize();

                solver.ApplySolverSettings();

                IBuilder.Generate(3);
                IMesh mesh = TryCreateIMesh(3);

                End();

                return mesh;
            }

            /// <summary>
            /// Create a shell mesh from a cone.
            /// </summary>
            /// <param name="plane"> Base plane. </param>
            /// <param name="radius1"> Radius of the lower face. </param>
            /// <param name="radius2"> Radius of the upper face. </param>
            /// <param name="angle"> Angular opening in degrees. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromCone(Plane plane, double radius1, double radius2, double angle, ISolver2D solver)
            {
                Initialize();

                Vector3d n = plane.Normal;
                int tag = IGeometryOCCKernel.IBuilder.AddCone(plane.OriginX, plane.OriginY, plane.OriginZ, n.X, n.Y, n.Z, radius1, radius2, angle * Math.PI / 180);
                IGeometryOCCKernel.IBuilder.Synchronize();

                solver.ApplySolverSettings();

                IBuilder.Generate(2);
                IMesh mesh = TryCreateIMesh(2);

                End();

                return mesh;
            }

            /// <summary>
            /// Create a shell mesh from a box.
            /// </summary>
            /// <param name="box"> Base box. </param>
            /// <param name="u"> Number of faces along the {x} direction. </param>
            /// <param name="v"> Number of faces along the {y} direction. </param>
            /// <param name="w"> Number of faces along the {z} direction. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromBox(Box box, int u, int v, int w, ISolver2D solver)
            {
                Initialize();

                Point3d p1 = box.PointAt(0, 0, 0);
                Point3d p2 = box.PointAt(1, 0, 0);
                Point3d p3 = box.PointAt(1, 1, 0);
                Point3d p4 = box.PointAt(0, 1, 0);

                IGeometryOCCKernel.IBuilder.AddPoint(p1.X, p1.Y, p1.Z, 1);
                IGeometryOCCKernel.IBuilder.AddPoint(p2.X, p2.Y, p2.Z, 2);
                IGeometryOCCKernel.IBuilder.AddPoint(p3.X, p3.Y, p3.Z, 3);
                IGeometryOCCKernel.IBuilder.AddPoint(p4.X, p4.Y, p4.Z, 4);
                int l1 = IGeometryOCCKernel.IBuilder.AddLine(1, 2, 1);
                int l2 = IGeometryOCCKernel.IBuilder.AddLine(2, 3, 2);
                int l3 = IGeometryOCCKernel.IBuilder.AddLine(3, 4, 3);
                int l4 = IGeometryOCCKernel.IBuilder.AddLine(4, 1, 4);
                int wireTag = IGeometryOCCKernel.IBuilder.AddCurveLoop(new[] { l1, l2, l3, l4 });
                int surfaceTag = IGeometryOCCKernel.IBuilder.AddPlaneSurface(new[] { wireTag });
                IGeometryOCCKernel.IBuilder.Synchronize();

                IBuilder.SetTransfiniteCurve(l1, u);
                IBuilder.SetTransfiniteCurve(l2, v);
                IBuilder.SetTransfiniteCurve(l3, u);
                IBuilder.SetTransfiniteCurve(l4, v);

                Point3d p5 = box.PointAt(0, 0, 1);
                Tuple<int, int>[] ov;
                Tuple<int, int>[] temp = new Tuple<int, int>[] { Tuple.Create(2, surfaceTag) };
                IGeometryOCCKernel.IBuilder.Extrude(temp, 0, 0, p5.Z * 2, out ov, new[] { w }, new[] { p5.Z }, true);

                IGeometryOCCKernel.IBuilder.Synchronize();

                IBuilder.Generate(2);
                IMesh mesh = TryCreateIMesh(2);

                End();

                return mesh;
            }

            /// <summary>
            /// Create a volume mesh from a box.
            /// </summary>
            /// <param name="box"> Base box. </param>
            /// <param name="u"> Number of faces along the {x} direction. </param>
            /// <param name="v"> Number of faces along the {y} direction. </param>
            /// <param name="w"> Number of faces along the {z} direction. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateVolumeMeshFromBox(Box box, int u, int v, int w, ISolver3D solver)
            {
                Initialize();

                Point3d p1 = box.PointAt(0, 0, 0);
                Point3d p2 = box.PointAt(1, 0, 0);
                Point3d p3 = box.PointAt(1, 1, 0);
                Point3d p4 = box.PointAt(0, 1, 0);

                IGeometryOCCKernel.IBuilder.AddPoint(p1.X, p1.Y, p1.Z, 1);
                IGeometryOCCKernel.IBuilder.AddPoint(p2.X, p2.Y, p2.Z, 2);
                IGeometryOCCKernel.IBuilder.AddPoint(p3.X, p3.Y, p3.Z, 3);
                IGeometryOCCKernel.IBuilder.AddPoint(p4.X, p4.Y, p4.Z, 4);
                int l1 = IGeometryOCCKernel.IBuilder.AddLine(1, 2, 1);
                int l2 = IGeometryOCCKernel.IBuilder.AddLine(2, 3, 2);
                int l3 = IGeometryOCCKernel.IBuilder.AddLine(3, 4, 3);
                int l4 = IGeometryOCCKernel.IBuilder.AddLine(4, 1, 4);
                int wireTag = IGeometryOCCKernel.IBuilder.AddCurveLoop(new[] { l1, l2, l3, l4 });
                int surfaceTag = IGeometryOCCKernel.IBuilder.AddPlaneSurface(new[] { wireTag });
                IGeometryOCCKernel.IBuilder.Synchronize();

                IBuilder.SetTransfiniteCurve(l1, u);
                IBuilder.SetTransfiniteCurve(l2, v);
                IBuilder.SetTransfiniteCurve(l3, u);
                IBuilder.SetTransfiniteCurve(l4, v);

                Point3d p5 = box.PointAt(0, 0, 1);
                Tuple<int, int>[] ov;
                Tuple<int, int>[] temp = new Tuple<int, int>[] { Tuple.Create(2, surfaceTag) };
                IGeometryOCCKernel.IBuilder.Extrude(temp, 0, 0, p5.Z * 2, out ov, new[] { w }, new[] { p5.Z }, true);

                IGeometryOCCKernel.IBuilder.Synchronize();

                solver.ApplySolverSettings();

                IBuilder.Generate(3);
                IMesh mesh = TryCreateIMesh(3);

                End();

                return mesh;
            }

            /// <summary>
            /// Create a shell mesh from a sphere.
            /// </summary>
            /// <param name="box"> Base sphere. </param>
            /// <param name="radius"> Sphere radius. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromSphere(Point3d center, double radius, ISolver2D solver)
            {
                Initialize();

                int sTag = IGeometryOCCKernel.IBuilder.AddSphere(center.X, center.Y, center.Z, radius);
                IBuilder.SetRecombine(2, sTag);

                IGeometryOCCKernel.IBuilder.Synchronize();

                solver.ApplySolverSettings();

                IBuilder.Generate(2);
                IMesh mesh = TryCreateIMesh(2);

                End();

                return mesh;
            }

            /// <summary>
            /// Create a volume mesh from a sphere.
            /// </summary>
            /// <param name="box"> Base sphere. </param>
            /// <param name="radius"> Sphere radius. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateVolumeMeshFromSphere(Point3d center, double radius, ISolver3D solver)
            {
                Initialize();

                int sTag = IGeometryOCCKernel.IBuilder.AddSphere(center.X, center.Y, center.Z, radius);
                IBuilder.SetRecombine(2, sTag);

                IGeometryOCCKernel.IBuilder.Synchronize();

                solver.ApplySolverSettings();

                IBuilder.Generate(3);
                IMesh mesh = TryCreateIMesh(3);

                End();

                return mesh;
            }

            /// <summary>
            /// Create a shell mesh from a cylinder.
            /// </summary>
            /// <param name="plane"> Base plane. </param>
            /// <param name="radius"> Radius. </param>
            /// <param name="angle"> Angular opening in degrees. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromCylinder(Plane plane, double radius, double angle, ISolver2D solver)
            {
                Initialize();

                Vector3d n = plane.Normal;

                int tag = IGeometryOCCKernel.IBuilder.AddCylinder(plane.OriginX, plane.OriginY, plane.OriginZ, n.X, n.Y, n.Z, radius, angle * Math.PI / 180);
                IGeometryOCCKernel.IBuilder.Synchronize();

                solver.ApplySolverSettings();

                IBuilder.Generate(2);
                IMesh mesh = TryCreateIMesh(2);

                End();

                return mesh;
            }

            /// <summary>
            /// Create a volume mesh from a cylinder.
            /// </summary>
            /// <param name="plane"> Base plane. </param>
            /// <param name="radius"> Radius. </param>
            /// <param name="angle"> Angular opening in degrees. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateVolumeMeshFromCylinder(Plane plane, double radius, double angle, ISolver3D solver)
            {
                Initialize();

                Vector3d n = plane.Normal;

                int tag = IGeometryOCCKernel.IBuilder.AddCylinder(plane.OriginX, plane.OriginY, plane.OriginZ, n.X, n.Y, n.Z, radius, angle * Math.PI / 180);
                IGeometryOCCKernel.IBuilder.Synchronize();

                solver.ApplySolverSettings();

                IBuilder.Generate(3);
                IMesh mesh = TryCreateIMesh(3);

                End();

                return mesh;
            }

            /// <summary>
            /// Create a shell mesh from a cylinder.
            /// </summary>
            /// <param name="center"> Base center point. </param>
            /// <param name="radius1"> Radius for the semi major axis. </param>
            /// <param name="radius2"> Radius for the semi minor axis. </param>
            /// <param name="angle"> Angular opening in degrees. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromTorus(Point3d center, double radius1, double radius2, double angle, ISolver2D solver)
            {
                Initialize();

                int tag = IGeometryOCCKernel.IBuilder.AddTorus(center.X, center.Y, center.Z, radius1, radius2, angle * Math.PI / 180);
                IGeometryOCCKernel.IBuilder.Synchronize();

                solver.ApplySolverSettings();

                IBuilder.Generate(2);
                IMesh mesh = TryCreateIMesh(2);

                IKernel.End();

                return mesh;
            }

            /// <summary>
            /// Create a volume mesh from a cylinder.
            /// </summary>
            /// <param name="center"> Base center point. </param>
            /// <param name="radius1"> Radius for the semi major axis. </param>
            /// <param name="radius2"> Radius for the semi minor axis. </param>
            /// <param name="angle"> Angular opening in degrees. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateVolumeMeshFromTorus(Point3d center, double radius1, double radius2, double angle, ISolver3D solver)
            {
                Initialize();

                int tag = IGeometryOCCKernel.IBuilder.AddTorus(center.X, center.Y, center.Z, radius1, radius2, angle * Math.PI / 180);
                IGeometryOCCKernel.IBuilder.Synchronize();

                solver.ApplySolverSettings();

                IBuilder.Generate(3);
                IMesh mesh = TryCreateIMesh(3);

                IKernel.End();

                return mesh;
            }

            #endregion

            #region Gmsh Methods 

            internal static class IBuilder
            {
                /// <summary>
                /// Generate a mesh of the current model, up to dimension `dim' (0, 1, 2 or 3).
                /// </summary>
                /// <param name="dim"></param>
                public static void Generate(int dim)
                {
                    IWrap.GmshModelMeshGenerate(dim, ref _ierr);
                }

                /// <summary>
                /// Get the nodes classified on the entity of dimension `dim'.  
                /// </summary>
                /// <param name="nodeTags_out"> `nodeTags_out' contains the node tags (their unique, strictly positive identification numbers).  </param>
                /// <param name="coord_out"> `coord' is a two-dimensional array that contains the x, y, z coordinates of the nodes. </param>
                /// <param name="parametricCoord_out"> If `dim' >= 0, `parametricCoord' contains the parametric coordinates([u1, u2, ...] or [u1, v1, u2, ...]) of the nodes, if available. </param>
                /// <param name="dim"/> If `dim' is negative (default), get all the nodes in the mesh. </param>
                public static void GetNodes(out long[] nodeTags_out, out double[][] coord_out, out double[][] parametricCoord_out, int dim = -1, int tag = -1)
                {
                    IntPtr nodeTags, coord, parametricCoord;
                    long nodeTags_Number, coord_Number, parametricCoord_Number;
                    IWrap.GmshModelMeshGetNodes(out nodeTags, out nodeTags_Number, out coord, out coord_Number, out parametricCoord, out parametricCoord_Number, dim, tag, Convert.ToInt32(true), Convert.ToInt32(true), ref _ierr);

                    nodeTags_out = new long[nodeTags_Number];
                    coord_out = new double[nodeTags_Number][];
                    parametricCoord_out = new double[0][];

                    // Tags
                    if (nodeTags_Number > 0)
                    {
                        // Coordinates
                        var xyz = new double[coord_Number];
                        Marshal.Copy(coord, xyz, 0, (int)coord_Number);
                        // Keys
                        var keys = new long[nodeTags_Number];
                        Marshal.Copy(nodeTags, keys, 0, (int)nodeTags_Number);

                        for (int i = 0; i < nodeTags_Number; i++)
                        {
                            coord_out[i] = new double[] { xyz[i * 3], xyz[i * 3 + 1], xyz[i * 3 + 2] };
                            nodeTags_out[i] = (int)keys[i];
                        }

                        int paramCount = (int)(parametricCoord_Number / dim);
                        var uvw = new double[parametricCoord_Number];
                        if (paramCount > 0)
                        {
                            parametricCoord_out = new double[paramCount][];
                            Marshal.Copy(parametricCoord, uvw, 0, (int)parametricCoord_Number);
                            for (int i = 0; i < paramCount; i++)
                            {
                                if (dim == 1) parametricCoord_out[i] = new double[] { uvw[i * dim] };
                                else if (dim == 2) parametricCoord_out[i] = new double[] { uvw[i * dim], uvw[i * dim + 1] };
                                else if (dim == 3) parametricCoord_out[i] = new double[] { uvw[i * dim], uvw[i * dim + 1], uvw[i * dim + 2] };
                            }
                        }
                    }

                    // Delete unmanaged allocated memory
                    IWrap.GmshFree(nodeTags);
                    IWrap.GmshFree(coord);
                    IWrap.GmshFree(parametricCoord);
                }

                public static void GetCenter(int dim, int tag, out double[] center)
                {
                    IntPtr nodeTags, coord, parametricCoord;
                    long nodeTags_Number, coord_Number, parametricCoord_Number;
                    IWrap.GmshModelMeshGetNodes(out nodeTags, out nodeTags_Number, out coord, out coord_Number, out parametricCoord, out parametricCoord_Number, dim, tag, Convert.ToInt32(true), Convert.ToInt32(true), ref _ierr);

                    center = new double[3];
                    // Tags
                    if (nodeTags_Number > 0)
                    {
                        // Coordinates
                        var xyz = new double[coord_Number];
                        Marshal.Copy(coord, xyz, 0, (int)coord_Number);

                        for (int i = 0; i < nodeTags_Number; i++)
                        {
                            center[0] += xyz[i * 3];
                            center[1] += xyz[i * 3 + 1];
                            center[2] += xyz[i * 3 + 2];
                        }

                        center[0] /= nodeTags_Number;
                        center[1] /= nodeTags_Number;
                        center[2] /= nodeTags_Number;
                    }

                    // Delete unmanaged allocated memory
                    IWrap.GmshFree(nodeTags);
                    IWrap.GmshFree(coord);
                    IWrap.GmshFree(parametricCoord);
                }

                /// <summary>
                /// Get the elements classified on the entity of dimension `dim'.
                /// `elementTypes' contains the MSH types of the elements (e.g. `2' for 3-node triangles: see `getElementProperties' to obtain the properties for a given element type). 
                /// `elementTags' is a vector of the same length as `elementTypes'; each entry is a vector containing the tags (unique, strictly positive identifiers) of the elements of the corresponding type.
                /// `nodeTags' is also a vector of the same length as `elementTypes'; each entry is a vector of length equal to the number of elements of the given type times the number N of nodes for this type of element, 
                /// that contains the node tags of all the elements of the given type, concatenated: [e1n1, e1n2, ..., e1nN, e2n1, ...]. 
                /// </summary>
                /// <param name="elementTypes"></param>
                /// <param name="elementTypes_n"></param>
                /// <param name="elementTags"></param>
                /// <param name="elementTags_n"></param>
                /// <param name="elementTags_nn"></param>
                /// <param name="nodeTags"></param>
                /// <param name="nodeTags_n"></param>
                /// <param name="nodeTags_nn"></param>
                /// <param name="dim"> If `dim' is negative (default), get all the elements in the mesh.  </param>
                public static void GetElements(out int[] elementTypes_out, out long[][] elementTags_out, out long[][] nodeTags_out, int dim = -1, int tag = -1)
                {
                    IntPtr elementTypes, elementTags, nodeTags, elementTags_n, nodeTags_n;
                    long elementTypes_Number, elementTags_NNumber, nodeTags_NNumber;

                    IWrap.GmshModelMeshGetElements(out elementTypes, out elementTypes_Number, out elementTags, out elementTags_n, out elementTags_NNumber, out nodeTags, out nodeTags_n, out nodeTags_NNumber, dim, tag, ref _ierr);

                    elementTypes_out = new int[elementTypes_Number];
                    var eTags_n = new long[elementTags_NNumber];
                    var nTags_n = new long[nodeTags_NNumber];

                    Marshal.Copy(elementTypes, elementTypes_out, 0, (int)elementTypes_Number);
                    Marshal.Copy(elementTags_n, eTags_n, 0, (int)elementTags_NNumber);
                    Marshal.Copy(nodeTags_n, nTags_n, 0, (int)nodeTags_NNumber);

                    var nTags_ptr = new IntPtr[nodeTags_NNumber];
                    var eTags_ptr = new IntPtr[elementTags_NNumber];

                    Marshal.Copy(nodeTags, nTags_ptr, 0, (int)nodeTags_NNumber);
                    Marshal.Copy(elementTags, eTags_ptr, 0, (int)elementTags_NNumber);

                    elementTags_out = new long[elementTags_NNumber][];
                    nodeTags_out = new long[nodeTags_NNumber][];

                    for (int i = 0; i < elementTags_NNumber; i++)
                    {
                        // Initializing containers
                        elementTags_out[i] = new long[eTags_n[i]];
                        nodeTags_out[i] = new long[nTags_n[i]];

                        // Marshalling
                        Marshal.Copy(eTags_ptr[i], elementTags_out[i], 0, (int)eTags_n[i]);
                        Marshal.Copy(nTags_ptr[i], nodeTags_out[i], 0, (int)nTags_n[i]);
                    }

                    // Delete unmanaged allocated memory
                    IWrap.GmshFree(elementTypes);
                    IWrap.GmshFree(elementTags);
                    IWrap.GmshFree(nodeTags);
                    IWrap.GmshFree(elementTags_n);
                    IWrap.GmshFree(nodeTags_n);

                    for (int i = 0; i < nTags_ptr.Length; i++)
                    {
                        IWrap.GmshFree(nTags_ptr[i]);
                    }
                    for (int i = 0; i < eTags_ptr.Length; i++)
                    {
                        IWrap.GmshFree(eTags_ptr[i]);
                    }
                }

                /// <summary>
                /// Optimize the mesh of the current model using.
                /// <param name="method"> `method' (empty for default tetrahedral mesh optimizer, "Netgen" for Netgen optimizer, "HighOrder" for
                /// direct high-order mesh optimizer, "HighOrderElastic" for high-order elastic smoother, "HighOrderFastCurving" for fast curving algorithm,
                /// "Laplace2D" for Laplace smoothing, "Relocate2D" and "Relocate3D" for node relocation)</param>
                /// <param name="niter"> Number of Iterations. Default is 5. </param>
                public static void Optimize(string method = default, int niter = 5)
                {
                    if (method == default) method = "";
                    IKernel.IWrap.GmshModelMeshOptimize(method, -1, niter, null, IntPtr.Zero, ref _ierr);
                }

                /// <summary>
                /// Remove duplicate nodes in the mesh of the current model.
                /// </summary>
                public static void RemoveDuplicateNodes()
                {
                    IKernel.IWrap.GmshModelMeshRemoveDuplicateNodes(ref _ierr);
                }

                /// <summary>
                /// Split (into two triangles) all quadrangles in surface `tag' whose quality is lower than `quality'. 
                /// </summary>
                /// <param name="quality"> Quality of the surface. </param>
                /// <param name="tag"> If `tag' < 0, split quadrangles in all surfaces. </param>
                public static void SplitQuadrangles(double quality, int tag = -1)
                {
                    IKernel.IWrap.GmshModelMeshSplitQuadrangles(quality, tag, ref _ierr);
                }

                /// <summary>
                /// Set a mesh size constraint on the model entities `dimTags'. Currently only entities of dimension 0 (points) are handled.
                /// </summary>
                /// <param name="dimTags"></param>
                /// <param name="size"></param>
                public static void SetSize(Tuple<int, int>[] dimTags, double size)
                {
                    int[] dimTags_flatten = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelMeshSetSize(dimTags_flatten, dimTags_flatten.LongLength, size, ref _ierr);
                }

                /// <summary>
                /// Set mesh size constraints at the given parametric points `parametricCoord'
                /// on the model entity of dimension `dim' and tag `tag'. Currently only
                /// entities of dimension 1 (lines) are handled.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="parametricCoord"></param>
                /// <param name="sizes"></param>
                public static void SetSizeAtParametricPoints(int dim, int tag, double[] parametricCoord, double[] sizes)
                {
                    IWrap.GmshModelMeshSetSizeAtParametricPoints(dim, tag, parametricCoord, parametricCoord.LongLength, sizes, sizes.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set a transfinite meshing constraint on the curve `tag', with `numNodes'
                /// nodes distributed according to `meshType' and `coef'. Currently supported
                /// types are "Progression" (geometrical progression with power `coef') and
                /// "Bump" (refinement toward both extremities of the curve).
                /// </summary>
                public static void SetTransfiniteCurve(int tag, int numNodes, string meshType = "Progression", double coef = 1)
                {
                    IWrap.GmshModelMeshSetTransfiniteCurve(tag, numNodes, meshType, coef, ref _ierr);
                }

                /// <summary>
                /// Set a transfinite meshing constraint on the surface `tag'. `arrangement'
                /// describes the arrangement of the triangles when the surface is not flagged
                /// as recombined: currently supported values are "Left", "Right",
                /// "AlternateLeft" and "AlternateRight". `cornerTags' can be used to specify
                /// the(3 or 4) corners of the transfinite interpolation explicitly;
                /// specifying the corners explicitly is mandatory if the surface has more that
                /// 3 or 4 points on its boundary.
                /// </summary>
                public static void SetTransfiniteSurface(int tag, string arrangement = "Left", int[] cornerTags = default)
                {
                    if (cornerTags == default) cornerTags = new int[0];
                    IWrap.GmshModelMeshSetTransfiniteSurface(tag, arrangement, cornerTags, cornerTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set a transfinite meshing constraint on the surface `tag'. `cornerTags' can be used to specify the(6 or 8) corners of the transfinite interpolation explicitly.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="cornerTags"></param>
                public static void SetTransfiniteVolume(int tag, int[] cornerTags = default)
                {
                    if (cornerTags == default) cornerTags = new int[] { };
                    IWrap.GmshModelMeshSetTransfiniteVolume(tag, cornerTags, cornerTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// (Only available with gmsh_4.7) Set transfinite meshing constraints on the model entities in `dimTag'.
                /// Transfinite meshing constraints are added to the curves of the quadrangular
                /// surfaces and to the faces of 6-sided volumes.Quadragular faces with a
                /// corner angle superior to `cornerAngle' (in radians) are ignored. The number
                /// of points is automatically determined from the sizing constraints. If
                /// `dimTag' is empty, the constraints are applied to all entities in the
                /// model.If `recombine' is true, the recombine flag is automatically set on
                /// the transfinite surfaces.
                /// </summary>
                public static void SetAutomaticTransfinite(Tuple<int, int>[] dimTags, double cornerAngle = 90, bool recombine = false)
                {
                    var data = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelMeshSetTransfiniteAutomatic(data, data.LongLength, cornerAngle * Math.PI / 180, Convert.ToInt32(recombine), ref _ierr);
                }

                /// <summary>
                /// Set a recombination meshing constraint on the model entity of dimension
                /// `dim' and tag `tag'. Currently only entities of dimension 2 (to recombine
                /// triangles into quadrangles) are supported.
                /// </summary>
                public static void SetRecombine(int dim, int tag)
                {
                    IWrap.GmshModelMeshSetRecombine(dim, tag, ref _ierr);
                }

                /// <summary>
                /// Set a smoothing meshing constraint on the model entity of dimension `dim' and tag `tag'. `val' iterations of a Laplace smoother are applied.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"></param>
                public static void SetSmoothing(int dim, int tag, int val)
                {
                    IWrap.GmshModelMeshSetSmoothing(dim, tag, val, ref _ierr);
                }

                /// <summary>
                /// Set a reverse meshing constraint on the model entity of dimension `dim' and
                /// tag `tag'. If `val' is true, the mesh orientation will be reversed with
                /// respect to the natural mesh orientation(i.e.the orientation consistent with the orientation of the geometry). If `val' is false, the mesh is left as-is.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"></param>
                public static void SetReverse(int dim, int tag, bool val)
                {
                    IWrap.GmshModelMeshSetReverse(dim, tag, Convert.ToInt32(val), ref _ierr);
                }

                /// <summary>
                /// Set the meshing algorithm on the model entity of dimension `dim' and tag
                /// `tag'. Currently only supported for `dim' == 2.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"></param>
                public static void SetAlgorithm(int dim, int tag, int val)
                {
                    IWrap.GmshModelMeshSetAlgorithm(dim, tag, val, ref _ierr);
                }

                /// <summary>
                /// Force the mesh size to be extended from the boundary, or not, for the model
                /// entity of dimension `dim' and tag `tag'. 
                /// Currently only supported for `dim' == 2.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tag"></param>
                /// <param name="val"></param>
                public static void SetSizeFromBoundary(int dim, int tag, bool val)
                {
                    IWrap.GmshModelMeshSetSizeFromBoundary(dim, tag, Convert.ToInt32(val), ref _ierr);
                }

                /// <summary>
                /// Set a compound meshing constraint on the model entities of dimension `dim'
                /// and tags `tags'. During meshing, compound entities are treated as a single
                /// discrete entity, which is automatically reparametrized.
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tags"></param>
                public static void SetCompound(int dim, int[] tags)
                {
                    IWrap.GmshModelMeshSetCompound(dim, tags, tags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set meshing constraints on the bounding surfaces of the volume of tag `tag'
                /// so that all surfaces are oriented with outward pointing normals.Currently
                /// only available with the OpenCASCADE kernel, as it relies on the STL triangulation.
                /// </summary>
                /// <param name="tag"></param>
                public static void SetOutwardOrientation(int tag)
                {
                    IWrap.GmshModelMeshSetOutwardOrientation(tag, ref _ierr);
                }

                /// <summary>
                /// Renumber the node tags in a continuous sequence.
                /// </summary>
                public static void RenumberNodes()
                {
                    IWrap.GmshModelMeshRenumberNodes(ref _ierr);
                }

                /// <summary>
                /// Renumber the element tags in a continuous sequence.
                /// </summary>
                public static void RenumberElements()
                {
                    IWrap.GmshModelMeshRenumberElements(ref _ierr);
                }

                /// <summary>
                /// Set the meshes of the entities of dimension `dim' and tag `tags' as
                /// periodic copies of the meshes of entities `tagsMaster', using the affine
                /// transformation specified in `affineTransformation' (16 entries of a 4x4
                /// matrix, by row). If used after meshing, generate the periodic node
                /// correspondence information assuming the meshes of entities `tags'
                /// effectively match the meshes of entities `tagsMaster' (useful for
                /// structured and extruded meshes). Currently only available for @code{dim} == 1 and @code { dim } == 2. 
                /// </summary>
                /// <param name="dim"></param>
                /// <param name="tags"></param>
                /// <param name="tagsMaster"></param>
                /// <param name="affineTransform"></param>
                public static void SetPeriodic(int dim, int[] tags, int[] tagsMaster, double[] affineTransform)
                {
                    IWrap.GmshModelMeshSetPeriodic(dim, tags, tags.LongLength, tagsMaster, tagsMaster.LongLength, affineTransform, affineTransform.LongLength, ref _ierr);
                }

                /// <summary>
                /// Classify ("color") the surface mesh based on the angle threshold `angle'
                /// (in radians), and create new discrete surfaces, curves and points
                /// accordingly.If `boundary' is set, also create discrete curves on the 
                /// boundary if the surface is open.If `forReparametrization' is set, create
                /// edges and surfaces that can be reparametrized using a single map.If
                /// `curveAngle' is less than Pi, also force curves to be split according to
                /// `curveAngle'.
                /// </summary>
                /// <param name="angle"></param>
                /// <param name="boundary"></param>
                /// <param name="forReparametrization"></param>
                /// <param name="curveAngle"></param>
                public static void ClassifySurfaces(double angle, bool boundary, bool forReparametrization, double curveAngle)
                {
                    IWrap.GmshModelMeshClassifySurfaces(angle, Convert.ToInt32(boundary), Convert.ToInt32(forReparametrization), curveAngle, ref _ierr);
                }

                /// <summary>
                /// Create a geometry for the discrete entities `dimTags' (represented solely
                /// by a mesh, without an underlying CAD description), i.e.create a
                /// parametrization for discrete curves and surfaces, assuming that each can be
                /// parametrized with a single map.If `dimTags' is empty, create a geometry
                /// for all the discrete entities.
                /// </summary>
                /// <param name="dimTags"></param>
                public static void CreateGeometry(long[] dimTags = default)
                {
                    if (dimTags == default) dimTags = new long[0];
                    IWrap.GmshModelMeshCreateGeometry(dimTags, dimTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Create a boundary representation from the mesh if the model does not have
                /// one(e.g.when imported from mesh file formats with no BRep representation
                /// of the underlying model). If `makeSimplyConnected' is set, enforce simply 
                /// connected discrete surfaces and volumes.If `exportDiscrete' is set, clear
                /// any built-in CAD kernel entities and export the discrete entities in the
                /// built-in CAD kernel.
                /// </summary>
                /// <param name="makeSimplyConnected"></param>
                /// <param name="exportDiscrete"></param>
                public static void CreateTopology(bool makeSimplyConnected, bool exportDiscrete)
                {
                    IWrap.GmshModelMeshCreateTopology(Convert.ToInt32(makeSimplyConnected), Convert.ToInt32(exportDiscrete), ref _ierr);
                }

                /// <summary>
                /// Compute a basis representation for homology spaces after a mesh has been
                /// generated.The computation domain is given in a list of physical group tags
                /// `domainTags'; if empty, the whole mesh is the domain. The computation
                /// subdomain for relative homology computation is given in a list of physical
                /// group tags `subdomainTags'; if empty, absolute homology is computed. The
                /// dimensions homology bases to be computed are given in the list `dim'; if
                /// empty, all bases are computed.Resulting basis representation chains are
                /// stored as physical groups in the mesh.
                /// </summary>
                /// <param name="domainTags"></param>
                /// <param name="subdomainTags"></param>
                /// <param name="dims"></param>
                public static void ComputeHomology(int[] domainTags, int[] subdomainTags, int[] dims)
                {
                    IWrap.GmshModelMeshComputeHomology(domainTags, domainTags.LongLength, subdomainTags, subdomainTags.LongLength, dims, dims.LongLength, ref _ierr);
                }

                /// <summary>
                /// Compute a basis representation for cohomology spaces after a mesh has been
                /// generated.The computation domain is given in a list of physical group tags
                /// `domainTags'; if empty, the whole mesh is the domain. The computation
                /// subdomain for relative cohomology computation is given in a list of
                /// physical group tags `subdomainTags'; if empty, absolute cohomology is
                /// computed.The dimensions homology bases to be computed are given in the
                /// list `dim'; if empty, all bases are computed. Resulting basis
                /// representation cochains are stored as physical groups in the mesh.
                /// </summary>
                /// <param name="domainTags"></param>
                /// <param name="subdomainTags"></param>
                /// <param name="dims"></param>
                public static void ComputeCohomology(int[] domainTags, int[] subdomainTags, [In, Out] int[] dims)
                {
                    IWrap.GmshModelMeshComputeCohomology(domainTags, domainTags.LongLength, subdomainTags, subdomainTags.LongLength, dims, dims.LongLength, ref _ierr);
                }

                /// <summary>
                /// Compute a cross field for the current mesh. The function creates 3 views: the H function, the Theta function and cross directions.
                /// Return the tags of the views
                /// </summary>
                /// <param name="viewTags_out"></param>
                public static void MeshComputeCrossField(out int[] viewTags_out)
                {
                    IntPtr viewTags;
                    long viewTags_n;
                    IWrap.GmshModelMeshComputeCrossField(out viewTags, out viewTags_n, ref _ierr);

                    viewTags_out = null;
                    if (viewTags_n > 0)
                    {
                        viewTags_out = new int[viewTags_n];
                        Marshal.Copy(viewTags, viewTags_out, 0, (int)viewTags_n);
                    }
                    IWrap.GmshFree(viewTags);
                }

                /// <summary>
                /// Refine the mesh of the current model by uniformly splitting the elements.
                /// </summary>
                public static void Refine()
                {
                    IWrap.GmshModelMeshRefine(ref _ierr);
                }

                /// <summary>
                /// Recombine the mesh of the current model.
                /// </summary>
                public static void Recombine()
                {
                    IWrap.GmshModelMeshRecombine(ref _ierr);
                }

                /// <summary>
                /// Embed the model entities of dimension `dim' and tags `tags' in the (`inDim', `inTag') model entity.
                /// The embedded entities should not be part of the boundary of the entity `inTag', whose mesh will conform to the mesh of the embedded entities.
                /// </summary>
                /// <param name="dim"> The dimension `dim' can 0, 1 or 2 and must be strictly smaller than `inDim', which must be either 2 or 3. </param>
                /// <param name="tags"></param>
                /// <param name="inDim"></param>
                /// <param name="inTag"></param>
                public static void Embed(int dim, int[] tags, int inDim, int inTag)
                {
                    IWrap.GmshModelMeshEmbed(dim, tags, tags.LongLength, inDim, inTag, ref _ierr);
                }

                /// <summary>
                /// Remove embedded entities from the model entities `dimTags'. if `dim' is >= 0, only remove embedded entities of the given dimension (e.g.embedded points if `dim' == 0).
                /// </summary>
                public static void RemoveEmbedded(Tuple<int, int>[] dimTags, int dim)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelMeshRemoveEmbedded(arr, arr.LongLength, dim, ref _ierr);
                }

                /// <summary>
                /// Reclassify all nodes on their associated model entity, based on the
                /// elements.Can be used when importing nodes in bulk(e.g.by associating
                /// them all to a single volume), to reclassify them correctly on model
                /// surfaces, curves, etc.after the elements have been set.
                /// </summary>
                public static void ReclassifyNodes()
                {
                    IWrap.GmshModelMeshReclassifyNodes(ref _ierr);
                }

                /// <summary>
                /// Add nodes classified on the model entity of dimension `dim' and tag `tag'.
                /// `nodeTags' contains the node tags (their unique, strictly positive
                /// identification numbers). `coord' is a vector of length 3 times the length
                /// of `nodeTags' that contains the x, y, z coordinates of the nodes,
                /// concatenated: [n1x, n1y, n1z, n2x, ...]. The optional `parametricCoord'
                /// vector contains the parametric coordinates of the nodes, if any.The length
                /// of `parametricCoord' can be 0 or `dim' times the length of `nodeTags'. If
                /// the `nodeTags' vector is empty, new tags are automatically assigned to the
                /// nodes.
                /// </summary>
                public static void AddNodes(int dim, int tag, long[] nodeTags, double[] coord, double[] parametricCoord = default)
                {
                    if (parametricCoord == default) parametricCoord = new double[0];
                    IWrap.GmshModelMeshAddNodes(dim, tag, nodeTags, nodeTags.LongLength, coord, coord.LongLength, parametricCoord, parametricCoord.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add nodes classified on the model entity of dimension `dim' and tag `tag'.
                /// `nodeTags' contains the node tags (their unique, strictly positive
                /// identification numbers). `coord' is a vector of length 3 times the length
                /// of `nodeTags' that contains the x, y, z coordinates of the nodes,
                /// concatenated: [n1x, n1y, n1z, n2x, ...]. The optional `parametricCoord'
                /// vector contains the parametric coordinates of the nodes, if any.The length
                /// of `parametricCoord' can be 0 or `dim' times the length of `nodeTags'. If
                /// the `nodeTags' vector is empty, new tags are automatically assigned to the
                /// nodes.
                /// </summary>
                public static void AddNodes(int tag, MeshVertexList vertices, MeshTextureCoordinateList textures)
                {
                    long[] nodeTags;
                    double[] coord;
                    IHelpers.ParseRhinoVertices(vertices, out coord, out nodeTags);
                    double[] parametricCoord;
                    IHelpers.ParseRhinoTextures(textures, out parametricCoord);

                    AddNodes(2, tag, nodeTags, coord, parametricCoord);
                }

                /// <summary>
                /// Add elements classified on the entity of dimension `dim' and tag `tag'.
                /// `types' contains the MSH types of the elements (e.g. `2' for 3-node
                /// triangles: see the Gmsh reference manual). `elementTags' is a vector of the
                /// same length as `types'; each entry is a vector containing the tags (unique,
                /// strictly positive identifiers) of the elements of the corresponding type.
                /// `nodeTags' is also a vector of the same length as `types'; each entry is a
                /// vector of length equal to the number of elements of the given type times
                /// the number N of nodes per element, that contains the node tags of all the
                /// elements of the given type, concatenated: [e1n1, e1n2, ..., e1nN, e2n1,...].
                /// </summary>
                public static void AddElements(int dim, int tag, int[] elementTypes, long[][] elementTags, long[][] nodeTags)
                {
                    long[] elementTags_flatten = IHelpers.FlattenLongArray(elementTags);
                    long[] nodesTags_flatten = IHelpers.FlattenLongArray(nodeTags);

                    IWrap.GmshModelMeshAddElements(dim, tag, elementTypes, elementTypes.LongLength, elementTags_flatten, elementTypes.LongLength, elementTags_flatten.LongLength, nodesTags_flatten, elementTypes.LongLength, nodesTags_flatten.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add elements classified on the entity of dimension `dim' and tag `tag'.
                /// `types' contains the MSH types of the elements (e.g. `2' for 3-node
                /// triangles: see the Gmsh reference manual). `elementTags' is a vector of the
                /// same length as `types'; each entry is a vector containing the tags (unique,
                /// strictly positive identifiers) of the elements of the corresponding type.
                /// `nodeTags' is also a vector of the same length as `types'; each entry is a
                /// vector of length equal to the number of elements of the given type times
                /// the number N of nodes per element, that contains the node tags of all the
                /// elements of the given type, concatenated: [e1n1, e1n2, ..., e1nN, e2n1,...].
                /// </summary>
                public static void AddElements(int tag, MeshFaceList faces)
                {
                    MeshFace f;
                    List<long> tagsT = new List<long>();
                    List<long> nodesT = new List<long>();
                    List<long> tagsQ = new List<long>();
                    List<long> nodesQ = new List<long>();

                    for (int i = 0; i < faces.Count; i++)
                    {
                        f = faces[i];
                        if (f.IsTriangle)
                        {
                            tagsT.Add(i);
                            nodesT.AddRange(new long[] { f.A, f.B, f.C });
                        }
                        else if (f.IsQuad)
                        {
                            tagsQ.Add(i);
                            nodesQ.AddRange(new long[] { f.A, f.B, f.C, f.D });
                        }
                    }

                    int[] elementTypes = new int[0];
                    if (tagsT.Count > 0 && tagsQ.Count > 0) elementTypes = new int[] { 2, 3 };
                    else if (tagsT.Count > 0 && tagsQ.Count == 0) elementTypes = new int[] { 2 };
                    else if (tagsT.Count == 0 && tagsQ.Count > 0) elementTypes = new int[] { 3 };

                    long[] elementTags_flatten = tagsT.Concat(tagsQ).ToArray();
                    long[] nodesTags_flatten = nodesT.Concat(nodesQ).ToArray();

                    IWrap.GmshModelMeshAddElements(2, tag, elementTypes, elementTypes.LongLength, elementTags_flatten, elementTypes.LongLength, elementTags_flatten.LongLength, nodesTags_flatten, elementTypes.LongLength, nodesTags_flatten.LongLength, ref _ierr);
                }

                /// <summary>
                /// Add elements of type `elementType' classified on the entity of tag `tag'.
                /// `elementTags' contains the tags (unique, strictly positive identifiers) of
                /// the elements of the corresponding type. `nodeTags' is a vector of length
                /// equal to the number of elements times the number N of nodes per element,
                /// that contains the node tags of all the elements, concatenated: [e1n1, e1n2,
                /// ..., e1nN, e2n1, ...]. If the `elementTag' vector is empty, new tags are
                /// automatically assigned to the elements.
                /// </summary>
                public static void AddElementsByType(int tag, int elementType, long[] elementTags, long[] nodeTags)
                {
                    IWrap.GmshModelMeshAddElementsByType(tag, elementType, elementTags, elementTags.LongLength, nodeTags, nodeTags.LongLength, ref _ierr);
                }

                /// <summary>
                /// Partition the mesh of the current model into `numPart' partitions.
                /// </summary>
                public static void Partition(int numPart)
                {
                    IWrap.GmshModelMeshPartition(numPart, ref _ierr);
                }

                /// <summary>
                /// Unpartition the mesh of the current model.
                /// </summary>
                public static void Unpartition()
                {
                    IWrap.GmshModelMeshUnpartition(ref _ierr);
                }

                /// <summary>
                /// Set the order of the elements in the mesh of the current model to `order'.
                /// </summary>
                public static void SetOrder(int order)
                {
                    IWrap.GmshModelMeshSetOrder(order, ref _ierr);
                }

                /// <summary>
                /// Clear the mesh, i.e. delete all the nodes and elements, for the entities
                /// `dimTags'. if `dimTags' is empty, clear the whole mesh.Note that the mesh
                /// of an entity can only be cleared if this entity is not on the boundary of
                /// another entity with a non-empty mesh.
                /// </summary>
                public static void Clear(Tuple<int, int>[] dimTags)
                {
                    var arr = IHelpers.FlattenIntTupleArray(dimTags);
                    IWrap.GmshModelMeshClear(arr, arr.LongLength, ref _ierr);
                }

                /// <summary>
                /// Get the nodes classified on the entity of tag `tag', for all the elements
                /// of type `elementType'. The other arguments are treated as in `getNodes'.
                /// </summary>
                public static void GetNodesByElementType(int elementType, out long[] nodeTags, out double[] coord, out double[] parametricCoord, int tag, bool returnParametricCoord)
                {
                    IntPtr ntP, cP, pcP;
                    long nodeTags_n, coord_n, parametricCoord_n;
                    IWrap.GmshModelMeshGetNodesByElementType(elementType, out ntP, out nodeTags_n, out cP, out coord_n, out pcP, out parametricCoord_n, tag, Convert.ToInt32(returnParametricCoord), ref _ierr);

                    nodeTags = new long[nodeTags_n];
                    coord = new double[coord_n];
                    parametricCoord = new double[parametricCoord_n];
                    Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);
                    Marshal.Copy(cP, coord, 0, (int)coord_n);
                    Marshal.Copy(pcP, parametricCoord, 0, (int)parametricCoord_n);

                    Free(ntP);
                    Free(cP);
                    Free(pcP);
                }

                /// <summary>
                /// Get the coordinates and the parametric coordinates (if any) of the node
                /// with tag `tag'. This function relies on an internal cache (a vector in case
                /// of dense node numbering, a map otherwise); for large meshes accessing nodes
                /// in bulk is often preferable.
                /// </summary>
                public static void GetNode(long nodeTag, out double[] coord, out double[] parametricCoord)
                {
                    IntPtr cP, pcP;
                    long coord_n, parametricCoord_n;

                    IWrap.GmshModelMeshGetNode(nodeTag, out cP, out coord_n, out pcP, out parametricCoord_n, ref _ierr);

                    coord = new double[coord_n];
                    parametricCoord = new double[parametricCoord_n];
                    Marshal.Copy(cP, coord, 0, (int)coord_n);
                    Marshal.Copy(pcP, parametricCoord, 0, (int)parametricCoord_n);

                    Free(cP);
                    Free(pcP);
                }

                /// <summary>
                /// Set the coordinates and the parametric coordinates (if any) of the node
                /// with tag `tag'. This function relies on an internal cache (a vector in case
                /// of dense node numbering, a map otherwise); for large meshes accessing nodes
                /// in bulk is often preferable.
                /// </summary>
                public static void SetNode(long nodeTag, double[] coord, double[] parametricCoord)
                {
                    IWrap.GmshModelMeshSetNode(nodeTag, coord, coord.LongLength, parametricCoord, parametricCoord.LongLength, ref _ierr);
                }

                /// <summary>
                /// Rebuild the node cache.
                /// </summary>
                public static void RebuildNodeCache(bool onlyIfNecessary)
                {
                    IWrap.GmshModelMeshRebuildNodeCache(Convert.ToInt32(onlyIfNecessary), ref _ierr);
                }

                /// <summary>
                /// Rebuild the element cache.
                /// </summary>
                public static void RebuildElementCache(bool onlyIfNecessary)
                {
                    IWrap.GmshModelMeshRebuildElementCache(Convert.ToInt32(onlyIfNecessary), ref _ierr);
                }

                /// <summary>
                /// Get the nodes from all the elements belonging to the physical group of
                /// dimension `dim' and tag `tag'. `nodeTags' contains the node tags; `coord'
                /// is a vector of length 3 times the length of `nodeTags' that contains the x,
                /// y, z coordinates of the nodes, concatenated: [n1x, n1y, n1z, n2x, ...]. 
                /// </summary>
                public static void GetNodesForPhysicalGroup(int dim, int tag, out long[] nodeTags, out double[] coord)
                {
                    IntPtr ntP, cP;
                    long nodeTags_n, coord_n;

                    IWrap.GmshModelMeshGetNodesForPhysicalGroup(dim, tag, out ntP, out nodeTags_n, out cP, out coord_n, ref _ierr);

                    nodeTags = new long[nodeTags_n];
                    coord = new double[coord_n];
                    Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);
                    Marshal.Copy(cP, coord, 0, (int)coord_n);

                    Free(ntP);
                    Free(cP);
                }

                /// <summary>
                /// Relocate the nodes classified on the entity of dimension `dim' and tag
                /// `tag' using their parametric coordinates. If `tag' < 0, relocate the nodes
                /// for all entities of dimension `dim'. If `dim' and `tag' are negative,
                /// relocate all the nodes in the mesh.
                /// </summary>
                public static void RelocateNodes(int dim, int tag)
                {
                    IWrap.GmshModelMeshRelocateNodes(dim, tag, ref _ierr);
                }

                /// <summary>
                /// Get the type and node tags of the element with tag `tag'. This function
                /// relies on an internal cache(a vector in case of dense element numbering, a
                /// map otherwise); for large meshes accessing elements in bulk is often preferable.
                /// </summary>
                public static void GetElement(long elementTag, out int elementType, out long[] nodeTags)
                {
                    IntPtr ntP;
                    long nodeTags_n;
                    IWrap.GmshModelMeshGetElement(elementTag, out elementType, out ntP, out nodeTags_n, ref _ierr);

                    nodeTags = new long[nodeTags_n];
                    Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);

                    Free(ntP);
                }

                /// <summary>
                /// Search the mesh for an element located at coordinates (`x', `y', `z'). This
                /// function performs a search in a spatial octree.If an element is found,
                /// return its tag, type and node tags, as well as the local coordinates(`u',
                /// `v', `w') within the reference element corresponding to search location.If
                /// `dim' is >= 0, only search for elements of the given dimension. If `strict'
                /// is not set, use a tolerance to find elements near the search location.
                /// </summary>
                public static void GetElementByCoordinates(double x, double y, double z, out long elementTag, out int elementType, out long[] nodeTags, out double u, out double v, out double w, int dim, bool strict = false)
                {
                    IntPtr ntP;
                    long nodeTags_n;
                    IWrap.GmshModelMeshGetElementByCoordinates(x, y, z, out elementTag, out elementType, out ntP, out nodeTags_n, out u, out v, out w, dim, Convert.ToInt32(strict), ref _ierr);

                    nodeTags = new long[nodeTags_n];
                    Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);

                    Free(ntP);
                }

                /// <summary>
                /// Search the mesh for element(s) located at coordinates (`x', `y', `z'). This
                /// function performs a search in a spatial octree.Return the tags of all
                /// found elements in `elementTags'. Additional information about the elements
                /// can be accessed through `getElement' and `getLocalCoordinatesInElement'. If
                /// `dim' is >= 0, only search for elements of the given dimension. If `strict'
                /// is not set, use a tolerance to find elements near the search location.
                /// </summary>
                public static void GetElementsByCoordinates(double x, double y, double z, out long[] elementTags, int dim, bool strict = false)
                {
                    IntPtr etP;
                    long elementTags_n;
                    IWrap.GmshModelMeshGetElementsByCoordinates(x, y, z, out etP, out elementTags_n, dim, Convert.ToInt32(strict), ref _ierr);

                    elementTags = new long[elementTags_n];
                    Marshal.Copy(etP, elementTags, 0, (int)elementTags_n);

                    Free(etP);
                }

                /// <summary>
                /// Return the local coordinates (`u', `v', `w') within the element
                /// `elementTag' corresponding to the model coordinates (`x', `y', `z'). This
                /// function relies on an internal cache(a vector in case of dense element
                /// numbering, a map otherwise); for large meshes accessing elements in bulk is
                /// often preferable.
                /// </summary>
                public static void GetLocalCoordinatesInElement(long elementTag, double x, double y, double z, out double u, out double v, out double w)
                {
                    IWrap.GmshModelMeshGetLocalCoordinatesInElement(elementTag, x, y, z, out u, out v, out w, ref _ierr);
                }

                /// <summary>
                /// Get the types of elements in the entity of dimension `dim' and tag `tag'.
                /// If `tag' < 0, get the types for all entities of dimension `dim'. If `dim'
                /// and `tag' are negative, get all the types in the mesh.
                /// </summary>
                public static void GetElementTypes(out int[] elementTypes, int dim, int tag)
                {
                    IntPtr etP;
                    long elementTypes_n;
                    IWrap.GmshModelMeshGetElementTypes(out etP, out elementTypes_n, dim, tag, ref _ierr);

                    elementTypes = new int[elementTypes_n];
                    Marshal.Copy(etP, elementTypes, 0, (int)elementTypes_n);

                    Free(etP);
                }

                /// <summary>
                /// Return an element type given its family name `familyName' ("point", "line",
                /// "triangle", "quadrangle", "tetrahedron", "pyramid", "prism", "hexahedron")
                /// and polynomial order `order'. If `serendip' is true, return the
                /// corresponding serendip element type(element without interior nodes).
                /// </summary>
                public static void GetElementType(string familyName, int order, bool serendip = false)
                {
                    IWrap.GmshModelMeshGetElementType(familyName, order, Convert.ToInt32(serendip), ref _ierr);
                }

                /// <summary>
                /// Get the properties of an element of type `elementType': its name
                /// (`elementName'), dimension (`dim'), order(`order'), number of nodes
                /// (`numNodes'), local coordinates of the nodes in the reference element
                /// (`localNodeCoord' vector, of length `dim' times `numNodes') and number of
                /// primary (first order) nodes (`numPrimaryNodes').
                /// </summary>
                public static void GetElementProperties(int elementType, string elementName, out int dim, out int order, out int numNodes, out double[] localNodeCoord, out int numPrimaryNodes)
                {
                    IntPtr lcdP;
                    long localNodeCoord_n;
                    IWrap.GmshModelMeshGetElementProperties(elementType, elementName, out dim, out order, out numNodes, out lcdP, out localNodeCoord_n, out numPrimaryNodes, ref _ierr);

                    localNodeCoord = new double[localNodeCoord_n];
                    Marshal.Copy(lcdP, localNodeCoord, 0, (int)localNodeCoord_n);

                    Free(lcdP);
                }

                /// <summary>
                /// Get the elements of type `elementType' classified on the entity of tag
                /// `tag'. If `tag' < 0, get the elements for all entities. `elementTags' is a
                /// vector containing the tags (unique, strictly positive identifiers) of the
                /// elements of the corresponding type. `nodeTags' is a vector of length equal
                /// to the number of elements of the given type times the number N of nodes for
                /// this type of element, that contains the node tags of all the elements of
                /// the given type, concatenated: [e1n1, e1n2, ..., e1nN, e2n1, ...]. If
                /// `numTasks' > 1, only compute and return the part of the data indexed by `task'.
                /// </summary>
                public static void GetElementsByType(int elementType, out long[] elementTags, out long[] nodeTags, int tag, long task, long numTasks)
                {
                    IntPtr etP, ntP;
                    long elementTags_n, nodeTags_n;
                    IWrap.GmshModelMeshGetElementsByType(elementType, out etP, out elementTags_n, out ntP, out nodeTags_n, tag, task, numTasks, ref _ierr);

                    elementTags = new long[elementTags_n];
                    nodeTags = new long[nodeTags_n];
                    Marshal.Copy(etP, elementTags, 0, (int)elementTags_n);
                    Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);

                    Free(ntP);
                    Free(etP);
                }

                /// <summary>
                /// Get the Jacobians of all the elements of type `elementType' classified on
                /// the entity of tag `tag', at the G evaluation points `localCoord' given as
                /// concatenated triplets of coordinates in the reference element[g1u, g1v,
                /// g1w, ..., gGu, gGv, gGw]. Data is returned by element, with elements in the
                /// same order as in `getElements' and `getElementsByType'. `jacobians'
                /// contains for each element the 9 entries of the 3x3 Jacobian matrix at each
                /// evaluation point.The matrix is returned by column: [e1g1Jxu, e1g1Jyu,
                /// e1g1Jzu, e1g1Jxv, ..., e1g1Jzw, e1g2Jxu, ..., e1gGJzw, e2g1Jxu, ...], with
                /// Jxu = dx / du, Jyu = dy / du, etc. `determinants' contains for each element the
                /// determinant of the Jacobian matrix at each evaluation point: [e1g1, e1g2,
                /// ... e1gG, e2g1, ...]. `coord' contains for each element the x, y, z
                /// coordinates of the evaluation points.If `tag' < 0, get the Jacobian data
                /// for all entities.If `numTasks' > 1, only compute and return the part of
                /// the data indexed by `task'.
                /// </summary>
                public static void GetJacobians(int elementType, double[] localCoord, out double[] jacobians, out double[] determinants, out double[] coord, int tag, long task, long numTasks)
                {
                    IntPtr jP, dP, cP;
                    long jacobians_n, determinants_n, coord_n;
                    IWrap.GmshModelMeshGetJacobians(elementType, localCoord, localCoord.LongLength, out jP, out jacobians_n, out dP, out determinants_n, out cP, out coord_n, tag, task, numTasks, ref _ierr);

                    jacobians = new double[jacobians_n];
                    determinants = new double[determinants_n];
                    coord = new double[coord_n];
                    Marshal.Copy(jP, jacobians, 0, (int)jacobians_n);
                    Marshal.Copy(dP, determinants, 0, (int)determinants_n);
                    Marshal.Copy(cP, coord, 0, (int)coord_n);

                    Free(jP);
                    Free(dP);
                    Free(cP);
                }

                /// <summary>
                ///  Get the Jacobian for a single element `elementTag', at the G evaluation
                ///  points `localCoord' given as concatenated triplets of coordinates in the
                ///  reference element[g1u, g1v, g1w, ..., gGu, gGv, gGw]. `jacobians' contains
                ///  the 9 entries of the 3x3 Jacobian matrix at each evaluation point.The
                ///  matrix is returned by column: [e1g1Jxu, e1g1Jyu, e1g1Jzu, e1g1Jxv, ...,
                ///  e1g1Jzw, e1g2Jxu, ..., e1gGJzw, e2g1Jxu, ...], with Jxu = dx / du, Jyu = dy / du,
                ///  etc. `determinants' contains the determinant of the Jacobian matrix at each
                ///  evaluation point. `coord' contains the x, y, z coordinates of the
                ///  evaluation points. This function relies on an internal cache(a vector in
                ///  case of dense element numbering, a map otherwise); for large meshes
                ///  accessing Jacobians in bulk is often preferable.
                /// </summary>
                public static void GetJacobian(long elementTag, double[] localCoord, out double[] jacobians, out double[] determinants, out double[] coord)
                {
                    IntPtr jP, dP, cP;
                    long jacobians_n, determinants_n, coord_n;

                    IWrap.GmshModelMeshGetJacobian(elementTag, localCoord, localCoord.LongLength, out jP, out jacobians_n, out dP, out determinants_n, out cP, out coord_n, ref _ierr);

                    jacobians = new double[jacobians_n];
                    determinants = new double[determinants_n];
                    coord = new double[coord_n];
                    Marshal.Copy(jP, jacobians, 0, (int)jacobians_n);
                    Marshal.Copy(dP, determinants, 0, (int)determinants_n);
                    Marshal.Copy(cP, coord, 0, (int)coord_n);

                    Free(jP);
                    Free(dP);
                    Free(cP);
                }

                /// <summary>
                /// Get information about the `keys'. `infoKeys' returns information about the
                /// functions associated with the `keys'. `infoKeys[0].first' describes the
                /// type of function(0 for  vertex function, 1 for edge function, 2 for face
                /// function and 3 for bubble function). `infoKeys[0].second' gives the order
                /// of the function associated with the key.Warning: this is an experimental
                /// feature and will probably change in a future release.
                /// </summary>
                public static void GetInformationForElements(int[] keys, int elementType, string functionSpaceType, out Tuple<int, int>[] infoKeys)
                {
                    IntPtr ikP;
                    long infoKeys_n;
                    IWrap.GmshModelMeshGetInformationForElements(keys, keys.LongLength, elementType, functionSpaceType, out ikP, out infoKeys_n, ref _ierr);

                    var temp = new int[infoKeys_n];
                    Marshal.Copy(ikP, temp, 0, (int)infoKeys_n);
                    infoKeys = IHelpers.GraftIntTupleArray(temp);

                    Free(ikP);
                }

                /// <summary>
                /// Get the barycenters of all elements of type `elementType' classified on the
                /// entity of tag `tag'. If `primary' is set, only the primary nodes of the
                /// elements are taken into account for the barycenter calculation.If `fast'
                /// is set, the function returns the sum of the primary node coordinates
                /// (without normalizing by the number of nodes). If `tag' < 0, get the
                /// barycenters for all entities.If `numTasks' > 1, only compute and return
                /// the part of the data indexed by `task'.
                /// </summary>
                public static void GetBarycenters(int elementType, int tag, bool fast, bool primary, out double[] barycenters, long task, long numTasks)
                {
                    IntPtr bP;
                    long barycenters_n;
                    IWrap.GmshModelMeshGetBarycenters(elementType, tag, Convert.ToInt32(fast), Convert.ToInt32(primary), out bP, out barycenters_n, task, numTasks, ref _ierr);

                    barycenters = new double[barycenters_n];
                    Marshal.Copy(bP, barycenters, 0, (int)barycenters_n);

                    Free(bP);
                }

                /// <summary>
                /// Get the nodes on the edges of all elements of type `elementType' classified
                /// on the entity of tag `tag'. `nodeTags' contains the node tags of the edges
                /// for all the elements: [e1a1n1, e1a1n2, e1a2n1, ...]. Data is returned by
                /// element, with elements in the same order as in `getElements' and
                /// `getElementsByType'. If `primary' is set, only the primary(begin/end)
                /// nodes of the edges are returned.If `tag' < 0, get the edge nodes for all
                /// entities.If `numTasks' > 1, only compute and return the part of the data
                /// indexed by `task'. 
                /// </summary>
                public static void GetElementEdgeNodes(int elementType, out long[] nodeTags, int tag, bool primary, long task, long numTasks)
                {
                    IntPtr ntP;
                    long nodeTags_n;
                    IWrap.GmshModelMeshGetElementEdgeNodes(elementType, out ntP, out nodeTags_n, tag, Convert.ToInt32(primary), task, numTasks, ref _ierr);

                    nodeTags = new long[nodeTags_n];
                    Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);

                    Free(ntP);
                }

                /// <summary>
                /// Get the nodes on the faces of type `faceType' (3 for triangular faces, 4
                /// for quadrangular faces) of all elements of type `elementType' classified on
                /// the entity of tag `tag'. `nodeTags' contains the node tags of the faces for
                /// all elements: [e1f1n1, ..., e1f1nFaceType, e1f2n1, ...]. Data is returne
                /// by element, with elements in the same order as in `getElements' and
                /// `getElementsByType'. If `primary' is set, only the primary(corner) nodes
                /// of the faces are returned.If `tag' < 0, get the face nodes for all
                /// entities.If `numTasks' > 1, only compute and return the part of the data
                /// indexed by `task'. 
                /// </summary>
                public static void GetElementFaceNodes(int elementType, int faceType, out long[] nodeTags, int tag, bool primary, long task, long numTasks)
                {
                    IntPtr ntP;
                    long nodeTags_n;
                    IWrap.GmshModelMeshGetElementFaceNodes(elementType, faceType, out ntP, out nodeTags_n, tag, Convert.ToInt32(primary), task, numTasks, ref _ierr);

                    nodeTags = new long[nodeTags_n];
                    Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);

                    Free(ntP);
                }

                /// <summary>
                /// Reorder the elements of type `elementType' classified on the entity of tag `tag' according to `ordering'.
                /// </summary>
                public static void ReorderElements(int elementType, int tag, long[] ordering)
                {
                    IWrap.GmshModelMeshReorderElements(elementType, tag, ordering, ordering.LongLength, ref _ierr);
                }

                /// <summary>
                /// Get the basis functions of the element of type `elementType' at the
                /// evaluation points `localCoord' (given as concatenated triplets of
                /// coordinates in the reference element[g1u, g1v, g1w, ..., gGu, gGv, gGw]),
                /// for the function space `functionSpaceType' (e.g. "Lagrange" or
                /// "GradLagrange" for Lagrange basis functions or their gradient, in the u, v,
                /// w coordinates of the reference element; or "H1Legendre3" or
                /// "GradH1Legendre3" for 3rd order hierarchical H1 Legendre functions).
                /// `numComponents' returns the number C of components of a basis function.
                /// `basisFunctions' returns the value of the N basis functions at the
                /// evaluation points, i.e. [g1f1, g1f2, ..., g1fN, g2f1, ...] when C == 1 or
                /// [g1f1u, g1f1v, g1f1w, g1f2u, ..., g1fNw, g2f1u, ...] when C == 3. For basis
                /// functions that depend on the orientation of the elements, all values for
                /// the first orientation are returned first, followed by values for the
                /// second, etc. `numOrientations' returns the overall number of orientations.
                /// If `wantedOrientations' is not empty, only return the values for the desired orientation indices.
                /// </summary>
                public static void GetBasisFunctions(int elementType, double[] localCoord, string functionSpaceType, out int numComponents, out double[] basisFunctions, out int numOrientations, int[] wantedOrientations)
                {
                    IntPtr bfP;
                    long basisFunctions_n;
                    IWrap.GmshModelMeshGetBasisFunctions(elementType, localCoord, localCoord.LongLength, functionSpaceType, out numComponents, out bfP, out basisFunctions_n, out numOrientations, wantedOrientations, wantedOrientations.LongLength, ref _ierr);

                    basisFunctions = new double[0];
                    if (basisFunctions_n > 0)
                    {
                        basisFunctions = new double[basisFunctions_n];
                        Marshal.Copy(bfP, basisFunctions, 0, (int)basisFunctions_n);
                    }

                    Free(bfP);
                }

                /// <summary>
                /// Get the orientation index of the elements of type `elementType' in the
                /// entity of tag `tag'. The arguments have the same meaning as in
                /// `getBasisFunctions'. `basisFunctionsOrientation' is a vector giving for
                /// each element the orientation index in the values returned by
                /// `getBasisFunctions'. For Lagrange basis functions the call is superfluous
                /// as it will return a vector of zeros.
                /// </summary>
                public static void GetBasisFunctionsOrientationForElements(int elementType, string functionSpaceType, out int[] basisFunctionsOrientation, int tag, long task, long numTasks)
                {
                    IntPtr bP;
                    long basisFunctionsOrientation_n;
                    IWrap.GmshModelMeshGetBasisFunctionsOrientationForElements(elementType, functionSpaceType, out bP, out basisFunctionsOrientation_n, tag, task, numTasks, ref _ierr);

                    basisFunctionsOrientation = new int[basisFunctionsOrientation_n];
                    Marshal.Copy(bP, basisFunctionsOrientation, 0, (int)basisFunctionsOrientation_n);

                    Free(bP);
                }

                /// <summary>
                /// Get the orientation of a single element `elementTag'.
                /// </summary>
                public static void GetBasisFunctionsOrientationForElement(long elementTag, string functionSpaceType, out int basisFunctionsOrientation)
                {
                    IWrap.GmshModelMeshGetBasisFunctionsOrientationForElement(elementTag, functionSpaceType, out basisFunctionsOrientation, ref _ierr);
                }

                /// <summary>
                /// Get the number of possible orientations for elements of type `elementType'
                /// and function space named `functionSpaceType'.
                /// </summary>
                public static void GetNumberOfOrientations(int elementType, string functionSpaceType)
                {
                    IWrap.GmshModelMeshGetNumberOfOrientations(elementType, functionSpaceType, ref _ierr);
                }

                /// <summary>
                /// Add a new mesh size field of type `fieldType'. 
                /// If `tag' is positive, assign the tag explicitly; otherwise a new tag is assigned automatically.Return the field tag.
                /// </summary>
                /// <param name="fieldType"></param>
                /// <param name="tag"></param>
                /// <returns></returns>
                public static int AddMeshField(string fieldType, int tag = -1)
                {
                    return IWrap.GmshModelMeshFieldAdd(fieldType, tag, ref _ierr);
                }

                /// <summary>
                /// Remove the field with tag `tag'.
                /// </summary>
                /// <param name="tag"></param>
                public static void RemoveMeshField(int tag)
                {
                    IWrap.GmshModelMeshFieldRemove(tag, ref _ierr);
                }

                /// <summary>
                /// Set the numerical option `option' to value `value' for field `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="option"></param>
                /// <param name="value"></param>
                public static void SetMeshFieldOptionNumber(int tag, string option, double value)
                {
                    IWrap.GmshModelMeshFieldSetNumber(tag, option, value, ref _ierr);
                }

                /// <summary>
                /// Set the string option `option' to value `value' for field `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="option"></param>
                /// <param name="value"></param>
                public static void SetMeshFieldOptionString(int tag, string option, string value)
                {
                    IWrap.GmshModelMeshFieldSetString(tag, option, value, ref _ierr);
                }

                /// <summary>
                /// Set the numerical list option `option' to value `value' for field `tag'.
                /// </summary>
                /// <param name="tag"></param>
                /// <param name="option"></param>
                /// <param name="value"></param>
                public static void SetMeshFieldOptionNumbers(int tag, string option, double[] value)
                {
                    IWrap.GmshModelMeshFieldSetNumbers(tag, option, value, value.LongLength, ref _ierr);
                }

                /// <summary>
                /// Set the field `tag' as the background mesh size field. 
                /// </summary>
                /// <param name="tag"></param>
                public static void SetMeshFieldAsBackgroundMesh(int tag)
                {
                    IWrap.GmshModelMeshFieldSetAsBackgroundMesh(tag, ref _ierr);
                }

                /// <summary>
                /// Set the field `tag' as a boundary layer size field.
                /// </summary>
                /// <param name="tag"></param>
                public static void SetMeshFieldAsBoundaryLayer(int tag)
                {
                    IWrap.GmshModelMeshFieldSetAsBoundaryLayer(tag, ref _ierr);
                }
                #endregion
            }
        }
    }
}
