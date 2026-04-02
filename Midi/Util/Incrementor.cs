using System;


namespace fractions
{

    /// <summary>
    /// A class that oscilates between two values
    /// </summary>
    public class Incrementor : ICloneable
    {
        /// <summary>
        /// Used when cloning an Incrementor instance
        /// </summary>
        /// <param name="inc"><see cref="IncrementMethod"/></param>
        public Incrementor(Incrementor inc)
            : this(inc.Index, inc.Min, inc.Max, inc.StepSize, inc.Method)
        {
            this.Index = inc.Index;
            this.Increasing = inc.Increasing;
        }

        /// <summary>
        /// Clone object using new stepSize value
        /// </summary>
        /// <param name="inc">object to clone</param>
        /// <param name="step">new stepSize value</param>
        public Incrementor(Incrementor inc, double step)
            : this(inc.Index, inc.Min, inc.Max, step, inc.Method)
        {
            this.Increasing = inc.Increasing;
        }

        /// <summary>
        /// Clone object using a new stepSize value and a new method
        /// </summary>
        /// <param name="inc">object to clone</param>
        /// <param name="step">new stepSize value</param>
        /// <param name="met"><see cref="IncrementMethod"/></param>
        public Incrementor(Incrementor inc, double step, IncrementMethod met = IncrementMethod.Cyclic)
            : this(inc.Index, inc.Min, inc.Max, step, met)
        {
            this.Index = inc.Index;
            this.Increasing = inc.Increasing;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="min">min value</param>
        /// <param name="max">max value</param>
        /// <param name="step">stepSize value</param>
        /// <param name="method"><see cref="IncrementMethod"/></param>
        public Incrementor(double min, double max, double step, IncrementMethod method = IncrementMethod.Cyclic)
            : this(min, min, max, step, method)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The current value. Will automatically be clamped between min and max</param>
        /// <param name="min">min value</param>
        /// <param name="max">max value</param>
        /// <param name="step">stepSize value</param>
        /// <param name="method"><see cref="IncrementMethod"/></param>
        /// <param name="autoCorrect">auto correct min max, whenever min > max</param>
        public Incrementor(double value, double min, double max, double step, IncrementMethod method, bool autoCorrect = true)
        {
            if (double.IsNaN(value)) value = min;

            ArgumentExceptionExtensions.ThrowIfTrue(double.IsNaN(min), "min is NaN", nameof(min));
            ArgumentExceptionExtensions.ThrowIfTrue(double.IsNaN(max), "max is NaN", nameof(max));
            ArgumentExceptionExtensions.ThrowIfTrue(double.IsNaN(step), "stepSize is NaN", nameof(step));
            ArgumentExceptionExtensions.ThrowIfTrue(Equals(min, max), "min cannot be equal to max", nameof(min));

            ValidateState(ref value, ref min, ref max, ref step, autoCorrect);

            Min = min;
            Max = max;
            StepSize = step;
            Index = value;
            Method = method;

            var originalValue = value;
            switch (Method)
            {
                case IncrementMethod.MaxMin:
                    Index = Max;
                    Increasing = false;
                    break;
                default:
                    Index = Min;
                    Increasing = true;
                    break;
            }

            if (value > max)
                originalValue = max;

            if (value < min)
                originalValue = min;

            if (originalValue != value)
            {
                Index = originalValue;
            }
        }

        private void ValidateState(ref double value, ref double min, ref double max, ref double stepSize, bool autoCorrect)
        {
            if (Math.Sign(stepSize) < 0)
            {
                if (autoCorrect)
                    stepSize = Math.Abs(stepSize);
                else
                    throw new ArgumentException("stepSize cannot be < zero", nameof(stepSize));
            }
            if (Math.Sign(stepSize) == 0)
            {
                if (autoCorrect)
                    stepSize = 1;
                else
                    throw new ArgumentException("stepSize cannot be zero", nameof(stepSize));
            }
            if (-1 == Math.Sign(min))
            {
                if (autoCorrect)
                    min = Math.Abs(min);
                else
                    throw new ArgumentException(message: "min cannot be less than zero", nameof(min));
            }
            if (-1 == Math.Sign(max))
            {
                if (autoCorrect)
                    max = Math.Abs(max);
                else
                    throw new ArgumentException(message: "max cannot be less than zero", nameof(max));
            }
            if (min > max)
            {
                if (autoCorrect)
                    (max, min) = (min, max);
                else
                    throw new ArgumentException("max cannot be less than min", nameof(max));
            }
            if (-1 == Math.Sign(value))
            {
                if (autoCorrect)
                    value = Math.Abs(value);
                else
                    throw new ArgumentException("value cannot be less than zero", nameof(value));
            }
            if (stepSize > (max - min))
            {
                if (autoCorrect)
                    stepSize = (max - min) / 2.0;
                else
                    throw new ArgumentException("StepSize size cannot be greater than max-min", nameof(stepSize));
            }
        }

        /// <summary>
        /// Min Index
        /// </summary>
        public double Min { get; internal set; }

        /// <summary>
        /// Max value
        /// </summary>
        public double Max { get; internal set; }
        
        /// <summary>
        /// The current value between min and max
        /// </summary>
        public double Index { get; internal set; }

        /// <summary>
        /// The previous value
        /// </summary>
        public double? PreviousIndex { get; internal set; } = null;

        /// <summary>
        /// StepSize value to move towards min or max when calling GetNext()
        /// </summary>
        public double StepSize { get; internal set; }

        public bool Increasing { get; internal set; } = true;

        /// <summary>
        /// The current value between min and max
        /// </summary>
        public void Reset() { Index = Min; PreviousIndex = null; }

        public IncrementMethod Method { get; set; }

        /// <summary>
        /// Updates the stepSize size. 
        /// </summary>
        /// <param name="newStepSize">The new stepSize size</param>
        /// <param name="wrapAround">Whether to take modulus of the stepSize size if the value is greater than Max.</param>
        /// <exception cref="ArgumentOutOfRangeException">newStepSize is zero or negative</exception>
        /// <exception cref="ArgumentOutOfRangeException">newStepSize is greater than Max and wrapAround is false</exception>
        public void SetStepSize(int newStepSize, bool wrapAround = true)
        {
            if (Math.Sign(newStepSize) <= 0)
                throw new ArgumentOutOfRangeException(nameof(newStepSize));

            if (newStepSize > Max)
            {
                if (wrapAround)
                    StepSize = newStepSize % Max;
                else
                    throw new ArgumentOutOfRangeException(nameof(newStepSize));
            }
            else
            {
                StepSize = newStepSize;
            }
        }

        /// <summary>
        /// GetNext value towards min or max
        /// </summary>
        /// <returns>The new value</returns>
        public double GetNext()
        {
            PreviousIndex = Index;

            switch (Method)
            {
                case IncrementMethod.Bit:
                    Index = (Index == Min) ? Max : Min;
                    break;

                case IncrementMethod.Cyclic:
                    if (Increasing)
                    {
                        if (Index + StepSize > Max)
                        {
                            Index = Max - ((Index + StepSize) % Max);
                            Increasing = !Increasing;
                        }
                        else
                        {
                            Index += StepSize;
                            if (Index == Max)
                                Increasing = !Increasing;
                        }
                    }
                    else
                    {
                        Index -= StepSize;
                        if (Index == Min)
                            Increasing = !Increasing;
                        else if (Index < Min)
                        {
                            Index = Min + Math.Abs(Index);
                            Increasing = !Increasing;
                        }
                    }

                    break;

                case IncrementMethod.MinMax:
                    if (Index + StepSize <= Max)
                    {
                        Index += StepSize;
                    }
                    else
                    {
                        var diff = Max - Min - Index;
                        Index = Min;
                        if (diff > 0)
                        {
                            var remain = Math.Abs(StepSize - diff);
                            if (Min == 0)
                            {
                                remain -= 1;
                            }
                            Index += remain;
                        }
                    }
                    break;

                case IncrementMethod.MaxMin:
                    if (Index - StepSize >= Min)
                    {
                        Index -= StepSize;
                    }
                    else
                    {
                        var remain = Index - Min;
                        Index = Max;
                        if (remain > 0)
                        {
                            Index -= remain;
                        }
                    }
                    break;

            }
            return Index;
        }

        /// <returns>Returns the next value GetNext() would return - without actually changing the Index</returns>
        public double Peek
        {
            get
            {
                var oldIncreasing = Increasing;
                var oldPreviousValue = PreviousIndex;
                var oldValue = Index;
                var peek = GetNext();
                Index = oldValue;
                PreviousIndex = oldPreviousValue;
                Increasing = oldIncreasing;
                return peek;
            }
        }

        /// <returns>Returns the next value GetNext() would return - without actually changing the Index</returns>
        public double PeekAt(int repeat)
        {
            if (Math.Sign(repeat) <= 0)
                throw new ArgumentException("must be positive", nameof(repeat));

            var oldIncreasing = Increasing;
            var oldPreviousValue = PreviousIndex;
            var oldValue = Index;
            var peek = GetNext();
            for (var i = 0; i < repeat - 1; i++)
            {
                peek = GetNext();
            }
            Index = oldValue;
            PreviousIndex = oldPreviousValue;
            Increasing = oldIncreasing;
            return peek;
        }

        public Incrementor Clone()
        {
            return new Incrementor(this);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
