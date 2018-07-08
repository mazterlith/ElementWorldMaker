using ElementWorldMaker.Existence.EnvironmentMaker;
using ElementWorldMaker.Existence.WoodMetal;

namespace ElementWorldMaker.EnvironmentViewing
{
    public class WoodMetalVM : IEnvironmentVM
    {
        public ElementPoint Size { get; protected set; }

        private int[,,] _environment;

        public WoodMetalVM(ElementPoint size, int[,,] environment)
        {
            Size = size;
            _environment = environment;
        }

        public EnvironmentColor GetColor(int water, int wood, int wind)
        {
            double woodLevel = (_environment[water, wood, wind] + 2) / 4.0;

            byte waterColor = (byte)(.5 * byte.MaxValue);
            byte woodColor = (byte)(woodLevel * byte.MaxValue);
            byte windColor = (byte)(.5 * byte.MaxValue);

            return new EnvironmentColor(waterColor, woodColor, windColor);
        }
    }
}