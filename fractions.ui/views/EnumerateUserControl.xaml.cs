using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Messaging;

namespace fractions.ui;

/// <summary>
/// Interaction logic for EnumerateUserControl.xaml
/// </summary>
public partial class EnumerateUserControl : UserControl
{
    private readonly IMessenger _messenger;

    public EnumerateUserControl(IMessenger messenger)
    {
        InitializeComponent();
        _messenger = messenger;

    }
}
