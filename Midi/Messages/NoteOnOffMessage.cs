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
using System.Diagnostics;

namespace fractions
{
    /// <summary>A Note On message which schedules its own Note Off message when played</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class NoteOnOffMessage : NoteMessage, ICloneable
    {
        #region Constructors

        /// <summary>Constructs a Note On/Off message</summary>
        /// <param name="device"> The device associated with this message</param>
        /// <param name="channel"> Channel, 0..15, 10 reserved for percussion</param>
        /// <param name="pitch"> The pitch for this note message</param>
        /// <param name="velocity"> Velocity, 0..127</param>
        /// <param name="time"> The timestamp for this message</param>
        /// <param name="clock"> The clock that should schedule the off message</param>
        /// <param name="duration"> Time delay between on message and off message</param>
        /// <param name="pan"> Pan, 0..127. Default is 64 (centered)</param>
        /// <param name="instrument"> Instrument, 0..127. Default is -1. Instrument will not change</param>
        /// <param name="reverb"> Reverb, 0..127. Default is null. The reverb level will not change when SendNow is called</param>
        /// <param name="tag">a user defined object</param>
        public NoteOnOffMessage(IDeviceBase device, Channel channel, Pitch pitch, double velocity, float time, Clock clock, float duration, double pan = -1, Instrument? instrument = null, double? reverb = -1, object tag = null)
            : base(device, channel, pitch, velocity, time, pan, instrument, reverb, tag)
        {
            Clock = clock;
            Duration = duration;
        }

        public NoteOnOffMessage(NoteOnOffMessage m)
            : base(m.Device, m.Channel, m.Pitch, m.Velocity, m.Time, m.Pan, m.Instrument, m.Reverb, m.Tag)
        {
            Clock = m.Clock;
            Duration = m.Duration;
        }

        public NoteOnOffMessage(NoteOnOffMessage m, int pitchOffset = 0, float timeOffset = 0f)
            : base(m.Device, m.Channel, m.Pitch + pitchOffset, m.Velocity, m.Time + timeOffset, m.Pan, m.Instrument, m.Reverb, m.Tag)
        {
            Clock = m.Clock;
            Duration = m.Duration;
        }

        public NoteOnOffMessage(NoteOnOffMessage m, int pitchOffset = 0, float timeOffset = 0f, double? newPan = null)
            : base(m.Device, m.Channel, m.Pitch + pitchOffset, m.Velocity, m.Time + timeOffset, newPan ?? m.Pan, m.Instrument, m.Reverb, m.Tag)
        {
            Clock = m.Clock;
            Duration = m.Duration;
        }

        #endregion Constructors

        public NoteOnOffHandler BeforeSendingNoteOnOff;
        public NoteOnOffHandler AfterSendingNoteOnOff;

        #region Properties

        /// <summary>The clock used to schedule the follow-up message</summary>
        public Clock Clock { get; }

        /// <summary>Time delay between the Note On and the Note Off</summary>
        public float Duration { get; set; }

        public int MinOctave { get; set; } = -1;
        public int MaxOctave { get; set; } = 9;

        #endregion Properties

        #region Methods

        public bool IsValid()
        {
            return false == (float.IsNaN(Duration) || double.IsNaN(Pan) || float.IsNaN(this.Time));
        }

        /// <summary>Returns a copy of this message, shifted in time by the specified amount</summary>
        public override Message MakeTimeShiftedCopy(float delta) =>
            new NoteOnOffMessage(Device, Channel, Pitch, Velocity, Time + delta, Clock, Duration, Pan, Instrument, Reverb, Tag);

        /// <summary>Sends this message immediately</summary>
        public override void SendNow()
        {
            BeforeSendingNoteOnOff?.Invoke(this);
            while (MaxOctave < Pitch.Octave())
            {
                Pitch -= 12;
            }
            while (MinOctave > Pitch.Octave())
            {
                Pitch += 12;
            }
            if (Device is IOutputDevice outputDevice)
            {
                if (Pan >= 0)
                {
                    outputDevice.SendControlChange(Channel, Control.Pan, DeviceBase.ClampControlChange(Pan));
                }
                if (Reverb is double reverb)
                {
                    outputDevice.SendControlChange(Channel, Control.ReverbLevel, DeviceBase.ClampControlChange(reverb));
                }
                if (Instrument is Instrument instrument)
                {
                    outputDevice.SendProgramChange(Channel, instrument);
                }
                outputDevice.SendNoteOn(Channel, Pitch, DeviceBase.ClampControlChange(Velocity));
            }
            Clock.Schedule(new NoteOffMessage(Device, Channel, Pitch, Velocity, Time + Duration));
            AfterSendingNoteOnOff?.Invoke(this);
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
            return new NoteOnOffMessage(Device, Channel, Pitch, Velocity, Time, Clock, Duration, Pan, Instrument, Reverb, Tag);
        }

        public override string ToString()
        {
            return $"{Channel}: Pitch: {Pitch}, Time: {Time:0.000}, Duration: {Duration:0.000}, Pan: {Pan:0.000}, Reverb: {Reverb:0.000}, Instrument: {Instrument}";
        }

        #endregion Methods

        #region Debug
        private string DebuggerDisplay
        {
            get
            {
                return $"{nameof(NoteOnOffMessage)}: {Channel}, Pitch: {Pitch}, Time: {Time:0.000}, Duration: {Duration:0.000}, Pan: {Pan:0.000}";
            }
        }

        #endregion
    }
}