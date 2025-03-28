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
    /// <summary>Note On message</summary>
    public class NoteOnMessage : NoteMessage, ICloneable
    {
        #region Constructors

        /// <summary>Constructs a Note On message</summary>
        /// <param name="device"> The device associated with this message</param>
        /// <param name="channel"> Channel, 0..15, 10 reserved for percussion</param>
        /// <param name="pitch"> The pitch for this note message</param>
        /// <param name="velocity"> Velocity, 0..127</param>
        /// <param name="time"> The timestamp for this message</param>
        /// <param name="pan"> Pan, 0..127</param>
        /// <param name="instrument"> Instrument, 0..127. Default is null. The instrument will not change when SendNow is called</param>
        /// <param name="reverb"> Reverb, 0..127</param>
        /// <param name="tag"> User object</param>
        public NoteOnMessage(IDeviceBase device, Channel channel, Pitch pitch = Pitch.C4, double velocity = 120, float time = 0, double pan = -1, Instrument? instrument = null, double? reverb = null, object tag = null)
            : base(device, channel, pitch, velocity, time, pan, instrument, reverb, tag) { }

        #endregion Constructors

        public NoteOnHandler BeforeSending;
        public NoteOnHandler AfterSending;

        #region Methods

        /// <summary>Sends this message immediately</summary>
        public override void SendNow()
        {
            BeforeSending?.Invoke(this);
            if (Device is IOutputDevice device)
            {
                if (Math.Sign(Pan) >= 0)
                {
                    device.SendControlChange(Channel, Control.Pan, DeviceBase.ClampControlChange(Pan));
                }
                if (Instrument is Instrument instrument)
                {
                    device.SendProgramChange(Channel, instrument);
                }
                device.SendNoteOn(Channel, Pitch, DeviceBase.ClampControlChange(Velocity));
            }

            AfterSending?.Invoke(this);
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
            return new NoteOnMessage(Device, Channel, Pitch, Velocity, Time, Pan, Instrument, Reverb, Tag);
        }

        #endregion Methods
    }
}