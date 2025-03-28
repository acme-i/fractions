using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace fractions.tests
{
    /// <summary>Unit tests for the Note class</summary>
    public class PermutationsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        /// <summary>
        /// Func to show how to call. It does a little test for an array of 4 items.
        /// </summary>
        public void Test()
        {
            Console.WriteLine("Ouellet heap's algorithm implementation");

            Permutations.PermutationsOf("123".ToCharArray(), (vals) =>
            {
                Console.WriteLine(String.Join("", vals));
                return false;
            });


            int[] values = new int[] { 0, 1, 2, 4 };
            Permutations.PermutationsOf(values, (vals) =>
            {
                Console.WriteLine(String.Join("", vals));
                return false;
            });

            // Performance Heap's against Linq version : huge differences
            values = Enumerable.Range(0, 10).ToArray();

            Stopwatch stopWatch = new Stopwatch();
            int count = 0;

            Permutations.PermutationsOf(values, (vals) =>
            {
                foreach (var v in vals)
                {
                    count++;
                }
                return false;
            });

            stopWatch.Stop();
        }
    }
}