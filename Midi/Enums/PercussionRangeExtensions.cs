namespace fractions
{
    public static class PercussionRangeExtensions
    {
        public static bool Inside(this PercussionRange range, Percussion value)
        {
            return value >= range.Min && value <= range.Max;
        }
    }
}