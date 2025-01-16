using System.Collections.Generic;
using System.Linq;

namespace fractions
{
    public static class Fractions
    {
        public static readonly Fraction One = new Fraction(1f);
        public static readonly Fraction Two = new Fraction(2f);
        public static readonly Fraction Three = new Fraction(3f);
        public static readonly Fraction Four = new Fraction(4f);
        public static readonly Fraction Five = new Fraction(5f);
        public static readonly Fraction Six = new Fraction(6f);
        public static readonly Fraction Seven = new Fraction(7f);
        public static readonly Fraction Eight = new Fraction(8f);
        public static readonly Fraction Nine = new Fraction(9f);
        public static readonly Fraction Ten = new Fraction(10f);
        public static readonly Fraction Elleven = new Fraction(11f);
        public static readonly Fraction Twelve = new Fraction(12f);
        public static readonly Fraction Thirteen = new Fraction(13f);
        public static readonly Fraction Fourteen = new Fraction(14f);
        public static readonly Fraction Fifteen = new Fraction(15f);
        public static readonly Fraction Sixteen = new Fraction(16f);
        public static readonly Fraction Seventeen = new Fraction(17f);
        public static readonly Fraction Eightteen = new Fraction(18f);
        public static readonly Fraction Nineteen = new Fraction(19f);
        public static readonly Fraction Twenty = new Fraction(20f);
        public static readonly Fraction TwentyOne = new Fraction(21f);
        public static readonly Fraction TwentyTwo = new Fraction(22f);
        public static readonly Fraction TwentyThree = new Fraction(23f);
        public static readonly Fraction TwentyFour = new Fraction(24f);
        public static readonly Fraction TwentyFive = new Fraction(25f);
        public static readonly Fraction TwentySix = new Fraction(26f);
        public static readonly Fraction TwentySeven = new Fraction(27f);
        public static readonly Fraction TwentyEight = new Fraction(28f);
        public static readonly Fraction TwentyNine = new Fraction(29f);
        public static readonly Fraction Thirty = new Fraction(30f);
        public static readonly Fraction ThirtyOne = new Fraction(31f);
        public static readonly Fraction ThirtyTwo = new Fraction(32f);
        public static readonly Fraction ThirtyThree = new Fraction(33f);
        public static readonly Fraction ThirtyFour = new Fraction(34f);
        public static readonly Fraction ThirtyFive = new Fraction(35f);
        public static readonly Fraction ThirtySix = new Fraction(36f);
        public static readonly Fraction ThirtySeven = new Fraction(37f);
        public static readonly Fraction ThirtyEight = new Fraction(38f);
        public static readonly Fraction ThirtyNine = new Fraction(39f);
        public static readonly Fraction Fourty = new Fraction(40f);
        public static readonly Fraction FourtyOne = new Fraction(41f);
        public static readonly Fraction FourtyTwo = new Fraction(42f);
        public static readonly Fraction FourtyThree = new Fraction(43f);
        public static readonly Fraction FourtyFour = new Fraction(44f);
        public static readonly Fraction FourtyFive = new Fraction(45f);
        public static readonly Fraction FourtySix = new Fraction(46f);
        public static readonly Fraction FourtySeven = new Fraction(47f);
        public static readonly Fraction FourtyEight = new Fraction(48f);
        public static readonly Fraction FourtyNine = new Fraction(49f);
        public static readonly Fraction Fifty = new Fraction(50f);
        public static readonly Fraction FiftyOne = new Fraction(51f);
        public static readonly Fraction FiftyTwo = new Fraction(52f);
        public static readonly Fraction FiftyThree = new Fraction(53f);
        public static readonly Fraction FiftyFour = new Fraction(54f);
        public static readonly Fraction FiftyFive = new Fraction(55f);
        public static readonly Fraction FiftySix = new Fraction(56f);
        public static readonly Fraction FiftySeven = new Fraction(57f);
        public static readonly Fraction FiftyEight = new Fraction(58f);
        public static readonly Fraction FiftyNine = new Fraction(59f);
        public static readonly Fraction Sixty = new Fraction(60f);
        public static readonly Fraction SixtyOne = new Fraction(61f);
        public static readonly Fraction SixtyTwo = new Fraction(62f);
        public static readonly Fraction SixtyThree = new Fraction(63f);
        public static readonly Fraction SixtyFour = new Fraction(64f);

