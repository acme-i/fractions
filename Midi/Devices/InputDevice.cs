using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;

namespace fractions
{
    /// <summary>
    /// Midi input device
    /// </summary>
    public class InputDevice : DeviceBase, IInputDevice
    {
        #region Events

        /// <summary>Event called when an input device receives a Control Change message</summary>
        public event ControlChangeHandler ControlChange;

        /// <summary>Event called when an input device receives a Note Off message</summary>
        public event NoteOffHandler NoteOff;

        /// <summary>Event called when an input device receives a Note On message</summary>
        public event NoteOnHandler NoteOn;

        /// <summary>Event called when an input device receives a Pitch Bend message</summary>
        public event PitchBendHandler PitchBend;

        /// <summary>Event called when an input device receives a Program Change message</summary>
        public event ProgramChangeHandler ProgramChange;

        /// <summary>Event called when an input device receives a SysEx message</summary>
        public event SysExHandler SysEx;

        /// <summary>Event called when an input device receives a NrPn message</summary>
        public event NrpnHandler Nrpn;

        /// <summary>Removes all event handlers from the input events on this device</summary>
        public void RemoveAllEventHandlers()
        {
            NoteOn = null;
            NoteOff = null;
            ControlChange = null;
            ProgramChange = null;
            PitchBend = null;
        }

        #endregion Events

        /// <summary>
        ///     Internal Constructor, only called by the getter for the InstalledDevices property.
        /// </summary>
        /// <param name="deviceId">Position of this device in the list of all devices.</param>
        /// <param name="caps">Win32 Struct with device metadata</param>
        internal InputDevice(UIntPtr deviceId, MIDIINCAPS caps)
            : base(caps.szPname)
        {
            this.deviceId = deviceId;
            this.caps = caps;
            inputCallbackDelegate = new Win32API.MidiInProc(InputCallback);
            isOpen = false;
            clock = null;
            nrpnWatcher = new NrpnWatcher(this);
        }

        #region Public Methods and Properties

        /// <summary>List of input devices installed on this system</summary>
        public static ReadOnlyCollection<IInputDevice> InstalledDevices
        {
            get
            {
                lock (staticLock)
                {
                    if (installedDevices == null)
                    {
                        installedDevices = MakeDeviceList();
                    }
                    return new ReadOnlyCollection<IInputDevice>(installedDevices);
                }
            }
        }

        /// <summary>True if this device has been successfully opened</summary>
        public bool IsOpen
        {
            get
            {
                if (isInsideInputHandler)
                {
                    return true;
                }
                lock (this)
                {
                    return isOpen;
                }
            }
        }

        /// <summary>True if this device is receiving messages</summary>
        public bool IsReceiving
        {
            get
            {
                if (isInsideInputHandler)
                {
                    return true;
                }
                lock (this)
                {
                    return isReceiving;
                }
            }
        }

        /// <summary>Opens this input device</summary>
        /// <exception cref="InvalidOperationException">The device is already open.</exception>
        /// <exception cref="DeviceException">The device cannot be opened.</exception>
        /// <remarks>
        /// Note that Open() establishes a connection to the device, but no messages will be
        /// received until <see cref="StartReceiving" /> is called.
        /// </remarks>
        public void Open()
        {
            if (isInsideInputHandler)
            {
                throw new InvalidOperationException("Device is open.");
            }
            lock (this)
            {
                CheckNotOpen();
                CheckReturnCode(Win32API.midiInOpen(out handle, deviceId, inputCallbackDelegate, (UIntPtr)0));
                isOpen = true;
            }
        }

        /// <summary>Closes this input device</summary>
        /// <exception cref="InvalidOperationException">
        /// The device is not open or is still receiving.
        ///</exception>
        /// <exception cref="DeviceException">The device cannot be closed.</exception>
        public void Close()
        {
            if (isInsideInputHandler)
            {
                throw new InvalidOperationException("Device is receiving.");
            }
            lock (this)
            {
                CheckOpen();

                isClosing = true;
                if (longMsgBuffers.Count > 0)
                {
                    CheckReturnCode(Win32API.midiInReset(handle));
                }
                //Destroy any Long Message buffers we created when opening this device.
                //foreach (IntPtr buffer in LongMsgBuffers)
                //{
                //    if (DestroyLongMsgBuffer(buffer))
                //    {
                //        LongMsgBuffers.Remove(buffer);
                //    }
                //}

                CheckReturnCode(Win32API.midiInClose(handle));
                isOpen = false;
            }
        }

