using System.Text;
using System.Linq;
using System;
using System.Collections.ObjectModel;

namespace MuAtl.Model
{
  public  class TestCaseInput : MuAtlModelBase, IComparable<TestCaseInput>
  {
    public int CompareTo(TestCaseInput other)
    {
      return Name.CompareTo(other.Name);
    }

    public override string Path
    {
      get
      {
        return base.Path;
      }
      set
      {
        base.Path = value;
        if(InputPathChanged != null)
        {
          InputPathChanged();
        }
      }
    }

    public event TestCaseInputPathChangedEventHandler InputPathChanged;
  }

  public delegate void TestCaseInputPathChangedEventHandler();

  public class TestCaseOutput : MuAtlModelBase, IComparable<TestCaseOutput>
  {
    public int CompareTo(TestCaseOutput other)
    {
      return Name.CompareTo(other.Name);
    }
  }

  public class MuAtlTestCase : ModelBase, IComparable<MuAtlTestCase>
  {
    public MuAtlTestCase()
    {
      InModels = new ObservableCollection<TestCaseInput>();
      InModels.CollectionChanged += InModels_CollectionChanged;
      OutModel = new TestCaseOutput();
    }

    private void InModels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
      {
        foreach (var newItem in e.NewItems)
        {
          var testCase = newItem as TestCaseInput;
          testCase.InputPathChanged += TestCase_InputPathChanged;
        }
      }
    }

    private void TestCase_InputPathChanged()
    {
      InputDescription = GetDescription();
    }

    private string mModulePath;
    public string ModulePath
    {
      get
      {
        return mModulePath;
      }
      set
      {
        mModulePath = value;
        OnPropertyChanged("ModulePath");
      }
    }

    private ObservableCollection<TestCaseInput> mInModels;
    public ObservableCollection<TestCaseInput> InModels
    {
      get
      {
        return mInModels;
      }
      set
      {
        mInModels = value;
        OnPropertyChanged("InModels");
      }
    }

    private TestCaseOutput mOutModel;
    public TestCaseOutput OutModel
    {
      get
      {
        return mOutModel;
      }
      set
      {
        mOutModel = value;
        OnPropertyChanged("OutModel");
      }
    }

    private string mInputDescription;
    public string InputDescription
    {
      get
      {
        mInputDescription = GetDescription();
        return mInputDescription;
      }
      set
      {
        mInputDescription = value;
        OnPropertyChanged("InputDescription");
      }
    }

    private string GetDescription()
    {
      if (InModels == null)
        return "";

      var sb = new StringBuilder();
      foreach (var inModel in InModels)
      {
        sb.Append(inModel.Name);
        sb.Append(";");
        sb.Append(inModel.Path);
        sb.Append(";");
      }

      return sb.ToString();
    }

    public ObservableCollection<string> mInModelNames;
    public ObservableCollection<string> InModelNames
    {
      get
      {
        if (InModels == null)
          return null;

        mInModelNames = new ObservableCollection<string>(InModels.Select(m => m.Name));
        return mInModelNames;
      }
      set
      {
        mInModelNames = value;
        OnPropertyChanged("InModelNames");
      }
    }

    public override bool Equals(object obj)
    {
      var other = obj as MuAtlTestCase;
      if (other == null)
        return false;

      return Name.Equals(other.Name);
    }

    public int CompareTo(MuAtlTestCase other)
    {
      return Name.CompareTo(other.Name);
    }
  }
}
