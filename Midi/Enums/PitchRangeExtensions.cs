namespace fractions
{
    public static class PitchRangeExtensions
    {
        public static bool IsInside(this PitchRange range, Pitch value)
        {
            return value >= range.Min && value <= range.Max;
        }
    }
}