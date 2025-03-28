// Copyright (c) 2009, Tom Lokovic
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fractions.examples
{
    internal class Program
    {
        #region Fields

        /// <summary>A dictionary mapping a console key to example instances</summary>
        private static readonly Dictionary<int, ExampleBase> examples = CreateList();

        private static Dictionary<int, ExampleBase> CreateList()
        {
            var result = new Dictionary<int, ExampleBase>();
            var count = 1;
            Assembly mscorlib = typeof(Program).Assembly;

            var sortedTypes = mscorlib.GetTypes()
                .OrderBy(f => f.FullName)
                .ToList();

            foreach (Type type in sortedTypes)
            {
                try
                {
                    if (Activator.CreateInstance(type) is ExampleBase instance)
                    {
                        //Console.WriteLine(type.FullName);
                        result.Add(count++, instance);
                    }
                }
                catch //(MissingMethodException)
                {
                    //Trace.WriteLine($"the type {type.FullName} is not a kind of {nameof(ExampleBase)}");
                }
            }

            return result;
        }

        #endregion Fields

        #region Methods

        private static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("MIDI Examples:");
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------");
                foreach (KeyValuePair<int, ExampleBase> example in examples)
                {
                    Console.WriteLine("{0}: {1}", example.Key, example.Value.Description);
                }
                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine();
                Console.Write("Enter the number for an example to run, or Escape to quit: ");

                try
                {
                    var line = Console.ReadLine().Trim();
                    if (int.TryParse(line, out int result))
                    {
                        ExampleBase example = examples[result];
                        Console.Clear();
                        Console.WriteLine($"Playing {example.Description}...");
                        example.Run();
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        #endregion Methods
    }
}