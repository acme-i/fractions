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
        private Clock Clock = new Clock(8);

        private readonly Enumerate<Channel> channels = new Enumerate<Channel>(
            new[] {
                Channel.Channel1,
                Channel.Channel2,
                Channel.Channel3,
                Channel.Channel4,
            }, IncrementMethod.MinMax);

        private readonly Enumerate<Channel> echoChannels = new Enumerate<Channel>(
            new[] {
                Channel.Channel5,
                Channel.Channel6,
                Channel.Channel7,
                Channel.Channel8,
            }, IncrementMethod.MinMax);

        private readonly Enumerate<Channel> secondChannels = new Enumerate<Channel>(
            new[] {
                Channel.Channel9,
                Channel.Channel11,
                Channel.Channel12,
                Channel.Channel13,
            }, IncrementMethod.MinMax);

        private readonly Enumerate<Channel> secondEchoChannels = new Enumerate<Channel>(
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

        private Enumerate<Instrument> mainInstruments = new Enumerate<Instrument>(
            instruments_.Reverse<Instrument>(),
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> secondInstruments = new Enumerate<Instrument>(
            instruments_,
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> echoMainInstruments = new Enumerate<Instrument>(
            instruments_.Reverse<Instrument>(),
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> echoSecondInstruments = new Enumerate<Instrument>(
            instruments_,
            IncrementMethod.Cyclic);

        static string path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
        static MidiFile file = new MidiFile(path);
        static float div = file.TicksPerQuarterNote + 0f;

        static (IEnumerable<MidiEvent> OnEvents, IEnumerable<MidiEvent> OffEvents, IEnumerable<float> Durations) result = file.GetEventsAndDurations();
        static Enumerate<MidiEvent> notes = result.OnEvents.AsEnumeration();
        static Enumerate<float> durations = result.Durations.AsEnumeration();

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

            foreach (var x in channels)
            {
                for (int y = 0; y < 3; y++)
                {
                    volMappers.GetNext().Add(x, new Enumerate<float>(vSteppers.GetNext(), IncrementMethod.Cyclic));
                    panMappers.GetNext().Add(x, new Enumerate<float>(pSteppers.GetNext(), IncrementMethod.Cyclic));
                    reverbMappers.GetNext().Add(x, new Enumerate<float>(rSteppers.GetNext(), IncrementMethod.Cyclic));
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

                var nextEcho = echoes.GetNext();
                if (nextEcho > 1)
                {
                    PlayEchos(i, nextEcho);
                    for (var j = 4; j <= repeats.GetNext(); j += increments.GetNext())
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
            var note = notes.GetNext();
            var dur = durations.GetNext();
            var chan = i % 2 == 0 ? channels : secondChannels;
            var ch = chan.GetNext();
            var pm = i % 2 == 0 ? PanMap[ch] : EchoPanMap[ch];
            var vol = i % 2 == 0 ? VolMap[ch] : EchoVolMap[ch];
            var instr = i % 4 != 0 ? mainInstruments : secondInstruments;
            var rev = i % 2 == 0 ? ReverbMap[ch] : EchoReverbMap[ch];

            var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note + 12, vol.GetNext(), note.Time / div, Clock, dur, pm.GetNext(), instr.GetNext(), rev.GetNext());
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

            var notsClone = notes.Clone();
            var durClone = durations.Clone();

            for (int y = 0; y < i - 1; y++)
            {
                notsClone.GetNext();
                durClone.GetNext();
            }

            var note = notsClone.GetNext();
            var dur = durClone.GetNext();

            var noteDest = notsClone.PeekAt(max);
            var durDest = durClone.PeekAt(max);
            var ps = i % 2 == 0 ? ps1 : ps2;
            var notesTimes = new Enumerate<float>(Interpolator.Interpolate(note.Time, noteDest.Time, max, 1));
            var noteDurs = new Enumerate<float>(Interpolator.Interpolate(dur, durDest, max, 1));
            var sm = max;
            max = i + max;

            for (var j = i; j < max; j++)
            {
                var chan = j % 2 == 0 ? echoChannels : secondEchoChannels;
                var ch = chan.GetNext();
                var pm = i % 2 == 0 ? EchoPanMap[ch] : EchoPanMap2[ch];
                var vol = i % 2 == 0 ? EchoVolMap[ch] : EchoVolMap2[ch];
                var instr = i % 2 == 0 ? echoMainInstruments : echoSecondInstruments;
                var rev = i % 2 == 0 ? EchoReverbMap[ch] : EchoReverbMap2[ch];

                Clock.Schedule(
                    new NoteOnOffMessage
                    (
                        OutputDevice, ch,
                        (Pitch)notsClone.GetNext().Note + ps.GetNext(),
                        vol.GetNext(),
                        (notsClone.Current.Time + notesTimes.GetNext()) / div,
                        Clock,
                        noteDurs.GetNext(),
                        pm.GetNext(),
                        instr.GetNext(),
                        rev.GetNext()
                    )
                );

                note = notsClone.GetNext();
                dur = durClone.GetNext();
            }

            ps.GetNext();
        }
    }
}