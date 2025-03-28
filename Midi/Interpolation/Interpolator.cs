using System;
using System.Collections.Generic;
using System.Linq;

/**
* Interpolation utility functions: easing, bezier, and catmull-rom.
* Consider using Unity's Animation curve editor and AnimationCurve class
* before scripting the desired behaviour using this utility.
*
* Interpolation functionality available at different levels of abstraction.
* Low level access via individual easing functions (ex. EaseInOutCirc),
* Bezier(), and CatmullRom(). High level access using sequence generators,
* NewEase(), NewBezier(), and NewCatmullRom().
*
* Sequence generators are typically used as follows:
*
* IEnumerable&lt;NoteOnMessage&gt; sequence = Interpolate.New[Ease|Bezier|CatmulRom](configuration);
* foreach (NoteOnMessage newPoint in sequence) {
*   transform.position = newPoint;
*   yield return WaitForSeconds(1.0f);
* }
*
* Or:
*
* IEnumerator&lt;NoteOnMessage&gt; sequence = Interpolate.New[Ease|Bezier|CatmulRom](configuration).GetEnumerator();
* function Update() {
*   if (sequence.MoveNext()) {
*     transform.position = sequence.Current;
*   }
* }
*
* The low level functions work similarly to Unity's built in Lerp and it is
* up to you to track and pass in elapsedTime and duration on every call. The
* functions take this form (or the logical equivalent for Bezier() and CatmullRom()).
*
* transform.position = ease(start, distance, elapsedTime, duration);
*
* For convenience in configuration you can use the Ease(EaseType) function to
* look up a concrete easing function:
* 
*  [SerializeField]
*  Interpolate.EaseType easeType; // set using Unity's property inspector
*  Interpolate.Function ease; // easing of a particular EaseType
* function Awake() {
*   ease = Interpolate.Ease(easeType);
* }
*
* @author Fernando Zapata (fernando@cpudreams.com)
* @Traduzione Andrea85cs (andrea85cs@dynematica.it)
*/

namespace fractions
{
    public delegate double EaseFunction(double start, double distance, double elapsedTime, double duration);

    /**
     * Different methods of easing interpolation.
     */
    public enum EaseType
    {
        Linear,
        EaseInCirc,
        EaseOutCirc,
        EaseInCubic,
        EaseOutCubic,
        EaseInExpo,
        EaseOutExpo,
        EaseInQuad,
        EaseOutQuad,
        EaseInQuart,
        EaseOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutCirc,
        EaseInOutCubic,
        EaseInOutExpo,
        EaseInOutQuad,
        EaseInOutQuart,
        EaseInOutQuint,
        EaseInOutSine,
    }

    public static class Interpolator
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

        public static EaseFunction RandomEase()
        {
            return Ease((EaseType)Rand.Next(22));
        }

        public static EaseFunction RandomEaseInFunction()
        {
            return easeInFunctions[Rand.Next(easeInFunctions.Count)];
        }

        public static EaseFunction RandomEaseOutFunction()
        {
            return easeOutFunctions[Rand.Next(easeOutFunctions.Count)];
        }

        private readonly static List<(EaseFunction easeIn, EaseFunction easeOut)> easeFunctions = new List<(EaseFunction easeIn, EaseFunction easeOut)>
        {
            (Ease(EaseType.EaseInCirc),  Ease(EaseType.EaseOutCirc)),
            (Ease(EaseType.EaseInCubic), Ease(EaseType.EaseOutCubic)),
            (Ease(EaseType.EaseInExpo),  Ease(EaseType.EaseOutExpo)),
            (Ease(EaseType.EaseInQuad),  Ease(EaseType.EaseOutQuad)),
            (Ease(EaseType.EaseInQuart), Ease(EaseType.EaseOutQuart)),
            (Ease(EaseType.EaseInQuint), Ease(EaseType.EaseOutQuint)),
            (Ease(EaseType.EaseInSine),  Ease(EaseType.EaseOutSine)),
        };
        public static List<(EaseFunction easeIn, EaseFunction easeOut)> EaseFunctions() => easeFunctions.ToList();

