using fractions.ui.viewmodels;
using System.Windows.Controls;

namespace fractions.ui.views;
/// <summary>
/// Interaction logic for FloatInterpolatorView.xaml
/// </summary>
public partial class FloatInterpolatorView : UserControl
{
    public FloatInterpolatorView()
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
    private FloatInterpolatorViewModel VM => (FloatInterpolatorViewModel)DataContext;
}
