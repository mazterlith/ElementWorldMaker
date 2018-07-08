using System;
using System.Windows.Media.Media3D;

namespace ElementWorldMaker.Existence
{
    public class MassVectorMap3D
    {
        public int Width { get; protected set; }
        public int Length { get; protected set; }
        public int Height { get; protected set; }

        public bool[,,] SolidWorld { get; protected set; }
        
        private double _initialMass;
        public double[,,] MassMap { get; protected set; }

        public double internalMinimum;
        public double internalMaximum;
        public double internalRange;
        public double minimum;
        public double maximum;
        public double range;
        
        private double _nearestNeighborContribution;
        private double _rescale;
        public Vector3D[,,] VectorMap { get; protected set; }
        protected Random _rand = new Random();

        //nearestNeighborContribution ~ (675 - 6 * initialMass) / (26 * (1006 * initialMass - 675))
        public MassVectorMap3D(int width, int length, int height, double nearestNeighborContribution, double initialMass)
        {
            _nearestNeighborContribution = nearestNeighborContribution;
            _initialMass = initialMass;

            _rescale = 1.0 / (new Vector3D(1.0, 1.0, 1.0).Length * (26.0 * nearestNeighborContribution + 1.0));

            Width = width;
            Length = length;
            Height = height;

            SolidWorld = new bool[Width, Length, Height];
            MassMap = new double[Width, Length, Height];

            ReinitVectorMap();
        }

        private void ReinitVectorMap()
        {
            VectorMap = new Vector3D[Width, Length, Height];

            for (int k = 1; k < Height - 1; k++)
            {
                for (int j = 1; j < Length - 1; j++)
                {
                    for (int i = 1; i < Width - 1; i++)
                    {
                        MassMap[i, j, k] = _initialMass;
                        VectorMap[i, j, k] = new Vector3D(_rand.NextDouble() * 2 - 1.0, _rand.NextDouble() * 2 - 1.0, _rand.NextDouble() * 2 - 1.0);
                    }
                }
            }
        }

        private void GetNearestNeighborContribution(int width, int length, int height)
        {
            Vector3D middleValue = VectorMap[width, length, height]; //Preserve middle value

            for (int k = -1; k < 2; k++)
            {
                for (int j = -1; j < 2; j++)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        VectorMap[width + i, length + j, height + k] += middleValue * _nearestNeighborContribution * MassMap[width, length, height];
                    }
                }
            }

            VectorMap[width, length, height] = middleValue; //Restore middle value
        }

        private void MoveMass(int width, int length, int height)
        {
            double massFlux = 0.0;
            for (int k = -1; k < 2; k++)
            {
                for (int j = -1; j < 2; j++)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        Vector3D directionalVector = new Vector3D(i, j, k);
                        if (directionalVector.Length == 0)
                            continue;
                        Vector3D unitDirectionalVector = directionalVector / directionalVector.Length;
                        double massScalar = Vector3D.DotProduct(VectorMap[width + i, length + j, height + k], unitDirectionalVector);
                        double massDifference = massScalar * MassMap[width + i, length + j, height + k] * _nearestNeighborContribution;
                        massFlux += massDifference;
                    }
                }
            }
            MassMap[width, length, height] += massFlux * -1;
        }

        public void IterateMassMap()
        {
            //Smoothing
            for (int k = 1; k < Height - 1; k++)
            {
                for (int j = 1; j < Length - 1; j++)
                {
                    for (int i = 1; i < Width - 1; i++)
                    {
                        GetNearestNeighborContribution(i, j, k);
                    }
                }
            }

            for (int k = 0; k < Height; k++)
            {
                for (int j = 0; j < Length; j++)
                {
                    for (int i = 0; i < Width; i++)
                    {
                        VectorMap[i, j, k] *= _rescale;
                    }
                }
            }

            double minimum = double.MaxValue;
            double maximum = double.MinValue;
            
            //Mass movement
            for (int k = 1; k < Height - 1; k++)
            {
                for (int j = 1; j < Length - 1; j++)
                {
                    for (int i = 1; i < Width - 1; i++)
                    {
                        MoveMass(i, j, k);

                        if (minimum > MassMap[i, j, k])
                            minimum = MassMap[i, j, k];
                        if (maximum < MassMap[i, j, k])
                            maximum = MassMap[i, j, k];
                    }
                }
            }

            double inverseRange = 1 / (maximum - minimum);
            
            //Renormalization
            for (int k = 1; k < Height - 1; k++)
            {
                for (int j = 1; j < Length - 1; j++)
                {
                    for (int i = 1; i < Width - 1; i++)
                    {
                        MassMap[i, j, k] -= minimum;
                        MassMap[i, j, k] *= inverseRange;
                    }
                }
            }
        }

        public struct HistorgramInfo
        {
            public double LowerInclusiveBound { get; set; }
            public double UpperExclusiveBound { get; set; }
            public int Frequency { get; set; }
        }

        public static MassVectorMap3D GenerateCloudMap(int width, int length, int height, int iterations, double initialMass = 1.0, double nearestNeighborContribution = .05)
        {
            // nearestNeighborContribution = (675 - 6 * initialMass) / (26 * ((1000 + 6) * initialMass - 675)) <- limit relation

            MassVectorMap3D mvm = new MassVectorMap3D(width, length, height, nearestNeighborContribution, initialMass);
            double specialValue1 = 1.0 / (26.0 * nearestNeighborContribution + 1.0);

            if (iterations <= 0)
                return mvm;

            for (int i = 0; i < iterations; i++)
            {
                mvm.IterateMassMap();
            }

            for (int k = 0; k < height; k++)
            {
                for (int j = 0; j < length; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        mvm.MassMap[i, j, k] *= k / (double)height;
                    }
                }
            }

            return mvm;
        }
    }
}