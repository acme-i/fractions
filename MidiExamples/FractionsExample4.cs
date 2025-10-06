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
    internal class FractionsExample4 : ExampleBase
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

        private readonly Enumerate<Channel> chans = new[] {
            Channel.Channel1,
            Channel.Channel2,
            Channel.Channel3,
            Channel.Channel4,
        }.AsEnumeration();

        private readonly Enumerate<Channel> echoChans = new[] {
            Channel.Channel5,
            Channel.Channel6,
            Channel.Channel7,
            Channel.Channel8,
        }.AsEnumeration();

        private readonly Enumerate<Channel> chans2 = new[] {
            Channel.Channel9,
            Channel.Channel11,
            Channel.Channel12,
            Channel.Channel13,
        }.AsEnumeration();

        private readonly Enumerate<Channel> echoChans2 = new[] {
            Channel.Channel14,
            Channel.Channel15,
            Channel.Channel16
        }.AsEnumeration();

        static Instrument[] instruments_ = new[] {
            Instrument.Vibraphone,
            Instrument.Xylophone,
        };

        private readonly Enumerate<Instrument> mainInstr = instruments_.AsCycle();
        private readonly Enumerate<Instrument> secondInstr = instruments_.AsCycle().AsReversed();
        private readonly Enumerate<Instrument> echoMainInstr = instruments_.AsCycle();
        private readonly Enumerate<Instrument> echoSecondInstr = instruments_.AsCycle().AsReversed();

        static string path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
        static MidiFile file = new MidiFile(path);
        static float div = file.TicksPerQuarterNote + 0f;

        static (IEnumerable<MidiEvent> OnEvents, IEnumerable<MidiEvent> OffEvents, IEnumerable<float> Durations) result = file.GetEventsAndDurations();
        static readonly Enumerate<MidiEvent> nots = result.OnEvents.AsEnumeration();
        static readonly Enumerate<float> durs = result.Durations.AsEnumeration();

        public FractionsExample4() : base("FractionsExample4 - BWV0999: Prelude in Cm for Lute") { }

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

            var voffset = 24;
            var poffset = 8;
            Enumerate<float> fen;
            foreach (var x in channels)
            {
                var s = (int)x;

                fen = vSteps.AsCycle();
                for (var i = 0; i < s; i++)
                    fen.GetNext();
                VolMap.Add(x, fen);

                fen = vSteps2.AsCycle();
                for (var i = 0; i < s + voffset; i++)
                    fen.GetNext();
                EchoVolMap.Add(x, fen);

                fen = vSteps.AsCycle();
                for (var i = 0; i < s + (int)x + voffset; i++)
                    fen.GetNext();
                EchoVolMap2.Add(x, fen);

                fen = pSteps.AsCycle();
                for (var i = 0; i < s; i++)
                    fen.GetNext();
                PanMap.Add(x, fen);

                fen = pSteps.AsCycle();
                for (var i = 0; i < s + poffset; i++)
                    fen.GetNext();
                EchoPanMap.Add(x, fen);

                fen = pSteps2.AsCycle();
                for (var i = 0; i < s + (int)x + poffset; i++)
                    fen.GetNext();
                EchoPanMap2.Add(x, fen);
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

            var echoes = resultTimes.AsCycle();
            for (var i = 0; i < result.Durations.Count(); i++)
            {
                PlayAt(i);
                var nextEcho = echoes.GetNext();
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
            var note = nots.GetNext();
            var dur = durs.GetNext();
            var chan = i % 2 == 0 ? chans : chans2;
            var ch = chan.GetNext();
            var pm = i % 2 == 0 ? PanMap[ch] : EchoPanMap[ch];
            var vol = i % 2 == 0 ? VolMap[ch] : EchoVolMap[ch];
            var instr = i % 4 != 0 ? mainInstr : secondInstr;

            var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note + 12, vol.GetNext(), note.Time / div, Clock, dur, pm.GetNext());
            nt.BeforeSendingNoteOnOff += m =>
            {
                OutputDevice.SendProgramChange(m.Channel, instr.GetNext());
            };
            Clock.Schedule(nt);
        }

        readonly Enumerate<int> ps1 = new[] { 
            12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0,
            24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12,
            0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24
        }.AsEnumeration();
        readonly Enumerate<int> ps2 = new[] {
            24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12,
            0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12,24,
            12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0,
        }.AsEnumeration();
        private void PlayEchos(int i, int max)
        {
            var notsClone = nots.Clone();
            var durClone = durs.Clone();

            var note = notsClone.GetNext();
            var dur = durClone.GetNext();

            var noteDest = notsClone.PeekAt(max);
            var durDest = durClone.PeekAt(max);
            var ps = i % 2 == 0 ? ps1 : ps2;

            var scaler = Interpolator.Interpolate(1f, 1.5f, max, 0).AsCycle();
            var notesTimes = Interpolator.Interpolate(note.Time, noteDest.Time, max, 1).AsEnumeration();
            var noteDurs = Interpolator.Interpolate(dur, durDest, max, 1).AsEnumeration();
            var sm = max;
            var scalers = Enumerable
                    .Range(1, sm)
                    .Select(x => x / (sm * scaler.GetNext()))
                    .AsMaxMinEnumeration();

            max = i + max;

            for (var j = i; j < max; j++)
            {
                var chan = j % 2 == 0 ? echoChans : echoChans2;
                var ch = chan.GetNext();
                var pm = i % 2 == 0 ? EchoPanMap[ch] : EchoPanMap2[ch];
                var vol = i % 2 == 0 ? EchoVolMap[ch] : EchoVolMap2[ch];
                var instr = j % 2 == 0 ? echoMainInstr : echoSecondInstr;

                var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note + ps.GetNext(), vol.GetNext() , (notsClone.Current.Time + notesTimes.GetNext()) / div, Clock, noteDurs.GetNext(), pm.GetNext());
                nt.BeforeSendingNoteOnOff += m =>
                {
                    OutputDevice.SendProgramChange(m.Channel, instr.GetNext());
                };
                Clock.Schedule(nt);

                note = notsClone.GetNext();
                dur = durClone.GetNext();
            }

            ps.GetNext();
        }
    }
}