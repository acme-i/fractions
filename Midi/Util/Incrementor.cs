using System;


namespace fractions
{

    /// <summary>
    /// A class that oscilates between two values
    /// </summary>
    public class Incrementor : ICloneable
    {
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="inc"><see cref="IncrementMethod"/></param>
        public Incrementor(Incrementor inc)
            : this(inc.Value, inc.Min, inc.Max, inc.Step, inc.Method)
        {
            this.Value = inc.Value;
            this.Increasing = inc.Increasing;
        }

        /// <summary>
        /// Clone object using new step value
        /// </summary>
        /// <param name="inc">object to clone</param>
        /// <param name="step">new step value</param>
        public Incrementor(Incrementor inc, double step)
            : this(inc.Value, inc.Min, inc.Max, step, inc.Method)
        {
            this.Increasing = inc.Increasing;
        }

        /// <summary>
        /// Clone object using a new step value and a new method
        /// </summary>
        /// <param name="inc">object to clone</param>
        /// <param name="step">new step value</param>
        /// <param name="met"><see cref="IncrementMethod"/></param>
        public Incrementor(Incrementor inc, double step, IncrementMethod met = IncrementMethod.Cyclic)
            : this(inc.Value, inc.Min, inc.Max, step, met)
        {
            this.Value = inc.Value;
            this.Increasing = inc.Increasing;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="min">min value</param>
        /// <param name="max">max value</param>
        /// <param name="step">step value</param>
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
        /// <param name="step">step value</param>
        /// <param name="method"><see cref="IncrementMethod"/></param>
        /// <param name="autoCorrect">auto correct min max, whenever min > max</param>
        public Incrementor(double value, double min, double max, double step, IncrementMethod method, bool autoCorrect = true)
        {
            if (double.IsNaN(value))
                value = min;
            if (double.IsNaN(min))
                throw new ArgumentException("min is NaN", nameof(min));
            if (double.IsNaN(max))
                throw new ArgumentException("max is NaN", nameof(max));
            if (double.IsNaN(step))
                throw new ArgumentException("step is NaN", nameof(step));

            if (Equals(min, max))
            {
                throw new ArgumentException("min cannot be equal to max", nameof(min));
            }

            if (Math.Sign(step) < 0)
            {
                if (autoCorrect)
                    step = Math.Abs(step);
                else
                    throw new ArgumentException("step cannot be < zero", nameof(step));
            }
            if (Math.Sign(step) == 0)
            {
                if (autoCorrect)
                    step = 1;
                else
                    throw new ArgumentException("step cannot be zero", nameof(step));
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
            if (step > (max - min))
            {
                if (autoCorrect)
                    step = (max - min) / 2.0;
                else
                    throw new ArgumentException("Step size cannot be greater than max-min", nameof(step));
            }


            Min = min;
            Max = max;
            Step = step;
            Value = value;
            Method = method;

            var originalValue = value;
            switch (Method)
            {
                case IncrementMethod.MaxMin:
                    Value = Max;
                    Increasing = false;
                    break;
                default:
                    Value = Min;
                    Increasing = true;
                    break;
            }

            if (originalValue > max)
                originalValue = max;

            if (value < min)
                originalValue = min;

            if (originalValue != value)
            {
                Value = originalValue;
            }
        }

        /// <summary>
        /// The current value between min and max
        /// </summary>
        public double Value { get; internal set; }

        /// <summary>
        /// The previous value
        /// </summary>
        public double? PreviousValue { get; internal set; } = null;

        /// <summary>
        /// Min Value
        /// </summary>
        public double Min { get; private set; }

        /// <summary>
        /// Max value
        /// </summary>
        public double Max { get; private set; }

        /// <summary>
        /// Step value to move towards min or max when calling GetNext()
        /// </summary>
        public double Step { get; private set; }

        public bool Increasing { get; private set; } = true;

        public IncrementMethod Method { get; set; }

        /// <summary>
        /// Updates the step size. 
        /// </summary>
        /// <param name="newStepSize">The new step size</param>
        /// <param name="wrapAround">Whether to take modulus of the step size if the value is greater than Max.</param>
        /// <exception cref="ArgumentOutOfRangeException">newStepSize is zero or negative</exception>
        /// <exception cref="ArgumentOutOfRangeException">newStepSize is greater than Max and wrapAround is false</exception>
        public void SetStepSize(int newStepSize, bool wrapAround = true)
        {
            if (Math.Sign(newStepSize) <= 0)
                throw new ArgumentOutOfRangeException(nameof(newStepSize));

            if (newStepSize > Max)
            {
                if (wrapAround)
                    Step = newStepSize % Max;
                else
                    throw new ArgumentOutOfRangeException(nameof(newStepSize));
            }
            else
            {
                Step = newStepSize;
            }
        }

        /// <summary>
        /// GetNext value towards min or max
        /// </summary>
        /// <returns>The new value</returns>
        public double GetNext()
        {
            PreviousValue = Value;

            switch (Method)
            {
                case IncrementMethod.Bit:
                    Value = (Value == Min) ? Max : Min;
                    break;

                case IncrementMethod.Cyclic:
                    if (Increasing)
                    {
                        if (Value + Step > Max)
                        {
                            Value = Max - ((Value + Step) % Max);
                            Increasing = !Increasing;
                        }
                        else
                        {
                            Value += Step;
                            if (Value == Max)
                                Increasing = !Increasing;
                        }
                    }
                    else
                    {
                        Value -= Step;
                        if (Value == Min)
                            Increasing = !Increasing;
                        else if (Value < Min)
                        {
                            Value = Min + Math.Abs(Value);
                            Increasing = !Increasing;
                        }
                    }

                    break;

                case IncrementMethod.MinMax:
                    if (Value + Step <= Max)
                    {
                        Value += Step;
                    }
                    else
                    {
                        var diff = Max - Min - Value;
                        Value = Min;
                        if (diff > 0)
                        {
                            var remain = Math.Abs(Step - diff);
                            if (Min == 0)
                            {
                                remain -= 1;
                            }
                            Value += remain;
                        }
                    }
                    break;

                case IncrementMethod.MaxMin:
                    if (Value - Step >= Min)
                    {
                        Value -= Step;
                    }
                    else
                    {
                        var remain = Value - Min;
                        Value = Max;
                        if (remain > 0)
                        {
                            Value -= remain;
                        }
                    }
                    break;

            }
            return Value;
        }

        /// <returns>Returns the next value GetNext() would return - without actually changing the Value</returns>
        public double Peek
        {
            get
            {               
                var oldIncreasing = Increasing;
                var oldPreviousValue = PreviousValue;
                var oldValue = Value;
                var peek = GetNext();
                Value = oldValue;
                PreviousValue = oldPreviousValue;
                Increasing = oldIncreasing;
                return peek;
            }
        }

        /// <returns>Returns the next value GetNext() would return - without actually changing the Value</returns>
        public double PeekAt(int repeat)
        {
            if (Math.Sign(repeat) <= 0)
                throw new ArgumentException("must be positive", nameof(repeat));

            var oldIncreasing = Increasing;
            var oldPreviousValue = PreviousValue;
            var oldValue = Value;
            var peek = GetNext();
            for (var i = 0; i < repeat - 1; i++)
            {
                peek = GetNext();
            }
            Value = oldValue;
            PreviousValue = oldPreviousValue;
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
