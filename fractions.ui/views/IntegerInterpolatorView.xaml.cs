using fractions.ui.viewmodels;
using System.Windows.Controls;

namespace fractions.ui.views;
/// <summary>
/// Interaction logic for IntegerInterpolatorView.xaml
/// </summary>
public partial class IntegerInterpolatorView : UserControl
{
    public IntegerInterpolatorView()
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
    private IntegerInterpolatorViewModel VM => (IntegerInterpolatorViewModel)DataContext;
}
