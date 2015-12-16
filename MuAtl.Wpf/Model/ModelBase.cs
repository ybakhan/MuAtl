using System.ComponentModel;

namespace MuAtl.Model
{
  public abstract class ModelBase : INotifyPropertyChanged
  {
    private string mName;
    public string Name
    {
      get
      {
        return mName;
      }
      set
      {
        mName = value;
        OnPropertyChanged("Name");
      }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string name)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(name));
      }
    }
  }

  public abstract class MuAtlModelBase : ModelBase
  {
    private string mPath;
    public virtual string Path
    {
      get
      {
        return mPath;
      }
      set
      {
        mPath = value;
        OnPropertyChanged("Path");
      }
    }
  }
}
