using System;
using System.Collections.Generic;

namespace ElementWorldMaker.Existence
{
    public class WindEarthEnvironmentMaker
    {
        private int _waterSize;
        private int _woodSize;
        private int _windSize;

        Random _rand = new Random();

        private WindEarthTerrainType[,,] _windEarthEnvironment;

        private int _totalNumberOfWindEarthSlots;

        bool _evenSize;
        int _windMiddle;

        public WindEarthEnvironmentMaker(int waterSize, int woodSize, int windSize)
        {
            _waterSize = waterSize;
            _woodSize = woodSize;
            _windSize = windSize;

            _windEarthEnvironment = new WindEarthTerrainType[_waterSize, _woodSize, _windSize];
            _totalNumberOfWindEarthSlots = (_waterSize - 2) * (_woodSize - 2) * 2;

            _evenSize = _windSize % 2 == 0;
            _windMiddle = _windSize / 2;
        }

        public WindEarthTerrainType[,,] MakeWindEarthEnvironment()
        {
            PopulateBoundaries();

            //TODO: Plan is to make # of earth slots decrease linearly until map is 50/50 earth/wind from 2 * positions to 25% of that.
            //      Then make # of earth slots decrease linearly the rest of the way to 0. This will hopefully make a continuous gradient of earth
            //      with a piecewise slope.
            //      Total slots is twice the number of positions so high density locations can exist. There is no high density neutral terrain.
            //      Earth slots go from 100% to 25% to 0%.
            //      Neutral slots go from 0% to 50% to 0%.
            //      Wind slots go from 0% to 25% to 100%.

            PopulateLowerHalfWithEarth();
            PopulateUpperHalfWithWind();

            PopulateMiddle();

            PopulateLowerHalfWithWind();
            PopulateUpperHalfWithEarth();

            return _windEarthEnvironment;
        }

        private void PopulateBoundaries()
        {
            //Water boundaries
            for (int windPosition = 0; windPosition < _windSize; windPosition++)
            {
                for (int woodPosition = 0; woodPosition < _woodSize; woodPosition++)
                {
                    double fractionAlongWorld = windPosition / (double)(_windSize - 1);
                    double rescaledFraction = (4 * fractionAlongWorld - 2);
                    int apparentAirEarthLevel = MathHelpers.Round(rescaledFraction);
                    _windEarthEnvironment[0, woodPosition, windPosition] = (WindEarthTerrainType)apparentAirEarthLevel;
                    _windEarthEnvironment[_waterSize - 1, woodPosition, windPosition] = (WindEarthTerrainType)apparentAirEarthLevel;
                }
            }

            //Wood boundaries
            for (int windPosition = 0; windPosition < _windSize; windPosition++)
            {
                for (int waterPosition = 0; waterPosition < _waterSize; waterPosition++)
                {
                    double fractionAlongWorld = windPosition / (double)(_windSize - 1);
                    double rescaledFraction = (4 * fractionAlongWorld - 2);
                    int apparentAirEarthLevel = MathHelpers.Round(rescaledFraction);
                    _windEarthEnvironment[waterPosition, 0, windPosition] = (WindEarthTerrainType)apparentAirEarthLevel;
                    _windEarthEnvironment[waterPosition, _woodSize - 1, windPosition] = (WindEarthTerrainType)apparentAirEarthLevel;
                }
            }

            //Wind boundaries
            for (int woodPosition = 0; woodPosition < _woodSize; woodPosition++)
            {
                for (int waterPosition = 0; waterPosition < _waterSize; waterPosition++)
                {
                    _windEarthEnvironment[waterPosition, woodPosition, 0] = WindEarthTerrainType.HighDensityEarth;
                    _windEarthEnvironment[waterPosition, woodPosition, _windSize - 1] = WindEarthTerrainType.HighDensityWind;
                }
            }
        }