        /// <summary>Starts this input device receiving messages</summary>
        /// <param name="clock">
        /// If non-null, the clock's <see cref="Clock.Time" /> property will be used to assign a
        /// timestamp to each incoming message. If null, timestamps will be in seconds since
        /// StartReceiving() was called.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     The device is not open or is already receiving.
        ///</exception>
        /// <exception cref="DeviceException">
        ///     The device cannot start receiving.
        /// </exception>
        /// <remarks>
        /// <para>
        ///     This method launches a background thread to listen for input events, and as events are
        ///     received, the event handlers are invoked on that background thread. Event handlers
        ///     should be written to work from a background thread. (For example, if they want to update
        ///     the GUI, they may need to BeginInvoke to arrange for GUI updates to happen on the
        ///     correct thread.)
        /// </para>
        /// <para>
        ///     The background thread which is created by this method is joined (shut down) in
        ///     <see cref="StopReceiving" />.
        /// </para>
        /// </remarks>
        public void StartReceiving(Clock clock)
        {
            StartReceiving(clock, false);
        }

        /// <summary>
        ///     Starts this input device receiving messages
        /// </summary>
        /// <param name="clock">
        ///     If non-null, the clock's <see cref="Clock.Time" /> property will be used to assign a
        ///     timestamp to each incoming message. If null, timestamps will be in seconds since
        ///     StartReceiving() was called.
        /// </param>
        /// <param name="handleSysEx"></param>
        public void StartReceiving(Clock clock, bool handleSysEx)
        {
            if (isInsideInputHandler)
            {
                throw new InvalidOperationException("Device is receiving.");
            }
            lock (this)
            {
                CheckOpen();
                CheckNotReceiving();

                if (handleSysEx)
                {
                    longMsgBuffers.Add(CreateLongMsgBuffer());
                }

                CheckReturnCode(Win32API.midiInStart(handle));
                isReceiving = true;
                this.clock = clock;
            }
        }

        /// <summary>
        ///     Stops this input device from receiving messages
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method waits for all in-progress input event handlers to finish, and then joins
        /// (shuts down) the background thread that was created in <see cref="StartReceiving" />.
        /// Thus, when this function returns you can be sure that no more event handlers will be invoked.
        /// </para>
        /// <para>
        /// It is illegal to call this method from an input event handler (ie, from the background
        /// thread), and doing so throws an exception. If an event handler really needs to call this
        /// method, consider using BeginInvoke to schedule it on another thread.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// The device is not open; is not receiving; or called from within an event handler (ie,
        /// from the background thread).
        ///</exception>
        /// <exception cref="DeviceException">The device cannot start receiving.</exception>
        public void StopReceiving()
        {
            if (isInsideInputHandler)
            {
                throw new InvalidOperationException("Can't call StopReceiving() from inside an input handler.");
            }
            lock (this)
            {
                CheckReceiving();
                CheckReturnCode(Win32API.midiInStop(handle));
                clock = null;
                isReceiving = false;
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
                rc = Win32API.midiInGetErrorText(rc, errorMsg);
                if (rc != MMRESULT.MMSYSERR_NOERROR)
                {
                    throw new DeviceException("no error details");
                }
                throw new DeviceException(errorMsg.ToString());
            }
        }

        /// <summary>
        /// Private method for constructing the array of MidiInputDevices by calling the Win32 api.
        ///</summary>
        /// <returns></returns>
        private static IInputDevice[] MakeDeviceList()
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

        /// <summary>Throws a MidiDeviceException if this device is open</summary>
        private void CheckNotOpen()
        {
            if (isOpen)
            {
                throw new InvalidOperationException("Device is open.");
            }
        }

        /// <summary>Throws a MidiDeviceException if this device is receiving</summary>
        private void CheckNotReceiving()
        {
            if (isReceiving)
            {
                throw new DeviceException("device receiving");
            }
        }

