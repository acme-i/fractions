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
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;

namespace fractions
{
    /// <summary>A MIDI output device</summary>
    /// <remarks>
    /// <para>
    /// Each instance of this class describes a MIDI output device installed on the system. You
    /// cannot create your own instances, but instead must go through the
    /// <see cref="InstalledDevices" /> property to find which devices are available. You may wish
    /// to examine the <see cref="DeviceBase.Name" /> property of each one and present the user with
    /// a choice of which device to use.
    /// </para>
    /// <para>
    /// Open an output device with <see cref="Open" /> and close it with <see cref="Close" />. While
    /// it is open, you may send MIDI messages with functions such as <see cref="SendNoteOn" />,
    /// <see cref="SendNoteOff" /> and <see cref="SendProgramChange" />. All notes may be silenced
    /// on the device by calling <see cref="SilenceAllNotes" />.
    /// </para>
    /// <para>
    /// Note that the above methods send their messages immediately. If you wish to arrange for a
    /// message to be sent at a specific future time, you'll need to instantiate some subclass of
    /// <see cref="Message" /> (eg <see cref="NoteOnMessage" />) and then pass it to
    /// <see cref="Clock.Schedule(Message)"> Clock.Schedule </see>.
    /// </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// <seealso cref="Clock" />
    /// <seealso cref="InputDevice" />
    public class OutputDevice : DeviceBase, IOutputDevice
    {
        #region Private Fields

        private static List<IOutputDevice> installedDevices = null;

        /// <summary>
        /// Access to the global state is guarded by lock(staticLock).
        /// </summary>
        private static readonly object staticLock = new object();

        /// <summary>
        /// Access to the local state is guarded by lock(objectLock).
        /// </summary>
        private readonly object objectLock = new object();

        private MIDIOUTCAPS caps;

        /// <summary>
        /// The fields initialized in the constructor never change after construction, so they don't need to be guarded by a lock.
        /// </summary>
        private readonly UIntPtr deviceId;

        private HMIDIOUT handle;

        /// <summary>
        /// Access to the Open/Close state is guarded by lock(objectLock).
        /// </summary>
        private bool isOpen;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Private Constructor, only called by the getter for the InstalledDevices property.
        ///</summary>
        /// <param name="deviceId"> Position of this device in the list of all devices</param>
        /// <param name="caps">Win32 Struct with device metadata</param>
        internal OutputDevice(UIntPtr deviceId, MIDIOUTCAPS caps) : base(caps.szPname)
        {
            this.deviceId = deviceId;
            this.caps = caps;
            isOpen = false;
        }

        #endregion

        #region Public Methods and Properties

        /// <summary>List of devices installed on this system</summary>
        public static ReadOnlyCollection<IOutputDevice> InstalledDevices
        {
            get
            {
                lock (staticLock)
                {
                    if (installedDevices == null)
                    {
                        installedDevices = MakeDeviceList();
                    }
                    return new ReadOnlyCollection<IOutputDevice>(installedDevices);
                }
            }
        }

        /// <summary>True if this device is open</summary>
        public bool IsOpen
        {
            get
            {
                lock (objectLock)
                {
                    return isOpen;
                }
            }
        }

