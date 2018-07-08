using ElementWorldMaker.Existence.EnvironmentMaker;
using System;
using System.Collections.Generic;

namespace ElementWorldMaker.Existence.WaterFire
{
    public static class WaterFireMaker
    {
        public static int[,,] Make(int waterSize, int woodSize, int windSize)
        {
            int[,,] initialEnvironment = CreateInitialEnvironment(waterSize, woodSize, windSize);
            int[,,] firstFireCrystalEnvironment = (int[,,])initialEnvironment.Clone();
            int[,,] secondFireCrystalEnvironment = (int[,,])initialEnvironment.Clone();
            int[,,] firstWaterCrystalEnvironment = (int[,,])initialEnvironment.Clone();
            int[,,] secondWaterCrystalEnvironment = (int[,,])initialEnvironment.Clone();

            Random rand = new Random();

            GrowFireCrystal(firstFireCrystalEnvironment, waterSize, woodSize, windSize, rand);
            GrowFireCrystal(secondFireCrystalEnvironment, waterSize, woodSize, windSize, rand);
            GrowWaterCrystal(firstWaterCrystalEnvironment, waterSize, woodSize, windSize, rand);
            GrowWaterCrystal(secondWaterCrystalEnvironment, waterSize, woodSize, windSize, rand);

            for (int wind = 1; wind < windSize - 1; wind++)
            {
                for (int wood = 1; wood < woodSize - 1; wood++)
                {
                    for (int water = 1; water < waterSize - 1; water++)
                    {
                        initialEnvironment[water, wood, wind] += firstFireCrystalEnvironment[water, wood, wind];
                        initialEnvironment[water, wood, wind] += secondFireCrystalEnvironment[water, wood, wind];
                        initialEnvironment[water, wood, wind] += firstWaterCrystalEnvironment[water, wood, wind];
                        initialEnvironment[water, wood, wind] += secondWaterCrystalEnvironment[water, wood, wind];
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
                    environment[0, wood, wind] = -2;
                    environment[waterSize - 1, wood, wind] = 2;
                }
            }

            for (int wind = 0; wind < windSize; wind++)
            {
                for (int water = 0; water < waterSize; water++)
                {
                    double fractionAlongWorld = water / (double)(windSize - 1);
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
                    double fractionAlongWorld = water / (double)(windSize - 1);
                    double rescaledFraction = (4 * fractionAlongWorld - 2);
                    int elementValue = MathHelpers.Round(rescaledFraction);
                    environment[water, wood, 0] = elementValue;
                    environment[water, wood, windSize - 1] = elementValue;
                }
            }

            return environment;
        }

        private static void GrowFireCrystal(int[,,] fireCrystalEnvironment, int waterSize, int woodSize, int windSize, Random rand)
        {
            int maxFire =  (woodSize - 2) * (windSize - 2);

            Dictionary<int, int> fireLimits = new Dictionary<int, int>();

            int totalFire = 0;

            for (int i = 1; i < waterSize - 1; i++)
            {
                double fractionAlongWorld = i / (double)(waterSize - 1);
                double spinalValue = SpinalOperations.WorldSpinalValue(fractionAlongWorld, 1.27);
                int fireLimit = (int)(maxFire * spinalValue);
                totalFire += fireLimit;
                fireLimits.Add(i, fireLimit);
            }

            List<ElementPoint> growthLocations = new List<ElementPoint>();

            for (int wind = 1; wind < windSize - 1; wind++)
            {
                for (int wood = 1; wood < woodSize - 1; wood++)
                {
                    for (int water = 1; water < (waterSize - 1) / 2; water++)
                    {
                        if (EnvironmentMakerOperations.HasNeighborOfValueOrLess(water, wood, wind, -1, fireCrystalEnvironment))
                        {
                            growthLocations.Add(new ElementPoint() { waterPosition = water, woodPosition = wood, windPosition = wind });
                        }
                    }
                }
            }

            while (totalFire > 0)
            {
                int numberOfPossibleLocations = growthLocations.Count;
                int growthChoice = rand.Next(0, numberOfPossibleLocations);
                ElementPoint growthPoint = growthLocations[growthChoice];

                growthLocations.Remove(growthPoint);
                fireCrystalEnvironment.SetPoint(growthPoint, -1);
                fireLimits[growthPoint.waterPosition]--;

                if (fireLimits[growthPoint.waterPosition] == 0)
                {
                    growthLocations.RemoveAll((p) => p.waterPosition == growthPoint.waterPosition);
                }

                totalFire--;

                int upperLayer = growthPoint.waterPosition + 1;
                if (fireLimits.ContainsKey(upperLayer) && fireLimits[upperLayer] != 0)
                {
                    ElementPoint topPoint = growthPoint.CloneWithOffset(1, 0, 0);
                    if (!growthLocations.Contains(topPoint) && fireCrystalEnvironment.WithinBounds(topPoint) && fireCrystalEnvironment.AtPoint(topPoint) == 0)
                    {
                        growthLocations.Add(topPoint);
                    }
                }
                if (fireLimits[growthPoint.waterPosition] != 0)
                {
                    ElementPoint rightPoint = growthPoint.CloneWithOffset(0, 1, 0);
                    if (!growthLocations.Contains(rightPoint) && fireCrystalEnvironment.WithinBounds(rightPoint) && fireCrystalEnvironment.AtPoint(rightPoint) == 0)
                    {
                        growthLocations.Add(rightPoint);
                    }

                    ElementPoint forwardPoint = growthPoint.CloneWithOffset(0, 0, 1);
                    if (!growthLocations.Contains(forwardPoint) && fireCrystalEnvironment.WithinBounds(forwardPoint) && fireCrystalEnvironment.AtPoint(forwardPoint) == 0)
                    {
                        growthLocations.Add(forwardPoint);
                    }

                    ElementPoint leftPoint = growthPoint.CloneWithOffset(0, -1, 0);
                    if (!growthLocations.Contains(leftPoint) && fireCrystalEnvironment.WithinBounds(leftPoint) && fireCrystalEnvironment.AtPoint(leftPoint) == 0)
                    {
                        growthLocations.Add(leftPoint);
                    }

                    ElementPoint backPoint = growthPoint.CloneWithOffset(0, 0, -1);
                    if (!growthLocations.Contains(backPoint) && fireCrystalEnvironment.WithinBounds(backPoint) && fireCrystalEnvironment.AtPoint(backPoint) == 0)
                    {
                        growthLocations.Add(backPoint);
                    }
                }
                int lowerLayer = growthPoint.waterPosition - 1;
                if (fireLimits.ContainsKey(lowerLayer) && fireLimits[lowerLayer] != 0)
                {
                    ElementPoint bottomPoint = growthPoint.CloneWithOffset(-1, 0, 0);
                    if (!growthLocations.Contains(bottomPoint) && fireCrystalEnvironment.WithinBounds(bottomPoint) && fireCrystalEnvironment.AtPoint(bottomPoint) == 0)
                    {
                        growthLocations.Add(bottomPoint);
                    }
                }
            }
        }

        private static void GrowWaterCrystal(int[,,] waterCrystalEnvironment, int waterSize, int woodSize, int windSize, Random rand)
        {
            int maxWater = (woodSize - 2) * (windSize - 2);

            Dictionary<int, int> waterLimits = new Dictionary<int, int>();

            int totalWater = 0;

            for (int i = 1; i < waterSize - 1; i++)
            {
                double fractionAlongWorld = (waterSize - 1 - i) / (double)(waterSize - 1);
                double spinalValue = SpinalOperations.WorldSpinalValue(fractionAlongWorld, 1.27);
                int waterLimit = (int)(maxWater * spinalValue);
                totalWater += waterLimit;
                waterLimits.Add(i, waterLimit);
            }

            List<ElementPoint> growthLocations = new List<ElementPoint>();

            for (int wind = 1; wind < windSize - 1; wind++)
            {
                for (int wood = 1; wood < woodSize - 1; wood++)
                {
                    for (int water = (waterSize - 1) / 2; water < waterSize - 1; water++)
                    {
                        if (EnvironmentMakerOperations.HasNeighborOfValueOrMore(water, wood, wind, 1, waterCrystalEnvironment))
                        {
                            growthLocations.Add(new ElementPoint() { waterPosition = water, woodPosition = wood, windPosition = wind });
                        }
                    }
                }
            }

            while (totalWater > 0)
            {
                int numberOfPossibleLocations = growthLocations.Count;
                int growthChoice = rand.Next(0, numberOfPossibleLocations);
                ElementPoint growthPoint = growthLocations[growthChoice];

                growthLocations.Remove(growthPoint);
                waterCrystalEnvironment.SetPoint(growthPoint, 1);
                waterLimits[growthPoint.waterPosition]--;

                if (waterLimits[growthPoint.waterPosition] == 0)
                {
                    growthLocations.RemoveAll((p) => p.waterPosition == growthPoint.waterPosition);
                }

                totalWater--;

                int upperLayer = growthPoint.waterPosition + 1;
                if (waterLimits.ContainsKey(upperLayer) && waterLimits[upperLayer] != 0)
                {
                    ElementPoint topPoint = growthPoint.CloneWithOffset(1, 0, 0);
                    if (!growthLocations.Contains(topPoint) && waterCrystalEnvironment.WithinBounds(topPoint) && waterCrystalEnvironment.AtPoint(topPoint) == 0)
                    {
                        growthLocations.Add(topPoint);
                    }
                }
                if (waterLimits[growthPoint.waterPosition] != 0)
                {
                    ElementPoint rightPoint = growthPoint.CloneWithOffset(0, 1, 0);
                    if (!growthLocations.Contains(rightPoint) && waterCrystalEnvironment.WithinBounds(rightPoint) && waterCrystalEnvironment.AtPoint(rightPoint) == 0)
                    {
                        growthLocations.Add(rightPoint);
                    }

                    ElementPoint forwardPoint = growthPoint.CloneWithOffset(0, 0, 1);
                    if (!growthLocations.Contains(forwardPoint) && waterCrystalEnvironment.WithinBounds(forwardPoint) && waterCrystalEnvironment.AtPoint(forwardPoint) == 0)
                    {
                        growthLocations.Add(forwardPoint);
                    }

                    ElementPoint leftPoint = growthPoint.CloneWithOffset(0, -1, 0);
                    if (!growthLocations.Contains(leftPoint) && waterCrystalEnvironment.WithinBounds(leftPoint) && waterCrystalEnvironment.AtPoint(leftPoint) == 0)
                    {
                        growthLocations.Add(leftPoint);
                    }

                    ElementPoint backPoint = growthPoint.CloneWithOffset(0, 0, -1);
                    if (!growthLocations.Contains(backPoint) && waterCrystalEnvironment.WithinBounds(backPoint) && waterCrystalEnvironment.AtPoint(backPoint) == 0)
                    {
                        growthLocations.Add(backPoint);
                    }
                }
                int lowerLayer = growthPoint.waterPosition - 1;
                if (waterLimits.ContainsKey(lowerLayer) && waterLimits[lowerLayer] != 0)
                {
                    ElementPoint bottomPoint = growthPoint.CloneWithOffset(-1, 0, 0);
                    if (!growthLocations.Contains(bottomPoint) && waterCrystalEnvironment.WithinBounds(bottomPoint) && waterCrystalEnvironment.AtPoint(bottomPoint) == 0)
                    {
                        growthLocations.Add(bottomPoint);
                    }
                }
            }
        }
    }
}