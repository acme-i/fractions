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
using System.Linq;
using System.Text.RegularExpressions;

namespace fractions
{
    public static class Instruments
    {
        private static readonly Lazy<List<Instrument>> _all = new Lazy<List<Instrument>>(() =>
            Enum.GetValues(typeof(Instrument))
                .Cast<Instrument>()
                .ToList()
        );

        private static readonly Lazy<List<Instrument>> _pianos = new Lazy<List<Instrument>>(() =>
            Enum.GetValues(typeof(Instrument))
                    .Cast<Instrument>()
                    .Where(i => Regex.IsMatch(i.Name(), "piano", RegexOptions.IgnoreCase))
                    .ToList()
        );

        private static readonly Lazy<List<Instrument>> _guitars = new Lazy<List<Instrument>>(() =>
            Enum.GetValues(typeof(Instrument))
                    .Cast<Instrument>()
                    .Where(i => Regex.IsMatch(i.Name(), "guitar", RegexOptions.IgnoreCase))
                    .ToList()
        );

        private static readonly Lazy<List<Instrument>> _softGuitars = new Lazy<List<Instrument>>(() =>
            Enum.GetValues(typeof(Instrument))
                    .Cast<Instrument>()
                    .Where(i => Regex.IsMatch(i.Name(), "guitar", RegexOptions.IgnoreCase))
                    .Except(new[] {
                        Instrument.OverdrivenGuitar, Instrument.DistortionGuitar, Instrument.GuitarHarmonics,
                        Instrument.GuitarFretNoise, Instrument.AcousticGuitarSteel })
                    .ToList()
        );

        private static readonly Lazy<List<Instrument>> _basses = new Lazy<List<Instrument>>(() =>
            Enum.GetValues(typeof(Instrument))
                    .Cast<Instrument>()
                    .Where(i => Regex.IsMatch(i.Name(), "bass", RegexOptions.IgnoreCase))
                    .Except(new[] { Instrument.Bassoon })
                    .ToList()
        );

        private static readonly Lazy<List<Instrument>> _pads = new Lazy<List<Instrument>>(() =>
            Enum.GetValues(typeof(Instrument))
                    .Cast<Instrument>()
                    .Where(i => Regex.IsMatch(i.Name(), "pad", RegexOptions.IgnoreCase))
                    .ToList()
        );

        private static readonly Lazy<List<Instrument>> _softBasses = new Lazy<List<Instrument>>(() =>
            new[] {
                Instrument.AcousticBass, Instrument.ElectricBassFinger,
                Instrument.ElectricBassPick, Instrument.FretlessBass
            }.ToList()
        );

        /// <summary>
        /// Returns a list of all instruments
        /// </summary>
        public static List<Instrument> All => _all.Value;


        /// <summary>
        /// Returns a list of all pianos
        /// </summary>
        public static List<Instrument> Pianos => _pianos.Value;

        /// <summary>
        /// Returns a list of all guitars
        /// </summary>
        public static List<Instrument> Guitars => _guitars.Value;

        /// <summary>
        /// Returns a list of all guitars
        /// </summary>
        public static List<Instrument> SoftGuitars => _softGuitars.Value;

        /// <summary>
        /// Returns a list of all pads
        /// </summary>
        public static List<Instrument> Pads = _pads.Value;

        /// <summary>
        /// Returns a list of all pads
        /// </summary>
        public static List<Instrument> Basses = _basses.Value;

        /// <summary>
        /// Returns a list of all guitars
        /// </summary>
        public static List<Instrument> SoftBasses = _softBasses.Value;

    }
}