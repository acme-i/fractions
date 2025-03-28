using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace fractions
{
    /// <summary>
    ///     Win32 handle for a MIDI input device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct HMIDIIN
    {
        public IntPtr handle;
    }
}