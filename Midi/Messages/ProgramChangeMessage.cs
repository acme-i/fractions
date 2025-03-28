using System;

namespace fractions
{
    /// <summary>Program Change message</summary>
    public class ProgramChangeMessage : ChannelMessage, ICloneable
    {
        #region Constructors

        /// <summary>
        ///     Constructs a Program Change message.
        /// </summary>
        /// <param name="device"> The device associated with this message</param>
        /// <param name="channel"> Channel</param>
        /// <param name="instrument"> Instrument</param>
        /// <param name="time"> The timestamp for this message</param>
        /// <param name="tag"> User-defined object</param>
        public ProgramChangeMessage(IDeviceBase device, Channel channel, Instrument instrument, float time, object tag = null)
            : base(device, channel, time, tag)
        {
            instrument.Validate();
            Instrument = instrument;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///     Instrument.
        /// </summary>
        public Instrument Instrument { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///     Returns a copy of this message, shifted in time by the specified amount.
        /// </summary>
        public override Message MakeTimeShiftedCopy(float delta) =>
            new ProgramChangeMessage(Device, Channel, Instrument, Time + delta, Tag);

        /// <summary>
        ///     Sends this message immediately.
        /// </summary>
        public override void SendNow() =>
            (Device as IOutputDevice)?.SendProgramChange(Channel, Instrument);

        public object Clone()
        {
            return new ProgramChangeMessage(Device, Channel, Instrument, Time, Tag);
        }

        #endregion Methods
    }
}