﻿using System;
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
    internal class FractionsExample2 : ExampleBase
    {
        public FractionsExample2() : base("FractionsExample2 - BWV0999: Prelude in Cm for Lute") { }

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
                Instrument.AcousticBass,
                Instrument.AcousticBass,
                Instrument.AcousticBass,
                Instrument.ElectricPiano1,

                Instrument.AcousticBass,
                Instrument.ElectricPiano1,
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,

                Instrument.AcousticBass,
                Instrument.ElectricBassPick,
                Instrument.AcousticBass,
                Instrument.ElectricPiano1,

                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,
                Instrument.ElectricBassPick,
                Instrument.ElectricPiano1,
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,

                Instrument.AcousticBass,
                Instrument.ElectricPiano1,
                Instrument.AcousticBass,
                Instrument.ElectricPiano1,

                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,
                Instrument.AcousticBass,
                Instrument.ElectricPiano1,
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,

                Instrument.AcousticBass,
                Instrument.ElectricPiano1,
                Instrument.AcousticBass,
                Instrument.ElectricPiano1,

                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,
                Instrument.ElectricGuitarMuted,
                Instrument.ElectricPiano1,
                Instrument.AcousticBass,
                Instrument.ElectricPiano1,
            };

        private Enumerate<Instrument> mainInstr = new Enumerate<Instrument>(
            instruments_,
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> secondInstr = new Enumerate<Instrument>(
            instruments_.Reverse<Instrument>(),
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> echoMainInstr = new Enumerate<Instrument>(
            Instruments.SoftGuitars,
            IncrementMethod.Cyclic);

        private Enumerate<Instrument> echoSecondInstr = new Enumerate<Instrument>(
            Instruments.SoftGuitars.Reverse<Instrument>(),
            IncrementMethod.Cyclic);

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

                var vv = new Enumerate<float>(vSteps, IncrementMethod.Cyclic);
                for (var i = 0; i < s; i++)
                    vv.Next();
                VolMap.Add(x, vv);

                var tempv2 = new Enumerate<float>(vSteps2, IncrementMethod.Cyclic);
                for (var i = 0; i < s + voffset; i++)
                    tempv2.Next();
                EchoVolMap.Add(x, tempv2);

                var pp = new Enumerate<float>(pSteps, IncrementMethod.Cyclic);
                for (var i = 0; i < s; i++)
                    pp.Next();
                PanMap.Add(x, pp);

                var tempp = new Enumerate<float>(pSteps, IncrementMethod.Cyclic);
                for (var i = 0; i < s + poffset; i++)
                    tempp.Next();
                EchoPanMap.Add(x, tempp);

                var tempv = new Enumerate<float>(vSteps, IncrementMethod.Cyclic);
                for (var i = 0; i < s + voffset; i++)
                    tempv.Next();
                EchoVolMap2.Add(x, tempv);

                var temppp = new Enumerate<float>(pSteps2, IncrementMethod.Cyclic);
                for (var i = 0; i < s + poffset; i++)
                    temppp.Next();
                EchoPanMap2.Add(x, temppp);

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

            var echos = new Enumerate<int>(resultTimes, IncrementMethod.MinMax);
            for (var i = 0; i < result.Durations.Count(); i++)
            {
                PlayAt(i);
                var echoes = echos.Next();
                if (echoes > 1)
                {
                    PlayEchos(i, echoes);
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

            var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note, vol.Next(), note.Time / div, Clock, dur, pm.Next());
            nt.BeforeSendingNoteOnOff += m =>
            {
                OutputDevice.SendProgramChange(m.Channel, instr.Next());
            };
            Clock.Schedule(nt);
        }

        Enumerate<int> ps1 = new Enumerate<int>(new[] { 12, 0 });
        Enumerate<int> ps2 = new Enumerate<int>(new[] { 0, 12 });

        private void PlayEchos(int i, int max)
        {
            var notsClone = nots.Clone();
            var durClone = durs.Clone();

            var note = notsClone.Next();
            var dur = durClone.Next();

            var noteDest = notsClone.Peek(max);
            var durDest = durClone.Peek(max);
            var ps = (i + 1) % 2 == 0 ? ps1 : ps2;

            var notesTimes = new Enumerate<float>(Interpolator.Interpolate(note.Time, noteDest.Time, max, 1));
            var noteDurs = new Enumerate<float>(Interpolator.Interpolate(dur, durDest, max, 1));
            var sm = max;
            var scalers = new Enumerate<float>(Enumerable.Range(1, sm).Select(x => x / (float)(sm * 1.25f)).ToList(), IncrementMethod.MaxMin);
            max = i + max;

            for (var j = i; j < max; j++)
            {
                var chan = j % 2 == 0 ? echoChans : echoChans2;
                var ch = chan.Next();
                var pm = EchoPanMap2[ch];
                var vol = EchoVolMap2[ch];
                var instr = j % 2 == 0 ? echoMainInstr : echoSecondInstr;

                var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note + ps.Next(), vol.Next() * scalers.Next(), (notsClone.Current().Time + notesTimes.Next()) / div, Clock, Math.Min(noteDurs.Next(), 0.125f), pm.Next());
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