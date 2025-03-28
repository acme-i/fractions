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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace fractions.tests
{
    /// <summary>Unit tests for the Channel enum</summary>
    public class ClockTests
    {
        #region Methods

        [Test]
        public void AlignSanityTest()
        {
            var clock = new Clock(128);
            var channels = new List<Channel>
            {
                Channel.Channel1,
                Channel.Channel2
            };
            var a = new NoteOnOffMessage(OutputDevice.InstalledDevices.First(), channels[0], Pitch.A2, 40, 0f, clock, 6);
            var b = new NoteOnOffMessage(OutputDevice.InstalledDevices.First(), channels[1], Pitch.A3, 40, 3f, clock, 12);
            clock.Schedule(a);
            clock.Schedule(b);
            clock.Align(channels, Pitch.A2, Pitch.A4);

            Assert.AreEqual(3f, a.Duration);
            Assert.AreEqual(12f, b.Duration);
        }

        [Test]
        public void Align()
        {
            var clock = new Clock(128);
            var channels = new List<Channel>
            {
                Channel.Channel1,
                Channel.Channel2
            };
            var device = OutputDevice.InstalledDevices.First();

            /*
             0     1     3            6                       12           15
            A|------------------------|                        |
            C|------------------------|                        |

            B|           -------------|------------------------|------------
            D|     -------------------|                        |
            E|     -------------------|                        |
            */

            /*
             0     1     3            6                       12           15
            A|-----                   |                        |
            C|-----                   |                        |

            B|           -------------|------------------------|------------
            D|     ------             |                        |
            E|     ------             |                        |
            */

            var a = new NoteOnOffMessage(device, channels[0], Pitch.A2, 40, 0f, clock, 6);
            var c = new NoteOnOffMessage(device, channels[0], Pitch.A2, 40, 0f, clock, 6);

            var b = new NoteOnOffMessage(device, channels[1], Pitch.A3, 40, 3f, clock, 12);

            var d = new NoteOnOffMessage(device, channels[0], Pitch.A2, 40, 1f, clock, 6);
            var e = new NoteOnOffMessage(device, channels[1], Pitch.A2, 40, 1f, clock, 6);

            clock.Schedule(c);
            clock.Schedule(d);
            clock.Schedule(b);
            clock.Schedule(a);
            clock.Schedule(e);

            clock.Align(channels, Pitch.A2, Pitch.A4);

            Assert.AreEqual(0f, a.Time);
            Assert.AreEqual(0f, c.Time);
            Assert.AreEqual(3f, b.Time);

            Assert.AreEqual(1f, d.Time);
            Assert.AreEqual(1f, e.Time);

            Assert.AreEqual(1f, a.Duration);
            Assert.AreEqual(1f, c.Duration);
            Assert.AreEqual(12f, b.Duration);

            Assert.AreEqual(2f, d.Duration);
            Assert.AreEqual(2f, e.Duration);
        }

        #endregion Methods
    }
}