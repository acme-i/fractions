﻿// Copyright (c) 2009, Tom Lokovic
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
using System.Linq;
using System.Text.RegularExpressions;

namespace fractions
{
    public static class Instruments
    {
        /// <summary>
        /// Returns a list of all instruments
        /// </summary>
        public static List<Instrument> All
        {
            get
            {
                var instruments = Enum.GetValues(typeof(Instrument))
                    .Cast<Instrument>()
                    .ToList();
                return instruments;
            }
        }

        /// <summary>
        /// Returns a list of all pianos
        /// </summary>
        public static List<Instrument> Pianos
        {
            get
            {
                var instruments = Enum.GetValues(typeof(Instrument))
                    .Cast<Instrument>()
                    .Where(i => Regex.IsMatch(i.Name(), "piano", RegexOptions.IgnoreCase))
                    .ToList();
                return instruments;
            }
        }

        /// <summary>
        /// Returns a list of all guitars
        /// </summary>
        public static List<Instrument> Guitars
        {
            get
            {
                var instruments = Enum.GetValues(typeof(Instrument))
                    .Cast<Instrument>()
                    .Where(i => Regex.IsMatch(i.Name(), "guitar", RegexOptions.IgnoreCase))
                    .ToList();
                return instruments;
            }
        }

        /// <summary>
        /// Returns a list of all guitars
        /// </summary>
        public static List<Instrument> SoftGuitars
        {
            get
            {
                var instruments = Enum.GetValues(typeof(Instrument))
                    .Cast<Instrument>()
                    .Where(i => Regex.IsMatch(i.Name(), "guitar", RegexOptions.IgnoreCase))
                    .Except(new[] {
                        Instrument.OverdrivenGuitar, Instrument.DistortionGuitar, Instrument.GuitarHarmonics,
                        Instrument.GuitarFretNoise, Instrument.AcousticGuitarSteel })
                    .ToList();
                return instruments;
            }
        }

        /// <summary>
        /// Returns a list of all pads
        /// </summary>
        public static List<Instrument> Pads
        {
            get
            {
                var instruments = Enum.GetValues(typeof(Instrument))
                    .Cast<Instrument>()
                    .Where(i => Regex.IsMatch(i.Name(), "pad", RegexOptions.IgnoreCase))
                    .ToList();
                return instruments;
            }
        }

        /// <summary>
        /// Returns a list of all pads
        /// </summary>
        public static List<Instrument> Basses
        {
            get
            {
                var instruments = Enum.GetValues(typeof(Instrument))
                    .Cast<Instrument>()
                    .Where(i => Regex.IsMatch(i.Name(), "bass", RegexOptions.IgnoreCase))
                    .Except(new[] { Instrument.Bassoon })
                    .ToList();
                return instruments;
            }
        }

        /// <summary>
        /// Returns a list of all guitars
        /// </summary>
        public static List<Instrument> SoftBasses
        {
            get
            {
                return new[] {
                        Instrument.AcousticBass, Instrument.ElectricBassFinger,
                        Instrument.ElectricBassPick, Instrument.FretlessBass
                }.ToList();
            }
        }

    }
}