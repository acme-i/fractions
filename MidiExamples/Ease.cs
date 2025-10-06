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
    internal class Ease : ExampleBase
    {
        public Ease() : base(nameof(Ease)) { }

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

        private static readonly Enumerate<Channel> chans = new Enumerate<Channel>(
            new[] {
                Channel.Channel1,
                Channel.Channel2,
                Channel.Channel3,
                Channel.Channel4,
                Channel.Channel5,
                Channel.Channel6,
                Channel.Channel7,
                Channel.Channel8,
            }, IncrementMethod.MinMax, 1, 0);

        private static readonly Enumerate<Channel> echoChans = new Enumerate<Channel>(
    new[] {
                Channel.Channel9,
                Channel.Channel11,
                Channel.Channel12,
                Channel.Channel13,
                Channel.Channel14,
                Channel.Channel15,
                Channel.Channel16
    }, IncrementMethod.MinMax, 1, 0);

        private static IOutputDevice OutputDevice;
        private static Clock Clock;
        private static readonly int BPM = 68;
        private static void Init()
        {
            var be = new Enumerate<Instrument>(Instruments.SoftGuitars, step: 1);
            be.AddRange(Instruments.SoftBasses);

            foreach (var c in Channels.InstrumentChannels)
            {
                OutputDevice.SendProgramChange(c, be.GetNext());
            }

            Clock = new Clock(BPM);

            Play();
        }

        private static void Play()
        {
            var path = @".\midifiles\bach_js_bwv1033_flute_sonata_in_c_1_andante_presto.mid";
            var file = new MidiFile(path);
            var result = file.GetNotes(OutputDevice, Clock);


            //var echos = new Enumerate<int>( new int[] { 1, 2 }, 1);
            for (var i = 0; i < result.Count - 1; i++)
            {
                var r = result[i];
                //var rn = result[i + 1];

                r.Channel = chans.GetNext();
                Clock.Schedule(r);

                //var ef = Interpolator.RandomEase();
                //var d = (rn.Time - r.Time);
                //var es = echos.GetNext();

                //var notes = NoteInterpolator<NoteOnOffMessage>.NewEase(ef, r, rn, es, NoteProperty.Time | NoteProperty.Duration);

                //var remain = notes.Skip(1).Take(es);

                //var count = 0;
                //foreach (var echo in remain)
                //{
                //    var v = ef(r.Velocity, rn.Velocity * 0.25f, count++, remain.Count());
                //    echo.Velocity = v;
                //    echo.Channel = echoChannels.GetNext();
                //    Clock.Schedule(echo);
                //}


            }

            Clock.Start();
            Thread.Sleep(1000 * BPM * 60);
            Clock.Stop();
        }
    }
}