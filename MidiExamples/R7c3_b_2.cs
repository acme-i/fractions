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
    internal class R7c3_b_2 : ExampleBase
    {
        public R7c3_b_2() : base(nameof(R7c3_b_2)) { }

        public override void Run()
        {
            try
            {
                Init();
                Console.WriteLine();
                ExampleUtil.PressAnyKeyToContinue();
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

        static IOutputDevice OutputDevice = fractions.OutputDevice.InstalledDevices.FirstOrDefault();
        static int BPM = 250;
        static Clock Clock = new Clock(BPM);

        static Random rand = new Random(3453);

        static int MelodiLength = 258;
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

        static string path = @".\midifiles\bach_js_bwv0825_keyboard_partita_no1_in_bb_2_allemande.mid";
        static MidiFile file = new MidiFile(path);
        static float div = file.TicksPerQuarterNote + 0f;
        static List<Pitch> lamb = file.GetNotes(OutputDevice, Clock).Select(n => n.Pitch).OctaveAbove().OctaveAbove().ToList();
        static List<Pitch> lamb2 = lamb.ToList();

        static Enumerate<Pitch> melodi1 = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic);

        static Enumerate<Pitch> melodi2 = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic).OctaveAbove();

        static Enumerate<Pitch> melodi3 = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic).OctaveAbove();

        static Enumerate<Pitch> melodi4 = new Enumerate<Pitch>(lamb, IncrementMethod.Cyclic).OctaveAbove();

        static Enumerate<Pitch> melodi1b = new Enumerate<Pitch>(lamb2, IncrementMethod.Cyclic, 1);

        static Enumerate<Pitch> melodi2b = new Enumerate<Pitch>(lamb2, IncrementMethod.Cyclic, 1).OctaveAboveOrBelow(2, 0.5, rand, true);

        static Enumerate<Pitch> melodi3b = new Enumerate<Pitch>(lamb2, IncrementMethod.Cyclic, 1).OctaveAboveOrBelow(2, 0.5, rand, true);

        static Enumerate<Pitch> melodi4b = new Enumerate<Pitch>(lamb2, IncrementMethod.Cyclic, 1).OctaveAboveOrBelow(2, 0.5, rand, true);

        static Enumerate<int> times = new Enumerate<int>(new[] { 1, 1, 2, 2, 4, 4, 8, 8, 16, 32, 64, 128 }, IncrementMethod.Cyclic);

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
            OutputDevice.Open();

            var list = new[] {
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricGuitarMuted,
            };

            var list2 = new[] {
                Instrument.SlapBass1,
                Instrument.ElectricGuitarMuted,
            };

            instruments = new Enumerate<Instrument>(Instruments.SoftGuitars, IncrementMethod.MinMax);
            instruments2 = new Enumerate<Instrument>(Instruments.SoftBasses, IncrementMethod.MinMax);

            Play();

            Clock.MinPitch(Channels.InstrumentChannels, Pitch.A1);
            Clock.MaxPitch(Channels.InstrumentChannels, Pitch.G5);
            //Clock.MaxDuration(Channels.InstrumentChannels, 8f);
            Clock.Cleanup();
            Clock.Start();
            Thread.Sleep(400000);
            Clock.Stop();
        }

        static void update(float panSteps, float volSteps, int stepsAhead)
        {
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
            var fractions1 = new Enumerate<Fraction>(Fractions.Evens, IncrementMethod.Cyclic);
            var fractions2 = new Enumerate<Fraction>(Fractions.Thirds, IncrementMethod.Cyclic);
            var fractions3 = new Enumerate<Fraction>(Fractions.Fourths, IncrementMethod.Cyclic);
            var fractions4 = new Enumerate<Fraction>(Fractions.Sixths, IncrementMethod.Cyclic);
            var fractions5 = new Enumerate<Fraction>(Fractions.Eighths, IncrementMethod.Cyclic);
            var fractions6 = new Enumerate<Fraction>(Fractions.Twelveths, IncrementMethod.Cyclic);
            var fractions7 = new Enumerate<Fraction>(Fractions.Sixteenths, IncrementMethod.Cyclic);
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

            update(1, 1, 16);
            
            for (float i = 0; i < MelodiLength; i++)
            {
                var fractions = shifts.Next();
                for (var j = 0; j < times.Next(); j += 1)
                {
                    var f = fractions.Next();
                    var s = j % 2 == 0 ? scaler1.Next() : scaler2.Next();
                    var k = j % 2 == 0 ? skips1.Next() : skips2.Next();

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
            var baseNote = new NoteOnOffMessage(OutputDevice, c, melodi1.Current(), VolMap[c].Next(), i, Clock, results.First(), PanMap[c].Next());
            Clock.Schedule(baseNote);

            var count = 1;
            results.ForEach(f =>
            {
                if (count % 2 == 0)
                {
                    var n = baseNote.MakeTimeShiftedCopy(f) as NoteOnOffMessage;
                    n.Duration = f;
                    n.Velocity = VolMap[n.Channel].Next() * 0.75;
                    n.Pan = PanMap[n.Channel].Next();
                    Clock.Schedule(n);

                    n.BeforeSendingNoteOnOff += (NoteOnOffMessage ne) =>
                    {
                        OutputDevice.SendProgramChange(ne.Channel, instruments.Next());
                        //celeste2.Incrementor.SetStepSize(celestest.Next());
                        //tremelo2.Incrementor.SetStepSize(tremelost.Next());
                        //reverb2.Incrementor.SetStepSize(reverbst.Next());
                        OutputDevice.SendControlChange(ne.Channel, Control.CelesteLevel, celeste.Next());
                        OutputDevice.SendControlChange(ne.Channel, Control.TremoloLevel, tremelo.Next());
                        OutputDevice.SendControlChange(ne.Channel, Control.ReverbLevel, reverb.Next());
                    };
                }
                else
                {
                    var n2 = baseNote.MakeTimeShiftedCopy(f) as NoteOnOffMessage;
                    n2.Channel = melodi2Chans.Next();
                    n2.Duration = f;
                    n2.Velocity = VolMapE[n2.Channel].Next() * 0.75;
                    n2.Pitch = bass1.Current();
                    n2.Pan = PanMapE[n2.Channel].Next();
                    Clock.Schedule(n2);

                    n2.BeforeSendingNoteOnOff += (NoteOnOffMessage n2e) =>
                    {
                        OutputDevice.SendProgramChange(n2e.Channel, instruments2.Next());
                        //celeste2.Incrementor.SetStepSize(celestest2.Next());
                        //tremelo2.Incrementor.SetStepSize(tremelost2.Next());
                        //reverb2.Incrementor.SetStepSize(reverbst2.Next());
                        OutputDevice.SendControlChange(n2e.Channel, Control.CelesteLevel, celeste2.Next());
                        OutputDevice.SendControlChange(n2e.Channel, Control.TremoloLevel, tremelo2.Next());
                        OutputDevice.SendControlChange(n2e.Channel, Control.ReverbLevel, reverb2.Next());
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
                if (count % 2 == 0)
                {
                    var n = baseNote.MakeTimeShiftedCopy(f) as NoteOnOffMessage;
                    n.Duration = f;
                    n.Velocity = VolMap[n.Channel].Next() * 0.75;
                    n.Pan = PanMap[n.Channel].Next();
                    Clock.Schedule(n);
                    OutputDevice.SendProgramChange(n.Channel, instruments.Next());

                    n.BeforeSendingNoteOnOff += (NoteOnOffMessage ne) =>
                    {
                        //OutputDevice.SendProgramChange(ne.Channel, instruments.Next());
                        //celeste2.Incrementor.SetStepSize(celestest.Next());
                        //tremelo2.Incrementor.SetStepSize(tremelost.Next());
                        //reverb2.Incrementor.SetStepSize(reverbst.Next());
                        OutputDevice.SendControlChange(ne.Channel, Control.CelesteLevel, celeste.Next());
                        OutputDevice.SendControlChange(ne.Channel, Control.TremoloLevel, tremelo.Next());
                        OutputDevice.SendControlChange(ne.Channel, Control.ReverbLevel, reverb.Next());
                    };
                }
                else
                {
                    var n2 = baseNote.MakeTimeShiftedCopy(f) as NoteOnOffMessage;
                    n2.Channel = bass2Chans.Next();
                    n2.Duration = f;
                    n2.Velocity = VolMapE[n2.Channel].Next() * 0.75;
                    n2.Pitch = bass2.Current();
                    n2.Pan = PanMapE[n2.Channel].Next();
                    Clock.Schedule(n2);

                    OutputDevice.SendProgramChange(n2.Channel, instruments2.Next());

                    n2.BeforeSendingNoteOnOff += (NoteOnOffMessage n2e) =>
                    {
                        //OutputDevice.SendProgramChange(n2e.Channel, instruments2.Next());
                        //celeste2.Incrementor.SetStepSize(celestest2.Next());
                        //tremelo2.Incrementor.SetStepSize(tremelost2.Next());
                        //reverb2.Incrementor.SetStepSize(reverbst2.Next());
                        OutputDevice.SendControlChange(n2e.Channel, Control.CelesteLevel, celeste2.Next());
                        OutputDevice.SendControlChange(n2e.Channel, Control.TremoloLevel, tremelo2.Next());
                        OutputDevice.SendControlChange(n2e.Channel, Control.ReverbLevel, reverb2.Next());
                    };
                }

                count++;
            });
        }

    }
}