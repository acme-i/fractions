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
    internal class bach_js_bwv0999_prelude_in_cm_for_lute : ExampleBase
    {
        public bach_js_bwv0999_prelude_in_cm_for_lute() : base(nameof(bach_js_bwv0999_prelude_in_cm_for_lute)) { }

        public override void Run()
        {
            try
            {
                outputDevice = OutputDevice.InstalledDevices.FirstOrDefault();
                if (outputDevice == null)
                {
                    Console.WriteLine("No output devices, so can't run this example.");
                    ExampleUtil.PressAnyKeyToContinue();
                }
                else
                {
                    outputDevice.Open();
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
                outputDevice?.Close();
            }
        }

        private static IOutputDevice outputDevice;
        private static Clock clock;
        private static int BPM;

        private static Enumerate<Pitch> melodi1, melodi2, melodi3;
        private static Enumerate<Channel> leftChan, rightChan;
        private static Enumerate<double> leftPan, leftVol, rightPan, rightVol;
        private static List<Pitch> pitches;
        private static MidiFile file;

        private static void Init()
        {
            BPM = 320;

            var path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
            
            file = new MidiFile(path);

            var ons = file.GetEventsAndDurations().OnEvents.Where(f => f.MidiEventType == MidiEventType.NoteOn);

            pitches = ons.Select(o => (Pitch)o.Note).ToList();

            melodi1 = new Enumerate<Pitch>(pitches, IncrementMethod.MinMax, 1);
            melodi2 = new Enumerate<Pitch>(pitches, IncrementMethod.MinMax, 1);
            melodi3 = new Enumerate<Pitch>(pitches, IncrementMethod.MinMax, 1);

            var pcurve = new List<double>();
            var vcurve = new List<double>();
            var eIn = Interpolator.EaseInFunctions();
            var eOut = Interpolator.EaseOutFunctions();
            for (var part = 0; part < eIn.Count; part++)
            {
                var ppoints = Interpolator.InOutCurve(0.10, 0.90, 6, 6, eIn[part], eOut[part]);
                var vpoints = Interpolator.InOutCurve(0.60, 0.90, 6, 6, eIn[part], eOut[part]);
                pcurve.AddRange(ppoints.Select(e => Math.Max(0, Math.Min(127, e * 127))));
                vcurve.AddRange(vpoints.Select(e => Math.Max(0, Math.Min(127, e * 127))));
            }

            leftChan = new Enumerate<Channel>(Channels.InstrumentChannels.Take(8), step: 1);
            rightChan = new Enumerate<Channel>(Channels.InstrumentChannels.Skip(8).Take(7), step: 1);

            leftPan = new Enumerate<double>(pcurve, IncrementMethod.Cyclic);
            leftVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic);

            rightPan = new Enumerate<double>(pcurve.AsEnumerable().Reverse(), IncrementMethod.Cyclic);
            rightVol = new Enumerate<double>(vcurve.AsEnumerable().Reverse(), IncrementMethod.Cyclic);

            foreach (var c in Channels.InstrumentChannels)
            {
                outputDevice.SendProgramChange(c, Instrument.Vibraphone);
                outputDevice.SendControlChange(c, Control.Volume, 100);
                outputDevice.SendControlChange(c, Control.CelesteLevel, 0);
                outputDevice.SendControlChange(c, Control.ReverbLevel, 100);
            }
            
            clock = new Clock(BPM);

            Play();
        }

        private static void Play()
        {
            var time = 1f;
            foreach (var p in pitches.Take(12*42+1))
            {

                var a = new NoteOnMessage(outputDevice, leftChan.Next(), melodi1.Next(), leftVol.Next(), time, leftPan.Next());

                a.BeforeSending += (e) =>
                {
                    Console.WriteLine(e.Time);
                };

                var b = new NoteOnMessage(outputDevice, rightChan.Next(), melodi2.Next(), rightVol.Next(), time, rightPan.Next());

                if (time % 2 == 0)
                    clock.Schedule(a);
                else
                    clock.Schedule(b);

                time++;
            }

            clock.Start();
            Thread.Sleep(504 * 1000);
            clock.Stop();
        }
    }
}