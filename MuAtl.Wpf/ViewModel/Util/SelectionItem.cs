using System;
using System.ComponentModel;

namespace MuAtl.ViewModel.Util
{
  public class SelectionItem<T> : INotifyPropertyChanged
  {
    #region Fields

    private bool isSelected;

    private T item;

    #endregion

    #region Properties

    public bool IsSelected
    {
      get { return isSelected; }
      set
      {
        if (value == isSelected) return;
        isSelected = value;
        OnPropertyChanged("IsSelected");
        OnSelectionChanged();
      }
    }

    public T Item
    {
      get { return item; }
      set
      {
        if (value.Equals(item)) return;
        item = value;
        OnPropertyChanged("Item");
      }
    }

    #endregion

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    public event EventHandler SelectionChanged;

    #endregion

    #region ctor

    public SelectionItem(T item)
      : this(false, item)
    {
    }

    public SelectionItem(bool selected, T item)
    {
      this.isSelected = selected;
      this.item = item;
    }

    #endregion

    #region Event invokers

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler changed = PropertyChanged;
      if (changed != null) changed(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnSelectionChanged()
    {
      EventHandler changed = SelectionChanged;
      if (changed != null) changed(this, EventArgs.Empty);
    }

    public override bool Equals(object obj)
    {
      var other = obj as SelectionItem<T> ;
      if (other == null)
        return false;

      return Item.Equals(other.Item);
    }

    #endregion
  }
}
