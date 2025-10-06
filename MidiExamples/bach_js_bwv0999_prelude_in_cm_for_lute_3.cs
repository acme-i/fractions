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
    internal class bach_js_bwv0999_prelude_in_cm_for_lute_3 : ExampleBase
    {
        string path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
        MidiFile file;
        List<NoteOnOffMessage> notes;
        Enumerate<NoteOnOffMessage> noteE;

        #region Constructors

        public bach_js_bwv0999_prelude_in_cm_for_lute_3() : base(nameof(bach_js_bwv0999_prelude_in_cm_for_lute_3)) { }

        #endregion Constructors

        #region Methods

        private IOutputDevice outputDevice;
        private Clock clock;
        private readonly int BPM = 35;

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

            foreach (var x in Channels.InstrumentChannels)
            {
                outputDevice.SendControlChange(x, Control.ReverbLevel, 0);
                outputDevice.SendControlChange(x, Control.Volume, DeviceBase.ControlChangeMax);
            }

            clock = new Clock(BPM);
            file = new MidiFile(path);
            notes = file.GetNotes(outputDevice, clock);
            noteE = new Enumerate<NoteOnOffMessage>(notes);

            Enumerate<int> stepsIn = new Enumerate<int>(new[] { 1, 2, 4, 8 }, IncrementMethod.Cyclic);
            Enumerate<int> stepsOut = new Enumerate<int>(new[] { 1, 4, 8, 16 }.Reverse(), IncrementMethod.Cyclic);
            Enumerate<int> maxEchos1 = new Enumerate<int>(new[] { 1, 2, 4, 1, 2, 4, 1, 2, 4, 1, 2, 4, 2, 8, 16, 2, 8, 16, 2, 8, 16, 4, 16, 32 }, IncrementMethod.Cyclic);
            Enumerate<int> maxEchos2 = new Enumerate<int>(new[] { 1, 2, 4, 1, 2, 4, 1, 2, 4, 1, 2, 4, 2, 8, 16, 2, 8, 16, 2, 8, 16, 4, 16, 32 }.Reverse(), IncrementMethod.Cyclic);

            List<double> pcurve = new List<double>();
            List<double> vcurve = new List<double>();
            List<(EaseFunction easeIn, EaseFunction easeOut)> efs = Interpolator.EaseFunctions();
            foreach (var ef in efs)
            {
                var sIn = stepsIn.GetNext();
                var sOut = stepsOut.GetNext();
                var ppoints = Interpolator.InOutCurve(0.30, 0.70, sIn, sOut, ef.easeIn, ef.easeOut);
                var vpoints = Interpolator.InOutCurve(0.50, 0.90, sIn, sOut, ef.easeIn, ef.easeOut);
                pcurve.AddRange(ppoints.Select(e => e * DeviceBase.ControlChangeMax));
                vcurve.AddRange(vpoints.Select(e => e * DeviceBase.ControlChangeMax));
            }

            Enumerate<double> leftPan = new Enumerate<double>(pcurve, IncrementMethod.Cyclic);
            Enumerate<double> leftVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic);

            Enumerate<double> rightPan = new Enumerate<double>(pcurve, IncrementMethod.Cyclic, 1, pcurve.Count / 2);
            Enumerate<double> rightVol = new Enumerate<double>(vcurve, IncrementMethod.Cyclic, 1, vcurve.Count / 2);

            var lInstr = new Enumerate<Instrument>(new[] {
                Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.Vibraphone, Instrument.ElectricPiano1,
                Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.ElectricPiano1, Instrument.Vibraphone,
                Instrument.SlapBass1, Instrument.Vibraphone, Instrument.ElectricPiano1, Instrument.ElectricPiano1,
            }, step: 1);
            var rInstr = new Enumerate<Instrument>(new[] {
                Instrument.StringEnsemble1, Instrument.ElectricBassPick, Instrument.ElectricPiano1, Instrument.ElectricBassPick,
                Instrument.StringEnsemble1, Instrument.ElectricBassPick, Instrument.ElectricBassPick, Instrument.ElectricPiano1,
                Instrument.StringEnsemble1, Instrument.ElectricPiano1, Instrument.ElectricBassPick, Instrument.ElectricBassPick,
            }, step: 1);

            var steps = 8;
            var startTimes = Interpolator.Interpolate(0, 1, steps);
            var multipliers = startTimes.Reverse<float>().ToList();

            for (int i = 0; i < steps; i++)
            {
                if (i % 2 == 0)
                {
                    Goahead(
                        Channels.Range(Channel.Channel1, Channel.Channel4),
                        Channels.Range(Channel.Channel5, Channel.Channel9),
                        lInstr.Clone(), rInstr.Clone(),
                        leftPan.Clone(), leftVol.Clone(),
                        rightPan.Clone(), rightVol.Clone(),
                        startTimes[i], multipliers[i]
                    );
                }
                else
                {
                    Goahead(
                        Channels.Range(Channel.Channel11, Channel.Channel13),
                        Channels.Range(Channel.Channel14, Channel.Channel16),
                        rInstr.Clone(), lInstr.Clone(),
                        rightPan.Clone(), rightVol.Clone(),
                        leftPan.Clone(), leftVol.Clone(),
                        startTimes[i], multipliers[i]
                    );
                }

                if (i % 4 == 0)
                {
                    lInstr.GetNext();
                    rInstr.GetNext();
                }

                leftPan.GetNext();
                rightPan.GetNext();

                leftVol.GetNext();
                rightVol.GetNext();
            }

            clock.Start();
            Thread.Sleep(400000);
            clock.Stop();
            
            // Close the output device.
            outputDevice.Close();

            // All done.
            Console.WriteLine();
            ExampleUtil.PressAnyKeyToContinue();
        }

        private void Goahead( 
            IEnumerable<Channel> lChans, IEnumerable<Channel> rChans,
            Enumerate<Instrument> lInstr, Enumerate<Instrument> rInstr,
            Enumerate<double> leftPan, Enumerate<double> leftVol,
            Enumerate<double> rightPan, Enumerate<double> rightVol,
            float offSet, float volMult
            )
        {
            var leftChans = new Enumerate<Channel>(lChans, step: 1);
            var rightChans = new Enumerate<Channel>(rChans, step: 1);

            var nec = noteE.Clone();
            for (var i = 0; i < notes.Count - 1; i++)
            {
                var note = nec.GetNext().MakeTimeShiftedCopy(offSet) as NoteOnOffMessage;
                if (i % 2 == 0)
                {
                    note.Channel = leftChans.GetNext();
                    note.Pan = leftPan.GetNext();
                    note.Velocity = leftVol.GetNext() * volMult;
                    note.BeforeSendingNoteOnOff += (e) =>
                    {
                        outputDevice.SendProgramChange(e.Channel, lInstr.GetNext());
                    };
                }
                else
                {
                    note.Channel = rightChans.GetNext();
                    note.Pan = rightPan.GetNext();
                    note.Velocity = rightVol.GetNext() * volMult;
                    note.BeforeSendingNoteOnOff += (e) =>
                    {
                        outputDevice.SendProgramChange(e.Channel, rInstr.GetNext());
                    };
                }

                

                clock.Schedule(note);
            }

        }

        #endregion Methods
    }
}