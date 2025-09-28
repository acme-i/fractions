using System;
using System.Collections.ObjectModel;

namespace fractions
{
    /// <summary>
    /// Extensions on ReadOnlyCollection
    /// </summary>
    public static class ReadOnlyCollectionExtensions
    {
        /// <summary>
        /// Executes callback on each member of collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        public static ReadOnlyCollection<T> ForEach<T>(this ReadOnlyCollection<T> collection, Action<T> action)
        {
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    action(item);
                }
            }
            return collection;
        }

        /// <summary>
        /// Returns whether the collection has any members.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns>whether the collection has any members</returns>
        public static bool Any<T>(this ReadOnlyCollection<T> collection)
        {
            if (collection == null)
            {
                throw new NullReferenceException(nameof(collection));
            }
            return collection.Count > 0;
        }

        public static void PrintToConsole<T>(this ReadOnlyCollection<T> collection, string none, string any)
        {
            if (!collection.Any())
            {
                Console.WriteLine(none);
            }
            else
            {
                Console.WriteLine(any);
                var i = 0;
                collection.ForEach(device =>
                {
                    Console.WriteLine($"  {i++}: {device}");
                });
                Console.WriteLine();
            }
        }
    }
}
