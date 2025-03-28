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

namespace fractions
{
    /// <summary>
    /// Device basis
    /// </summary>
    public interface IDeviceBase
    {
        /// <summary>
        /// The name of this device.
        /// </summary>
        string Name { get; }
    }

    /// <summary>Common base class for input and output devices.
    /// This base class exists mainly so that input and output devices can both go into the same
    /// kinds of MidiMessages.
    /// </summary>
    public class DeviceBase : IDeviceBase
    {
        #region Constructors

        /// <summary>Protected constructor</summary>
        /// <param name="name">The name of this device</param>
        protected DeviceBase(string name)
        {
            Name = string.Copy(name);
        }

        #endregion Constructors

        #region Properties

        /// <summary>The name of this device</summary>
        public string Name { get; }

        #endregion Properties

        #region Class Members

        /// <summary>Max. posible control change value (127)</summary>
        public const int ControlChangeMax = 127;
        /// <summary>Min. posible control change value (0)</summary>
        public const int ControlChangeMin = 0;

        /// <summary>Max. posible pitch bend (16383)</summary>
        public const int PitchBendMax = 16383;
        /// <summary>Centered pitch bend (8192)</summary>
        public const int PitchBendCentered = 8192;
        /// <summary>Min. posible pitch bend (0)</summary>
        public const int PitchBendMin = 0;

        /// <returns>The value clamped between [min; max]</returns>
        public static double Clamp(double value, double min, double max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <returns>The value clamped between [min; max]</returns>
        public static float ClampFloat(double value, double min, double max)
        {
            return (float)Math.Min(Math.Max(value, min), max);
        }

        /// <returns>The value clamped between [min; max]</returns>
        public static double ClampDouble(double value, double min, double max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <returns>The value clamped between [ControlChangeMin; ControlChangeMax]</returns>
        public static int ClampControlChange(double value)
        {
            return (int)Clamp(value, ControlChangeMin, ControlChangeMax);
        }

        /// <returns>The value clamped between [PitchBendMin; PitchBendMax]</returns>
        public static int ClampPitchBend(double value)
        {
            return (int)Clamp(value, PitchBendMin, PitchBendMax);
        }

        #endregion
    }
}