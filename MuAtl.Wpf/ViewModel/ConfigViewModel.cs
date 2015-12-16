using GalaSoft.MvvmLight.CommandWpf;
using MuAtl.Model;
using MuAtl.ViewModel.Base;
using MuAtl.Service;
using System.ComponentModel;
using MuAtl.Service.Reader;
using Antlr4.Runtime.Tree;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using System.IO;

namespace MuAtl.ViewModel
{
  public class ConfigViewModel : MuAtlViewModelBase
  {
    #region constants

    private const string OutMmPathKey = "Project.Dependency.OutMetamodel.Path";
    private const string OutMmPathErrorMsg = "Output metamodel batchFilePath is required";

    private const string ModulePathKey = "Project.Module";
    private const string ModulePathErrorMsg = "Module batchFilePath is required";

    #endregion

    private IAtlParser mParser;

    #region props

    private bool mModuleSelected;
    public bool ModuleSelected
    {
      get
      {
        return mModuleSelected;
      }
      set
      {
        mModuleSelected = value;
        RaisePropertyChanged("ModuleSelected");
      }
    }

    private bool mPersistentProj;
    public bool PersistentProj
    {
      get
      {
        return mPersistentProj;
      }
      set
      {
        mPersistentProj = value;
        RaisePropertyChanged("PersistentProj");
      }
    }

    public override MuAtlProject Project
    {
      get
      {
        return base.Project;
      }
      set
      {
        base.Project = value;
        Project.PropertyChanged += OnModuleChanged;
        Project.Cleared += OnProjectCleared;
      }
    }

    #region sup mod

    private const string SupModNameKey = "SupModName";
    private const string SupModNameErrorMsg = "name is required";

    private string mSupModName;
    public string SupModName
    {
      get
      {
        return mSupModName;
      }
      set
      {
        mSupModName = value;
        RaisePropertyChanged(SupModNameKey);
        ValidateProperty(SupModName, SupModNameKey, SupModNameErrorMsg);
      }
    }

    private const string SupModPathKey = "SupModPath";
    private const string SupModPathErrorMsg = "path is required";

    private string mSupModPath;
    public string SupModPath
    {
      get
      {
        return mSupModPath;
      }
      set
      {
        mSupModPath = value;
        RaisePropertyChanged(SupModPathKey);
        ValidateProperty(SupModPath, SupModPathKey, SupModPathErrorMsg);
      }
    }

    #endregion

    #region commands

    public RelayCommand SelectModuleCmd { get; private set; }
    public RelayCommand<Metamodel> SelectInMmCmd { get; private set; }
    public RelayCommand<Metamodel> AddDpndntMmCmd { get; private set; }
    public RelayCommand<Metamodel> DelDpndntMmCmd { get; private set; }
    public RelayCommand SelectOutMmCmd { get; private set; }
    public RelayCommand<AtlLibrary> SelectLibCmd { get; private set; }
    public RelayCommand AddSupModCmd { get; private set; }
    public RelayCommand UpdateSupModCmd { get; private set; }
    public RelayCommand DelSupModCmd { get; private set; }
    public RelayCommand SelectSupModCmd { get; private set; }
    public RelayCommand OnTableSelectCmd { get; private set; }

    #endregion

    #endregion

    private Metamodel mSelectedDepMm;
    public Metamodel SelectedDepMm
    {
      get
      {
        return mSelectedDepMm;
      }
      set
      {
        mSelectedDepMm = value;
        RaisePropertyChanged("SelectedDepMm");
      }
    }

    private SuperImposedModule mSelectedSupMod;
    public SuperImposedModule SelectedSupMod
    {
      get
      {
        return mSelectedSupMod;
      }
      set
      {
        mSelectedSupMod = value;
        RaisePropertyChanged("SelectedSupMod");
      }
    }

    private IRepository mRepository;

