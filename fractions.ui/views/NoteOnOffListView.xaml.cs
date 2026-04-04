using fractions.ui.viewmodels;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Threading;

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
            VM.ListView = this;
            VM.IsLoaded = true;
        };
        Unloaded += (_, _) =>
        {
            VM.IsLoaded = false;
        };
        IsVisibleChanged += (_, e) =>
        {
            VM.IsVisible = (bool)e.NewValue;
        };
    }

    private NoteOnOffListViewModel VM
    {
        get
        {
            if (DataContext is NoteOnOffListViewModel noteOnOffListViewModel)
            {
                return noteOnOffListViewModel;
            }
            else if (DataContext is MainViewModel mainViewModel)
            {
                return mainViewModel.NoteOnOffListViewModel;
            }
            return null;
        }
    }

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
