using System.Collections.Generic;
using NUnit.Framework;

namespace fractions.tests
{
    /// <summary>Unit tests for the Channel enum</summary>
    public class EnumerateTests
    {
        #region Methods

        [Test]
        public void OctaveAboveTest()
        {
            var list = new Enumerate<Pitch>(new[] { Pitch.A0, Pitch.A1 }, step: 1);
            var octaveAbove = list.OctaveAbove().ToList();
            Assert.AreEqual(Pitch.A1, octaveAbove[0]);
            Assert.AreEqual(Pitch.A2, octaveAbove[1]);
        }

        [Test]
        public void OctaveBelowTest()
        {
            var list = new Enumerate<Pitch>(new[] { Pitch.A1, Pitch.A2 }, step: 1);
            var octaveAbove = list.OctaveBelow().ToList();
            Assert.AreEqual(Pitch.A0, octaveAbove[0]);
            Assert.AreEqual(Pitch.A1, octaveAbove[1]);
        }

        #endregion Methods

        [Test]
        public void StartsAtZero()
        {

        var maxLeft = 10;
        var maxRight = 117;

        var pSteps = Interpolator.Interpolate(maxLeft, maxRight, 4 * 12, 0);
            var pSteps2 = Interpolator.Interpolate(maxLeft, maxRight, 4 * 6, 0);

            var pSteppers = new Enumerate<List<float>>
            (
                new[] { pSteps, pSteps2 },
                IncrementMethod.MinMax
            );

            Assert.AreEqual(0, pSteppers.Incrementor.Value);
        }
    }
}