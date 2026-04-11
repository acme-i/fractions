using fractions.ui.viewmodels;
using System.Windows.Controls;

namespace fractions.ui.views;
/// <summary>
/// Interaction logic for ChannelSelectorView.xaml
/// </summary>
public partial class ChannelSelectorView : UserControl
{
    public ChannelSelectorView()
    {
        InitializeComponent();
        //Loaded += (_, _) =>
        //{
        //    VM.View = this;
        //    VM.IsLoaded = true;
        //};
        //Unloaded += (_, _) =>
        //{
        //    VM.IsLoaded = false;
        //};
        //IsVisibleChanged += (o, e) =>
        //{
        //    VM.IsVisible = (bool)e.NewValue;
        //};
    }
    private ChannelViewModel VM => (ChannelViewModel)DataContext;
}