        /// <summary>Throws a MidiDeviceException if this device is not open</summary>
        private void CheckOpen()
        {
            if (!isOpen)
            {
                throw new InvalidOperationException("Device is not open.");
            }
        }

        /// <summary>Throws a MidiDeviceException if this device is not receiving</summary>
        private void CheckReceiving()
        {
            if (!isReceiving)
            {
                throw new DeviceException("device not receiving");
            }
        }

        /// <summary>The input callback for midiOutOpen</summary>
        private void InputCallback(HMIDIIN hMidiIn, MidiInMessage wMsg, UIntPtr dwInstance, UIntPtr dwParam1, UIntPtr dwParam2)
        {
            isInsideInputHandler = true;
            try
            {
                switch (wMsg)
                {
                    case MidiInMessage.MIM_DATA:
                        HandleInputMimData(dwParam1, dwParam2);
                        break;

                    case MidiInMessage.MIM_LONGDATA:
                        HandleInputMimLongData(dwParam1, dwParam2);
                        break;

                    case MidiInMessage.MIM_MOREDATA:
                        SysEx?.Invoke(new SysExMessage(this, new byte[] { 0x13 }, 13));
                        break;

                    case MidiInMessage.MIM_OPEN:
                        //SysEx(new SysExMessage(this, new byte[] { 0x01 }, 1));
                        break;
                    case MidiInMessage.MIM_CLOSE:
                        //SysEx(new SysExMessage(this, new byte[] { 0x02 }, 2));
                        break;
                    case MidiInMessage.MIM_ERROR:
                        SysEx?.Invoke(new SysExMessage(this, new byte[] { 0x03 }, 3));
                        break;
                    case MidiInMessage.MIM_LONGERROR:
                        SysEx?.Invoke(new SysExMessage(this, new byte[] { 0x04 }, 4));
                        break;
                    default:
                        SysEx?.Invoke(new SysExMessage(this, new byte[] { 0x05 }, 5));
                        break;
                }

            }
            finally
            {
                isInsideInputHandler = false;
            }
        }

        private void HandleInputMimData(UIntPtr dwParam1, UIntPtr dwParam2)
        {
            Channel channel;
            Pitch pitch;
            int velocity;
            int value;
            uint win32Timestamp;
            if (ShortMsg.IsNoteOn(dwParam1, dwParam2))
            {
                if (NoteOn == null)
                    return;

                ShortMsg.DecodeNoteOn(dwParam1, dwParam2, out channel, out pitch, out velocity, out win32Timestamp);
                NoteOn(new NoteOnMessage(this, channel, pitch, velocity, clock?.Time ?? win32Timestamp / 1000f));
            }
            else if (ShortMsg.IsNoteOff(dwParam1, dwParam2))
            {
                if (NoteOff == null)
                    return;

                ShortMsg.DecodeNoteOff(dwParam1, dwParam2, out channel, out pitch, out velocity, out win32Timestamp);
                NoteOff(new NoteOffMessage(this, channel, pitch, velocity, clock?.Time ?? win32Timestamp / 1000f));
            }
            else if (ShortMsg.IsControlChange(dwParam1, dwParam2))
            {
                ShortMsg.DecodeControlChange(dwParam1, dwParam2, out channel, out Control control, out value, out win32Timestamp);
                var msg = new ControlChangeMessage(this, channel, control, value, clock?.Time ?? win32Timestamp / 1000f);
                nrpnWatcher.ReceivedControlChange(msg);
            }
            else if (ShortMsg.IsProgramChange(dwParam1, dwParam2))
            {
                if (ProgramChange == null)
                    return;

                ShortMsg.DecodeProgramChange(dwParam1, dwParam2, out channel, out Instrument instrument, out win32Timestamp);
                ProgramChange(new ProgramChangeMessage(this, channel, instrument, clock?.Time ?? win32Timestamp / 1000f));
            }
            else if (ShortMsg.IsPitchBend(dwParam1, dwParam2))
            {
                if (PitchBend == null)
                    return;

                ShortMsg.DecodePitchBend(dwParam1, dwParam2, out channel, out value, out win32Timestamp);
                PitchBend(new PitchBendMessage(this, channel, value, clock?.Time ?? win32Timestamp / 1000f));
            }
        }

