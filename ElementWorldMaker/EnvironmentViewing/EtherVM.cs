using ElementWorldMaker.Existence.EnvironmentMaker;

namespace ElementWorldMaker.EnvironmentViewing
{
    public class EtherVM : IEnvironmentVM
    {
        public ElementPoint Size { get; protected set; }

        int[,,] _environment;

        int max = 0;

        public EtherVM(ElementPoint size, int[,,] environment)
        {
            Size = size;
            _environment = environment;
        }

        public EnvironmentColor GetColor(int water, int wood, int wind)
        {
            int etherLevel = _environment[water, wood, wind];

            if (etherLevel > max)
                max = etherLevel;

            byte halfByte = byte.MaxValue / 2;

            byte etherByte = (byte)(halfByte * (8 - etherLevel) / 8.0);

            return new EnvironmentColor(etherByte, halfByte, halfByte);
        }
    }
}