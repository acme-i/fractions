using System.Linq;
using NUnit.Framework;

namespace fractions.tests
{
    [TestFixture]
    public class FractionsTests
    {
        [Test]
        public void GenerateTest()
        {
            var bases = Enumerable.Range(1, 7).ToList();
            var fs = Fractions.Generate(bases);

            Assert.AreEqual(bases.Count, fs.Count);

            foreach (var b in bases)
            {
                Assert.AreEqual(b, fs[b - 1].Base);
            }
        }

        [Test]
        public void GenerateDivTest()
        {
            var bases = Enumerable.Range(1, 7).ToList();
            Assert.AreEqual(bases[0], 1);
            Assert.AreEqual(bases[1], 2);
            Assert.AreEqual(bases[2], 3);
            Assert.AreEqual(bases[3], 4);
            Assert.AreEqual(bases[4], 5);
            Assert.AreEqual(bases[5], 6);
            Assert.AreEqual(bases[6], 7);

            var fs = Fractions.Generate(bases, 2);
            Assert.AreEqual(fs.Count, 3);
            Assert.AreEqual(fs[0].Base, 2f);
            Assert.AreEqual(fs[1].Base, 4f);
            Assert.AreEqual(fs[2].Base, 6f);
        }
    }
}