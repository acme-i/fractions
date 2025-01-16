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
    ///     Extension methods for the Interval enum.
    /// </summary>
    public static class IntervalExtensions
    {
        #region Fields

        /// <summary>
        ///     Table of interval names.
        /// </summary>
        private static readonly string[] IntervalNames = {
            "Unison",
            "Semitone",
            "Whole tone",
            "Minor third",
            "Major third",
            "Perfect fourth",
            "Tritone",
            "Perfect fifth",
            "Minor sixth",
            "Major sixth",
            "Minor seventh",
            "Major seventh",
            "Octave"
        };

        #endregion Fields

        #region Methods

        /// <summary>
        ///     Returns the human-readable name of an interval.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns>
        ///     The human-readable name.  If the interval is less than an octave, it gives
        ///     the standard term (eg, "Major third").  If the interval is more than an octave, it
        ///     gives the number of semitones in the interval.
        /// </returns>
        public static string Name(this Interval interval)
        {
            var value = Math.Abs((int)interval);
            if (value >= 0 && value <= 12)
            {
                return IntervalNames[value];
            }
            return $"{value} semitones";
        }

        #endregion Methods
    }
}