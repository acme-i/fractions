using System;
using System.Collections.Generic;
using System.Linq;

namespace fractions
{
    public class Fraction
    {
        public Fraction(float baseNumber)
        {
            if (Math.Sign(baseNumber) == 0)
                throw new ArgumentOutOfRangeException();

            this.Base = baseNumber;
        }

        public float Base { get; private set; }

        public float Over(float denom)
        {
            if (denom == 0) throw new DivideByZeroException();

            return Base / denom;
        }

        public float Under(float numerator)
        {
            if (numerator == 0) return 0f;

            return numerator / Base;
        }

        public float Identity { get { return Over(1f); } }
        public float Half { get { return Over(2f); } }
        public float Third { get { return Over(3f); } }
        public float Fourth { get { return Over(4f); } }
        public float Fifth { get { return Over(5f); } }
        public float Sixth { get { return Over(6f); } }
        public float Seventh { get { return Over(7f); } }
        public float Eighth { get { return Over(8f); } }
        public float Nineth { get { return Over(9f); } }
        public float Tenth { get { return Over(10f); } }
        public float Elleventh { get { return Over(11f); } }
        public float Twelveth { get { return Over(12f); } }
        public float Thirteenth { get { return Over(13f); } }
        public float Fourteenth { get { return Over(14f); } }
        public float Fifteenth { get { return Over(15f); } }
        public float Sixteenth { get { return Over(16f); } }
        public float Seventeenth { get { return Over(17f); } }
        public float Eigthteenth { get { return Over(18f); } }
        public float Nineteenth { get { return Over(19f); } }
        public float Twenteenth { get { return Over(20f); } }
        public float TwentyOneth { get { return Over(21f); } }
        public float TwentySecond { get { return Over(22f); } }
        public float TwentyThird { get { return Over(23f); } }
        public float TwentyFourth { get { return Over(24f); } }
        public float TwentyFifth { get { return Over(25f); } }
        public float TwentySixth { get { return Over(26f); } }
        public float TwentySeventh { get { return Over(27f); } }
        public float TwentyEighth { get { return Over(28f); } }
        public float TwentyNineth { get { return Over(29f); } }
        public float Thirtyth { get { return Over(30f); } }
        public float ThirtyOneth { get { return Over(31f); } }
        public float ThirtySecond { get { return Over(32f); } }
        public float ThirtyThird { get { return Over(33f); } }
        public float ThirtyFourth { get { return Over(34f); } }
        public float ThirtyFifth { get { return Over(35f); } }
        public float ThirtySixth { get { return Over(36f); } }
        public float ThirtySeventh { get { return Over(37f); } }
        public float ThirtyEighth { get { return Over(38f); } }
        public float ThirtyNineth { get { return Over(39f); } }
        public float Fourtyth { get { return Over(40f); } }
        public float FourtyOneth { get { return Over(41f); } }
        public float FourtySecond { get { return Over(42f); } }
        public float FourtyThird { get { return Over(43f); } }
        public float FourtyFourth { get { return Over(44f); } }
        public float FourtyFifth { get { return Over(45f); } }
        public float FourtySixth { get { return Over(46f); } }
        public float FourtySeventh { get { return Over(47f); } }
        public float FourtyEighth { get { return Over(48f); } }
        public float FourtyNineth { get { return Over(49f); } }
        public float Fiftyth { get { return Over(50f); } }
        public float FiftyOneth { get { return Over(51f); } }
        public float FiftySecond { get { return Over(52f); } }
        public float FiftyThird { get { return Over(53f); } }
        public float FiftyFourth { get { return Over(54f); } }
        public float FiftyFifth { get { return Over(55f); } }
        public float FiftySixth { get { return Over(56f); } }
        public float FiftySeventh { get { return Over(57f); } }
        public float FiftyEigth { get { return Over(58f); } }
        public float FiftyNineth { get { return Over(59f); } }
        public float Sixtyth { get { return Over(60f); } }
        public float SixtyOneth { get { return Over(61f); } }
        public float SixtySecond { get { return Over(62f); } }
        public float SixtyThird { get { return Over(63f); } }
        public float SixtyFourth { get { return Over(64f); } }

        public List<float> Fourths
        {
            get
            {
                return new List<float>
                {
                    Identity, Half, Third, Fourth
                };
            }
        }

        public List<float> Eighths
        {
            get
            {
                return new List<float>
                {
                    Identity, Half, Third, Fourth, Fifth, Sixth, Seventh, Eighth,
                };
            }
        }

        public List<float> Sixteenths
        {
            get
            {
                return new List<float>
                {
                    Identity, Half, Third, Fourth, Fifth, Sixth, Seventh, Eighth, Nineth,
                    Tenth, Elleventh, Twelveth, Thirteenth, Fourteenth, Fifteenth, Sixteenth
                };
            }
        }