    public ConfigViewModel(IDialogService dlgService, IAtlParser parser, IRepository repository)
      : base(dlgService)
    {
      mParser = parser;
      mRepository = repository;
      mRepository.ProjectSaved += OnProjectSaved;
      mRepository.ProjectLoaded += OnProjectLoaded;

      #region init cmd

      SelectModuleCmd = new RelayCommand(SelectModule);
      SelectInMmCmd = new RelayCommand<Metamodel>(SelectInMm);
      AddDpndntMmCmd = new RelayCommand<Metamodel>(AddDpndntMm);
      DelDpndntMmCmd = new RelayCommand<Metamodel>(DelDpndntMm);
      SelectOutMmCmd = new RelayCommand(SelectOutMm);
      SelectLibCmd = new RelayCommand<AtlLibrary>(SelectLib);
      AddSupModCmd = new RelayCommand(AddSupMod);
      DelSupModCmd = new RelayCommand(DelSupMod);
      UpdateSupModCmd = new RelayCommand(UpdateSupMod);
      SelectSupModCmd = new RelayCommand(SelectSupMod);
      OnTableSelectCmd = new RelayCommand(OnTableSelect);

      #endregion

      if (IsInDesignModeStatic)
      {
        ModuleSelected = true;
      }
    }

    private void OnTableSelect()
    {
      if (SelectedSupMod == null)
        return;

      SupModName = SelectedSupMod.Name;
      SupModPath = SelectedSupMod.Path;
    }

    private void OnProjectLoaded()
    {
      ModuleSelected = true;
      PersistentProj = true;
    }

    private void OnProjectSaved()
    {
      PersistentProj = true;
    }

    public const string ModuleUpdateMsg = "Updating the ATL module path will create a new project";
    public const string SelectModuleCaption = "Select ATL module";

    private void SelectModule()
    {
      if (!PersistentProj)
      {
        SelectModuleInternal();
        return;
      }

      var dlgResult = mDlgService.YesNoDialog(ModuleUpdateMsg, SelectModuleCaption);
      if (dlgResult)
      {
        Messenger.Default.Send(0, "NewProject");
        SelectModuleInternal();
      }
    }

    private void SelectModuleInternal()
    {
      var path = mDlgService.BrowseModule();
      if (string.IsNullOrEmpty(path))
        return;

      Project.Module = path;
      //ModuleSelected = true;
    }

    #region InMetamodel

    public const string InputMetamodelTitle = "Input Metamodel";

    private void SelectInMm(Metamodel mm)
    {
      if (mm == null)
        return;

      var path = mDlgService.BrowseMetamodel(InputMetamodelTitle);
      if (!string.IsNullOrEmpty(path))
      {
        mm.Path = path;
      }
    }

    #region Dpndnt Module

    public const string DependentMetamodelMsg = "Dependent Metamodel of";
    public const string AddDpndntMmFailMsg = "Cannot add same name metamodel as dependent metamodel of";
    public const string AddDpndntMmCaption = "Add dependent metamodel";

    private void AddDpndntMm(Metamodel mm)
    {
      if (mm == null)
        return;

      var path = mDlgService.BrowseMetamodel(string.Format("{0} {1}", DependentMetamodelMsg, mm.Name));
      if (string.IsNullOrEmpty(path))
        return;

      var depMm = new Metamodel
      {
        Name = Path.GetFileNameWithoutExtension(path).ToUpper(),
        Path = path
      };

      if (mm.Equals(depMm))
      {
        mDlgService.Error(string.Format("{0} {1}", AddDpndntMmFailMsg, mm.Name), AddDpndntMmCaption);
        return;
      }

      if (mm.Dependencies.Any(d => d.Path == depMm.Path || d.Name == depMm.Name))
      {
        if (mDlgService.YesNoDialog(string.Format("Dependent metamodel {0} already exists. Update existing?", depMm.Name), AddDpndntMmCaption))
        {
          AddDepMm(mm, depMm);
        }
        else
        {
          return;
        }
      }

      AddDepMm(mm, depMm);
    }

