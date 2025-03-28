// Copyright (c) 2009, Tom Lokovic All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;

namespace fractions
{
    /// <summary>Pitch Bend message</summary>
    public class PitchBendMessage : ChannelMessage, ICloneable
    {
        #region Constructors

        /// <summary>
        ///     Constructs a Pitch Bend message.
        /// </summary>
        /// <param name="device"> The device associated with this message</param>
        /// <param name="channel"> Channel, 0..15, 10 reserved for percussion</param>
        /// <param name="value"> Pitch bend value, 0..16383, 8192 is centered</param>
        /// <param name="time"> The timestamp for this message</param>
        /// <param name="tag"> User-defined object</param>
        public PitchBendMessage(IDeviceBase device, Channel channel, double value, float time, object tag = null)
            : base(device, channel, time, tag)
        {
            Value = value;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///     Pitch bend value, 0..16383, 8192 is centered.
        /// </summary>
        public double Value { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///     Returns a copy of this message, shifted in time by the specified amount.
        /// </summary>
        public override Message MakeTimeShiftedCopy(float delta) =>
            new PitchBendMessage(Device, Channel, Value, Time + delta, Tag);

        /// <summary>
        ///     Sends this message immediately.
        /// </summary>
        public override void SendNow() =>
            (Device as IOutputDevice)?.SendPitchBend(Channel, DeviceBase.ClampPitchBend(Value));

        public object Clone()
        {
            return new PitchBendMessage(Device, Channel, Value, Time, Tag);
        }

        #endregion Methods
    }
}