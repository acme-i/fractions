using System;
using System.Collections.Generic;

namespace fractions
{
    /// <summary>
    ///     Extension methods for the Control enum.
    /// </summary>
    /// Be sure to "using midi" if you want to use these as extension methods.
    public static class ControlExtensions
    {
        #region Fields

        /// <summary>
        ///     Table of control names.
        /// </summary>
        private static readonly Dictionary<int, string> ControlNames = new Dictionary<int, string>
        {
            {1, "Modulation wheel"},
            {6, "Data Entry MSB"},
            {7, "Volume"},
            {10, "Pan"},
            {11, "Expression"},
            {38, "Data Entry LSB"},
            {64, "Sustain pedal"},
            {91, "Reverb level"},
            {92, "Tremolo level"},
            {93, "Chorus level"},
            {94, "Celeste level"},
            {95, "Phaser level"},
            {98, "Non-registered Parameter LSB"},
            {99, "Non-registered Parameter MSB"},
            {100, "Registered Parameter Number LSB"},
            {101, "Registered Parameter Number MSB"},
            {121, "All controllers off"},
            {123, "All notes off"}
        };

        #endregion Fields

        #region Methods

        public static Control Clamp(this Control value)
        {
            return (Control)Math.Min(Math.Max((int)value, (int)Control.MinControl), (int)Control.MaxControl);
        }

        /// <summary>
        ///     Returns true if the specified control is valid.
        /// </summary>
        /// <param name="control">The Control to test.</param>
        public static bool IsValid(this Control control) => control >= Control.MinControl && control <= Control.MaxControl;

        /// <summary>
        ///     Returns the human-readable name of a MIDI control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <exception cref="ArgumentOutOfRangeException">The control is out-of-range.</exception>
        public static string Name(this Control control)
        {
            control.Validate();
            if (ControlNames.ContainsKey((int)control))
            {
                return ControlNames[(int)control];
            }
            return "Other Control (see MIDI specification for details).";
        }

        /// <summary>
        ///     Throws an exception if control is not valid.
        /// </summary>
        /// <param name="control">The control to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">The control is out-of-range.</exception>
        public static void Validate(this Control control)
        {
            if (!control.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(control));
            }
        }

        #endregion Methods
    }
}