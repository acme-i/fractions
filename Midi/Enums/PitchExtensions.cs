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

namespace fractions
{
    /// <summary>Extension methods for the Pitch enum</summary>
    public static class PitchExtensions
    {
        #region Fields

        /// <summary>Maps PositionInOctave() to a Note preferring flats</summary>
        private static readonly Note[] PositionInOctaveToNotesPreferringFlats = {
            new Note('C'), new Note('D', Note.Flat),
            new Note('D'), new Note('E', Note.Flat),
            new Note('E'),
            new Note('F'), new Note('G', Note.Flat),
            new Note('G'), new Note('A', Note.Flat),
            new Note('A'), new Note('B', Note.Flat),
            new Note('B')
        };

        /// <summary>Maps PositionInOctave() to a Note preferring sharps</summary>
        private static readonly Note[] PositionInOctaveToNotesPreferringSharps = {
            new Note('C'), new Note('C', Note.Sharp),
            new Note('D'), new Note('D', Note.Sharp),
            new Note('E'),
            new Note('F'), new Note('F', Note.Sharp),
            new Note('G'), new Note('G', Note.Sharp),
            new Note('A'), new Note('A', Note.Sharp),
            new Note('B')
        };

        #endregion Fields

        #region Methods

        public static Pitch Clamp(this Pitch value)
        {
            return (Pitch)Math.Min(Math.Max((int)value, 0), 127);
        }

        /// <summary>Returns true if pitch is in the MIDI range [1..127]</summary>
        /// <param name="pitch"> The pitch to test</param>
        /// <returns> True if the pitch is in [0..127]. </returns>
        public static bool IsInMidiRange(this Pitch pitch)
        {
            return pitch >= 0 && (int)pitch < 128;
        }

        public static Pitch ToMidiRange(this Pitch pitch)
        {
            if (pitch > Pitch.G9)
            {
                while (pitch > Pitch.G9)
                {
                    pitch -= 12;
                }
            }
            else
            {
                while (pitch < Pitch.CNeg1)
                {
                    pitch += 12;
                }
            }
            return pitch;
        }


        /// <summary>
        /// Returns the simplest note that resolves to this pitch, preferring flats where needed.
        ///</summary>
        /// <param name="pitch"> The pitch</param>
        /// <returns>
        /// The simplest note for that pitch. If that pitch is a "white key", the note is simply a
        /// letter with no accidentals (and is the same as <see cref="NotePreferringSharps" />).
        /// Otherwise the note has a flat.
        /// </returns>
        public static Note NotePreferringFlats(this Pitch pitch)
        {
            return PositionInOctaveToNotesPreferringFlats[pitch.PositionInOctave()];
        }

        /// <summary>
        /// Returns the simplest note that resolves to this pitch, preferring sharps where needed.
        ///</summary>
        /// <param name="pitch"> The pitch</param>
        /// <returns>
        /// The simplest note for that pitch. If that pitch is a "white key", the note is simply a
        /// letter with no accidentals (and is the same as <see cref="NotePreferringFlats" />).
        /// Otherwise the note has a sharp.
        /// </returns>
        public static Note NotePreferringSharps(this Pitch pitch)
        {
            return PositionInOctaveToNotesPreferringSharps[pitch.PositionInOctave()];
        }

        /// <summary>Returns the note that would name this pitch if it used the given letter</summary>
        /// <param name="pitch"> The pitch being named</param>
        /// <param name="letter"> The letter to use in the name, in ['A'..'G']</param>
        /// <returns>
        /// The note for pitch with letter. The result may have a large number of accidentals if
        /// pitch is not easily named by letter.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">letter is out of range.</exception>
        public static Note NoteWithLetter(this Pitch pitch, char letter)
        {
            if (letter < 'A' || letter > 'G')
            {
                throw new ArgumentOutOfRangeException(nameof(letter));
            }
            var pitchNote = pitch.NotePreferringSharps();
            var letterNote = new Note(letter);
            var upTo = letterNote.SemitonesUpTo(pitchNote);
            var downTo = letterNote.SemitonesDownTo(pitchNote);
            if (upTo <= downTo)
            {
                return new Note(letter, upTo);
            }
            return new Note(letter, -downTo);
        }

        /// <summary>Returns the octave containing this pitch</summary>
        /// <param name="pitch"> The pitch</param>
        /// <returns>
        /// The octave, where octaves begin at each C, and Middle C is the first pitch in octave 4.
        /// </returns>
        public static int Octave(this Pitch pitch)
        {
            var p = (int)pitch;
            return (p < 0 ? (p - 11) / 12 : p / 12) - 1;
        }

        /// <summary>Returns the position of this pitch in its octave</summary>
        /// <param name="pitch"> The pitch</param>
        /// <returns>
        /// The pitch's position in its octave, where octaves start at each C, so C's position is 0,
        /// C#'s position is 1, etc.
        /// </returns>
        public static int PositionInOctave(this Pitch pitch)
        {
            var p = (int)pitch;
            return p < 0 ? 11 - ((-p - 1) % 12) : p % 12;
        }

        #endregion Methods
    }
}