using fractions.ui.viewmodels;
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

namespace fractions.ui.views;
/// <summary>
/// Interaction logic for NoteOnOffListView.xaml
/// </summary>
public partial class NoteOnOffListView : UserControl, IListView<NoteOnOffViewModel>
{
    public NoteOnOffListView()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            VM.View = this;
        };
    }

    private NoteOnOffListViewModel VM => (NoteOnOffListViewModel)DataContext;

    public void SelectAll()
    {
        dataGrid.SelectAll();
    }

    public IList<NoteOnOffViewModel> GetSelectedItems()
    {
        var list = dataGrid.SelectedItems.Cast<NoteOnOffViewModel>().ToList();
        return list;
    }

    public int SelectedIndex
    {
        get => dataGrid.SelectedIndex;
        set
        {
            VM.LastException = null;

            try
            {
                dataGrid.SelectedIndex = Math.Min(value, dataGrid.Items.Count - 1);
                dataGrid.Focus();
            }
            catch (Exception ex)
            {
                VM.LastException = ex;
            }
        }
    }

    public int SelectedItemsCount => dataGrid.SelectedItems.Count;
}
