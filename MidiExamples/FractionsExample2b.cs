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
    internal class FractionsExample2b : ExampleBase
    {
        public FractionsExample2b() : base("FractionsExample2b - BWV0999: Prelude in Cm for Lute") { }

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
        private Clock Clock = new Clock(62);

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
                Instrument.ElectricGuitarJazz,
                Instrument.ElectricPiano1,
                Instrument.ElectricGuitarJazz,
            };

        static Instrument[] echoInstruments_ = new[] {
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricGuitarJazz,
                Instrument.ElectricPiano1,
            };

        private readonly Enumerate<Instrument> mainInstr = instruments_.AsCycle();
        private readonly Enumerate<Instrument> secondInstr = instruments_.AsCycle().AsReversed();
        private readonly Enumerate<Instrument> echoMainInstr = echoInstruments_.AsCycle().AsReversed();
        private readonly Enumerate<Instrument> echoSecondInstr = echoInstruments_.AsCycle();

        private void Setup()
        {
            var channels = Channels.InstrumentChannels;
            var pSteps = Interpolator.Interpolate(maxLeft, maxRight, 4 * 12, 0);
            var vSteps = Interpolator.Interpolate(minVol, maxVol, 4 * 4, 0);

            var pSteps2 = Interpolator.Interpolate(maxLeft, maxRight, 4 * 6, 0);
            var vSteps2 = Interpolator.Interpolate(minVol, maxVol, 4 * 2, 0);

            var voffset = 24;
            var poffset = 8;
            foreach (var x in channels)
            {
                var s = (int)x;

                var vv = vSteps.AsCycle();
                for (var i = 0; i < s; i++)
                    vv.GetNext();
                VolMap.Add(x, vv);

                var tempv2 = vSteps2.AsCycle();
                for (var i = 0; i < s + voffset; i++)
                    tempv2.GetNext();
                EchoVolMap.Add(x, tempv2);

                var pp = pSteps.AsCycle();
                for (var i = 0; i < s; i++)
                    pp.GetNext();
                PanMap.Add(x, pp);

                var tempp = pSteps.AsCycle();
                for (var i = 0; i < s + poffset; i++)
                    tempp.GetNext();
                EchoPanMap.Add(x, tempp);

                var tempv = vSteps.AsCycle();
                for (var i = 0; i < s + voffset; i++)
                    tempv.GetNext();
                EchoVolMap2.Add(x, tempv);

                var temppp = pSteps2.AsCycle();
                for (var i = 0; i < s + poffset; i++)
                    temppp.GetNext();
                EchoPanMap2.Add(x, temppp);

            }
        }

        static readonly string path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
        static readonly MidiFile file = new MidiFile(path);
        static readonly float div = file.TicksPerQuarterNote + 0f;

        static (IEnumerable<MidiEvent> OnEvents, IEnumerable<MidiEvent> OffEvents, IEnumerable<float> Durations) result = file.GetEventsAndDurations();
        static readonly Enumerate<MidiEvent> nots = result.OnEvents.AsEnumeration();
        static readonly Enumerate<float> durs = result.Durations.AsEnumeration();

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
            resultTimes.AddRange(times.Select(t => t * 2));

            var echos = resultTimes.AsEnumeration();

            var ps1 = new[] { 12, 0, 24, 0 }.AsEnumeration();
            var ps2 = new[] { 0, 12, 0, 24 }.AsEnumeration();
            var ps3 = new[] { 24, 0, 12, 0 }.AsEnumeration();
            var ps4 = new[] { 0, 24, 0, 12 }.AsEnumeration();

            var ps1a = new[] { 2, 4, 8, 16 }.AsEnumeration();
            var ps2a = new[] { 16, 2, 4, 8 }.AsEnumeration();
            var ps3a = new[] { 8, 16, 2, 4 }.AsEnumeration();
            var ps4a = new[] { 4, 8, 16, 2 }.AsEnumeration();

            var fofx = new[] { ps1a, ps2a, ps3a, ps4a }.AsEnumeration();
            var echosc = new[] { 0.5f, 0.75f, 1.25f, 1.5f }.AsEnumeration();

            var j = 1;
            var k = 1;
            for (var i = 0; i < result.Durations.Count(); i++)
            {
                j++;

                PlayAt(i);
                var echoes = echos.GetNext();
                if (echoes > 1)
                {
                    j++;
                    k++;

                    if (i % 2 == 0)
                    {
                        PlayEchos(ps1, ps2, i, echoes);
                        if (j % 2 == 0)
                            PlayEchos(ps2, ps1, i + fofx.GetNext().GetNext(), (int)(echoes * echosc.GetNext()));
                        else
                            PlayEchos(ps2, ps1, i - fofx.GetNext().GetNext(), (int)(echoes * echosc.GetNext()));
                    }
                    else
                    {
                        PlayEchos(ps3, ps4, i, echoes);
                        if (k % 2 == 0)
                            PlayEchos(ps4, ps3, i + fofx.GetNext().GetNext(), (int)(echoes * echosc.GetNext()));
                        else
                            PlayEchos(ps4, ps3, i - fofx.GetNext().GetNext(), (int)(echoes * echosc.GetNext()));
                    }
                }
            }

            Clock.SetMaxPitch(Pitch.G4);
            Clock.SetMinPitch(Pitch.C3);

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

            var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note, vol.GetNext(), note.Time / div, Clock, dur, pm.GetNext());
            nt.BeforeSendingNoteOnOff += m =>
            {
                OutputDevice.SendProgramChange(m.Channel, instr.GetNext());
            };
            Clock.Schedule(nt);
        }

        private void PlayEchos(Enumerate<int> leftDelay, Enumerate<int> rightDelay, int i, int max)
        {
            if (max < 2f)
                return;

            var notsClone = nots.Clone();
            var durClone = durs.Clone();

            var note = notsClone.GetNext();
            var dur = durClone.GetNext();

            var noteDest = notsClone.PeekAt(max);
            var durDest = durClone.PeekAt(max);
            var ps = (i + 1) % 2 == 0 ? leftDelay.Clone() : rightDelay.Clone();

            var notesTimes = Interpolator.Interpolate(note.Time, noteDest.Time, max, 1).AsEnumeration();
            var noteDurs = Interpolator.Interpolate(dur, durDest, max, 1).AsEnumeration();
            var sm = max;
            var scalers = Enumerable.Range(1, sm)
                .Select(x => x / (float)(sm * 1.25f))
                .AsEnumeration(IncrementMethod.MaxMin);

            max = i + max;

            for (var j = i; j < max; j++)
            {
                var chan = j % 2 == 0 ? echoChans : echoChans2;
                var ch = chan.GetNext();
                var pm = EchoPanMap2[ch];
                var vol = EchoVolMap2[ch];
                var instr = j % 2 == 0 ? echoMainInstr : echoSecondInstr;

                var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note + ps.GetNext(), vol.GetNext() * scalers.GetNext(), (notsClone.Current.Time + notesTimes.GetNext()) / div, Clock, Math.Min(noteDurs.GetNext(), 0.125f), pm.GetNext());
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