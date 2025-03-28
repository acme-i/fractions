using System;
using System.Collections.Generic;
using System.Linq;

namespace fractions
{
    [Flags]
    public enum NoteProperty
    {
        Time = 1,
        Velocity = 2,
        Pan = 4,
        Duration = 8,
        Reverb = 16,
    }

    public static class NoteInterpolator<T>
        where T : NoteMessage
    {
        /// <summary>
        /// Returns sequence generator from start to end over duration using the given easing function.
        /// The sequence is generated as it is accessed using the Time.deltaTime to calculate the portion
        /// of duration that has elapsed.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="duration"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<T> NewEase(EaseFunction ease, T start, T end, double duration, NoteProperty property = NoteProperty.Time)
        {
            IEnumerable<double> timer = NewTimer(duration, end.Time - (start.Time / 16));
            return NewEase(ease, start, end, duration, timer, property);
        }

        static IEnumerable<double> NewTimer(double duration, double deltaTime)
        {
            double elapsedTime = 0.0f;
            while (elapsedTime < duration)
            {
                yield return elapsedTime;
                elapsedTime += deltaTime;
                // make sure last value is never skipped
                if (elapsedTime >= duration)
                {
                    yield return elapsedTime;
                }
            }
        }

        /// <summary>
        /// Generates sequence of integers from start to end (inclusive) one step at a time.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        static IEnumerable<double> NewCounter(int start, int end, int step)
        {
            for (int i = start; i <= end; i += step)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Instead of easing based on time, generate n interpolated points (slices) between the start and end positions.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="slices"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<T> NewEase(EaseFunction ease, T start, T end, int slices, NoteProperty property = NoteProperty.Time)
        {
            IEnumerable<double> counter = NewCounter(0, slices + 1, 1);
            return NewEase(ease, start, end, slices + 1, counter, property);
        }

        /// <summary>
        /// Generic easing sequence generator used to implement the time and slice variants.
        /// Normally you would not use this function directly.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="total"></param>
        /// <param name="driver"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        static IEnumerable<T> NewEase(EaseFunction ease, T start, T end, double total, IEnumerable<double> driver, NoteProperty property = NoteProperty.Time)
        {
            T distance = (T)(end - start);
            foreach (double i in driver)
            {
                yield return Ease(ease, start, distance, i, total, property);
            }
        }

        /// <summary>
        /// T interpolation using given easing method.
        /// Easing is done independently on all three vector axis.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="start"></param>
        /// <param name="distance"></param>
        /// <param name="elapsedTime"></param>
        /// <param name="duration"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        static T Ease(EaseFunction ease, T start, T distance, double elapsedTime, double duration, NoteProperty property = NoteProperty.Time)
        {
            var target = start.Clone() as T;

            if ((NoteProperty.Duration & property) == NoteProperty.Duration && typeof(T) == typeof(NoteOnOffMessage))
                (target as NoteOnOffMessage).Duration = (float)ease((start as NoteOnOffMessage).Duration, (distance as NoteOnOffMessage).Duration, elapsedTime, duration);

            if ((NoteProperty.Time & property) == NoteProperty.Time)
                target.Time = (float)ease(start.Time, distance.Time, elapsedTime, duration);

            if ((NoteProperty.Pan & property) == NoteProperty.Pan)
                target.Pan = (float)ease(start.Pan, distance.Pan, elapsedTime, duration);

            if ((NoteProperty.Velocity & property) == NoteProperty.Velocity)
                target.Velocity = (float)ease(start.Velocity, distance.Velocity, elapsedTime, duration);

            if ((NoteProperty.Reverb & property) == NoteProperty.Reverb)
                if (start.Reverb is double startReverb && distance.Reverb is double endReverb)
                    target.Reverb = (float)ease(startReverb, endReverb, elapsedTime, duration);

            return target;
        }

        /// <summary>
        /// A T[] variation of the Transform[] NewBezier() function.
        /// Same functionality but using Vector3s to define bezier curve.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="points"></param>
        /// <param name="duration"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static IEnumerable<T> NewBezier(EaseFunction ease, T[] points, double duration, double deltaTime = 1 / 16.0)
        {
            IEnumerable<double> timer = NewTimer(duration, deltaTime);
            return NewBezier(ease, points, duration, timer);
        }

        /// <summary>
        /// A T[] variation of the Transform[] NewBezier() function.
        /// Same functionality but using Vector3s to define bezier curve.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="points"></param>
        /// <param name="slices"></param>
        /// <returns></returns>
        public static IEnumerable<T> NewBezier(EaseFunction ease, T[] points, int slices)
        {
            IEnumerable<double> counter = NewCounter(0, slices + 1, 1);
            return NewBezier(ease, points, slices + 1, counter);
        }

        /// <summary>
        /// Generic bezier spline sequence generator used to implement the time and slice variants. Normally you would not use this function directly.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="nodes"></param>
        /// <param name="maxStep"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        static IEnumerable<T> NewBezier(EaseFunction ease, IList<T> nodes, double maxStep, IEnumerable<double> steps)
        {
            // need at least two nodes to spline between
            if (nodes.Count >= 2)
            {
                // copy nodes array since Bezier is destructive
                T[] points = new T[nodes.Count];
                foreach (double step in steps)
                {
                    // re-initialize copy before each destructive call to Bezier
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        points[i] = (T)((ICloneable)nodes[i]).Clone();
                    }
                    yield return Bezier(ease, points, step, maxStep);
                    // make sure last value is always generated
                }
            }
        }

        /// <summary>
        /// A T n-degree bezier spline.
        /// 
        /// WARNING: The points array is modified by Bezier.See NewBezier() for a
        /// safe and user friendly alternative.
        /// 
        /// You can pass zero control points, just the start and end points, for just
        /// plain easing.In other words a zero-degree bezier spline curve is just the
        /// easing method.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="points"></param>
        /// <param name="elapsedTime"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        static T Bezier(EaseFunction ease, T[] points, double elapsedTime, double duration)
        {
            // Reference: http://ibiblio.org/e-notes/Splines/Bezier.htm
            // Interpolate the n starting points to generate the next j = (n - 1) points,
            // then interpolate those n - 1 points to generate the next n - 2 points,
            // continue this until we have generated the last point (n - (n - 1)), j = 1.
            // We store the next set of output points in the same array as the
            // input points used to generate them. This works because we store the
            // result in the slot of the input point that is no longer used for this
            // iteration.
            for (int j = points.Length - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    points[i].Velocity = ease(points[i].Velocity, points[i + 1].Velocity - points[i].Velocity, elapsedTime, duration);
                }
            }
            return points[0];
        }

