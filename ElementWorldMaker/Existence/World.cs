using ElementWorldMaker.Existence.EnvironmentMaker;
using ElementWorldMaker.Existence.Ether;
using ElementWorldMaker.Existence.WaterFire;
using ElementWorldMaker.Existence.WindEarth;
using ElementWorldMaker.Existence.WoodMetal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElementWorldMaker.Existence
{
    public class World
    {
        public ElementPoint Size { get; protected set; }
        public int MaxElementalVariance { get; set; }

        public int[,,] WaterWorld { get; protected set; }
        public int[,,] WoodWorld { get; protected set; }
        public int[,,] WindWorld { get; protected set; }

        public int[,,] WindEarthEnvironment { get; protected set; }
        public int[,,] WaterFireEnvironment { get; protected set; }
        public int[,,] WoodMetalEnvironment { get; protected set; }
        public int[,,] EtherResistanceEnvironment { get; protected set; }
        public int[,,] EtherEnvironment { get; protected set; }

        public double RigidWaterWorldNumber { get; set; } = 1.0;
        public double RigidWoodWorldNumber { get; set; } = 1.0;
        public double RigidWindWorldNumber { get; set; } = 1.0;

        protected Random _rand = new Random();

        public World(int waterSize, int woodSize, int windSize, int maxElementalVariance = 10)
        {
            Size = new ElementPoint() { waterPosition = waterSize, woodPosition = woodSize, windPosition = windSize };
            MaxElementalVariance = maxElementalVariance;

            WaterWorld = new int[waterSize, woodSize, windSize];
            WoodWorld = new int[waterSize, woodSize, windSize];
            WindWorld = new int[waterSize, woodSize, windSize];

            RemakeElementalMap();
            WindEarthEnvironment = WindEarthMaker.Make(waterSize, woodSize, windSize);
            WaterFireEnvironment = WaterFireMaker.Make(waterSize, woodSize, windSize);
            WoodMetalEnvironment = WoodMetalMaker.Make(waterSize, woodSize, windSize);
            EtherResistanceEnvironment = EtherResistanceMaker.Make(WindEarthEnvironment, WaterFireEnvironment, WoodMetalEnvironment);
            EtherEnvironment = EtherMaker.Make(EtherResistanceEnvironment);
        }

        public void UpdateElementalValuesAndRemake(int? maxElementalVariance = null, double? rigidWaterWorldNumber = null, double? rigidWoodWorldNumber = null, double? rigidWindWorldNumber = null)
        {
            if (maxElementalVariance != null)
            {
                MaxElementalVariance = maxElementalVariance.Value;
            }
            if (rigidWaterWorldNumber != null)
            {
                RigidWaterWorldNumber = rigidWaterWorldNumber.Value;
            }
            if (rigidWoodWorldNumber != null)
            {
                RigidWoodWorldNumber = rigidWoodWorldNumber.Value;
            }
            if (rigidWindWorldNumber != null)
            {
                RigidWindWorldNumber = rigidWindWorldNumber.Value;
            }

            RemakeElementalMap();
        }

        public void RemakeElementalMap()
        {
            RemakeElementalMapWaterBoundaries();

            RemakeElementalMapWoodBoundaries();

            RemakeElementalMapWindBoundaries();

            RemakeElementalMapNonBoundaries();
        }

        private void RemakeElementalMapWaterBoundaries()
        {
            int WaterSize = Size.waterPosition;
            int WoodSize = Size.woodPosition;
            int WindSize = Size.windPosition;

            for (int windPosition = 0; windPosition < WindSize; windPosition++)
            {
                for (int woodPosition = 0; woodPosition < WoodSize; woodPosition++)
                {
                    WaterWorld[0, woodPosition, windPosition] = 100;
                    WaterWorld[WaterSize - 1, woodPosition, windPosition] = -100;

                    double fractionAlongWorld = woodPosition / (double)(WoodSize - 1);
                    int varianceRelativeTo = (int)(200 * SpinalOperations.WorldSpinalValue(fractionAlongWorld, RigidWoodWorldNumber) - 100);

                    WoodWorld[0, woodPosition, windPosition] = varianceRelativeTo;
                    WoodWorld[WaterSize - 1, woodPosition, windPosition] = varianceRelativeTo;

                    fractionAlongWorld = windPosition / (double)(WindSize - 1);
                    varianceRelativeTo = (int)(200 * SpinalOperations.WorldSpinalValue(fractionAlongWorld, RigidWindWorldNumber) - 100);

                    WindWorld[0, woodPosition, windPosition] = varianceRelativeTo;
                    WindWorld[WaterSize - 1, woodPosition, windPosition] = varianceRelativeTo;
                }
            }
        }

        private void RemakeElementalMapWoodBoundaries()
        {
            int WaterSize = Size.waterPosition;
            int WoodSize = Size.woodPosition;
            int WindSize = Size.windPosition;

            for (int windPosition = 0; windPosition < WindSize; windPosition++)
            {
                for (int waterPosition = 0; waterPosition < WaterSize; waterPosition++)
                {
                    double fractionAlongWorld = waterPosition / (double)(WaterSize - 1);
                    int varianceRelativeTo = (int)(200 * SpinalOperations.WorldSpinalValue(fractionAlongWorld, RigidWaterWorldNumber) - 100);

                    WaterWorld[waterPosition, 0, windPosition] = varianceRelativeTo;
                    WaterWorld[waterPosition, WoodSize - 1, windPosition] = varianceRelativeTo;

                    WoodWorld[waterPosition, 0, windPosition] = 100;
                    WoodWorld[waterPosition, WoodSize - 1, windPosition] = -100;

                    fractionAlongWorld = windPosition / (double)(WindSize - 1);
                    varianceRelativeTo = (int)(200 * SpinalOperations.WorldSpinalValue(fractionAlongWorld, RigidWindWorldNumber) - 100);

                    WindWorld[waterPosition, 0, windPosition] = varianceRelativeTo;
                    WindWorld[waterPosition, WoodSize - 1, windPosition] = varianceRelativeTo;
                }
            }
        }

        private void RemakeElementalMapWindBoundaries()
        {
            int WaterSize = Size.waterPosition;
            int WoodSize = Size.woodPosition;
            int WindSize = Size.windPosition;

            for (int woodPosition = 0; woodPosition < WoodSize; woodPosition++)
            {
                for (int waterPosition = 0; waterPosition < WaterSize; waterPosition++)
                {
                    double fractionAlongWorld = waterPosition / (double)(WaterSize - 1);
                    int varianceRelativeTo = (int)(200 * SpinalOperations.WorldSpinalValue(fractionAlongWorld, RigidWaterWorldNumber) - 100);

                    WaterWorld[waterPosition, woodPosition, 0] = varianceRelativeTo;
                    WaterWorld[waterPosition, woodPosition, WindSize - 1] = varianceRelativeTo;

                    fractionAlongWorld = woodPosition / (double)(WoodSize - 1);
                    varianceRelativeTo = (int)(200 * SpinalOperations.WorldSpinalValue(fractionAlongWorld, RigidWoodWorldNumber) - 100);

                    WoodWorld[waterPosition, woodPosition, 0] = varianceRelativeTo;
                    WoodWorld[waterPosition, woodPosition, WindSize - 1] = varianceRelativeTo;

                    WindWorld[waterPosition, woodPosition, 0] = 100;
                    WindWorld[waterPosition, woodPosition, WindSize - 1] = -100;
                }
            }
        }

        private void RemakeElementalMapNonBoundaries()
        {
            int WaterSize = Size.waterPosition;
            int WoodSize = Size.woodPosition;
            int WindSize = Size.windPosition;

            for (int windPosition = 1; windPosition < WindSize - 1; windPosition++)
            {
                for (int woodPosition = 1; woodPosition < WoodSize - 1; woodPosition++)
                {
                    for (int waterPosition = 1; waterPosition < WaterSize - 1; waterPosition++)
                    {
                        double fractionAlongWorld = waterPosition / (double)(WaterSize - 1);
                        int varianceRelativeTo = (int)(200 * Math.Pow((1 - Math.Pow(fractionAlongWorld, 1 / RigidWaterWorldNumber)), RigidWaterWorldNumber) - 100);

                        WaterWorld[waterPosition, woodPosition, windPosition] = varianceRelativeTo + _rand.Next(-MaxElementalVariance, MaxElementalVariance);

                        fractionAlongWorld = woodPosition / (double)(WoodSize - 1);
                        varianceRelativeTo = (int)(200 * Math.Pow((1 - Math.Pow(fractionAlongWorld, 1 / RigidWoodWorldNumber)), RigidWoodWorldNumber) - 100);

                        WoodWorld[waterPosition, woodPosition, windPosition] = varianceRelativeTo + _rand.Next(-MaxElementalVariance, MaxElementalVariance);

                        fractionAlongWorld = windPosition / (double)(WindSize - 1);
                        varianceRelativeTo = (int)(200 * Math.Pow((1 - Math.Pow(fractionAlongWorld, 1 / RigidWindWorldNumber)), RigidWindWorldNumber) - 100);

                        WindWorld[waterPosition, woodPosition, windPosition] = varianceRelativeTo + _rand.Next(-MaxElementalVariance, MaxElementalVariance);
                    }
                }
            }
        }
    }
}