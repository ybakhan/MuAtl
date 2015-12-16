using System.Collections.ObjectModel;

namespace MuAtl.Model
{
  public class ProjectConfig : MuAtlModelBase
  {
    public ProjectConfig()
    {
      InMetamodels = new ObservableCollection<Metamodel>();
      Libraries = new ObservableCollection<AtlLibrary>();
      SuperImposedModules = new ObservableCollection<SuperImposedModule>();
      InModels = new ObservableCollection<InModel>();
      OutMetamodel = new Metamodel();
      OutputModel = new OutModel();
    }
    
    private ObservableCollection<Metamodel> mInMetamodels;
    public ObservableCollection<Metamodel> InMetamodels
    {
      get
      {
        return mInMetamodels;
      }
      set
      {
        mInMetamodels = value;
        OnPropertyChanged("InMetamodels");
      }
    }

    private Metamodel mOutMetamodel;
    public Metamodel OutMetamodel
    {
      get
      {
        return mOutMetamodel;
      }
      set
      {
        mOutMetamodel = value;
        OnPropertyChanged("OutMetamodel");
      }
    }

    private ObservableCollection<InModel> mInModels;
    public ObservableCollection<InModel> InModels
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

    private OutModel mOutModel;
    public OutModel OutputModel
    {
      get
      {
        return mOutModel;
      }
      set
      {
        mOutModel = value;
        OnPropertyChanged("OutputModel");
      }
    }

    private ObservableCollection<AtlLibrary> mLibraries;
    public ObservableCollection<AtlLibrary> Libraries
    {
      get
      {
        return mLibraries;
      }
      set
      {
        mLibraries = value;
        OnPropertyChanged("Libraries");
      }
    }

    private ObservableCollection<SuperImposedModule> mSuperImposedModules;
    public ObservableCollection<SuperImposedModule> SuperImposedModules
    {
      get
      {
        return mSuperImposedModules;
      }
      set
      {
        mSuperImposedModules = value;
        OnPropertyChanged("SuperImposedModules");
      }
    }

    public void AddLibrary(AtlLibrary library)
    {
      if (Libraries.Contains(library))
        Libraries.Remove(library);

      Libraries.Add(library);
    }

    public void AddSuperImposedModule(SuperImposedModule module)
    {
      if (SuperImposedModules.Contains(module))
        SuperImposedModules.Remove(module);

      SuperImposedModules.Add(module);
    }
  }
}