        /// <summary>
        /// A NoteOnMessage[] variation of the Transform[] NewCatmullRom() function.
        /// Same functionality but using Vector3s to define curve.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="slices"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static IEnumerable<T> NewCatmullRom(T[] points, int slices, bool loop)
        {
            return NewCatmullRom(points.ToList(), slices, loop);
        }

        /// <summary>
        /// Generic catmull-rom spline sequence generator used to implement the T[] and Transform[] variants.
        /// Normally you would not use this function directly.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="slices"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        static IEnumerable<T> NewCatmullRom(IList<T> nodes, int slices, bool loop)
        {
            // need at least two nodes to spline between
            if (nodes.Count >= 2)
            {
                // yield the first point explicitly, if looping the first point
                // will be generated again in the step for loop when interpolating
                // from last point back to the first point
                yield return nodes[0];

                int last = nodes.Count - 1;
                for (int current = 0; loop || current < last; current++)
                {
                    // wrap around when looping
                    if (loop && current > last)
                    {
                        current = 0;
                    }
                    // handle edge cases for looping and non-looping scenarios
                    // when looping we wrap around, when not looping use start for previous
                    // and end for next when you at the ends of the nodes array
                    int previous = (current == 0) ? (loop ? last : current) : current - 1;
                    int start = current;
                    int end = (current == last) ? (loop ? 0 : current) : current + 1;
                    int next = (end == last) ? (loop ? 0 : end) : end + 1;

                    // adding one guarantees yielding at least the end point
                    int stepCount = slices + 1;
                    for (int step = 1; step <= stepCount; step++)
                    {
                        yield return CatmullRom(nodes[previous], nodes[start], nodes[end], nodes[next], step, stepCount);
                    }
                }
            }
        }

        /**
         * A T Catmull-Rom spline. Catmull-Rom splines are similar to bezier
         * splines but have the useful property that the generated curve will go
         * through each of the control points.
         *
         * NOTE: The NewCatmullRom() functions are an easier to use alternative to this
         * raw Catmull-Rom implementation.
         *
         * @param previous the point just before the start point or the start point
         *                 itself if no previous point is available
         * @param start generated when elapsedTime == 0
         * @param end generated when elapsedTime >= duration
         * @param next the point just after the end point or the end point itself if no
         *             next point is available
         */
        static T CatmullRom(T previous, T start, T end, T next, double elapsedTime, double duration)
        {
            // References used:
            // p.266 GemsV1
            //
            // tension is often set to 0.5 but you can use any reasonable value:
            // http://www.cs.cmu.edu/~462/projects/assn2/assn2/catmullRom.pdf
            //
            // bias and tension controls:
            // http://local.wasp.uwa.edu.au/~pbourke/miscellaneous/interpolation/

            double percentComplete = elapsedTime / duration;
            double percentCompleteSquared = percentComplete * percentComplete;
            double percentCompleteCubed = percentCompleteSquared * percentComplete;

            return (T)
                (
                    (previous * ((-0.5f * percentCompleteCubed) +
                                          percentCompleteSquared -
                                 (0.5f * percentComplete))) +
                    (start * ((1.5f * percentCompleteCubed) +
                              (-2.5f * percentCompleteSquared) + 1.0f)) +
                    (end * ((-1.5f * percentCompleteCubed) +
                            (2.0f * percentCompleteSquared) +
                            (0.5f * percentComplete))) +
                    (next * ((0.5f * percentCompleteCubed) -
                             (0.5f * percentCompleteSquared)))
                );
        }

    }
}
