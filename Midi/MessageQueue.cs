// Copyright (c) 2009, Tom Lokovic All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Linq;

namespace fractions
{
    /// <summary>A time-sorted queue of MIDI messages</summary>
    /// Messages can be added in any order, and can be popped off in timestamp order.
    public class MessageQueue
    {
        #region Fields

        // Linked list where each node correponds to a specific timestamp, nodes are sorted by
        // timestamp increasing, and each node contains an ordered list of messages that have been
        // added for that timestamp.
        private readonly LinkedList<List<Message>> messages = new LinkedList<List<Message>>();

        #endregion Fields

        #region Properties

        /// <summary>The timestamp of the earliest messsage(s) in the queue</summary>
        /// Throws an exception if the queue is empty.
        public float EarliestTimestamp
        {
            get
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("queue is empty");
                }
                return messages.First.Value[0].Time;
            }
        }

        /// <summary>True if the queue is empty</summary>
        public bool IsEmpty => messages.Count == 0;

        #endregion Properties

        #region Methods

        public void AlignDissonants(IEnumerable<Channel> channels, PitchRange range)
        {
            var relevant = messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteOnOffMessage))
                .Cast<NoteOnOffMessage>()
                .Where(m => channels.Contains(m.Channel) && range.IsInside(m.Pitch))
                .Where(m => !float.IsNaN(m.Time))
                .OrderBy(m => m.Time)
                .ThenBy(m => m.Duration)
                .ToList();

            for (var i = 0; i <= relevant.Count - 2; i++)
            {
                var current = relevant[i];

                for (var j = i + 1; j < relevant.Count; j++)
                {
                    var next = relevant[j];
                    if (next.Time > current.Time + current.Duration)
                    {
                        break;
                    }

                    int left = (int)current.Pitch;
                    int right = (int)next.Pitch;
                    while (left > 12)
                    {
                        left -= 12;
                    }
                    while (right > 12)
                    {
                        right -= 12;
                    }

                    if (Math.Abs(left - right) < 3)
                    {
                        var diff = current.Time + current.Duration - next.Time;
                        if (diff == 0 && current.Time == next.Time)
                        {
                            current.Velocity = 0;
                        }
                        else if (diff == 0 || diff == current.Duration)
                        {
                            current.Velocity = 0;
                        }
                        else
                        {
                            current.Duration -= diff;
                        }
                    }
                }

            }
        }

        public void AlignDissonants(float timeMin, float timeMax, IEnumerable<Channel> channels, PitchRange range)
        {
            var relevant = messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteOnOffMessage))
                .Cast<NoteOnOffMessage>()
                .Where(m => channels.Contains(m.Channel) && range.IsInside(m.Pitch))
                .Where(m => !float.IsNaN(m.Time))
                .Where(m => m.Time >= timeMin && m.Time < timeMax)
                .OrderBy(m => m.Time)
                .ThenBy(m => m.Duration)
                .ToList();

            for (var i = 0; i <= relevant.Count - 2; i++)
            {
                var current = relevant[i];

                for (var j = i + 1; j < relevant.Count; j++)
                {
                    var next = relevant[j];
                    if (next.Time > current.Time + current.Duration)
                    {
                        break;
                    }

                    int left = (int)current.Pitch;
                    int right = (int)next.Pitch;
                    while (left > 12)
                    {
                        left -= 12;
                    }
                    while (right > 12)
                    {
                        right -= 12;
                    }

                    if (Math.Abs(left - right) < 3)
                    {
                        var diff = current.Time + current.Duration - next.Time;
                        if (diff == 0 && current.Time == next.Time)
                        {
                            current.Velocity = 0;
                        }
                        else if (diff == 0 || diff == current.Duration)
                        {
                            current.Velocity = 0;
                        }
                        else
                        {
                            current.Duration -= diff;
                        }
                    }
                }

            }
        }

        public void Align(IEnumerable<Channel> channels, PitchRange range)
        {
            var relevant = messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteOnOffMessage))
                .Cast<NoteOnOffMessage>()
                .Where(m => channels.Contains(m.Channel) && range.IsInside(m.Pitch))
                .OrderBy(m => m.Time)
                .ThenBy(m => m.Duration)
                .ToList();

            for (var i = 0; i <= relevant.Count - 2; i++)
            {
                var current = relevant[i];
                for (var j = i + 1; j < relevant.Count; j++)
                {
                    var next = relevant[j];
                    if (next.Time > current.Time + current.Duration)
                    {
                        break;
                    }

                    var diff = current.Time + current.Duration - next.Time;
                    if (diff == 0 && current.Time == next.Time)
                    {
                        current.Velocity = 0;
                    }
                    else if (diff == 0 || diff == current.Duration)
                    {
                        current.Velocity = 0;
                    }
                    else
                    {
                        current.Duration -= diff;
                    }
                }

            }
        }

        public void Align(float timeMin, float timeMax, IEnumerable<Channel> channels, PitchRange range)
        {
            var relevant = messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteOnOffMessage))
                .Cast<NoteOnOffMessage>()
                .Where(m => channels.Contains(m.Channel) && range.IsInside(m.Pitch))
                .Where(m => m.Time >= timeMin && m.Time < timeMax)
                .OrderBy(m => m.Time)
                .ThenBy(m => m.Duration)
                .ToList();

            for (var i = 0; i <= relevant.Count - 2; i++)
            {
                var current = relevant[i];
                for (var j = i + 1; j < relevant.Count; j++)
                {
                    var next = relevant[j];
                    if (next.Time > current.Time + current.Duration)
                    {
                        break;
                    }

                    var diff = current.Time + current.Duration - next.Time;
                    if (diff == 0 && current.Time == next.Time)
                    {
                        current.Velocity = 0;
                    }
                    else if (diff == 0 || diff == current.Duration)
                    {
                        current.Velocity = 0;
                    }
                    else
                    {
                        current.Duration -= diff;
                    }
                }

            }
        }

        public void SetMaxDuration(IEnumerable<Channel> channels, float maxDuration, PitchRange range)
        {
            AssertGreaterThanZero(maxDuration, nameof(maxDuration));

            messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteOnOffMessage))
                .Cast<NoteOnOffMessage>()
                .Where(m => channels.Contains(m.Channel) && m.Duration > maxDuration && range.IsInside(m.Pitch))
                .ToList()
                .ForEach(a =>
                {
                    a.Duration = maxDuration;
                });
        }

        public void SetMinDuration(IEnumerable<Channel> channels, float minDuration, PitchRange range)
        {
            AssertGreaterThanZero(minDuration, nameof(minDuration));

            messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteOnOffMessage))
                .Cast<NoteOnOffMessage>()
                .Where(m => channels.Contains(m.Channel) && m.Duration < minDuration && range.IsInside(m.Pitch))
                .ToList()
                .ForEach(a =>
                {
                    a.Duration = minDuration;
                });
        }

        public void SetMaxVelocity(IEnumerable<Channel> channels, float maxVelocity, PitchRange range)
        {
            AssertGreaterThanZero(maxVelocity, nameof(maxVelocity));

            messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteMessage))
                .Cast<NoteMessage>()
                .Where(m => channels.Contains(m.Channel) && m.Velocity > maxVelocity && range.IsInside(m.Pitch))
                .ToList()
                .ForEach(a =>
                {
                    a.Velocity = maxVelocity;
                });
        }

        public void SetMinVelocity(IEnumerable<Channel> channels, float minVelocity, PitchRange range)
        {
            AssertGreaterThanZero(minVelocity, nameof(minVelocity));

            messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteMessage))
                .Cast<NoteMessage>()
                .Where(m => channels.Contains(m.Channel) && m.Velocity > minVelocity && range.IsInside(m.Pitch))
                .ToList()
                .ForEach(a =>
                {
                    a.Velocity = minVelocity;
                });
        }

        public void SetMaxPitch(IEnumerable<Channel> channels, Pitch maxPitch)
        {
            messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteOnOffMessage))
                .Cast<NoteOnOffMessage>()
                .Where(m => channels.Contains(m.Channel) && m.Pitch > maxPitch)
                .ToList()
                .ForEach(a =>
                {
                    while (a.Pitch > maxPitch)
                        a.Pitch -= 12;
                });
        }

        public void SetMinPitch(IEnumerable<Channel> channels, Pitch minPitch)
        {
            messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteOnOffMessage))
                .Cast<NoteOnOffMessage>()
                .Where(m => channels.Contains(m.Channel) && m.Pitch < minPitch)
                .ToList()
                .ForEach(a =>
                {
                    while (a.Pitch < minPitch)
                        a.Pitch += 12;
                });
        }

        public void MovePitchAbove(IEnumerable<Channel> channels, Pitch max, Predicate<Pitch> shouldBeDeleted = null, Predicate<Pitch> shouldBeMoved = null)
        {
            bool deletedAny = false;

            messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteMessage))
                .Cast<NoteMessage>()
                .Where(m => channels.Contains(m.Channel) && m.Pitch > max)
                .ToList()
                .ForEach(a =>
                {
                    if (shouldBeDeleted != null && shouldBeDeleted(a.Pitch))
                    {
                        deletedAny = true;
                        a.Time = -1;
                    }
                    else
                        while (a.Pitch > max && shouldBeMoved(a.Pitch))
                            if (a.Pitch - 12 > Pitch.CNeg1)
                                a.Pitch -= 12;
                            else
                                break;
                });

            if (deletedAny)
            {
                Cleanup();
            }
        }

        public void MovePitchBelow(IEnumerable<Channel> channels, Pitch min, Predicate<Pitch> shouldBeDeleted = null, Predicate<Pitch> shouldBeMoved = null)
        {
            bool deletedAny = false;

            messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteMessage))
                .Cast<NoteMessage>()
                .Where(m => channels.Contains(m.Channel) && m.Pitch < min)
                .ToList()
                .ForEach(a =>
                {
                    if (shouldBeDeleted != null && shouldBeDeleted(a.Pitch))
                    {
                        deletedAny = true;
                        a.Time = -1;
                    }
                    else
                        while (a.Pitch > min && shouldBeMoved(a.Pitch))
                            if (a.Pitch + 12 < Pitch.G9)
                                a.Pitch += 12;
                            else
                                break;
                });

            if (deletedAny)
            {
                Cleanup();
            }
        }
        
        public void ToPrecision(IEnumerable<Channel> channels, float fraction, PitchRange range)
        {
            AssertGreaterThanZero(fraction, nameof(fraction));

            messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteMessage))
                .Cast<NoteMessage>()
                .Where(m => channels.Contains(m.Channel) && range.IsInside(m.Pitch))
                .ToList()
                .ForEach(m =>
                {
                    m.Time = (float)(Math.Round(m.Time * fraction) / fraction);
                });

            if (channels.Contains(Channel.Channel10))
                messages.SelectMany(x => x)
                    .Where(m => m.GetType() == typeof(PercussionMessage))
                    .Cast<PercussionMessage>()
                    .ToList()
                    .ForEach(m =>
                    {
                        m.Time = (float)(Math.Round(m.Time * fraction) / fraction);
                    });
        }

        public void Cleanup()
        {
            foreach (var coll in messages)
                if (coll.Any())
                    coll.RemoveAll((m) =>
                    {
                        return float.IsNaN(m.Time) || -1 == Math.Sign(m.Time);
                    });

            var emptys = messages.ToList().Where(ms => !ms.Any());
            foreach (var empty in emptys)
            {
                messages.Remove(empty);
            }
        }

        public void RemoveIdenticalNotes(IEnumerable<Channel> channels, PitchRange range)
        {
            var groupings = messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteOnOffMessage))
                .Cast<NoteOnOffMessage>()
                .Where(m => channels.Contains(m.Channel))
                .Where(m => range.IsInside(m.Pitch))
                .GroupBy(m => m.Pitch)
                .Where(m => m.Count() > 1)
                    .SelectMany(m => m)
                    .GroupBy(m => m.Time)
                    .Where(m => m.Count() > 1)
                        .SelectMany(m => m)
                        .GroupBy(m => m.Duration)
                        .Where(m => m.Count() > 1)
                .ToList();

            foreach (var grouping in groupings)
            {
                var relevant = grouping.ToList().OrderBy(m => m.Time).ThenByDescending(m => m.Velocity).ToList();
                var first = relevant.First();
                var firstTime = first.Time;
                var relevantMessages = messages.Where(c => c.Any() && c.Last().Time >= firstTime);
                foreach (List<Message> coll in relevantMessages)
                {
                    coll.RemoveAll((m) =>
                    {
                        if (m is NoteOnOffMessage nm)
                        {
                            return nm != first && nm.Time == first.Time && nm.Duration == first.Duration;
                        }
                        return false;
                    });
                }
            }

            Cleanup();
        }

        public void RemoveIdenticalPercussionNotes(Percussion minPercussion = Percussion.BassDrum2, Percussion maxPercussion = Percussion.OpenTriangle)
        {
            AssertMinMaxPercussion(minPercussion, maxPercussion);

            var groupings = messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(PercussionMessage))
                .Cast<PercussionMessage>()
                .Where(m => m.Percussion >= minPercussion && m.Percussion <= maxPercussion)
                .GroupBy(m => m.Percussion)
                .Where(m => m.Count() > 1)
                    .SelectMany(m => m)
                    .GroupBy(m => m.Time)
                    .Where(m => m.Count() > 1)
                .ToList();

            foreach (var grouping in groupings)
            {
                var relevant = grouping.ToList().OrderBy(m => m.Time).ToList();
                var first = relevant.First();
                var firstTime = first.Time;
                var relevantMessages = messages.Where(c => c.Any() && c.Last().Time >= firstTime);
                foreach (List<Message> coll in relevantMessages)
                {
                    coll.RemoveAll((m) =>
                    {
                        if (m is PercussionMessage nm)
                        {
                            return nm != first && nm.Time == first.Time && nm.Percussion == first.Percussion;
                        }
                        return false;
                    });
                }
            }

            Cleanup();
        }

        public void RemoveNotesLongerThan(IEnumerable<Channel> channels, float duration)
        {
            AssertGreaterThanZero(duration, nameof(duration));

            foreach (var coll in messages)
            {
                if (coll.Any())
                {
                    var tooShort = coll.Where(c => c.GetType() == typeof(NoteOnOffMessage))
                        .Cast<NoteOnOffMessage>()
                        .Where(c => channels.Contains(c.Channel))
                        .Where(c => c.Duration > duration);
                    coll.RemoveAll(c =>
                    {
                        return tooShort.Contains(c);
                    });
                }
            }

            Cleanup();
        }

        public void RemoveNotesShorterThan(IEnumerable<Channel> channels, float duration)
        {
            AssertGreaterThanZero(duration, nameof(duration));

            foreach (var coll in messages)
            {
                if (coll.Any())
                {
                    var tooShort = coll.Where(c => c.GetType() == typeof(NoteOnOffMessage))
                        .Cast<NoteOnOffMessage>()
                        .Where(c => channels.Contains(c.Channel))
                        .Where(c => c.Duration < duration);
                    coll.RemoveAll(c =>
                    {
                        return tooShort.Contains(c);
                    });
                }
            }

            Cleanup();
        }

        public void RemoveNotesAbove(IEnumerable<Channel> channels, Pitch max)
        {
            foreach (var coll in messages)
            {
                if (coll.Any())
                {
                    var tooHigh = coll.Where(c => c.GetType() == typeof(NoteMessage))
                        .Cast<NoteMessage>()
                        .Where(c => channels.Contains(c.Channel))
                        .Where(c => c.Pitch > max);
                    coll.RemoveAll(c =>
                    {
                        return tooHigh.Contains(c);
                    });
                }
            }

            Cleanup();
        }

        public void ApplyMelody(IEnumerable<Channel> channels, Enumerate<Pitch> melody, int maxRepeats = 1)
        {
            if (melody.Length > 0)
                foreach (var coll in messages)
                {
                    var notes = coll
                        .Where(c => c.GetType() == typeof(NoteOnOffMessage))
                        .Cast<NoteOnOffMessage>()
                        .Where(c => channels.Contains(c.Channel))
                        .OrderBy(c => c.Time)
                        .ThenBy(c => c.Channel);
                    var counter = 0;
                    var repeater = Pitch.A0;
                    foreach (var note in notes)
                    {
                        if (maxRepeats > 0)
                        {
                            if (counter == 0)
                            {
                                var newNote = melody.GetNext();
                                while (newNote == repeater)
                                {
                                    newNote = melody.GetNext();
                                }
                                repeater = newNote;
                            }
                            note.Pitch = repeater;
                            if (counter + 1 > maxRepeats)
                            {
                                counter = 0;
                            }
                            else
                            {
                                counter += 1;
                            }
                        }
                        else
                        {
                            note.Pitch = melody.GetNext();
                        }
                    }
                }
        }

        public void ScaleDurationAndTime(IEnumerable<Channel> channels, float amount)
        {
            if (amount != 1)
                foreach (var coll in messages)
                {
                    var notes = coll
                        .Where(c => c.GetType() == typeof(NoteOnOffMessage))
                        .Cast<NoteOnOffMessage>()
                        .Where(c => channels.Contains(c.Channel));
                    foreach (var note in notes)
                    {
                        note.Duration *= amount;
                        note.Time *= amount;
                    }
                }
        }

        public void InterpolateVelocity(List<Channel> channels, double newMin, double newMax, PitchRange range, int method = 1)
        {
            AssertMinMax(newMin, newMax);

            messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteMessage))
                .Cast<NoteMessage>()
                .Where(m => channels.Contains(m.Channel))
                .Where(m => range.IsInside(m.Pitch))
                .ToList()
                .ForEach(m =>
                {
                    var mv = DeviceBase.ClampControlChange(m.Velocity);
                    var newVelocity = Interpolator.Interpolate(mv, 0.0, 127.0, newMin, newMax, method);
                    m.Velocity = DeviceBase.ClampControlChange(newVelocity);
                });

            if (channels.Contains(Channel.Channel10))
                messages.SelectMany(x => x)
                    .Where(m => m.GetType() == typeof(PercussionMessage))
                    .Cast<PercussionMessage>()
                    .ToList()
                    .ForEach(m =>
                    {
                        var mv = DeviceBase.ClampControlChange(m.Velocity);
                        var newVelocity = Interpolator.Interpolate(mv, 0.0, 127.0, newMin, newMax, method);
                        m.Velocity = DeviceBase.ClampControlChange(newVelocity);
                    });
        }

        /// <summary>
        /// From all NoteMessages in messages, 
        ///    which have a channel equal to one of sourceChannels
        ///    raise each note an octave - unless it would raise it above Pitch.G9 in which case it is ignored
        /// </summary>
        public void SetOctaveAbove(IEnumerable<Channel> channels)
        {
            messages.SelectMany(x => x)
               .Where(m => m.GetType() == typeof(NoteMessage))
               .Cast<NoteMessage>()
               .Where(m => channels.Contains(m.Channel))
               .ToList()
               .ForEach(m =>
               {
                   if (m.Pitch + 12 <= Pitch.G9)
                       m.Pitch += 12;
               });
        }

        /// <summary>
        /// From all NoteMessages in messages, 
        ///    which have a channel equal to one of sourceChannels
        ///    drop each note an octave - unless it would drop it below Pitch.CNeg1 in which case it is ignored
        /// </summary>
        public void SetOctaveBelow(IEnumerable<Channel> channels)
        {
            messages.SelectMany(x => x)
               .Where(m => m.GetType() == typeof(NoteMessage))
               .Cast<NoteOnOffMessage>()
               .Where(m => channels.Contains(m.Channel))
               .ToList()
               .ForEach(m =>
               {
                   if (m.Pitch - 12 >= Pitch.CNeg1)
                       m.Pitch -= 12;
               });
        }

        /// <summary>
        /// From all NoteMessages in messages, 
        ///    which notes belong to the interval [min;max[
        ///    which have a channel equal to one of sourceChannels
        ///    in the order as listed by sourceChannels
        ///    set each the channel of each note to whatever channel the targetChannels.GetNext() returns
        /// </summary>
        public void MapNoteMessages(IEnumerable<Channel> sourceChannels, Enumerate<Channel> targetChannels, PitchRange range)
        {
            if (!sourceChannels.Any() || !targetChannels.ToList().Any() || (targetChannels.Length == 1 && targetChannels.First() == Channel.Channel10))
            {
                return;
            }

            var relevant = messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteMessage))
                .Cast<NoteMessage>();

            var relevantChannels = sourceChannels.Where(c => c != Channel.Channel10).ToList();
            foreach (var sourceChannel in relevantChannels)
            {
                relevant
                    .Where(m => m.Channel == sourceChannel && range.IsInside(m.Pitch))
                    .ToList()
                    .ForEach(m =>
                    {
                        var t = targetChannels.GetNext();
                        if (t != Channel.Channel10)
                        {
                            m.Channel = t;
                        }
                    });
            }
        }

        public void MapSourceChannelsToTargetChannel(IEnumerable<Channel> sourceChannels, Channel targetChannel, PitchRange range)
        {
            if (!sourceChannels.Any() || targetChannel == Channel.Channel10)
            {
                return;
            }

            var relevant = messages.SelectMany(x => x)
                .Where(m => m.GetType() == typeof(NoteMessage))
                .Cast<NoteMessage>();

            var relevantChannels = sourceChannels.Where(c => c != Channel.Channel10).ToList();
            foreach (var channel in relevantChannels)
            {
                relevant
                    .Where(m => m.Channel == channel && range.IsInside(m.Pitch))
                    .ToList()
                    .ForEach(m =>
                    {
                        if (targetChannel != Channel.Channel10)
                        {
                            m.Channel = targetChannel;
                        }
                    });
            }
        }

        /// <summary>Adds a message to the queue</summary>
        /// <param name="message"> The message to add to the queue</param>
        /// The message must have a valid timestamp (not MidiMessage.Now), but other than that there
        /// is no restriction on the timestamp. For example, it is legal to add a message with a
        /// timestamp earlier than some other message which was previously removed from the queue.
        /// Such a message would become the new "earliest" message, and so would be be the first
        /// message returned by PopEarliest().
        public void AddMessage(Message message)
        {
            // If the list is empty or message is later than any message we already have, we can add
            // this as a new timeslice to the end.
            if (IsEmpty || message.Time > messages.Last.Value[0].Time)
            {
                var timeslice = new List<Message>
                {
                    message
                };
                messages.AddLast(timeslice);
                return;
            }
            // We need to scan through the list to find where this should be inserted.
            LinkedListNode<List<Message>> node = messages.Last;
            while (node.Previous != null && node.Value[0].Time > message.Time)
            {
                node = node.Previous;
            }
            // At this point, node refers to a LinkedListNode which either has the correct
            // timestamp, or else a new timeslice needs to be added before or after node.
            if (node.Value[0].Time < message.Time)
            {
                messages.AddAfter(node, new List<Message> { message });
            }
            else if (node.Value[0].Time > message.Time)
            {
                messages.AddBefore(node, new List<Message> { message });
            }
            else
            {
                node.Value.Add(message);
            }
        }

        public void AddMessages(List<Message> messages)
        {
            messages.ForEach(message =>
            {
                AddMessage(message);
            });
        }

        public void AddTimeshiftedMessages(List<Message> messages, float beatTimeDelta)
        {
            messages.ForEach(message =>
            {
                AddMessage(message.MakeTimeShiftedCopy(beatTimeDelta));
            });
        }

        /// <summary>Discards all messages in the queue</summary>
        public void Clear() => messages.Clear();

        /// <summary>
        /// Removes and returns the message(s) in the queue that have the earliest timestamp.
        ///</summary>
        public List<Message> PopEarliest()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("queue is empty");
            }
            List<Message> result = messages.First.Value;
            messages.RemoveFirst();
            return result;
        }

        #region Assertions

        private static void AssertMinMax(double min, double max)
        {
            if (min > max)
            {
                throw new ArgumentException($"{nameof(min)} must be <= {nameof(max)}", nameof(min));
            }
        }

        private static void AssertMinMaxPitch(Pitch min, Pitch max)
        {
            if (min > max)
            {
                throw new ArgumentException($"{nameof(min)} must be <= {nameof(max)}", nameof(min));
            }
        }

        private static void AssertMinMaxPercussion(Percussion min, Percussion max)
        {
            if (min > max)
            {
                throw new ArgumentException($"{nameof(min)} must be <= {nameof(max)}", nameof(min));
            }
        }

        private static void AssertGreaterThanZero(float number, string parameterName = "number")
        {
            if (Math.Sign(number) <= 0 || float.IsNaN(number) || float.IsNegativeInfinity(number))
            {
                throw new ArgumentException($"{parameterName} must be > 0", parameterName);
            }
        }

        #endregion Assertions

        #endregion Methods
    }
}