        private readonly static List<EaseFunction> easeInFunctions = new List<EaseFunction>
        {
            Ease(EaseType.EaseInCirc),
            Ease(EaseType.EaseInCubic),
            Ease(EaseType.EaseInExpo),
            Ease(EaseType.EaseInQuad),
            Ease(EaseType.EaseInQuart),
            Ease(EaseType.EaseInQuint),
            Ease(EaseType.EaseInSine),
        };
        public static List<EaseFunction> EaseInFunctions() => easeInFunctions.ToList();

        private readonly static List<EaseFunction> easeOutFunctions = new List<EaseFunction>
        {
            Ease(EaseType.EaseOutCirc),
            Ease(EaseType.EaseOutCubic),
            Ease(EaseType.EaseOutExpo),
            Ease(EaseType.EaseOutQuad),
            Ease(EaseType.EaseOutQuart),
            Ease(EaseType.EaseOutQuint),
            Ease(EaseType.EaseOutSine),
        };
        public static List<EaseFunction> EaseOutFunctions() => easeOutFunctions.ToList();

        /// <summary>
        /// Returns the static method that implements the given easing type for scalars.
        /// Use this method to easily switch between easing interpolation types.
        /// 
        /// All easing methods clamp elapsedTime so that it is always &lt;= duration.
        /// 
        /// var ease = Interpolate.Ease(EaseType.EaseInQuad);
        /// i = ease(start, distance, elapsedTime, duration);
        /// </summary>
        public static EaseFunction Ease(EaseType type)
        {
            // Source Flash easing functions:
            // http://gizma.com/easing/
            // http://www.robertpenner.com/easing/easing_demo.html
            //
            // Changed to use more friendly variable names, that follow my Lerp
            // conventions:
            // start = b (start value)
            // distance = c (change in value)
            // elapsedTime = t (current time)
            // duration = d (time duration)

            EaseFunction f = null;
            switch (type)
            {
                case EaseType.Linear:
                    f = Linear;
                    break;
                case EaseType.EaseInQuad:
                    f = EaseInQuad;
                    break;
                case EaseType.EaseOutQuad:
                    f = EaseOutQuad;
                    break;
                case EaseType.EaseInOutQuad:
                    f = EaseInOutQuad;
                    break;
                case EaseType.EaseInCubic:
                    f = EaseInCubic;
                    break;
                case EaseType.EaseOutCubic:
                    f = EaseOutCubic;
                    break;
                case EaseType.EaseInOutCubic:
                    f = EaseInOutCubic;
                    break;
                case EaseType.EaseInQuart:
                    f = EaseInQuart;
                    break;
                case EaseType.EaseOutQuart:
                    f = EaseOutQuart;
                    break;
                case EaseType.EaseInOutQuart:
                    f = EaseInOutQuart;
                    break;
                case EaseType.EaseInQuint:
                    f = EaseInQuint;
                    break;
                case EaseType.EaseOutQuint:
                    f = EaseOutQuint;
                    break;
                case EaseType.EaseInOutQuint:
                    f = EaseInOutQuint;
                    break;
                case EaseType.EaseInSine:
                    f = EaseInSine;
                    break;
                case EaseType.EaseOutSine:
                    f = EaseOutSine;
                    break;
                case EaseType.EaseInOutSine:
                    f = EaseInOutSine;
                    break;
                case EaseType.EaseInExpo:
                    f = EaseInExpo;
                    break;
                case EaseType.EaseOutExpo:
                    f = EaseOutExpo;
                    break;
                case EaseType.EaseInOutExpo:
                    f = EaseInOutExpo;
                    break;
                case EaseType.EaseInCirc:
                    f = EaseInCirc;
                    break;
                case EaseType.EaseOutCirc:
                    f = EaseOutCirc;
                    break;
                case EaseType.EaseInOutCirc:
                    f = EaseInOutCirc;
                    break;
            }
            return f;
        }

