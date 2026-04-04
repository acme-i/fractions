using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;

namespace fractions.ui.viewmodels;

/// <summary>
/// ViewModel for displaying and selecting MIDI pitch values.
/// </summary>
public partial class PitchViewModel : BaseViewModel
{
    public PitchViewModel() : this(Pitch.C4) { }

    internal PitchViewModel(Pitch pitch) : base()
    {
        _pitch = pitch;
    }

    private Pitch _pitch;

    /// <summary>
    /// The selected pitch value.
    /// </summary>
    public Pitch Pitch
    {
        get => _pitch;
        set
        {
            if (_pitch != value)
            {
                NotifyPropertyChangingOnUiThread(nameof(Pitch));
                NotifyPropertyChangingOnUiThread(nameof(DisplayName));
                _pitch = value;
                NotifyPropertyChangedOnUiThread(nameof(Pitch));
                NotifyPropertyChangedOnUiThread(nameof(DisplayName));
            }
        }
    }

    /// <summary>
    /// Display-friendly name for the pitch (e.g., "C4 (Middle C)").
    /// </summary>
    public string DisplayName
    {
        get
        {
            return _pitch.ToString();
        }
    }

    /// <summary>
    /// Gets all available pitches as PitchViewModel instances for binding to a ComboBox.
    /// </summary>
    public IEnumerable<PitchViewModel> AllPitches
    {
        get
        {
            return Enum.GetValues<Pitch>()
                .Select(p => new PitchViewModel(p));
        }
    }

    /// <summary>
    /// Gets common pitches (88-key piano range: A♯0 to C8) for a smaller ComboBox list.
    /// </summary>
    public IEnumerable<PitchViewModel> CommonPitches
    {
        get
        {
            return Enum.GetValues<Pitch>()
                .Where(p => p >= Pitch.ASharp0 && p <= Pitch.C8)
                .Select(p => new PitchViewModel(p));
        }
    }
}