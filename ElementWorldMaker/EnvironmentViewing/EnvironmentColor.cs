namespace ElementWorldMaker.EnvironmentViewing
{
    public class EnvironmentColor
    {
        public byte WaterColor { get; protected set; }
        public byte WoodColor { get; protected set; }
        public byte WindColor { get; protected set; }

        public EnvironmentColor(byte waterColor, byte woodColor, byte windColor)
        {
            WaterColor = waterColor;
            WoodColor = woodColor;
            WindColor = windColor;
        }
    }
}
