// ReSharper disable InconsistentNaming

namespace fractions
{
    /// <summary>
    ///     Values for wTechnology field of MIDIOUTCAPS structure.
    /// </summary>
    internal enum MidiDeviceType : ushort
    {
        MOD_MIDIPORT = 1,
        MOD_SYNTH = 2,
        MOD_SQSYNTH = 3,
        MOD_FMSYNTH = 4,
        MOD_MAPPER = 5,
        MOD_WAVETABLE = 6,
        MOD_SWSYNTH = 7
    }
}