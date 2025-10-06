using System;

namespace fractions
{
    public class PitchRange : EnumRange<Pitch>
    {
        public static readonly Pitch _minPitch = Pitch.CNeg1;
        public static readonly Pitch _maxPitch = Pitch.G9;

        public PitchRange() : base(_minPitch, _maxPitch)
        {
        }

        public PitchRange(Pitch min, Pitch max) : base(min, max)
        {
        }

        public static PitchRange Default 
        { 
            get => new PitchRange();
        }

        public override void Assert(Pitch min, Pitch max)
        {
            if (min < _minPitch)
                throw new ArgumentException($"{nameof(Min)} must be >= {_minPitch}", nameof(Min));
            if (max > _maxPitch)
                throw new ArgumentException($"{nameof(Max)} must be <= {_maxPitch}", nameof(Max));
            if (min > max)
                throw new ArgumentException($"{nameof(Min)} must be <= {nameof(Max)}", nameof(Min));
        }
    }
}