using fractions.ui.viewmodels;
using System.Windows.Controls;

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
        IsVisibleChanged += (o, e) =>
        {
            VM.IsVisible = (bool)e.NewValue;
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
