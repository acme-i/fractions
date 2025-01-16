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
    /// <summary>
    ///     Extension methods for the Percussion enum.
    /// </summary>
    /// Be sure to "using midi" if you want to use these as extension methods.
    public static class PercussionExtensions
    {
        #region Fields

        private static readonly string[] PercussionNames = {
            "Bass Drum 2",
            "Bass Drum 1",
            "Side Stick",
            "Snare Drum 1",
            "Hand Clap",
            "Snare Drum 2",
            "Low Tom 2",
            "Closed Hi-hat",
            "Low Tom 1",
            "Pedal Hi-hat",
            "Mid Tom 2",
            "Open Hi-hat",
            "Mid Tom 1",
            "High Tom 2",
            "Crash Cymbal 1",
            "High Tom 1",
            "Ride Cymbal 1",
            "Chinese Cymbal",
            "Ride Bell",
            "Tambourine",
            "Splash Cymbal",
            "Cowbell",
            "Crash Cymbal 2",
            "Vibra Slap",
            "Ride Cymbal 2",
            "High Bongo",
            "Low Bongo",
            "Mute High Conga",
            "Open High Conga",
            "Low Conga",
            "High Timbale",
            "Low Timbale",
            "High Agogo",
            "Low Agogo",
            "Cabasa",
            "Maracas",
            "Short Whistle",
            "Long Whistle",
            "Short Guiro",
            "Long Guiro",
            "Claves",
            "High Wood Block",
            "Low Wood Block",
            "Mute Cuica",
            "Open Cuica",
            "Mute Triangle",
            "Open Triangle"
        };

        #endregion Fields

        #region Methods

        /// <summary>Returns true if the specified percussion is valid</summary>
        /// <param name="percussion"> The percussion to test</param>
        public static bool IsValid(this Percussion percussion) =>
            (int)percussion >= 35 && (int)percussion <= 81;

        /// <summary>Returns the human-readable name of a MIDI percussion</summary>
        /// <param name="percussion"> The percussion</param>
        public static string Name(this Percussion percussion)
        {
            percussion.Validate();
            return PercussionNames[(int)percussion - 35];
        }

        /// <summary>Throws an exception if percussion is not valid</summary>
        /// <param name="percussion"> The percussion to validate</param>
        /// <exception cref="ArgumentOutOfRangeException">The percussion is out-of-range.</exception>
        public static void Validate(this Percussion percussion)
        {
            if (!percussion.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(percussion));
            }
        }

        #endregion Methods
    }
}