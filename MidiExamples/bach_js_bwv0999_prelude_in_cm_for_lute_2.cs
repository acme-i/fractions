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

            var leftInstr = new Enumerate<Instrument>(Instruments.SoftGuitars, step: 1);
            var rightInstr = new Enumerate<Instrument>(Instruments.SoftBasses, step: 1);

            var file = new MidiFile(path);
            var div = (float)file.TicksPerQuarterNote;
            var chans = new Enumerate<Channel>(Channels.InstrumentChannels, step: 1);

            var lChans = Channels.Range(Channel.Channel1, Channel.Channel9);
            var rChans = Channels.Range(Channel.Channel11, Channel.Channel16);
            var leftChans = new Enumerate<Channel>(lChans, step: 1);
            var rightChans = new Enumerate<Channel>(rChans, step: 1);

            foreach (var x in lChans)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 0);
                outputDevice.SendControlChange(x, Control.Volume, DeviceBase.ControlChangeMax);
                outputDevice.SendProgramChange(x, leftInstr.GetNext());
            }

            foreach (var x in rChans)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 100);
                outputDevice.SendControlChange(x, Control.Volume, DeviceBase.ControlChangeMax);
                outputDevice.SendProgramChange(x, rightInstr.GetNext());
            }

            clock = new Clock(BPM);

            var stepsIn = new Enumerate<int>(new[] { 1, 2, 4, 8 }, IncrementMethod.Cyclic);
            var stepsOut = new Enumerate<int>(new[] { 1, 4, 8, 16 }.Reverse(), IncrementMethod.Cyclic);
            var maxEchos1 = new Enumerate<int>(new[] { 1, 2, 4, 1, 2, 4, 1, 2, 4, 1, 2, 4, 2, 8, 16, 2, 8, 16, 2, 8, 16, 4, 16, 32 }, IncrementMethod.Cyclic);
            var maxEchos2 = new Enumerate<int>(new[] { 1, 2, 4, 1, 2, 4, 1, 2, 4, 1, 2, 4, 2, 8, 16, 2, 8, 16, 2, 8, 16, 4, 16, 32 }.Reverse(), IncrementMethod.Cyclic);

            var pcurve = new List<double>();
            var vcurve = new List<double>();
            var efs = Interpolator.EaseFunctions();
            foreach (var ef in efs)
            {
                var sIn = stepsIn.GetNext();
                var sOut = stepsOut.GetNext();
                var ppoints = Interpolator.InOutCurve(0.30, 0.70, sIn, sOut, ef.easeIn, ef.easeOut);
                var vpoints = Interpolator.InOutCurve(0.50, 0.90, sIn, sOut, ef.easeIn, ef.easeOut);
                pcurve.AddRange(ppoints.Select(e => e * DeviceBase.ControlChangeMax));
                vcurve.AddRange(vpoints.Select(e => e * DeviceBase.ControlChangeMax));
            }

            var leftPan = new Enumerate<double>(pcurve, IncrementMethod.Cyclic);
            var leftVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic);

            var rightPan = new Enumerate<double>(pcurve, IncrementMethod.Cyclic, 1, pcurve.Count / 2);
            var rightVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic, 1, vcurve.Count / 2);

            var notes = file.GetNotes(outputDevice, clock);

            var noteE = new Enumerate<NoteOnOffMessage>(notes);

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