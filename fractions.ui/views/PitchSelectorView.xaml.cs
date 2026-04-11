using fractions.ui.viewmodels;
using System.Windows.Controls;

namespace fractions.ui.views;
/// <summary>
/// Interaction logic for PitchView.xaml
/// </summary>
public partial class PitchSelectorView : UserControl
{
    public PitchSelectorView()
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
    private PitchViewModel VM => (PitchViewModel)DataContext;
}