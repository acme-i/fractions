using System.Linq;
using NUnit.Framework;

namespace fractions.tests
{
    public class EaseTests
    {
        IOutputDevice device = null;

        [SetUp]
        public void Setup()
        {
            device = OutputDevice.InstalledDevices.First();
            device.Open();
            device.SendProgramChange(Channel.Channel1, Instrument.ElectricBassFinger);
        }

        [TearDown]
        public void TearDown()
        {
            device.Close();
            device = null;
        }

        [Test]
        public void EaseTest()
        {
            var start = new NoteOnMessage(device, Channel.Channel1, Pitch.A4, 20, 1, 1, null, 0);
            var end = new NoteOnMessage(device, Channel.Channel1, Pitch.A4, 80, 1000, 120, null, 127);

            NoteProperty prop = NoteProperty.Time;
            prop |= NoteProperty.Velocity;
            prop |= NoteProperty.Pan;
            prop |= NoteProperty.Reverb;

            var seq = NoteInterpolator<NoteOnMessage>.NewEase(
                Interpolator.Ease(EaseType.EaseOutSine),
                start,
                end,
                18,
                prop
            ).ToList();

            Assert.AreEqual(20, seq.Count);

            var startResult = seq.First();
            var endResult = seq.Last();

            Assert.AreEqual(20, startResult.Velocity);
            Assert.AreEqual(80, endResult.Velocity);

            Assert.AreEqual(1, startResult.Pan);
            Assert.AreEqual(120, endResult.Pan);

            Assert.AreEqual(0, startResult.Reverb);
            Assert.AreEqual(127, endResult.Reverb);

        }

    }
}