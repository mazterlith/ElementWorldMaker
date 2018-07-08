using ElementWorldMaker.Existence.EnvironmentMaker;
using ElementWorldMaker.Existence.WindEarth;

namespace ElementWorldMaker.EnvironmentViewing
{
    public class WindEarthVM : IEnvironmentVM
    {
        public ElementPoint Size { get; protected set; }

        private int[,,] _environment;

        public WindEarthVM(ElementPoint size, int[,,] environment)
        {
            Size = size;
            _environment = environment;
        }

        public EnvironmentColor GetColor(int water, int wood, int wind)
        {
            double windLevel = (_environment[water, wood, wind] + 2) / 4.0;

            byte waterColor = (byte)(.5 * byte.MaxValue);
            byte woodColor = (byte)(.5 * byte.MaxValue);
            byte windColor = (byte)(windLevel * byte.MaxValue);
            
            return new EnvironmentColor(waterColor, woodColor, windColor);
        }
    }
}