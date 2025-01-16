using NUnit.Framework;
using System;

namespace fractions.tests
{
    [TestFixture]
    public class FractionTests
    {
        private Fraction CreateFraction(float baseNum)
        {
            return new Fraction(baseNum);
        }

        [Test]
        public void ConstructWithZero_ThrowsArgumentException()
        {
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => this.CreateFraction(0f));
        }

        [Test]
        public void Over_Test()
        {
            // Arrange
            var fraction = this.CreateFraction(2f);
            float denom = 4;

            // Act
            var result = fraction.Over(denom); // 2f / 4f

            // Assert
            Assert.AreEqual(0.5f, result);
        }

        [Test]
        public void OverNeg_Test()
        {
            // Arrange
            var fraction = this.CreateFraction(-2f);
            float denom = 4;

            // Act
            var result = fraction.Over(denom); // -2f / 4f

            // Assert
            Assert.AreEqual(-0.5f, result);
        }

        [Test]
        public void UnderNeg_Test()
        {
            // Arrange
            var fraction = this.CreateFraction(-2f);
            float nume = 4;

            // Act
            var result = fraction.Under(nume); // 4f / -2f

            // Assert
            Assert.AreEqual(-2f, result);
        }
    }
}