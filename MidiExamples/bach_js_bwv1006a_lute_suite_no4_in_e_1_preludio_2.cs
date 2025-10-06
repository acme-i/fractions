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
    internal class bach_js_bwv1006a_lute_suite_no4_in_e_1_preludio_2 : ExampleBase
    {
        public bach_js_bwv1006a_lute_suite_no4_in_e_1_preludio_2() : base("FractionsExample - bach_js_bwv0999_prelude_in_cm_for_lute 3") { }

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
        private readonly int maxLeft = 10;
        private readonly int maxRight = 117;
        private readonly Dictionary<Channel, Enumerate<float>> VolMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoVolMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> PanMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoPanMap = new Dictionary<Channel, Enumerate<float>>();

        private IOutputDevice OutputDevice = fractions.OutputDevice.InstalledDevices.FirstOrDefault();
        private Clock Clock = new Clock(80);

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

        private Enumerate<Instrument> mainInstr = new Enumerate<Instrument>(
            new[] {
                Instrument.Vibraphone,
                Instrument.Vibraphone,
            },
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> secondInstr = new Enumerate<Instrument>(
            new[] {
                Instrument.Vibraphone,
                Instrument.Vibraphone,
            },
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> echoMainInstr = new Enumerate<Instrument>(
            new[] {
                Instrument.Vibraphone,
                Instrument.Vibraphone,
            },
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> echoSecondInstr = new Enumerate<Instrument>(
            new[] {
                Instrument.Vibraphone,
                Instrument.Xylophone,
            },
            IncrementMethod.Cyclic);

        private void Setup()
        {
            var channels = Channels.InstrumentChannels;
            var pSteps = Interpolator.Interpolate(maxLeft, maxRight, 4, 0);
            var vSteps = Interpolator.Interpolate(minVol, maxVol, 4, 0);

            var voffset = 8;
            var poffset = 8;
            foreach (var x in channels)
            {
                var s = (int)x;

                var vv = new Enumerate<float>(vSteps, IncrementMethod.Cyclic);
                for (var i = 0; i < s; i++)
                    vv.GetNext();
                VolMap.Add(x, vv);

                var tempv = new Enumerate<float>(vSteps, IncrementMethod.Cyclic);
                for (var i = 0; i < s + voffset; i++)
                    tempv.GetNext();
                EchoVolMap.Add(x, tempv);

                var pp = new Enumerate<float>(pSteps, IncrementMethod.Cyclic);
                for (var i = 0; i < s; i++)
                    pp.GetNext();
                PanMap.Add(x, pp);

                var tempp = new Enumerate<float>(pSteps, IncrementMethod.Cyclic);
                for (var i = 0; i < s + poffset; i++)
                    tempp.GetNext();
                EchoPanMap.Add(x, tempp);
            }
        }

        static string path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
        static MidiFile file = new MidiFile(path);
        static float div = file.TicksPerQuarterNote + 0f;

        static (IEnumerable<MidiEvent> OnEvents, IEnumerable<MidiEvent> OffEvents, IEnumerable<float> Durations) result = file.GetEventsAndDurations();
        static Enumerate<MidiEvent> nots = new Enumerate<MidiEvent>(result.OnEvents, IncrementMethod.MinMax);
        static Enumerate<float> durs = new Enumerate<float>(result.Durations, IncrementMethod.MinMax);

        private void Play()
        {
            var times = new[] {
                0, 0, 0, 1,
                0, 0, 2, 0,
                0, 1, 0, 0,
                2, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 2, 0,

                0, 2, 0, 1,
                1, 0, 2, 0,
                0, 1, 0, 2,
                2, 0, 1, 0,
                0, 1, 0, 0,
                2, 0, 2, 0,
            };

            IEnumerable<int> shift(int[] source, int amt, int mult)
            {
                var left = source.Take(amt);
                var right = source.Skip(amt).ToList();
                right.AddRange(left);
                if (mult > 1)
                {
                    right = right.Select(r => r * mult).ToList();
                }
                return right;
            }

            var multen = new Enumerate<int>(new[] { 1, 2, 1, 2, 2, 1, 1, 4, 1, 1, 1, 2, 2, 2, 4, 4 }, IncrementMethod.Cyclic);
            var shifter = new Enumerate<int>(Enumerable.Range(1, times.Count() - 1), IncrementMethod.Cyclic);

            var resultTimes = new List<int>(times);
            while(resultTimes.Count() < nots.Count())
            {
                resultTimes.AddRange(shift(times, shifter.GetNext(), multen.GetNext()));
            }
            resultTimes = resultTimes.Take(nots.Count()).ToList();

            var echos = new Enumerate<int>(resultTimes, IncrementMethod.MinMax);
            for (var i = 0; i < result.Durations.Count(); i++)
            {
                PlayAt(i);
                var echoes = echos.GetNext();
                if (echoes > 1)
                {
                    PlayEchos(i, echoes);
                }
            }

            Clock.SetMaxPitch(Pitch.G5);
            Clock.SetMinPitch(Pitch.C3);
            Clock.ToPrecision(128f);
            Clock.Start();
            Thread.Sleep((int)(nots.Last().Time / div * 1000));
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
            var instr = i % 2 == 0 ? mainInstr : secondInstr;

            var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note, vol.GetNext(), note.Time / div, Clock, dur, pm.GetNext());
            nt.BeforeSendingNoteOnOff += m =>
            {
                OutputDevice.SendProgramChange(m.Channel, instr.GetNext());
            };
            Clock.Schedule(nt);
        }

        private void PlayEchos(int i, int max)
        {
            var notsClone = nots.Clone();
            var durClone = durs.Clone();

            var note = notsClone.GetNext();
            var dur = durClone.GetNext();

            var noteDest = notsClone.PeekAt(max);
            var durDest = durClone.PeekAt(max);

            var notesTimes = new Enumerate<float>(Interpolator.Interpolate(note.Time, noteDest.Time, max, 1));
            var noteDurs = new Enumerate<float>(Interpolator.Interpolate(dur, durDest, max, 1));
            var sm = max + 1;
            var scalers = new Enumerate<float>(Enumerable.Range(sm / 4, sm).Select(x => x / (float)(sm * 1.0125f)).ToList(), IncrementMethod.MaxMin);

            max = i + max;

            for (var j = i; j < max; j++)
            {
                var chan = j % 2 == 0 ? echoChans : echoChans2;
                var ch = chan.GetNext();
                var pm = j % 2 == 0 ? PanMap[ch] : EchoPanMap[ch];
                var vol = j % 2 == 0 ? VolMap[ch] : EchoVolMap[ch];
                var instr = j % 2 == 0 ? echoMainInstr : echoSecondInstr;

                var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note + 12, vol.GetNext() * scalers.GetNext(), (notsClone.Current.Time + notesTimes.GetNext()) / div, Clock, noteDurs.GetNext(), pm.GetNext());
                nt.BeforeSendingNoteOnOff += m =>
                {
                    OutputDevice.SendProgramChange(m.Channel, instr.GetNext());
                };
                Clock.Schedule(nt);

                note = notsClone.GetNext();
                dur = durClone.GetNext();
            }
        }
    }
}