using fractions.ui.viewmodels;
using System.Windows.Controls;

namespace fractions.ui.views;
/// <summary>
/// Interaction logic for NoteOnOffListView.xaml
/// </summary>
public partial class MidiFileSourceView : UserControl
{
    public MidiFileSourceView()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            VM.View = this;
            VM.IsLoaded = true;
        };
        Unloaded += (_, _) =>
        {
            VM.IsLoaded = false;
        };
        IsVisibleChanged += (o, e) =>
        {
            VM.IsVisible = (bool)e.NewValue;
        };
    }
    private MidiFileSourceViewModel VM => (MidiFileSourceViewModel)DataContext;
}
