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
using NUnit.Framework;

namespace fractions.tests
{
    /// <summary>Unit tests for the Scale class</summary>
    public class ScaleTests
    {
        #region Methods

        [Test]
        public void Construction()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new Scale(new Note('F'), null));
            Assert.AreEqual(new Scale(new Note('C'), Scale.Major), new Scale(new Note('C'), Scale.Major));
            Assert.AreNotEqual(new Scale(new Note('C'), Scale.Major), new Scale(new Note('C', Note.Sharp), Scale.Major));
            Assert.AreNotEqual(new Scale(new Note('C'), Scale.Major), new Scale(new Note('D'), Scale.Major));
            Assert.AreNotEqual(new Scale(new Note('C'), Scale.Major), new Scale(new Note('C'), Scale.NaturalMinor));
        }

        [Test]
        public void Contains()
        {
            var cmajor = new Scale(new Note("C"), Scale.Major);
            var cmajorPitches = new Pitch[] {
                Pitch.C2, Pitch.D2, Pitch.E2, Pitch.F2, Pitch.G2, Pitch.A2, Pitch.B2 };
            var cmajorNotPitches = new Pitch[] {
                Pitch.CSharp2, Pitch.DSharp2, Pitch.FSharp2, Pitch.GSharp2, Pitch.ASharp2 };
            foreach (Pitch p in cmajorPitches)
            {
                Assert.True(cmajor.Contains(p));
            }
            foreach (Pitch p in cmajorNotPitches)
            {
                Assert.False(cmajor.Contains(p));
            }

            var bbhminor = new Scale(new Note("Bb"), Scale.HarmonicMinor);
            var bbhminorPitches = new Pitch[] {
                Pitch.ASharp2, Pitch.C3, Pitch.CSharp3, Pitch.DSharp3, Pitch.F3, Pitch.FSharp3,
                Pitch.A3 };
            var bbhminorNotPitches = new Pitch[] {
                Pitch.B2, Pitch.D3, Pitch.E3, Pitch.G3, Pitch.GSharp3  };
            foreach (Pitch p in bbhminorPitches)
            {
                Assert.True(bbhminor.Contains(p));
            }
            foreach (Pitch p in bbhminorNotPitches)
            {
                Assert.False(bbhminor.Contains(p));
            }
        }

        [Test]
        public void NoteSequences()
        {
            Assert.AreEqual(SequenceString(new Scale(new Note("C"), Scale.Major)), "C, D, E, F, G, A, B");
            Assert.AreEqual(SequenceString(new Scale(new Note("F#"), Scale.Major)), "F#, G#, A#, B, C#, D#, E#");
            Assert.AreEqual(SequenceString(new Scale(new Note("Gb"), Scale.Major)), "Gb, Ab, Bb, Cb, Db, Eb, F");
            Assert.AreEqual(SequenceString(new Scale(new Note("Bb"), Scale.NaturalMinor)), "Bb, C, Db, Eb, F, Gb, Ab");
            Assert.AreEqual(SequenceString(new Scale(new Note("Bb"), Scale.NaturalMinor)), "Bb, C, Db, Eb, F, Gb, Ab");
            Assert.AreEqual(SequenceString(new Scale(new Note("Bb"), Scale.HarmonicMinor)), "Bb, C, Db, Eb, F, Gb, A");
        }

        [Test]
        public void Properties()
        {
            Assert.AreEqual(new Scale(new Note("Fb"), Scale.NaturalMinor).Name, "Fb Natural Minor");
            Assert.AreEqual(new Scale(new Note("Fb"), Scale.NaturalMinor).Tonic, new Note("Fb"));
            Assert.AreEqual(new Scale(new Note("Fb"), Scale.NaturalMinor).Pattern, Scale.NaturalMinor);
        }

        /// <summary>Returns a comma-separated string with the note names of c's NoteSequence</summary>
        private static string SequenceString(Scale s) => string.Join(", ", s.NoteSequence);

        #endregion Methods
    }
}