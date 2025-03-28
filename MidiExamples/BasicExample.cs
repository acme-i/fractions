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
    internal class BasicExample : ExampleBase
    {
        public BasicExample() : base(nameof(BasicExample)) { }

        public override void Run()
        {
            OutputDevice = fractions.OutputDevice.InstalledDevices.FirstOrDefault();
            if (OutputDevice == null)
            {
                Console.WriteLine("No output devices, so can't run this example.");
                ExampleUtil.PressAnyKeyToContinue();
                return;
            }
            OutputDevice.Open();

            Play();

            Console.WriteLine();
            ExampleUtil.PressAnyKeyToContinue();

            OutputDevice.Close();
        }

        private IOutputDevice OutputDevice;
        private Clock Clock;
        private readonly Enumerate<Channel> chans = new Enumerate<Channel>(Channels.InstrumentChannels, IncrementMethod.MinMax, 1, 0);

        private void Play()
        {
            Clock = new Clock(80);
            var path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
            var file = new MidiFile(path);
            var durFile = new MidiFile(@".\midifiles\bach_js_bwv1056_harpsichod_concerto_in_fm_2_largo_(arioso).mid");
            float div = durFile.TicksPerQuarterNote + 0f;
            var result = file.GetEventsAndDurations();

            var max = result.OnEvents.Count();
            var nots = new Enumerate<MidiEvent>(result.OnEvents, IncrementMethod.MinMax, 1, 0);
            for (var i = 0; i < max; i++)
            {
                var note = nots.Next();
                var nt = new NoteOnOffMessage(OutputDevice, chans.Next(), (Pitch)note.Note, note.Velocity, note.Time / div, Clock, 4 * note.Value / div);
                Clock.Schedule(nt);
            }

            Clock.Start();
            Thread.Sleep(400000);
            Clock.Stop();
        }
    }
}