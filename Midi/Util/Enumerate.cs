using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace fractions
{
    public class Enumerate<T> : IEnumerable<T>, INotifyCollectionChanged
    {
        internal IList<T> collection = new List<T>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        public Enumerate(IEnumerable<T> collection, IncrementMethod method = IncrementMethod.MinMax, double step = 1, double startIndex = 0, string name = null)
        {
            AssertCollection(collection);
            AssertMethod(method);

            this.collection = collection.ToList();
            this.Incrementor = new Incrementor(Math.Max(0, startIndex), 0, this.collection.Count - 1, Math.Min(Math.Max(1, step), this.collection.Count - 1), method);
            this.Name = name ?? Guid.NewGuid().ToString();
        }

        public Enumerate(IEnumerable<T> collection, Incrementor incrementor, string name = null)
        {
            AssertCollection(collection);

            this.collection = collection.ToList();
            this.Incrementor = incrementor;
            this.Name = name ?? Guid.NewGuid().ToString();
        }

        public Enumerate()
        {
        }

        public Incrementor Incrementor { get; set; }

        /// <summary>
        /// Optional name for this instance
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The current value between min and max
        /// </summary>
        public double Index => Incrementor.Index;

        /// <summary>
        /// Returns the current, but does not advance the incrementor
        /// </summary>
        /// <returns></returns>
        public T Current
        {
            get => collection[(int)Incrementor.Index];
        }

        /// <summary>
        /// Returns the current, then advances the incrementor
        /// </summary>
        /// <returns></returns>
        public T GetNext()
        {
            var current = collection[(int)Incrementor.Index];
            Incrementor.GetNext();
            Counter++;
            return current;
        }

        /// <summary>
        /// Returns the current, then advances the incrementor
        /// </summary>
        /// <returns></returns>
        public void Reset()
        {
            Incrementor.Reset();
        }

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
        /// The previous value
        /// </summary>
        public double? PreviousIndex => Incrementor.PreviousIndex;

        /// <summary>
        /// StepSize value to move towards min or max when calling GetNext()
        /// </summary>
        public double StepSize => Incrementor.StepSize;

        public bool Increasing => Incrementor.Increasing;

        public IncrementMethod Method => Incrementor.Method;

        /// <summary>
        /// Returns the next, then advances back one step
        /// </summary>
        /// <returns></returns>
        public T Peek
        {
            get => collection[(int)Incrementor.Peek];
        }

        /// <summary>
        /// Returns the next value steps ahead, then advances back to current
        /// </summary>
        /// <returns></returns>
        public T PeekAt(int steps)
        {
            return collection[(int)Incrementor.PeekAt(steps)];
        }

        public void Set(IEnumerable<T> others)
        {
            AssertCollection(others);

            Counter = 0;

            if (Incrementor.Index >= collection.Count)
                Incrementor.Index %= collection.Count;

            collection.Clear();
            AddRange(others);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Add(T item)
        {
            if (collection is List<T> source)
            {
                source.AddRange(new[] { item });
            }
            else
                collection.Add(item);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
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

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Returns a clone of this instance.
        /// The clone will have the same collection, incrementor state and name, but a different Id.
        /// </summary>
        /// <returns></returns>
        public Enumerate<T> Clone()
        {
            return new Enumerate<T>(collection, Incrementor.Clone(), Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Returns a clone of this instance.
        /// The clone will have the same collection, incrementor state and name, but a different Id.
        /// </summary>
        /// <returns></returns>
        public Enumerate<T> CloneWithPreviousIndex()
        {
            var incr = Incrementor.Clone();
            incr.PreviousIndex = Incrementor.PreviousIndex;
            return new Enumerate<T>(collection, incr, Guid.NewGuid().ToString());
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
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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