        public static readonly List<Fraction> AllFractions = new List<Fraction>
        {
            One, Two, Three, Four, Five, Six, Seven, Eight, Nine,
            Ten, Elleven, Twelve, Thirteen, Fourteen, Fifteen, Sixteen, Seventeen, Eightteen, Nineteen,
            Twenty, TwentyOne, TwentyTwo, TwentyThree, TwentyFour, TwentyFive, TwentySix, TwentySeven, TwentyEight, TwentyNine,
            Thirty, ThirtyOne, ThirtyTwo, ThirtyThree, ThirtyFour, ThirtyFive, ThirtySix, ThirtySeven, ThirtyEight, ThirtyNine,
            Fourty, FourtyOne, FourtyTwo, FourtyThree, FourtyFour, FourtyFive, FourtySix, FourtySeven, FourtyEight, FourtyNine,
            Fifty,  FiftyOne,  FiftyTwo,  FiftyThree,  FiftyFour,  FiftyFive,  FiftySix,  FiftySeven,  FiftyEight,  FiftyNine,
            Sixty,  SixtyOne,  SixtyTwo,  SixtyThree,  SixtyFour
        };

        /// <summary>
        /// AllFraction where base % 2 == 0
        /// </summary>
        public static List<Fraction> Evens = AllFractions.Where(f => f.Base % 2 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 2 != 0
        /// </summary>
        public static List<Fraction> Odds = AllFractions.Where(f => f.Base % 2 != 0).ToList();
        /// <summary>
        /// AllFraction where base % 3 == 0
        /// </summary>
        public static List<Fraction> Thirds = AllFractions.Where(f => f.Base % 3 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 4 == 0
        /// </summary>
        public static List<Fraction> Fourths = AllFractions.Where(f => f.Base % 4 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 5 == 0
        /// </summary>
        public static List<Fraction> Fifths = AllFractions.Where(f => f.Base % 5 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 6 == 0
        /// </summary>
        public static List<Fraction> Sixths = AllFractions.Where(f => f.Base % 6 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 7 == 0
        /// </summary>
        public static List<Fraction> Sevenths = AllFractions.Where(f => f.Base % 7 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 8 == 0
        /// </summary>
        public static List<Fraction> Eighths = AllFractions.Where(f => f.Base % 8 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 9 == 0
        /// </summary>
        public static List<Fraction> Nineths = AllFractions.Where(f => f.Base % 9 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 10 == 0
        /// </summary>
        public static List<Fraction> Tenths = AllFractions.Where(f => f.Base % 10 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 11 == 0
        /// </summary>
        public static List<Fraction> Ellevenths = AllFractions.Where(f => f.Base % 11 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 12 == 0
        /// </summary>
        public static List<Fraction> Twelveths = AllFractions.Where(f => f.Base % 12 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 13 == 0
        /// </summary>
        public static List<Fraction> Thirteenths = AllFractions.Where(f => f.Base % 13 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 14 == 0
        /// </summary>
        public static List<Fraction> Fourteenths = AllFractions.Where(f => f.Base % 14 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 14 == 0
        /// </summary>
        public static List<Fraction> Fifteenths = AllFractions.Where(f => f.Base % 15 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 16 == 0
        /// </summary>
        public static List<Fraction> Sixteenths = AllFractions.Where(f => f.Base % 16 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 17 == 0
        /// </summary>
        public static List<Fraction> Seventeenths = AllFractions.Where(f => f.Base % 17 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 18 == 0
        /// </summary>
        public static List<Fraction> Eightsteenths = AllFractions.Where(f => f.Base % 18 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 19 == 0
        /// </summary>
        public static List<Fraction> Nineteenths = AllFractions.Where(f => f.Base % 19 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 20 == 0
        /// </summary>
        public static List<Fraction> Twenteenths = AllFractions.Where(f => f.Base % 20 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 21 == 0
        /// </summary>
        public static List<Fraction> TwentyOneths = AllFractions.Where(f => f.Base % 21 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 22 == 0
        /// </summary>
        public static List<Fraction> TwentyTwoths = AllFractions.Where(f => f.Base % 22 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 23 == 0
        /// </summary>
        public static List<Fraction> TwentyThreeths = AllFractions.Where(f => f.Base % 23 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 24 == 0
        /// </summary>
        public static List<Fraction> TwentyFourths = AllFractions.Where(f => f.Base % 24 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 25 == 0
        /// </summary>
        public static List<Fraction> TwentyFifths = AllFractions.Where(f => f.Base % 25 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 26 == 0
        /// </summary>
        public static List<Fraction> TwentySixths = AllFractions.Where(f => f.Base % 26 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 27 == 0
        /// </summary>
        public static List<Fraction> TwentySevenths = AllFractions.Where(f => f.Base % 27 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 28 == 0
        /// </summary>
        public static List<Fraction> TwentyEighths = AllFractions.Where(f => f.Base % 28 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 29 == 0
        /// </summary>
        public static List<Fraction> TwentyNineths = AllFractions.Where(f => f.Base % 29 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 30 == 0
        /// </summary>
        public static List<Fraction> Thirtyths = AllFractions.Where(f => f.Base % 30 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 31 == 0
        /// </summary>
        public static List<Fraction> ThirtyOneths = AllFractions.Where(f => f.Base % 31 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 32 == 0
        /// </summary>
        public static List<Fraction> ThirtySeconds = AllFractions.Where(f => f.Base % 32 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 33 == 0
        /// </summary>
        public static List<Fraction> ThirtyThreeths = AllFractions.Where(f => f.Base % 33 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 34 == 0
        /// </summary>
        public static List<Fraction> ThirtyFourths = AllFractions.Where(f => f.Base % 34 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 35 == 0
        /// </summary>
        public static List<Fraction> ThirtyFifths = AllFractions.Where(f => f.Base % 35 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 36 == 0
        /// </summary>
        public static List<Fraction> ThirtySixths = AllFractions.Where(f => f.Base % 36 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 37 == 0
        /// </summary>
        public static List<Fraction> ThirtySevenths = AllFractions.Where(f => f.Base % 37 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 38 == 0
        /// </summary>
        public static List<Fraction> ThirtyEighths = AllFractions.Where(f => f.Base % 38 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 39 == 0
        /// </summary>
        public static List<Fraction> ThirtyNineths = AllFractions.Where(f => f.Base % 39 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 40 == 0
        /// </summary>
        public static List<Fraction> Fourtyths = AllFractions.Where(f => f.Base % 40 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 41 == 0
        /// </summary>
        public static List<Fraction> FourtyOneths = AllFractions.Where(f => f.Base % 41 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 42 == 0
        /// </summary>
        public static List<Fraction> FourtyTwooths = AllFractions.Where(f => f.Base % 42 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 43 == 0
        /// </summary>
        public static List<Fraction> FourtyThreeths = AllFractions.Where(f => f.Base % 43 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 44 == 0
        /// </summary>
        public static List<Fraction> FourtyFourths = AllFractions.Where(f => f.Base % 44 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 45 == 0
        /// </summary>
        public static List<Fraction> FourtyFifths = AllFractions.Where(f => f.Base % 45 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 46 == 0
        /// </summary>
        public static List<Fraction> FourtySixths = AllFractions.Where(f => f.Base % 46 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 47 == 0
        /// </summary>
        public static List<Fraction> FourtySevenths = AllFractions.Where(f => f.Base % 47 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 48 == 0
        /// </summary>
        public static List<Fraction> FourtyEighths = AllFractions.Where(f => f.Base % 48 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 49 == 0
        /// </summary>
        public static List<Fraction> FourtyNineths = AllFractions.Where(f => f.Base % 49 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 50 == 0
        /// </summary>
        public static List<Fraction> Fiftyths = AllFractions.Where(f => f.Base % 50 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 51 == 0
        /// </summary>
        public static List<Fraction> FiftyOneths = AllFractions.Where(f => f.Base % 51 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 52 == 0
        /// </summary>
        public static List<Fraction> FiftyTwoths = AllFractions.Where(f => f.Base % 52 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 53 == 0
        /// </summary>
        public static List<Fraction> FiftyThreeths = AllFractions.Where(f => f.Base % 53 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 54 == 0
        /// </summary>
        public static List<Fraction> FiftyFourths = AllFractions.Where(f => f.Base % 54 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 55 == 0
        /// </summary>
        public static List<Fraction> FiftyFifths = AllFractions.Where(f => f.Base % 55 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 56 == 0
        /// </summary>
        public static List<Fraction> FiftySixths = AllFractions.Where(f => f.Base % 56 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 57 == 0
        /// </summary>
        public static List<Fraction> FiftySevenths = AllFractions.Where(f => f.Base % 57 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 58 == 0
        /// </summary>
        public static List<Fraction> FiftyEighths = AllFractions.Where(f => f.Base % 58 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 59 == 0
        /// </summary>
        public static List<Fraction> FiftyNineths = AllFractions.Where(f => f.Base % 59 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 60 == 0
        /// </summary>
        public static List<Fraction> Sixtyths = AllFractions.Where(f => f.Base % 60 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 61 == 0
        /// </summary>
        public static List<Fraction> SixtyOneths = AllFractions.Where(f => f.Base % 61 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 62 == 0
        /// </summary>
        public static List<Fraction> SixtySeconds = AllFractions.Where(f => f.Base % 62 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 63 == 0
        /// </summary>
        public static List<Fraction> SixtyThreeths = AllFractions.Where(f => f.Base % 63 == 0).ToList();
        /// <summary>
        /// AllFraction where base % 64 == 0
        /// </summary>
        public static List<Fraction> SixtyFourths = AllFractions.Where(f => f.Base % 64 == 0).ToList();

        /// <summary>
        /// Receive a list of fractions having the given base
        /// </summary>
        public static List<Fraction> Generate(IEnumerable<int> bases)
        {
            var fs = bases.Select(b => new Fraction(b));
            return fs.ToList();
        }

        /// <summary>
        /// Receive a list of fractions having the given base
        /// </summary>
        public static List<Fraction> WithBases(IEnumerable<float> bases)
        {
            var fs = bases.Select(b => new Fraction(b));
            return fs.ToList();
        }

        /// <summary>
        /// Receive a list of fractions whose Base divedes evenly with div
        /// </summary>
        public static List<Fraction> Generate(IEnumerable<int> bases, float div)
        {
            var fs = bases.Select(b => new Fraction(b)).ToList();
            return fs.Where(f => f.Base % div == 0).ToList();
        }

        /// <summary>
        /// Receive a list of fractions whose Base divedes evenly with div
        /// </summary>
        public static List<Fraction> Generate(IEnumerable<float> bases, float div)
        {
            var fs = bases.Select(b => new Fraction(b)).ToList();
            return fs.Where(f => f.Base % div == 0).ToList();
        }

        public static List<Fraction> GenerateAllFractions(float div)
        {
            return AllFractions.Where(f => f.Base % div == 0).ToList();
        }

        public static List<Fraction> GenerateAllFractions(IEnumerable<float> divs)
        {
            var results = new List<Fraction>();
            foreach (var div in divs)
            {
                results.AddRange(GenerateAllFractions(div));
            }
            return results;
        }
    }
}
