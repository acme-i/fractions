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
            Assert.AreEqual(100, inc.Value);

            inc = new Incrementor(10, 20, 100, 1, IncrementMethod.MinMax, true); ;
            Assert.AreEqual(20, inc.Value);
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
            Assert.AreEqual(0, original.Value);
            Assert.AreEqual(true, original.Increasing);

            original.Next();
            original.Next();
            Assert.AreEqual(40, original.Value);

            var inc = new Incrementor(original);
            Assert.AreEqual(original.Value, inc.Value);
            Assert.AreEqual(40, inc.Value);

            Assert.AreEqual(original.Step, inc.Step);
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
            Assert.AreEqual(0, inc.Value);
            Assert.AreEqual(20, inc.Next());
            Assert.AreEqual(40, inc.Next());
            Assert.AreEqual(60, inc.Next());
            Assert.AreEqual(80, inc.Next());
            Assert.AreEqual(100, inc.Next());
            Assert.AreEqual(80, inc.Next());
            Assert.AreEqual(60, inc.Next());
            Assert.AreEqual(40, inc.Next());
            Assert.AreEqual(20, inc.Next());
            Assert.AreEqual(0, inc.Next());
            Assert.AreEqual(20, inc.Next());
        }

        [TestCase(0, 9, 1, IncrementMethod.Cyclic)]
        [TestCase(3, 5, 1, IncrementMethod.MinMax)]
        [TestCase(2, 23, 7, IncrementMethod.Bit)]
        public void IncrementMethod_Ctor(int min, int max, int step, IncrementMethod met)
        {
            var inc = new Incrementor(min, max, step, met);
            Assert.AreEqual(min, inc.Value);
            Assert.AreEqual(null, inc.PreviousValue);
            Assert.AreEqual(step, inc.Step);
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
            Assert.AreEqual(max, inc.Value);
            Assert.AreEqual(null, inc.PreviousValue);
            Assert.AreEqual(step, inc.Step);
            Assert.AreEqual(min, inc.Min);
            Assert.AreEqual(max, inc.Max);
            Assert.AreEqual(met, inc.Method);
            Assert.AreEqual(false, inc.Increasing);
        }

        [Test]
        public void Ctors_MaxMin()
        {
            var inc = new Incrementor(0, 100, 20, IncrementMethod.MaxMin);
            Assert.AreEqual(IncrementMethod.MaxMin, inc.Method);
            Assert.AreEqual(false, inc.Increasing);
            Assert.AreEqual(100, inc.Value);
            Assert.AreEqual(80, inc.Next());
            Assert.AreEqual(60, inc.Next());
            Assert.AreEqual(40, inc.Next());
            Assert.AreEqual(20, inc.Next());
            Assert.AreEqual(0, inc.Next());
            Assert.AreEqual(100, inc.Next());
            Assert.AreEqual(80, inc.Next());
        }

        [Test]
        public void Ctors_MinMax()
        {
            var inc = new Incrementor(0, 100, 20, IncrementMethod.MinMax);
            Assert.AreEqual(IncrementMethod.MinMax, inc.Method);
            Assert.AreEqual(true, inc.Increasing);
            Assert.AreEqual(0, inc.Value);
            Assert.AreEqual(20, inc.Next());
            Assert.AreEqual(40, inc.Next());
            Assert.AreEqual(60, inc.Next());
            Assert.AreEqual(80, inc.Next());
            Assert.AreEqual(100, inc.Next());
            Assert.AreEqual(0, inc.Next());
            Assert.AreEqual(20, inc.Next());
        }

        [Test]
        public void CloneTest()
        {
            var inc = new Incrementor(0, 100, 20, IncrementMethod.Cyclic);
            inc.Next();
            inc.Next();
            var clone = inc.Clone();
            Assert.AreEqual(inc.Value, clone.Value);
            Assert.AreEqual(inc.Step, clone.Step);
            Assert.AreEqual(inc.Min, clone.Min);
            Assert.AreEqual(inc.Max, clone.Max);
            Assert.AreEqual(inc.Method, clone.Method);
            Assert.AreEqual(inc.Increasing, clone.Increasing);
        }

        #endregion

        #region Methods

        [Test]
        public void MinMaxTest()
        {
            var inc = new Incrementor(0, 100, 20, IncrementMethod.MinMax);

            double pan;

            Assert.AreEqual(true, inc.Increasing);
            Assert.AreEqual(0, inc.Value);

            pan = inc.Next();
            Assert.AreEqual(20, pan);

            pan = inc.Next();
            Assert.AreEqual(40, pan);

            pan = inc.Next();
            Assert.AreEqual(60, pan);

            pan = inc.Next();
            Assert.AreEqual(80, pan);

            pan = inc.Next();
            Assert.AreEqual(100, pan);

            pan = inc.Next();
            Assert.AreEqual(0, pan);

            pan = inc.Next();
            Assert.AreEqual(20, pan);
        }

        [Test]
        public void MaxMinTest()
        {
            var inc = new Incrementor(0, 100, 20, IncrementMethod.MaxMin);

            double pan;

            Assert.AreEqual(false, inc.Increasing);
            Assert.AreEqual(100, inc.Value);

            pan = inc.Next();
            Assert.AreEqual(80, pan);

            pan = inc.Next();
            Assert.AreEqual(60, pan);

            pan = inc.Next();
            Assert.AreEqual(40, pan);

            pan = inc.Next();
            Assert.AreEqual(20, pan);

            pan = inc.Next();
            Assert.AreEqual(0, pan);

            pan = inc.Next();
            Assert.AreEqual(100, pan);
        }

        [Test]
        public void IncrementMethod_MinMax_step_1()
        {
            var inc = new Incrementor(0, 9, 1, IncrementMethod.MinMax);
            Assert.AreEqual(0.0, inc.Value);
            Assert.AreEqual(1.0, inc.Next());
            Assert.AreEqual(2.0, inc.Next());
            Assert.AreEqual(3.0, inc.Next());
            Assert.AreEqual(4.0, inc.Next());
            Assert.AreEqual(5.0, inc.Next());
            Assert.AreEqual(6.0, inc.Next());
            Assert.AreEqual(7.0, inc.Next());
            Assert.AreEqual(8.0, inc.Next());
            Assert.AreEqual(9.0, inc.Next());
            Assert.AreEqual(0.0, inc.Next());
        }

        [Test]
        public void IncrementMethod_MaxMin_step_1()
        {
            var inc = new Incrementor(0, 9, 1, IncrementMethod.MaxMin);
            Assert.AreEqual(9.0, inc.Value);
            Assert.AreEqual(8.0, inc.Next());
            Assert.AreEqual(7.0, inc.Next());
            Assert.AreEqual(6.0, inc.Next());
            Assert.AreEqual(5.0, inc.Next());
            Assert.AreEqual(4.0, inc.Next());
            Assert.AreEqual(3.0, inc.Next());
            Assert.AreEqual(2.0, inc.Next());
            Assert.AreEqual(1.0, inc.Next());
            Assert.AreEqual(0.0, inc.Next());
            Assert.AreEqual(9.0, inc.Next());
        }


        [Test]
        [TestCase(0, 9, 1)]
        [TestCase(12, 24, 20)]
        public void IncrementMethod_Bit_step_1(int min, int max, int step)
        {
            var inc = new Incrementor(min, max, step, IncrementMethod.Bit);
            Assert.AreEqual(min, inc.Value);
            Assert.AreEqual(max, inc.Next());
            Assert.AreEqual(min, inc.Next());
            Assert.AreEqual(max, inc.Next());
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
            var value = inc.Value;
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
            var oldValue = inc.Value;
            var oldPreviousValue = inc.PreviousValue;
            inc.Peek();
            Assert.AreEqual(oldRight, inc.Increasing);
            Assert.AreEqual(oldValue, inc.Value);
            Assert.AreEqual(oldPreviousValue, inc.PreviousValue);
        }

        [Test]
        public void IncrementMethod_Cyclic()
        {
            var inc = new Incrementor(2, 8, 2, IncrementMethod.Cyclic);
            Assert.AreEqual(4, inc.Next());
            Assert.AreEqual(6, inc.Next());
            Assert.AreEqual(8, inc.Next());
            Assert.AreEqual(6, inc.Next());
            Assert.AreEqual(4, inc.Next());
            Assert.AreEqual(2, inc.Next());
            Assert.AreEqual(4, inc.Next());

            inc = new Incrementor(3, 8, 2, IncrementMethod.Cyclic);
            Assert.AreEqual(5, inc.Next());
            Assert.AreEqual(7, inc.Next());
            Assert.AreEqual(7, inc.Next());
            Assert.AreEqual(5, inc.Next());
            Assert.AreEqual(3, inc.Next());
            Assert.AreEqual(5, inc.Next());
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
            var value = inc.Value;
            double expected;
            Assert.AreEqual(max, value);
            for (var i = min; i <= max; i += step) 
            {
                if (inc.Value - inc.Step >= inc.Min)
                {
                    expected = inc.Value - inc.Step;
                }
                else
                {
                    var remain = inc.Value - inc.Min;
                    expected = inc.Max;
                    if (remain > 0)
                    {
                        expected -= remain;
                    }
                }

                Assert.AreEqual(expected, inc.Next());
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
            var value = inc.Value;
            Assert.AreEqual(min, value);
            while (true)
            {
                if (value + step <= max)
                    Assert.AreEqual(value + step, inc.Next());
                else
                    break;

                value = inc.Value;
            }
            var expected = min +  (max - (max % (value + step)));
            Assert.AreEqual(expected, inc.Next());
        }

        #endregion
    }
}