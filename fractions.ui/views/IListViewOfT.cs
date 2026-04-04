namespace fractions.ui.views;

public interface IListView<T>
    where T : class
{
    void SelectAll();
    IList<T> GetSelectedItems();
    int SelectedIndex { get; set; }
    int SelectedItemsCount { get; }
}