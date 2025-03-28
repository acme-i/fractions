using System;
using System.Collections.ObjectModel;

namespace fractions
{
    /// <summary>
    ///     MIDI Device Manager, providing access to available input/output midi devices
    /// </summary>
    public static class DeviceManager
    {
        private static readonly object InputDeviceLock = new object();
        private static readonly object OutputDeviceLock = new object();
        private static IInputDevice[] inputDevices;
        private static IOutputDevice[] outputDevices;

        /// <summary>
        ///     List of input devices installed on this system.
        /// </summary>
        public static ReadOnlyCollection<IInputDevice> InputDevices
        {
            get
            {
                if (inputDevices == null)
                {
                    lock (InputDeviceLock)
                    {
                        if (inputDevices == null)
                        {
                            var inputDevices = EnumerateInputDevices();
                            DeviceManager.inputDevices = inputDevices;
                        }
                    }
                }
                return new ReadOnlyCollection<IInputDevice>(inputDevices);
            }
        }

        /// <summary>
        ///     List of devices installed on this system.
        /// </summary>
        public static ReadOnlyCollection<IOutputDevice> OutputDevices
        {
            get
            {
                if (outputDevices == null)
                {
                    lock (OutputDeviceLock)
                    {
                        if (outputDevices == null)
                        {
                            var outputDevices = EnumerateOutputDevices();
                            DeviceManager.outputDevices = outputDevices;
                        }
                    }
                }
                return new ReadOnlyCollection<IOutputDevice>(outputDevices);
            }
        }

        /// <summary>
        ///     Refresh the list of input devices
        /// </summary>
        public static void UpdateInputDevices()
        {
            lock (InputDeviceLock)
            {
                inputDevices = null;
            }
        }

        /// <summary>
        ///     Refresh the list of input devices
        /// </summary>
        public static void UpdateOutputDevices()
        {
            lock (OutputDeviceLock)
            {
                outputDevices = null;
            }
        }

        private static IInputDevice[] EnumerateInputDevices()
        {
            var inDevs = Win32API.midiInGetNumDevs();
            var result = new IInputDevice[inDevs];
            for (uint deviceId = 0; deviceId < inDevs; deviceId++)
            {
                Win32API.midiInGetDevCaps((UIntPtr)deviceId, out MIDIINCAPS caps);
                result[deviceId] = new InputDevice((UIntPtr)deviceId, caps);
            }
            return result;
        }

        private static IOutputDevice[] EnumerateOutputDevices()
        {
            var outDevs = Win32API.midiOutGetNumDevs();
            var result = new IOutputDevice[outDevs];
            for (uint deviceId = 0; deviceId < outDevs; deviceId++)
            {
                Win32API.midiOutGetDevCaps((UIntPtr)deviceId, out MIDIOUTCAPS caps);
                result[deviceId] = new OutputDevice((UIntPtr)deviceId, caps);
            }
            return result;
        }
    }
}