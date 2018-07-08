using ElementWorldMaker.Existence.EnvironmentMaker;

namespace ElementWorldMaker.EnvironmentViewing
{
    public interface IEnvironmentVM
    {
        ElementPoint Size { get; }

        EnvironmentColor GetColor(int water, int wood, int wind);
    }
}
