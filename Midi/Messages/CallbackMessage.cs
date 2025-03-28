using System;

namespace fractions
{
    /// <summary>
    ///     Pseudo-MIDI message used to arrange for a callback at a certain time.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This message can be scheduled with
    ///         <see cref="Clock.Schedule(Message)">
    ///             Clock.Schedule
    ///         </see>
    ///         just like any other message.  When its time comes and it
    ///         gets "sent", it invokes the callback provided in the constructor.
    ///     </para>
    ///     <para>
    ///         The idea is that you can embed callback points into the music you've
    ///         scheduled, so that (if the clock gets to that point in the music) your code has
    ///         an opportunity for some additional processing.
    ///     </para>
    ///     <para>The callback is invoked on the MidiOutputDevice's worker thread.</para>
    /// </remarks>
    public class CallbackMessage : Message
    {
        #region Delegates

        /// <summary>
        ///     Delegate called when a CallbackMessage is sent.
        /// </summary>
        /// <param name="time">The time at which this event was scheduled.</param>
        /// <returns>
        ///     Additional messages which should be scheduled as a result of this callback,
        ///     or null.
        /// </returns>
        public delegate void CallbackType(float time);

        #endregion Delegates

        #region Constructors

        /// <summary>
        ///     Constructs a Callback message.
        /// </summary>
        /// <param name="callback">The callback to invoke when this message is "sent".</param>
        /// <param name="time">The timestamp for this message.</param>
        public CallbackMessage(CallbackType callback, float time)
            : base(time)
        {
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///     The callback to invoke when this message is "sent".
        /// </summary>
        public CallbackType Callback { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///     Returns a copy of this message, shifted in time by the specified amount.
        /// </summary>
        public override Message MakeTimeShiftedCopy(float delta) =>
            new CallbackMessage(Callback, Time + delta);

        /// <summary>
        ///     Sends this message immediately, ignoring the beatTime.
        /// </summary>
        public override void SendNow() => Callback(Time);

        #endregion Methods
    }
}