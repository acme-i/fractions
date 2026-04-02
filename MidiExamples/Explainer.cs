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
                OutputDevice = ExampleUtil.ChooseOutputDeviceFromConsole();
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
        private static readonly int MinVol = 40;
        private static readonly int MaxVol = 100;
        private static readonly int MaxLeft = 20;
        private static readonly int MaxRight = 127;
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

            var panStep = Math.Abs(MaxRight - MaxLeft) / 32.0;
            var volStep = Math.Abs(MaxVol - MinVol) / 32.0;
            var revStep = Math.Abs(MaxRev - MinRev) / 4.0;

            Channels.InstrumentChannels.ForEach(c =>
            {
                OutputDevice.SendProgramChange(c, Instrument.AcousticGrandPiano);

                OutputDevice.SendControlChange(c, Control.Volume, 100);
                OutputDevice.SendControlChange(c, Control.CelesteLevel, 60);
                OutputDevice.SendControlChange(c, Control.ReverbLevel, 60);
            });

            Clock = new Clock(BPM);

            Play();
        }

        private static void Play()
        {
            for (int time = 0; time < MelodiLength; time++)
            {
                var node = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), 80, time, Clock, 1);
                node.Pan = (int)node.Pitch;
                Clock.Schedule(node);
            }

            var start = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), 80, 5, Clock, 1, 0);
            var notes = LinearInterpolator<NoteOnOffMessage>.Interpolate(start, 80, 127, 4, 1, 1, 1, 1);
            notes.ForEach(n =>
            {
                n.Pitch = Melodi1.GetNext();
                n.Pan = (int)n.Pitch;
                Clock.Schedule(n);
            });

            var start1 = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), 80, 10, Clock, 1, 0);
            var notes1 = LinearInterpolator<NoteOnOffMessage>.Interpolate(start1, 64, 127, 64, 1 / 64F, 1, 1, 1);
            notes1.ForEach(n =>
            {
                n.BeforeSendingNoteOnOff += noom =>
                {
                    noom.Pitch = Melodi1.GetNext();
                    noom.Pan = (int)noom.Pitch;
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
                    noom.Pan = (int)noom.Pitch;
                };
                Clock.Schedule(n);
            });

            var volumes = new Enumerate<int>(Enumerable.Range(MinVol, MaxVol), IncrementMethod.Cyclic, 5);
            var pans = new Enumerate<int>(Enumerable.Range(MaxLeft, MaxRight), IncrementMethod.Cyclic, 5);
            var steps = new Enumerate<int>(Enumerable.Range(2, 12), IncrementMethod.Cyclic);
            var volMets = new Enumerate<int>(Enumerable.Range(0, 3), IncrementMethod.Cyclic);
            var durMets = new Enumerate<int>(Enumerable.Range(0, 3), IncrementMethod.Cyclic);
            var panMets = new Enumerate<int>(Enumerable.Range(0, 3), IncrementMethod.Cyclic);
            for (var i = 0; i < 1000; i++)
            {
                var m = Melodi1.GetNext();
                var nt = new NoteOnOffMessage(
                    OutputDevice, 
                    Channel.Channel1,
                    m, 
                    volumes.GetNext(), 
                    12 + i, 
                    Clock, 
                    1, 
                    (int)m
                );
                
                var st = steps.GetNext();
                var nts = LinearInterpolator<NoteOnOffMessage>.Interpolate(nt, volumes.GetNext(), pans.GetNext(), st, 1F / st, volMets.GetNext(), durMets.GetNext(), panMets.GetNext());
                nts.ForEach(n =>
                {
                    //n.Velocity = volumes.GetNext();
                    n.BeforeSendingNoteOnOff += noom =>
                    {
                        noom.Pitch = Melodi1.GetNext();
                        noom.Pan = (int)noom.Pitch;
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