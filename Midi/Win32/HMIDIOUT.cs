using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace fractions
{
    /// <summary>
    ///     Win32 handle for a MIDI output device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct HMIDIOUT
    {
        public IntPtr handle;
    }
}