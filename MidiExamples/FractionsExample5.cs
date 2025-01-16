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
    internal class FractionsExample5 : ExampleBase
    {
        private readonly int minVol = 47;
        private readonly int maxVol = 107;
        private readonly int maxLeft = 4;
        private readonly int maxRight = 127;

        private readonly Dictionary<Channel, Enumerate<float>> VolMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoVolMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoVolMap2 = new Dictionary<Channel, Enumerate<float>>();

        private readonly Dictionary<Channel, Enumerate<float>> PanMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoPanMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoPanMap2 = new Dictionary<Channel, Enumerate<float>>();


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
                Instrument.Xylophone,
            };

        private Enumerate<Instrument> mainInstr = new Enumerate<Instrument>(
            instruments_,
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> secondInstr = new Enumerate<Instrument>(
            instruments_.Reverse<Instrument>(),
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> echoMainInstr = new Enumerate<Instrument>(
            instruments_,
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> echoSecondInstr = new Enumerate<Instrument>(
            instruments_.Reverse<Instrument>(),
            IncrementMethod.Cyclic);

        static string path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
        static MidiFile file = new MidiFile(path);
        static float div = file.TicksPerQuarterNote + 0f;

        static (IEnumerable<MidiEvent> OnEvents, IEnumerable<MidiEvent> OffEvents, IEnumerable<float> Durations) result = file.GetEventsAndDurations();
        static Enumerate<MidiEvent> nots = new Enumerate<MidiEvent>(result.OnEvents, IncrementMethod.MinMax);
        static Enumerate<float> durs = new Enumerate<float>(result.Durations, IncrementMethod.MinMax);

        public FractionsExample5() : base("FractionsExample5 - BWV0999: Prelude in Cm for Lute") { }

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

            var pSteps2 = Interpolator.Interpolate(maxLeft, maxRight, 4 * 6, 0);
            var vSteps2 = Interpolator.Interpolate(minVol, maxVol, 4 * 2, 0);

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

            var voffsets = new Enumerate<int>
            (
                new[] { 2, 8, 2, 16, 2, 32, 2, 64 },
                IncrementMethod.Cyclic
            );
            var poffsets = new Enumerate<int>
            (
                new[] { 2, 4, 8, 12, 16, 18, 24 },
                IncrementMethod.MinMax
            );

            Enumerate<float> fen;

            foreach (var x in channels)
            {
                var s = (int)x;
                var voffset = voffsets.Next();
                var poffset = poffsets.Next();

                var vLimit = new Enumerate<int>(
                    new[] { s, s + voffset, s + 2 * voffset },
                    IncrementMethod.Cyclic
                );
                var pLimit = new Enumerate<int>(
                    new[] { s, s + poffset, s + 2 * poffset },
                    IncrementMethod.Cyclic
                );

                for (int y = 0; y < 3; y++)
                {
                    fen = new Enumerate<float>(vSteppers.Next(), IncrementMethod.Cyclic);
                    volMappers.Next().Add(x, fen);

                    fen = new Enumerate<float>(pSteppers.Next(), IncrementMethod.Cyclic);
                    panMappers.Next().Add(x, fen);
                }
            }
        }

        private void Play()
        {
            var times = new[] {
                0, 0, 0, 2,
                0, 0, 4, 0,
                0, 2, 0, 0,
                4, 0, 0, 0,

                0, 2, 0, 0,
                0, 0, 4, 0,
                0, 0, 0, 2,
                0, 0, 4, 0,

                0, 2, 0, 0,
                4, 0, 0, 0,
                0, 2, 0, 0,
                0, 0, 4, 0,

                4, 0, 0, 0,
                0, 8, 0, 0,
                0, 0, 4, 0,
                0, 0, 0, 2,

                0, 0, 4, 0,
                0, 8, 0, 0,
                4, 0, 0, 0,
                0, 2, 0, 0,

                0, 0, 4, 0,
            };

            var resultTimes = new List<int>(times);
            resultTimes.AddRange(times.Select(t => 2));
            resultTimes.AddRange(times.Select(t => 4));
            resultTimes.AddRange(times.Select(t => 8));
            resultTimes.AddRange(times.Select(t => 4));

            var echoes = new Enumerate<int>(resultTimes, IncrementMethod.Cyclic);
            for (var i = 0; i < result.Durations.Count(); i++)
            {
                PlayAt(i);
                var nextEcho = echoes.Next();
                if (nextEcho > 1)
                {
                    PlayEchos(i, nextEcho);
                }
            }

            Clock.Start();
            Thread.Sleep(400000);
            Clock.Stop();
        }

        private void PlayAt(int i)
        {
            var note = nots.Next();
            var dur = durs.Next();
            var chan = i % 2 == 0 ? chans : chans2;
            var ch = chan.Next();
            var pm = i % 2 == 0 ? PanMap[ch] : EchoPanMap[ch];
            var vol = i % 2 == 0 ? VolMap[ch] : EchoVolMap[ch];
            var instr = i % 4 != 0 ? mainInstr : secondInstr;

            var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note + 12, vol.Next(), note.Time / div, Clock, dur, pm.Next());
            nt.BeforeSendingNoteOnOff += m =>
            {
                OutputDevice.SendProgramChange(m.Channel, instr.Next());
            };
            Clock.Schedule(nt);
        }

        Enumerate<int> ps1 = new Enumerate<int>(
            new[] {
                12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0,
                24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12,
                0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24
            });
        Enumerate<int> ps2 = new Enumerate<int>(
            new[] {
                24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12,
                0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12,24,
                12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0,
            });
        private void PlayEchos(int i, int max)
        {
            var notsClone = nots.Clone();
            var durClone = durs.Clone();

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
                var instr = j % 2 == 0 ? echoMainInstr : echoSecondInstr;

                var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note + ps.Next(), vol.Next(), (notsClone.Current().Time + notesTimes.Next()) / div, Clock, noteDurs.Next(), pm.Next());
                nt.BeforeSendingNoteOnOff += m =>
                {
                    OutputDevice.SendProgramChange(m.Channel, instr.Next());
                };
                Clock.Schedule(nt);

                note = notsClone.Next();
                dur = durClone.Next();
            }

            ps.Next();
        }
    }
}