        private void PopulateLowerHalfWithEarth()
        {
            // Y = mx + b
            // Y is number of earth tiles
            // m is mEarth
            // x is position along wind axis
            // b is bEarth
            double mEarth = (.25 - 1.00) / (_windMiddle - 0);
            double bEarth = 1.00 - mEarth * 0;

            //Non boundaries
            for (int windPosition = 1; windPosition < _windMiddle; windPosition++)
            {
                List<int[]> validLowDensityEarthPoints = new List<int[]>();
                List<int[]> validHighDensityEarthPoints = new List<int[]>();

                //Populate initial conditions
                for (int woodPosition = 1; woodPosition < _woodSize - 1; woodPosition++)
                {
                    for (int waterPosition = 1; waterPosition < _waterSize - 1; waterPosition++)
                    {
                        if (ValidLowDensityEarthPlacement(waterPosition, woodPosition, windPosition))
                        {
                            validLowDensityEarthPoints.Add(new[] { waterPosition, woodPosition });
                        }
                    }
                }

                //Run algorithm
                for (int numberOfEarthSlots = 2 * MathHelpers.Round(_totalNumberOfWindEarthSlots * (mEarth * windPosition + bEarth)); numberOfEarthSlots > 0; numberOfEarthSlots--)
                {
                    int totalNumberOfPossibilities = validLowDensityEarthPoints.Count + validHighDensityEarthPoints.Count;

                    int choice = _rand.Next(1, totalNumberOfPossibilities) - 1;

                    if (choice > validLowDensityEarthPoints.Count - 1)
                    {
                        //High Density point was chosen
                        choice -= (validLowDensityEarthPoints.Count - 1);

                        int[] chosenPoint = validHighDensityEarthPoints[choice];

                        int waterPosition = chosenPoint[0];
                        int woodPosition = chosenPoint[1];

                        _windEarthEnvironment[waterPosition, woodPosition, windPosition] = WindEarthTerrainType.HighDensityEarth;

                        validHighDensityEarthPoints.Remove(chosenPoint);
                    }
                    else
                    {
                        //Low Density point was chosen
                        int[] chosenPoint = validLowDensityEarthPoints[choice];

                        int waterPosition = chosenPoint[0];
                        int woodPosition = chosenPoint[1];

                        _windEarthEnvironment[waterPosition, woodPosition, windPosition] = WindEarthTerrainType.LowDensityEarth;

                        validLowDensityEarthPoints.Remove(chosenPoint);

                        if (ValidHighDensityEarthPlacement(waterPosition, woodPosition, windPosition))
                            validHighDensityEarthPoints.Add(chosenPoint);

                        List<int[]> potentialNextPoints = new List<int[]>();

                        if (waterPosition - 1 > 0)
                            potentialNextPoints.Add(new int[] { waterPosition - 1, woodPosition });
                        if (woodPosition - 1 > 0)
                            potentialNextPoints.Add(new int[] { waterPosition, woodPosition - 1 });
                        if (waterPosition + 1 < _waterSize - 1)
                            potentialNextPoints.Add(new int[] { waterPosition + 1, woodPosition });
                        if (woodPosition + 1 < _woodSize - 1)
                            potentialNextPoints.Add(new int[] { waterPosition, woodPosition + 1 });

                        foreach (int[] testPoint in potentialNextPoints)
                        {
                            int testWaterPosition = testPoint[0];
                            int testWoodPosition = testPoint[1];

                            bool emptySlot = _windEarthEnvironment[testWaterPosition, testWoodPosition, windPosition] == WindEarthTerrainType.Neutral;
                            bool alreadyAdded = validLowDensityEarthPoints.Contains(testPoint);

                            if (emptySlot && !alreadyAdded)
                            {
                                validLowDensityEarthPoints.Add(testPoint);
                            }
                        }
                    }
                }
            }
        }

