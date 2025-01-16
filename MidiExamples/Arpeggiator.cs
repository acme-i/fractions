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

using System.Collections.Generic;

namespace fractions.examples
{
    public class Arpeggiator
    {
        #region Fields

        private readonly Clock clock;

        private int currentChordPattern;

        private int currentScalePattern;

        private readonly IInputDevice inputDevice;

        private readonly Dictionary<Pitch, List<Pitch>> lastSequenceForPitch;

        private readonly IOutputDevice outputDevice;

        private bool playingChords;

        #endregion Fields

        #region Constructors

        public Arpeggiator(IInputDevice inputDevice, IOutputDevice outputDevice, Clock clock)
        {
            this.inputDevice = inputDevice;
            this.outputDevice = outputDevice;
            this.clock = clock;
            currentChordPattern = 0;
            currentScalePattern = 0;
            playingChords = false;
            lastSequenceForPitch = new Dictionary<Pitch, List<Pitch>>();

            if (this.inputDevice != null)
            {
                this.inputDevice.NoteOn += NoteOn;
                this.inputDevice.NoteOff += NoteOff;
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>String describing the arpeggiator's current configuration</summary>
        public string Status
        {
            get
            {
                lock (this)
                {
                    if (playingChords)
                    {
                        return $"Chord: {Chord.Patterns[currentChordPattern].Name}";
                    }
                    return "Scale: " + Scale.Patterns[currentScalePattern].Name;
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>Changes the current chord or scale, whichever is the current mode</summary>
        public void Change(int delta)
        {
            lock (this)
            {
                if (playingChords)
                {
                    currentChordPattern += delta;
                    while (currentChordPattern < 0)
                    {
                        currentChordPattern += Chord.Patterns.Length;
                    }
                    while (currentChordPattern >= Chord.Patterns.Length)
                    {
                        currentChordPattern -= Chord.Patterns.Length;
                    }
                }
                else
                {
                    currentScalePattern += delta;
                    while (currentScalePattern < 0)
                    {
                        currentScalePattern += Scale.Patterns.Length;
                    }
                    while (currentScalePattern >= Scale.Patterns.Length)
                    {
                        currentScalePattern -= Scale.Patterns.Length;
                    }
                }
            }
        }

        public void NoteOff(NoteOffMessage msg)
        {
            if (!lastSequenceForPitch.ContainsKey(msg.Pitch))
            {
                return;
            }
            List<Pitch> pitches = lastSequenceForPitch[msg.Pitch];
            lastSequenceForPitch.Remove(msg.Pitch);
            for (int i = 1; i < pitches.Count; ++i)
            {
                clock.Schedule(new NoteOffMessage(outputDevice, msg.Channel,
                    pitches[i], msg.Velocity, msg.Time + i));
            }
        }

        public void NoteOn(NoteOnMessage msg)
        {
            lock (this)
            {
                var pitches = new List<Pitch>();
                if (playingChords)
                {
                    var chord = new Chord(msg.Pitch.NotePreferringSharps(), Chord.Patterns[currentChordPattern], 0);
                    Pitch p = msg.Pitch;
                    for (int i = 0; i < chord.NoteSequence.Length; ++i)
                    {
                        p = chord.NoteSequence[i].PitchAtOrAbove(p);
                        pitches.Add(p);
                    }
                }
                else
                {
                    var scale = new Scale(msg.Pitch.NotePreferringSharps(), Scale.Patterns[currentScalePattern]);
                    Pitch p = msg.Pitch;
                    for (int i = 0; i < scale.NoteSequence.Length; ++i)
                    {
                        p = scale.NoteSequence[i].PitchAtOrAbove(p);
                        pitches.Add(p);
                    }
                    pitches.Add(msg.Pitch + 12);
                }
                lastSequenceForPitch[msg.Pitch] = pitches;
                for (int i = 1; i < pitches.Count; ++i)
                {
                    clock.Schedule(
                        new NoteOnMessage(outputDevice, msg.Channel, pitches[i], msg.Velocity, msg.Time + i)
                    );
                }
            }
        }

        /// <summary>Toggle between playing chords and playing scales</summary>
        public void ToggleMode()
        {
            lock (this)
            {
                playingChords = !playingChords;
            }
        }

        #endregion Methods
    }
}