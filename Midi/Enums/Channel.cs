﻿// Copyright (c) 2009, Tom Lokovic
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

namespace fractions
{
    /// <summary>
    ///     A MIDI Channel.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Each MIDI device has 16 independent channels.  Channels are named starting at 1, but
    ///         are encoded programmatically starting at 0.
    ///     </para>
    ///     <para>
    ///         All of the channels are general-purpose except for Channel10, which is the
    ///         dedicated percussion channel.  Any notes sent to that channel will play
    ///         <see cref="Percussion">percussion notes</see>, regardless of any
    ///         <see cref="OutputDevice.SendProgramChange">Program Change</see> messages sent on that
    ///         channel.
    ///     </para>
    ///     <para>
    ///         This enum has extension methods, such as <see cref="ChannelExtensions.Name" />
    ///         and <see cref="ChannelExtensions.IsValid" />, defined in
    ///         <see cref="ChannelExtensions" />.
    ///     </para>
    /// </remarks>
    public enum Channel
    {
        /// <summary>MIDI Channel 1</summary>
        Channel1 = 0,

        /// <summary>MIDI Channel 2</summary>
        Channel2 = 1,

        /// <summary>MIDI Channel 3</summary>
        Channel3 = 2,

        /// <summary>MIDI Channel 4</summary>
        Channel4 = 3,

        /// <summary>MIDI Channel 5</summary>
        Channel5 = 4,

        /// <summary>MIDI Channel 6</summary>
        Channel6 = 5,

        /// <summary>MIDI Channel 7</summary>
        Channel7 = 6,

        /// <summary>MIDI Channel 8</summary>
        Channel8 = 7,

        /// <summary>MIDI Channel 9</summary>
        Channel9 = 8,

        /// <summary>MIDI Channel 10, the dedicated percussion channel</summary>
        Channel10 = 9,

        /// <summary>MIDI Channel 11</summary>
        Channel11 = 10,

        /// <summary>MIDI Channel 12</summary>
        Channel12 = 11,

        /// <summary>MIDI Channel 13</summary>
        Channel13 = 12,

        /// <summary>MIDI Channel 14</summary>
        Channel14 = 13,

        /// <summary>MIDI Channel 15</summary>
        Channel15 = 14,

        /// <summary>MIDI Channel 16</summary>
        Channel16 = 15

    };
}