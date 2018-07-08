using System;

namespace ElementWorldMaker.Existence.Ether
{
    public static class EtherResistanceMaker
    {
        public static int[,,] Make(int[,,] windEarthEnvironment, int[,,] waterFireEnvironment, int[,,] woodMetalEnvironment)
        {
            int[,,] EtherResistanceMap = new int[windEarthEnvironment.GetLength(0), windEarthEnvironment.GetLength(1), windEarthEnvironment.GetLength(2)];

            for (int wind = 0; wind < windEarthEnvironment.GetLength(2); wind++)
            {
                for (int wood = 0; wood < windEarthEnvironment.GetLength(1); wood++)
                {
                    for (int water = 0; water < windEarthEnvironment.GetLength(0); water++)
                    {
                        EtherResistanceMap[water, wood, wind] = Math.Abs(windEarthEnvironment[water, wood, wind]) + Math.Abs(waterFireEnvironment[water, wood, wind]) + Math.Abs(woodMetalEnvironment[water, wood, wind]);
                    }
                }
            }
            return EtherResistanceMap;
        }
    }
}