        private void HandleInputMimLongData(UIntPtr dwParam1, UIntPtr dwParam2)
        {
            if (LongMsg.IsSysEx(dwParam1, dwParam2))
            {
                if (SysEx != null)
                {
                    LongMsg.DecodeSysEx(dwParam1, dwParam2, out byte[] data, out uint win32Timestamp);
                    if (data.Length != 0)
                    {
                        SysEx(new SysExMessage(this, data, clock?.Time ?? win32Timestamp / 1000f));
                    }

                    if (isClosing)
                    {
                        //buffers no longer needed
                        DestroyLongMsgBuffer(dwParam1);
                    }
                    else
                    {
                        //prepare the buffer for the next message
                        RecycleLongMsgBuffer(dwParam1);
                    }
                }
            }
        }


        private IntPtr CreateLongMsgBuffer()
        {
            //add a buffer so we can receive SysEx messages
            IntPtr ptr;
            var size = (uint)Marshal.SizeOf(typeof(MIDIHDR));
            var header = new MIDIHDR
            {
                lpData = Marshal.AllocHGlobal(4096),
                dwBufferLength = 4096,
                dwFlags = 0
            };

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

            CheckReturnCode(Win32API.midiInPrepareHeader(handle, ptr, size));
            CheckReturnCode(Win32API.midiInAddBuffer(handle, ptr, size));
            //CheckReturnCode(Win32API.midiInUnprepareHeader(handle, ptr, size));

            return ptr;
        }

        private void RecycleLongMsgBuffer(UIntPtr ptr)
        {
            var newPtr = ptr.ToIntPtr();
            var size = (uint)Marshal.SizeOf(typeof(MIDIHDR));
            CheckReturnCode(Win32API.midiInUnprepareHeader(handle, newPtr, size));
            CheckReturnCode(Win32API.midiInPrepareHeader(handle, newPtr, size));
            CheckReturnCode(Win32API.midiInAddBuffer(handle, newPtr, size));
            //return unchecked((UIntPtr)(ulong)(long)newPtr);
        }

        private void DestroyLongMsgBuffer(UIntPtr ptr)
        {
            var newPtr = ptr.ToIntPtr();
            var size = (uint)Marshal.SizeOf(typeof(MIDIHDR));
            CheckReturnCode(Win32API.midiInUnprepareHeader(handle, newPtr, size));

            var header = (MIDIHDR)Marshal.PtrToStructure(newPtr, typeof(MIDIHDR));
            Marshal.FreeHGlobal(header.lpData);
            Marshal.FreeHGlobal(newPtr);

            longMsgBuffers.Remove(newPtr);
        }

        #endregion Private Methods

        #region Private Classes

        private class NrpnWatcher
        {
            private readonly InputDevice device;
            private readonly Queue<ControlChangeMessage> messageQueue;
            private bool queueMessages;
            private Control lastControl;

            public NrpnWatcher(InputDevice device)
            {
                this.device = device;
                messageQueue = new Queue<ControlChangeMessage>(4);
            }

            public void ReceivedControlChange(ControlChangeMessage msg)
            {
                var control = msg.Control;

                if (queueMessages)
                {
                    // Check if messages are following the NRPN protocol
                    if (IsExpectedControl(control))
                    {
                        messageQueue.Enqueue(msg);

                        // When we have all four NRPN messages, we can assemble and send it
                        if (messageQueue.Count == 4)
                        {
                            SendNrpn();
                        }
                    }
                    else
                    {
                        // Messages cannot be a NRPN, release queue
                        ReleaseQueue();
                        SendControlChange(msg);
                    }
                }
                else if (msg.Control == Control.NonRegisteredParameterMSB)
                {
                    // Might be start of NRPN, start queueing messages
                    queueMessages = true;

                    messageQueue.Enqueue(msg);
                }
                else
                {
                    SendControlChange(msg);
                }


                lastControl = control;
            }

            private void ReleaseQueue()
            {
                while (messageQueue.Count > 0)
                {
                    var msg = messageQueue.Dequeue();
                    SendControlChange(msg);
                }

                queueMessages = false;
            }

