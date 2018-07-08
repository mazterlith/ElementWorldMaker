using ElementWorldMaker.Existence.EnvironmentMaker;
using System;
using System.Collections.Generic;

namespace ElementWorldMaker.Existence.WindEarth
{
    public static class WindEarthMaker
    {
        public static int[,,] Make(int waterSize, int woodSize, int windSize)
        {
            int[,,] initialEnvironment = CreateInitialEnvironment(waterSize, woodSize, windSize);
            int[,,] firstEarthCrystalEnvironment = (int[,,])initialEnvironment.Clone();
            int[,,] secondEarthCrystalEnvironment = (int[,,])initialEnvironment.Clone();
            int[,,] firstWindCrystalEnvironment = (int[,,])initialEnvironment.Clone();
            int[,,] secondWindCrystalEnvironment = (int[,,])initialEnvironment.Clone();

            Random rand = new Random();

            GrowEarthCrystal(firstEarthCrystalEnvironment, waterSize, woodSize, windSize, rand);
            GrowEarthCrystal(secondEarthCrystalEnvironment, waterSize, woodSize, windSize, rand);
            GrowWindCrystal(firstWindCrystalEnvironment, waterSize, woodSize, windSize, rand);
            GrowWindCrystal(secondWindCrystalEnvironment, waterSize, woodSize, windSize, rand);

            for (int wind = 1; wind < windSize - 1; wind++)
            {
                for (int wood = 1; wood < woodSize - 1; wood++)
                {
                    for (int water = 1; water < waterSize - 1; water++)
                    {
                        initialEnvironment[water, wood, wind] += firstEarthCrystalEnvironment[water, wood, wind];
                        initialEnvironment[water, wood, wind] += secondEarthCrystalEnvironment[water, wood, wind];
                        initialEnvironment[water, wood, wind] += firstWindCrystalEnvironment[water, wood, wind];
                        initialEnvironment[water, wood, wind] += secondWindCrystalEnvironment[water, wood, wind];
                    }
                }
            }

            return initialEnvironment;
        }

        private static int[,,] CreateInitialEnvironment(int waterSize, int woodSize, int windSize)
        {
            int[,,] environment = new int[waterSize, woodSize, windSize];
            for (int wind = 0; wind < windSize; wind++)
            {
                for (int wood = 0; wood < woodSize; wood++)
                {
                    double fractionAlongWorld = wind / (double)(windSize - 1);
                    double rescaledFraction = (4 * fractionAlongWorld - 2);
                    int elementValue = MathHelpers.Round(rescaledFraction);
                    environment[0, wood, wind] = elementValue;
                    environment[waterSize - 1, wood, wind] = elementValue;
                }
            }

            for (int wind = 0; wind < windSize; wind++)
            {
                for (int water = 0; water < waterSize; water++)
                {
                    double fractionAlongWorld = wind / (double)(windSize - 1);
                    double rescaledFraction = (4 * fractionAlongWorld - 2);
                    int elementValue = MathHelpers.Round(rescaledFraction);
                    environment[water, 0, wind] = elementValue;
                    environment[water, woodSize - 1, wind] = elementValue;
                }
            }

            for (int wood = 0; wood < woodSize; wood++)
            {
                for (int water = 0; water < waterSize; water++)
                {
                    environment[water, wood, 0] = -2;
                    environment[water, wood, windSize - 1] = 2;
                }
            }

            return environment;
        }