        /// <summary>Closes this output device</summary>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The device cannot be closed.</exception>
        public void Close()
        {
            lock (objectLock)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutClose(handle));
                isOpen = false;
            }
        }

        /// <summary>Opens this output device</summary>
        /// <exception cref="InvalidOperationException">The device is already open.</exception>
        /// <exception cref="DeviceException">The device cannot be opened.</exception>
        public void Open()
        {
            lock (objectLock)
            {
                CheckNotOpen();
                CheckReturnCode(Win32API.midiOutOpen(out handle, deviceId, null, (UIntPtr)0));
                isOpen = true;
            }
        }

        /// <summary>Sends a Control Change message to this MIDI output device</summary>
        /// <param name="channel"> The channel</param>
        /// <param name="control"> The control</param>
        /// <param name="value"> The new value 0..127</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// channel, control, or value is out-of-range.
        ///</exception>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SendControlChange(Channel channel, Control control, int value)
        {
            lock (objectLock)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutShortMsg(handle, ShortMsg.EncodeControlChange(channel, control, ClampControlChange(value))));
            }
        }

        /// <summary>Sends a Note Off message to this MIDI output device</summary>
        /// <param name="channel"> The channel</param>
        /// <param name="pitch"> The pitch</param>
        /// <param name="velocity"> The velocity 0..127</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// channel, note, or velocity is out-of-range.
        ///</exception>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SendNoteOff(Channel channel, Pitch pitch, int velocity)
        {
            lock (objectLock)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutShortMsg(handle, ShortMsg.EncodeNoteOff(channel, pitch, ClampControlChange(velocity))));
            }
        }

        /// <summary>Sends a Note On message to this MIDI output device</summary>
        /// <param name="channel"> The channel</param>
        /// <param name="pitch"> The pitch</param>
        /// <param name="velocity"> The velocity 0..127</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// channel, pitch, or velocity is out-of-range.
        ///</exception>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SendNoteOn(Channel channel, Pitch pitch, int velocity)
        {
            lock (objectLock)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutShortMsg(handle, ShortMsg.EncodeNoteOn(channel, pitch, ClampControlChange(velocity))));
            }
        }

        /// <summary>Sends a Note On message to Channel10 of this MIDI output device</summary>
        /// <param name="percussion"> The percussion</param>
        /// <param name="velocity"> The velocity 0..127</param>
        /// <remarks>
        /// This is simply shorthand for a Note On message on Channel10 with a percussion-specific
        /// note, so there is no corresponding message to receive from an input device.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">percussion or velocity is out-of-range.</exception>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SendPercussion(Percussion percussion, int velocity)
        {
            lock (objectLock)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutShortMsg(handle, ShortMsg.EncodeNoteOn(Channel.Channel10, (Pitch)percussion, ClampControlChange(velocity))));
            }
        }

        /// <summary>Sends a Pitch Bend message to this MIDI output device</summary>
        /// <param name="channel"> The channel</param>
        /// <param name="value"> The pitch bend value, 0..16383, 8192 is centered</param>
        /// <exception cref="ArgumentOutOfRangeException">channel or value is out-of-range.</exception>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SendPitchBend(Channel channel, int value)
        {
            lock (objectLock)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutShortMsg(handle, ShortMsg.EncodePitchBend(channel, ClampPitchBend(value))));
            }
        }

        /// <summary>Sends a Program Change message to this MIDI output device</summary>
        /// <param name="channel"> The channel</param>
        /// <param name="instrument"> The instrument</param>
        /// <exception cref="ArgumentOutOfRangeException">channel or instrument is out-of-range.</exception>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        /// <remarks>
        /// A Program Change message is used to switch among instrument settings, generally
        /// instrument voices. An instrument conforming to General Midi 1 will have the instruments
        /// described in the <see cref="Instrument" /> enum; other instruments may have different
        /// instrument sets.
        /// </remarks>
        public void SendProgramChange(Channel channel, Instrument instrument)
        {
            lock (objectLock)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutShortMsg(handle, ShortMsg.EncodeProgramChange(channel, instrument)));
            }
        }

        public void SendSysEx(byte[] data)
        {
            lock (objectLock)
            {
                //Win32API.MMRESULT result;
                IntPtr ptr;
                var size = (uint)Marshal.SizeOf(typeof(MIDIHDR));
                var header = new MIDIHDR { lpData = Marshal.AllocHGlobal(data.Length) };
                for (var i = 0; i < data.Length; i++)
                    Marshal.WriteByte(header.lpData, i, data[i]);
                header.dwBufferLength = data.Length;
                header.dwBytesRecorded = data.Length;
                header.dwFlags = 0;

                try
                {
                    ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MIDIHDR)));
                }
                catch (Exception)
                {
                    Marshal.FreeHGlobal(header.lpData);
                    throw;
                }

                try
                {
                    Marshal.StructureToPtr(header, ptr, false);
                }
                catch (Exception)
                {
                    Marshal.FreeHGlobal(header.lpData);
                    Marshal.FreeHGlobal(ptr);
                    throw;
                }

                //result = Win32API.midiOutPrepareHeader(handle, ptr, size);
                //if (result == 0) result = Win32API.midiOutLongMsg(handle, ptr, size);
                //if (result == 0) result = Win32API.midiOutUnprepareHeader(handle, ptr, size);
                CheckReturnCode(Win32API.midiOutPrepareHeader(handle, ptr, size));
                CheckReturnCode(Win32API.midiOutLongMsg(handle, ptr, size));
                CheckReturnCode(Win32API.midiOutUnprepareHeader(handle, ptr, size));

                Marshal.FreeHGlobal(header.lpData);
                Marshal.FreeHGlobal(ptr);
            }
        }

        public void SendNrpn(Channel channel, int parameter, int value)
        {
            lock (objectLock)
            {
                CheckOpen();

                var parameter14 = new Int14(parameter);
                var value14 = new Int14(value);

                // Parameter, MSB
                CheckReturnCode(Win32API.midiOutShortMsg(handle, ShortMsg.EncodeControlChange(channel, Control.NonRegisteredParameterMSB, parameter14.MSB)));

                // Parameter, LSB
                CheckReturnCode(Win32API.midiOutShortMsg(handle, ShortMsg.EncodeControlChange(channel, Control.NonRegisteredParameterLSB, parameter14.LSB)));

                // Value, MSB
                CheckReturnCode(Win32API.midiOutShortMsg(handle, ShortMsg.EncodeControlChange(channel, Control.DataEntryMSB, value14.MSB)));

                // Value, LSB
                CheckReturnCode(Win32API.midiOutShortMsg(handle, ShortMsg.EncodeControlChange(channel, Control.DataEntryLSB, value14.LSB)));
            }
        }

        /// <summary>Silences all notes on this output device</summary>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SilenceAllNotes()
        {
            lock (objectLock)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutReset(handle));
            }
        }

        #endregion Public Methods and Properties

        #region Private Methods

        /// <summary>
        /// Makes sure rc is MidiWin32Wrapper.MMSYSERR_NOERROR. If not, throws an exception with an
        /// appropriate error message.
        ///</summary>
        /// <param name="rc"></param>
        private static void CheckReturnCode(MMRESULT rc)
        {
            if (rc != MMRESULT.MMSYSERR_NOERROR)
            {
                var errorMsg = new StringBuilder(128);
                rc = Win32API.midiOutGetErrorText(rc, errorMsg);
                if (rc != MMRESULT.MMSYSERR_NOERROR)
                {
                    throw new DeviceException("no error details");
                }
                throw new DeviceException(errorMsg.ToString());
            }
        }

        /// <summary>
        /// Private method for constructing the array of MidiOutputDevices by calling the Win32 api.
        ///</summary>
        /// <returns></returns>
        private static List<IOutputDevice> MakeDeviceList()
        {
            var outDevs = Win32API.midiOutGetNumDevs();
            var result = new List<IOutputDevice>((int)outDevs);
            for (uint deviceId = 0; deviceId < outDevs; deviceId++)
            {
                Win32API.midiOutGetDevCaps((UIntPtr)deviceId, out MIDIOUTCAPS caps);
                result.Add(new OutputDevice((UIntPtr)deviceId, caps));
            }
            return result;
        }

        /// <summary>Throws a MidiDeviceException if this device is open</summary>
        private void CheckNotOpen()
        {
            if (isOpen)
            {
                throw new InvalidOperationException("device open");
            }
        }

        /// <summary>Throws a MidiDeviceException if this device is not open</summary>
        private void CheckOpen()
        {
            if (!isOpen)
            {
                throw new InvalidOperationException("device not open");
            }
        }

        #endregion Private Methods

        #region Overrides

        /// <summary>
        /// Returns the name of the output device
        /// </summary>
        /// <returns>the Name</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}