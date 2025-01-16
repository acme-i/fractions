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

namespace fractions.examples
{
    public class Drummer
    {
        #region Fields

        private readonly int beatsPerMeasure;

        private readonly Clock clock;

        private readonly List<Message> messagesForOneMeasure;

        private readonly IOutputDevice outputDevice;

        #endregion Fields

        #region Constructors

        public Drummer(Clock clock, IOutputDevice outputDevice, int beatsPerMeasure)
        {
            this.clock = clock;
            this.outputDevice = outputDevice;
            this.beatsPerMeasure = beatsPerMeasure;
            messagesForOneMeasure = new List<Message>();
            for (int i = 0; i < beatsPerMeasure; ++i)
            {
                Percussion percussion = i == 0 ? Percussion.PedalHiHat : Percussion.MidTom1;
                int velocity = i == 0 ? 100 : 40;
                messagesForOneMeasure.Add(new PercussionMessage(this.outputDevice, percussion, velocity, i));
            }
            messagesForOneMeasure.Add(new CallbackMessage(new CallbackMessage.CallbackType(CallbackHandler), 0));
            clock.Schedule(messagesForOneMeasure);
        }

        #endregion Constructors

        #region Methods

        private void CallbackHandler(float time)
        {
            // Round up to the next measure boundary.
            float timeOfNextMeasure = time + beatsPerMeasure;
            clock.Schedule(messagesForOneMeasure, timeOfNextMeasure);
        }

        #endregion Methods
    }
}