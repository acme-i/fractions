using fractions.ui.viewmodels;
using System.Windows.Controls;

namespace fractions.ui.views;
/// <summary>
/// Interaction logic for FloatEnumeratorView.xaml
/// </summary>
public partial class FloatEnumeratorView : UserControl
{
    public FloatEnumeratorView()
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
    private FloatEnumeratorViewModel VM => (FloatEnumeratorViewModel)DataContext;
}