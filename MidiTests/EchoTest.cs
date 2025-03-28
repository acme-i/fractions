using System.Linq;
using NUnit.Framework;

namespace fractions.tests
{
    /// <summary>Unit tests for the Note class</summary>
    public class EchoTests
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

        #region Methods

        [Test]
        public void TestEchoes()
        {
            var note = new NoteOnOffMessage(device, Channel.Channel1, Pitch.A4, 80, 0, new Clock(120), 1, 64);
            var notes = LinearInterpolator<NoteOnOffMessage>.Interpolate(note, 0, 4, 4, 0, 0);
            Assert.AreEqual(4, notes.Count);
            Assert.AreEqual(notes.First().Velocity, note.Velocity);
            Assert.AreEqual(notes.First().Time, note.Time);
            Assert.AreEqual(notes.Last().Velocity, 0);
            Assert.AreEqual(notes.Last().Time, 4);
        }

        [Test]
        public void TestLinearInterpolation()
        {
            var note = new NoteOnOffMessage(device, Channel.Channel1, Pitch.A4, 80, 0, new Clock(120), 1, 64);
            var notes = LinearInterpolator<NoteOnOffMessage>.Interpolate(note, 0, 4, 4, 0, 0);
            Assert.AreEqual(4, notes.Count);
            Assert.AreEqual(notes.First().Velocity, note.Velocity);
            Assert.AreEqual(notes.First().Time, note.Time);
            Assert.AreEqual(notes.Last().Velocity, 0);
            Assert.AreEqual(notes.Last().Time, 4);
        }

        [Test]
        public void TestSineInterpolation()
        {
            var note = new NoteOnOffMessage(device, Channel.Channel1, Pitch.A4, 80, 0, new Clock(120), 1, 64);
            var notes = LinearInterpolator<NoteOnOffMessage>.Interpolate(note, 0, 4, 4, 0, 0);
            Assert.AreEqual(4, notes.Count);
            Assert.AreEqual(notes.First().Velocity, note.Velocity);
            Assert.AreEqual(notes.First().Time, note.Time);
            Assert.AreEqual(notes.Last().Velocity, 0);
            Assert.AreEqual(notes.Last().Time, 4);
        }

        #endregion
    }
}