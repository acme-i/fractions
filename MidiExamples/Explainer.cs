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
    internal class Explainer : ExampleBase
    {
        public Explainer() : base(nameof(Explainer)) { }

        public override void Run()
        {
            try
            {
                OutputDevice = fractions.OutputDevice.InstalledDevices.FirstOrDefault();
                if (OutputDevice == null)
                {
                    Console.WriteLine("No output devices, so can't run this example.");
                    ExampleUtil.PressAnyKeyToContinue();
                }
                else
                {
                    OutputDevice.Open();
                    Init();
                    Console.WriteLine();
                    ExampleUtil.PressAnyKeyToContinue();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                OutputDevice?.Close();
            }
        }

        private static readonly int MelodiLength = 4;
        private static readonly int MinRev = 16;
        private static readonly int MaxRev = 70;
        private static readonly int MinVol = 7;
        private static readonly int MaxVol = 120;
        private static readonly int MaxLeft = 30;
        private static readonly int MaxRight = 97;
        private static readonly Dictionary<Channel, Incrementor> VolMap = new Dictionary<Channel, Incrementor>();
        private static readonly Dictionary<Channel, Incrementor> PanMap = new Dictionary<Channel, Incrementor>();
        private static readonly Dictionary<Channel, Incrementor> RevMap = new Dictionary<Channel, Incrementor>();

        private static readonly Enumerate<Channel> chans = new Enumerate<Channel>(
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
                Channel.Channel13,
                Channel.Channel14,
                Channel.Channel15,
                Channel.Channel16
            }, IncrementMethod.MinMax, 1, 0);

        private static IOutputDevice OutputDevice;
        private static Clock Clock;
        private static int BPM;

        private static Enumerate<Pitch> Melodi1;

        private static void Init()
        {
            BPM = 64;

            var path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
            var file = new MidiFile(path);
            var div = file.TicksPerQuarterNote + 0f;

            var pitches = file.GetEventsAndDurations().OnEvents.Select(o => (Pitch)o.Note);

            Melodi1 = new Enumerate<Pitch>(pitches, IncrementMethod.MinMax, 1);

            OutputDevice.SendProgramChange(Channel.Channel1, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel2, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel3, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel4, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel5, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel6, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel7, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel8, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel9, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel11, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel12, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel13, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel14, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel15, Instrument.Vibraphone);
            OutputDevice.SendProgramChange(Channel.Channel16, Instrument.Vibraphone);

            var panStep = Math.Abs(MaxRight - MaxLeft) / 16.0;
            var volStep = Math.Abs(MaxVol - MinVol) / 16.0;
            var revStep = Math.Abs(MaxRev - MinRev) / 16.0;
            for (var x = Channel.Channel1; x <= Channel.Channel16; x++)
            {
                OutputDevice.SendProgramChange(x, Instrument.ElectricGuitarMuted);

                VolMap.Add(x, new Incrementor((double)x * volStep, MinVol, MaxVol, volStep, IncrementMethod.MaxMin));
                PanMap.Add(x, new Incrementor((double)x * panStep, MaxLeft, MaxRight, panStep, IncrementMethod.Cyclic));
                RevMap.Add(x, new Incrementor((double)x * revStep, MinRev, MaxRev, revStep, IncrementMethod.Cyclic));

                OutputDevice.SendControlChange(x, Control.Volume, 100);
                OutputDevice.SendControlChange(x, Control.CelesteLevel, 0);
                OutputDevice.SendControlChange(x, Control.ReverbLevel, 100);
            }

            Clock = new Clock(BPM);

            Play();
        }

        private static void Play()
        {
            Console.WriteLine($"Play C3 on time. Length {MelodiLength}, Duration 1, Velocity 80");
            for (int time = 0; time < MelodiLength; time++)
            {
                Clock.Schedule(new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.Next(), 80, time, Clock, 1));
            }

            Console.WriteLine($"Play C3 on time panning left to right");
            var start = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.Next(), 80, 5, Clock, 1, 0);
            var notes = LinearInterpolator<NoteOnOffMessage>.Interpolate(start, 80, 127, 4, 1, 1, 1, 1);
            notes.ForEach((NoteOnOffMessage n) =>
            {
                n.Pitch = Melodi1.Next();
                Clock.Schedule(n);
            });

            var start1 = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.Next(), 80, 10, Clock, 1, 0);
            var notes1 = LinearInterpolator<NoteOnOffMessage>.Interpolate(start1, 64, 127, 64, 1 / 64F, 1, 1, 1);
            notes1.ForEach((NoteOnOffMessage n) =>
            {
                n.BeforeSendingNoteOnOff += (NoteOnOffMessage a) =>
                {
                    a.Pitch = Melodi1.Next();
                };
            });

            var start2 = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.Next(), 64, 11, Clock, 1, 127);
            var notes2 = LinearInterpolator<NoteOnOffMessage>.Interpolate(start2, 80, 0, 64, 1 / 64F, 1, 1, 1);
            notes2.ForEach((NoteOnOffMessage n) =>
            {
                //n.Pitch = Melodi2.Next();
                n.BeforeSendingNoteOnOff += (NoteOnOffMessage a) =>
                {
                    //OutputDevice.SendControlChange(a.Channel, Control.Pan, (int)a.Pan);
                    a.Pitch = Melodi1.Next();
                };
                Clock.Schedule(n);
            });

            var volumes = new Enumerate<int>(Enumerable.Range(50, 80), IncrementMethod.Cyclic, 11);
            var pans = new Enumerate<int>(Enumerable.Range(0, 127), IncrementMethod.Cyclic, 7);
            var steps = new Enumerate<int>(Enumerable.Range(2, 12), IncrementMethod.Cyclic);
            var volMets = new Enumerate<int>(Enumerable.Range(0, 3), IncrementMethod.Cyclic);
            var durMets = new Enumerate<int>(Enumerable.Range(0, 3), IncrementMethod.Cyclic);
            var panMets = new Enumerate<int>(Enumerable.Range(0, 3), IncrementMethod.Cyclic);
            for (var i = 0; i < 1000; i++)
            {
                var nt = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.Next(), volumes.Next(), 12 + i, Clock, 1, 127 - pans.Next());
                var st = steps.Next();
                var nts = LinearInterpolator<NoteOnOffMessage>.Interpolate(nt, volumes.Next(), pans.Next(), st, 1F / st, volMets.Next(), durMets.Next(), panMets.Next());
                nts.ForEach((NoteOnOffMessage n) =>
                {
                    //n.Velocity = volumes.Next();
                    n.BeforeSendingNoteOnOff += (NoteOnOffMessage a) =>
                    {
                        a.Pitch = Melodi1.Next();
                        //OutputDevice.SendControlChange(a.Channel, Control.Pan, (int)a.Pan);
                        //OutputDevice.SendControlChange(a.Channel, Control.ReverbLevel, 127 - (int)a.Velocity);
                    };
                    Clock.Schedule(n);
                });
            }

            Clock.Start();
            Thread.Sleep(1000 * BPM * 60);
            Clock.Stop();
        }
    }
}