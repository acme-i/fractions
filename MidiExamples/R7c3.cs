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
    internal class R7c3 : ExampleBase
    {
        public R7c3() : base(nameof(R7c3)) { }

        public override void Run()
        {
            try
            {
                OutputDevice = fractions.OutputDevice.InstalledDevices.FirstOrDefault();
                if (OutputDevice == null)
                {
                    Console.WriteLine("No output devices, so can't run this example.");
                    ExampleUtil.PressAnyKeyToContinue();
                }
                else
                {
                    OutputDevice.Open();
                    Init();
                    Console.WriteLine();
                    ExampleUtil.PressAnyKeyToContinue();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                OutputDevice?.Close();
            }
        }

        static IOutputDevice OutputDevice;
        static Clock Clock;
        static Random rand = new Random(1976);
        static int BPM = 75;
        static int MelodiLength = 500;
        static int MinRev = 8;
        static int MaxRev = 70;
        static int MinVol = 10;
        static int MaxVol = 120;
        static int MaxLeft = 10;
        static int MaxRight = 117;
        static Dictionary<Channel, Incrementor> VolMap = new Dictionary<Channel, Incrementor>();
        static Dictionary<Channel, Incrementor> VolMapE = new Dictionary<Channel, Incrementor>();
        static Dictionary<Channel, Incrementor> PanMap = new Dictionary<Channel, Incrementor>();
        static Dictionary<Channel, Incrementor> PanMapE = new Dictionary<Channel, Incrementor>();

        static Enumerate<Channel> bassChans = new Enumerate<Channel>(new[] {
            Channel.Channel1,
            Channel.Channel2,
        }, IncrementMethod.MinMax, 1);

        static Enumerate<Channel> bass2Chans = new Enumerate<Channel>(new[] {
            Channel.Channel3,
            Channel.Channel4,
            Channel.Channel5,
        }, IncrementMethod.MinMax, 1);

        static Enumerate<Channel> melodiChans = new Enumerate<Channel>(new[] {
            Channel.Channel6,
            Channel.Channel7,
            Channel.Channel8,
            Channel.Channel9,
        }, IncrementMethod.MinMax, 1);

        static Enumerate<Channel> melodi2Chans = new Enumerate<Channel>(new[] {
            Channel.Channel11,
            Channel.Channel12,
            Channel.Channel13,
            Channel.Channel14,
        }, IncrementMethod.MinMax, 1);

        static Enumerate<Channel> echoChans = new Enumerate<Channel>(new[] {
            Channel.Channel15,
            Channel.Channel16,
        }, IncrementMethod.MinMax, 1);

        static Pitch[] lamb = new[] {
            Pitch.E4, Pitch.D4, Pitch.C4, Pitch.D4,
            Pitch.E4, Pitch.E4, Pitch.E4,
        };

        static Pitch[] lamb2 = new[] {
            Pitch.E4, Pitch.D4, Pitch.C4, Pitch.D4,
            Pitch.E4, Pitch.F4, Pitch.G4,
        };

        static string[] chords2 = new[] {
            "Am", "Bdim", "C", "Dm", "Em", "F", "G"
        };

        static Enumerate<Pitch> melodi1 = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic);

        static Enumerate<Pitch> melodi2 = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic).OctaveBelow();

        static Enumerate<Pitch> melodi3 = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic).PitchAbove(5);

        static Enumerate<Pitch> melodi4 = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic).PitchAbove(7);

        static Enumerate<Pitch> melodi1b = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic, 1).OctaveBelow();

        static Enumerate<Pitch> melodi2b = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic, 1).OctaveAbove().OctaveAbove();

        static Enumerate<Pitch> melodi3b = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic, 1).OctaveAbove().PitchAbove(5);

        static Enumerate<Pitch> melodi4b = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic, 1).OctaveAbove().PitchAbove(7);

        static Enumerate<int> times = new Enumerate<int>(new[] { 1, 1, 2, 2, 4, 4, 8, 8, 16, 16, 32, 32, 64, 64, 128, 128 }, IncrementMethod.Cyclic);

        static Enumerate<int> celeste = new Enumerate<int>(Enumerable.Range(MinRev, 127), IncrementMethod.Cyclic, 12, rand.Next(0, 33));
        static Enumerate<int> tremelo = new Enumerate<int>(Enumerable.Range(MinRev, 127), IncrementMethod.Cyclic, 12, rand.Next(34, 66));
        static Enumerate<int> reverb = new Enumerate<int>(Enumerable.Range(MinRev, 127), IncrementMethod.Cyclic, 12, rand.Next(67, 127));

        static Enumerate<int> celestest = new Enumerate<int>(Enumerable.Range(1, 64), IncrementMethod.Cyclic, 7);
        static Enumerate<int> tremelost = new Enumerate<int>(Enumerable.Range(1, 64), IncrementMethod.Cyclic, 7);
        static Enumerate<int> reverbst = new Enumerate<int>(Enumerable.Range(1, 64), IncrementMethod.Cyclic, 7);

        static Enumerate<int> celeste2 = new Enumerate<int>(Enumerable.Range(MinRev, 127), IncrementMethod.Cyclic, 12, rand.Next(0, 33));
        static Enumerate<int> tremelo2 = new Enumerate<int>(Enumerable.Range(MinRev, 127), IncrementMethod.Cyclic, 12, rand.Next(34, 66));
        static Enumerate<int> reverb2 = new Enumerate<int>(Enumerable.Range(MinRev, 127), IncrementMethod.Cyclic, 12, rand.Next(67, 127));

        static Enumerate<int> celestest2 = new Enumerate<int>(Enumerable.Range(1, 64), IncrementMethod.Cyclic, 7);
        static Enumerate<int> tremelost2 = new Enumerate<int>(Enumerable.Range(1, 64), IncrementMethod.Cyclic, 7);
        static Enumerate<int> reverbst2 = new Enumerate<int>(Enumerable.Range(1, 64), IncrementMethod.Cyclic, 7);

        static Enumerate<int> mod1 = new Enumerate<int>(Enumerable.Range(2, count: 9), IncrementMethod.Cyclic);
        static Enumerate<int> mod2 = new Enumerate<int>(Enumerable.Range(2, count: 9), IncrementMethod.Cyclic);

        static Enumerate<int> modi1 = new Enumerate<int>(Enumerable.Range(2, count: 6), IncrementMethod.MinMax);
        static Enumerate<int> modi2 = new Enumerate<int>(Enumerable.Range(2, count: 6), IncrementMethod.MinMax);


        static Enumerate<Enumerate<Pitch>> melodies = new Enumerate<Enumerate<Pitch>>(new[] {
            melodi1, melodi2, melodi3, melodi4
        }, IncrementMethod.MinMax, 1);

        static Enumerate<Enumerate<Pitch>> melodies2 = new Enumerate<Enumerate<Pitch>>(new[] {
            melodi1b, melodi2b, melodi3b, melodi4b
        }, IncrementMethod.MinMax, 1);

        static Enumerate<Instrument> instruments;
        static Enumerate<Instrument> instruments2;

        static void Init()
        {
            var list = new[] {
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,
                Instrument.ElectricGuitarMuted,
            };

            var list2 = new[] {
                Instrument.ElectricPiano1,
                Instrument.AcousticBass,
                Instrument.ElectricPiano1,
            };

            instruments = new Enumerate<Instrument>(list, IncrementMethod.MinMax);
            instruments2 = new Enumerate<Instrument>(list2, IncrementMethod.MinMax);

            var channels = Channels.InstrumentChannels;
            foreach (var x in channels)
            {
                var instr = instruments.Current();
                OutputDevice.SendProgramChange(x, instr);
                OutputDevice.SendControlChange(x, Control.Volume, 100);
                instruments.Next();
            }

            update(16, 30, 2);

            Clock = new Clock(BPM);

            Play();

            //Clock.MinPitch(Channels.InstrumentChannels, Pitch.A1);
            Clock.MaxPitch(Channels.InstrumentChannels, Pitch.G5);
            //Clock.MaxDuration(Channels.InstrumentChannels, 8f);
            Clock.Cleanup();
            Clock.Start();
            Thread.Sleep(400000);
            Clock.Stop();
        }

        static void update(float panSteps, float volSteps, int stepsAhead)
        {
            //MaxLeft = 20 + rand.Next(30);
            //MaxRight = MaxLeft + rand.Next(57);

            //MinVol = 20 + rand.Next(30);
            //MaxVol = MaxLeft + rand.Next(57);

            var panStep = Math.Abs(MaxRight - MaxLeft) / panSteps;
            var volStep = Math.Abs(MaxVol - MinVol) / volSteps;
            VolMap.Clear();
            VolMapE.Clear();
            PanMap.Clear();
            PanMapE.Clear();

            foreach (var x in Channels.InstrumentChannels)
            {
                var xf = (float)x;
                xf += 1;
                VolMap.Add(x, new Incrementor(xf * volStep, MinVol, MaxVol, volStep, IncrementMethod.Cyclic));
                VolMapE.Add(x, new Incrementor(xf * volStep, MinVol, MaxVol, volStep, IncrementMethod.Cyclic));
                PanMap.Add(x, new Incrementor(xf * panStep, MaxLeft, MaxRight, panStep, IncrementMethod.Cyclic));
                PanMapE.Add(x, new Incrementor(xf * panStep, MaxLeft, MaxRight, panStep, IncrementMethod.Cyclic));
            }

            foreach (var p in PanMapE.Values)
                for (var i = 0; i < PanMapE.Values.Count / stepsAhead; i++)
                    p.Next();
            foreach (var v in VolMapE.Values)
                for (var i = 0; i < VolMapE.Values.Count / stepsAhead; i++)
                    v.Next();
        }

        static (Enumerate<Pitch>, Enumerate<Pitch>) GetMelody(float time)
        {
            Enumerate<Pitch> mel = melodies.Next();
            Enumerate<Pitch> bas = melodies2.Next();
            return (mel, bas);
        }

        static void AdvanceMelody()
        {
            foreach (var m in melodies)
                m.Next();
            foreach (var m in melodies2)
                m.Next();
        }

        static void Play()
        {
            var fractions1 = new Enumerate<Fraction>(Fractions.Evens.Where(f => f.Base < 5), IncrementMethod.Cyclic);
            var fractions2 = new Enumerate<Fraction>(Fractions.Thirds.Where(f => f.Base < 9), IncrementMethod.Cyclic);
            var fractions3 = new Enumerate<Fraction>(Fractions.Fourths.Where(f => f.Base < 16), IncrementMethod.Cyclic);
            var fractions4 = new Enumerate<Fraction>(Fractions.Sixths.Where(f => f.Base < 24), IncrementMethod.Cyclic);
            var fractions5 = new Enumerate<Fraction>(Fractions.Eighths.Where(f => f.Base < 32), IncrementMethod.Cyclic);
            var fractions6 = new Enumerate<Fraction>(Fractions.Twelveths.Where(f => f.Base < 48), IncrementMethod.Cyclic);
            var fractions7 = new Enumerate<Fraction>(Fractions.Sixteenths.Where(f => f.Base < 64), IncrementMethod.Cyclic);
            var shifts = new Enumerate<Enumerate<Fraction>>(new[] { fractions1, fractions2, fractions3, fractions4, fractions5, fractions6, fractions7 }, IncrementMethod.MinMax);
            var mets = new Enumerate<IncrementMethod>(new[] { IncrementMethod.MinMax, IncrementMethod.MaxMin, IncrementMethod.Cyclic }, IncrementMethod.Cyclic);
            var scaler1 = new Enumerate<int>(new[] { 2, 4, 8, 16/*, 32, 64*/ }, IncrementMethod.MinMax);
            var scaler2 = new Enumerate<int>(new[] { 4, 8, 16, 32/*, 64, 128*/ }, IncrementMethod.MinMax);
            var skips1 = new Enumerate<int>(new[] { 1, 2, 4, 8/*, 16, 32*/ }, IncrementMethod.MinMax);
            var skips2 = new Enumerate<int>(new[] { 2, 4, 8, 16/*, 32*/ }, IncrementMethod.MinMax);
            var times = new Enumerate<int>(new[] { 1, 2, 4 }, IncrementMethod.Cyclic);
            var panSteps = new Enumerate<int>(Enumerable.Range(1, 16), IncrementMethod.Cyclic, 3);
            var volSteps = new Enumerate<int>(Enumerable.Range(1, 16), IncrementMethod.Cyclic);
            var stepsAhead = new Enumerate<int>(new[] { 2, 3, 4, 5, 6, 7, 8 }, IncrementMethod.Cyclic);
            var sum = 0;
            for (float i = 0; i < MelodiLength; i++)
            {
                var fractions = shifts.Next();
                for (var j = 0; j < times.Next(); j += 1)
                {
                    var f = fractions.Next();
                    var s = j % 2 == 0 ? scaler1.Next() : scaler2.Next();
                    var k = j % 2 == 0 ? skips1.Next() : skips2.Next();

                    update(panSteps.Next(), volSteps.Next(), stepsAhead.Next());
                    AdvanceMelody();
                    if (sum % 2 == 0)
                        PlayMelody1(sum, f, s, k);
                    else
                        PlayMelody2(sum, f, s, k);

                    sum++;
                }
            }
        }

        static void PlayMelody1(float i, Fraction fraction, int scaler, int stepSize)
        {
            var results = fraction.ResultsBetween(1, fraction.Base * scaler, stepSize, includeOne: true, moveIntoRange: true).Take(times.Next()).ToList();

            if (stepSize < results.Count)
            {
                var list = new List<float>();
                for (var x = 0; x < results.Count; x += stepSize)
                {
                    list.Add(results[x]);
                }
                results = list;
            }

            var mels = GetMelody(i);
            var melodi1 = mels.Item1.Clone();
            var bass1 = mels.Item2.Clone();
            var c = melodiChans.Next();
            var baseNote = new NoteOnOffMessage(OutputDevice, melodiChans.Next(), melodi1.Current(), VolMap[c].Next(), i, Clock, results.First(), PanMap[c].Next());
            Clock.Schedule(baseNote);

            var count = 1;
            results.ForEach(f =>
            {
                AdvanceMelody();

                var nc = melodiChans.Next();
                var n = baseNote.MakeTimeShiftedCopy(f) as NoteOnOffMessage;
                n.Channel = nc;
                n.Duration = f;
                n.Velocity = VolMap[nc].Next();
                n.Pitch = melodi1.Current();
                n.Pan = PanMap[nc].Next();
                Clock.Schedule(n);

                if ((i + count) % mod1.Next() == 0)
                {
                    n.BeforeSendingNoteOnOff += (NoteOnOffMessage ne) =>
                    {
                        AdvanceMelody();

                        ne.Channel = bassChans.Next();
                        ne.Velocity = VolMap[ne.Channel].Next();
                        ne.Pan = PanMap[ne.Channel].Next();

                        if ((i + count) % modi1.Next() == 0)
                        {
                            foreach (var x in Channels.InstrumentChannels)
                            {
                                OutputDevice.SendProgramChange(x, instruments.Next());
                            }
                            celeste.Incrementor.SetStepSize(celestest.Next());
                            tremelo.Incrementor.SetStepSize(tremelost.Next());
                            reverb.Incrementor.SetStepSize(reverbst.Next());
                        }
                        else
                        {
                            ne.Velocity *= 0.75f;
                        }

                        foreach (var x in Channels.InstrumentChannels)
                        {
                            OutputDevice.SendControlChange(x, Control.CelesteLevel, celeste.Next());
                            OutputDevice.SendControlChange(x, Control.TremoloLevel, tremelo.Next());
                            OutputDevice.SendControlChange(x, Control.ReverbLevel, reverb.Next());
                        }
                    };
                }

                var nc2 = bassChans.Next();
                var n2 = baseNote.MakeTimeShiftedCopy(f) as NoteOnOffMessage;
                n2.Channel = nc2;
                n2.Duration = f;
                n2.Velocity = VolMapE[nc2].Next();
                n2.Pitch = bass1.Current();
                n2.Pan = PanMapE[nc2].Next();
                Clock.Schedule(n2);

                if ((i + count) % mod2.Next() == 0)
                {
                    n2.BeforeSendingNoteOnOff += (NoteOnOffMessage n2e) =>
                    {
                        n2e.Channel = melodiChans.Next();
                        n2e.Velocity = VolMapE[n2e.Channel].Next();
                        n2e.Pan = PanMapE[n2e.Channel].Next();

                        if ((i + count) % modi2.Next() == 0)
                        {
                            foreach (var x in Channels.InstrumentChannels)
                            {
                                OutputDevice.SendProgramChange(x, instruments2.Next());
                            }
                            celeste2.Incrementor.SetStepSize(celestest2.Next());
                            tremelo2.Incrementor.SetStepSize(tremelost2.Next());
                            reverb2.Incrementor.SetStepSize(reverbst2.Next());
                        }
                        else
                        {
                            n2e.Velocity *= 0.75f;
                        }
                        foreach (var x in Channels.InstrumentChannels)
                        {
                            OutputDevice.SendControlChange(x, Control.CelesteLevel, celeste2.Next());
                            OutputDevice.SendControlChange(x, Control.TremoloLevel, tremelo2.Next());
                            OutputDevice.SendControlChange(x, Control.ReverbLevel, reverb2.Next());
                        }
                    };
                }

                count++;

            });
        }

        static void PlayMelody2(float i, Fraction fraction, int scaler, int stepSize)
        {
            var results = fraction.ResultsBetween(1, fraction.Base * scaler, stepSize, includeOne: true, moveIntoRange: true).Take(times.Next()).ToList();

            if (stepSize < results.Count)
            {
                var list = new List<float>();
                for (var x = 0; x < results.Count; x += stepSize)
                {
                    list.Add(results[x]);
                }
                results = list;
            }

            var mels = GetMelody(i);
            var melodi2 = mels.Item1.Clone().OctaveBelow();
            var bass2 = mels.Item2.Clone().OctaveBelow();
            var c = melodi2Chans.Next();
            var baseNote = new NoteOnOffMessage(OutputDevice, melodi2Chans.Next(), melodi2.Current(), VolMapE[c].Next(), i, Clock, results.First(), PanMapE[c].Next());
            Clock.Schedule(baseNote);

            var count = 1;

            results.ForEach(f =>
            {
                AdvanceMelody();

                var nc = melodi2Chans.Next();
                var n = baseNote.MakeTimeShiftedCopy(f) as NoteOnOffMessage;
                n.Channel = nc;
                n.Duration = f;
                n.Velocity = VolMapE[nc].Next();
                n.Pitch = melodi2.Current();
                n.Pan = PanMapE[nc].Next();
                Clock.Schedule(n);

                if ((i + count) % mod1.Next() == 0)
                {
                    n.BeforeSendingNoteOnOff += (NoteOnOffMessage ne) =>
                    {
                        ne.Channel = bassChans.Next();
                        ne.Velocity = VolMap[ne.Channel].Next();
                        ne.Pan = PanMap[ne.Channel].Next();

                        if ((i + count) % modi1.Next() == 0)
                        {
                            foreach (var x in Channels.InstrumentChannels)
                            {
                                OutputDevice.SendProgramChange(x, instruments.Next());
                            }
                            celeste2.Incrementor.SetStepSize(celestest2.Next());
                            tremelo2.Incrementor.SetStepSize(tremelost2.Next());
                            reverb2.Incrementor.SetStepSize(reverbst2.Next());
                        }
                        else
                        {
                            ne.Velocity *= 0.75f;
                        }
                        foreach (var x in Channels.InstrumentChannels)
                        {
                            OutputDevice.SendControlChange(x, Control.CelesteLevel, celeste.Next());
                            OutputDevice.SendControlChange(x, Control.TremoloLevel, tremelo.Next());
                            OutputDevice.SendControlChange(x, Control.ReverbLevel, reverb.Next());
                        }
                    };
                }

                var nc2 = bass2Chans.Next();
                var n2 = baseNote.MakeTimeShiftedCopy(f) as NoteOnOffMessage;
                n2.Channel = nc2;
                n2.Duration = f;
                n2.Velocity = VolMap[nc2].Next();
                n2.Pitch = bass2.Current();
                n2.Pan = PanMap[nc2].Next();
                Clock.Schedule(n2);

                if ((i + count) % mod2.Next() == 0)
                {
                    n2.BeforeSendingNoteOnOff += (NoteOnOffMessage n2e) =>
                    {
                        AdvanceMelody();

                        n2e.Channel = melodiChans.Next();
                        n2e.Velocity = VolMapE[n2e.Channel].Next();
                        n2e.Pan = PanMapE[n2e.Channel].Next();

                        if ((i + count) % modi2.Next() == 0)
                        {
                            foreach (var x in Channels.InstrumentChannels)
                            {
                                OutputDevice.SendProgramChange(x, instruments2.Next());
                            }
                            celeste2.Incrementor.SetStepSize(celestest2.Next());
                            tremelo2.Incrementor.SetStepSize(tremelost2.Next());
                            reverb2.Incrementor.SetStepSize(reverbst2.Next());
                        }
                        else
                        {
                            n2e.Velocity *= 0.75f;
                        }
                        foreach (var x in Channels.InstrumentChannels)
                        {
                            OutputDevice.SendControlChange(x, Control.CelesteLevel, celeste2.Next());
                            OutputDevice.SendControlChange(x, Control.TremoloLevel, tremelo2.Next());
                            OutputDevice.SendControlChange(x, Control.ReverbLevel, reverb2.Next());
                        }
                    };
                }

                count++;
            });
        }

    }
}