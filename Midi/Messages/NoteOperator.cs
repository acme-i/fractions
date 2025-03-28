using System;

namespace fractions
{
    public static class NoteOperator
    {
        public static double Plus(double left, double right)
        {
            var value = left + right;
            if (value > DeviceBase.ControlChangeMax)
            {
                value = DeviceBase.ControlChangeMax - value;
            }
            return DeviceBase.ClampControlChange(value);
        }

        public static NoteMessage Multiply(NoteMessage left, double right, NoteProperty property = NoteProperty.Velocity)
        {
            var clone = left.Clone() as NoteMessage;
            if (1.0 != right && clone.Velocity > 0)
            {
                var value = clone.Velocity * Math.Abs(right);
                if (value > DeviceBase.ControlChangeMax)
                {
                    value = DeviceBase.ControlChangeMax - (value % DeviceBase.ControlChangeMax);
                }
                clone.Velocity = DeviceBase.ClampControlChange(value);
            }
            return clone;
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
            if (value < DeviceBase.ControlChangeMin)
            {
                value = Math.Abs(value);
            }
            return DeviceBase.ClampControlChange(value);
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
    }
}