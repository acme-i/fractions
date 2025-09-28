using System;
using System.Collections.Generic;
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
    internal class bach_js_bwv1067_orchestral_suite_2_in_bm_5_polonaise_double_2 : ExampleBase
    {
        #region Constructors

        public bach_js_bwv1067_orchestral_suite_2_in_bm_5_polonaise_double_2()
            : base(nameof(bach_js_bwv1067_orchestral_suite_2_in_bm_5_polonaise_double_2))
        {
        }

        #endregion Constructors

        #region Methods

        private IOutputDevice outputDevice;
        private Clock clock;
        private readonly int BPM = 20;

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

            Goahead(299040);

            // Close the output device.
            outputDevice.Close();

            // All done.
            Console.WriteLine();
            ExampleUtil.PressAnyKeyToContinue();
        }

        private void Goahead(int seed)
        {
            //var seed = 299041; // 299040;
            Interpolator.Seed = seed;
            var rand = new Random(seed);

            //string[] files = Directory.GetFiles(@".\midifiles\", "*.mid");
            //string path = files[rand.Next(files.Count())];
            var path = @".\midifiles\bach_js_bwv1067_orchestral_suite_2_in_bm_5_polonaise_double.mid";

            Console.WriteLine(path);
            Console.WriteLine(seed);

            var leftInstr = new Enumerate<Instrument>(new[] { Instrument.ElectricBassFinger, Instrument.AcousticBass, Instrument.SlapBass1 }, step: 1);
            var rightInstr = new Enumerate<Instrument>(new[] { Instrument.ElectricPiano1, Instrument.ElectricPiano1, Instrument.ElectricPiano1, Instrument.ElectricPiano2 }, step: 1);

            var file = new MidiFile(path);
            var div = (float)file.TicksPerQuarterNote;
            var chans = new Enumerate<Channel>(Channels.InstrumentChannels, step: 1);

            var lChans = Channels.Range(Channel.Channel1, Channel.Channel9);
            var rChans = Channels.Range(Channel.Channel11, Channel.Channel16);
            var leftChans = new Enumerate<Channel>(lChans, step: 1);
            var rightChans = new Enumerate<Channel>(rChans, step: 1);

            foreach (var x in lChans)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 0);
                outputDevice.SendControlChange(x, Control.Volume, DeviceBase.ControlChangeMax);
                outputDevice.SendProgramChange(x, leftInstr.Next());
            }

            foreach (var x in rChans)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 100);
                outputDevice.SendControlChange(x, Control.Volume, DeviceBase.ControlChangeMax);
                outputDevice.SendProgramChange(x, rightInstr.Next());
            }

            clock = new Clock(BPM);

            var stepsIn = new Enumerate<int>(new[] { 1, 2, 4, 8, 16 }, IncrementMethod.Cyclic);
            var stepsOut = new Enumerate<int>(new[] { 1, 2, 4, 8, 16 }.Reverse(), IncrementMethod.Cyclic);
            var maxEchos1 = new Enumerate<int>(new[] { 8, 4, 16, 64, 32, 128 }.OrderBy(i => i).Reverse(), IncrementMethod.Cyclic);
            var maxEchos2 = new Enumerate<int>(new[] { 9, 6, 48, 24, 12, 96 }.OrderBy(i => i).Reverse(), IncrementMethod.Cyclic);

            var pcurve = new List<double>();
            var vcurve = new List<double>();
            var efs = Interpolator.EaseFunctions();
            foreach ((EaseFunction easeIn, EaseFunction easeOut) in efs)
            {
                var sIn = stepsIn.Next();
                var sOut = stepsOut.Next();
                var ppoints = Interpolator.InOutCurve(rand.Next(10, 50)/100f, rand.Next(51, 90) / 100f, sIn, sOut, easeIn, easeOut);
                var vpoints = Interpolator.InOutCurve(rand.Next(50, 70) / 100f, rand.Next(71, 90) / 100f, sIn, sOut, easeIn, easeOut);
                pcurve.AddRange(ppoints.Select(e => e * DeviceBase.ControlChangeMax));
                vcurve.AddRange(vpoints.Select(e => e * DeviceBase.ControlChangeMax));
            }

            var leftPan = new Enumerate<double>(pcurve, IncrementMethod.Cyclic, 14);
            var leftVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic, 1);

            var rightPan = new Enumerate<double>(pcurve.Reverse<double>(), IncrementMethod.Cyclic, 14);
            var rightVol = new Enumerate<double>(vcurve.Reverse<double>(), IncrementMethod.Cyclic, 1);

            var notes = file.GetNotes(outputDevice, clock);
            var newNotes = new List<NoteOnOffMessage>(notes.Count * 32);

            var noteE = new Enumerate<NoteOnOffMessage>(notes);

            var enumes = new Enumerate<float>(new[] { 4f, 8f, 16f, 32f, 64f, 128f, 64f, 32f, 8f }, IncrementMethod.Cyclic);
            var enumes2 = new Enumerate<float>(new[] { 16f, 32f, 64f, 128f }.Reverse(), IncrementMethod.Cyclic);

            var temp = new List<float> { 0.25f, 0.5f, 1f, 2f, 3f, 4f, 6f, 12f, 16f, 8f, 4f, 3f, 2f, 1f, 0.5f };
            temp.AddRange(temp.ToList().Select(t => t + 1 / 3f));
            temp.AddRange(temp.ToList().Select(t => t + 1 / 4f));
            temp.AddRange(temp.ToList().Select(t => t + 1 / 6f));
            temp.AddRange(temp.ToList().Select(t => t + 1 / 8f));
            temp.AddRange(temp.ToList().Select(t => t + 1 / 12f));
            temp.AddRange(temp.ToList().Select(t => t + 1 / 16f));
            temp.AddRange(temp.ToList().Select(t => t + 1 / 24f));
            temp.AddRange(temp.ToList().Select(t => t + 1 / 32f));
            temp.AddRange(temp.ToList().Select(t => t + 1 / 48f));
            temp.AddRange(temp.ToList().Select(t => t + 1 / 64f));
            temp.AddRange(temp.ToList().Select(t => t + 1 / 96f));
            temp.AddRange(temp.ToList().Select(t => t + 1 / 128f));

            //temp = temp.Distinct().ToList();
            temp.Sort();
            temp = temp.Distinct().ToList();

            var temp2 = temp.Reverse<float>().ToList();

            var denoms = new Enumerate<float>(temp, IncrementMethod.Cyclic);
            var denoms2 = new Enumerate<float>(temp2, IncrementMethod.Cyclic);
            var leftFractionLists = new Enumerate<List<Fraction>>(
                new[] { Fractions.Fourths, Fractions.Eighths, Fractions.Eighths, Fractions.Sixteenths }
            );
            var rightFractionLists = new Enumerate<List<Fraction>>(
                new[] { Fractions.Sixteenths, Fractions.Eighths, Fractions.Eighths, Fractions.Fourths }
            );

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

                var denom = i % 2 == 0 ? denoms.Next() : denoms2.Next();
                var enumerator = i % 2 == 0 ? enumes.Next() : enumes2.Next();
                var maxEchos = i % 2 == 0 ? maxEchos1.Next() : maxEchos2.Next();
                var nEchoes = new Enumerate<float>(leftFractionLists.Next().Select(f => enumerator / f.Over(denom)), IncrementMethod.MinMax);
                var nEchoes2 = new Enumerate<float>(rightFractionLists.Next().Select(f => enumerator / f.Over(denom)), IncrementMethod.MaxMin);

                var mods = new Enumerate<int>(new[] { 2, 3, 5, 7, 11, 13 }, IncrementMethod.MinMax);

                var nEcho = i % 2 != 0 ? nEchoes.Next() : nEchoes2.Next();

                if (nEcho > 0 && note.Time != next.Time)
                {
                    var leftP = leftPan.Clone();
                    var rightP = rightPan.Clone();
                    var leftV = leftVol.Clone();
                    var rightV = rightVol.Clone();
                    var minDur = (next.Time - note.Time) / nEcho;
                    var fractions = new Enumerate<float>(leftFractionLists.Next().Select(f => f.Over(nEcho)), IncrementMethod.Cyclic);
                    var fractions2 = new Enumerate<float>(rightFractionLists.Next().Select(f => f.Over(nEcho)), IncrementMethod.Cyclic);
                    var fract = i % 2 == 0 ? fractions.Next() : fractions2.Next();

                    if (i % 2 == 0)
                        fractions2.Next();
                    else
                        fractions.Next();

                    var diff = next.Time - note.Time;
                    var p = note.Pitch;
                    var j = i + 1;

                    nEcho = Math.Min((int)Math.Ceiling(nEcho), maxEchos);

                    for (var x = 1f; x <= nEcho; x++)
                    {
                        var clone = note.Clone() as NoteOnOffMessage;
                        clone.Time += diff * (fract * x);

                        if (clone.Time > next.Time)
                            break;

                        clone.Pan = rightP.Next();
                        clone.Channel = rightChans.Next();
                        clone.Velocity = rightV.Next();
                        clone.Reverb = 127-clone.Velocity;
                        clone.Duration = Math.Max(clone.Duration, minDur);

                        if (j++ % mods.Next() == 0)
                        {
                            clone.Pitch = p.OctaveAbove();
                        }

                        newNotes.Add(clone);
                    }
                }
            }

            newNotes.AddRange(notes);

            clock.Schedule(newNotes);

            clock.Start();
            Thread.Sleep(400000);
            clock.Stop();
        }

        #endregion Methods
    }
}