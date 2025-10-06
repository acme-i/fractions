namespace fractions.tests
{
    using System;
    using System.Collections.Generic;
    using fractions;
    using NUnit.Framework;

    [TestFixture]
    public static class InstrumentsTests
    {
        [Test]
        public static void CanGetAll()
        {
            // Assert
            Assert.That(Instruments.All, Is.InstanceOf<List<Instrument>>());

            var instr = Instruments.All;
            Assert.That(instr, Is.InstanceOf<List<Instrument>>());
            Assert.AreEqual(instr.Count, Instruments.All.Count);
            for (var i = 0; i < instr.Count; i++)
                Assert.AreEqual(instr[i], Instruments.All[i]);
            instr.Clear();
            Assert.AreNotEqual(instr.Count, Instruments.All.Count);
            Assert.That(Instruments.All, Is.InstanceOf<List<Instrument>>());
        }

        [Test]
        public static void CanGetPianos()
        {
            // Assert
            Assert.That(Instruments.Pianos, Is.InstanceOf<List<Instrument>>());
        }

        [Test]
        public static void CanGetGuitars()
        {
            // Assert
            Assert.That(Instruments.Guitars, Is.InstanceOf<List<Instrument>>());
        }

        [Test]
        public static void CanGetSoftGuitars()
        {
            // Assert
            Assert.That(Instruments.SoftGuitars, Is.InstanceOf<List<Instrument>>());
        }
    }
}