    private void AddDepMm(Metamodel mm, Metamodel depMm)
    {
      if (mm.Dependencies.Contains(depMm))
        mm.Dependencies.Remove(depMm);
      mm.Dependencies.Add(depMm);
    }

    public const string SelectDpndntMmMsg = "Select dependent metamodel";
    public const string DelDpndntMmCaption = "Delete dependent metamodel";

    private void DelDpndntMm(Metamodel mm)
    {
      if (mm == null)
        return;

      if (SelectedDepMm == null)
      {
        mDlgService.Error(SelectDpndntMmMsg, DelDpndntMmCaption);
        return;
      }

      mm.Dependencies.Remove(SelectedDepMm);
    }

    #endregion

    #endregion

    public const string OutMmMsg = "Output Metamodel";

    private void SelectOutMm()
    {
      var path = mDlgService.BrowseMetamodel(OutMmMsg);
      if (!string.IsNullOrEmpty(path))
        Project.Dependency.OutMetamodel.Path = path;
    }

    public const string LibMsg = "Library";

    private void SelectLib(AtlLibrary library)
    {
      if (library == null)
        return;

      var path = mDlgService.BrowseAtl(LibMsg);
      if (!string.IsNullOrEmpty(path))
      {
        library.Path = path;
      }
    }

    #region sup mod

    public const string UpdateSupModCaption = "Update superimposed module";

    private void UpdateSupMod()
    {
      if (SelectedSupMod == null)
      {
        mDlgService.Error(SelectSupModMsg, UpdateSupModCaption);
        return;
      }

      ValidateProperty(SupModName, SupModNameKey, SupModNameErrorMsg);
      ValidateProperty(SupModPath, SupModPathKey, SupModPathErrorMsg);
      if (HasErrors)
      {
        return;
      }

      if (Project.Dependency.SuperImposedModules.Except(new[] { SelectedSupMod }).Any(s => s.Name == SupModName))
      {
        mDlgService.Error(string.Format("Superimposed module {0} already exists", SupModName),
          UpdateSupModCaption);
        return;
      }

      if (!File.Exists(SupModPath))
      {
        mDlgService.Error(string.Format("Superimposed module {0} does not exist", SupModPath),
          UpdateSupModCaption);
        return;
      }

      SelectedSupMod.Name = SupModName;
      SelectedSupMod.Path = SupModPath;
    }

    public const string SelectSupModMsg = "Select superimposed module";
    public const string DelSupModCaption = "Delete superimposed module";

    private void DelSupMod()
    {
      if (SelectedSupMod == null)
      {
        mDlgService.Error(SelectSupModMsg, DelSupModCaption);
        return;
      }

      Project.Dependency.SuperImposedModules.Remove(SelectedSupMod);
      SupModName = string.Empty;
      SupModPath = string.Empty;

      ClearProp("SupModName");
      ClearProp("SupModPath");
    }

    public const string SupModMsg = "Super Imposed Module";

    private void SelectSupMod()
    {
      var path = mDlgService.BrowseAtl(SupModMsg);
      if (!string.IsNullOrEmpty(path))
        SupModPath = path;

      if (string.IsNullOrEmpty(SupModName))
        SupModName = Path.GetFileNameWithoutExtension(SupModPath);
    }

    #endregion

    protected override void RaisePropertyChanged(string propertyName = null)
    {
      base.RaisePropertyChanged(propertyName);
      if (propertyName != "Project")
        return;

      if (string.IsNullOrEmpty(Project.Module) || !System.IO.File.Exists(Project.Module))
      {
        Messenger.Default.Send(string.Empty, "ModuleParsed");
        ModuleSelected = false;
        return;
      }

      Parse();
    }

