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
    internal class FractionsExample7 : ExampleBase
    {
        private readonly int minVol = 27;
        private readonly int maxVol = 107;

        private readonly int maxLeft = 10;
        private readonly int maxRight = 117;

        private readonly int minReverb = 0;
        private readonly int maxReverb = 100;

        private readonly Dictionary<Channel, Enumerate<float>> VolMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoVolMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoVolMap2 = new Dictionary<Channel, Enumerate<float>>();

        private readonly Dictionary<Channel, Enumerate<float>> PanMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoPanMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoPanMap2 = new Dictionary<Channel, Enumerate<float>>();

        private readonly Dictionary<Channel, Enumerate<float>> ReverbMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoReverbMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoReverbMap2 = new Dictionary<Channel, Enumerate<float>>();

        private IOutputDevice OutputDevice = fractions.OutputDevice.InstalledDevices.FirstOrDefault();
        private Clock Clock = new Clock(32);

        private readonly Enumerate<Channel> chans = new Enumerate<Channel>(
            new[] {
                Channel.Channel1,
                Channel.Channel2,
                Channel.Channel3,
                Channel.Channel4,
            }, IncrementMethod.MinMax);


        private readonly Enumerate<Channel> echoChans = new Enumerate<Channel>(
            new[] {
                Channel.Channel5,
                Channel.Channel6,
                Channel.Channel7,
                Channel.Channel8,
            }, IncrementMethod.MinMax);

        private readonly Enumerate<Channel> chans2 = new Enumerate<Channel>(
            new[] {
                Channel.Channel9,
                Channel.Channel11,
                Channel.Channel12,
                Channel.Channel13,
            }, IncrementMethod.MinMax);


        private readonly Enumerate<Channel> echoChans2 = new Enumerate<Channel>(
            new[] {
                Channel.Channel14,
                Channel.Channel15,
                Channel.Channel16
            }, IncrementMethod.MinMax);

        static Instrument[] instruments_ = new[] {
                Instrument.Vibraphone,
                Instrument.ElectricPiano1,
                Instrument.SlapBass1,
            };

        private Enumerate<Instrument> mainInstr = new Enumerate<Instrument>(
            instruments_.Reverse<Instrument>(),
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> secondInstr = new Enumerate<Instrument>(
            instruments_,
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> echoMainInstr = new Enumerate<Instrument>(
            instruments_.Reverse<Instrument>(),
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> echoSecondInstr = new Enumerate<Instrument>(
            instruments_,
            IncrementMethod.Cyclic);

        static string path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
        static MidiFile file = new MidiFile(path);
        static float div = file.TicksPerQuarterNote + 0f;

        static (IEnumerable<MidiEvent> OnEvents, IEnumerable<MidiEvent> OffEvents, IEnumerable<float> Durations) result = file.GetEventsAndDurations();
        static Enumerate<MidiEvent> nots = new Enumerate<MidiEvent>(result.OnEvents, IncrementMethod.MinMax);
        static Enumerate<float> durs = new Enumerate<float>(result.Durations, IncrementMethod.MinMax);

        public FractionsExample7() : base("FractionsExample7 - BWV0999: Prelude in Cm for Lute") { }

        public override void Run()
        {
            if (OutputDevice == null)
            {
                Console.WriteLine("No output devices, so can't run this example.");
                ExampleUtil.PressAnyKeyToContinue();
                return;
            }
            OutputDevice.Open();

            Setup();
            Play();

            OutputDevice.Close();

            Console.WriteLine();
            ExampleUtil.PressAnyKeyToContinue();
        }

        private void Setup()
        {
            var channels = Channels.InstrumentChannels;
            var pSteps = Interpolator.Interpolate(maxLeft, maxRight, 4 * 12, 0);
            var vSteps = Interpolator.Interpolate(minVol, maxVol, 4 * 4, 0);
            var rSteps = Interpolator.Interpolate(minReverb, maxReverb, 4 * 4, 0);

            var pSteps2 = Interpolator.Interpolate(maxLeft, maxRight, 4 * 6, 0);
            var vSteps2 = Interpolator.Interpolate(minVol, maxVol, 4 * 2, 0);
            var rSteps2 = Interpolator.Interpolate(minReverb, maxReverb, 4, 0);

            var volMappers = new Enumerate<Dictionary<Channel, Enumerate<float>>>
            (
                new[] { VolMap, EchoVolMap, EchoVolMap2 },
                IncrementMethod.MinMax
            );

            var panMappers = new Enumerate<Dictionary<Channel, Enumerate<float>>>
            (
                new[] { PanMap, EchoPanMap, EchoPanMap2 },
                IncrementMethod.MinMax
            );

            var reverbMappers = new Enumerate<Dictionary<Channel, Enumerate<float>>>
            (
                new[] { ReverbMap, EchoReverbMap, EchoReverbMap2 },
                IncrementMethod.MinMax
            );

            var vSteppers = new Enumerate<List<float>>
            (
                new[] { vSteps, vSteps2 },
                IncrementMethod.MinMax
            );

            var pSteppers = new Enumerate<List<float>>
            (
                new[] { pSteps, pSteps2 },
                IncrementMethod.MinMax
            );

            var rSteppers = new Enumerate<List<float>>
            (
                new[] { rSteps, rSteps2 },
                IncrementMethod.Cyclic
            );

            Enumerate<float> fen;
            foreach (var x in channels)
            {
                var s = (int)x;

                for (int y = 0; y < 3; y++)
                {
                    volMappers.Next().Add(x, new Enumerate<float>(vSteppers.Next(), IncrementMethod.Cyclic));
                    panMappers.Next().Add(x, new Enumerate<float>(pSteppers.Next(), IncrementMethod.Cyclic));
                    reverbMappers.Next().Add(x, new Enumerate<float>(rSteppers.Next(), IncrementMethod.Cyclic));
                }
            }
        }

        private void Play()
        {
            var resultTimes = new List<int>(new[] {
                1, 2, 4, 8, 16, 32, 16, 8, 4, 2,
            });

            resultTimes.AddRange(new[] {
                1, 4, 16, 8, 4, 2,
            });

            resultTimes.AddRange(new[] {
                1, 16, 4,
            });

            resultTimes.AddRange(new[] {
                16, 32,
            });

            resultTimes.AddRange(new[] {
                16, 8,
            });

            var echoes = new Enumerate<int>(resultTimes, IncrementMethod.Cyclic);
            var repeats = new Enumerate<int>(Enumerable.Range(4, 32).Where(i => i % 4 == 0), IncrementMethod.Cyclic);
            var repeats2 = new Enumerate<int>(Enumerable.Range(8, 64).Where(i => i % 8 == 0), IncrementMethod.Cyclic);
            var repeats3 = new Enumerate<int>(Enumerable.Range(16, 128).Where(i => i % 16 == 0), IncrementMethod.Cyclic);
            var repeats4 = new Enumerate<int>(Enumerable.Range(32, 256).Where(i => i % 32 == 0), IncrementMethod.Cyclic);
            var increments = new Enumerate<int>(Enumerable.Range(2, 16).Where(i => i % 2 == 0), IncrementMethod.Cyclic);
            var increments2 = new Enumerate<int>(Enumerable.Range(4, 32).Where(i => i % 4 == 0), IncrementMethod.Cyclic);
            var increments3 = new Enumerate<int>(Enumerable.Range(8, 64).Where(i => i % 8 == 0), IncrementMethod.Cyclic);
            var increments4 = new Enumerate<int>(Enumerable.Range(16, 128).Where(i => i % 16 == 0), IncrementMethod.Cyclic);
            
            for (var i = 0; i < result.Durations.Count(); i++)
            {
                var result = PlayAt(i);

                var nextEcho = echoes.Next();
                if (nextEcho > 1)
                {
                    PlayEchos(i, nextEcho);
                    for (var j = 4; j <= repeats.Next(); j += increments.Next())
                    {
                        if ((i / j) > 1)
                            PlayEchos(i, j);
                    }
                }
            }

            Clock.Start();
            Thread.Sleep(400000);
            Clock.Stop();
        }

        private NoteOnOffMessage PlayAt(int i)
        {
            var note = nots.Next();
            var dur = durs.Next();
            var chan = i % 2 == 0 ? chans : chans2;
            var ch = chan.Next();
            var pm = i % 2 == 0 ? PanMap[ch] : EchoPanMap[ch];
            var vol = i % 2 == 0 ? VolMap[ch] : EchoVolMap[ch];
            var instr = i % 4 != 0 ? mainInstr : secondInstr;
            var rev = i % 2 == 0 ? ReverbMap[ch] : EchoReverbMap[ch];

            var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note + 12, vol.Next(), note.Time / div, Clock, dur, pm.Next(), instr.Next(), rev.Next());
            Clock.Schedule(nt);

            return nt;
        }

        Enumerate<int> ps1 = new Enumerate<int>(
            new[] {
                12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0,
                24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12,
                0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24
            });
        Enumerate<int> ps2 = new Enumerate<int>(
            new[] {
                -24, 0,-12, -24, 0,-12, -24, 0,-12, -24, 0,-12,
                0,-12, -24, 0,-12, -24, 0,-12, -24, 0,-12,-24,
               -12, -24, 0,-12, -24, 0,-12, -24, 0,-12, -24, 0,
            });
        private void PlayEchos(int i, int max)
        {
            //Console.WriteLine($"Echo {i} {max}");

            var notsClone = nots.Clone();
            var durClone = durs.Clone();

            for (int y = 0; y < i - 1; y++)
            {
                notsClone.Next();
                durClone.Next();
            }

            var note = notsClone.Next();
            var dur = durClone.Next();

            var noteDest = notsClone.Peek(max);
            var durDest = durClone.Peek(max);
            var ps = i % 2 == 0 ? ps1 : ps2;
            var notesTimes = new Enumerate<float>(Interpolator.Interpolate(note.Time, noteDest.Time, max, 1));
            var noteDurs = new Enumerate<float>(Interpolator.Interpolate(dur, durDest, max, 1));
            var sm = max;
            max = i + max;

            for (var j = i; j < max; j++)
            {
                var chan = j % 2 == 0 ? echoChans : echoChans2;
                var ch = chan.Next();
                var pm = i % 2 == 0 ? EchoPanMap[ch] : EchoPanMap2[ch];
                var vol = i % 2 == 0 ? EchoVolMap[ch] : EchoVolMap2[ch];
                var instr = i % 2 == 0 ? echoMainInstr : echoSecondInstr;
                var rev = i % 2 == 0 ? EchoReverbMap[ch] : EchoReverbMap2[ch];

                Clock.Schedule(
                    new NoteOnOffMessage
                    (
                        OutputDevice, ch,
                        (Pitch)notsClone.Next().Note + ps.Next(),
                        vol.Next(),
                        (notsClone.Current().Time + notesTimes.Next()) / div,
                        Clock,
                        noteDurs.Next(),
                        pm.Next(),
                        instr.Next(),
                        rev.Next()
                    )
                );

                note = notsClone.Next();
                dur = durClone.Next();
            }

            ps.Next();
        }
    }
}