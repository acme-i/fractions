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
    internal class bach_js_bwv0775_two_part_invention_04 : ExampleBase
    {
        #region Constructors

        public bach_js_bwv0775_two_part_invention_04() : base(nameof(bach_js_bwv0775_two_part_invention_04)) { }

        #endregion Constructors

        #region Methods

        private IOutputDevice outputDevice;
        private Clock clock;
        private readonly int BPM = 48;

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
            path = @".\midifiles\bach_js_bwv0775_two-part_invention_04.mid";

            Console.WriteLine(path);
            Console.WriteLine(seed);

            var leftInstr = new Enumerate<Instrument>(new[] { Instrument.Vibraphone, Instrument.Vibraphone }.Reverse());
            var rightInstr = new Enumerate<Instrument>(new[] { Instrument.Vibraphone, Instrument.Vibraphone });

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

            var steps = new Enumerate<int>(new[] { 1, 3, 6, 12, 24 }, IncrementMethod.Cyclic);
            var pcurve = new List<double>();
            var vcurve = new List<double>();
            var eIn = Interpolator.EaseInFunctions();
            var eOut = Interpolator.EaseOutFunctions();
            for (var part = 0; part < eIn.Count; part++)
            {
                var sIn = steps.Next();
                var sOut = sIn; // steps.Next();
                var ppoints = Interpolator.InOutCurve(0.10, 0.90, sIn, sOut, eIn[part], eOut[part]);
                var vpoints = Interpolator.InOutCurve(0.90, 1.00, sIn, sOut, eIn[part], eOut[part]);
                pcurve.AddRange(ppoints.Select(e => e * 127));
                vcurve.AddRange(vpoints.Select(e => e * 120));
            }

            var leftPan = new Enumerate<double>(pcurve, IncrementMethod.Cyclic);
            var leftVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic);

            var rightPan = new Enumerate<double>(pcurve.Select(p => 127f - p), IncrementMethod.Cyclic);
            var rightVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic);

            var notes = file.GetNotes(outputDevice, clock);

            var noteE = new Enumerate<NoteOnOffMessage>(notes);

            var enumes = new Enumerate<float>(new[] { 32f, 64f, 128f }, IncrementMethod.Cyclic);
            var enumes2 = new Enumerate<float>(new[] { 16f, 32f, 64f, 128f }.Reverse(), IncrementMethod.Cyclic, 3);

            var enumValues = new[] { 0.25f, 0.5f, 1f, 2f, 3f, 4f, 6f, 12f, 16f, 8f, 4f, 3f, 2f, 1f, 0.5f };

            var denoms = new Enumerate<float>(enumValues, IncrementMethod.Cyclic);
            var denoms2 = new Enumerate<float>(enumValues.Select(e => e * 3.33f), IncrementMethod.Cyclic);

            var fs = new Enumerate<List<Fraction>>(new[] { Fractions.Thirds, Fractions.Fourths, Fractions.Sixths, Fractions.Eighths, Fractions.Twelveths, Fractions.Sixteenths, Fractions.TwentyFourths, Fractions.ThirtySeconds }, IncrementMethod.Cyclic);

            for (var i = 0; i < notes.Count - 1; i++)
            {
                var note = noteE.Next();
                var next = noteE.Peek();

                note.Pitch = note.Pitch.OctaveAbove();

                if (note.Pitch <= Pitch.C3)
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

                var denom = i % 2 == 0 ? denoms.Next() : denoms2.Next();
                var enumerator = i % 2 == 0 ? enumes.Next() : enumes2.Next();
                var nEchoes = new Enumerate<float>(Fractions.Fourths.Select(f => enumerator / f.Over(denom)), IncrementMethod.Cyclic);
                var nEchoes2 = new Enumerate<float>(Fractions.Fourths.Select(f => enumerator / f.Over(denom)).Reverse(), IncrementMethod.Cyclic);

                var nEcho = i % 2 != 0 ? nEchoes.Next() : nEchoes2.Next();
                if (note.Time != next.Time)
                {
                    var leftC = new Enumerate<double>(leftPan, IncrementMethod.Cyclic);
                    var rightC = new Enumerate<double>(rightPan, IncrementMethod.Cyclic);
                    var leftV = new Enumerate<double>(leftVol, IncrementMethod.Cyclic);
                    var rightV = new Enumerate<double>(rightVol, IncrementMethod.Cyclic);
                    var minDur = (next.Time - note.Time) / nEcho;
                    var fss = fs.Next();
                    var fractions = new Enumerate<float>(fss.Select(f => f.Over(nEcho)), IncrementMethod.Cyclic);
                    var fractions2 = new Enumerate<float>(fss.Select(f => f.Over(nEcho)).Reverse(), IncrementMethod.Cyclic);
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

                        clone.Pan = rightC.Next();
                        clone.Channel = rightChans.Next();
                        clone.Velocity = rightV.Next();
                        clone.Duration = Math.Max(clone.Duration, minDur);

                        if (j++ % 2 == 0)
                        {
                            clone.Pitch = p.OctaveAbove();
                            clone.Pan = clone.Pan = rightC.Next();
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