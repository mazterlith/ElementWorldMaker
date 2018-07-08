using System;

namespace ElementWorldMaker.Existence
{
    public static class SpinalOperations
    {
        public static double WorldSpinalValue(double fractionAlongWorld, double rigidWorldNumber)
        {
            return Math.Pow((1.0 - Math.Pow(fractionAlongWorld, 1.0 / rigidWorldNumber)), rigidWorldNumber);
        }

        public static double FractionAlongWorld(double worldSpinalValue, double rigidWorldNumber)
        {
            return Math.Log((1.0 - Math.Log(worldSpinalValue, rigidWorldNumber)), 1.0 / rigidWorldNumber);
        }
    }
}