    private void OnModuleChanged(object sender, PropertyChangedEventArgs e)
    {
      var proj = sender as MuAtlProject;
      if (e.PropertyName != "Module")
        return;

      Project.TestSuite.Clear();
      Project.Mutants.Clear();
      Project.Results.Clear();
      Project.Dependency = new ProjectConfig();
      Project.IsRunning = false;

      if (string.IsNullOrEmpty(proj.Module) || !File.Exists(proj.Module))
      {
        Messenger.Default.Send(false, "ProjectPersistent");
        ModuleSelected = false;
        return;
      }

      Parse();
    }

    private void Parse()
    {
      try
      {
        var tokens = (new AtlTokenizer()).GetTokens(Project.Module);
        var listener = new ConfigListener(tokens);
        var parseTree = mParser.Parse(tokens);
        var walker = new ParseTreeWalker();
        walker.Walk(listener, parseTree);

        if (string.IsNullOrEmpty(listener.OutModelName) ||
          string.IsNullOrEmpty(listener.OutMetamodelName))
        {
          throw new System.Exception();
        }

        foreach (var libName in listener.LibNames)
        {
          var lib = new AtlLibrary
          {
            Name = libName,
            Path = string.Empty
          };
          if (!Project.Dependency.Libraries.Contains(lib))
          {
            Project.Dependency.Libraries.Add(lib);
          }
        }

        Project.Dependency.OutputModel.Name = listener.OutModelName;
        Project.Dependency.OutMetamodel.Name = listener.OutMetamodelName;

        foreach (var inModel in listener.InModels)
        {
          var inMm = new Metamodel
          {
            Name = inModel.Value
          };

          if (!Project.Dependency.InMetamodels.Contains(inMm))
          {
            Project.Dependency.InMetamodels.Add(inMm);
          }

          var inModels = new InModel
          {
            Name = inModel.Key,
            Metamodels = inMm
          };

          if (!Project.Dependency.InModels.Contains(inModels))
          {
            Project.Dependency.InModels.Add(inModels);
          }
        }
        Messenger.Default.Send(Project.Module, "ModuleParsed");
        ModuleSelected = true;
      }
      catch (System.Exception)
      {
        mDlgService.Error("ATL module is invalid", "Parse ATL module");
        Messenger.Default.Send(false, "ModuleParsed");
        ModuleSelected = false;
      }
    }

    public const string AppSupModCaption = "Add superimposed module";

    private void AddSupMod()
    {
      ValidateProperty(SupModName, SupModNameKey, SupModNameErrorMsg);
      ValidateProperty(SupModPath, SupModPathKey, SupModPathErrorMsg);
      if (mValidationErrors.ContainsKey(SupModNameKey) || mValidationErrors.ContainsKey(SupModPathKey))
      {
        return;
      }

      if (Project.Dependency.SuperImposedModules.Any(s => s.Name == SupModName))
      {
        mDlgService.Error(string.Format("Cannot add superimposed module {0} because it already exists", SupModName),
          AppSupModCaption);
        return;
      }

      if (!File.Exists(SupModPath))
      {
        mDlgService.Error(string.Format("Superimposed module {0} does not exist", SupModPath),
         AppSupModCaption);
        return;
      }

      Project.AddSuperImposedModule(SupModName, SupModPath);
      SelectedSupMod = null;
      ClearSupModProps();
    }

    private void OnProjectCleared(object sender, System.EventArgs e)
    {
      ClearSupModProps();

      mValidationErrors.Clear();

      RaiseErrorsChanged(ModulePathKey);
      RaiseErrorsChanged(OutMmPathKey);

      ModuleSelected = false;
      PersistentProj = false;
    }

    private void ClearSupModProps()
    {
      SupModName = string.Empty;
      SupModPath = string.Empty;

      mValidationErrors.Remove(SupModNameKey);
      mValidationErrors.Remove(SupModPathKey);

      RaiseErrorsChanged(SupModNameKey);
      RaiseErrorsChanged(SupModPathKey);
    }
  }
}