using System.Collections.Generic;
using NUnit.Framework;

namespace fractions.tests
{
    /// <summary>Unit tests for the Channel enum</summary>
    public class EnumerateTests
    {
        #region Methods

        [Test]
        public void GetNextIsInSyncWithCurrentTest()
        {
            var list = new[] { Pitch.A0, Pitch.A1, Pitch.A2 }.AsEnumeration();
            Assert.AreEqual(null, list.PreviousIndex);
            Assert.AreEqual(Pitch.A0, list.Current);
            Assert.AreEqual(Pitch.A0, list.GetNext());
            Assert.AreEqual(Pitch.A1, list.Current);
            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(Pitch.A2, list.Current);
            Assert.AreEqual(Pitch.A2, list.GetNext());
        }

        [Test]
        public void OctaveAboveTest()
        {
            var list = new[] { Pitch.A0, Pitch.A1 }.AsEnumeration();
            var octaveAbove = list.OctaveAbove().ToList();
            Assert.AreEqual(Pitch.A1, octaveAbove[0]);
            Assert.AreEqual(Pitch.A2, octaveAbove[1]);
        }

        [Test]
        public void OctaveBelowTest()
        {
            var list = new[] { Pitch.A1, Pitch.A2 }.AsEnumeration();
            var octaveAbove = list.OctaveBelow().ToList();
            Assert.AreEqual(Pitch.A0, octaveAbove[0]);
            Assert.AreEqual(Pitch.A1, octaveAbove[1]);
        }

        [Test]
        public void AsCycleTest()
        {
            var list = new[] { Pitch.A1, Pitch.A2 }.AsCycle();

            Assert.AreEqual(IncrementMethod.Cyclic, list.Incrementor.Method);
            Assert.AreEqual(0, list.Incrementor.Index);
            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(Pitch.A2, list.GetNext());
            Assert.AreEqual(Pitch.A1, list.GetNext());
        }

        [Test]
        public void AsCycleAsReversedTest()
        {
            var list = new[] { Pitch.A1, Pitch.A2, Pitch.A3 }.AsCycle().AsReversed();
            Assert.AreEqual(IncrementMethod.Cyclic, list.Incrementor.Method);
            Assert.AreEqual(0, list.Incrementor.Index);
            Assert.AreEqual(Pitch.A3, list.GetNext());
            Assert.AreEqual(Pitch.A2, list.GetNext());
            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(Pitch.A2, list.GetNext());
        }

        [Test]
        public void AsMaxMinEnumerationTest()
        {
            var list = new[] { Pitch.A1, Pitch.A2, Pitch.A3 }.AsMaxMinEnumeration();
            Assert.AreEqual(Pitch.A3, list.GetNext());
            Assert.AreEqual(Pitch.A2, list.GetNext());
            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(Pitch.A3, list.GetNext());
            Assert.AreEqual(Pitch.A2, list.GetNext());
        }


        [Test]
        public void AsMaxMinEnumerationAsReversedTest()
        {
            var list = new[] { Pitch.A1, Pitch.A2, Pitch.A3 }.AsMaxMinEnumeration().AsReversed();
            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(Pitch.A2, list.GetNext());
            Assert.AreEqual(Pitch.A3, list.GetNext());
            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(Pitch.A2, list.GetNext());
        }


        [Test]
        public void AsEnumerationTest()
        {
            var list = new[] { Pitch.A1, Pitch.A2 }.AsEnumeration();
            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(Pitch.A2, list.GetNext());
            Assert.AreEqual(Pitch.A1, list.GetNext());
        }

        [Test]
        public void AsEnumerationAsAsReversedTest()
        {
            var list = new[] { Pitch.A1, Pitch.A2 }.AsEnumeration().AsReversed();
            Assert.AreEqual(Pitch.A2, list.GetNext());
            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(Pitch.A2, list.GetNext());
        }

