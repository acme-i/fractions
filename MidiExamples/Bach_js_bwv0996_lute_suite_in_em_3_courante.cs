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
using System.IO;
using System.Linq;
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
    internal class bach_js_bwv0996_lute_suite_in_em_3_courante : ExampleBase
    {
        #region Constructors

        public bach_js_bwv0996_lute_suite_in_em_3_courante() : base(nameof(bach_js_bwv0996_lute_suite_in_em_3_courante)) { }

        #endregion Constructors

        #region Methods

        private IOutputDevice outputDevice;
        private Clock clock;
        private readonly int BPM = 180;

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

            Goahead();

            // Close the output device.
            outputDevice.Close();

            // All done.
            Console.WriteLine();
            ExampleUtil.PressAnyKeyToContinue();
        }

        private void Goahead()
        {
            var seed = 228480; // DateTime.Now.Second * DateTime.Now.Minute * DateTime.Now.Millisecond;
            Interpolator.Seed = seed;
            var rand = new Random(seed);

            var files = Directory.GetFiles(@".\midifiles\", "*.mid");
            var path = files[rand.Next(files.Count())];
            //path = @".\midifiles\bach_js_bwv0578_fugue_in_gm_little_fugue.mid";
            path = @".\midifiles\bach_js_bwv0996_lute_suite_in_em_3_courante.mid";

            Console.WriteLine(path);
            Console.WriteLine(seed);

            var instrList = new Enumerate<Instrument>(new[] { Instrument.Vibraphone, Instrument.Vibraphone }, step: 1);

            var file = new MidiFile(path);
            var div = (float)file.TicksPerQuarterNote;
            var chans = new Enumerate<Channel>(Channels.InstrumentChannels, step: 1);

            var lChans = Channels.Range(Channel.Channel1, Channel.Channel9);
            var rChans = Channels.Range(Channel.Channel11, Channel.Channel16);
            var leftChans = new Enumerate<Channel>(lChans, step: 1);
            var rightChans = new Enumerate<Channel>(rChans, step: 1);

            foreach (var x in Channels.InstrumentChannels)
                outputDevice.SendProgramChange(x, instrList.GetNext());

            foreach (var x in lChans)
                outputDevice.SendControlChange(x, Control.ReverbLevel, 0);

            foreach (var x in rChans)
                outputDevice.SendControlChange(x, Control.ReverbLevel, 127);

            clock = new Clock(BPM);

            var steps = new Enumerate<int>(new[] { 1, 2, 4, 8, 16 }, IncrementMethod.MinMax, 1);
            var pcurve = new List<double>();
            var vcurve = new List<double>();
            var eIn = Interpolator.EaseInFunctions();
            var eOut = Interpolator.EaseOutFunctions();
            for (var part = 0; part < eIn.Count; part++)
            {
                var sIn = steps.GetNext();
                var sOut = steps.GetNext();
                var ppoints = Interpolator.InOutCurve(0.10, 0.90, sIn, sOut, eIn[part], eOut[part]);
                var vpoints = Interpolator.InOutCurve(0.80, 1.00, sIn, sOut, eIn[part], eOut[part]);
                pcurve.AddRange(ppoints.Select(e => e * 127));
                vcurve.AddRange(vpoints.Select(e => e * 120));
            }
            var leftPan = new Enumerate<double>(pcurve, IncrementMethod.Cyclic, 6);
            var leftVol = new Enumerate<double>(vcurve.Select(v => v * 0.75), IncrementMethod.Cyclic);

            var rightPan = new Enumerate<double>(pcurve.Select(p => 127 - p), IncrementMethod.Cyclic, 6);
            var rightVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic);
            var fractions = new Enumerate<float>(new[] { 1 / 2f, 1 / 3f, 1 / 4f, 1 / 8f, 1 / 12f, 1 / 16f, 1 / 24f, 1 / 32f }, IncrementMethod.MinMax, 3);
            var nEchoes = new Enumerate<float>(new[] { 0f, 2f, 4f, 8f, 16f, 32f }, IncrementMethod.Cyclic);
            var playEchoes = true;

            var notes = file.GetNotes(outputDevice, clock);
            var noteE = new Enumerate<NoteOnOffMessage>(notes, step: 1);
            for (var i = 0; i < notes.Count - 1; i++)
            {
                var note = noteE.GetNext();
                var next = noteE.Peek;
                
                if (note.Pitch < Pitch.C3)
                {
                    note.Channel = leftChans.GetNext();
                    note.Pan = leftPan.GetNext();
                    note.Velocity = leftVol.GetNext();
                }
                else
                {
                    note.Channel = rightChans.GetNext();
                    note.Pan = rightPan.GetNext();
                    note.Velocity = rightVol.GetNext();
                }

                var nEcho = nEchoes.GetNext();
                if (playEchoes && nEcho > 0 && note.Pitch > (Pitch.C4 - 3) && note.Time != next.Time)
                {
                    var leftC = new Enumerate<double>(leftPan, IncrementMethod.Cyclic, 2);
                    var rightC = new Enumerate<double>(rightPan, IncrementMethod.Cyclic, 2);
                    var leftV = new Enumerate<double>(leftVol, IncrementMethod.Cyclic, 2);
                    var rightV = new Enumerate<double>(rightVol, IncrementMethod.Cyclic, 2);
                    var minDur = (next.Time - note.Time) / nEcho;
                    var j = i + 1;
                    var fract = fractions.GetNext();
                    var diff = next.Time - note.Time;
                    var p = note.Pitch;
                    for (var x = 0f; x < nEcho; x++)
                    {
                        var clone = note.Clone() as NoteOnOffMessage;
                        clone.Time += diff * (fract * x);

                        if (clone.Time > (next.Time + fract))
                            break;

                        if (j % 2 == 0)
                        {
                            clone.Channel = leftChans.GetNext();
                            clone.Pitch = p.OctaveAbove();
                            clone.Pan = (note.Pan + leftC.GetNext()) / 2f;
                            clone.Velocity = leftV.GetNext() * nEcho / (1.125f + x);
                        }
                        else
                        {
                            clone.Channel = rightChans.GetNext();
                            clone.Pitch = p.OctaveAbove().OctaveAbove();
                            clone.Pan = (note.Pan + rightC.GetNext()) / 2f;
                            clone.Velocity = rightV.GetNext() * nEcho / (1.125f + x);
                        }
                        j++;
                        clone.Duration = Math.Max(clone.Duration, minDur);
                        clock.Schedule(clone);
                    }
                }
            }
            clock.Schedule(notes);

            clock.Start();
            Thread.Sleep(400000);
            clock.Stop();
        }

        #endregion Methods
    }
}