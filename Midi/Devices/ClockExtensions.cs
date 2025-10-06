using fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fractions
{
    public static class ClockExtensions
    {
        public static void Align(this Clock clock, IEnumerable<Channel> channels)
        {
            Align(clock, channels, PitchRange.Default);
        }

        public static void Align(this Clock clock, IEnumerable<Channel> channels, PitchRange range)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.Align(channels, range);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void Align(this Clock clock, float timeMin, float timeMax, IEnumerable<Channel> channels, PitchRange range)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.Align(timeMin, timeMax, channels, range);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void ApplyMelody(this Clock clock, Enumerate<Pitch> melodi, int maxRepeats = 2)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.ApplyMelody(Channels.InstrumentChannels, melodi, maxRepeats);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void Cleanup(this Clock clock)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.Cleanup();
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void Dampen(this Clock clock, List<Channel> channels, int newMin, int newMax, PitchRange range, int method = 1)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.InterpolateVelocity(channels, newMin, newMax, range, method);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void MapNoteMessages(this Clock clock, IEnumerable<Channel> sources, Enumerate<Channel> target, PitchRange range)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.MapNoteMessages(sources, target, range);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void MapSourceChannelsToTargetChannel(this Clock clock, IEnumerable<Channel> sourceChannels, Channel targetChannel, PitchRange range)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.MapSourceChannelsToTargetChannel(sourceChannels, targetChannel, range);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void MovePitchAbove(this Clock clock, IEnumerable<Channel> channels, Pitch above, Predicate<Pitch> shouldBeDeleted = null, Predicate<Pitch> shouldBeMoved = null)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.MovePitchAbove(channels, above, shouldBeDeleted, shouldBeMoved);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void MovePitchBelow(this Clock clock, IEnumerable<Channel> channels, Pitch above, Predicate<Pitch> shouldBeDeleted = null, Predicate<Pitch> shouldBeMoved = null)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.MovePitchBelow(channels, above, shouldBeDeleted, shouldBeMoved);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void RemoveIdenticalNotes(this Clock clock, PitchRange range)
        {
            clock.RemoveIdenticalNotes(Channels.All, range);
        }

        public static void RemoveIdenticalNotes(this Clock clock, IEnumerable<Channel> channels, PitchRange range)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.RemoveIdenticalNotes(channels, range);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void RemoveIdenticalPercussionNotes(this Clock clock, Percussion filterMin = Percussion.BassDrum2, Percussion filterMax = Percussion.OpenTriangle)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.RemoveIdenticalPercussionNotes(filterMin, filterMax);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void RemoveNotesLongerThan(this Clock clock, IEnumerable<Channel> channel, float duration)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.RemoveNotesLongerThan(channel, duration);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void RemoveNotesShorterThan(this Clock clock, IEnumerable<Channel> channel, float duration)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.RemoveNotesShorterThan(channel, duration);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void RemoveNotesAbove(this Clock clock, IEnumerable<Channel> channels, Pitch p)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.RemoveNotesAbove(channels, p);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void ScaleDurationAndTime(this Clock clock, float amount)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.ScaleDurationAndTime(Channels.InstrumentChannels, amount);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void ScaleDurationAndTime(this Clock clock, IEnumerable<Channel> channels, float amount)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.ScaleDurationAndTime(channels, amount);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void SetMinPitch(this Clock clock, Pitch minPitch)
        {
            clock.SetMinPitch(Channels.InstrumentChannels, minPitch);
        }

        public static void SetMinPitch(this Clock clock, IEnumerable<Channel> channels, Pitch minPitch)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.SetMinPitch(channels, minPitch);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void SetMaxPitch(this Clock clock, Pitch maxPitch)
        {
            clock.SetMaxPitch(Channels.InstrumentChannels, maxPitch);
        }

        public static void SetMaxPitch(this Clock clock, IEnumerable<Channel> channels, Pitch maxPitch)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.SetMaxPitch(channels, maxPitch);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void SetMinVelocity(this Clock clock, IEnumerable<Channel> channels, float min, PitchRange range)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.SetMinVelocity(channels, min, range);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void SetMaxVelocity(this Clock clock, IEnumerable<Channel> channels, float max, PitchRange range)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.SetMaxVelocity(channels, max, range);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void SetOctaveAbove(this Clock clock, IEnumerable<Channel> sources)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.SetOctaveAbove(sources);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void SetOctaveBelow(this Clock clock, IEnumerable<Channel> sources)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.SetOctaveBelow(sources);
                Monitor.Pulse(clock.threadLock);
            }
        }

        public static void ToPrecision(this Clock clock, float fraction)
        {
            clock.ToPrecision(Channels.All, fraction, PitchRange.Default);
        }

        public static void ToPrecision(this Clock clock, float fraction, PitchRange range)
        {
            clock.ToPrecision(Channels.All, fraction, range);
        }

        public static void ToPrecision(this Clock clock, IEnumerable<Channel> channels, float fraction, PitchRange range)
        {
            lock (clock.threadLock)
            {
                clock.threadMessageQueue.ToPrecision(channels, fraction, range);
                Monitor.Pulse(clock.threadLock);
            }
        }

    }
}
