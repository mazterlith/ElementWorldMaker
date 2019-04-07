using ElementWorldMaker.Existence.EnvironmentMaker;
using System;
using System.Collections.Generic;

namespace ElementWorldMaker.Existence.Ether
{
    public static class EtherMaker
    {
        public static int[,,] Make(int[,,] etherResistanceEnvironment)
        {
            int waterMax = etherResistanceEnvironment.GetLength(0);
            int woodMax = etherResistanceEnvironment.GetLength(1);
            int windMax = etherResistanceEnvironment.GetLength(2);

            int[,,] etherEnvironment = new int[waterMax, woodMax, windMax];

            float proportion = .5f;

            int solidWaterEther = (int)(proportion * waterMax);
            int solidWoodEther = (int)(proportion * woodMax);
            int solidWindEther = (int)(proportion * windMax);

            int[,] solidWaterLeft = new int[woodMax, windMax];
            int[,] holeWaterLeft = new int[woodMax, windMax];

            for (int windPosition = 0; windPosition < windMax; windPosition++)
            {
                for (int woodPosition = 0; woodPosition < woodMax; woodPosition++)
                {
                    solidWaterLeft[woodPosition, windPosition] = solidWaterEther;
                    holeWaterLeft[woodPosition, windPosition] = waterMax - solidWaterEther;
                }
            }

            int[,] solidWoodLeft = new int[waterMax, windMax];
            int[,] holeWoodLeft = new int[waterMax, windMax];

            for (int windPosition = 0; windPosition < windMax; windPosition++)
            {
                for (int waterPosition = 0; waterPosition < waterMax; waterPosition++)
                {
                    solidWoodLeft[waterPosition, windPosition] = solidWoodEther;
                    holeWoodLeft[waterPosition, windPosition] = woodMax - solidWoodEther;
                }
            }

            int[,] solidWindLeft = new int[waterMax, woodMax];
            int[,] holeWindLeft = new int[waterMax, woodMax];

            for (int woodPosition = 0; woodPosition < woodMax; woodPosition++)
            {
                for (int waterPosition = 0; waterPosition < waterMax; waterPosition++)
                {
                    solidWindLeft[waterPosition, woodPosition] = solidWindEther;
                    holeWindLeft[waterPosition, woodPosition] = windMax - solidWindEther;
                }
            }

            Random rand = new Random();

            for (int windPosition = 0; windPosition < windMax; windPosition++)
            {
                for (int woodPosition = 0; woodPosition < woodMax; woodPosition++)
                {
                    for (int waterPosition = 0; waterPosition < waterMax; waterPosition++)
                    {
                        bool? choseSolid = null;

                        if (solidWaterLeft[woodPosition, windPosition] == 0)
                        {
                            choseSolid = false;
                        }
                        else if (!choseSolid.HasValue && holeWaterLeft[woodPosition, windPosition] == 0)
                        {
                            choseSolid = true;
                        }
                        else if (!choseSolid.HasValue && solidWoodLeft[waterPosition, windPosition] == 0)
                        {
                            choseSolid = false;
                        }
                        else if (!choseSolid.HasValue && holeWoodLeft[waterPosition, windPosition] == 0)
                        {
                            choseSolid = true;
                        }
                        else if (!choseSolid.HasValue && solidWindLeft[waterPosition, woodPosition] == 0)
                        {
                            choseSolid = false;
                        }
                        else if (!choseSolid.HasValue && holeWindLeft[waterPosition, woodPosition] == 0)
                        {
                            choseSolid = true;
                        }
                        else if (!choseSolid.HasValue)
                        {
                            int solidsLeft = solidWaterLeft[woodPosition, windPosition];
                            int holesLeft = holeWaterLeft[woodPosition, windPosition];

                            int choice = rand.Next(solidsLeft + holesLeft) + 1 - solidsLeft;
                            if (choice > 0)
                            {
                                choseSolid = false;
                            }
                            else
                            {
                                choseSolid = true;
                            }
                        }

                        if (choseSolid.Value)
                        {
                            // solid was chosen
                            solidWaterLeft[woodPosition, windPosition] -= 1;
                            solidWoodLeft[waterPosition, windPosition] -= 1;
                            solidWindLeft[waterPosition, woodPosition] -= 1;
                            etherEnvironment[waterPosition, woodPosition, windPosition] = 1;
                        }
                        else
                        {
                            // hole was chosen
                            holeWaterLeft[woodPosition, windPosition] -= 1;
                            holeWoodLeft[waterPosition, windPosition] -= 1;
                            holeWindLeft[waterPosition, woodPosition] -= 1;
                            etherEnvironment[waterPosition, woodPosition, windPosition] = 0;
                        }
                    }
                }
            }

            //ElementPoint[] sources = new ElementPoint[]
            //{
            //    new ElementPoint { waterPosition = waterMax - 2, woodPosition = woodMax - 2, windPosition = windMax - 2 }, //waterWoodWindCorner
            //    new ElementPoint { waterPosition = waterMax - 2, woodPosition = woodMax - 2, windPosition = 1 }, //waterWoodEarthCorner
            //    new ElementPoint { waterPosition = waterMax - 2, woodPosition = 1, windPosition = windMax - 2 }, //waterMetalWindCorner
            //    new ElementPoint { waterPosition = waterMax - 2, woodPosition = 1, windPosition = 1 }, //waterMetalEarthCorner
            //    new ElementPoint { waterPosition = 1, woodPosition = woodMax - 2, windPosition = windMax - 2 }, //fireWoodWindCorner
            //    new ElementPoint { waterPosition = 1, woodPosition = woodMax - 2, windPosition = 1 }, //fireWoodEarthCorner
            //    new ElementPoint { waterPosition = 1, woodPosition = 1, windPosition = windMax - 2 }, //fireMetalWindCorner
            //    new ElementPoint { waterPosition = 1, woodPosition = 1, windPosition = 1 }, //fireMetalEarthCorner
            //    new ElementPoint { waterPosition = waterMax / 2 + 1, woodPosition = woodMax / 2 + 1, windPosition = windMax / 2 + 1}
            //};

            //foreach (ElementPoint etherSource in sources)
            //{
            //    GrowStream(etherResistanceEnvironment, etherEnvironment, etherSource);
            //}

            return etherEnvironment;
        }

