using System;
using System.Diagnostics;

namespace fractions
{
    /// <summary>Base class for messages relevant to a specific note</summary>
    [DebuggerDisplay("Pitch = {Pitch}, Velocity = {Velocity}, Channel = {Channel}, Time = {Time}, Pan = {Pan}")]
    public abstract class NoteMessage : ChannelMessage, INoteMessage
    {
        #region Constructors

        /// <summary>Protected constructor</summary>
        protected NoteMessage(IDeviceBase device, Channel channel, Pitch pitch, double velocity, float time, double pan = -1, Instrument? instrument = null, double? reverb = null, object tag = null)
            : base(device, channel, time, tag)
        {
            if (float.IsInfinity(time))
                throw new ArgumentException(nameof(time));
            if (double.IsInfinity(pan))
                pan = 63;

            Pitch = pitch.ToMidiRange();
            Velocity = velocity.ClampControlChange();
            Pan = pan.ClampControlChange();
            Instrument = instrument;
            Reverb = reverb;
        }

        #endregion Constructors

        #region Methods

        public static NoteMessage operator +(NoteMessage left, NoteMessage right)
        {
            return NoteOperator.Plus(left, right, NoteProperty.Velocity | NoteProperty.Pan);
        }

        public static NoteMessage operator +(NoteMessage left, double right)
        {
            return NoteOperator.Plus(left, right, NoteProperty.Velocity | NoteProperty.Pan);
        }

        public static NoteMessage operator -(NoteMessage left, NoteMessage right)
        {
            return NoteOperator.Minus(left, right, NoteProperty.Velocity | NoteProperty.Pan);
        }

        public static NoteMessage operator -(NoteMessage left, double right)
        {
            return NoteOperator.Minus(left, right, NoteProperty.Velocity | NoteProperty.Pan);
        }

        public static NoteMessage operator *(NoteMessage left, double right)
        {
            return NoteOperator.Multiply(left, right, NoteProperty.Velocity | NoteProperty.Pan);
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