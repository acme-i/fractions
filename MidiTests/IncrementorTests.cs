using System;
using NUnit.Framework;

namespace fractions.tests
{
    /// <summary>Unit tests for the Note class</summary>
    [TestFixture]
    public class IncrementorTests
    {
        #region Constructors

        [Test]
        public void Ctor_MinMaxIsNegativeOrZero_ArgumentIsThrown()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Incrementor(0, -1, 100, 1, IncrementMethod.MinMax, false);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                new Incrementor(0, 0, -1, 1, IncrementMethod.MinMax, false);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                new Incrementor(0, -1, -2, 1, IncrementMethod.MinMax, false);
            });
        }

        [Test]
        public void Ctor_MinMaxIsNegativeOrZeroWithAutoCorrect_ArgumentIsNotThrown()
        {
            Assert.DoesNotThrow(() =>
            {
                new Incrementor(0, -1, 100, 1, IncrementMethod.MinMax, true);
            });
            Assert.DoesNotThrow(() =>
            {
                new Incrementor(0, 0, -1, 1, IncrementMethod.MinMax, true);
            });
            Assert.DoesNotThrow(() =>
            {
                new Incrementor(0, -1, -2, 1, IncrementMethod.MinMax, true);
            });
        }

        [Test]
        public void Ctor_ValueIsAlwaysClampedBetweenMinAndMaxWithAutoCorrect()
        {
            Assert.DoesNotThrow(() =>
            {
                new Incrementor(120, 0, 100, 1, IncrementMethod.MinMax, true);
            });

            var inc = new Incrementor(120, 0, 100, 1, IncrementMethod.MinMax, true); ;
            Assert.AreEqual(100, inc.Index);

            inc = new Incrementor(10, 20, 100, 1, IncrementMethod.MinMax, true); ;
            Assert.AreEqual(20, inc.Index);
        }

        [Test]
        public void Ctor_ValueIsNegative_ArgumentExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Incrementor(-1, 0, 100, 1, IncrementMethod.MinMax, false);
            });
        }

        [Test]
        public void Ctor_StepSizeCannotBeOutsideMinMaxRange_ArgumentExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Incrementor(0, 0, 100, 101, IncrementMethod.MinMax, false);
            });
        }

        [Test]
        public void Ctor_StepSizeEqualToOrSmallerThanZero_ArgumentExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Incrementor(0, 0, 100, 0, IncrementMethod.MinMax, false);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                new Incrementor(0, 0, 100, -1, IncrementMethod.MinMax, false);
            });
        }

        [Test]
        public void Ctor_Incrementor()
        {
            var original = new Incrementor(0, 100, 20, IncrementMethod.Cyclic);
            Assert.AreEqual(0, original.Index);
            Assert.AreEqual(true, original.Increasing);

            original.GetNext();
            original.GetNext();
            Assert.AreEqual(40, original.Index);

            var inc = new Incrementor(original);
            Assert.AreEqual(original.Index, inc.Index);
            Assert.AreEqual(40, inc.Index);

            Assert.AreEqual(original.StepSize, inc.StepSize);
            Assert.AreEqual(original.Min, inc.Min);
            Assert.AreEqual(original.Max, inc.Max);
            Assert.AreEqual(original.Method, inc.Method);
            Assert.AreEqual(original.Increasing, inc.Increasing);
        }

        [Test]
        public void Ctors_Cyclic()
        {
            var inc = new Incrementor(0, 100, 20, IncrementMethod.Cyclic);
            Assert.AreEqual(IncrementMethod.Cyclic, inc.Method);
            Assert.AreEqual(true, inc.Increasing);
            Assert.AreEqual(0, inc.Index);
            Assert.AreEqual(20, inc.GetNext());
            Assert.AreEqual(40, inc.GetNext());
            Assert.AreEqual(60, inc.GetNext());
            Assert.AreEqual(80, inc.GetNext());
            Assert.AreEqual(100, inc.GetNext());
            Assert.AreEqual(80, inc.GetNext());
            Assert.AreEqual(60, inc.GetNext());
            Assert.AreEqual(40, inc.GetNext());
            Assert.AreEqual(20, inc.GetNext());
            Assert.AreEqual(0, inc.GetNext());
            Assert.AreEqual(20, inc.GetNext());
        }

        [TestCase(0, 9, 1, IncrementMethod.Cyclic)]
        [TestCase(3, 5, 1, IncrementMethod.MinMax)]
        [TestCase(2, 23, 7, IncrementMethod.Bit)]
        public void IncrementMethod_Ctor(int min, int max, int step, IncrementMethod met)
        {
            var inc = new Incrementor(min, max, step, met);
            Assert.AreEqual(min, inc.Index);
            Assert.AreEqual(null, inc.PreviousIndex);
            Assert.AreEqual(step, inc.StepSize);
            Assert.AreEqual(min, inc.Min);
            Assert.AreEqual(max, inc.Max);
            Assert.AreEqual(met, inc.Method);
            Assert.AreEqual(true, inc.Increasing);
        }

        [TestCase(0, 9, 1, IncrementMethod.MaxMin)]
        [TestCase(3, 5, 1, IncrementMethod.MaxMin)]
        [TestCase(2, 23, 7, IncrementMethod.MaxMin)]
        public void IncrementMethod_Ctor_MaxMin(int min, int max, int step, IncrementMethod met)
        {
            var inc = new Incrementor(min, max, step, met);
            Assert.AreEqual(max, inc.Index);
            Assert.AreEqual(null, inc.PreviousIndex);
            Assert.AreEqual(step, inc.StepSize);
            Assert.AreEqual(min, inc.Min);
            Assert.AreEqual(max, inc.Max);
            Assert.AreEqual(met, inc.Method);
            Assert.AreEqual(false, inc.Increasing);
        }

        [TestCase(-10, 10, 12)]
        [TestCase(0, 0, 10)]
        [TestCase(2, 5, 10)]
        public void Ctor_Value_Is_Clamped_To_Min(int value, int min, int max)
        {
            var inc = new Incrementor(value, min, max, 1, IncrementMethod.MinMax);
            Assert.True(inc.Index == min);
        }

        [TestCase(-1, 0, 9)]
        [TestCase(600, 3, 5)]
        [TestCase(10, 2, 23)]
        [TestCase(2, 2, 23)]
        [TestCase(23, 2, 23)]
        public void Ctor_Value_Is_Clamped_To_Max(int value, int min, int max)
        {
            var inc = new Incrementor(value, min, max, 1, IncrementMethod.MaxMin);
            Assert.True( inc.Index == max );
        }

        [Test]
        public void Ctors_MaxMin()
        {
            var inc = new Incrementor(0, 100, 20, IncrementMethod.MaxMin);
            Assert.AreEqual(IncrementMethod.MaxMin, inc.Method);
            Assert.AreEqual(false, inc.Increasing);
            Assert.AreEqual(100, inc.Index);
            Assert.AreEqual(80, inc.GetNext());
            Assert.AreEqual(60, inc.GetNext());
            Assert.AreEqual(40, inc.GetNext());
            Assert.AreEqual(20, inc.GetNext());
            Assert.AreEqual(0, inc.GetNext());
            Assert.AreEqual(100, inc.GetNext());
            Assert.AreEqual(80, inc.GetNext());
        }

        [Test]
        public void Ctors_MinMax()
        {
            var inc = new Incrementor(0, 100, 20, IncrementMethod.MinMax);
            Assert.AreEqual(IncrementMethod.MinMax, inc.Method);
            Assert.AreEqual(true, inc.Increasing);
            Assert.AreEqual(0, inc.Index);
            Assert.AreEqual(20, inc.GetNext());
            Assert.AreEqual(40, inc.GetNext());
            Assert.AreEqual(60, inc.GetNext());
            Assert.AreEqual(80, inc.GetNext());
            Assert.AreEqual(100, inc.GetNext());
            Assert.AreEqual(0, inc.GetNext());
            Assert.AreEqual(20, inc.GetNext());
        }

        [Test]
        public void CloneTest()
        {
            var inc = new Incrementor(0, 100, 20, IncrementMethod.Cyclic);
            inc.GetNext();
            var clone = inc.Clone();
            Assert.AreEqual(clone.Index, 20);
            Assert.AreEqual(clone.StepSize, 20);
            Assert.AreEqual(clone.Min, 0);
            Assert.AreEqual(clone.Max, 100);
            Assert.AreEqual(clone.Method, IncrementMethod.Cyclic);
            Assert.AreEqual(clone.Increasing, true);
        }

        #endregion

        #region Methods

        [Test]
        public void MinMaxTest()
        {
            var inc = new Incrementor(0, 100, 20, IncrementMethod.MinMax);

            double pan;

            Assert.AreEqual(true, inc.Increasing);
            Assert.AreEqual(0, inc.Index);

            pan = inc.GetNext();
            Assert.AreEqual(20, pan);

            pan = inc.GetNext();
            Assert.AreEqual(40, pan);

            pan = inc.GetNext();
            Assert.AreEqual(60, pan);

            pan = inc.GetNext();
            Assert.AreEqual(80, pan);

            pan = inc.GetNext();
            Assert.AreEqual(100, pan);

            pan = inc.GetNext();
            Assert.AreEqual(0, pan);

            pan = inc.GetNext();
            Assert.AreEqual(20, pan);
        }

        [Test]
        public void MaxMinTest()
        {
            var inc = new Incrementor(0, 100, 20, IncrementMethod.MaxMin);

            double pan;

            Assert.AreEqual(false, inc.Increasing);
            Assert.AreEqual(100, inc.Index);

            pan = inc.GetNext();
            Assert.AreEqual(80, pan);

            pan = inc.GetNext();
            Assert.AreEqual(60, pan);

            pan = inc.GetNext();
            Assert.AreEqual(40, pan);

            pan = inc.GetNext();
            Assert.AreEqual(20, pan);

            pan = inc.GetNext();
            Assert.AreEqual(0, pan);

            pan = inc.GetNext();
            Assert.AreEqual(100, pan);
        }

        [Test]
        public void IncrementMethod_MinMax_step_1()
        {
            var inc = new Incrementor(0, 9, 1, IncrementMethod.MinMax);
            Assert.AreEqual(0.0, inc.Index);
            Assert.AreEqual(1.0, inc.GetNext());
            Assert.AreEqual(2.0, inc.GetNext());
            Assert.AreEqual(3.0, inc.GetNext());
            Assert.AreEqual(4.0, inc.GetNext());
            Assert.AreEqual(5.0, inc.GetNext());
            Assert.AreEqual(6.0, inc.GetNext());
            Assert.AreEqual(7.0, inc.GetNext());
            Assert.AreEqual(8.0, inc.GetNext());
            Assert.AreEqual(9.0, inc.GetNext());
            Assert.AreEqual(0.0, inc.GetNext());
        }

        [Test]
        public void IncrementMethod_MaxMin_step_1()
        {
            var inc = new Incrementor(0, 9, 1, IncrementMethod.MaxMin);
            Assert.AreEqual(9.0, inc.Index);
            Assert.AreEqual(8.0, inc.GetNext());
            Assert.AreEqual(7.0, inc.GetNext());
            Assert.AreEqual(6.0, inc.GetNext());
            Assert.AreEqual(5.0, inc.GetNext());
            Assert.AreEqual(4.0, inc.GetNext());
            Assert.AreEqual(3.0, inc.GetNext());
            Assert.AreEqual(2.0, inc.GetNext());
            Assert.AreEqual(1.0, inc.GetNext());
            Assert.AreEqual(0.0, inc.GetNext());
            Assert.AreEqual(9.0, inc.GetNext());
        }


        [Test]
        [TestCase(0, 9, 1)]
        [TestCase(12, 24, 20)]
        public void IncrementMethod_Bit_step_1(int min, int max, int step)
        {
            var inc = new Incrementor(min, max, step, IncrementMethod.Bit);
            Assert.AreEqual(min, inc.Index);
            Assert.AreEqual(max, inc.GetNext());
            Assert.AreEqual(min, inc.GetNext());
            Assert.AreEqual(max, inc.GetNext());
        }

        [Test]
        [TestCase(0, 9, 1)]
        [TestCase(2, 8, 2)]
        [TestCase(3, 30, 3)]
        [TestCase(1, 8, 3)]
        [TestCase(2, 23, 7)]
        public void IncrementMethod_InitMinValue_Cyclic(int min, int max, int step)
        {
            var inc = new Incrementor(min, max, step, IncrementMethod.Cyclic);
            var value = inc.Index;
            Assert.AreEqual(min, value);
        }

        [TestCase(0, 9, 1,  IncrementMethod.Cyclic)]
        [TestCase(1, 12, 3,  IncrementMethod.MaxMin)]
        [TestCase(3, 343, 26,  IncrementMethod.MinMax)]
        [TestCase(2, 32, 5,  IncrementMethod.Bit)]
        public void IncrementMethod_Peek(int min, int max, int step, IncrementMethod met)
        {
            var inc = new Incrementor(min, max, step, met);
            var oldRight = inc.Increasing;
            var oldValue = inc.Index;
            var oldPreviousValue = inc.PreviousIndex;
            var _ = inc.Peek;
            Assert.AreEqual(oldRight, inc.Increasing);
            Assert.AreEqual(oldValue, inc.Index);
            Assert.AreEqual(oldPreviousValue, inc.PreviousIndex);
        }

        [Test]
        public void IncrementMethod_Cyclic()
        {
            var inc = new Incrementor(2, 8, 2, IncrementMethod.Cyclic);
            Assert.AreEqual(4, inc.GetNext());
            Assert.AreEqual(6, inc.GetNext());
            Assert.AreEqual(8, inc.GetNext());
            Assert.AreEqual(6, inc.GetNext());
            Assert.AreEqual(4, inc.GetNext());
            Assert.AreEqual(2, inc.GetNext());
            Assert.AreEqual(4, inc.GetNext());

            inc = new Incrementor(3, 8, 2, IncrementMethod.Cyclic);
            Assert.AreEqual(5, inc.GetNext());
            Assert.AreEqual(7, inc.GetNext());
            Assert.AreEqual(7, inc.GetNext());
            Assert.AreEqual(5, inc.GetNext());
            Assert.AreEqual(3, inc.GetNext());
            Assert.AreEqual(5, inc.GetNext());
        }

        [Test]
        [TestCase(0, 9, 1)]
        [TestCase(2, 8, 2)]
        [TestCase(3, 30, 3)]
        [TestCase(1, 8, 3)]
        [TestCase(2, 23, 7)]
        public void IncrementMethod_MaxMin(int min, int max, int step)
        {
            var inc = new Incrementor(min, max, step, IncrementMethod.MaxMin);
            var value = inc.Index;
            double expected;
            Assert.AreEqual(max, value);
            for (var i = min; i <= max; i += step) 
            {
                if (inc.Index - inc.StepSize >= inc.Min)
                {
                    expected = inc.Index - inc.StepSize;
                }
                else
                {
                    var remain = inc.Index - inc.Min;
                    expected = inc.Max;
                    if (remain > 0)
                    {
                        expected -= remain;
                    }
                }

                Assert.AreEqual(expected, inc.GetNext());
            }
        }

        [Test]
        [TestCase(0, 9, 1)]
        [TestCase(2, 8, 2)]
        [TestCase(3, 30, 3)]
        [TestCase(1, 8, 3)]
        [TestCase(2, 23, 7)]
        public void IncrementMethod_MinMax(int min, int max, int step)
        {
            var inc = new Incrementor(min, max, step, IncrementMethod.MinMax);
            var value = inc.Index;
            Assert.AreEqual(min, value);
            while (true)
            {
                if (value + step <= max)
                    Assert.AreEqual(value + step, inc.GetNext());
                else
                    break;

                value = inc.Index;
            }
            var expected = min +  (max - (max % (value + step)));
            Assert.AreEqual(expected, inc.GetNext());
        }

        #endregion
    }
}