        private static void GrowEarthCrystal(int[,,] earthCrystalEnvironment, int waterSize, int woodSize, int windSize, Random rand)
        {
            int maxEarth = (waterSize - 2) * (woodSize - 2);

            Dictionary<int, int> earthLimits = new Dictionary<int, int>();

            int totalEarth = 0;

            for (int i = 1; i < windSize - 1; i++)
            {
                double fractionAlongWorld = i / (double)(windSize - 1);
                double spinalValue = SpinalOperations.WorldSpinalValue(fractionAlongWorld, 1.27);
                int earthLimit = (int)(maxEarth * spinalValue);
                totalEarth += earthLimit;
                earthLimits.Add(i, earthLimit);
            }

            List<ElementPoint> growthLocations = new List<ElementPoint>();

            for (int wind = 1; wind < (windSize - 1) / 2; wind++)
            {
                for (int wood = 1; wood < woodSize - 1; wood++)
                {
                    for (int water = 1; water < waterSize - 1; water++)
                    {
                        if (EnvironmentMakerOperations.HasNeighborOfValueOrLess(water, wood, wind, -1, earthCrystalEnvironment))
                        {
                            growthLocations.Add(new ElementPoint() { waterPosition = water, woodPosition = wood, windPosition = wind });
                        }
                    }
                }
            }

            while (totalEarth > 0)
            {
                int numberOfPossibleLocations = growthLocations.Count;
                int growthChoice = rand.Next(0, numberOfPossibleLocations);
                ElementPoint growthPoint = growthLocations[growthChoice];

                growthLocations.Remove(growthPoint);
                earthCrystalEnvironment.SetPoint(growthPoint, -1);
                earthLimits[growthPoint.windPosition]--;

                if (earthLimits[growthPoint.windPosition] == 0)
                {
                    growthLocations.RemoveAll((p) => p.windPosition == growthPoint.windPosition);
                }

                totalEarth--;

                int upperLayer = growthPoint.windPosition + 1;
                if (earthLimits.ContainsKey(upperLayer) && earthLimits[upperLayer] != 0)
                {
                    ElementPoint topPoint = growthPoint.CloneWithOffset(0, 1, 0);
                    if (!growthLocations.Contains(topPoint) && earthCrystalEnvironment.WithinBounds(topPoint) && earthCrystalEnvironment.AtPoint(topPoint) == 0)
                    {
                        growthLocations.Add(topPoint);
                    }
                }
                if (earthLimits[growthPoint.windPosition] != 0)
                {
                    ElementPoint rightPoint = growthPoint.CloneWithOffset(1, 0, 0);
                    if (!growthLocations.Contains(rightPoint) && earthCrystalEnvironment.WithinBounds(rightPoint) && earthCrystalEnvironment.AtPoint(rightPoint) == 0)
                    {
                        growthLocations.Add(rightPoint);
                    }

                    ElementPoint forwardPoint = growthPoint.CloneWithOffset(0, 0, 1);
                    if (!growthLocations.Contains(forwardPoint) && earthCrystalEnvironment.WithinBounds(forwardPoint) && earthCrystalEnvironment.AtPoint(forwardPoint) == 0)
                    {
                        growthLocations.Add(forwardPoint);
                    }

                    ElementPoint leftPoint = growthPoint.CloneWithOffset(-1, 0, 0);
                    if (!growthLocations.Contains(leftPoint) && earthCrystalEnvironment.WithinBounds(leftPoint) && earthCrystalEnvironment.AtPoint(leftPoint) == 0)
                    {
                        growthLocations.Add(leftPoint);
                    }

                    ElementPoint backPoint = growthPoint.CloneWithOffset(0, 0, -1);
                    if (!growthLocations.Contains(backPoint) && earthCrystalEnvironment.WithinBounds(backPoint) && earthCrystalEnvironment.AtPoint(backPoint) == 0)
                    {
                        growthLocations.Add(backPoint);
                    }
                }
                int lowerLayer = growthPoint.windPosition - 1;
                if (earthLimits.ContainsKey(lowerLayer) && earthLimits[lowerLayer] != 0)
                {
                    ElementPoint bottomPoint = growthPoint.CloneWithOffset(0, -1, 0);
                    if (!growthLocations.Contains(bottomPoint) && earthCrystalEnvironment.WithinBounds(bottomPoint) && earthCrystalEnvironment.AtPoint(bottomPoint) == 0)
                    {
                        growthLocations.Add(bottomPoint);
                    }
                }
            }
        }

