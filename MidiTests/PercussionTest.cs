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
    /// <summary>Unit tests for the Percussion enum</summary>
    public class PercussionTests
    {
        #region Methods

        [Test]
        public void NameTest()
        {
            Assert.AreEqual(Percussion.VibraSlap.Name(), "Vibra Slap");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => ((Percussion)82).Name());
        }

        [Test]
        public void IsValidTest()
        {
            Assert.True(Percussion.BassDrum2.IsValid());
            Percussion.BassDrum2.Validate();
            Assert.True(Percussion.OpenTriangle.IsValid());
            Percussion.OpenTriangle.Validate();
            Assert.False(((Percussion)34).IsValid());
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => ((Percussion)34).Validate());
            Assert.False(((Percussion)82).IsValid());
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => ((Percussion)82).Validate());
        }

        [Test]
        public void RangeTest()
        {
            Assert.AreEqual(Percussion.BassDrum2, PercussionRange.Default.Min);
            Assert.AreEqual(Percussion.OpenTriangle, PercussionRange.Default.Max);

            var range = new PercussionRange(Percussion.BassDrum2, Percussion.OpenTriangle);
            Assert.AreEqual(Percussion.BassDrum2, range.Min);
            Assert.AreEqual(Percussion.OpenTriangle, range.Max);

            Assert.Throws(typeof(ArgumentException), () => new PercussionRange((Percussion)34, Percussion.OpenTriangle));
            Assert.Throws(typeof(ArgumentException), () => new PercussionRange(Percussion.BassDrum2, (Percussion)82));
            Assert.Throws(typeof(ArgumentException), () => new PercussionRange(Percussion.SnareDrum1, Percussion.BassDrum2));
            
            range.Min = Percussion.ClosedHiHat;
            Assert.AreEqual(Percussion.ClosedHiHat, range.Min);

            range.Max = Percussion.CrashCymbal1;
            Assert.AreEqual(Percussion.CrashCymbal1, range.Max);

            Assert.Throws(typeof(ArgumentException), () => range.Min = (Percussion)2);
            Assert.Throws(typeof(ArgumentException), () => range.Max = (Percussion)82);
            Assert.Throws(typeof(ArgumentException), () => range.Min = Percussion.OpenTriangle);
            Assert.Throws(typeof(ArgumentException), () => range.Max = Percussion.BassDrum2);
        }

        #endregion Methods
    }
}