        private static void GrowStream(int[,,] etherResistanceEnvironment, int[,,] streamEnvironment, ElementPoint corner)
        {
            Random rand = new Random();

            int streamLength = 20 * streamEnvironment.GetLength(0);

            ElementPoint newGrowth = corner;

            HashSet<ElementPoint> streamPath = new HashSet<ElementPoint>() { newGrowth };
            HashSet<ElementPoint> validBuds = new HashSet<ElementPoint>() { newGrowth };
            streamEnvironment.SetPoint(newGrowth, streamEnvironment.AtPoint(newGrowth) + 1);

            HashSet<EtherStreamGrowthDirection> streamGrowth = new HashSet<EtherStreamGrowthDirection>();

            ElementPoint growthBud = corner;

            while (streamLength > 0)
            {
                AppendToStreamGrowth(etherResistanceEnvironment, streamPath, streamGrowth, growthBud, out int totalWeights);

                if (streamGrowth.Count == 0)
                {
                    break;
                }

                if (totalWeights == 0)
                {
                    validBuds.Remove(growthBud);
                    streamGrowth.RemoveWhere(direction => direction.To == growthBud);
                    growthBud = ChooseNewGrowthBud(validBuds, rand);
                    continue;
                }

                EtherStreamGrowthDirection growthChoice = null;

                int choice = rand.Next(totalWeights) + 1;
                foreach (EtherStreamGrowthDirection growth in streamGrowth)
                {
                    int growthWeight = growth.Weight;
                    choice -= growthWeight;
                    if (choice <= 0)
                    {
                        growthChoice = growth;
                        break;
                    }
                }

                newGrowth = growthChoice.To;

                streamPath.Add(newGrowth);
                validBuds.Add(newGrowth);

                streamEnvironment.SetPoint(newGrowth, streamEnvironment.AtPoint(newGrowth) + 1);
                streamGrowth.Remove(growthChoice);
                streamGrowth.RemoveWhere(direction => direction.To == newGrowth);

                growthBud = ChooseNewGrowthBud(validBuds, rand);

                streamLength--;
            }
        }
        
        private static ElementPoint ChooseNewGrowthBud(HashSet<ElementPoint> streamPath, Random rand)
        {
            int choice = rand.Next(streamPath.Count) + 1;
            ElementPoint growthBud = null;
            foreach (ElementPoint streamPoint in streamPath)
            {
                if (--choice == 0)
                {
                    growthBud = streamPoint;
                    break;
                }
            }
            return growthBud;
        }

        private static void AppendToStreamGrowth(int[,,] etherResistanceEnvironment, HashSet<ElementPoint> streamPath, HashSet<EtherStreamGrowthDirection> streamGrowth, ElementPoint growthPoint, out int totalWeights)
        {
            totalWeights = 0;

            ElementPoint[] directionalPoints = new ElementPoint[]
            {
                growthPoint.CloneWithOffset(1, 0, 0),
                growthPoint.CloneWithOffset(0, 1, 0),
                growthPoint.CloneWithOffset(0, 0, 1),
                growthPoint.CloneWithOffset(-1, 0, 0),
                growthPoint.CloneWithOffset(0, -1, 0),
                growthPoint.CloneWithOffset(0, 0, -1)
            };

            foreach (ElementPoint directionalPoint in directionalPoints)
            {
                if (!streamPath.Contains(directionalPoint) && etherResistanceEnvironment.WithinBounds(directionalPoint))
                {
                    int directionalWeight = etherResistanceEnvironment.AtPoint(growthPoint) - etherResistanceEnvironment.AtPoint(directionalPoint) + 1;
                    if (directionalWeight > 0)
                    {
                        streamGrowth.Add(new EtherStreamGrowthDirection() { From = growthPoint, To = directionalPoint, Weight = directionalWeight });
                        totalWeights += directionalWeight;
                    }
                }
            }
        }
    }
}