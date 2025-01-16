using System;
using System.Collections.Generic;
using System.Linq;

namespace fractions
{
    public static class EnumerateExtension
    {
        public static Pitch PitchAbove(this Pitch p, int semiTones, bool wrapAround = false)
        {
            var q = ((int)p) + semiTones;
            if (q > 127)
            {
                p = wrapAround
                    ? (Pitch)(q % 127)
                    : p;
            }
            else
            {
                p = (Pitch)q;
            }
            return p;
        }

        public static Enumerate<Pitch> PitchAbove(this Enumerate<Pitch> pitches, int semiTones, bool wrapAround = false)
        {
            if (pitches != null && semiTones > 0)
                for (int i = 0; i < pitches.Length; i++)
                    pitches.collection[i] = pitches.collection[i].PitchAbove(semiTones, wrapAround);
            return pitches;
        }

        public static Pitch OctaveAbove(this Pitch p, bool wrapAround = false)
        {
            return PitchAbove(p, 12, wrapAround);
        }

        public static Pitch OctaveAbove(this Pitch p, int numberOfOctaves, Random rand, double stopProp, bool wrapAround = false)
        {
            for (int i = 0; i < numberOfOctaves; i++)
            {
                p = p.OctaveAbove(wrapAround);
                if (rand.NextDouble() < stopProp)
                    break;
            }
            return p;
        }

        public static Enumerate<Pitch> OctaveAbove(this Enumerate<Pitch> pitches, bool wrapAround = false)
        {
            if (pitches != null)
                for (int i = 0; i < pitches.Length; i++)
                    pitches.collection[i] = pitches.collection[i].OctaveAbove(wrapAround);
            return pitches;
        }

        public static Enumerate<Pitch> OctaveAbove(this Enumerate<Pitch> pitches, int octaves, double prop, Random rand, bool wrapAround = false)
        {
            if (pitches != null)
                for (int i = 0; i < pitches.Length; i++)
                    pitches.collection[i] = pitches.collection[i].OctaveAbove(octaves, rand, prop, wrapAround);
            return pitches;
        }

        public static Pitch OctaveBelow(this Pitch p, bool wrapAround = false)
        {
            if ((int)p - 12 < 0)
            {
                p = wrapAround
                    ? 127 - p
                    : p;

            }
            else
            {
                if (p - 12 >= Pitch.CNeg1)
                    p -= 12;
            }
            return p;
        }

        public static Pitch OctaveBelow(this Pitch p, int octaves, Random rand, double prop, bool wrapAround = false)
        {
            for (int i = 0; i < octaves; i++)
            {
                p = p.OctaveBelow(wrapAround);
                if (rand.NextDouble() < prop)
                    break;
            }
            return p;
        }

        public static Enumerate<Pitch> OctaveBelow(this Enumerate<Pitch> pitches, bool wrapAround = false)
        {
            if (pitches != null)
                for (int i = 0; i < pitches.Length; i++)
                    pitches.collection[i] = pitches.collection[i].OctaveBelow(wrapAround);
            return pitches;
        }

        public static Enumerate<Pitch> OctaveBelow(this Enumerate<Pitch> pitches, int octaves, double prop, Random rand, bool wrapAround = false)
        {
            if (pitches != null)
                for (int i = 0; i < pitches.Length; i++)
                    pitches.collection[i] = pitches.collection[i].OctaveBelow(octaves, rand, prop, wrapAround);
            return pitches;
        }

        public static Enumerate<Pitch> OctaveAboveOrBelow(this Enumerate<Pitch> pitches, int octaves, double prop, Random rand, bool wrapAround = false)
        {
            if (pitches?.Any() == false || octaves < 1)
                return pitches;

            return (rand.NextDouble() < 0.5)
                ? pitches?.OctaveAbove(octaves, prop, rand, wrapAround)
                : pitches?.OctaveBelow(octaves, prop, rand, wrapAround);
        }

        /// <returns>Scales by amount</returns>
        public static Enumerate<float> Scale(this Enumerate<float> input, double amount)
        {
            if (input?.Any() == false || amount == 1)
                return input;
            var coll = input.Select(p => (float)Math.Round(p * amount));
            return new Enumerate<float>(coll, input.Incrementor);
        }

        /// <returns>Scales by amount</returns>
        public static Enumerate<int> Scale(this Enumerate<int> input, double amount)
        {
            if (input?.Any() == false || amount == 1)
                return input;
            var coll = input.Select(p => (int)Math.Round(p * amount));
            return new Enumerate<int>(coll, input.Incrementor);
        }

        public static List<Pitch> OctaveAbove(this IEnumerable<Pitch> pitches, bool wrapAround = false)
        {
            var list = new List<Pitch>();
            pitches?.ToList().ForEach(p => list.Add(p.OctaveAbove(wrapAround)));
            return list;
        }

        public static List<Pitch> OctaveBelow(this IEnumerable<Pitch> pitches, bool wrapAround = false)
        {
            var list = new List<Pitch>();
            pitches?.ToList().ForEach(p => list.Add(p.OctaveBelow(wrapAround)));
            return list;
        }

        public static List<Pitch> PitchesAbove(this IEnumerable<Pitch> pitches, int semiTones, bool wrapAround = false)
        {
            var list = new List<Pitch>();
            if (semiTones > 0)
                pitches?.ToList().ForEach(p => list.Add(p.PitchAbove(semiTones, wrapAround)));
            return list;
        }
    }
}
