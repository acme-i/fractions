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

using NUnit.Framework;
using System;
using static NUnit.Framework.Constraints.Tolerance;

namespace fractions.tests
{
    /// <summary>Unit tests for the Pitch enum</summary>
    public class PitchTests
    {
        #region Methods

        [Test]
        public void AddSubtractTest()
        {
            Assert.AreEqual(Pitch.C4 + 1, Pitch.CSharp4);
            Assert.AreEqual(Pitch.F4 + 12, Pitch.F5);
            Assert.AreEqual(Pitch.C4 - 1, Pitch.B3);
            Assert.AreEqual(Pitch.D3 - 5, Pitch.A2);
        }

        [Test]
        public void IsInMidiRangeTest()
        {
            Assert.False(((Pitch)(-1)).IsInMidiRange());
            Assert.True(((Pitch)0).IsInMidiRange());
            Assert.True(((Pitch)127).IsInMidiRange());
            Assert.False(((Pitch)128).IsInMidiRange());
        }

        [Test]
        public void NoteWithLetterTest()
        {
            Assert.AreEqual(Pitch.C4.NoteWithLetter('C'), new Note('C', Note.Natural));
            Assert.AreEqual(Pitch.C4.NoteWithLetter('B'), new Note('B', Note.Sharp));
            Assert.AreEqual(Pitch.C4.NoteWithLetter('D'), new Note('D', Note.DoubleFlat));
            Assert.AreEqual(Pitch.CSharp4.NoteWithLetter('C'), new Note('C', Note.Sharp));
            Assert.AreEqual(Pitch.CSharp4.NoteWithLetter('B'), new Note('B', Note.DoubleSharp));
            Assert.AreEqual(Pitch.CSharp4.NoteWithLetter('D'), new Note('D', Note.Flat));
            Assert.AreEqual(Pitch.B3.NoteWithLetter('B'), new Note('B', Note.Natural));
            Assert.AreEqual(Pitch.B3.NoteWithLetter('C'), new Note('C', Note.Flat));
            Assert.AreEqual(Pitch.B3.NoteWithLetter('D'), new Note('D', -3));
            Assert.AreEqual(Pitch.B3.NoteWithLetter('E'), new Note('E', -5));
        }

        [Test]
        public void OctaveTest()
        {
            Assert.AreEqual(Pitch.C4.Octave(), 4);
            Assert.AreEqual(Pitch.B3.Octave(), 3);
            Assert.AreEqual(((Pitch)0).Octave(), -1);
            Assert.AreEqual(((Pitch)(-1)).Octave(), -2);
        }

        [Test]
        public void PitchToNoteTest()
        {
            Assert.AreEqual(Pitch.C4.NotePreferringSharps(), new Note('C', Note.Natural));
            Assert.AreEqual(Pitch.C4.NotePreferringFlats(), new Note('C', Note.Natural));
            Assert.AreEqual(Pitch.CSharp4.NotePreferringSharps(), new Note('C', Note.Sharp));
            Assert.AreEqual(Pitch.CSharp4.NotePreferringFlats(), new Note('D', Note.Flat));
            Assert.AreEqual(Pitch.B3.NotePreferringSharps(), new Note('B', Note.Natural));
            Assert.AreEqual(Pitch.B3.NotePreferringFlats(), new Note('B', Note.Natural));
            Assert.AreEqual(((Pitch)0).NotePreferringSharps(), new Note('C', Note.Natural));
            Assert.AreEqual(((Pitch)(-1)).NotePreferringSharps(), new Note('B', Note.Natural));
        }

        [Test]
        public void PositionInOctaveTest()
        {
            Assert.AreEqual(Pitch.C4.PositionInOctave(), 0);
            Assert.AreEqual(Pitch.B3.PositionInOctave(), 11);
            Assert.AreEqual(((Pitch)0).PositionInOctave(), 0);
            Assert.AreEqual(((Pitch)(-1)).PositionInOctave(), 11);
        }

