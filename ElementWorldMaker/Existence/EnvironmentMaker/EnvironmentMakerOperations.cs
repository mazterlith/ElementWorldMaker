namespace ElementWorldMaker.Existence.EnvironmentMaker
{
    public static class EnvironmentMakerOperations
    {
        public static bool HasNeighborOfValue(int water, int wood, int wind, int value, int[,,] environment)
        {
            bool hasNeighborOfValue = false;
            hasNeighborOfValue = hasNeighborOfValue || environment[water - 1, wood, wind] == value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water + 1, wood, wind] == value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood - 1, wind] == value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood + 1, wind] == value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood, wind - 1] == value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood, wind + 1] == value;

            return hasNeighborOfValue;
        }

        public static bool HasNeighborOfValueOrLess(int water, int wood, int wind, int value, int[,,] environment)
        {
            bool hasNeighborOfValue = false;
            hasNeighborOfValue = hasNeighborOfValue || environment[water - 1, wood, wind] <= value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water + 1, wood, wind] <= value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood - 1, wind] <= value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood + 1, wind] <= value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood, wind - 1] <= value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood, wind + 1] <= value;

            return hasNeighborOfValue;
        }

        public static bool HasNeighborOfValueOrMore(int water, int wood, int wind, int value, int[,,] environment)
        {
            bool hasNeighborOfValue = false;
            hasNeighborOfValue = hasNeighborOfValue || environment[water - 1, wood, wind] >= value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water + 1, wood, wind] >= value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood - 1, wind] >= value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood + 1, wind] >= value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood, wind - 1] >= value;
            hasNeighborOfValue = hasNeighborOfValue || environment[water, wood, wind + 1] >= value;

            return hasNeighborOfValue;
        }
    }
}