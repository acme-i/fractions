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
    internal class MetronomeExample : ExampleBase
    {
        public MetronomeExample() : base(nameof(MetronomeExample)) { }

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

        private bool inited = false;

        private readonly Enumerate<Channel> chans = new Enumerate<Channel>(Channels.InstrumentChannels, IncrementMethod.MinMax, 1, 0);

        private readonly Instrument[] instrList = new[] {
            Instrument.ElectricPiano1,
            Instrument.ElectricPiano1,
            Instrument.ElectricGuitarMuted,
            Instrument.ElectricPiano1,
        };

        private Enumerate<Instrument> instrs;

        private void Setup(IOutputDevice outputDevice)
        {
            OutputDevice = outputDevice;

            if (!inited)
            {
                inited = true;

                var pSteps = Interpolator.Interpolate(maxLeft, maxRight, 4 * 12, 0);
                var vSteps = Interpolator.Interpolate(minVol, maxVol, 4 * 4, 0);

                var voffset = 8;
                var poffset = 8;
                foreach (var channel in Channels.InstrumentChannels)
                {
                    var s = (int)channel;

                    var vv = new Enumerate<float>(vSteps, IncrementMethod.Cyclic);
                    for (var i = 0; i < s; i++)
                        vv.Next();
                    VolMap.Add(channel, vv);

                    var tempv = new Enumerate<float>(vSteps, IncrementMethod.Cyclic);
                    for (var i = 0; i < s + voffset; i++)
                        tempv.Next();
                    EchoVolMap.Add(channel, tempv);

                    var pp = new Enumerate<float>(pSteps, IncrementMethod.Cyclic);
                    for (var i = 0; i < s; i++)
                        pp.Next();
                    PanMap.Add(channel, pp);

                    var tempp = new Enumerate<float>(pSteps, IncrementMethod.Cyclic);
                    for (var i = 0; i < s + poffset; i++)
                        tempp.Next();
                    EchoPanMap.Add(channel, tempp);
                }

                BPM = 80;
                Clock = new Clock(BPM);
            }

            Play();
        }

        private void Play()
        {
            var path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
            var file = new MidiFile(path);
            var div = file.TicksPerQuarterNote + 0f;

            var durFile = new MidiFile(@".\midifiles\bach_js_bwv1056_harpsichod_concerto_in_fm_2_largo_(arioso).mid");
            var div2 = durFile.TicksPerQuarterNote + 0f;

            var result = file.GetEventsAndDurations();
            var durResult = durFile.GetEventsAndDurations();

            instrs = new Enumerate<Instrument>(instrList, IncrementMethod.Cyclic, 1, 0);
            var max = result.OnEvents.Count();
            var nots = new Enumerate<MidiEvent>(result.OnEvents, IncrementMethod.MinMax, 1, 0);
            var durs = new Enumerate<float>(durResult.Durations, IncrementMethod.MinMax, 1, 0);

            for (var i = 0; i < max; i++)
            {
                var note = nots.Next();
                var ch = chans.Next();
                var dur = durs.Next();
                var pm = i % 2 == 0 ? PanMap[ch] : EchoPanMap[ch];
                var vol = i % 2 == 0 ? VolMap[ch] : EchoVolMap[ch];

                var nt = new NoteOnOffMessage(OutputDevice, ch, (Pitch)note.Note, vol.Next(), note.Time / div2, Clock, dur, pm.Next());
                Clock.Schedule(nt);
            }

            //Clock.AlignDissonants();
            Clock.Start();
            Thread.Sleep(400000);
            Clock.Stop();
        }
    }
}