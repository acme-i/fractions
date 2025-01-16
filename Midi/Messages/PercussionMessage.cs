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
    /// <summary>Percussion message</summary>
    /// <remarks>
    /// A percussion message is simply shorthand for sending a Note On message to Channel10 with a
    /// percussion-specific note. This message can be sent to an OutputDevice but will be received
    /// by an InputDevice as a NoteOn message.
    /// </remarks>
    public class PercussionMessage : DeviceMessage, ICloneable
    {
        #region Constructors

        public PercussionHandler BeforeSendingPercussion;
        public PercussionHandler AfterSendingPercussion;

        /// <summary>Constructs a Percussion message</summary>
        /// <param name="device"> The device associated with this message</param>
        /// <param name="percussion"> Percussion</param>
        /// <param name="velocity"> Velocity, 0..127</param>
        /// <param name="time"> The timestamp for this message</param>
        /// <param name="tag">a user-defined object</param>
        public PercussionMessage(IDeviceBase device, Percussion percussion, int velocity, float time, object tag = null)
            : base(device, time)
        {
            percussion.Validate();
            Percussion = percussion;
            Velocity = velocity;
            Tag = tag;
        }

        #endregion Constructors

        #region Properties

        /// <summary>Percussion</summary>
        public Percussion Percussion { get; set; }

        /// <summary>Velocity, 0..127</summary>
        public int Velocity { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>Returns a copy of this message, shifted in time by the specified amount</summary>
        public override Message MakeTimeShiftedCopy(float delta) =>
            new PercussionMessage(Device, Percussion, Velocity, Time + delta, Tag);

        /// <summary>Sends this message immediately</summary>
        public override void SendNow()
        {
            BeforeSendingPercussion?.Invoke(this);
            (Device as IOutputDevice)?.SendNoteOn(Channel.Channel10, (Pitch)Percussion, DeviceBase.ClampControlChange(Velocity));
            AfterSendingPercussion?.Invoke(this);
        }

        public object Clone()
        {
            return new PercussionMessage(Device, Percussion, Velocity, Time, Tag);
        }

        #endregion Methods
    }
}