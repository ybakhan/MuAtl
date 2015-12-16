using GalaSoft.MvvmLight.CommandWpf;
using MuAtl.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using MuAtl.Service;
using MuAtl.ViewModel.Base;
using GalaSoft.MvvmLight.Messaging;
using System.IO;

namespace MuAtl.ViewModel
{
  public class TestSuiteViewModel : MutantViewModelBase
  {
    #region constants
    
    private const string ExpectedPathKey = "ExpectedPath";
    private const string TestCaseNameKey = "TestCaseName";

    #endregion

    #region props

    public override MuAtlProject Project
    {
      get
      {
        return base.Project;
      }
      set
      {
        base.Project = value;
        Project.Cleared += OnProjectCleared;
        Project.PropertyChanged += Project_PropertyChanged;
      }
    }

    private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Dependency" && Project != null)
      {
        Project.Dependency.InModels.CollectionChanged += OnInModelsChanged;
      }
    }

    private bool mModuleValid;
    public bool ProjectPersistent
    {
      get
      {
        return mModuleValid;
      }
      set
      {
        mModuleValid = value;
        RaisePropertyChanged("ProjectPersistent");
      }
    }

    protected override void RaisePropertyChanged(string propertyName = null)
    {
      base.RaisePropertyChanged(propertyName);
      if (propertyName == "Project" && Project != null)
      {
        foreach (var inModel in Project.Dependency.InModels)
        {
          Input.Add(new TestCaseInput
          {
            Name = inModel.Name,
            Path = inModel.Path
          });
        }
      }
    }

    private string mTestCaseName;
    public string TestCaseName
    {
      get
      {
        return mTestCaseName;
      }
      set
      {
        mTestCaseName = value;
        RaisePropertyChanged(TestCaseNameKey);
        ValidateProperty(TestCaseName, TestCaseNameKey, "Test case name is required");
      }
    }

    private string mExpectedPath;
    public string ExpectedPath
    {
      get
      {
        return mExpectedPath;
      }
      set
      {
        mExpectedPath = value;
        RaisePropertyChanged(ExpectedPathKey);
        ValidateProperty(ExpectedPath, ExpectedPathKey, "Expected output model path is required");
      }
    }

    private ObservableCollection<TestCaseInput> mInModels = new ObservableCollection<TestCaseInput>();
    public ObservableCollection<TestCaseInput> Input
    {
      get
      {
        return mInModels;
      }
      set
      {
        mInModels = value;
        RaisePropertyChanged("Input");
      }
    }

    private MuAtlTestCase mSelectedTestCase;
    public MuAtlTestCase SelectedTestCase
    {
      get
      {
        return mSelectedTestCase;
      }
      set
      {
        mSelectedTestCase = value;
        RaisePropertyChanged("SelectedTestCase");
      }
    }

    private MuAtlTestCase mFoundTestCase;
    public MuAtlTestCase FoundTestCase
    {
      get
      {
        return mFoundTestCase;
      }
      set
      {
        mFoundTestCase = value;
        RaisePropertyChanged("FoundTestCase");
      }
    }

    #region commands

    public RelayCommand AddCmd { get; private set; }
    public RelayCommand DelCmd { get; private set; }
    public RelayCommand UpdateCmd { get; private set; }
    public RelayCommand<object> SelectCmd { get; private set; }
    public RelayCommand<TestCaseInput> SelectInputCmd { get; private set; }
    public RelayCommand SelectOutputCmd { get; private set; }

    #endregion

    #endregion

    public TestSuiteViewModel(IDialogService dlgService)
      : base(dlgService)
    {
      AddCmd = new RelayCommand(AddTestCase);
      DelCmd = new RelayCommand(DelTestCase);
      UpdateCmd = new RelayCommand(UpdateTestCase);
      SelectCmd = new RelayCommand<object>(SelectTestCase);
      SelectInputCmd = new RelayCommand<TestCaseInput>(SelectInput);
      SelectOutputCmd = new RelayCommand(SelectOutput);

      Messenger.Default.Register<bool>(this, "ProjectPersistent", OnProjectPresistent);
      if (IsInDesignModeStatic)
      {
        ProjectPersistent = true;
      }
    }

    private void OnProjectPresistent(bool persistent)
    {
        ProjectPersistent = persistent;
    }

    public const string AddTestCaseCaption = "Add test case";
    public const string AddInputModelErrorMsg = "Cannot add test case. Add input model(s)";

