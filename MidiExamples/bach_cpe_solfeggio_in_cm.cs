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
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace fractions.examples
{
    /// <summary>Demonstrates simple single-threaded output</summary>
    /// <remarks>
    /// This example uses the OutputDevice.Send* methods directly to generate output. It uses
    /// Thread.Sleep for timing, which isn't practical in real applications because it blocks the
    /// main thread, forcing the user to sit and wait. See Example03.cs for a more realistic example
    /// using Clock for scheduling.
    /// </remarks>
    internal class BachCpeSolfeggioInCm : ExampleBase
    {
        #region Constructors

        public BachCpeSolfeggioInCm() : base("bach_cpe_solfeggio_in_cm") { }
        private static IOutputDevice outputDevice;

        #endregion Constructors

        #region Methods

        public override void Run()
        {
            // Prompt user to choose an output device (or if there is only one, use that one).
            outputDevice = OutputDevice.InstalledDevices.FirstOrDefault();
            if (outputDevice == null)
            {
                Console.WriteLine("No output devices, so can't run this example.");
                ExampleUtil.PressAnyKeyToContinue();
                return;
            }
            outputDevice.Open();

            Console.WriteLine($"Playing {Description}...");

            Play(outputDevice);
            // Close the output device.
            outputDevice.Close();

            // All done.
            Console.WriteLine();
            ExampleUtil.PressAnyKeyToContinue();
        }

        private static List<double> GetPanValues()
        {
            var panValues = Interpolator.NewEaseInOut(
                EaseType.EaseInCirc,
                EaseType.EaseOutCirc,
                0.25,
                0.5,
                2
            );

            panValues.AddRange(
                Interpolator.NewEaseInOut(
                    EaseType.EaseInExpo,
                    EaseType.EaseOutExpo,
                    0.5,
                    0.75,
                    2
                )
            );

            return panValues;
        }

        private static void Play(IOutputDevice outputDevice)
        {

            var vibraphoneChannels = Channels.InstrumentChannels.Take(13);
            var stringChannels = Channels.InstrumentChannels.Skip(13);

            foreach (var x in vibraphoneChannels)
                outputDevice.SendProgramChange(x, Instrument.Vibraphone);

            foreach (var x in stringChannels)
                outputDevice.SendProgramChange(x, Instrument.StringEnsemble1);

            var bpm = 88;
            var bps = bpm * 60;
            var clock = new Clock(bpm);

            //var rand = new Random(1976);
            //var files = Directory.GetFiles(@".\midifiles\", "*.mid");
            //var path = files[rand.GetNext(files.Count())];
            var path = @".\midifiles\bach_cpe_solfeggio_in_cm.mid";

            var file = new MidiFile(path);
            var div = (float)file.TicksPerQuarterNote;

            var (OnEvents, OffEvents, Durations) = file.GetEventsAndDurations();
            var ons = OnEvents.ToList();
            var durs = Durations.ToList();

            var strings = file.GetEventsAndDurations(Channel.Channel3);
            var stringOns = strings.OnEvents.ToList();
            var stringDurs = strings.Durations.ToList();

            var mainChannels = new Enumerate<Channel>(vibraphoneChannels);
            var striChannels = new Enumerate<Channel>(stringChannels);
            var panValues = GetPanValues();

            var minValues = new Enumerate<int>(new[] { 0, 64, 20, 64, 30, 64, 40, 64, 50, 64 }, IncrementMethod.Cyclic);
            var volMinValues = new Enumerate<int>(new[] { 80, 90, 100 }, IncrementMethod.Cyclic);

            var pmin = minValues.GetNext();
            var vmin = volMinValues.GetNext();

            var panner = new Enumerate<double>(panValues.Select(p => Math.Max(pmin, p * (127 - pmin))), IncrementMethod.MinMax);
            var voller = new Enumerate<double>(panValues.Select(p => Math.Max(vmin, p * (127 - vmin))), IncrementMethod.MinMax);
            var prevTrack = -1;
            for (int time = 0; time < ons.Count; time++)
            {
                var evt = ons[time];
                var dur = durs[time];

                if (-1 == prevTrack)
                {
                    prevTrack = evt.Channel;
                }

                var note = new NoteOnOffMessage(outputDevice, mainChannels.GetNext(), (Pitch)evt.Note, voller.GetNext(), evt.Time / div, clock, dur * 0.125f, panner.GetNext());
                clock.Schedule(note);

                if (evt.Channel != prevTrack)
                {
                    pmin = minValues.GetNext();
                    vmin = volMinValues.GetNext();
                    panner.Set(panValues.Select(p => Math.Max(pmin, p * (127 - pmin))));
                    voller.Set(panValues.Select(p => Math.Max(vmin, p * (127 - vmin))));
                }

                prevTrack = evt.Channel;
            }

            for (int time = 0; time < stringOns.Count; time++)
            {
                var evt = stringOns[time];
                var dur = stringDurs[time];
                var note = new NoteOnOffMessage(outputDevice, striChannels.GetNext(), (Pitch)evt.Note, voller.GetNext(), evt.Time / div, clock, dur / div, panner.GetNext());
                clock.Schedule(note);
            }

            clock.Start();
            Thread.Sleep(ons.Count * (int)div);
            clock.Stop();
        }


        #endregion Methods
    }
}