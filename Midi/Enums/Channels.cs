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

namespace fractions
{
    public static class Channels
    {
        private readonly static List<Channel> all = Enum.GetValues(typeof(Channel)).Cast<Channel>().OrderBy(c => (int)c).ToList();
        private readonly static List<Channel> instrumentChannels = all.Where(c => !c.IsPercussion()).ToList();
        private readonly static List<Channel> percussionChannels = all.Where(c => c.IsPercussion()).ToList();

        /// <summary>
        /// Returns a list of all channels
        /// </summary>
        public static List<Channel> All => all;

        /// <summary>
        /// Returns a list of all channels except the Percussion channel
        /// </summary>
        public static List<Channel> InstrumentChannels => instrumentChannels;

        /// <summary>
        /// Returns a list of all the percussion channels.
        /// </summary>
        public static List<Channel> PercussionChannels => percussionChannels;

        /// <summary>
        /// Channel numbers are index based.
        /// Returns a list of all the channels in the range [indexStart; indexEnd].
        /// </summary>
        public static List<Channel> Range(int indexStart, int? indexEnd = null)
        {
            if (!indexEnd.HasValue)
                indexEnd = indexStart;

            var first = (int)all.First();
            var last = (int)all.Last();

            if (indexStart > indexEnd || indexStart > last || indexEnd > last || indexStart < first || indexEnd < first)
                throw new ArgumentOutOfRangeException();

            return Enumerable.Range(indexStart, indexEnd.Value - indexStart + 1).Cast<Channel>().ToList();
        }


        /// <summary>
        /// Returns a list of all the channels in the range [start; end]
        /// </summary>
        public static List<Channel> Range(Channel start, Channel? end = null)
        {
            if (!end.HasValue)
                end = start;

            return Range((int)start, (int)end);
        }
    }
}