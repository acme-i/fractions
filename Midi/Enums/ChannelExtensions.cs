// Copyright (c) 2009, Tom Lokovic
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

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

        /// <summary>
        ///     Returns true if the specified channel is valid.
        /// </summary>
        /// <param name="channel">The channel to test.</param>
        public static bool IsValid(this Channel channel) => channel >= 0 && (int)channel <= 16;

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