        public List<float> ThirtySeconds
        {
            get
            {
                return new List<float>
                {
                    Identity, Half, Third, Fourth, Fifth, Sixth, Seventh, Eighth, Nineth,
                    Tenth, Elleventh, Twelveth, Thirteenth, Fourteenth, Fifteenth, Sixteenth, Seventeenth, Eigthteenth, Nineteenth,
                    Twenteenth, TwentyOneth, TwentySecond, TwentyThird, TwentyFourth, TwentyFifth, TwentySixth, TwentySeventh, TwentyEighth, TwentyNineth,
                    Thirtyth, ThirtyOneth, ThirtySecond
                };
            }
        }

        public List<float> SixtyFourths
        {
            get
            {
                return new List<float>
                {
                    Identity, Half, Third, Fourth, Fifth, Sixth, Seventh, Eighth, Nineth,
                    Tenth, Elleventh, Twelveth, Thirteenth, Fourteenth, Fifteenth, Sixteenth, Seventeenth, Eigthteenth, Nineteenth,
                    Twenteenth, TwentyOneth, TwentySecond, TwentyThird, TwentyFourth, TwentyFifth, TwentySixth, TwentySeventh, TwentyEighth, TwentyNineth,
                    Thirtyth, ThirtyOneth, ThirtySecond, ThirtyThird, ThirtyFourth, ThirtyFifth, ThirtySixth, ThirtySeventh, ThirtyEighth, ThirtyNineth,
                    Fourtyth, FourtyOneth, FourtySecond, FourtyThird, FourtyFourth, FourtyFifth, FourtySixth, FourtySeventh, FourtyEighth, FourtyNineth,
                    Fiftyth, FiftyOneth, FiftySecond, FiftyThird, FiftyFourth, FiftyFifth, FiftySixth, FiftySeventh, FiftyEigth, FiftyNineth,
                    Sixtyth, SixtyOneth, SixtySecond, SixtyThird, SixtyFourth
                };
            }
        }

        /// <summary>
        /// ResultsBetween(float start = 2f, float end = 16f, float stepSize = 2f)
        /// yields
        /// 2 / 16
        /// 4 / 16
        /// 6 / 16
        /// 8 / 16
        /// 10 / 16
        /// 12 / 16
        /// 14 / 16
        /// 16 / 16
        /// </summary>
        /// <param name="start">start denominator</param>
        /// <param name="end">end denominator</param>
        /// <param name="stepSize">spacing between range of denominators</param>
        /// <param name="moveIntoRange">whether to clamp stepSize into [start; end]</param>
        /// <returns></returns>
        private IEnumerable<float> ResultsBetween(float start = 2f, float end = 32f, float stepSize = 2f, bool moveIntoRange = false)
        {
            if (Math.Sign(stepSize) == 0)
            {
                if (moveIntoRange)
                    stepSize = 1;
                else
                    throw new ArgumentOutOfRangeException($"{nameof(start)} and {nameof(end)} must be bigger than zero");
            }

            if (Math.Sign(start) <= 0 && Math.Sign(end) <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(start)} and {nameof(end)} must be bigger than zero");

            if (Math.Sign(end - start) < 0)
                throw new ArgumentOutOfRangeException($"{nameof(start)} must be smaller than {nameof(end)} and bigger than zero");

            if (Math.Sign(end - start) == 0)
            {
                yield return 1f;
            }
            else
            {
                if (moveIntoRange && stepSize > end)
                    stepSize = start + (stepSize % (end - start));

                for (float s = start; s <= end; s += stepSize)
                    yield return Base / s;
            }
        }

        /// <summary>
        /// ResultsBetween(float start = 2f, float end = 16f, float stepSize = 2f)
        /// yields
        /// 2 / 16
        /// 4 / 16
        /// 6 / 16
        /// 8 / 16
        /// 10 / 16
        /// 12 / 16
        /// 14 / 16
        /// 16 / 16
        /// </summary>
        /// <param name="start">start denominator</param>
        /// <param name="end">end denominator</param>
        /// <param name="stepSize">spacing between range of denominators</param>
        /// <param name="includeOne">whether to include 1f at index 0</param>
        /// <param name="moveIntoRange">whether to clamp stepSize into [start; end]</param>
        /// <returns></returns>
        public List<float> ResultsBetween(float start = 2f, float end = 32f, float stepSize = 2f, bool includeOne = false, bool moveIntoRange = false)
        {
            if (Math.Sign(stepSize) == 0)
            {
                if (moveIntoRange)
                    stepSize = 1;
                else
                    throw new ArgumentOutOfRangeException($"{nameof(stepSize)} must be bigger than zero");
            }

            var results = ResultsBetween(start, end, stepSize, moveIntoRange).ToList();
            if (includeOne)
            {
                var insert = false;
                if (results.Any())
                    insert = results[0] != 1f;
                if (insert)
                    results.Insert(0, 1f);
            }
            return results;
        }
    }
}
