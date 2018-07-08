using ElementWorldMaker.Existence.EnvironmentMaker;
using ElementWorldMaker.Existence.WaterFire;

namespace ElementWorldMaker.EnvironmentViewing
{
    public class WaterFireVM : IEnvironmentVM
    {
        public ElementPoint Size { get; protected set; }

        private int[,,] _environment;

        public WaterFireVM(ElementPoint size, int[,,] environment)
        {
            Size = size;
            _environment = environment;
        }

        public EnvironmentColor GetColor(int water, int wood, int wind)
        {
            double waterLevel = (_environment[water, wood, wind] + 2) / 4.0;

            byte waterColor = (byte)((1.0 - waterLevel) * byte.MaxValue);
            byte woodColor = (byte)(.5 * byte.MaxValue);
            byte windColor = (byte)(.5 * byte.MaxValue);

            return new EnvironmentColor(waterColor, woodColor, windColor);
        }
    }
}