using System;
using System.Security.Policy;

namespace fractions
{
    public static class NoteOperator
    {
        public static double Plus(double left, double right)
        {
            var value = left + right;
            var max = (double)Control.MaxControl;
            if (value > max)
            {
                value = max - value;
            }
            return value.ClampControlChange();
        }

        public static NoteMessage Plus(NoteMessage left, double right, NoteProperty property = NoteProperty.Velocity)
        {
            var clone = left.Clone() as NoteMessage;
            if ((NoteProperty.Velocity & property) == NoteProperty.Velocity)
            {
                clone.Velocity = Plus(clone.Velocity, right);
            }
            if ((NoteProperty.Pan & property) == NoteProperty.Pan)
            {
                clone.Pan = Plus(clone.Pan, right);
            }
            if ((NoteProperty.Duration & property) == NoteProperty.Duration && clone is NoteOnOffMessage leftOn)
            {
                leftOn.Duration = (float)(leftOn.Duration + right);
            }
            if ((NoteProperty.Time & property) == NoteProperty.Time)
            {
                clone.Time = (float)(left.Time + right);
            }
            return clone;
        }

        public static double Minus(double left, double right)
        {
            var value = left - right;
            if (value < (double)Control.MinControl)
            {
                value = Math.Abs(value);
            }
            return value.ClampControlChange();
        }

        public static NoteMessage Minus(NoteMessage left, double right, NoteProperty property = NoteProperty.Velocity)
        {
            var clone = left.Clone() as NoteMessage;
            if ((NoteProperty.Velocity & property) == NoteProperty.Velocity)
            {
                clone.Velocity = Minus(clone.Velocity, right);
            }
            if ((NoteProperty.Pan & property) == NoteProperty.Pan)
            {
                clone.Pan = Minus(clone.Pan, right);
            }
            if ((NoteProperty.Duration & property) == NoteProperty.Duration && clone is NoteOnOffMessage leftOn)
            {
                leftOn.Duration = (float)Math.Abs(leftOn.Duration - right);
            }
            if ((NoteProperty.Time & property) == NoteProperty.Time)
            {
                clone.Time = (float)(left.Time - right);
            }
            return clone;
        }

        public static NoteMessage Plus(NoteMessage left, NoteMessage right, NoteProperty property = NoteProperty.Velocity)
        {
            var clone = left.Clone() as NoteMessage;
            if ((NoteProperty.Velocity & property) == NoteProperty.Velocity)
            {
                clone.Velocity = Plus(clone.Velocity, right.Velocity);
            }
            if ((NoteProperty.Pan & property) == NoteProperty.Pan)
            {
                clone.Pan = Plus(clone.Pan, right.Pan);
            }
            if ((NoteProperty.Duration & property) == NoteProperty.Duration && clone is NoteOnOffMessage leftOn && right is NoteOnOffMessage rightOn)
            {
                leftOn.Duration += rightOn.Duration;
            }
            if ((NoteProperty.Time & property) == NoteProperty.Time)
            {
                clone.Time += right.Time;
            }
            return clone;
        }

        public static NoteMessage Minus(NoteMessage left, NoteMessage right, NoteProperty property = NoteProperty.Velocity)
        {
            var clone = left.Clone() as NoteMessage;
            if ((NoteProperty.Velocity & property) == NoteProperty.Velocity)
            {
                clone.Velocity = Minus(clone.Velocity, right.Velocity);
            }
            if ((NoteProperty.Pan & property) == NoteProperty.Pan)
            {
                clone.Pan = Minus(clone.Pan, right.Pan);
            }
            if ((NoteProperty.Duration & property) == NoteProperty.Duration && clone is NoteOnOffMessage leftOn && right is NoteOnOffMessage rightOn)
            {
                leftOn.Duration = Math.Abs(leftOn.Duration - rightOn.Duration);
            }
            if ((NoteProperty.Time & property) == NoteProperty.Time)
            {
                clone.Time -= right.Time;
            }
            return clone;
        }

        public static double Multiply(double left, double right)
        {
            if (1.0 != right && right > 0.0)
            {
                var value = left * Math.Abs(right);
                var max = (double)Control.MaxControl;
                if (value > max)
                {
                    value = max - (value % max);
                }
                return value.ClampControlChange();
            }
            return left;
        }

        public static NoteMessage Multiply(NoteMessage left, double right, NoteProperty property = NoteProperty.Velocity)
        {
            var clone = left.Clone() as NoteMessage;
            if ((property & NoteProperty.Velocity) == NoteProperty.Velocity)
            {
                clone.Velocity = Multiply(left.Velocity, right);
            }
            if ((property & NoteProperty.Pan) == NoteProperty.Pan)
            {
                clone.Pan = Multiply(left.Pan, right);
            }
            if ((property & NoteProperty.Reverb) == NoteProperty.Reverb)
            {
                clone.Pan = Multiply(left.Reverb ?? 1.0, right);
            }
            return clone;
        }

        public static NoteMessage Multiply(NoteMessage left, NoteMessage right, NoteProperty property = NoteProperty.Velocity)
        {
            var clone = left.Clone() as NoteMessage;
            if ((property & NoteProperty.Velocity) == NoteProperty.Velocity)
            {
                clone.Velocity = Multiply(left.Velocity, right.Velocity);
            }
            if ((property & NoteProperty.Pan) == NoteProperty.Pan)
            {
                clone.Pan = Multiply(left.Pan, right.Pan);
            }
            if ((property & NoteProperty.Reverb) == NoteProperty.Reverb)
            {
                if (left.Reverb != null || right.Reverb != null)
                {
                    if (left.Reverb == null)
                        clone.Reverb = right.Reverb;
                    else if (right.Reverb == null)
                        clone.Reverb = left.Reverb;
                    else 
                        clone.Reverb = Multiply(left.Reverb ?? 1.0, right.Reverb ?? 1.0);
                }
            }
            return clone;
        }
    }
}