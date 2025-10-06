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
    internal class bach_js_bwv1056_harpsichod_concerto_in_fm_2_largo_arioso : ExampleBase
    {
        #region Constructors

        public bach_js_bwv1056_harpsichod_concerto_in_fm_2_largo_arioso() : base(nameof(bach_js_bwv1056_harpsichod_concerto_in_fm_2_largo_arioso)) { }

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
            path = @".\midifiles\bach_js_bwv1056_harpsichod_concerto_in_fm_2_largo_(arioso).mid";

            Console.WriteLine(path);
            Console.WriteLine(seed);

            var leftInstr = new Enumerate<Instrument>(new[] {
                Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.Vibraphone, Instrument.ElectricPiano1,
                Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.ElectricPiano1, Instrument.Vibraphone,
                Instrument.SlapBass1, Instrument.Vibraphone, Instrument.ElectricPiano1, Instrument.ElectricPiano1,
            }, step: 1);
            var rightInstr = new Enumerate<Instrument>(new[] {
                Instrument.StringEnsemble1, Instrument.ElectricBassPick, Instrument.ElectricPiano1, Instrument.ElectricBassPick,
                Instrument.StringEnsemble1, Instrument.ElectricBassPick, Instrument.ElectricBassPick, Instrument.ElectricPiano1,
                Instrument.StringEnsemble1, Instrument.ElectricPiano1, Instrument.ElectricBassPick, Instrument.ElectricBassPick,
            }, step: 1);

            var file = new MidiFile(path);
            var div = (float)file.TicksPerQuarterNote;
            var chans = Channels.InstrumentChannels.AsEnumeration();

            var lChans = Channels.Range(Channel.Channel1, Channel.Channel9);
            var rChans = Channels.Range(Channel.Channel11, Channel.Channel16);
            var leftChans = new Enumerate<Channel>(lChans, step: 1);
            var rightChans = new Enumerate<Channel>(rChans, step: 1);

            foreach (var x in lChans)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 0);
                outputDevice.SendProgramChange(x, leftInstr.GetNext());
            }

            foreach (var x in rChans)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 100);
                outputDevice.SendControlChange(x, Control.Volume, 127);
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
                var ppoints = Interpolator.InOutCurve(0.40, 0.60, sIn, sOut, eIn[part], eOut[part]);
                var vpoints = Interpolator.InOutCurve(0.75, 0.90, sIn, sOut, eIn[part], eOut[part]);
                pcurve.AddRange(ppoints.Select(e => e * DeviceBase.ControlChangeMax));
                vcurve.AddRange(vpoints.Select(e => e * DeviceBase.ControlChangeMax));
            }
            var leftPan = pcurve.AsCycle();
            var leftVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic);

            var rightPan = new Enumerate<double>(pcurve, IncrementMethod.Cyclic, pcurve.Count / 2);
            var rightVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic, vcurve.Count / 2);

            var fractions = new Enumerate<float>(new[] { 1 / 4f, 1 / 8f, 1 / 16f }, IncrementMethod.Cyclic);
            var fractions2 = new Enumerate<float>(new[] { 1 / 8f, 1 / 16f, 1 / 32f }, IncrementMethod.Cyclic);
            
            var nEchoes = new Enumerate<float>(new[] { 1f, 2f, 1f, 4f, 1f, 8f, 1f, 16f }, IncrementMethod.Cyclic);
            var nEchoes2 = new Enumerate<float>(new[] { 1f, 2f, 1f, 4f, 1f, 8f, 1f, 16f }, IncrementMethod.Cyclic);
            
            var playEchoes = true;

            var notes = file.GetNotes(outputDevice, clock);

            var noteE = new Enumerate<NoteOnOffMessage>(notes, step: 1);
            for (var i = 0; i < notes.Count - 1; i++)
            {
                var note = noteE.GetNext();
                var next = noteE.Peek;

                if ((int)note.Pitch <= 48)
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
                clock.Schedule(note);

                var nEcho = i % 2 != 0 ? nEchoes.GetNext() : nEchoes2.GetNext();

                var leftP = new Enumerate<double>(leftPan, IncrementMethod.Cyclic);
                var rightP = new Enumerate<double>(rightPan, IncrementMethod.Cyclic);
                var leftV = new Enumerate<double>(leftVol, IncrementMethod.Cyclic);
                var rightV = new Enumerate<double>(rightVol, IncrementMethod.Cyclic);

                for (var P_ = 0; playEchoes && P_ <= 48 && (int)nEcho > 0 && note.Time != next.Time; P_ += 12)
                {
                    var minDur = (next.Time - note.Time) / nEcho;
                    var fract = i % 2 == 0 ? fractions.GetNext() : fractions2.GetNext();
                    var diff = next.Time - note.Time;
                    var p = note.Pitch + P_;
                    var j = i + 1;
                    for (var x = 0f; x < nEcho; x++)
                    {
                        var clone = note.Clone() as NoteOnOffMessage;
                        clone.Time += diff * (fract * x);

                        if (clone.Time > next.Time)
                            break;

                        clone.Pan = (note.Pan + rightP.GetNext()) / 2f;
                        clone.Channel = rightChans.GetNext();
                        clone.Velocity = rightV.GetNext() * (nEcho / (x + 1));
                        clone.Duration = Math.Max(clone.Duration, minDur);
                        clock.Schedule(clone);
                    }

                    //nEcho *= 0.75f;
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