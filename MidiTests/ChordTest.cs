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
using NUnit.Framework;

namespace fractions.tests
{

    /// <summary>Unit tests for the Chord class</summary>
    public class ChordTests
    {
        #region Methods

        [Test]
        public void Construction()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new Chord(new Note('F'), null, 0));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new Chord(new Note('F'), Chord.Major, -1));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new Chord(new Note('F'), Chord.Major, 7));
            Assert.AreEqual(new Chord("C"), new Chord(new Note('C'), Chord.Major, 0));
            Assert.AreEqual(new Chord("Cm"), new Chord(new Note('C'), Chord.Minor, 0));
            Assert.AreEqual(new Chord("C#m"), new Chord(new Note('C', Note.Sharp), Chord.Minor, 0));
            Assert.AreEqual(new Chord("Fbdim"), new Chord(new Note('F', Note.Flat), Chord.Diminished, 0));
            Assert.AreEqual(new Chord("C/E"), new Chord(new Note('C'), Chord.Major, 1));
            Assert.AreEqual(new Chord("Cm/Eb"), new Chord(new Note('C'), Chord.Minor, 1));
            Assert.AreEqual(new Chord("C/G"), new Chord(new Note('C'), Chord.Major, 2));
            Assert.AreEqual(new Chord("Cm/G"), new Chord(new Note('C'), Chord.Minor, 2));
            Assert.Throws(typeof(ArgumentNullException), () => new Chord(null));
            Assert.Throws(typeof(ArgumentException), () => new Chord(""));
            Assert.Throws(typeof(ArgumentException), () => new Chord("X"));
            Assert.Throws(typeof(ArgumentException), () => new Chord("Cx"));
            Assert.Throws(typeof(ArgumentException), () => new Chord("C#b"));
            Assert.Throws(typeof(ArgumentException), () => new Chord("C/X"));
            Assert.Throws(typeof(ArgumentException), () => new Chord("Fbdimx"));
        }

        [Test]
        public void Contains()
        {
            var cmajor = new Chord("C");
            var cmajorPitches = new Pitch[] { Pitch.C2, Pitch.E2, Pitch.G2 };
            var cmajorNotPitches = new Pitch[] {
                Pitch.CSharp2,
                Pitch.D2, Pitch.DSharp2,
                Pitch.F2, Pitch.FSharp2,
                Pitch.GSharp2,
                Pitch.A2, Pitch.ASharp2
            };
            foreach (Pitch p in cmajorPitches)
            {
                Assert.True(cmajor.Contains(p));
            }
            foreach (Pitch p in cmajorNotPitches)
            {
                Assert.False(cmajor.Contains(p));
            }

            var bbminor = new Chord("Bbm");
            var bbminorPitches = new Pitch[] {
                Pitch.ASharp2, Pitch.CSharp3, Pitch.F3
            };
            var bbminorNotPitches = new Pitch[] {
                Pitch.B2, Pitch.C3, Pitch.D3, Pitch.DSharp3,
                Pitch.E3, Pitch.G3,  Pitch.FSharp3,
                Pitch.GSharp3, Pitch.A3
            };
            foreach (Pitch p in bbminorPitches)
            {
                Assert.True(bbminor.Contains(p));
            }
            foreach (Pitch p in bbminorNotPitches)
            {
                Assert.False(bbminor.Contains(p));
            }
        }

        [Test]
        public void NoteSequences()
        {
            Assert.AreEqual(SequenceString(new Chord("C")), "C, E, G");
            Assert.AreEqual(SequenceString(new Chord("C#")), "C#, E#, G#");
            Assert.AreEqual(SequenceString(new Chord("Ab")), "Ab, C, Eb");
            Assert.AreEqual(SequenceString(new Chord("Ab/Eb")), "Eb, Ab, C");
            Assert.AreEqual(SequenceString(new Chord("Cm")), "C, Eb, G");
            Assert.AreEqual(SequenceString(new Chord("Ebm")), "Eb, Gb, Bb");
            Assert.AreEqual(SequenceString(new Chord("G#m")), "G#, B, D#");
            Assert.AreEqual(SequenceString(new Chord("G#m/B")), "B, D#, G#");
            Assert.AreEqual(SequenceString(new Chord("Abm")), "Ab, Cb, Eb");
            Assert.AreEqual(SequenceString(new Chord("Cdim")), "C, Eb, Gb");
            Assert.AreEqual(SequenceString(new Chord("Cdim/Eb")), "Eb, Gb, C");
        }

        [Test]
        public void Properties()
        {
            Assert.AreEqual(new Chord("Fm/Ab").Name, "Fm/Ab");
            Assert.AreEqual(new Chord("Fm/Ab").Root, new Note('F'));
            Assert.AreEqual(new Chord("Fm/Ab").Bass, new Note('A', Note.Flat));
            Assert.AreEqual(new Chord("Fm/Ab").Pattern, Chord.Minor);
        }

        [Test]
        public void TestAllChords()
        {
            // This test runs over every possible note/pattern/inversion combination over pitches in
            // a range extending beyond the MIDI range, and performs several consistency checks on
            // every one of those chords.

            // For every pitch in the range...
            for (Pitch pitch = (Pitch)(-50); pitch < (Pitch)200; ++pitch)
            {
                // Find the one or two common notes for this pitch.
                var notes = new List<Note>
                {
                    pitch.NotePreferringSharps()
                };
                if (pitch.NotePreferringFlats() != pitch.NotePreferringSharps())
                {
                    notes.Add(pitch.NotePreferringFlats());
                }
                // For the one or two notes for this pitch...
                notes.ForEach(note =>
                {
                    // For every chord pattern...
                    foreach (ChordPattern pattern in Chord.Patterns)
                    {
                        // For every legal inversion of the pattern...
                        for (int inversion = 0; inversion < pattern.Ascent.Length; ++inversion)
                        {
                            // Construct the chord.
                            var c = new Chord(note, pattern, inversion);
                            // Make sure the chord's name, when parsed, is equivalent to the chord.
                            Assert.AreEqual(c, new Chord(c.Name));
                            // Generated a set of pitches based on the chord, starting at or above
                            // the original pitch.
                            Pitch p = pitch;
                            var pitches = new List<Pitch>();
                            foreach (Note n in c.NoteSequence)
                            {
                                p = n.PitchAtOrAbove(p);
                                pitches.Add(p);
                            }
                            Assert.True(Chord.FindMatchingChords(pitches).Contains(c));
                        }
                    }
                });
            }
        }

        /// <summary>Returns a comma-separated string with the note names of c's NoteSequence</summary>
        private static string SequenceString(Chord c) => string.Join(", ", c.NoteSequence);

        #endregion Methods
    }
}