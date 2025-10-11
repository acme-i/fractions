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
    /// main thread, forcing the user to sit and wait. See Example03.cs for n more realistic example
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

            Melodi1 = file.GetEventsAndDurations().OnEvents.Select(o => (Pitch)o.Note).AsEnumeration();

            var panStep = Math.Abs(MaxRight - MaxLeft) / 16.0;
            var volStep = Math.Abs(MaxVol - MinVol) / 16.0;
            var revStep = Math.Abs(MaxRev - MinRev) / 16.0;

            Channels.InstrumentChannels.ForEach(c =>
            {
                OutputDevice.SendProgramChange(c, Instrument.ElectricGuitarMuted);

                VolMap.Add(c, new Incrementor((double)c * volStep, MinVol, MaxVol, volStep, IncrementMethod.MaxMin));
                PanMap.Add(c, new Incrementor((double)c * panStep, MaxLeft, MaxRight, panStep, IncrementMethod.Cyclic));
                RevMap.Add(c, new Incrementor((double)c * revStep, MinRev, MaxRev, revStep, IncrementMethod.Cyclic));

                OutputDevice.SendControlChange(c, Control.Volume, 100);
                OutputDevice.SendControlChange(c, Control.CelesteLevel, 0);
                OutputDevice.SendControlChange(c, Control.ReverbLevel, 100);
            });

            Clock = new Clock(BPM);

            Play();
        }

        private static void Play()
        {
            for (int time = 0; time < MelodiLength; time++)
                Clock.Schedule(new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), 80, time, Clock, 1));

            var start = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), 80, 5, Clock, 1, 0);
            var notes = LinearInterpolator<NoteOnOffMessage>.Interpolate(start, 80, 127, 4, 1, 1, 1, 1);
            notes.ForEach(n =>
            {
                n.Pitch = Melodi1.GetNext();
                Clock.Schedule(n);
            });

            var start1 = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), 80, 10, Clock, 1, 0);
            var notes1 = LinearInterpolator<NoteOnOffMessage>.Interpolate(start1, 64, 127, 64, 1 / 64F, 1, 1, 1);
            notes1.ForEach(n =>
            {
                n.BeforeSendingNoteOnOff += noom =>
                {
                    noom.Pitch = Melodi1.GetNext();
                };
            });

            var start2 = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), 64, 11, Clock, 1, 127);
            var notes2 = LinearInterpolator<NoteOnOffMessage>.Interpolate(start2, 80, 0, 64, 1 / 64F, 1, 1, 1);
            notes2.ForEach(n =>
            {
                //n.Pitch = Melodi2.GetNext();
                n.BeforeSendingNoteOnOff += noom =>
                {
                    noom.Pitch = Melodi1.GetNext();
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
                var nt = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), volumes.GetNext(), 12 + i, Clock, 1, 127 - pans.GetNext());
                var st = steps.GetNext();
                var nts = LinearInterpolator<NoteOnOffMessage>.Interpolate(nt, volumes.GetNext(), pans.GetNext(), st, 1F / st, volMets.GetNext(), durMets.GetNext(), panMets.GetNext());
                nts.ForEach(n =>
                {
                    //n.Velocity = volumes.GetNext();
                    n.BeforeSendingNoteOnOff += noom =>
                    {
                        noom.Pitch = Melodi1.GetNext();
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