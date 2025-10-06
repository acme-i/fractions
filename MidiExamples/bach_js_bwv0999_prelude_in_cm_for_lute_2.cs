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
    internal class bach_js_bwv0999_prelude_in_cm_for_lute_2 : ExampleBase
    {
        #region Constructors

        public bach_js_bwv0999_prelude_in_cm_for_lute_2() : base(nameof(bach_js_bwv0999_prelude_in_cm_for_lute_2)) { }

        #endregion Constructors

        #region Methods

        private IOutputDevice outputDevice;
        private Clock clock;
        private readonly int BPM = 70;

        public override void Run()
        {
            // Prompt user to choose an output device (or if there is only one, use that one).
            outputDevice = OutputDevice.InstalledDevices.FirstOrDefault();
            if (outputDevice == null)
            {
                Console.WriteLine("No output devices, so can't run this example.");
                ExampleUtil.PressAnyKeyToContinue();
                return;
            }
            outputDevice.Open();

            Goahead(299040);

            // Close the output device.
            outputDevice.Close();

            // All done.
            Console.WriteLine();
            ExampleUtil.PressAnyKeyToContinue();
        }

        private void Goahead(int seed)
        {
            //var seed = 299041; // 299040;
            Interpolator.Seed = seed;
            var rand = new Random(seed);

            //string[] files = Directory.GetFiles(@".\midifiles\", "*.mid");
            //string path = files[rand.GetNext(files.Count())];
            var path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";

            Console.WriteLine(path);
            Console.WriteLine(seed);


            var file = new MidiFile(path);
            var div = (float)file.TicksPerQuarterNote;
            var chans = Channels.InstrumentChannels.AsEnumeration();

            var leftInstr = Instruments.SoftGuitars.AsEnumeration();
            var leftChans = Channels.Range(Channel.Channel1, Channel.Channel9).AsEnumeration();
            leftChans.ForEach(x =>
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 0);
                outputDevice.SendControlChange(x, Control.Volume, DeviceBase.ControlChangeMax);
                outputDevice.SendProgramChange(x, leftInstr.GetNext());
            });

            var rightInstr = Instruments.SoftBasses.AsEnumeration();
            var rightChans = Channels.Range(Channel.Channel11, Channel.Channel16).AsEnumeration();
            rightChans.ForEach(x =>
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 100);
                outputDevice.SendControlChange(x, Control.Volume, DeviceBase.ControlChangeMax);
                outputDevice.SendProgramChange(x, rightInstr.GetNext());
            });

            clock = new Clock(BPM);

            var stepsIn = new[] { 1, 2, 4, 8 }.AsCycle();
            var stepsOut = stepsIn.AsReversed();
            var maxEchos1 = new[] { 1, 2, 4, 1, 2, 4, 1, 2, 4, 1, 2, 4, 2, 8, 16, 2, 8, 16, 2, 8, 16, 4, 16, 32 }.AsCycle();
            var maxEchos2 = maxEchos1.AsReversed();

            var pcurve = new List<double>();
            var vcurve = new List<double>();
            var efs = Interpolator.EaseFunctions();
            efs.ForEach(ef =>
            {
                var sIn = stepsIn.GetNext();
                var sOut = stepsOut.GetNext();
                var ppoints = Interpolator.InOutCurve(0.30, 0.70, sIn, sOut, ef.easeIn, ef.easeOut);
                var vpoints = Interpolator.InOutCurve(0.50, 0.90, sIn, sOut, ef.easeIn, ef.easeOut);
                pcurve.AddRange(ppoints.Select(e => e * DeviceBase.ControlChangeMax));
                vcurve.AddRange(vpoints.Select(e => e * DeviceBase.ControlChangeMax));
            });

            var leftPan = pcurve.AsCycle();
            var leftVol = vcurve.AsCycle();

            var rightPan = pcurve.AsCycle(startIndex: pcurve.Count / 2);
            var rightVol = vcurve.AsCycle(startIndex: vcurve.Count / 2);

            var notes = file.GetNotes(outputDevice, clock);

            var noteE = notes.AsEnumeration();

            for (var i = 0; i < notes.Count - 1; i++)
            {
                var note = noteE.GetNext();
                var next = noteE.Peek;
                if (i % 2 == 0)
                {
                    note.Channel = leftChans.GetNext();
                    note.Pan = leftPan.GetNext();
                    note.Velocity = leftVol.GetNext();
                }
                else
                {
                    note.Channel = rightChans.GetNext();
                    note.Pan = rightPan.GetNext();
                    note.Velocity = rightVol.GetNext();
                }
            }
            clock.Schedule(notes);

            clock.Start();
            Thread.Sleep(400000);
            clock.Stop();
        }

        #endregion Methods
    }
}