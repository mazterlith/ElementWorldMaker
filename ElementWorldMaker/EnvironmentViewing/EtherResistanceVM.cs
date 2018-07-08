using ElementWorldMaker.Existence.EnvironmentMaker;

namespace ElementWorldMaker.EnvironmentViewing
{
    public class EtherResistanceVM : IEnvironmentVM
    {
        public ElementPoint Size { get; protected set; }

        int[,,] _environment;

        public EtherResistanceVM(ElementPoint size, int[,,] environment)
        {
            Size = size;
            _environment = environment;
        }

        public EnvironmentColor GetColor(int water, int wood, int wind)
        {
            int etherResistanceLevel = _environment[water, wood, wind];

            byte etherByte = (byte)(etherResistanceLevel / 6.0 * byte.MaxValue);

            return new EnvironmentColor(etherByte, etherByte, etherByte);
        }
    }
}