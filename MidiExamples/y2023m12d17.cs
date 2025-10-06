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
    internal class y2023m12d17 : ExampleBase
    {
        public y2023m12d17() : base(nameof(y2023m12d17)) { }

        public override void Run()
        {
            var outputDevice = fractions.OutputDevice.InstalledDevices.FirstOrDefault();
            if (outputDevice == null)
            {
                Console.WriteLine("No output devices, so can't run this example.");
                ExampleUtil.PressAnyKeyToContinue();
                return;
            }
            outputDevice.Open();

            Setup(outputDevice);

            outputDevice.Close();

            Console.WriteLine();
            ExampleUtil.PressAnyKeyToContinue();
        }

        private readonly int minVol = 47;
        private readonly int maxVol = 107;
        private readonly int maxLeft = 4;
        private readonly int maxRight = 127;
        private readonly Dictionary<Channel, Enumerate<float>> VolMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoVolMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> PanMap = new Dictionary<Channel, Enumerate<float>>();
        private readonly Dictionary<Channel, Enumerate<float>> EchoPanMap = new Dictionary<Channel, Enumerate<float>>();

        private IOutputDevice OutputDevice;
        private Clock Clock;
        private int BPM;

        private readonly Enumerate<Channel> chans = new Enumerate<Channel>(
            new[] {
                Channel.Channel1,
                Channel.Channel2,
                Channel.Channel3,
                Channel.Channel4,
                Channel.Channel5,
                Channel.Channel6,
                Channel.Channel7,
                Channel.Channel8,
                Channel.Channel9,
                Channel.Channel11,
                Channel.Channel12,
            }, IncrementMethod.MinMax, 1, 0);

        private readonly Enumerate<Channel> stringChans = new Enumerate<Channel>(
            new[] {
                Channel.Channel13,
                Channel.Channel14,
                Channel.Channel15,
                Channel.Channel16
            }, IncrementMethod.MinMax, 1, 0);

        private readonly Instrument[] instrList = new[] {
            Instrument.AcousticBass,
            Instrument.AcousticBass,
            Instrument.AcousticBass,
            Instrument.ElectricPiano1,

            Instrument.ElectricGuitarJazz,
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
            Instrument.ElectricGuitarJazz,
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
            Instrument.ElectricGuitarJazz,
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
            Instrument.ElectricGuitarJazz,
            Instrument.ElectricPiano1,
        };

        private readonly Instrument[] stringInstrList = new[] {
            Instrument.StringEnsemble1,
            Instrument.StringEnsemble2,
            Instrument.Viola,
            Instrument.Violin,
        };

        private Enumerate<Instrument> instrs;
        private Enumerate<Instrument> stringInstrs;

        private void Setup(IOutputDevice outputDevice)
        {
            OutputDevice = outputDevice;

            var channels = Channels.InstrumentChannels;
            //foreach (var x in channels)
            //{
            //    OutputDevice.SendControlChange(x, Control.Volume, 100);
            //    OutputDevice.SendControlChange(x, Control.CelesteLevel, 0);
            //    OutputDevice.SendControlChange(x, Control.TremoloLevel, 25);
            //    OutputDevice.SendControlChange(x, Control.ReverbLevel, 100);
            //}

            var pSteps = Interpolator.Interpolate(maxLeft, maxRight, 4 * 12, 0);
            var vSteps = Interpolator.Interpolate(minVol, maxVol, 4 * 4, 0);

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

            BPM = 82;
            Clock = new Clock(BPM);

            Play();
        }

        private void Play()
        {
            var path = @".\midifiles\bach_js_bwv1007_cello_suite_no1_in_g_1_prelude.mid";
            var file = new MidiFile(path);
            var div = file.TicksPerQuarterNote + 0f;

            var result = file.GetEventsAndDurations();

            var nots = new Enumerate<MidiEvent>(result.OnEvents, IncrementMethod.MinMax, 1, 0);
            var durs = new Enumerate<float>(result.Durations, IncrementMethod.MinMax, 1, 0);

            instrs = new Enumerate<Instrument>(instrList, IncrementMethod.Cyclic, 1, 0);
            stringInstrs = new Enumerate<Instrument>(stringInstrList, IncrementMethod.Cyclic, 1, 0);
            for (var i = 0; i < result.Durations.Count(); i++)
            {
                var note = nots.GetNext();
                var ch = chans.GetNext();
                var dur = durs.GetNext();
                var pm = i % 2 == 0 ? PanMap[ch] : EchoPanMap[ch];
                var vol = i % 2 == 0 ? VolMap[ch] : EchoVolMap[ch];

                var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note, vol.GetNext(), note.Time / div, Clock, dur, pm.GetNext());
                nt.BeforeSendingNoteOnOff += m =>
                {
                    OutputDevice.SendProgramChange(m.Channel, instrs.GetNext());
                };
                Clock.Schedule(nt);

                if (i % 4 == 0 || i % 9 == 0 || i % 12 == 0)
                {
                    ch = stringChans.GetNext();
                    var ns = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note, vol.Current, note.Time / div, Clock, dur / div, pm.Current);
                    ns.BeforeSendingNoteOnOff += m =>
                    {
                        OutputDevice.SendProgramChange(m.Channel, stringInstrs.GetNext());
                    };
                    Clock.Schedule(ns);
                }
            }

            //Clock.AlignDissonants();
            Clock.Start();
            Thread.Sleep(400000);
            Clock.Stop();
        }
    }
}