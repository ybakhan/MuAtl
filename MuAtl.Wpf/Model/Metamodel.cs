using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
namespace MuAtl.Model
{
  public class Metamodel : MuAtlModelBase, IComparable<Metamodel>
  {

    private ObservableCollection<Metamodel> mDependencies;
    public ObservableCollection<Metamodel> Dependencies
    {
      get
      {
        return mDependencies;
      }
      set
      {
        mDependencies = value;
        OnPropertyChanged("Dependencies");
      }
    }

    private string mHandler;
    public string Handler
    {
      get
      {
        return mHandler;
      }
      set
      {
        mHandler = value;
        OnPropertyChanged("Handler");
      }
    }

    public Metamodel()
    {
      Dependencies = new ObservableCollection<Metamodel>();
    }

    public Metamodel(Metamodel metamodel)
    {
      Name = metamodel.Name;
      Path = metamodel.Path;
      Handler = metamodel.Handler;
    }

    public override bool Equals(object obj)
    {
      var other = obj as Metamodel;
      if (other == null)
        return false;

      return Name.Equals(other.Name);
    }

    public int CompareTo(Metamodel other)
    {
      return Name.CompareTo(other.Name);
    }
  }
}
