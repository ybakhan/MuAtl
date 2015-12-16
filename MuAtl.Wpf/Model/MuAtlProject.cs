using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Linq;

namespace MuAtl.Model
{
  public delegate void ProjectClearedEventHandler(object sender, EventArgs e);

  public class MuAtlProject : MuAtlModelBase
  {
    public MuAtlProject()
    {
      Dependency = new ProjectConfig();
      TestSuite = new ObservableCollection<MuAtlTestCase>();
      Mutants = new ObservableCollection<MuAtlMutant>();
      Results = new ObservableCollection<MuAtlResult>();
    }

    public event ProjectClearedEventHandler Cleared;

    protected void RaiseProjectCleared()
    {
      var handler = Cleared;
      if (handler != null)
      {
        Cleared(this, new EventArgs());
      }
    }

    private bool mIsRunning;
    [XmlIgnoreAttribute]
    public bool IsRunning
    {
      get
      {
        return mIsRunning;
      }
      set
      {
        mIsRunning = value;
        OnPropertyChanged("IsRunning");
      }
    }

    private string mModule;
    public string Module
    {
      get
      {
        return mModule;
      }
      set
      {
        mModule = value;
        OnPropertyChanged("Module");
      }
    }

    private ProjectConfig mDependency;
    public ProjectConfig Dependency
    {
      get
      {
        return mDependency;
      }
      set
      {
        mDependency = value;
        OnPropertyChanged("Dependency");
      }
    }

    private ObservableCollection<MuAtlTestCase> mTestSuite;
    public ObservableCollection<MuAtlTestCase> TestSuite
    {
      get
      {
        return mTestSuite;
      }
      set
      {
        mTestSuite = value;
        OnPropertyChanged("TestSuite");
      }
    }

    private ObservableCollection<MuAtlMutant> mMutants;
    public ObservableCollection<MuAtlMutant> Mutants
    {
      get
      {
        return mMutants;
      }
      set
      {
        mMutants = value;
        OnPropertyChanged("Mutants");
      }
    }

    private ObservableCollection<MuAtlResult> mResults;
    internal object p;

    public ObservableCollection<MuAtlResult> Results
    {
      get
      {
        return mResults;
      }
      set
      {
        mResults = value;
        OnPropertyChanged("Results");
      }
    }

    public void AddTestCase(MuAtlTestCase testCase)
    {
      if (TestSuite.Contains(testCase))
        TestSuite.Remove(testCase);

      TestSuite.Add(testCase);
    }

    public bool ContainsTestCase(string name)
    {
      return TestSuite.Contains(new MuAtlTestCase { Name = name });
    }

    public bool ContainsMutant(string name)
    {
      return Mutants.Contains(new MuAtlMutant { Name = name });
    }

    public void RemoveTestCase(string name)
    {
      TestSuite.Remove(new MuAtlTestCase { Name = name });
    }

    public void AddMutant(MuAtlMutant mutant)
    {
      if (Mutants.Contains(mutant))
        Mutants.Remove(mutant);

      Mutants.Add(mutant);
    }

    public void Clear()
    {
      Module = string.Empty;
      Dependency.InMetamodels.Clear();
      Dependency.InModels.Clear();
      Dependency.Libraries.Clear();
      Dependency.SuperImposedModules.Clear();

      Dependency.OutMetamodel.Name = string.Empty;
      Dependency.OutMetamodel.Path = string.Empty;
      Dependency.OutMetamodel.Handler = string.Empty;
      Dependency.OutputModel.Name = string.Empty;
      RaiseProjectCleared();

      TestSuite.Clear();
      Mutants.Clear();
      Results.Clear();
      Name = string.Empty;
      IsRunning = false;
    }

    public void AddInMetamodel(Metamodel mm)
    {
      if (Dependency.InMetamodels.Contains(mm))
        Dependency.InMetamodels.Remove(mm);
      Dependency.InMetamodels.Add(mm);
    }

    public void AddInModel(InModel im)
    {
      if (Dependency.InModels.Contains(im))
      {
        Dependency.InModels.Remove(im);
      }
      Dependency.InModels.Add(im);
    }

    public void AddLibrary(string name, string path)
    {
      var library = new AtlLibrary
      {
        Name = name,
        Path = path
      };
      Dependency.AddLibrary(library);
    }

    public void AddSuperImposedModule(string name, string path)
    {
      var supMod = new SuperImposedModule
      {
        Name = name,
        Path = path
      };
      Dependency.AddSuperImposedModule(supMod);
    }

    public void AddResult(MuAtlResult result)
    {
      if (Results.Contains(result))
      {
        Results.Remove(result);
      }
      Results.Add(result);
    }
  }
}
