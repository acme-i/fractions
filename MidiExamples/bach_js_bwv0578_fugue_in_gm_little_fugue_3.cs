using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
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
    internal class bach_js_bwv0578_fugue_in_gm_little_fugue_3 : ExampleBase
    {
        #region Constructors

        public bach_js_bwv0578_fugue_in_gm_little_fugue_3() : base(nameof(bach_js_bwv0578_fugue_in_gm_little_fugue_3)) { }

        #endregion Constructors

        #region Methods

        private IOutputDevice outputDevice;
        private Clock clock;
        private readonly int BPM = 16;

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
            var path = @".\midifiles\bach_js_bwv0578_fugue_in_gm_little_fugue.mid";

            Console.WriteLine(path);
            Console.WriteLine(seed);

            var leftInstr = new[] {
                Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.Vibraphone, Instrument.ElectricPiano1,
                Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.ElectricPiano1, Instrument.Vibraphone,
                Instrument.SlapBass1, Instrument.Vibraphone, Instrument.ElectricPiano1, Instrument.ElectricPiano1,
            }.AsEnumeration(method: IncrementMethod.MinMax);
            var rightInstr = leftInstr.AsEnumeration(startIndex: 5);

            var echoLeftInstr = new[] {
                Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.Vibraphone, Instrument.ElectricPiano1,
                Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.ElectricPiano1, Instrument.Vibraphone,
                Instrument.SlapBass1, Instrument.Vibraphone, Instrument.ElectricPiano1, Instrument.ElectricPiano1,
            }.AsEnumeration(method: IncrementMethod.MaxMin);
            var echoRightInstr = echoLeftInstr.AsEnumeration(startIndex: 5);

            var file = new MidiFile(path);
            var div = (float)file.TicksPerQuarterNote;
            var chans = Channels.InstrumentChannels.AsEnumeration();

            var melodyChannels = Channels.Range(Channel.Channel1, Channel.Channel9).AsEnumeration();
            foreach (var x in melodyChannels)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 0);
                outputDevice.SendControlChange(x, Control.Volume, 100);
                outputDevice.SendProgramChange(x, leftInstr.GetNext());
            }
            var echoChannels = Channels.Range(Channel.Channel11, Channel.Channel16).AsEnumeration();
            foreach (var x in echoChannels)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 127);
                outputDevice.SendControlChange(x, Control.Volume, 100);
                outputDevice.SendProgramChange(x, rightInstr.GetNext());
            }

            clock = new Clock(BPM);

            var steps = new[] { 1, 2, 4, 8, 16 }.AsEnumeration();
            var steps2 = steps.AsReversed();
            steps2.GetNext();
            var pcurve = new List<double>();
            var vcurve = new List<double>();
            var eIn = Interpolator.EaseInFunctions();
            var eOut = Interpolator.EaseOutFunctions();
            for (var part = 0; part < eIn.Count; part++)
            {
                var sIn = steps.GetNext();
                var sOut = steps2.GetNext();
                var ppoints = Interpolator.InOutCurve(0.10, 0.90, sIn, sOut, eIn[part], eOut[part]);
                var vpoints = Interpolator.InOutCurve(0.10, 0.90, sIn, sOut, eIn[part], eOut[part]);

                pcurve.AddRange(ppoints.Select(e => e * 80));
                pcurve.AddRange(ppoints.Select(e => e * 80).Reverse());

                vcurve.AddRange(vpoints.Select(e => e * 80));
                vcurve.AddRange(vpoints.Select(e => e * 80).Reverse());
            }

            var echoLeft = pcurve.AsCycle();
            var echoRight = pcurve.AsCycle(startIndex: pcurve.Count / 2);

            var echoLeft2 = echoLeft.AsCycle(startIndex: pcurve.Count / 4);
            var echoRight2 = echoLeft2.AsReversed();

            var echoLeft3 = echoLeft.AsCycle(startIndex: pcurve.Count / 8);
            var echoRight3 = echoLeft3.AsReversed();

            var leftPan = pcurve.AsCycle();
            var leftVol = vcurve.Select(v => v * 0.75).AsCycle();

            var rightPan = pcurve.Select(p => 127 - p).AsCycle();
            var rightVol = vcurve.AsCycle();

            var fractions = new[] {
                1 / 2f,
                1 / 4f,
                1 / 8f,
                1 / 16f,
                1 / 32f,
                1 / 48f,
                1 / 64f,
                1 / 128f
            }.AsEnumeration();

            var fractions2 = fractions.AsReversed();
            var nEchoes = new[] { 2f, 4f, 16f, 32f }.AsEnumeration();
            var playEchoes = true;

            var notes = file.GetNotes(outputDevice, clock);
            var notes2 = file.GetNotes(outputDevice, clock);
            var notes3 = file.GetNotes(outputDevice, clock);

            var counter = 1;
            var counter2 = 2;
            var modCounter = 1;
            var pitches1 = new[] { Pitch.C1, Pitch.C2, Pitch.C3, Pitch.C4 }.AsCycle();
            var pitches2 = new[] { Pitch.C3, Pitch.C4, Pitch.C2, Pitch.C1 }.AsCycle();
            var mod1 = new[] { 1, 2, 3, 4, 5, 6 }.AsCycle();
            var mod2 = new[] { 1, 2, 3, 4, 5, 6 }.AsCycle(startIndex: 4);
            for (var x = 1; x <= 8; x++)
            {
                var delay = x % 2 == 0 ? fractions.GetNext() : fractions2.GetNext();
                var temp = notes2
                    .Select(n => ((NoteOnOffMessage)n.MakeTimeShiftedCopy(x * delay)))
                    .ToList();

                temp.ForEach(e =>
                {
                    var p1 = pitches1.GetNext();
                    var p2 = pitches2.GetNext();
                    var m1 = mod1.GetNext();
                    var m2 = mod2.GetNext();
                    var c1 = counter++;
                    var c2 = counter2++;

                    if (modCounter++ % 2 == 0)
                    {
                        if (e.Pitch < p1 && c1 % m1 == 0)
                        {
                            e.SetOctaveAbove();
                        }
                        if (e.Pitch < p2 && c2 % m2 == 0)
                        {
                            e.SetOctaveAbove();
                        }
                    }
                    else
                    {
                        if (e.Pitch < p1 && c1 % m1 == 0)
                        {
                            e.SetOctaveBelow();
                        }
                        if (e.Pitch < p2 && c2 % m2 == 0)
                        {
                            e.SetOctaveBelow();
                        }
                    }
                });

                notes.AddRange(temp);
            }

            var noteE = notes.AsEnumeration();

            for (var i = 0; i < notes.Count - 1; i++)
            {
                var note = noteE.GetNext();
                var next = noteE.Peek;

                if (next.Time < note.Time)
                    continue;

                note.Channel = melodyChannels.GetNext();
                if (i % 2 == 0)
                {
                    note.Pan = leftPan.GetNext();
                    note.Velocity = leftVol.GetNext();
                    note.BeforeSendingNoteOnOff += n =>
                    {
                        outputDevice.SendProgramChange(note.Channel, leftInstr.GetNext());
                    };
                }
                else
                {
                    note.Pan = rightPan.GetNext();
                    note.Velocity = rightVol.GetNext();
                    note.BeforeSendingNoteOnOff += n =>
                    {
                        outputDevice.SendProgramChange(note.Channel, rightInstr.GetNext());
                    };
                }

                var nEchoes2 = nEchoes.Clone().Scale(1 / nEchoes.Peek);
                var nEcho = i % 3 != 0 ? nEchoes.GetNext() : nEchoes2.GetNext();
                if (playEchoes && nEcho > 0 && note.Pitch >= Pitch.C3 && note.Time != next.Time)
                {
                    var leftC = leftPan.Clone().AsCycle(step: 2);
                    var rightC = rightPan.Clone().AsCycle(step: 2);
                    var leftV = leftVol.Clone().AsCycle(step: 2);
                    var rightV = rightVol.Clone().AsCycle(step: 2);
                    var minDur = Math.Abs((next.Time - note.Time) / nEcho);
                    var fract = i % 2 == 0 ? fractions.GetNext() : fractions2.GetNext();
                    var diff = Math.Abs(next.Time - note.Time);

                    var p = note.Pitch;
                    var j = i + 1;
                    for (var x = 0f; x < nEcho; x++)
                    {
                        var scaler = nEcho / (x + 1f);
                        var clone = note.Clone() as NoteOnOffMessage;
                        clone.Time += diff * (fract * x);

                        if (clone.Time > next.Time)
                            break;

                        clone.Channel = echoChannels.GetNext();
                        if (x % 2 == 0)
                        {
                            if (clone.Pitch < Pitch.C2)
                            {
                                clone.SetOctaveAbove();
                                clone.SetOctaveAbove();
                                clone.Reverb = 127 - echoLeft3.GetNext();
                            }
                            else
                            {
                                clone.Reverb = echoLeft3.GetNext();
                            }

                            clone.Pan = Math.Abs(note.Pan - echoLeft.GetNext());
                            clone.Velocity = Math.Abs(note.Velocity - echoLeft2.GetNext());
                            clone.BeforeSendingNoteOnOff += n =>
                            {
                                outputDevice.SendProgramChange(clone.Channel, echoLeftInstr.GetNext());
                            };
                        }
                        else
                        {
                            if (clone.Pitch > Pitch.C6)
                            {
                                clone.SetOctaveBelow();
                                clone.SetOctaveBelow();
                                clone.Reverb = 127 - echoLeft3.GetNext();
                            }
                            else
                            {
                                clone.Reverb = echoLeft3.GetNext();
                            }

                            clone.Pan = Math.Abs(note.Pan - echoRight.GetNext());
                            clone.Velocity = Math.Abs(note.Velocity - echoRight2.GetNext());
                            clone.Reverb = echoRight3.GetNext();
                            clone.BeforeSendingNoteOnOff += n =>
                            {
                                outputDevice.SendProgramChange(clone.Channel, echoRightInstr.GetNext());
                            };
                        }
                        clone.SetOctaveAbove();
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