        private void PopulateUpperHalfWithWind()
        {
            // Y = mx + b
            // Y is number of wind tiles
            // m is mWind
            // x is position along wind axis
            // b is bWind
            double mWind = (.25 - 1.00) / (_windMiddle - (_windSize - 1));
            double bWind = 1.00 - mWind * (_windSize - 1);

            //Non boundaries
            for (int windPosition = _windSize - 2; windPosition > _windMiddle; windPosition--)
            {
                List<int[]> validLowDensityWindPoints = new List<int[]>();
                List<int[]> validHighDensityWindPoints = new List<int[]>();

                //Populate initial conditions
                for (int woodPosition = 1; woodPosition < _woodSize - 1; woodPosition++)
                {
                    for (int waterPosition = 1; waterPosition < _waterSize - 1; waterPosition++)
                    {
                        if (ValidLowDensityWindPlacement(waterPosition, woodPosition, windPosition))
                        {
                            validLowDensityWindPoints.Add(new[] { waterPosition, woodPosition });
                        }
                    }
                }

                //Run algorithm
                for (int numberOfWindSlots = 2 * MathHelpers.Round(_totalNumberOfWindEarthSlots * (mWind * windPosition + bWind)); numberOfWindSlots > 0; numberOfWindSlots--)
                {
                    int totalNumberOfPossibilities = validLowDensityWindPoints.Count + validHighDensityWindPoints.Count;

                    int choice = _rand.Next(1, totalNumberOfPossibilities) - 1;

                    if (choice > validLowDensityWindPoints.Count - 1)
                    {
                        //High Density point was chosen
                        choice -= (validLowDensityWindPoints.Count - 1);

                        int[] chosenPoint = validHighDensityWindPoints[choice];

                        int waterPosition = chosenPoint[0];
                        int woodPosition = chosenPoint[1];

                        _windEarthEnvironment[waterPosition, woodPosition, windPosition] = WindEarthTerrainType.HighDensityWind;

                        validHighDensityWindPoints.Remove(chosenPoint);
                    }
                    else
                    {
                        //Low Density point was chosen
                        int[] chosenPoint = validLowDensityWindPoints[choice];

                        int waterPosition = chosenPoint[0];
                        int woodPosition = chosenPoint[1];

                        _windEarthEnvironment[waterPosition, woodPosition, windPosition] = WindEarthTerrainType.LowDensityWind;

                        validLowDensityWindPoints.Remove(chosenPoint);

                        if (ValidHighDensityWindPlacement(waterPosition, woodPosition, windPosition))
                            validHighDensityWindPoints.Add(chosenPoint);

                        List<int[]> potentialNextPoints = new List<int[]>();

                        if (waterPosition - 1 > 0)
                            potentialNextPoints.Add(new int[] { waterPosition - 1, woodPosition });
                        if (woodPosition - 1 > 0)
                            potentialNextPoints.Add(new int[] { waterPosition, woodPosition - 1 });
                        if (waterPosition + 1 < _waterSize - 1)
                            potentialNextPoints.Add(new int[] { waterPosition + 1, woodPosition });
                        if (woodPosition + 1 < _woodSize - 1)
                            potentialNextPoints.Add(new int[] { waterPosition, woodPosition + 1 });

                        foreach (int[] testPoint in potentialNextPoints)
                        {
                            int testWaterPosition = testPoint[0];
                            int testWoodPosition = testPoint[1];

                            bool emptySlot = _windEarthEnvironment[testWaterPosition, testWoodPosition, windPosition] == WindEarthTerrainType.Neutral;
                            bool alreadyAdded = validLowDensityWindPoints.Contains(testPoint);

                            if (emptySlot && !alreadyAdded)
                            {
                                validLowDensityWindPoints.Add(testPoint);
                            }
                        }
                    }
                }
            }
        }

