using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace fractions.examples
{
    /// <summary>Demonstrates simple single-threaded output</summary>
    /// <remarks>
    /// This example uses the OutputDevice.Send* methods directly to generate output. It uses
    /// Thread.Sleep for timing, which isn't practical in real applications because it blocks the
    /// main thread, forcing the user to sit and wait. See Example03.cs for n more realistic example
    /// using Clock for scheduling.
    /// </remarks>
    internal class Explainer2 : ExampleBase
    {
        public Explainer2() : base(nameof(Explainer2)) { }

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
        private static readonly double MinRev = 16.0;
        private static readonly double MaxRev = 70.0;
        private static readonly int MinVol = 70;
        private static readonly int MaxVol = 100;
        private static readonly int MaxLeft = 20;
        private static readonly int MaxRight = 127;
        private static IOutputDevice OutputDevice;
        private static Clock Clock;
        private static int BPM;
        private static float DIV;

        private static Enumerate<Pitch> Melodi1;
        private static Enumerate<Pitch> Melodi2;
        private static Enumerate<Pitch> Melodi3;
        private static Enumerate<Pitch> Melodi4;
        private static Enumerate<Pitch> Melodi5;
        private static Enumerate<int> PAN = new Enumerate<int>(
            Enumerable.Range(0, 20),
            IncrementMethod.Cyclic, 7);


        private static Enumerate<int> PAN1;
        private static Enumerate<int> PAN2;
        private static Enumerate<int> PAN3;
        
        private static void Init()
        {
            PAN.AddRange(Enumerable.Range(0, 20).Select(i => -1 * i));

            PAN1 = PAN.Clone();
            PAN2 = PAN.Clone();
            PAN3 = PAN.Clone();

            var path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
            var file = new MidiFile(path);

            BPM = 62;
            DIV = file.TicksPerQuarterNote + 0f;

            Melodi1 = file.GetEventsAndDurations().OnEvents.Select(o => (Pitch)o.Note).AsEnumeration();
            Melodi2 = Melodi1.Clone();
            Melodi3 = Melodi1.Clone();
            Melodi4 = Melodi1.Clone();
            Melodi5 = Melodi1.Clone();

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
            for(int x1=0; x1 < 4; x1++)
            {
                PAN1.GetNext();
                PAN2.GetNext();
                PAN3.GetNext();
            }
            for (int x1 = 0; x1 < 4; x1++)
            {
                PAN2.GetNext();
                PAN3.GetNext();
            }
            for (int x1 = 0; x1 < 4; x1++)
                PAN3.GetNext();


            var Methods1 = new[] { 0, 1, 2, 3 }.AsCycle();
            var Methods2 = new[] { 0, 1, 2, 3 }.AsCycle();
            var Methods3 = new[] { 0, 1, 2, 3 }.AsCycle();
            var VelMethods1 = new[] { 0, 1, 2, 3 }.AsCycle();
            var VelMethods2 = new[] { 0, 1, 2, 3 }.AsCycle();
            var VelMethods3 = new[] { 0, 1, 2, 3 }.AsCycle();
            var Steps1 = new[] { 4f, 8f, 16f }.AsCycle();
            var Steps2 = new[] { 4f, 8f, 16f }.AsCycle();
            var Steps3 = new[] { 4f, 8f, 16f }.AsCycle();
            var Times1 = new[] { DIV * 4f, DIV * 6f, DIV * 8f }.Reverse().AsCycle();
            var Times2 = new[] { DIV * 2f, DIV * 4f, DIV * 6f }.Reverse().AsCycle();
            var Times3 = new[] { DIV * 3f, DIV * 6f, DIV * 12f }.Reverse().AsCycle();

            Methods2.GetNext();
            Methods3.GetNext();
            Methods3.GetNext();

            VelMethods2.GetNext();
            VelMethods3.GetNext();
            VelMethods3.GetNext();

            Steps2.GetNext();
            Steps3.GetNext();
            Steps3.GetNext();

            var revs = new Enumerate<double>(Interpolator.Interpolate(MinRev, MaxRev, 12, 1), IncrementMethod.Cyclic, 1);

            for (int time = 0; time < MelodiLength; time++)
            {
                var node = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), 80, time, Clock, 1);
                node.Pan = (int)node.Pitch;
                node.Reverb = revs.GetNext();
                Clock.Schedule(node);
            }

            var s1 = Steps1.GetNext();
            var s2 = Steps2.GetNext();
            var s3 = Steps3.GetNext();

            var start = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), 80, Times1.GetNext(), Clock, 1);
            var notes = LinearInterpolator<NoteOnOffMessage>.Interpolate(start, 80, 127, (int)Math.Round(s1), 1 / s1, VelMethods1.GetNext(), Methods1.GetNext());
            notes.ForEach(n =>
            {
                n.Pitch = Melodi2.GetNext();
                n.Pan = Math.Abs(PAN.GetNext() - (int)((float)n.Pitch));
                n.Reverb = revs.GetNext();
                Clock.Schedule(n);
            });

            var start1 = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), (80 + 64) / 2.0, Times2.GetNext(), Clock, 1);
            var notes1 = LinearInterpolator<NoteOnOffMessage>.Interpolate(start1, 64, 127, (int)Math.Round(s2), 1 / s2, VelMethods2.GetNext(), Methods2.GetNext());
            notes1.ForEach(n =>
            {
                n.BeforeSendingNoteOnOff += noom =>
                {
                    noom.Pitch = Melodi3.GetNext();
                    noom.Pan = Math.Abs(PAN1.GetNext() - (int)((float)noom.Pitch));
                    noom.Reverb = revs.GetNext();
                };
            });

            var start2 = new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.GetNext(), 64, Times3.GetNext(), Clock, 1);
            var notes2 = LinearInterpolator<NoteOnOffMessage>.Interpolate(start2, 80, 0, (int)Math.Round(s3), 1 / s3, VelMethods3.GetNext(), Methods3.GetNext());
            notes2.ForEach(n =>
            {
                //n.Pitch = Melodi2.GetNext();
                n.BeforeSendingNoteOnOff += noom =>
                {
                    noom.Pitch = Melodi4.GetNext();
                    noom.Pan = Math.Abs(PAN2.GetNext() - (int)((float)noom.Pitch));
                    noom.Reverb = revs.GetNext();
                };
                Clock.Schedule(n);
            });

            var volumes = new Enumerate<int>(Enumerable.Range(MinVol, MaxVol), IncrementMethod.Cyclic, 5);
            var pans = new Enumerate<int>(Enumerable.Range(MaxLeft, MaxRight), IncrementMethod.Cyclic, 5);
            var steps = new Enumerate<int>(Enumerable.Range(4, 12), IncrementMethod.Cyclic);
            var volMets = new Enumerate<int>(Enumerable.Range(0, 3), IncrementMethod.Cyclic);
            var durMets = new Enumerate<int>(Enumerable.Range(0, 3), IncrementMethod.Cyclic);
            var panMets = new Enumerate<int>(Enumerable.Range(0, 3), IncrementMethod.Cyclic);
            var revMets = new Enumerate<int>(Enumerable.Range(0, 3), IncrementMethod.Cyclic);

            Melodi1.Reset();
            for (var i = 0; i < MelodiLength * 1000; i++)
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
                    (int)m,
                    instrument: null,
                    reverb: revs.GetNext()
                );

                var st = steps.GetNext();
                var nts = LinearInterpolator<NoteOnOffMessage>.Interpolate(
                    nt,
                    volumes.GetNext(), pans.GetNext(), revs.GetNext(),
                    st, 1F / st,
                    volMets.GetNext(), durMets.GetNext(), panMets.GetNext(), revMets.GetNext()
                );
                nts.ForEach(n =>
                {
                    //n.Velocity = volumes.GetNext();
                    n.BeforeSendingNoteOnOff += noom =>
                    {
                        noom.Pitch = Melodi5.GetNext();
                        noom.Pan = Math.Abs(PAN3.GetNext() - (int)((float)noom.Pitch));
                        noom.Reverb = revs.GetNext();
                    };
                    Clock.Schedule(n);
                });
            }

            Clock.RemoveIdenticalNotes(PitchRange.Default);

            Melodi1.Reset();
            Melodi2.Reset();
            Melodi3.Reset();
            Melodi4.Reset();
            Melodi5.Reset();

            Melodi2.GetNext();
            Melodi3.GetNext();
            Melodi3.GetNext();
            Melodi4.GetNext();
            Melodi4.GetNext();
            Melodi4.GetNext();
            Melodi5.GetNext();
            Melodi5.GetNext();
            Melodi5.GetNext();
            Melodi5.GetNext();

            Clock.Start();
            Thread.Sleep(1000 * BPM * 60);
            Clock.Stop();
        }
    }
}