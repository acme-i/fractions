﻿using System;
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
    internal class bach_js_bwv0578_fugue_in_gm_little_fugue_2 : ExampleBase
    {
        #region Constructors

        public bach_js_bwv0578_fugue_in_gm_little_fugue_2() : base(nameof(bach_js_bwv0578_fugue_in_gm_little_fugue_2)) { }

        #endregion Constructors

        #region Methods

        private IOutputDevice outputDevice;
        private Clock clock;
        private readonly int BPM = 64;

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
            var seed = 299040; // 200; // 2000;
            Interpolator.Seed = seed;
            var rand = new Random(seed);

            var files = Directory.GetFiles(@".\midifiles\", "*.mid");
            var path = files[rand.Next(files.Count())];
            path = @".\midifiles\bach_js_bwv0578_fugue_in_gm_little_fugue.mid";
            //path = @".\midifiles\bach_js_bwv0996_lute_suite_in_em_3_courante.mid";

            Console.WriteLine(path);
            Console.WriteLine(seed);

            var leftInstr = new Enumerate<Instrument>(new[] { Instrument.Vibraphone, Instrument.Vibraphone });
            var rightInstr = new Enumerate<Instrument>(new[] { Instrument.Vibraphone, Instrument.Vibraphone, Instrument.Vibraphone });

            var file = new MidiFile(path);
            var div = (float)file.TicksPerQuarterNote;
            var chans = new Enumerate<Channel>(Channels.InstrumentChannels);

            var lChans = Channels.Range(Channel.Channel1, Channel.Channel9);
            var rChans = Channels.Range(Channel.Channel11, Channel.Channel16);
            var leftChans = new Enumerate<Channel>(lChans);
            var rightChans = new Enumerate<Channel>(rChans);

            foreach (var x in lChans)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 0);
                outputDevice.SendProgramChange(x, leftInstr.Next());
            }

            foreach (var x in rChans)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 127);
                outputDevice.SendControlChange(x, Control.Volume, 100);
                outputDevice.SendProgramChange(x, rightInstr.Next());
            }

            clock = new Clock(BPM);

            var steps = new Enumerate<int>(new[] { 1, 2, 4, 8, 16 });
            var pcurve = new List<double>();
            var vcurve = new List<double>();
            var eIn = Interpolator.EaseInFunctions();
            var eOut = Interpolator.EaseOutFunctions();
            for (var part = 0; part < eIn.Count; part++)
            {
                var sIn = steps.Next();
                var sOut = steps.Next();
                var ppoints = Interpolator.InOutCurve(0.10, 0.90, sIn, sOut, eIn[part], eOut[part]);
                var vpoints = Interpolator.InOutCurve(0.80, 1.00, sIn, sOut, eIn[part], eOut[part]);
                pcurve.AddRange(ppoints.Select(e => e * 127));
                vcurve.AddRange(vpoints.Select(e => e * 120));
            }
            var leftPan = new Enumerate<double>(pcurve, IncrementMethod.Cyclic);
            var leftVol = new Enumerate<double>(vcurve.Select(v => v * 0.75), IncrementMethod.Cyclic);

            var rightPan = new Enumerate<double>(pcurve.Select(p => 127 - p), IncrementMethod.Cyclic);
            var rightVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic);

            var fractions = new Enumerate<float>(new[] { 1 / 4f, 1 / 8f, 1 / 16f, 1 / 32f, 1 / 64f });
            var fractions2 = new Enumerate<float>(new[] { 1 / 4f, 1 / 8f, 1 / 16f, 1 / 32f, 1 / 64f }.Reverse());
            var nEchoes = new Enumerate<float>(new[] { 2f, 4f, 8f }, IncrementMethod.Cyclic);
            var nEchoes2 = new Enumerate<float>(new[] { 2f * 1 / 3f, 4f * 1 / 3f, 8f * 1 / 3f }, IncrementMethod.Cyclic);
            var playEchoes = true;

            var notes = file.GetNotes(outputDevice, clock);
            var notes2 = file.GetNotes(outputDevice, clock);

            for (var x = 1; x <= 12; x++)
            {
                var delay = x % 2 == 0 ? fractions.Next() : fractions2.Next();

                var delayed = notes2.Select(n => ((NoteOnOffMessage)n.MakeTimeShiftedCopy(x * delay)));
                notes.AddRange(delayed);
            }

            var noteE = new Enumerate<NoteOnOffMessage>(notes);
            for (var i = 0; i < notes.Count - 1; i++)
            {
                var note = noteE.Next();
                var next = noteE.Peek();

                if (i % 2 == 0)
                {
                    note.Channel = leftChans.Next();
                    note.Pan = leftPan.Next();
                    note.Velocity = leftVol.Next();
                }
                else
                {
                    note.Channel = rightChans.Next();
                    note.Pan = rightPan.Next();
                    note.Velocity = rightVol.Next();
                }

                var nEcho = i % 3 != 0 ? nEchoes.Next() : nEchoes2.Next();
                if (playEchoes && nEcho > 0 && note.Pitch >= (Pitch)Pitch.C3 && note.Time != next.Time)
                {
                    var leftC = new Enumerate<double>(leftPan, IncrementMethod.Cyclic, 2);
                    var rightC = new Enumerate<double>(rightPan, IncrementMethod.Cyclic, 2);
                    var leftV = new Enumerate<double>(leftVol, IncrementMethod.Cyclic, 2);
                    var rightV = new Enumerate<double>(rightVol, IncrementMethod.Cyclic, 2);
                    var minDur = (next.Time - note.Time) / nEcho;
                    var fract = i % 2 == 0 ? fractions.Next() : fractions2.Next();
                    var diff = next.Time - note.Time;
                    var p = note.Pitch;
                    var j = i + 1;
                    for (var x = 0f; x < nEcho; x++)
                    {
                        var clone = note.Clone() as NoteOnOffMessage;
                        clone.Time += diff * (fract * x);

                        if (clone.Time > next.Time)
                            break;

                        clone.Pan = (note.Pan + rightC.Next()) / 2f;
                        clone.Channel = rightChans.Next();
                        clone.Velocity = rightV.Next() * (nEcho / (x + 1));
                        clone.Duration = Math.Max(clone.Duration, minDur);
                        if (j++ % 2 == 0)
                        {
                            clone.Pitch = p.OctaveAbove();
                        }
                        if (j % 3 == 0)
                        {
                            clone.Pitch = p.OctaveAbove();
                            clone.Pan = 127f - ((note.Pan + rightC.Next()) / 2f);
                        }
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