        private void PopulateMiddle()
        {
            List<int[]> validLowDensityEarthPoints = new List<int[]>();
            List<int[]> validHighDensityEarthPoints = new List<int[]>();
            List<int[]> validLowDensityWindPoints = new List<int[]>();
            List<int[]> validHighDensityWindPoints = new List<int[]>();

            List<int[]> validReduceLowDensityEarthPoints = new List<int[]>();
            List<int[]> validReduceHighDensityEarthPoints = new List<int[]>();
            List<int[]> validReduceLowDensityWindPoints = new List<int[]>();
            List<int[]> validReduceHighDensityWindPoints = new List<int[]>();

            //Populate initial conditions
            for (int woodPosition = 1; woodPosition < _woodSize - 1; woodPosition++)
            {
                for (int waterPosition = 1; waterPosition < _waterSize - 1; waterPosition++)
                {
                    if (ValidLowDensityEarthPlacement(waterPosition, woodPosition, _windMiddle))
                    {
                        validLowDensityEarthPoints.Add(new[] { waterPosition, woodPosition });
                    }

                    if (ValidLowDensityWindPlacement(waterPosition, woodPosition, _windMiddle))
                    {
                        validLowDensityWindPoints.Add(new[] { waterPosition, woodPosition });
                    }
                }
            }

            //Run algorithm
            for (int numberOfWindEarthSlots = 2 * MathHelpers.Round(_totalNumberOfWindEarthSlots * .25); numberOfWindEarthSlots > 0; numberOfWindEarthSlots--)
            {
                //Earth goes first, then wind, but possibly later flip a coin

                int totalNumberOfEarthPossibilities = validLowDensityEarthPoints.Count + validHighDensityEarthPoints.Count + validReduceLowDensityWindPoints.Count + validReduceHighDensityWindPoints.Count;

                int earthChoice = _rand.Next(1, totalNumberOfEarthPossibilities) - 1;

                if (earthChoice > validLowDensityEarthPoints.Count - 1)
                {
                    //High Density point was chosen
                    earthChoice -= (validLowDensityEarthPoints.Count - 1);

                    int[] chosenPoint = validHighDensityEarthPoints[earthChoice];

                    int waterPosition = chosenPoint[0];
                    int woodPosition = chosenPoint[1];

                    _windEarthEnvironment[waterPosition, woodPosition, _windMiddle] = WindEarthTerrainType.HighDensityEarth;

                    validHighDensityEarthPoints.Remove(chosenPoint);

                    //Check if valid to reduce
                }
                else
                {
                    //Low Density point was chosen
                    int[] chosenPoint = validLowDensityEarthPoints[earthChoice];

                    int waterPosition = chosenPoint[0];
                    int woodPosition = chosenPoint[1];

                    _windEarthEnvironment[waterPosition, woodPosition, _windMiddle] = WindEarthTerrainType.LowDensityEarth;

                    validLowDensityEarthPoints.Remove(chosenPoint);

                    if (ValidHighDensityEarthPlacement(waterPosition, woodPosition, _windMiddle))
                        validHighDensityEarthPoints.Add(chosenPoint);

                    List<int[]> potentialNextPoints = new List<int[]>();

                    if (waterPosition - 1 > 0)
                        potentialNextPoints.Add(new int[] { waterPosition - 1, woodPosition });
                    if (woodPosition - 1 > 0)
                        potentialNextPoints.Add(new int[] { waterPosition, woodPosition - 1 });
                    if (waterPosition + 1 < _waterSize - 1)
                        potentialNextPoints.Add(new int[] { waterPosition + 1, woodPosition });
                    if (woodPosition + 1 < _woodSize - 1)
                        potentialNextPoints.Add(new int[] { waterPosition, woodPosition + 1 });

                    foreach (int[] testPoint in potentialNextPoints)
                    {
                        int testWaterPosition = testPoint[0];
                        int testWoodPosition = testPoint[1];

                        bool emptySlot = _windEarthEnvironment[testWaterPosition, testWoodPosition, _windMiddle] == WindEarthTerrainType.Neutral;
                        bool alreadyAdded = validLowDensityEarthPoints.Contains(testPoint);

                        if (emptySlot && !alreadyAdded)
                        {
                            validLowDensityEarthPoints.Add(testPoint);
                        }
                    }
                }
            }
        }

        private void PopulateLowerHalfWithWind()
        {

        }

        private void PopulateUpperHalfWithEarth()
        {

        }

        private bool ValidLowDensityEarthPlacement(int waterPosition, int woodPosition, int windPosition)
        {
            bool emptySlot = _windEarthEnvironment[waterPosition, woodPosition, windPosition] == WindEarthTerrainType.Neutral;
            bool touchingAdjacentValid =
                _windEarthEnvironment[waterPosition - 1, woodPosition, windPosition] < WindEarthTerrainType.Neutral ||
                _windEarthEnvironment[waterPosition, woodPosition - 1, windPosition] < WindEarthTerrainType.Neutral ||
                _windEarthEnvironment[waterPosition + 1, woodPosition, windPosition] < WindEarthTerrainType.Neutral ||
                _windEarthEnvironment[waterPosition, woodPosition + 1, windPosition] < WindEarthTerrainType.Neutral ||
                _windEarthEnvironment[waterPosition, woodPosition, windPosition - 1] < WindEarthTerrainType.Neutral;

            return emptySlot && touchingAdjacentValid;
        }

