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
using System.Collections.Generic;

namespace fractions.examples
{
    /// <summary>
    /// Summarizer class
    /// </summary>
    public class Summarizer
    {
        #region Fields

        /// <summary>
        /// Returns the input device
        /// </summary>
        public IInputDevice InputDevice { get; private set; }

        /// <summary>
        /// Returns the pitches pressed
        /// </summary>
        public Dictionary<Pitch, bool> PitchesPressed { get; private set; }

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs the summarizer
        /// </summary>
        /// <param name="inputDevice"></param>
        public Summarizer(IInputDevice inputDevice)
        {
            InputDevice = inputDevice;
            InputDevice.NoteOn += NoteOn;
            InputDevice.NoteOff += NoteOff;
            PitchesPressed = new Dictionary<Pitch, bool>();
            PrintStatus();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Turns off the note
        /// </summary>
        /// <param name="msg"></param>
        public void NoteOff(NoteOffMessage msg)
        {
            lock (this)
            {
                PitchesPressed.Remove(msg.Pitch);
                PrintStatus();
            }
        }

        /// <summary>
        /// Turns on the note
        /// </summary>
        public void NoteOn(NoteOnMessage msg)
        {
            lock (this)
            {
                PitchesPressed[msg.Pitch] = true;
                PrintStatus();
            }
        }

        private void PrintStatus()
        {
            Console.Clear();
            Console.WriteLine("Play notes and chords on the MIDI input device, and watch");
            Console.WriteLine("their names printed here.  Press any QUERTY key to quit.");
            Console.WriteLine();

            // Print the currently pressed notes.
            var pitches = new List<Pitch>(PitchesPressed.Keys);
            pitches.Sort();
            Console.Write("Notes: ");
            for (int i = 0; i < pitches.Count; ++i)
            {
                Pitch pitch = pitches[i];
                if (i > 0)
                {
                    Console.Write(", ");
                }
                Console.Write("{0}", pitch.NotePreferringSharps());
                if (pitch.NotePreferringSharps() != pitch.NotePreferringFlats())
                {
                    Console.Write(" or {0}", pitch.NotePreferringFlats());
                }
            }
            Console.WriteLine();
            // Print the currently held down chord.
            var chords = Chord.FindMatchingChords(pitches);
            Console.Write("Chords: ");
            for (int i = 0; i < chords.Count; ++i)
            {
                Chord chord = chords[i];
                if (i > 0)
                {
                    Console.Write(", ");
                }
                Console.Write("{0}", chord);
            }
            Console.WriteLine();
        }

        #endregion Methods
    }
}