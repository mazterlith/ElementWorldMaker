namespace ElementWorldMaker.Existence.EnvironmentMaker
{
    public class ElementPoint
    {
        public int waterPosition { get; set; }
        public int woodPosition { get; set; }
        public int windPosition { get; set; }

        public ElementPoint CloneWithOffset(int waterOffset, int woodOffset, int windOffset)
        {
            return new ElementPoint() { waterPosition = waterPosition + waterOffset, woodPosition = woodPosition + woodOffset, windPosition = windPosition + windOffset };
        }

        public static ElementPoint operator +(ElementPoint left, ElementPoint right)
        {
            return left.CloneWithOffset(right.waterPosition, right.woodPosition, right.windPosition);
        }

        public static ElementPoint operator -(ElementPoint left, ElementPoint right)
        {
            return left.CloneWithOffset(-right.waterPosition, -right.woodPosition, -right.windPosition);
        }

        public static ElementPoint operator *(ElementPoint point, int scalar)
        {
            return new ElementPoint() { waterPosition = point.waterPosition * scalar, woodPosition = point.woodPosition * scalar, windPosition = point.windPosition * scalar };
        }

        public override bool Equals(object obj)
        {
            if (obj is ElementPoint point)
            {
                if (point.waterPosition != waterPosition || point.woodPosition != woodPosition || point.windPosition != windPosition) return false;

                return true;
            }
            else return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -952849686;
            hashCode = hashCode * -1521134295 + waterPosition.GetHashCode();
            hashCode = hashCode * -1521134295 + woodPosition.GetHashCode();
            hashCode = hashCode * -1521134295 + windPosition.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ElementPoint left, ElementPoint right)
        {
            return left.waterPosition == right.waterPosition &&
                   left.woodPosition == right.woodPosition &&
                   left.windPosition == right.windPosition;
        }

        public static bool operator !=(ElementPoint left, ElementPoint right)
        {
            return !(left == right);
        }
    }
}