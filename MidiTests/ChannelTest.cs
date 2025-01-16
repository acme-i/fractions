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
using NUnit.Framework;

namespace fractions.tests
{
    /// <summary>Unit tests for the Channel enum</summary>
    [TestFixture]
    public class ChannelTests
    {
        #region Methods

        [Test]
        public void NameTest()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => ((Channel)(-1)).Name());
            Assert.AreEqual("Channel 1", Channel.Channel1.Name());
            Assert.AreEqual("Channel 16", Channel.Channel16.Name());
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => ((Channel)17).Name());
        }

        [Test]
        public void IsPercussionTest()
        {
            Assert.AreEqual(true, Channel.Channel10.IsPercussion());
            Assert.True(Channel.Channel10.IsValid());
            Assert.DoesNotThrow(() => Channel.Channel10.Validate());

            Assert.IsTrue(Channels.PercussionChannels.Contains(Channel.Channel10));

            foreach (var c in Channels.PercussionChannels)
                Assert.IsTrue(c.IsPercussion());

            foreach (var c in Channels.InstrumentChannels)
                Assert.IsFalse(c.IsPercussion());
        }

        [Test]
        public void ValidTest()
        {
            foreach (var c in Channels.All)
            {
                Assert.True(c.IsValid(), $"Expected channel {(int)c} to be valid");
                Assert.DoesNotThrow(() => c.Validate());
            }
            Assert.False(((Channel)(-1)).IsValid());
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => ((Channel)(-1)).Validate());
            Assert.False(((Channel)17).IsValid());
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => ((Channel)17).Validate());
        }

        [Test]
        public void RangeTest()
        {
            Assert.AreEqual(new List<Channel> { Channel.Channel1 }, Channels.Range(0));
            Assert.AreEqual(new List<Channel> { Channel.Channel1 }, Channels.Range(0, 0));

            Assert.AreEqual(new List<Channel> { Channel.Channel1 }, Channels.Range(Channel.Channel1));
            Assert.AreEqual(new List<Channel> { Channel.Channel1 }, Channels.Range(Channel.Channel1, Channel.Channel1));

            Assert.AreEqual(new List<Channel> { Channel.Channel2 }, Channels.Range(1));
            Assert.AreEqual(new List<Channel> { Channel.Channel2 }, Channels.Range(1, 1));

            Assert.AreEqual(Channels.All, Channels.Range(0, 15));
            Assert.AreEqual(new List<Channel> { Channel.Channel2, Channel.Channel3, Channel.Channel4, Channel.Channel5 }, Channels.Range(Channel.Channel2, Channel.Channel5));

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => Channels.Range(-1, 0));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => Channels.Range(4, 17));
        }


        [Test]
        public void AllChannelsTest()
        {
            var cs = new List<Channel> {
                Channel.Channel1, Channel.Channel2, Channel.Channel3, Channel.Channel4,
                Channel.Channel5, Channel.Channel6, Channel.Channel7, Channel.Channel8, Channel.Channel9,
                Channel.Channel10, Channel.Channel11, Channel.Channel12, Channel.Channel13,
                Channel.Channel14, Channel.Channel15, Channel.Channel16
            };
            Assert.AreEqual(Channels.All, cs);
        }

        [Test]
        public void InstrumentChannelsTest()
        {
            var cs = new List<Channel> {
                Channel.Channel1, Channel.Channel2, Channel.Channel3, Channel.Channel4,
                Channel.Channel5, Channel.Channel6, Channel.Channel7, Channel.Channel8, Channel.Channel9,
                                  Channel.Channel11, Channel.Channel12, Channel.Channel13,
                Channel.Channel14, Channel.Channel15, Channel.Channel16
            };
            Assert.AreEqual(Channels.InstrumentChannels, cs);
        }

        [Test]
        public void PercussionChannelsTest()
        {
            var cs = new List<Channel> { Channel.Channel10 };
            Assert.AreEqual(Channels.PercussionChannels, cs);
        }

        #endregion Methods
    }
}