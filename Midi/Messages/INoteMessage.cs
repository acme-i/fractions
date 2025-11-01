using System;

namespace fractions
{
    public interface INoteMessage : ICloneable
    {
        Instrument? Instrument { get; set; }
        double Pan { get; set; }
        Pitch Pitch { get; set; }
        double? Reverb { get; set; }
        double Velocity { get; set; }

        Message MakeTimeShiftedCopy(float delta);
        Message MakeTimeShiftedCopy(float delta, double newVelocity);
        Message MakeTimeShiftedOffCopy(float delta);
        void SetOctaveAbove();
        void SetOctaveBelow();
    }
}