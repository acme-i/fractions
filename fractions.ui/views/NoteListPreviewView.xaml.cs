using fractions.ui.viewmodels;
using System.Windows.Controls;

namespace fractions.ui.views;

public partial class NoteListPreviewView : UserControl
{
    public NoteListPreviewView()
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
    private NoteOnOffListViewModel VM => (NoteOnOffListViewModel)DataContext;
}