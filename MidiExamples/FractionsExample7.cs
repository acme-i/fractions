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
        private Clock Clock = new Clock(64);

        private readonly Enumerate<Channel> channels = new[] {
            Channel.Channel1, Channel.Channel2, Channel.Channel3, Channel.Channel4,
        }.AsEnumeration();

        private readonly Enumerate<Channel> echoChannels = new[] {
            Channel.Channel5, Channel.Channel6, Channel.Channel7, Channel.Channel8,
        }.AsEnumeration();

        private readonly Enumerate<Channel> secondChannels = new[] {
            Channel.Channel9, Channel.Channel11, Channel.Channel12, Channel.Channel13,
        }.AsEnumeration();

        private readonly Enumerate<Channel> secondEchoChannels = new[] {
            Channel.Channel14, Channel.Channel15, Channel.Channel16
        }.AsEnumeration();

        readonly static Instrument[] instruments_ = new[] {
                Instrument.ElectricPiano1,
                Instrument.ElectricPiano1,
            };

        readonly static Instrument[] secondInstruments_ = new[] {
                Instrument.ElectricPiano1,
                Instrument.ElectricPiano1,
            };

        readonly static Instrument[] echoInstruments_ = new[] {
                Instrument.SlapBass1,
                Instrument.ElectricGuitarMuted,
            };

        readonly static Instrument[] echoSecondInstruments_ = new[] {
                Instrument.ElectricGuitarMuted,
                Instrument.SlapBass1,
            };

        private readonly Enumerate<Instrument> mainInstruments = instruments_.AsCycle().AsReversed();
        private readonly Enumerate<Instrument> secondInstruments = secondInstruments_.AsCycle();
        private readonly Enumerate<Instrument> echoMainInstruments = echoInstruments_.AsCycle().AsReversed();
        private readonly Enumerate<Instrument> echoSecondInstruments = echoSecondInstruments_.AsCycle();

        static readonly string path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
        static readonly MidiFile file = new MidiFile(path);
        static readonly float div = file.TicksPerQuarterNote + 0f;

        static (IEnumerable<MidiEvent> OnEvents, IEnumerable<MidiEvent> OffEvents, IEnumerable<float> Durations) result = file.GetEventsAndDurations();
        static readonly Enumerate<MidiEvent> notes = result.OnEvents.AsEnumeration();
        static readonly Enumerate<float> durations = result.Durations.AsEnumeration();

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
            var pSteps = Interpolator.Interpolate(maxLeft, maxRight, 4, 0);
            var vSteps = Interpolator.Interpolate(minVol, maxVol, 4, 0);
            var rSteps = Interpolator.Interpolate(minReverb, maxReverb, 3, 0);

            var pSteps2 = Interpolator.Interpolate(maxLeft, maxRight, 8, 0);
            var vSteps2 = Interpolator.Interpolate(minVol, maxVol, 8, 0);
            var rSteps2 = Interpolator.Interpolate(minReverb, maxReverb, 8, 0);

            var volMappers = new[] { VolMap, EchoVolMap, EchoVolMap2 }.AsEnumeration();
            var panMappers = new[] { PanMap, EchoPanMap, EchoPanMap2 }.AsEnumeration();
            var reverbMappers = new[] { ReverbMap, EchoReverbMap, EchoReverbMap2 }.AsEnumeration();

            var vSteppers = new[] { vSteps, vSteps2 }.AsCycle();
            var pSteppers = new[] { pSteps, pSteps2 }.AsCycle();
            var rSteppers = new[] { rSteps, rSteps2 }.AsCycle();
            foreach (var x in channels)
            {
                for (int y = 0; y < 3; y++)
                {
                    volMappers.GetNext().Add(x, vSteppers.GetNext().AsCycle());
                    panMappers.GetNext().Add(x, pSteppers.GetNext().AsCycle());
                    reverbMappers.GetNext().Add(x, rSteppers.GetNext().AsCycle());
                }
            }
        }

        private void Play()
        {
            var echoes = new[] { 1, 2 }.AsCycle();
            var repeats1 = Enumerable.Range(2, 8).Where(i => i % 2 == 0).AsCycle();
            var increments1 = Enumerable.Range(2, 16).Where(i => i % 2 == 0).AsCycle();
            var increments2 = Enumerable.Range(2, 32).Where(i => i % 4 == 0).AsCycle();
            var increments3 = Enumerable.Range(2, 64).Where(i => i % 8 == 0).AsCycle();
            var increments4 = Enumerable.Range(2, 128).Where(i => i % 12 == 0).AsCycle();
            var allIncrements = new[] { increments1, increments2, increments3, increments4 }.AsCycle();

            for (var i = 0; i < result.Durations.Count(); i++)
            {
                var result = PlayAt(i);
                var nextEcho = echoes.GetNext();
                for (var j = 1; j <= repeats1.GetNext(); j += allIncrements.GetNext().GetNext())
                    PlayEchos(i, j);
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

        private void PlayEchos(int i, int max)
        {
            //Console.WriteLine($"Echo {i} {max}");

            var notsClone = notes.Clone();
            var durClone = durations.Clone();

            for (int y = 0; y < i; y++)
            {
                notsClone.GetNext();
                durClone.GetNext();
            }

            var note = notsClone.GetNext();
            var dur = durClone.GetNext();

            var noteDest = notsClone.PeekAt(max);
            var durDest = durClone.PeekAt(max);
            var notesTimes = Interpolator.Interpolate(note.Time, noteDest.Time, max, 1).AsEnumeration();
            var noteDurs = Interpolator.Interpolate(dur, durDest, max, 1).AsEnumeration();
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
                        (Pitch)notsClone.GetNext().Note,
                        vol.GetNext(),
                        (notsClone.Current.Time + notesTimes.GetNext()) / div,
                        Clock,
                        noteDurs.GetNext(),
                        pm.GetNext(),
                        instr.GetNext(),
                        rev.GetNext()
                    )
                );

                notsClone.GetNext();
                durClone.GetNext();
            }
        }
    }
}