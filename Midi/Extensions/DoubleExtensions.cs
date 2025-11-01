
using System;

namespace fractions
{
    public static class DoubleExtensions
    {
        /// <returns>The value clamped between [min; max]</returns>
        public static double Clamp(this double value, double min, double max)
        {
            if (double.IsPositiveInfinity(value)) value = max;
            if (double.IsNegativeInfinity(value)) value = min;

            return Math.Min(Math.Max(value, min), max);
        }

        /// <returns>The value clamped between [ControlChangeMin; ControlChangeMax]</returns>
        public static int ClampControlChange(this int value)
        {
            return (int)Clamp(value, (double)Control.MinControl, (double)Control.MaxControl);
        }

        /// <returns>The value clamped between [ControlChangeMin; ControlChangeMax]</returns>
        public static int ClampControlChange(this double value)
        {
            if (double.IsNegativeInfinity(value)) return (int)Control.MinControl;
            if (double.IsPositiveInfinity(value)) return (int)Control.MaxControl;

            return (int)Clamp(value, (double)Control.MinControl, (double)Control.MaxControl);
        }

        /// <returns>The value clamped between [PitchBendMin; PitchBendMax]</returns>
        public static int ClampPitchBend(this int value)
        {
            return ClampPitchBend((double)value);
        }

        /// <returns>The value clamped between [PitchBendMin; PitchBendMax]</returns>
        public static int ClampPitchBend(this double value)
        {
            if (double.IsNegativeInfinity(value)) return PitchBendMessage.PitchBendMin;
            if (double.IsPositiveInfinity(value)) return PitchBendMessage.PitchBendMax;

            return (int)Clamp(value, PitchBendMessage.PitchBendMin, PitchBendMessage.PitchBendMax);
        }
    }
}