        private static void GrowWindCrystal(int[,,] windCrystalEnvironment, int waterSize, int woodSize, int windSize, Random rand)
        {
            int maxWind = (waterSize - 2) * (woodSize - 2);

            Dictionary<int, int> windLimits = new Dictionary<int, int>();

            int totalWind = 0;

            for (int i = 1; i < windSize - 1; i++)
            {
                double fractionAlongWorld = (windSize - 1 - i) / (double)(windSize - 1);
                double spinalValue = SpinalOperations.WorldSpinalValue(fractionAlongWorld, 1.27);
                int windLimit = (int)(maxWind * spinalValue);
                totalWind += windLimit;
                windLimits.Add(i, windLimit);
            }

            List<ElementPoint> growthLocations = new List<ElementPoint>();

            for (int wind = (windSize - 1) / 2; wind < windSize - 1; wind++)
            {
                for (int wood = 1; wood < woodSize - 1; wood++)
                {
                    for (int water = 1; water < waterSize - 1; water++)
                    {
                        if (EnvironmentMakerOperations.HasNeighborOfValueOrMore(water, wood, wind, 1, windCrystalEnvironment))
                        {
                            growthLocations.Add(new ElementPoint() { waterPosition = water, woodPosition = wood, windPosition = wind });
                        }
                    }
                }
            }

            while (totalWind > 0)
            {
                int numberOfPossibleLocations = growthLocations.Count;
                int growthChoice = rand.Next(0, numberOfPossibleLocations);
                ElementPoint growthPoint = growthLocations[growthChoice];

                growthLocations.Remove(growthPoint);
                windCrystalEnvironment.SetPoint(growthPoint, 1);
                windLimits[growthPoint.windPosition]--;

                if (windLimits[growthPoint.windPosition] == 0)
                {
                    growthLocations.RemoveAll((p) => p.windPosition == growthPoint.windPosition);
                }

                totalWind--;

                int upperLayer = growthPoint.windPosition + 1;
                if (windLimits.ContainsKey(upperLayer) && windLimits[upperLayer] != 0)
                {
                    ElementPoint topPoint = growthPoint.CloneWithOffset(0, 1, 0);
                    if (!growthLocations.Contains(topPoint) && windCrystalEnvironment.WithinBounds(topPoint) && windCrystalEnvironment.AtPoint(topPoint) == 0)
                    {
                        growthLocations.Add(topPoint);
                    }
                }
                if (windLimits[growthPoint.windPosition] != 0)
                {
                    ElementPoint rightPoint = growthPoint.CloneWithOffset(1, 0, 0);
                    if (!growthLocations.Contains(rightPoint) && windCrystalEnvironment.WithinBounds(rightPoint) && windCrystalEnvironment.AtPoint(rightPoint) == 0)
                    {
                        growthLocations.Add(rightPoint);
                    }

                    ElementPoint forwardPoint = growthPoint.CloneWithOffset(0, 0, 1);
                    if (!growthLocations.Contains(forwardPoint) && windCrystalEnvironment.WithinBounds(forwardPoint) && windCrystalEnvironment.AtPoint(forwardPoint) == 0)
                    {
                        growthLocations.Add(forwardPoint);
                    }

                    ElementPoint leftPoint = growthPoint.CloneWithOffset(-1, 0, 0);
                    if (!growthLocations.Contains(leftPoint) && windCrystalEnvironment.WithinBounds(leftPoint) && windCrystalEnvironment.AtPoint(leftPoint) == 0)
                    {
                        growthLocations.Add(leftPoint);
                    }

                    ElementPoint backPoint = growthPoint.CloneWithOffset(0, 0, -1);
                    if (!growthLocations.Contains(backPoint) && windCrystalEnvironment.WithinBounds(backPoint) && windCrystalEnvironment.AtPoint(backPoint) == 0)
                    {
                        growthLocations.Add(backPoint);
                    }
                }
                int lowerLayer = growthPoint.windPosition - 1;
                if (windLimits.ContainsKey(lowerLayer) && windLimits[lowerLayer] != 0)
                {
                    ElementPoint bottomPoint = growthPoint.CloneWithOffset(0, -1, 0);
                    if (!growthLocations.Contains(bottomPoint) && windCrystalEnvironment.WithinBounds(bottomPoint) && windCrystalEnvironment.AtPoint(bottomPoint) == 0)
                    {
                        growthLocations.Add(bottomPoint);
                    }
                }
            }
        }
    }
}