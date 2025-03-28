using System;
using System.Collections.Generic;

namespace fractions
{
    public static class LinearInterpolator<T>
        where T : NoteMessage
    {
        private static int seed = 1976;

        private static Random random = null;
        private static Random Rand
        {
            get
            {
                return random ?? new Random(seed);
            }
        }

        public static int Seed
        {
            get { return seed; }
            set { seed = value; random = new Random(seed); }
        }

        public static List<T> Interpolate(T start, double endVelocity, int steps, ChordPattern pattern, double duration = 1, int velocityMethod = 1, int timeMethod = 1, int pitchMethod = 1)
        {
            return Interpolate(start, endVelocity, start.Pan, steps, pattern, duration, velocityMethod, timeMethod, panMethod: 1, pitchMethod);
        }

        public static List<T> Interpolate(T start, double endVelocity, double endPan, int steps, ChordPattern pattern, double duration = 1, int velocityMethod = 1, int timeMethod = 1, int panMethod = 1, int pitchMethod = 1)
        {
            var messages = new List<T>();
            var maxTime = start.Time + duration;
            var chord = new Chord(start.Pitch.NoteWithLetter(start.Pitch.ToString()[0]), pattern, pattern.Ascent.Length - 1);
            Pitch p;
            var rand = Rand.NextDouble();

            if ((int)start.Pitch > 24 && rand < 0.5)
            {
                p = start.Pitch - 12;
            }
            else if (start.Pitch < Pitch.G8 && rand < 0.25)
            {
                p = start.Pitch + 12;
            }
            else
            {
                p = start.Pitch;
            }

            var pitches = new List<Pitch>(steps);
            for (int x = 0; x < chord.NoteSequence.Length; ++x)
            {
                p = chord.NoteSequence[x].PitchAtOrAbove(p);
                pitches.Add(p);
            }

            for (int i = 1; i <= steps; i++)
            {
                var clone = (T)start.Clone();
                clone.Velocity = Interpolator.Interpolate(i, 1, steps, start.Velocity, endVelocity, velocityMethod);
                clone.Time = (float)Interpolator.Interpolate(i, 1, steps, start.Time, maxTime, timeMethod);
                clone.Pan = Math.Max(0, (int)Interpolator.Interpolate(i, 1, steps, start.Pan, endPan, panMethod));
                clone.Pitch = pitches[(int)Math.Max(0, Interpolator.Interpolate(i, 1, steps, 0, pitches.Count - 1, pitchMethod))];

                messages.Add(clone);
            }

            return messages;
        }

        public static List<T> Echoes(T start, double endVelocity, int numberOfEchoes, float maxEchoDuration = 1f, int decayMethod = 0b1, int timeMethod = 0b1)
        {
            var messages = new List<T>();
            var startVel = start.Velocity;
            var startTime = start.Time;
            var maxTime = start.Time + maxEchoDuration;
            for (int i = 1; i <= numberOfEchoes; i++)
            {
                var clone = (T)start.Clone();
                clone.Velocity = Interpolator.Interpolate(i, 1, numberOfEchoes, startVel, endVelocity, decayMethod);
                clone.Time = (float)Interpolator.Interpolate(i, 1, numberOfEchoes, startTime, maxTime, timeMethod);

                messages.Add(clone);
            }

            return messages;
        }

        public static List<T> Interpolate(T start, double endVelocity, int steps, float duration = 1f, int velocityMethod = 0b1, int timeMethod = 0b1)
        {
            var messages = new List<T>(steps);
            var startVel = start.Velocity;
            var startTime = start.Time;
            var maxTime = start.Time + duration;
            for (int i = 1; i <= steps; i++)
            {
                var clone = (T)start.Clone();
                clone.Velocity = Interpolator.Interpolate(i, 1, steps, startVel, endVelocity, velocityMethod);
                clone.Time = (float)Interpolator.Interpolate(i, 1, steps, startTime, maxTime, timeMethod);

                messages.Add(clone);
            }

            return messages;
        }

        public static List<T> Interpolate(T start, double endVelocity, double endPan, int steps, float duration = 1f, int velocityMethod = 0b1, int timeMethod = 0b1, int panMethod = 0b1)
        {
            var messages = new List<T>(steps);
            var startVel = start.Velocity;
            var startTime = start.Time;
            var startPan = start.Pan;
            var maxTime = start.Time + (steps * duration);
            for (int i = 1; i <= steps; i++)
            {
                var clone = (T)start.Clone();
                clone.Velocity = Interpolator.Interpolate(i + 0.002, 1.001, steps, startVel, endVelocity, velocityMethod);
                clone.Time = (float)Interpolator.Interpolate(i + 0.002, 1.001, steps, startTime, maxTime, timeMethod);
                clone.Pan = Interpolator.Interpolate(i + 0.002, 1.001, steps, startPan, endPan, panMethod);

                messages.Add(clone);
            }

            return messages;
        }

        public static List<T> Interpolate(Pitch[] pitches, int pitchStart, T start, double endVelocity, int steps, float duration = 1F, int velocityMethod = 1, int timeMethod = 1)
        {
            var messages = new List<T>(steps);
            var startVel = start.Velocity;
            var startTime = start.Time;
            var maxTime = start.Time + duration;
            var j = pitchStart;
            for (int i = 1; i <= steps; i++)
            {
                var clone = (T)start.Clone();
                clone.Velocity = Interpolator.Interpolate(i, 1, steps, startVel, endVelocity, velocityMethod);
                clone.Time = (float)Interpolator.Interpolate(i, 1, steps, startTime, maxTime, timeMethod);
                clone.Pitch = pitches[(j + (i - 1)) % pitches.Length];
                j += 1;

                messages.Add(clone);
            }

            return messages;
        }

    }
}