        [Test]
        public void IncreasingAsCycleTest()
        {
            var list = new[] { Pitch.A1, Pitch.A2, Pitch.A3 }.AsCycle();
            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(true, list.Increasing);

            Assert.AreEqual(Pitch.A2, list.GetNext());
            Assert.AreEqual(true, list.Increasing);

            Assert.AreEqual(Pitch.A3, list.GetNext());
            Assert.AreEqual(false, list.Increasing);

            Assert.AreEqual(Pitch.A2, list.GetNext());
            Assert.AreEqual(false, list.Increasing);

            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(true, list.Increasing);
        }

        [Test]
        public void IncreasingAsEnumerationTest()
        {
            var list = new[] { Pitch.A1, Pitch.A2, Pitch.A3 }.AsEnumeration();
            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(true, list.Increasing);

            Assert.AreEqual(Pitch.A2, list.GetNext());
            Assert.AreEqual(true, list.Increasing);

            Assert.AreEqual(Pitch.A3, list.GetNext());
            Assert.AreEqual(true, list.Increasing);

            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(true, list.Increasing);
        }

        [Test]
        public void IncreasingAsMaxMinEnumerationTest()
        {
            var list = new[] { Pitch.A1, Pitch.A2, Pitch.A3 }.AsMaxMinEnumeration();
            Assert.AreEqual(Pitch.A3, list.GetNext());
            Assert.AreEqual(false, list.Increasing);

            Assert.AreEqual(Pitch.A2, list.GetNext());
            Assert.AreEqual(false, list.Increasing);

            Assert.AreEqual(Pitch.A1, list.GetNext());
            Assert.AreEqual(false, list.Increasing);

            Assert.AreEqual(Pitch.A3, list.GetNext());
            Assert.AreEqual(false, list.Increasing);
        }

        #endregion Methods

        [Test]
        public void StartsAtZero()
        {
            var maxLeft = 10;
            var maxRight = 117;
            var pSteps = Interpolator.Interpolate(maxLeft, maxRight, 4 * 12, 0);
            var pSteps2 = Interpolator.Interpolate(maxLeft, maxRight, 4 * 6, 0);
            var pSteppers = new[] { pSteps, pSteps2 }.AsEnumeration();
            Assert.AreEqual(0, pSteppers.Index);
            Assert.AreEqual(2, pSteppers.Count);
            Assert.AreEqual(IncrementMethod.MinMax, pSteppers.Method);

            Assert.AreEqual(pSteps, pSteppers.GetNext());
            Assert.AreEqual(pSteps2, pSteppers.GetNext());
        }

        [Test]
        public void CloneTest()
        {
            var maxLeft = 10;
            var maxRight = 117;
            var pSteps = Interpolator.Interpolate(maxLeft, maxRight, 4 * 12, 0);
            var enumSteps = pSteps.AsEnumeration();

            enumSteps.GetNext();
            enumSteps.GetNext();

            var clone = enumSteps.Clone();

            Assert.AreEqual(enumSteps.Index, clone.Index);
            Assert.AreEqual(null, clone.PreviousIndex);
            Assert.AreEqual(enumSteps.Method, clone.Method);
            Assert.AreEqual(enumSteps.Count, clone.Count);
            Assert.AreEqual(enumSteps.ToList(), clone.ToList());
            Assert.AreNotEqual(enumSteps.Name, clone.Name);
        }

        [Test]
        public void CloneWithPreviousIndexTest()
        {
            var maxLeft = 10;
            var maxRight = 117;
            var pSteps = Interpolator.Interpolate(maxLeft, maxRight, 4 * 12, 0);
            var enumSteps = pSteps.AsEnumeration();

            enumSteps.GetNext();
            enumSteps.GetNext();

            var clone = enumSteps.CloneWithPreviousIndex();

            Assert.AreEqual(enumSteps.Index, clone.Index);
            Assert.AreEqual(enumSteps.PreviousIndex, clone.PreviousIndex);
            Assert.AreEqual(enumSteps.Method, clone.Method);
            Assert.AreEqual(enumSteps.Count, clone.Count);
            Assert.AreEqual(enumSteps.ToList(), clone.ToList());
            Assert.AreNotEqual(enumSteps.Name, clone.Name);
        }
    }
}