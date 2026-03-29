using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fractions
{
    public class Enumerate<T> : IEnumerable<T>
    {
        internal IList<T> collection = new List<T>();

        public Enumerate(IEnumerable<T> collection, IncrementMethod method = IncrementMethod.MinMax, double step = 1, double startIndex = 0, string name = null)
        {
            AssertCollection(collection);
            AssertMethod(method);

            this.collection = collection.ToList();
            Method = method;
            StepSize = step;
            Index = startIndex;
            Name = name ?? Guid.NewGuid().ToString();
        }

        public Enumerate()
        {
        }

        /// <summary>
        /// Optional name for this instance
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Min Index
        /// </summary>
        public double Min { get { return 0; } }

        /// <summary>
        /// Max value
        /// </summary>
        public double Max { get { return collection.Count - 1; } }

        /// <summary>
        /// Gets the length of the collection
        /// </summary>
        public int Count { get { return collection.Count; } }

        /// <summary>
        /// Gets the number of times GetNext has been called on this instance.
        /// Counter starts at 0 and is only reset when Set is called.
        /// </summary>
        public long Counter { get; private set; } = 0;

        /// <summary>
        /// An unique identifier for this instance. This is needed to differentiate
        /// instances when Enumerate are cloned.
        /// </summary>
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Returns the current, but does not advance the incrementor
        /// </summary>
        /// <returns></returns>
        public T Current
        {
            get => collection[(int)Index];
        }

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
        /// The current value between min and max
        /// </summary>
        public double Index { get; set; }

        /// <summary>
        /// The previous value
        /// </summary>
        public double? PreviousIndex { get; set; } = null;

        /// <summary>
        /// StepSize value to move towards min or max when calling GetNext()
        /// </summary>
        public double StepSize { get; internal set; } = 1;

        public bool Increasing { get; internal set; } = true;

        public IncrementMethod Method { get; set; }

        /// <summary>
        /// GetNext value towards min or max
        /// </summary>
        /// <returns>The new value</returns>
        public T GetNext()
        {
            if(PreviousIndex == null)
            {
                PreviousIndex = 0;
                return collection[0];
            }

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

            return collection[(int)Index];
        }

        /// <returns>Returns the next value GetNext() would return - without actually changing the Index</returns>
        public T Peek
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
        public T PeekAt(int repeat)
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

        public void Set(IEnumerable<T> others)
        {
            AssertCollection(others);

            collection.Clear();
            AddRange(others);
        }

        public void Add(T item)
        {
            if (collection is List<T> source)
            {
                source.AddRange(new[] { item });
            }
            else
                collection.Add(item);
        }

        /// <summary>
        /// Adds a range of items to the collection
        /// </summary>
        /// <param name="others">Items to add to the collection</param>
        public void AddRange(IEnumerable<T> others)
        {
            if (collection is List<T> source)
            {
                source.AddRange(others);
            }
            else
                foreach (var other in others)
                    collection.Add(other);
        }

        /// <summary>
        /// Executes the specified action on each item in the collection.
        /// </summary>
        /// <param name="action">The action to execute on each item.</param>
        public void ForEach(Action<T> action)
        {
            if (action is null)
                return;

            if (collection is List<T> source)
                source.ForEach(c => action(c));
            else
                collection.ToList().ForEach(c => action(c));
        }

        /// <summary>
        /// Reverses the order of the items in the collection.
        /// </summary>
        public void Reverse()
        {
            collection = collection.Reverse().ToList();
        }

        /// <summary>
        /// Returns a clone of this instance.
        /// The clone will have the same collection, incrementor state and name, but a different Id.
        /// </summary>
        /// <returns></returns>
        public Enumerate<T> Clone()
        {
            return new Enumerate<T>(collection, Method, StepSize, Index, Guid.NewGuid().ToString());
        }

        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)collection).GetEnumerator();
        }

        public List<T> ToList()
        {
            return collection.ToList();
        }

        public void Clear()
        {
            collection.Clear();
        }

        private static void AssertMethod(IncrementMethod method)
        {
            var possibleMethods = Enum.GetValues(typeof(IncrementMethod)).Cast<IncrementMethod>().ToList();
            if (!possibleMethods.Contains(method)) throw new ArgumentOutOfRangeException(nameof(method));
        }

        private static void AssertCollection(IEnumerable<T> collection)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));
        }
    }
}