        [Test]
        public void Default_Is_Not_Readonly_But_Always_Returns_Default_Range()
        {
            Assert.AreEqual(Pitch.CNeg1, PitchRange.Default.Min);
            Assert.AreEqual(Pitch.G9, PitchRange.Default.Max);

            var range = PitchRange.Default;
            Assert.AreEqual(Pitch.CNeg1, range.Min);
            Assert.AreEqual(Pitch.G9, range.Max);

            range.Min = Pitch.C4;
            Assert.AreEqual(Pitch.C4, range.Min);
            Assert.AreEqual(Pitch.G9, range.Max);

            range.Max = Pitch.G5;
            Assert.AreEqual(Pitch.G5, range.Max);

            var range2 = PitchRange.Default;
            Assert.AreEqual(Pitch.CNeg1, range2.Min);
            Assert.AreEqual(Pitch.G9, range2.Max);

            Assert.AreNotEqual(range.Min, range2.Min);
            Assert.AreNotEqual(range.Max, range2.Max);
        }

        [Test]
        public void Default_Constructor_Works()
        {
            var range = new PitchRange(Pitch.CNeg1, Pitch.G9);
            Assert.AreEqual(Pitch.CNeg1, range.Min);
            Assert.AreEqual(Pitch.G9, range.Max);

            Assert.Throws(typeof(ArgumentException), () => new PitchRange((Pitch)((int)Pitch.CNeg1 - 1), Pitch.G9));
            Assert.Throws(typeof(ArgumentException), () => new PitchRange(Pitch.CNeg1, (Pitch)((int)Pitch.G9 + 1)));
            Assert.Throws(typeof(ArgumentException), () => new PitchRange((Pitch)((int)Pitch.CNeg1 - 1), (Pitch)((int)Pitch.G9 + 1)));
        }

        [Test]
        public void Set_MinMax_Works()
        {
            var range = PitchRange.Default;

            range.Min = Pitch.C4;
            Assert.AreEqual(Pitch.C4, range.Min);

            range.Max = Pitch.C5;
            Assert.AreEqual(Pitch.C5, range.Max);
        }

        [Test]
        public void Setting_MinMax_Beyond_Range_Throws_ArgumentException()
        {
            var range = PitchRange.Default;
            Assert.Throws(typeof(ArgumentException), () => range.Min = (Pitch)((int)Pitch.CNeg1 - 1));
            Assert.Throws(typeof(ArgumentException), () => range.Max = (Pitch)((int)Pitch.G9 + 1));
            Assert.Throws(typeof(ArgumentException), () => range.Min = (Pitch)((int)Pitch.G9 + 1));
            Assert.Throws(typeof(ArgumentException), () => range.Max = (Pitch)((int)Pitch.CNeg1 - 1));
        }

        [Test]
        public void IsInside_works()
        {
            var range = new PitchRange(Pitch.C4, Pitch.B5);
            Assert.True(range.IsInside(Pitch.C4)); // min edge
            Assert.True(range.IsInside(Pitch.F4)); // inside
            Assert.True(range.IsInside(Pitch.B5)); // max edge

            Assert.False(range.IsInside(Pitch.B3)); // below min
            Assert.False(range.IsInside(Pitch.C6)); // above max

            range.Min = Pitch.C3; // expand down
            Assert.True(range.IsInside(Pitch.B3)); // now inside
            Assert.False(range.IsInside(Pitch.C6)); // still outside

            range.Max = Pitch.C6; // expand up
            Assert.True(range.IsInside(Pitch.B3)); // still inside
            Assert.True(range.IsInside(Pitch.C6)); // now inside

            Assert.False(range.IsInside(Pitch.B2)); // below new min
            Assert.False(range.IsInside(Pitch.C7)); // above new max

            // expand both ways
            range.Min = Pitch.C2;
            range.Max = Pitch.C7;
            Assert.True(range.IsInside(Pitch.B3)); // still inside
            Assert.True(range.IsInside(Pitch.C6)); // still inside

            Assert.False(range.IsInside(Pitch.C1)); // below new min
            Assert.False(range.IsInside(Pitch.C8)); // above new max
        }

        #endregion Methods
    }
}