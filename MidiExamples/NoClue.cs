using System;
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
    internal class NoClue : ExampleBase
    {
        #region Constructors

        public NoClue() : base("NoClue")
        { }

        #endregion Constructors

        #region Methods

        public override void Run()
        {
            // Prompt user to choose an output device (or if there is only one, use that one).
            var outputDevice = ExampleUtil.ChooseOutputDeviceFromConsole();
            if (outputDevice == null)
            {
                Console.WriteLine("No output devices, so can't run this example.");
                ExampleUtil.PressAnyKeyToContinue();
                return;
            }
            outputDevice.Open();

            MaryHadALittleLamb(outputDevice);
            //SustainPedalExample(outputDevice);
            //SustainedChordRun(outputDevice);

            // Close the output device.
            outputDevice.Close();

            // All done.
            Console.WriteLine();
            ExampleUtil.PressAnyKeyToContinue();
        }

        private void SustainedChordRun(IOutputDevice outputDevice)
        {
            Console.WriteLine("Playing sustained chord runs up the keyboard...");
            outputDevice.SendControlChange(Channel.Channel1, Control.Pan, 127);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
            outputDevice.SendControlChange(Channel.Channel1, Control.ReverbLevel, 127);
            PlayChordRun(outputDevice, new Chord("C"), 100);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
            outputDevice.SendControlChange(Channel.Channel1, Control.Pan, 100);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
            PlayChordRun(outputDevice, new Chord("F"), 100);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
            outputDevice.SendControlChange(Channel.Channel1, Control.Pan, 75);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
            PlayChordRun(outputDevice, new Chord("G"), 100);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
            outputDevice.SendControlChange(Channel.Channel1, Control.Pan, 50);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
            PlayChordRun(outputDevice, new Chord("C"), 100);
            Thread.Sleep(2000);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
        }

        private static void SustainPedalExample(IOutputDevice outputDevice)
        {
            Console.WriteLine("Playing an arpeggiated C chord and then bending it down.");

            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
            outputDevice.SendPitchBend(Channel.Channel1, 8192);
            // Play C, E, G in half second intervals.
            outputDevice.SendNoteOn(Channel.Channel1, Pitch.C4, 80);
            Thread.Sleep(500);
            outputDevice.SendNoteOn(Channel.Channel1, Pitch.E4, 80);
            Thread.Sleep(500);
            outputDevice.SendNoteOn(Channel.Channel1, Pitch.G4, 80);
            Thread.Sleep(500);

            // Now apply the sustain pedal.
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);

            // Now release the C chord notes, but they should keep ringing because of the sustain
            // pedal.
            outputDevice.SendNoteOff(Channel.Channel1, Pitch.C4, 80);
            outputDevice.SendNoteOff(Channel.Channel1, Pitch.E4, 80);
            outputDevice.SendNoteOff(Channel.Channel1, Pitch.G4, 80);

            // Now bend the pitches down.
            for (int i = 0; i < 17; ++i)
            {
                outputDevice.SendPitchBend(Channel.Channel1, 8192 - i * 450);
                Thread.Sleep(200);
            }

            // Now release the sustain pedal, which should silence the notes, then center
            // the pitch bend again.
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
            outputDevice.SendPitchBend(Channel.Channel1, 8192);
        }

        private static void melodi_1(IOutputDevice outputDevice)
        {
            outputDevice.SendProgramChange(Channel.Channel1, Instrument.ElectricPiano1);

            var melody = new[] { Pitch.E4, Pitch.D4, Pitch.C4, Pitch.D4, Pitch.E4, Pitch.E4, Pitch.E4 };

            var scale = Scale.Major;



        }

        private static void MaryHadALittleLamb(IOutputDevice outputDevice)
        {
            Console.WriteLine("Playing the first two bars of Mary Had a Little Lamb...");

            outputDevice.SendProgramChange(Channel.Channel1, Instrument.SlapBass1);
            outputDevice.SendProgramChange(Channel.Channel2, Instrument.ElectricGuitarMuted);
            outputDevice.SendProgramChange(Channel.Channel3, Instrument.Xylophone);
            outputDevice.SendProgramChange(Channel.Channel4, Instrument.SlapBass1);
            outputDevice.SendProgramChange(Channel.Channel5, Instrument.Woodblock);
            outputDevice.SendProgramChange(Channel.Channel6, Instrument.Glockenspiel);
            outputDevice.SendProgramChange(Channel.Channel7, Instrument.Pad5Bowed);

            var clock = new Clock(16);
            var riff = new Pitch?[] { Pitch.A4, Pitch.B2, Pitch.E2, Pitch.A2, null, Pitch.B2, Pitch.E2 };
            var riff2 = new Pitch?[] { Pitch.A3, Pitch.B3, Pitch.E2, Pitch.A2, null, Pitch.B2, Pitch.E2 };
            var riff3 = new Pitch?[] { Pitch.A3, Pitch.B3, Pitch.E3, Pitch.A3, Pitch.A3, Pitch.B3, Pitch.E3 };

            var maxPitch = (double)riff.Max();
            var maxPitch2 = (double)riff2.Max();
            var rand = new Random(DateTime.Now.Second * DateTime.Now.Millisecond);

            void noteOnHandler(NoteOnMessage m)
            {
                var pan = Interpolator.Interpolate(Math.Cos(m.Time / 100 * Math.PI * 2 * 64), -1, 1, 20, 107, 0);
                //Console.WriteLine(pan);
                outputDevice.SendControlChange(m.Channel, Control.Pan, (int)pan);

                var note = new NoteOnMessage(outputDevice, Channel.Channel5, m.Pitch, 20, m.Time);
                note.SendNow();
            }

            void noteOnHandler2(NoteOnMessage m)
            {
                var pan = Interpolator.Interpolate(m.Time, 0, 100, 0, 127, 0);
                var vol = Interpolator.Interpolate(m.Time, 0, 100, 2, 127, 0);
                //Console.WriteLine(pan);
                outputDevice.SendControlChange(m.Channel, Control.PhaserLevel, (int)pan);
                outputDevice.SendControlChange(m.Channel, Control.ReverbLevel, (int)vol);
                outputDevice.SendControlChange(m.Channel, Control.ChorusLevel, (int)vol);
            }

            void noteOnHandler3(NoteOnMessage m)
            {
                var pan = Interpolator.Interpolate(m.Time, 0, 100, 127, 0, 2);
                var vol = Interpolator.Interpolate(m.Time, 0, 100, 127, 2, 3);
                //Console.WriteLine(pan);
                outputDevice.SendControlChange(m.Channel, Control.ChorusLevel, (int)pan);
                outputDevice.SendControlChange(m.Channel, Control.ReverbLevel, (int)vol);
                outputDevice.SendControlChange(m.Channel, Control.ChorusLevel, (int)vol);
            }

            for (int time = 0; time < 100; time++)
            {
                if (riff[time % riff.Length] is Pitch p1)
                {
                    var startVol = rand.NextVelocity(20, 60);
                    var endVol = rand.NextVelocity(2, 10);
                    var note = new NoteOnMessage(outputDevice, Channel.Channel2, p1, startVol, time);
                    var steps = (int)Math.Round(8 * (double)p1 / maxPitch);
                    var notes = LinearInterpolator<NoteOnMessage>.Interpolate(start: note, endVelocity: endVol, steps: steps, duration: 1f, velocityMethod: rand.Next(0, 3), timeMethod: rand.Next(0, 3));
                    notes.ForEach(n =>
                    {
                        n.BeforeSending += noteOnHandler;
                        clock.Schedule(n);
                    });

                    if (riff3[time % riff.Length] is Pitch p1b)
                    {
                        var note2 = new NoteOnMessage(outputDevice, Channel.Channel3, p1b, rand.NextVelocity(20, 60), time);
                        var notes2 = LinearInterpolator<NoteOnMessage>.Interpolate(start: note, endVelocity: 64 - endVol, steps: Math.Abs(32 - steps), duration: 1f, velocityMethod: rand.Next(0, 3), timeMethod: rand.Next(0, 3));
                        notes2.ForEach(n =>
                        {
                            n.AfterSending += noteOnHandler3;
                            clock.Schedule(n);
                        });
                    }
                }

                if (riff2[time % riff.Length] is Pitch p2)
                {
                    var startVol = rand.NextVelocity(20, 40);
                    var endVol = rand.NextVelocity(2, 5);
                    var note = new NoteOnMessage(outputDevice, Channel.Channel4, p2, startVol, time);
                    var notes = LinearInterpolator<NoteOnMessage>.Interpolate(note, endVol, (int)Math.Round(4 * (double)p2 / maxPitch2), duration: 1, velocityMethod: 2, timeMethod: 2);
                    notes.ForEach(n =>
                    {
                        n.BeforeSending += noteOnHandler;
                        clock.Schedule(n);
                    });
                }
                if (riff2[time % riff.Length] is Pitch p3)
                {
                    var vol = rand.NextVelocity(3, 10);
                    var note = new NoteOnMessage(outputDevice, Channel.Channel4, p3, vol, time);
                    var notes = LinearInterpolator<NoteOnMessage>.Interpolate(note, vol, (int)Math.Round(128 * (double)p3 / maxPitch2), duration: 1, velocityMethod: 2, timeMethod: 2);
                    notes.ForEach(n =>
                    {
                        n.BeforeSending += noteOnHandler2;
                        clock.Schedule(n);
                    });
                }
                if (riff2[time % riff.Length] is Pitch p4)
                {
                    var vol = rand.NextVelocity(2, 5);
                    var note = new NoteOnMessage(outputDevice, Channel.Channel6, p4, vol, time);
                    var notes = LinearInterpolator<NoteOnMessage>.Interpolate(note, vol, (int)Math.Round(64 * (double)p4 / maxPitch2), duration: 1, velocityMethod: 0, timeMethod: 0);
                    notes.ForEach(n =>
                    {
                        n.BeforeSending += noteOnHandler2;
                        clock.Schedule(n);
                    });
                    /*
                    notes.ForEach(n =>
                    {
                        n.BeforeSending += noteOnHandler3;
                        clock.Schedule(n);
                    });*/
                }
                if (riff3[time % riff.Length] is Pitch p5)
                {
                    var vol = rand.NextVelocity(10, 20);
                    var note = new NoteOnOffMessage(outputDevice, Channel.Channel7, p5, vol, time, clock, 1F);
                    var notes = LinearInterpolator<NoteOnOffMessage>.Interpolate(note, vol, (int)Math.Round(64 * (double)p5 / maxPitch2), 1F, 0, 0);
                    notes.ForEach(n =>
                    {
                        clock.Schedule(n);
                    });
                }
            }

            clock.Start();
            Thread.Sleep(100000);
            clock.Stop();
        }

        private void PlayChordRun(IOutputDevice outputDevice, Chord chord, int millisecondsBetween)
        {
            var previousNote = (Pitch)(-1);
            for (Pitch pitch = Pitch.A0; pitch < Pitch.C8; ++pitch)
            {
                if (chord.Contains(pitch))
                {
                    if (previousNote != (Pitch)(-1))
                    {
                        outputDevice.SendNoteOff(Channel.Channel1, previousNote, 80);
                    }
                    outputDevice.SendNoteOn(Channel.Channel1, pitch, 80);
                    Thread.Sleep(millisecondsBetween);
                    previousNote = pitch;
                }
            }
            if (previousNote != (Pitch)(-1))
            {
                outputDevice.SendNoteOff(Channel.Channel1, previousNote, 80);
            }
        }

        #endregion Methods
    }
}