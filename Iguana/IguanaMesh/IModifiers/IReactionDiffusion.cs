using System;
using System.Collections.Generic;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.IModifiers
{
    public class IReactionDiffusion
    {
        public double FeedFactor { get; set; } = 0.055;
        public double KillFactor { get; set; } = 0.062;
        public double DiffusionRateA { get; set; } = 1.0;
        public double DiffusionRateB { get; set; } = 0.3;
        public int MaxInterations { get; set; } = 100;
        public double TimeStep { get; set; } = 1.0;
        public Vector3d[] AuxiliarVectorField { get; set; } = null;
        public double NeighborsContributionFactor { get; set; } = 1.0;
        public int RandomValuesPopulation { get; set; } = 10;

        private double[] A, B, nextA, nextB;
        private double[][] coef;
        private int count;

        public IReactionDiffusion() { }

        public IReactionDiffusion(double dA, double dB, double k, double f, double dt, double nF, int iter, int rP) {
            DiffusionRateA = dA;
            DiffusionRateB = dB;
            FeedFactor = f;
            KillFactor = k;
            TimeStep = dt;
            NeighborsContributionFactor = nF;
            MaxInterations = iter;
            RandomValuesPopulation = rP;
        }

        private void init(IMesh mesh)
        {
            count = mesh.Vertices.Count;
            A = new double[count];
            B = new double[count];
            nextA = new double[count];
            nextB = new double[count];
            coef = new double[count][];
            double teta;
            IVector3D direction;

            for (int i = 0; i < count; i++)
            {
                int vK = mesh.Vertices.VerticesKeys[i];
                int[] vStart = mesh.Topology.GetVertexAdjacentVertices(vK);
                ITopologicVertex vertex = mesh.Vertices.GetVertexWithKey(vK);

                A[i] = 1;
                B[i] = 0;

                coef[i] = new double[vStart.Length];

                for (int j = 0; j < vStart.Length; j++)
                {
                    ITopologicVertex neighbor = mesh.Vertices.GetVertexWithKey(vStart[j]);
                    direction = neighbor.Position - vertex;

                    teta = 1.0;
                    if (AuxiliarVectorField != null && AuxiliarVectorField.Length>0)
                    {
                        if(AuxiliarVectorField.Length==count)
                        teta = IVector3D.AngleBetween(direction, new IVector3D(AuxiliarVectorField[i]));
                        else teta = IVector3D.AngleBetween(direction, new IVector3D(AuxiliarVectorField[0]));
                    }

                    coef[i][j] = Math.Sqrt(Math.Sin(teta) * Math.Sin(teta) + Math.Pow(NeighborsContributionFactor,2) * Math.Cos(teta) * Math.Cos(teta));
                }
            }

            Random rnd = new Random();
            for (int i = 0; i < RandomValuesPopulation; i++)
            {
                int idx = rnd.Next(0, count);

                for (int j = idx; j < idx + 10; j++)
                {
                    A[j] = 1;
                    B[j] = 1;
                }
            }
        }


        public IMesh ApplyModifier(IMesh mesh)
        {
            if (mesh != null)
            {
                this.init(mesh);

                IMesh nMesh = new IMesh();

                for (int iter = 0; iter < MaxInterations; iter++)
                {
                    for (int i = 0; i < count; i++)
                    {
                        //Laplacian calculation
                        double dxA = 0.0;
                        double dxB = 0.0;

                        int vK = mesh.Vertices.VerticesKeys[i];
                        int[] vStart = mesh.Topology.GetVertexAdjacentVertices(vK);

                        for (int j = 0; j < vStart.Length; j++)
                        {
                            int idx = vStart[j];

                            dxA += coef[i][j] * (A[idx] - A[i]) / vStart.Length;
                            dxB += coef[i][j] * (B[idx] - B[i]) / vStart.Length;
                        }

                        //Reaction-diffusion equation
                        double AB2 = A[i] * B[i] * B[i];
                        nextA[i] = A[i] + (DiffusionRateA * dxA - AB2 + FeedFactor * (1 - A[i])) * TimeStep;
                        nextB[i] = B[i] + (DiffusionRateB * dxB + AB2 - (KillFactor + FeedFactor) * B[i]) * TimeStep;

                    }


                    for (int i = 0; i < count; i++)
                    {
                        A[i] = Math.Min(Math.Max(nextA[i], 0.0), 1.0);
                        B[i] = Math.Min(Math.Max(nextB[i], 0.0), 1.0);
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    int vK = mesh.Vertices.VerticesKeys[i];
                    ITopologicVertex vertex = mesh.Vertices.GetVertexWithKey(vK);

                    double param = (A[i] - B[i]);

                    IVector3D n = mesh.Topology.ComputeVertexNormal(vK);
                    n*= (A[i] - B[i]);

                    vertex.Position += n;

                    nMesh.Vertices.AddVertex(vK, vertex);
                }

                nMesh.Elements.AddRangeElements(mesh.Elements.ElementsValues);

                nMesh.BuildTopology();

                return nMesh;
            }
            else return null;
        }
    }
}
