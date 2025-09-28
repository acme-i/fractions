using System;

namespace fractions
{
    /// <summary>
    ///     Extension methods for the Channel enum.
    /// </summary>
    public static class ChannelExtensions
    {
        #region Fields

        /// <summary>
        ///     Table of channel names.
        /// </summary>
        private static readonly string[] ChannelNames = {
            "Channel 1",
            "Channel 2",
            "Channel 3",
            "Channel 4",
            "Channel 5",
            "Channel 6",
            "Channel 7",
            "Channel 8",
            "Channel 9",
            "Channel 10",
            "Channel 11",
            "Channel 12",
            "Channel 13",
            "Channel 14",
            "Channel 15",
            "Channel 16",
            "All channels",
        };

        #endregion Fields

        #region Methods

        public static Channel Clamp(this Control value)
        {
            return (Channel)Math.Min(Math.Max((int)value, (int)Channel.Channel1), (int)Channel.Channel16);
        }

        /// <summary>
        ///     Returns true if the specified channel is valid.
        /// </summary>
        /// <param name="channel">The channel to test.</param>
        public static bool IsValid(this Channel channel) => channel >= Channel.Channel1 && channel <= Channel.Channel16;

        /// <summary>
        ///     Returns the human-readable name of a MIDI channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <exception cref="ArgumentOutOfRangeException">The channel is out-of-range.</exception>
        public static string Name(this Channel channel)
        {
            channel.Validate();
            return ChannelNames[(int)channel];
        }

        /// <summary>
        ///     Throws an exception if channel is not valid.
        /// </summary>
        /// <param name="channel">The channel to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">The channel is out-of-range.</exception>
        public static void Validate(this Channel channel)
        {
            if (!channel.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }
        }

        public static bool IsPercussion(this Channel channel)
        {
            return channel == Channel.Channel10;
        }

        #endregion Methods
    }
}