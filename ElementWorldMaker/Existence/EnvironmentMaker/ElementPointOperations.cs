namespace ElementWorldMaker.Existence.EnvironmentMaker
{
    public static class ElementPointOperations
    {
        public static bool WithinBounds<T>(this T[,,] environment, ElementPoint point)
        {
            if (point.waterPosition < 0 || point.woodPosition < 0 || point.windPosition < 0)
                return false;

            if (point.waterPosition > environment.GetUpperBound(0) || point.woodPosition > environment.GetUpperBound(1) || point.windPosition > environment.GetUpperBound(2))
                return false;

            return true;
        }

        public static T AtPoint<T>(this T[,,] environment, ElementPoint point)
        {
            return environment[point.waterPosition, point.woodPosition, point.windPosition];
        }

        public static void SetPoint<T>(this T[,,] environment, ElementPoint point, T value)
        {
            environment[point.waterPosition, point.woodPosition, point.windPosition] = value;
        }
    }
}