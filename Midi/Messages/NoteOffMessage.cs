using System;

namespace fractions
{
    /// <summary>Note Off message</summary>
    public class NoteOffMessage : NoteMessage, ICloneable
    {
        #region Constructors

        /// <summary>Constructs a Note Off message</summary>
        /// <param name="device"> The device associated with this message</param>
        /// <param name="channel"> Channel, 0..15, 10 reserved for percussion</param>
        /// <param name="pitch"> The pitch for this note message</param>
        /// <param name="velocity"> Velocity, 0..127</param>
        /// <param name="time"> The timestamp for this message</param>
        /// <param name="pan"> Pan, 0..127</param>
        /// <param name="instrument"> Instrument, Default is null. The instrument will not change when SendNow is called</param>
        /// <param name="reverb"> Reverb, 0..127. Default is null. The reverb level will not change when SendNow is called</param>
        /// <param name="tag"> User-defined object</param>
        public NoteOffMessage(IDeviceBase device, Channel channel, Pitch pitch, double velocity, float time, double pan = -1, Instrument? instrument = null, double? reverb = null, object tag = null)
            : base(device, channel, pitch, velocity, time, pan, instrument, reverb, tag) { }

        #endregion Constructors

        public NoteOffHandler BeforeSendingNoteOff;
        public NoteOffHandler AfterSendingNoteOff;

        #region Methods

        /// <summary>Sends this message immediately</summary>
        public override void SendNow()
        {
            BeforeSendingNoteOff?.Invoke(this);
            (Device as IOutputDevice)?.SendNoteOff(Channel, Pitch, DeviceBase.ClampControlChange(Velocity));
            AfterSendingNoteOff?.Invoke(this);
        }

        public override void SetOctaveAbove()
        {
            Pitch = Pitch.OctaveAbove();
        }

        public override void SetOctaveBelow()
        {
            Pitch = Pitch.OctaveBelow();
        }

        public override object Clone()
        {
            return new NoteOffMessage(Device, Channel, Pitch, Velocity, Time, Pan, Instrument, Reverb, Tag);
        }

        #endregion Methods
    }
}