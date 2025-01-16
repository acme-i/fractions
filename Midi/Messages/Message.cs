using System;
using System.Diagnostics;

namespace fractions
{
    /// <summary>Base class for all MIDI messages</summary>
    [DebuggerDisplay("Time = {Time}")]
    public abstract class Message : IEquatable<Message>
    {
        #region Constructors

        private readonly Guid id;

        /// <summary>Protected constructor</summary>
        /// <param name="time">The timestamp for this message</param>
        /// <param name="tag">User-defined object</param>
        protected Message(float time, object tag = null)
        {
            Time = time;
            Tag = tag;
            id = Guid.NewGuid();
        }

        #endregion Constructors

        #region Properties

        /// <summary>Milliseconds since the music started</summary>
        public float Time { get; set; }

        /// <summary>User object</summary>
        public object Tag { get; set; } = null;

        #endregion Properties

        #region Methods

        /// <summary>Returns a copy of this message, shifted in time by the specified amount</summary>
        public abstract Message MakeTimeShiftedCopy(float delta);

        /// <summary>Sends this message immediately</summary>
        public abstract void SendNow();

        #endregion Methods

        #region Object overrides

        public bool Equals(Message other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (id == other.id) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        #endregion
    }
}