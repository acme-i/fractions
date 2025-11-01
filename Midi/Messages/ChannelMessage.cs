using System.Diagnostics;

namespace fractions
{
    /// <summary>
    ///     Base class for messages relevant to a specific device channel.
    /// </summary>
    [DebuggerDisplay("Channel = {Channel}, Time = {Time}")]
    public abstract class ChannelMessage : DeviceMessage, IChannelMessage, IDeviceMessage
    {
        #region Constructors

        /// <summary>
        ///     Protected constructor.
        /// </summary>
        protected ChannelMessage(IDeviceBase device, Channel channel, float time, object tag = null)
            : base(device, time, tag)
        {
            channel.ThrowIfInvalid();
            Channel = channel;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///     Channel.
        /// </summary>
        public Channel Channel { get; set; }

        #endregion Properties
    }
}