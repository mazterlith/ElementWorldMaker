namespace ElementWorldMaker
{
    public static class MathHelpers
    {
        public static int Round(double d)
        {
            if (d < 0.0)
                return (int)(d - 0.5);
            return (int)(d + 0.5);
        }
    }
}