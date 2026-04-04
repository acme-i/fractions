using fractions.ui.viewmodels;
using System.Windows.Controls;

namespace fractions.ui.views;

/// <summary>
/// Interaction logic for EnumerateUserControl.xaml
/// </summary>
public partial class IntegerEnumeratorView : UserControl
{
    public IntegerEnumeratorView()
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
    private IntegerEnumeratorViewModel VM => (IntegerEnumeratorViewModel)DataContext;
}
