using fractions.ui.viewmodels;
using System.Windows.Controls;

namespace fractions.ui.views;
/// <summary>
/// Interaction logic for DoubleInterpolatorView.xaml
/// </summary>
public partial class DoubleInterpolatorView : UserControl
{
    public DoubleInterpolatorView()
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
    private DoubleInterpolatorViewModel VM => (DoubleInterpolatorViewModel)DataContext;
}
