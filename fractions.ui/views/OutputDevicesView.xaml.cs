using fractions.ui.viewmodels;
using System.Windows.Controls;

namespace fractions.ui.views;
/// <summary>
/// Interaction logic for OutputDevicesView.xaml
/// </summary>
public partial class OutputDevicesView : UserControl
{
    public OutputDevicesView()
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
    private OutputDevicesViewModel VM => (OutputDevicesViewModel)DataContext;
}