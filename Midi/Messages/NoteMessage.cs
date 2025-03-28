using System;
using System.Diagnostics;

namespace fractions
{
    /// <summary>Base class for messages relevant to a specific note</summary>
    [DebuggerDisplay("Pitch = {Pitch}, Velocity = {Velocity}, Channel = {Channel}, Time = {Time}, Pan = {Pan}")]
    public abstract class NoteMessage : ChannelMessage, ICloneable
    {
        #region Constructors

        /// <summary>Protected constructor</summary>
        protected NoteMessage(IDeviceBase device, Channel channel, Pitch pitch, double velocity, float time, double pan = -1, Instrument? instrument = null, double? reverb = null, object tag = null)
            : base(device, channel, time, tag)
        {
            if (float.IsInfinity(time))
                throw new ArgumentException(nameof(time));
            if (double.IsPositiveInfinity(velocity))
                velocity = 127;
            if (double.IsNegativeInfinity(velocity))
                velocity = 0;
            if (double.IsInfinity(pan))
                pan = 63;

            if (!pitch.IsInMidiRange())
            {
                pitch = pitch.ToMidiRange();
            }
            Pitch = pitch;
            Velocity = DeviceBase.ClampControlChange(velocity);
            Pan = DeviceBase.ClampControlChange(pan);
            Instrument = instrument;
            Reverb = reverb;
        }

        #endregion Constructors

        #region Methods

        public static NoteMessage operator +(NoteMessage left, NoteMessage right)
        {
            var props = NoteProperty.Velocity;
            props |= NoteProperty.Pan;
            return NoteOperator.Plus(left, right, props);
        }

        public static NoteMessage operator +(NoteMessage left, double right)
        {
            var props = NoteProperty.Velocity;
            props |= NoteProperty.Pan;
            return NoteOperator.Plus(left, right, props);
        }

        public static NoteMessage operator -(NoteMessage left, NoteMessage right)
        {
            var props = NoteProperty.Velocity;
            props |= NoteProperty.Pan;
            return NoteOperator.Minus(left, right, props);
        }

        public static NoteMessage operator -(NoteMessage left, double right)
        {
            var props = NoteProperty.Velocity;
            props |= NoteProperty.Pan;
            return NoteOperator.Minus(left, right, props);
        }

        public static NoteMessage operator *(NoteMessage left, double right)
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

        /// <summary>Returns a copy of this message, shifted in time by the specified amount</summary>
        public override Message MakeTimeShiftedCopy(float delta) => new NoteOnMessage(Device, Channel, Pitch, Velocity, Time + delta, Pan, Instrument, Reverb, Tag);

        /// <summary>
        /// Returns a copy of this message, shifted in time by the specified amount with the new velocity.
        ///</summary>
        public Message MakeTimeShiftedCopy(float delta, double newVelocity) => new NoteOnMessage(Device, Channel, Pitch, newVelocity, Time + delta, Pan, Instrument, Reverb, Tag);

        /// <summary>Returns a copy of this message, shifted in time by the specified amount</summary>
        public Message MakeTimeShiftedOffCopy(float delta) => new NoteOffMessage(Device, Channel, Pitch, Velocity, Time + delta, Pan, Instrument, Reverb, Tag);

        public abstract void SetOctaveAbove();

        public abstract void SetOctaveBelow();

        public abstract object Clone();

        #endregion

        #region Properties

        /// <summary>The pitch for this note message</summary>
        public Pitch Pitch { get; set; }

        /// <summary>Velocity, 0..127</summary>
        public double Velocity { get; set; }

        /// <summary>Pan, 0..127</summary>
        public double Pan { get; set; }

        /// <summary>Instrument, 0..127</summary>
        public Instrument? Instrument { get; set; }

        /// <summary>Instrument, 0..127</summary>
        public double? Reverb { get; set; }

        #endregion Properties
    }
}