using ElementWorldMaker.Existence.EnvironmentMaker;
using System;
using System.Collections.Generic;

namespace ElementWorldMaker.Existence.WoodMetal
{
    public static class WoodMetalMaker
    {
        public static int[,,] Make(int waterSize, int woodSize, int windSize)
        {
            int[,,] initialEnvironment = CreateInitialEnvironment(waterSize, woodSize, windSize);
            int[,,] firstMetalCrystalEnvironment = (int[,,])initialEnvironment.Clone();
            int[,,] secondMetalCrystalEnvironment = (int[,,])initialEnvironment.Clone();
            int[,,] firstWoodCrystalEnvironment = (int[,,])initialEnvironment.Clone();
            int[,,] secondWoodCrystalEnvironment = (int[,,])initialEnvironment.Clone();

            Random rand = new Random();

            GrowMetalCrystal(firstMetalCrystalEnvironment, waterSize, woodSize, windSize, rand);
            GrowMetalCrystal(secondMetalCrystalEnvironment, waterSize, woodSize, windSize, rand);
            GrowWoodCrystal(firstWoodCrystalEnvironment, waterSize, woodSize, windSize, rand);
            GrowWoodCrystal(secondWoodCrystalEnvironment, waterSize, woodSize, windSize, rand);

            for (int wind = 1; wind < windSize - 1; wind++)
            {
                for (int wood = 1; wood < woodSize - 1; wood++)
                {
                    for (int water = 1; water < waterSize - 1; water++)
                    {
                        initialEnvironment[water, wood, wind] += firstMetalCrystalEnvironment[water, wood, wind];
                        initialEnvironment[water, wood, wind] += secondMetalCrystalEnvironment[water, wood, wind];
                        initialEnvironment[water, wood, wind] += firstWoodCrystalEnvironment[water, wood, wind];
                        initialEnvironment[water, wood, wind] += secondWoodCrystalEnvironment[water, wood, wind];
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
                    double fractionAlongWorld = wood / (double)(windSize - 1);
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
                    environment[water, 0, wind] = -2;
                    environment[water, woodSize - 1, wind] = 2;
                }
            }

            for (int wood = 0; wood < woodSize; wood++)
            {
                for (int water = 0; water < waterSize; water++)
                {
                    double fractionAlongWorld = wood / (double)(windSize - 1);
                    double rescaledFraction = (4 * fractionAlongWorld - 2);
                    int elementValue = MathHelpers.Round(rescaledFraction);
                    environment[water, wood, 0] = elementValue;
                    environment[water, wood, windSize - 1] = elementValue;
                }
            }

            return environment;
        }

        private static void GrowMetalCrystal(int[,,] metalCrystalEnvironment, int waterSize, int woodSize, int windSize, Random rand)
        {
            int maxMetal = (waterSize - 2) * (windSize - 2);

            Dictionary<int, int> metalLimits = new Dictionary<int, int>();

            int totalMetal = 0;

            for (int i = 1; i < woodSize - 1; i++)
            {
                double fractionAlongWorld = i / (double)(woodSize - 1);
                double spinalValue = SpinalOperations.WorldSpinalValue(fractionAlongWorld, 1.27);
                int metalLimit = (int)(maxMetal * spinalValue);
                totalMetal += metalLimit;
                metalLimits.Add(i, metalLimit);
            }

            List<ElementPoint> growthLocations = new List<ElementPoint>();

            for (int wind = 1; wind < windSize - 1; wind++)
            {
                for (int wood = 1; wood < (woodSize - 1) / 2; wood++)
                {
                    for (int water = 1; water < waterSize - 1; water++)
                    {
                        if (EnvironmentMakerOperations.HasNeighborOfValueOrLess(water, wood, wind, -1, metalCrystalEnvironment))
                        {
                            growthLocations.Add(new ElementPoint() { waterPosition = water, woodPosition = wood, windPosition = wind });
                        }
                    }
                }
            }

            while (totalMetal > 0)
            {
                int numberOfPossibleLocations = growthLocations.Count;
                int growthChoice = rand.Next(0, numberOfPossibleLocations);
                ElementPoint growthPoint = growthLocations[growthChoice];

                growthLocations.Remove(growthPoint);
                metalCrystalEnvironment.SetPoint(growthPoint, -1);
                metalLimits[growthPoint.woodPosition]--;

                if (metalLimits[growthPoint.woodPosition] == 0)
                {
                    growthLocations.RemoveAll((p) => p.woodPosition == growthPoint.woodPosition);
                }

                totalMetal--;

                int upperLayer = growthPoint.woodPosition + 1;
                if (metalLimits.ContainsKey(upperLayer) && metalLimits[upperLayer] != 0)
                {
                    ElementPoint topPoint = growthPoint.CloneWithOffset(0, 0, 1);
                    if (!growthLocations.Contains(topPoint) && metalCrystalEnvironment.WithinBounds(topPoint) && metalCrystalEnvironment.AtPoint(topPoint) == 0)
                    {
                        growthLocations.Add(topPoint);
                    }
                }
                if (metalLimits[growthPoint.woodPosition] != 0)
                {
                    ElementPoint rightPoint = growthPoint.CloneWithOffset(0, 1, 0);
                    if (!growthLocations.Contains(rightPoint) && metalCrystalEnvironment.WithinBounds(rightPoint) && metalCrystalEnvironment.AtPoint(rightPoint) == 0)
                    {
                        growthLocations.Add(rightPoint);
                    }

                    ElementPoint forwardPoint = growthPoint.CloneWithOffset(1, 0, 0);
                    if (!growthLocations.Contains(forwardPoint) && metalCrystalEnvironment.WithinBounds(forwardPoint) && metalCrystalEnvironment.AtPoint(forwardPoint) == 0)
                    {
                        growthLocations.Add(forwardPoint);
                    }

                    ElementPoint leftPoint = growthPoint.CloneWithOffset(0, -1, 0);
                    if (!growthLocations.Contains(leftPoint) && metalCrystalEnvironment.WithinBounds(leftPoint) && metalCrystalEnvironment.AtPoint(leftPoint) == 0)
                    {
                        growthLocations.Add(leftPoint);
                    }

                    ElementPoint backPoint = growthPoint.CloneWithOffset(-1, 0, 0);
                    if (!growthLocations.Contains(backPoint) && metalCrystalEnvironment.WithinBounds(backPoint) && metalCrystalEnvironment.AtPoint(backPoint) == 0)
                    {
                        growthLocations.Add(backPoint);
                    }
                }
                int lowerLayer = growthPoint.woodPosition - 1;
                if (metalLimits.ContainsKey(lowerLayer) && metalLimits[lowerLayer] != 0)
                {
                    ElementPoint bottomPoint = growthPoint.CloneWithOffset(0, 0, -1);
                    if (!growthLocations.Contains(bottomPoint) && metalCrystalEnvironment.WithinBounds(bottomPoint) && metalCrystalEnvironment.AtPoint(bottomPoint) == 0)
                    {
                        growthLocations.Add(bottomPoint);
                    }
                }
            }
        }
        
        private static void GrowWoodCrystal(int[,,] woodCrystalEnvironment, int waterSize, int woodSize, int windSize, Random rand)
        {
            int maxWood = (waterSize - 2) * (windSize - 2);

            Dictionary<int, int> woodLimits = new Dictionary<int, int>();

            int totalWood = 0;

            for (int i = 1; i < woodSize - 1; i++)
            {
                double fractionAlongWorld = (woodSize - 1 - i) / (double)(woodSize - 1);
                double spinalValue = SpinalOperations.WorldSpinalValue(fractionAlongWorld, 1.27);
                int woodLimit = (int)(maxWood * spinalValue);
                totalWood += woodLimit;
                woodLimits.Add(i, woodLimit);
            }

            List<ElementPoint> growthLocations = new List<ElementPoint>();

            for (int wind = 1; wind < windSize - 1; wind++)
            {
                for (int wood = (woodSize - 1) / 2; wood < woodSize - 1; wood++)
                {
                    for (int water = 1; water < waterSize - 1; water++)
                    {
                        if (EnvironmentMakerOperations.HasNeighborOfValueOrMore(water, wood, wind, 1, woodCrystalEnvironment))
                        {
                            growthLocations.Add(new ElementPoint() { waterPosition = water, woodPosition = wood, windPosition = wind });
                        }
                    }
                }
            }

            while (totalWood > 0)
            {
                int numberOfPossibleLocations = growthLocations.Count;
                int growthChoice = rand.Next(0, numberOfPossibleLocations);
                ElementPoint growthPoint = growthLocations[growthChoice];

                growthLocations.Remove(growthPoint);
                woodCrystalEnvironment.SetPoint(growthPoint, 1);
                woodLimits[growthPoint.woodPosition]--;

                if (woodLimits[growthPoint.woodPosition] == 0)
                {
                    growthLocations.RemoveAll((p) => p.woodPosition == growthPoint.woodPosition);
                }

                totalWood--;

                int upperLayer = growthPoint.woodPosition + 1;
                if (woodLimits.ContainsKey(upperLayer) && woodLimits[upperLayer] != 0)
                {
                    ElementPoint topPoint = growthPoint.CloneWithOffset(0, 0, 1);
                    if (!growthLocations.Contains(topPoint) && woodCrystalEnvironment.WithinBounds(topPoint) && woodCrystalEnvironment.AtPoint(topPoint) == 0)
                    {
                        growthLocations.Add(topPoint);
                    }
                }
                if (woodLimits[growthPoint.woodPosition] != 0)
                {
                    ElementPoint rightPoint = growthPoint.CloneWithOffset(0, 1, 0);
                    if (!growthLocations.Contains(rightPoint) && woodCrystalEnvironment.WithinBounds(rightPoint) && woodCrystalEnvironment.AtPoint(rightPoint) == 0)
                    {
                        growthLocations.Add(rightPoint);
                    }

                    ElementPoint forwardPoint = growthPoint.CloneWithOffset(1, 0, 0);
                    if (!growthLocations.Contains(forwardPoint) && woodCrystalEnvironment.WithinBounds(forwardPoint) && woodCrystalEnvironment.AtPoint(forwardPoint) == 0)
                    {
                        growthLocations.Add(forwardPoint);
                    }

                    ElementPoint leftPoint = growthPoint.CloneWithOffset(0, -1, 0);
                    if (!growthLocations.Contains(leftPoint) && woodCrystalEnvironment.WithinBounds(leftPoint) && woodCrystalEnvironment.AtPoint(leftPoint) == 0)
                    {
                        growthLocations.Add(leftPoint);
                    }

                    ElementPoint backPoint = growthPoint.CloneWithOffset(-1, 0, 0);
                    if (!growthLocations.Contains(backPoint) && woodCrystalEnvironment.WithinBounds(backPoint) && woodCrystalEnvironment.AtPoint(backPoint) == 0)
                    {
                        growthLocations.Add(backPoint);
                    }
                }
                int lowerLayer = growthPoint.woodPosition - 1;
                if (woodLimits.ContainsKey(lowerLayer) && woodLimits[lowerLayer] != 0)
                {
                    ElementPoint bottomPoint = growthPoint.CloneWithOffset(0, 0, -1);
                    if (!growthLocations.Contains(bottomPoint) && woodCrystalEnvironment.WithinBounds(bottomPoint) && woodCrystalEnvironment.AtPoint(bottomPoint) == 0)
                    {
                        growthLocations.Add(bottomPoint);
                    }
                }
            }
        }
    }
}