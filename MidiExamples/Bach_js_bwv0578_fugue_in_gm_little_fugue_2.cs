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

            var leftInstr = new[] { Instrument.Vibraphone, Instrument.Vibraphone }.AsEnumeration();
            var rightInstr = new[] { Instrument.Vibraphone, Instrument.Vibraphone, Instrument.Vibraphone }.AsEnumeration();

            var file = new MidiFile(path);
            var div = (float)file.TicksPerQuarterNote;
            var chans = Channels.InstrumentChannels.AsEnumeration();

            var leftChans = Channels.Range(Channel.Channel1, Channel.Channel9).AsEnumeration();
            foreach (var x in leftChans)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 0);
                outputDevice.SendProgramChange(x, leftInstr.GetNext());
            }
            var rightChans = Channels.Range(Channel.Channel11, Channel.Channel16).AsEnumeration();
            foreach (var x in rightChans)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 127);
                outputDevice.SendControlChange(x, Control.Volume, 100);
                outputDevice.SendProgramChange(x, rightInstr.GetNext());
            }

            clock = new Clock(BPM);

            var steps = new[] { 1, 2, 4, 8, 16 }.AsEnumeration();
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
            var leftPan = pcurve.AsCycle();
            var leftVol = vcurve.Select(v => v * 0.75).AsCycle();

            var rightPan = pcurve.Select(p => 127 - p).AsCycle();
            var rightVol = vcurve.AsCycle();

            var fractions = new[] { 1 / 4f, 1 / 8f, 1 / 16f, 1 / 32f, 1 / 64f }.AsEnumeration();
            var fractions2 = fractions.AsReversed();
            var nEchoes = new[] { 2f, 4f, 8f }.AsEnumeration();
            var nEchoes2 = nEchoes.Scale(1/3f).AsCycle();
            var playEchoes = true;

            var notes = file.GetNotes(outputDevice, clock);
            var notes2 = file.GetNotes(outputDevice, clock);

            for (var x = 1; x <= 12; x++)
            {
                var delay = x % 2 == 0 ? fractions.GetNext() : fractions2.GetNext();
                notes.AddRange
                (
                    notes2.Select(n => ((NoteOnOffMessage)n.MakeTimeShiftedCopy(x * delay)))
                );
            }

            var noteE = notes.AsEnumeration();
            for (var i = 0; i < notes.Count - 1; i++)
            {
                var note = noteE.GetNext();
                var next = noteE.Peek;
                if (i % 2 == 0)
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

                var nEcho = i % 3 != 0 ? nEchoes.GetNext() : nEchoes2.GetNext();
                if (playEchoes && nEcho > 0 && note.Pitch >= Pitch.C3 && note.Time != next.Time)
                {
                    var leftC = leftPan.AsCycle(step: 2);
                    var rightC = rightPan.AsCycle(step: 2);
                    var leftV = leftVol.AsCycle(step: 2);
                    var rightV = rightVol.AsCycle(step: 2);
                    var minDur = (next.Time - note.Time) / nEcho;
                    var fract = i % 2 == 0 ? fractions.GetNext() : fractions2.GetNext();
                    var diff = next.Time - note.Time;
                    var p = note.Pitch;
                    var j = i + 1;
                    for (var x = 0f; x < nEcho; x++)
                    {
                        var clone = note.Clone() as NoteOnOffMessage;
                        clone.Time += diff * (fract * x);

                        if (clone.Time > next.Time)
                            break;

                        clone.Pan = (note.Pan + rightC.GetNext()) / 2f;
                        clone.Channel = rightChans.GetNext();
                        clone.Velocity = rightV.GetNext() * (nEcho / (x + 1));
                        clone.Duration = Math.Max(clone.Duration, minDur);
                        if (j++ % 2 == 0)
                        {
                            clone.Pitch = p.OctaveAbove();
                        }
                        if (j % 3 == 0)
                        {
                            clone.Pitch = p.OctaveAbove();
                            clone.Pan = 127f - ((note.Pan + rightC.GetNext()) / 2f);
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