        private bool ValidHighDensityEarthPlacement(int waterPosition, int woodPosition, int windPosition)
        {
            bool emptySlot = _windEarthEnvironment[waterPosition, woodPosition, windPosition] == WindEarthTerrainType.LowDensityEarth;
            bool touchingAdjacentValid =
                _windEarthEnvironment[waterPosition - 1, woodPosition, windPosition] < WindEarthTerrainType.LowDensityEarth ||
                _windEarthEnvironment[waterPosition, woodPosition - 1, windPosition] < WindEarthTerrainType.LowDensityEarth ||
                _windEarthEnvironment[waterPosition + 1, woodPosition, windPosition] < WindEarthTerrainType.LowDensityEarth ||
                _windEarthEnvironment[waterPosition, woodPosition + 1, windPosition] < WindEarthTerrainType.LowDensityEarth ||
                _windEarthEnvironment[waterPosition, woodPosition, windPosition - 1] < WindEarthTerrainType.LowDensityEarth;

            return emptySlot && touchingAdjacentValid;
        }

        private bool ValidLowDensityWindPlacement(int waterPosition, int woodPosition, int windPosition)
        {
            bool emptySlot = _windEarthEnvironment[waterPosition, woodPosition, windPosition] == WindEarthTerrainType.Neutral;
            bool touchingAdjacentValid =
                _windEarthEnvironment[waterPosition - 1, woodPosition, windPosition] > WindEarthTerrainType.Neutral ||
                _windEarthEnvironment[waterPosition, woodPosition - 1, windPosition] > WindEarthTerrainType.Neutral ||
                _windEarthEnvironment[waterPosition + 1, woodPosition, windPosition] > WindEarthTerrainType.Neutral ||
                _windEarthEnvironment[waterPosition, woodPosition + 1, windPosition] > WindEarthTerrainType.Neutral ||
                _windEarthEnvironment[waterPosition, woodPosition, windPosition + 1] > WindEarthTerrainType.Neutral;

            return emptySlot && touchingAdjacentValid;
        }

        private bool ValidHighDensityWindPlacement(int waterPosition, int woodPosition, int windPosition)
        {
            bool emptySlot = _windEarthEnvironment[waterPosition, woodPosition, windPosition] == WindEarthTerrainType.LowDensityWind;
            bool touchingAdjacentValid =
                _windEarthEnvironment[waterPosition - 1, woodPosition, windPosition] > WindEarthTerrainType.LowDensityWind ||
                _windEarthEnvironment[waterPosition, woodPosition - 1, windPosition] > WindEarthTerrainType.LowDensityWind ||
                _windEarthEnvironment[waterPosition + 1, woodPosition, windPosition] > WindEarthTerrainType.LowDensityWind ||
                _windEarthEnvironment[waterPosition, woodPosition + 1, windPosition] > WindEarthTerrainType.LowDensityWind ||
                _windEarthEnvironment[waterPosition, woodPosition, windPosition + 1] > WindEarthTerrainType.LowDensityWind;

            return emptySlot && touchingAdjacentValid;
        }

        private bool ValidEarthReduction(int waterPosition, int woodPosition, int windPosition)
        {
            return _windEarthEnvironment[waterPosition - 1, woodPosition, windPosition] > WindEarthTerrainType.Neutral ||
                   _windEarthEnvironment[waterPosition, woodPosition - 1, windPosition] > WindEarthTerrainType.Neutral ||
                   _windEarthEnvironment[waterPosition + 1, woodPosition, windPosition] > WindEarthTerrainType.Neutral ||
                   _windEarthEnvironment[waterPosition, woodPosition + 1, windPosition] > WindEarthTerrainType.Neutral ||
                   _windEarthEnvironment[waterPosition, woodPosition, windPosition + 1] > WindEarthTerrainType.Neutral;
        }

        private bool ValidWindReduction(int waterPosition, int woodPosition, int windPosition)
        {
            return _windEarthEnvironment[waterPosition - 1, woodPosition, windPosition] < WindEarthTerrainType.Neutral ||
                   _windEarthEnvironment[waterPosition, woodPosition - 1, windPosition] < WindEarthTerrainType.Neutral ||
                   _windEarthEnvironment[waterPosition + 1, woodPosition, windPosition] < WindEarthTerrainType.Neutral ||
                   _windEarthEnvironment[waterPosition, woodPosition + 1, windPosition] < WindEarthTerrainType.Neutral ||
                   _windEarthEnvironment[windPosition, woodPosition, windPosition - 1] < WindEarthTerrainType.Neutral;
        }
    }
}