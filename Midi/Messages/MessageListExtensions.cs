using System.Collections.Generic;

namespace fractions
{
    public static class MessageListExtensions
    {
        /// <summary>
        /// Sends all messages immediately, if any
        /// </summary>
        /// <param name="messages">messages to send</param>
        public static void SendNow(this List<Message> messages)
        {
            messages?.ForEach(message =>
            {
                message?.SendNow();
            });
        }

        public static List<Message> AddTimeShiftedOffCopies(this List<Message> noteOnMessages, float delta)
        {
            if (noteOnMessages != null)
            {
                foreach(var message in noteOnMessages)
                {
                    if (message is NoteOnMessage note)
                    {
                        noteOnMessages.Add(note.MakeTimeShiftedOffCopy(delta));
                    }
                }
            }
            return noteOnMessages;
        }
    }

    public static class NoteMessageListExtensions
    {
        public static void SetOctaveAbove<T>(this List<T> messages)
            where T : NoteMessage
        {
            messages?.ForEach(message =>
            {
                message?.SetOctaveAbove();
            });
        }

        public static void SetOctaveBelow<T>(this List<T> messages)
            where T : NoteMessage
        {
            messages?.ForEach(message =>
            {
                message?.SetOctaveBelow();
            });
        }
    }
}
