using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fractions
{
    public class Enumerate<T> : IEnumerable<T>
    {
        internal IList<T> collection;

        public Enumerate(Enumerate<T> source, int newStepValue, string name = null)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            this.collection = source.ToList();
            this.Incrementor = new Incrementor(source.Incrementor, newStepValue);
            this.Name = name ?? Guid.NewGuid().ToString();
        }

        public Enumerate(IEnumerable<T> collection, IncrementMethod method = IncrementMethod.MinMax, int step = 1, int startIndex = 0, string name = null)
        {
            AssertCollection(collection);
            AssertMethod(method);

            this.collection = collection.ToList();
            this.Incrementor = new Incrementor(Math.Max(0, startIndex), 0, this.collection.Count - 1, Math.Min(Math.Max(1, step), this.collection.Count - 1), method);
            this.Name = name ?? Guid.NewGuid().ToString();
        }

        public Enumerate(IEnumerable<T> collection, Incrementor source, string name = null)
        {
            AssertCollection(collection);

            this.collection = collection.ToList();
            this.Incrementor = source.Clone();
            this.Name = name ?? Guid.NewGuid().ToString();
        }
        
        public string Name { get; set; } = string.Empty;

        public Incrementor Incrementor { get; internal set; }

        public int Length { get { return collection.Count; } }

        /// <summary>
        /// Returns the current, but does not advance
        /// </summary>
        /// <returns></returns>
        public T Current
        {
            get => collection[(int)Incrementor.Value];
        }

        public T Min
        {
            get => collection.Min();
        }

        public T Max
        {
            get => collection.Max();
        }

        /// <summary>
        /// Returns the current, then advances
        /// </summary>
        /// <returns></returns>
        public T GetNext()
        {
            var current = collection[(int)Incrementor.Value];
            Incrementor.GetNext();
            return current;
        }

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

            if (Incrementor.Value >= collection.Count)
                Incrementor.Value %= collection.Count;

            collection.Clear();
            AddRange(others);
        }

        public void AddRange(IEnumerable<T> others)
        {
            if (collection is List<T> source)
                source.AddRange(others);
            else
                foreach (var other in others)
                    collection.Add(other);
        }

        public void ForEach(Action<T> action)
        {
            if (action is null)
                return;

            if (collection is List<T> source)
                source.ForEach(c => action(c));
            else
                collection.ToList().ForEach(c => action(c));
        }

        public void Reverse()
        {
            collection = collection.Reverse().ToList();
        }

        public Enumerate<T> Clone()
        {
            return new Enumerate<T>(collection, Incrementor.Clone());
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

        private static void AssertMethod(IncrementMethod method)
        {
            var possibleMethods = Enum.GetValues(typeof(IncrementMethod)).Cast<IncrementMethod>().ToList();
            if (!possibleMethods.Contains(method)) throw new ArgumentOutOfRangeException(nameof(method));
        }

        private static void AssertCollection(IEnumerable<T> collection)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            if (collection.Count() <= 1) throw new ArgumentOutOfRangeException(nameof(collection), "collection must have at least 1 items");
        }
    }
}