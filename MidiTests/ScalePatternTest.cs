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
using NUnit.Framework;

namespace fractions.tests
{
    /// <summary>Unit tests for the ScalePattern class</summary>
    public class ScalePatternTests
    {
        #region Methods

        [Test]
        public void ConstructionErrors()
        {
            Assert.Throws(typeof(ArgumentNullException),
                () => new ScalePattern(null, new int[] { 0, 1, 2, 3 }));
            Assert.Throws(typeof(ArgumentNullException),
                () => new ScalePattern("a", null));
            Assert.Throws(typeof(ArgumentException),
                () => new ScalePattern("a", new int[] { 1, 2, 3, 4 }));
            Assert.Throws(typeof(ArgumentException),
                () => new ScalePattern("a", new int[] { 0, 1, 2, 3, 3 }));
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(
                new ScalePattern("a", new int[] { 0, 1, 2 }),
                new ScalePattern("a", new int[] { 0, 1, 2 }));
            Assert.AreNotEqual(
                new ScalePattern("a", new int[] { 0, 1, 2 }),
                new ScalePattern("b", new int[] { 0, 1, 2 }));
            Assert.AreNotEqual(
                new ScalePattern("a", new int[] { 0, 1, 3 }),
                new ScalePattern("a", new int[] { 0, 1, 2 }));
            Assert.AreNotEqual(
                new ScalePattern("a", new int[] { 0, 1, 2, 3 }),
                new ScalePattern("a", new int[] { 0, 1, 2 }));
        }

        [Test]
        public void Properties()
        {
            var sp = new ScalePattern("a", new int[] { 0, 1, 2, 3 });
            Assert.AreEqual(sp.Name, "a");
            Assert.AreEqual(sp.Ascent.Length, 4);
        }

        #endregion Methods
    }
}