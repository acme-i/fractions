using System;

namespace fractions
{
    /// <summary>Control change message</summary>
    public class ControlChangeMessage : ChannelMessage, ICloneable
    {
        #region Constructors

        /// <summary>
        ///     Construts a Control Change message.
        /// </summary>
        /// <param name="device"> The device associated with this message</param>
        /// <param name="channel"> Channel, 0..15, 10 reserved for percussion</param>
        /// <param name="control"> Control, 0..119 </param>
        /// <param name="value"> Value, 0..127</param>
        /// <param name="time"> The timestamp for this message</param>
        /// <param name="tag"> User-defined object</param>
        public ControlChangeMessage(IDeviceBase device, Channel channel, Control control, int value, float time, object tag = null)
            : base(device, channel, time, tag)
        {
            control.Validate();
            Control = control;
            Value = value;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///     The control for this message.
        /// </summary>
        public Control Control { get; }

        /// <summary>
        ///     Value, 0..127.
        /// </summary>
        public int Value { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///     Returns a copy of this message, shifted in time by the specified amount.
        /// </summary>
        public override Message MakeTimeShiftedCopy(float delta) =>
            new ControlChangeMessage(Device, Channel, Control, Value, Time + delta, Tag);

        /// <summary>
        ///     Sends this message immediately.
        /// </summary>
        public override void SendNow() =>
            (Device as IOutputDevice)?.SendControlChange(Channel, Control, DeviceBase.ClampControlChange(Value));

        public object Clone()
        {
            return new ControlChangeMessage(Device, Channel, Control, Value, Time, Tag);
        }

        public override string ToString()
        {
            return $"Control: {Control}, Value: {Value}";
        }

        #endregion Methods
    }
}