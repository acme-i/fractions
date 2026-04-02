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
    /// Thread.Sleep for timing, which isn'offset practical in real applications because it blocks the
    /// main thread, forcing the user to sit and wait. See Example03.cs for n more realistic example
    /// using Clock for scheduling.
    /// </remarks>
    internal class SpiralingEchoes : ExampleBase
    {
        public SpiralingEchoes() : base(nameof(SpiralingEchoes)) { }

        public override void Run()
        {
            try
            {
                OutputDevice = ExampleUtil.ChooseOutputDeviceFromConsole();
                if (OutputDevice == null)
                {
                    Console.WriteLine("No output devices, so can'offset run this example.");
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

        private static IOutputDevice OutputDevice;
        private static Clock Clock;
        private static int BPM;
        private static float DIV;
        private static Enumerate<float> On1, On2;
        private static Enumerate<Pitch> Melodi1, Melodi2;
        private static Enumerate<float> Dur1, Dur2;
        private static int MelodiLength;

        private static void Init()
        {
            var path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
            var file = new MidiFile(path);

            BPM = 68;
            DIV = file.TicksPerQuarterNote + 0f;

            //Channels.InstrumentChannels.ForEach(c =>
            //{
            //    OutputDevice.SendProgramChange(c, Instrument.AcousticGrandPiano);

            //    OutputDevice.SendControlChange(c, Control.Volume, 100);
            //    OutputDevice.SendControlChange(c, Control.CelesteLevel, 60);
            //    OutputDevice.SendControlChange(c, Control.ReverbLevel, 60);
            //});

            var query = file.GetEventsAndDurations();

            On1 = query.OnEvents.Select(o => (float)o.Time / DIV).AsEnumeration();
            On2 = On1.Clone();

            Melodi1 = query.OnEvents.Select(o => (Pitch)o.Note).AsEnumeration();
            Melodi2 = Melodi1.Clone();

            Dur1 = query.Durations.AsEnumeration();
            Dur2 = Dur1.Clone();

            MelodiLength = Melodi1.Count;

            Clock = new Clock(BPM);

            Play();
        }

        private static void Play()
        {
            var maxLeft = 20;
            var maxRight = 107.0;

            var minVol = 40.0;
            var maxVol = 107.0;

            var panList = new List<double>();
            var volList = new List<double>();
            var methods = new[] { 0, 1, 2, 3 }.AsCycle();
            var volMethods = new[] { 0, 1, 2, 3 }.AsCycle();
            var steps = new[] { 1, 2, 3, 4 }.AsCycle();
            var volSteps = new[] { 4, 5, 6, 7, 8, 9, 10 }.AsCycle();

            for (var i = 0; i < 4; i++)
            {
                var pans = Interpolator.Interpolate(maxLeft, maxRight, 1, methods.GetNext());
                if (i % 2 != 0) pans.Reverse();

                var vols = Interpolator.Interpolate(minVol, maxVol, 1, volMethods.GetNext());
                if (i % 2 != 0) vols.Reverse();

                panList.AddRange(pans);
                volList.AddRange(vols);
            }

            var leftPans = panList.AsCycle();
            var rightPans = leftPans.AsReversed();

            var leftVols = volList.AsCycle();
            var rightVols = leftVols.AsReversed();

            var leftMult = Enumerable.Range(80, 120).Select(f => f / 100f).Select(f => Math.Round(f)).AsCycle(step: 7);
            var rightMult = leftMult.AsReversed().AsCycle();

            var channels = Channels.InstrumentChannels.AsCycle();
            var guitars = Instruments.SoftGuitars.AsCycle();

            //channels.ForEach(c =>
            //{
            //    OutputDevice.SendProgramChange(c, guitars.GetNext());
            //});

            var leftPathStepper = Enumerable.Range(1, 7).AsCycle();
            var rightPanStepper = leftPathStepper.AsReversed();

            var echoes = Enumerable.Range(2, 64).Where(e => e % 2 == 0).AsCycle(step: 7);

            var echostepper1 = new[] { 3, 7 }.AsCycle();

            var leftVolStepper = Enumerable.Range(1, 7).AsCycle(startIndex: 3);
            var leftMulStepper = Enumerable.Range(1, 7).AsCycle();

            var rightMulStepper = leftMulStepper.AsReversed();
            var rightVolStepper = leftVolStepper.AsReversed();


            void IncreaseSteppersLeftRight(bool left, bool right)
            {
                echoes.Incrementor.SetStepSize(echostepper1.GetNext());

                if (left)
                {
                    leftPans.Incrementor.SetStepSize(leftPathStepper.GetNext());
                    leftVols.Incrementor.SetStepSize(leftVolStepper.GetNext());
                    leftMult.Incrementor.SetStepSize(leftMulStepper.GetNext());
                }
                if (right)
                {
                    rightPans.Incrementor.SetStepSize(rightPanStepper.GetNext());
                    rightVols.Incrementor.SetStepSize(rightVolStepper.GetNext());
                    rightMult.Incrementor.SetStepSize(rightMulStepper.GetNext());
                }
            }

            void IncreaseSteppers()
            {
                IncreaseSteppersLeftRight(true, true);
            }

            var mods = new[] {
                3, 7, 4, 8, 16
            }.AsEnumeration(step: 3);

            var offset = 0f;

            var echoChannel = new[] { Channel.Channel2, Channel.Channel1, Channel.Channel3 }.AsCycle();
            var octaves = new[] { 2 * 12, 3 * 12, 2 * 12, 3 * 12, 3 * 12, 3 * 12, 4 * 12, 4 * 12, 4 * 12, 4 * 12, 3 * 12, 3 * 12, 2 * 12 }.AsCycle();

            for (int time = 0; time < MelodiLength; time++)
            {
                var t1 = On1.GetNext();
                float t2 = t1;

                On2.GetNext();

                var peekAt = 1;
                while (true && time < MelodiLength - 1)
                {
                    t2 = On2.PeekAt(peekAt++);
                    if (t2 > t1 || peekAt > 100) break;
                }

                var dur = Dur1.GetNext();

                IncreaseSteppersLeftRight(true, false);

                Clock.Schedule(new NoteOnOffMessage(OutputDevice, Channel.Channel1, Melodi1.Current, leftVols.Current, t1, Clock, dur)
                {
                    Pan = leftPans.GetNext()
                });

                if (time > 0 && t1 < t2)
                {
                    var nextMods = mods.GetNext();

                    if (time % nextMods == 0)
                    {
                        var numEchoes = echoes.GetNext();

                        for (var e = 1; e <= numEchoes && e <= 64; e++)
                        {
                            var fraction = (t2 - t1) / numEchoes;

                            if (float.IsInfinity(fraction))
                                continue;

                            offset = fraction * e;

                            if (float.IsInfinity(offset))
                                break;

                            IncreaseSteppersLeftRight(false, true);
                            Clock.Schedule(new NoteOnOffMessage(OutputDevice, echoChannel.GetNext(), Melodi1.Current + octaves.GetNext(), 1, t1 + offset, Clock, Math.Abs(dur - offset))
                            {
                                Pan = rightPans.GetNext()
                            });

                            if (t1 + offset >= t2) break;
                        }
                    }
                }

                Melodi1.GetNext();
            }

            Clock.Start();
            Thread.Sleep((int)(DIV * 1000 * MelodiLength));
            Clock.Stop();
        }
    }
}

