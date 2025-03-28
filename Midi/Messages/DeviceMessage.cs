using System;

namespace fractions
{
    /// <summary>Base class for messages relevant to a specific device</summary>
    public abstract class DeviceMessage : Message
    {
        #region Constructors

        /// <summary>Protected constructor</summary>
        protected DeviceMessage(IDeviceBase device, float time, object tag = null)
            : base(time, tag)
        {
            Device = device ?? throw new ArgumentNullException(nameof(device));
        }

        #endregion Constructors

        #region Properties

        /// <summary>The device from which this message originated, or for which it is destined</summary>
        public IDeviceBase Device { get; }

        #endregion Properties
    }
}