            private void SendControlChange(ControlChangeMessage msg)
            {
                device.ControlChange?.Invoke(msg);
            }

            private void SendNrpn()
            {
                var nrpn = device.Nrpn;
                if (nrpn != null)
                {
                    var firstMsg = messageQueue.Peek();
                    var parameter = DequeueInt14();
                    var value = DequeueInt14();

                    var msg = new NrpnMessage(device, firstMsg.Channel, parameter.Value, value.Value, firstMsg.Time);
                    nrpn(msg);
                }
                else
                {
                    messageQueue.Clear();
                }

                // Stop queueing messages
                queueMessages = false;
            }

            private Int14 DequeueInt14()
            {
                if (messageQueue.Count < 2)
                    throw new InvalidOperationException(
                        $"Cannot construct 14-bit Integer from message queue, as there are only {messageQueue.Count} messages, needs at least 2");

                return new Int14
                {
                    MSB = messageQueue.Dequeue().Value,
                    LSB = messageQueue.Dequeue().Value
                };
            }

            private bool IsExpectedControl(Control control)
            {
                switch (control)
                {
                    case Control.NonRegisteredParameterLSB:
                        return lastControl == Control.NonRegisteredParameterMSB;
                    case Control.DataEntryMSB:
                        return lastControl == Control.NonRegisteredParameterLSB;
                    case Control.DataEntryLSB:
                        return lastControl == Control.DataEntryMSB;
                    default:
                        return false;
                }
            }
        }

        #endregion

        #region Private Fields

        private bool isClosing;

        private readonly List<IntPtr> longMsgBuffers = new List<IntPtr>();
        private readonly NrpnWatcher nrpnWatcher;

        private static IInputDevice[] installedDevices;

        /// <summary>
        /// Thread-local, set to true when called by an input handler, false in all other threads.
        ///</summary>
        [ThreadStatic]
        private static bool isInsideInputHandler;

        // Access to the global state is guarded by lock(staticLock).
        private static readonly object staticLock = new object();

        private MIDIINCAPS caps;

        private Clock clock;

        // These fields initialized in the constructor never change after construction, so they
        // don't need to be guarded by a lock. We keep a reference to the callback delegate because
        // we pass it to unmanaged code (midiInOpen) and unmanaged code cannot prevent the garbage
        // collector from collecting the delegate.
        private readonly UIntPtr deviceId;

        private HMIDIIN handle;

        private readonly Win32API.MidiInProc inputCallbackDelegate;

        // Access to the Open/Close state is guarded by lock(this).
        private bool isOpen;

        private bool isReceiving;

        #endregion Private Fields
    }

    #region Delegates

    /// <summary>Delegate called when an input device receives a Control Change message</summary>
    public delegate void ControlChangeHandler(ControlChangeMessage msg);

    /// <summary>Delegate called when an input device receives a Note Off message
    /// or when a Note Off message is just about to be sent to the output device
    /// or when a Note Off message has just been sent to the output device</summary>
    public delegate void NoteOffHandler(NoteOffMessage msg);

    /// <summary>Delegate called when an input device receives a Note On message
    ///  or when a Note On message is just about to be sent to the output device
    ///  or when a Note On message has just been sent to the output device</summary>
    public delegate void NoteOnHandler(NoteOnMessage msg);

    /// <summary>Delegate called before and after sending NoteOnOffMessage to the output device</summary>
    public delegate void NoteOnOffHandler(NoteOnOffMessage msg);

    /// <summary>Delegate called before and after sending PercussionMessage to the output device</summary>
    public delegate void PercussionHandler(PercussionMessage msg);

    /// <summary>Delegate called when an input device receives a Pitch Bend message</summary>
    public delegate void PitchBendHandler(PitchBendMessage msg);

    /// <summary>Delegate called when an input device receives a Program Change message</summary>
    public delegate void ProgramChangeHandler(ProgramChangeMessage msg);

    /// <summary>Delegate called when an input device receives a SysEx message</summary>
    public delegate void SysExHandler(SysExMessage msg);

    /// <summary>Delegate called when an input device receives a NrPn message</summary>
    public delegate void NrpnHandler(NrpnMessage msg);

    #endregion Delegates

}