        public static List<double> NewEaseInOut(EaseType easeIn, EaseType easeOut, double start, double end, double duration)
        {
            return NewEaseInOut(easeIn, easeOut, start, end, duration, start, end, duration);
        }

        public static List<double> NewEaseInOut(EaseType easeIn, EaseType easeOut, double inStart, double inEnd, double inDuration, double outStart, double outEnd, double outDuration)
        {
            var values = new List<double>();

            values.AddRange(NewEase(
                Ease(easeIn),
                inStart,
                inEnd,
                inDuration
            ).Skip(1).Take((int)inDuration));

            values.AddRange(NewEase(
                Ease(easeOut),
                outStart,
                outEnd,
                outDuration
            ).Skip(1).Take((int)outDuration).Reverse());

            return values;
        }

        /// <summary>
        /// Returns sequence generator from start to end over duration using the given easing function.
        /// The sequence is generated as it is accessed using the Time.deltaTime to calculate the portion
        /// of duration that has elapsed.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static IEnumerable<double> NewEase(EaseFunction ease, double start, double end, double duration)
        {
            IEnumerable<double> timer = NewTimer(duration, end - start);
            return NewEase(ease, start, end, duration, timer);
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
        /// <returns></returns>
        static IEnumerable<double> NewEase(EaseFunction ease, double start, double end, double total, IEnumerable<double> driver)
        {
            double distance = (end - start);
            foreach (double i in driver)
            {
                yield return ease(start, distance, i, total);
            }
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
        /// interpolate between start and end in n number of steps using the method m
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="n">number of steps</param>
        /// <param name="m">method. 0 = cosine, 1 = linear, 2 = square, etc</param>
        /// <returns>the progression of start towards end in n number of steps</returns>
        public static List<float> Interpolate(float start, float end, int n, int m = 1)
        {
            if (Math.Sign(n) <= 0) throw new ArgumentOutOfRangeException(nameof(n));

            var isReversed = start > end;
            if (isReversed)
            {
                (end, start) = (start, end);
            }

            var values = new List<float>(n);
            var step = (end - start) / n;
            for (double i = 1; i <= n; i++)
                values.Add((float)Interpolate(step * i, start, end, start, end, m));

            if (isReversed) values.Reverse();

            return values;
        }

        /// <summary>
        /// interpolate between start and end in n number of steps using the method m
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="n">number of steps</param>
        /// <param name="m">method. 0 = cosine, 1 = linear, 2 = square, etc</param>
        /// <returns>the progression of start towards end in n number of steps</returns>
        public static List<double> Interpolate(double start, double end, int n, int m = 1)
        {
            if (Math.Sign(n) <= 0) throw new ArgumentOutOfRangeException(nameof(n));

            var isReversed = start > end;
            if (isReversed)
            {
                (end, start) = (start, end);
            }

            var values = new List<double>(n);
            var step = (end - start) / n;
            for (double i = 1; i <= n; i++)
                values.Add(Interpolate(step * i, start, end, start, end, m));

            if (isReversed) values.Reverse();

            return values;
        }

        /// <summary>
        /// Interpolation
        /// </summary>
        /// <param name="gc">Global current</param>
        /// <param name="gs">Global start</param>
        /// <param name="ge">Global end</param>
        /// <param name="ts">Target start</param>
        /// <param name="te">Target end</param>
        /// <param name="method">interpolation method
        /// Method = 0 - cosine interpolation
        /// Method &gt; 0 - exponential (1 - linear, etc)
        /// </param>
        /// <returns></returns>
        public static double Interpolate(double gc, double gs, double ge, double ts, double te, int method = 1)
        {
            double result;
            if (method == 0)
            {
                double x = (gc - gs) / (ge - gs);
                double f = (1.0 - Math.Cos(x * Math.PI)) * 0.5;
                result = (ts * (1.0 - f)) + (te * f);
            }
            else
            {
                result = ts + ((te - ts) * Math.Pow((gc - gs) / (ge - gs), method));
            }
            return result;
        }

        public static List<double> InOutCurve(double start, double end, int inSteps, int outSteps, EaseFunction easeIn, EaseFunction easeOut)
        {
            var samples = new List<double>();

            samples.AddRange(NewEase(
                easeIn,
                start,
                end,
                inSteps
            ).Skip(1).Take(inSteps));

            samples.AddRange(NewEase(
                easeOut,
                start,
                end,
                outSteps
            ).Reverse().Skip(1).Take(outSteps));

            return samples;
        }

        public static List<double> InOutCurve(double startIn, double endIn, double startOut, double endOut, int inSteps, int outSteps, EaseFunction easeIn, EaseFunction easeOut)
        {
            var samples = new List<double>();

            samples.AddRange(NewEase(
                easeIn,
                startIn,
                endIn,
                inSteps
            ).Skip(1).Take(inSteps)); ;

            samples.AddRange(NewEase(
                easeOut,
                startOut,
                endOut,
                outSteps
            ).Reverse().Skip(1).Take(outSteps));

            return samples;
        }

        /**
         * Linear interpolation (same as Math.Lerp)
         */
        public static double Linear(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime to be <= duration
            if (elapsedTime > duration)
            {
                elapsedTime = duration;
            }
            return (distance * (elapsedTime / duration)) + start;
        }

        /**
         * quadratic easing in - accelerating from zero velocity
         */
        public static double EaseInQuad(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
            return (distance * elapsedTime * elapsedTime) + start;
        }

        /**
         * quadratic easing out - decelerating to zero velocity
         */
        public static double EaseOutQuad(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 1.0f
                : elapsedTime / duration;
            return (-distance * elapsedTime * (elapsedTime - 2)) + start;
        }

        /**
         * quadratic easing in/out - acceleration until halfway, then deceleration
         */
        public static double EaseInOutQuad(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 2.0f
                : elapsedTime / (duration / 2);
            if (elapsedTime < 1)
                return (distance / 2 * elapsedTime * elapsedTime) + start;
            elapsedTime--;
            return (-distance / 2 * ((elapsedTime * (elapsedTime - 2)) - 1)) + start;
        }

        /**
         * cubic easing in - accelerating from zero velocity
         */
        public static double EaseInCubic(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 1.0f
                : elapsedTime / duration;
            return (distance * elapsedTime * elapsedTime * elapsedTime) + start;
        }

        /**
         * cubic easing out - decelerating to zero velocity
         */
        public static double EaseOutCubic(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 1.0f
                : elapsedTime / duration;
            elapsedTime--;
            return (distance * ((elapsedTime * elapsedTime * elapsedTime) + 1)) + start;
        }

        /**
         * cubic easing in/out - acceleration until halfway, then deceleration
         */
        public static double EaseInOutCubic(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 2.0f
                : elapsedTime / (duration / 2);
            if (elapsedTime < 1)
                return (distance / 2 * elapsedTime * elapsedTime * elapsedTime) + start;
            elapsedTime -= 2;
            return (distance / 2 * ((elapsedTime * elapsedTime * elapsedTime) + 2)) + start;
        }

        /**
         * quartic easing in - accelerating from zero velocity
         */
        public static double EaseInQuart(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 1.0f
                : elapsedTime / duration;
            return (distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime) + start;
        }

        /**
         * quartic easing out - decelerating to zero velocity
         */
        public static double EaseOutQuart(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 1.0f
                : elapsedTime / duration;
            elapsedTime--;
            return (-distance * ((elapsedTime * elapsedTime * elapsedTime * elapsedTime) - 1)) + start;
        }

        /**
         * quartic easing in/out - acceleration until halfway, then deceleration
         */
        public static double EaseInOutQuart(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 2.0f
                : elapsedTime / (duration / 2);
            if (elapsedTime < 1)
                return (distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime) + start;
            elapsedTime -= 2;
            return (-distance / 2 * ((elapsedTime * elapsedTime * elapsedTime * elapsedTime) - 2)) + start;
        }


        /**
         * quintic easing in - accelerating from zero velocity
         */
        static double EaseInQuint(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 1.0f
                : elapsedTime / duration;
            return (distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime) + start;
        }

        /**
         * quintic easing out - decelerating to zero velocity
         */
        public static double EaseOutQuint(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 1.0f
                : elapsedTime / duration;
            elapsedTime--;
            return (distance * ((elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime) + 1)) + start;
        }

        /**
         * quintic easing in/out - acceleration until halfway, then deceleration
         */
        public static double EaseInOutQuint(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 2.0f
                : elapsedTime / (duration / 2f);
            if (elapsedTime < 1)
                return (distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime) + start;
            elapsedTime -= 2;
            return (distance / 2 * ((elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime) + 2)) + start;
        }

        /**
         * sinusoidal easing in - accelerating from zero velocity
         */
        public static double EaseInSine(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime to be <= duration
            if (elapsedTime > duration)
            {
                elapsedTime = duration;
            }
            return (-distance * Math.Cos(elapsedTime / duration * (Math.PI / 2))) + distance + start;
        }

        /**
         * sinusoidal easing out - decelerating to zero velocity
         */
        public static double EaseOutSine(double start, double distance, double elapsedTime, double duration)
        {
            if (elapsedTime > duration)
            {
                elapsedTime = duration;
            }
            return (distance * Math.Sin(elapsedTime / duration * (Math.PI / 2))) + start;
        }

        /**
         * sinusoidal easing in/out - accelerating until halfway, then decelerating
         */
        public static double EaseInOutSine(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime to be <= duration
            if (elapsedTime > duration)
            {
                elapsedTime = duration;
            }
            return (-distance / 2 * (Math.Cos(Math.PI * elapsedTime / duration) - 1)) + start;
        }

        /**
         * exponential easing in - accelerating from zero velocity
         */
        public static double EaseInExpo(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime to be <= duration
            if (elapsedTime > duration)
            {
                elapsedTime = duration;
            }
            return (distance * Math.Pow(2, 10 * ((elapsedTime / duration) - 1))) + start;
        }

        /**
         * exponential easing out - decelerating to zero velocity
         */
        public static double EaseOutExpo(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime to be <= duration
            if (elapsedTime > duration)
            {
                elapsedTime = duration;
            }
            return (distance * (-Math.Pow(2, -10 * elapsedTime / duration) + 1)) + start;
        }

        /**
         * exponential easing in/out - accelerating until halfway, then decelerating
         */
        public static double EaseInOutExpo(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 2.0f
                : elapsedTime / (duration / 2);
            if (elapsedTime < 1)
                return (distance / 2 * Math.Pow(2, 10 * (elapsedTime - 1))) + start;
            elapsedTime--;
            return (distance / 2 * (-Math.Pow(2, -10 * elapsedTime) + 2)) + start;
        }

        /**
         * circular easing in - accelerating from zero velocity
         */
        public static double EaseInCirc(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 1.0f
                : elapsedTime / duration;
            return (-distance * (Math.Sqrt(1 - (elapsedTime * elapsedTime)) - 1)) + start;
        }

        /**
         * circular easing out - decelerating to zero velocity
         */
        public static double EaseOutCirc(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 1.0f
                : elapsedTime / duration;
            elapsedTime--;
            return (distance * Math.Sqrt(1 - (elapsedTime * elapsedTime))) + start;
        }

        /**
         * circular easing in/out - acceleration until halfway, then deceleration
         */
        public static double EaseInOutCirc(double start, double distance, double elapsedTime, double duration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration)
                ? 2.0f
                : elapsedTime / (duration / 2);
            if (elapsedTime < 1)
                return (-distance / 2 * (Math.Sqrt(1 - (elapsedTime * elapsedTime)) - 1)) + start;
            elapsedTime -= 2;
            return (distance / 2 * (Math.Sqrt(1 - (elapsedTime * elapsedTime)) + 1)) + start;
        }
    }
}