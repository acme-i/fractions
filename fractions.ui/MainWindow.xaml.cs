using fractions.ui.viewmodels;
using System.Windows;

namespace fractions.ui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel mainViewModel)
    {
        DataContext = mainViewModel;
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
    private MainViewModel VM => (MainViewModel)DataContext;
}