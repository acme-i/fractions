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

namespace fractions
{
    /// <summary>
    ///     Extension methods for the Control enum.
    /// </summary>
    /// Be sure to "using midi" if you want to use these as extension methods.
    public static class ControlExtensions
    {
        #region Fields

        /// <summary>
        ///     Table of control names.
        /// </summary>
        private static readonly Dictionary<int, string> ControlNames = new Dictionary<int, string>
        {
            {1, "Modulation wheel"},
            {6, "Data Entry MSB"},
            {7, "Volume"},
            {10, "Pan"},
            {11, "Expression"},
            {38, "Data Entry LSB"},
            {64, "Sustain pedal"},
            {91, "Reverb level"},
            {92, "Tremolo level"},
            {93, "Chorus level"},
            {94, "Celeste level"},
            {95, "Phaser level"},
            {98, "Non-registered Parameter LSB"},
            {99, "Non-registered Parameter MSB"},
            {100, "Registered Parameter Number LSB"},
            {101, "Registered Parameter Number MSB"},
            {121, "All controllers off"},
            {123, "All notes off"}
        };

        #endregion Fields

        #region Methods

        /// <summary>
        ///     Returns true if the specified control is valid.
        /// </summary>
        /// <param name="control">The Control to test.</param>
        public static bool IsValid(this Control control) => control >= 0 && (int)control < 128;

        /// <summary>
        ///     Returns the human-readable name of a MIDI control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <exception cref="ArgumentOutOfRangeException">The control is out-of-range.</exception>
        public static string Name(this Control control)
        {
            control.Validate();
            if (ControlNames.ContainsKey((int)control))
            {
                return ControlNames[(int)control];
            }
            return "Other Control (see MIDI specification for details).";
        }

        /// <summary>
        ///     Throws an exception if control is not valid.
        /// </summary>
        /// <param name="control">The control to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">The control is out-of-range.</exception>
        public static void Validate(this Control control)
        {
            if (!control.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(control));
            }
        }

        #endregion Methods
    }
}