    private void AddTestCase()
    {
      ValidateProperty(ExpectedPath, ExpectedPathKey, "Expected output model path is required");
      ValidateProperty(TestCaseName, TestCaseNameKey, "Test case name is required");
      if (HasErrors)
      {
        return;
      }

      if (Input.Any(m => string.IsNullOrEmpty(m.Path)))
      {
        mDlgService.Error(AddInputModelErrorMsg, AddTestCaseCaption);
        return;
      }

      if (Project.TestSuite.Any(t => t.Name == TestCaseName))
      {
        mDlgService.Error(string.Format("Cannot add test case {0} because it already exists", TestCaseName), AddTestCaseCaption);
        return;
      }

      foreach (var inModel in Input)
      {
        if (!File.Exists(inModel.Path))
        {
          mDlgService.Error(string.Format("Input model {0} does not exist", inModel.Path), AddTestCaseCaption);
          return;
        }
      }

      if (!File.Exists(ExpectedPath))
      {
        mDlgService.Error(string.Format("Out model {0} does not exist", ExpectedPath), AddTestCaseCaption);
        return;
      }

      var testCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = Project.Dependency.OutputModel.Name,
          Path = string.Copy(ExpectedPath)
        },
        ModulePath = ""
      };

      foreach(var inModel in Input)
      {
        testCase.InModels.Add(new TestCaseInput
        {
          Name = inModel.Name,
          Path = inModel.Path
        });
      }

      Project.AddTestCase(testCase);
      SelectedTestCase = null;
      ClearProperties();
      ClearProp(TestCaseNameKey);
      ClearProp(ExpectedPathKey);
    }

    public const string UpdateTestCaseCaption = "Update test case";
    public const string UpdateErrorMsg = "Cannot update test case. Add input model(s)";

    private void UpdateTestCase()
    {
      if (SelectedTestCase == null)
      {
        mDlgService.Error(SelectTestCaseMsg, UpdateTestCaseCaption);
        return;
      }

      ValidateProperty(ExpectedPath, ExpectedPathKey, "Expected output model path is required");
      ValidateProperty(TestCaseName, TestCaseNameKey, "Test case name is required");
      if (HasErrors)
      {
        return;
      }

      if (Input.Any(m => m.Path == string.Empty))
      {
        mDlgService.Error(UpdateErrorMsg, UpdateTestCaseCaption);
        return;
      }

      if (Project.TestSuite.Except(new [] { SelectedTestCase }).Any(t => t.Name == TestCaseName))
      {
        mDlgService.Error(string.Format("Test case {0} already exists", TestCaseName), UpdateTestCaseCaption);
        return;
      }

      foreach(var inModel in Input)
      {
        if (!File.Exists(inModel.Path))
        {
          mDlgService.Error(string.Format("Input model {0} does not exist", inModel.Path), UpdateTestCaseCaption);
          return;
        }
      }

      if (!File.Exists(ExpectedPath))
      {
        mDlgService.Error(string.Format("Out model {0} does not exist", ExpectedPath), UpdateTestCaseCaption);
        return;
      }

      SelectedTestCase.Name = TestCaseName;
      SelectedTestCase.OutModel.Path = string.Copy(ExpectedPath);

      foreach (var inModel in Input)
      {
        var input = SelectedTestCase.InModels.SingleOrDefault(m => m.Name == inModel.Name);
        if (input == null)
          continue;
        input.Path = inModel.Path;
      }
    }

    public const string DelTestCaseCaption = "Delete test case";
    public const string SelectTestCaseMsg = "Select test case";

    private void DelTestCase()
    {
      if (SelectedTestCase == null)
      {
        mDlgService.Error(SelectTestCaseMsg, DelTestCaseCaption);
        return;
      }
      Project.TestSuite.Remove(SelectedTestCase);
    }

    #region Browse

    public const string InputModelMsg = "Input model";

    private void SelectInput(TestCaseInput input)
    {
      var path = mDlgService.BrowseModel(InputModelMsg);
      if (!string.IsNullOrEmpty(path))
        input.Path = path;
    }

    public const string OutputModelMsg = "Output model";

    private void SelectOutput()
    {
      var path = mDlgService.BrowseModel(OutputModelMsg);
      if (!string.IsNullOrEmpty(path))
        ExpectedPath = path;
    }

    #endregion

    private void SelectTestCase(object param)
    {
      if (SelectedTestCase == null)
        return;
      ShowTestCase(SelectedTestCase);
    }

    protected override void FindItem(string searchParam)
    {
      FoundTestCase = Project.TestSuite.FirstOrDefault(m => string.Compare(m.Name, searchParam, true) == 0);

      if (FoundTestCase != null)
        ShowTestCase(FoundTestCase);
    }

    private void ShowTestCase(MuAtlTestCase testCase)
    {
      if (testCase == null)
        return;

      TestCaseName = testCase.Name;
      foreach(var model in testCase.InModels)
      {
        var inModel = Input.SingleOrDefault(m => m.Name == model.Name);
        if (inModel == null)
          continue;
        inModel.Path = model.Path;
      }
      ExpectedPath = testCase.OutModel.Path;
    }

    private void OnProjectCleared(object sender, System.EventArgs e)
    {
      ClearProperties();
      ClearValidationErrors();
      Project.Dependency.OutputModel.Name = string.Empty;
      Project.TestSuite.Clear();
      Input.Clear();
    }

    private void OnInModelsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
      {
        foreach (InModel model in e.NewItems)
        {
          Input.Add(new TestCaseInput
          {
            Name = model.Name,
            Path = model.Path
          });
        }
      }

      if (e.OldItems != null)
      {
        foreach (InModel model in e.OldItems)
        {
          Input.Remove(new TestCaseInput
          {
            Name = model.Name
          });
        }
      }
    }

    private void ClearProperties()
    {
      TestCaseName = string.Empty;
      ExpectedPath = string.Empty;
      foreach (var model in Input)
      {
        model.Path = string.Empty;
      }
    }

    private void ClearValidationErrors()
    {
      mValidationErrors.Clear();
      RaiseErrorsChanged(ExpectedPathKey);
      RaiseErrorsChanged(TestCaseNameKey);
    }
  }
}