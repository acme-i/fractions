using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fractions.ui.views;

public interface IListView<T>
    where T : class
{
    void SelectAll();
    IList<T> GetSelectedItems();
    int SelectedIndex { get; set; }
    int SelectedItemsCount { get; }
}