using System.Collections.Generic;

namespace fractions
{
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
