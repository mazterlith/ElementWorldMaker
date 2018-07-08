using ElementWorldMaker.Existence.EnvironmentMaker;
using System.Collections.Generic;

namespace ElementWorldMaker.Existence.Ether
{
    public class EtherStreamGrowthDirection
    {
        public ElementPoint From { get; set; }
        public ElementPoint To { get; set; }
        public int Weight { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is EtherStreamGrowthDirection direction)
            {
                if (direction.From != From || direction.To != To || direction.Weight != Weight) return false;

                return true;
            }
            else return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 1272664866;
            hashCode = hashCode * -1521134295 + EqualityComparer<ElementPoint>.Default.GetHashCode(From);
            hashCode = hashCode * -1521134295 + EqualityComparer<ElementPoint>.Default.GetHashCode(To);
            hashCode = hashCode * -1521134295 + Weight.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(EtherStreamGrowthDirection left, EtherStreamGrowthDirection right)
        {
            return left.From == right.From &&
                   left.To == right.To &&
                   left.Weight == right.Weight;
        }

        public static bool operator !=(EtherStreamGrowthDirection left, EtherStreamGrowthDirection right)
        {
            return !(left == right);
        }
    }
}