using GalaSoft.MvvmLight.CommandWpf;
using MuAtl.Model;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using MuAtl.Model.MuAtlJavaArgs;
using GalaSoft.MvvmLight.Threading;
using System.Threading;
using System.Collections.Specialized;
using log4net;
using MuAtl.Service;
using MuAtl.ViewModel.Base;
using MuAtl.ViewModel.Util;
using MuAtl.Service.Runner;

namespace MuAtl.ViewModel
{
  public class RunTestsViewModel : MuAtlViewModelBase
  {
    private static readonly ILog logger = LogManager.GetLogger(typeof(RunTestsViewModel));
    
    #region instance vars

    private readonly IMutantRunner mService;
    private CancellationTokenSource mTokenSource = new CancellationTokenSource();

    #endregion

    #region props

    private bool mUml2Strtypes;
    public bool Uml2Strtypes
    {
      get
      {
        return mUml2Strtypes;
      }
      set
      {
        mUml2Strtypes = value;
        RaisePropertyChanged("Uml2Strtypes");
      }
    }

    private bool mInterModelRefs;
    public bool InterModelRefs
    {
      get
      {
        return mInterModelRefs;
      }
      set
      {
        mInterModelRefs = value;
        RaisePropertyChanged("InterModelRefs");
      }
    }

    private double mExecutionProgress;
    public double ExecutionProgress
    {
      get
      {
        return mExecutionProgress;
      }
      set
      {
        mExecutionProgress = value;
        RaisePropertyChanged("ExecutionProgress");
      }
    }

    private string mProgressMessage;
    public string ProgressMessage
    {
      get
      {
        return mProgressMessage;
      }
      set
      {
        mProgressMessage = value;
        RaisePropertyChanged("ProgressMessage");
      }
    }

    #region cmds

    public RelayCommand ExecuteCmd { get; private set; }
    public RelayCommand AbortCmd { get; private set; }
	
    public RelayCommand SelectAllTestCaseCmd { get; private set; }
    public RelayCommand UnSelectAllTestCaseCmd { get; private set; }

    public RelayCommand<MutantType> CheckMutantTypeCmd { get; private set; }
    public RelayCommand<MutantType> UnCheckMutantTypeCmd { get; private set; }

    #endregion

    #region observable collections

    private SelectionList<MuAtlTestCase> mSelectionTestCases = new SelectionList<MuAtlTestCase>();
    public SelectionList<MuAtlTestCase> SelectionTestCases
    {
      get
      {
        return mSelectionTestCases;
      }
      set
      {
        mSelectionTestCases = value;
        RaisePropertyChanged("SelectionTestCases");
      }
    }

    private SelectionList<MuAtlMutant> mSelectionMutants = new SelectionList<MuAtlMutant>();
    public SelectionList<MuAtlMutant> SelectionMutants
    {
      get
      {
        return mSelectionMutants;
      }
      set
      {
        mSelectionMutants = value;
        RaisePropertyChanged("SelectionMutants");
      }
    }

    #endregion

    public List<AtlCompilerType> Compilers { get; set; }

    private AtlCompilerType mCompiler;
    public AtlCompilerType Compiler
    {
      get
      {
        return mCompiler;
      }
      set
      {
        mCompiler = value;
        RaisePropertyChanged("Compiler");
      }
    }

    private AtlVmType mVm;
    public AtlVmType Vm
    {
      get
      {
        return mVm;
      }
      set
      {
        mVm = value;
        RaisePropertyChanged("Vm");
      }
    }

    public List<AtlVmType> Vms { get; set; }

    public override MuAtlProject Project
    {
      get
      {
        return base.Project;
      }
      set
      {
        base.Project = value;

        foreach (var mutant in Project.Mutants)
          SelectionMutants.Add(mutant);

        foreach (var testCase in Project.TestSuite)
          SelectionTestCases.Add(testCase);

        Project.Mutants.CollectionChanged += OnMutantsCollectionChanged;
        Project.TestSuite.CollectionChanged += OnTestSuiteCollectionChanged;
        Project.Cleared += OnProjectCleared;
      }
    }

    #endregion

    public RunTestsViewModel(IMutantRunner service, IDialogService dlgService)
      : base(dlgService)
    {
      mService = service;

      #region cmd

      ExecuteCmd = new RelayCommand(Execute);
      AbortCmd = new RelayCommand(OnAbortClick);

      SelectAllTestCaseCmd = new RelayCommand(CheckAllTestCase);
      UnSelectAllTestCaseCmd = new RelayCommand(UnSelectAllTestCase);

      CheckMutantTypeCmd = new RelayCommand<MutantType>(MutantTypeChecked);
      UnCheckMutantTypeCmd = new RelayCommand<MutantType>(UnCheckMutantType);

      #endregion

      Compilers = Enum.GetValues(typeof(AtlCompilerType)).Cast<AtlCompilerType>().ToList();
      Vms = Enum.GetValues(typeof(AtlVmType)).Cast<AtlVmType>().ToList();

      if (IsInDesignModeStatic)
      {
        Project = new MuAtlProject();
        Project.TestSuite.Add(new MuAtlTestCase
        {
          Name = "Design Test Case 1"
        });

        Project.TestSuite.Add(new MuAtlTestCase
        {
          Name = "Design Test Case 2"
        });

        Project.Mutants.Add(new MuAtlMutant
        {
          Name = "Design Mutant 1",
          Type = MutantType.AAM
        });

        Project.Mutants.Add(new MuAtlMutant
        {
          Name = "Design Mutant 2",
          Type = MutantType.AAM
        });
      }
    }

    #region execute

    public const string NoMutantsErrorMsg = "No mutants to execute";
    public const string RunTestsCaption = "Run Tests";
    public const string NoTestCasesErrorMsg = "No test cases to execute";
    public const string NoMutantsSelectedErrorMsg = "No mutants selected";
    public const string NoTestCasesSelectedErrorMsg = "No test cases selected";

    private void Execute()
    {
      if (!SelectionMutants.Any())
      {
        mDlgService.Error(NoMutantsErrorMsg, RunTestsCaption);
        return;
      }

      if (!Project.TestSuite.Any())
      {
        mDlgService.Error(NoTestCasesErrorMsg, RunTestsCaption);
        return;
      }

      if (!SelectionMutants.SelectedItems.Any())
      {
        mDlgService.Error(NoMutantsSelectedErrorMsg, RunTestsCaption);
        return;
      }

      if (!SelectionTestCases.SelectedItems.Any())
      {
        mDlgService.Error(NoTestCasesSelectedErrorMsg, RunTestsCaption);
        return;
      }

      mTokenSource = new CancellationTokenSource();
      Task.Factory.StartNew(() =>
        Execute(SelectionMutants.SelectedItems,
                SelectionTestCases.SelectedItems),
                mTokenSource.Token);
    }

    private void Execute(IEnumerable<MuAtlMutant> mutants, IEnumerable<MuAtlTestCase> testCases)
    {
      Project.IsRunning = true;
      var total = mutants.Count() * testCases.Count();
      var progress = 0;

      mService.Compiler = Compiler;
      mService.Vm = Vm;
      mService.Uml2Strtypes = Uml2Strtypes;
      mService.InterModelRefs = InterModelRefs;
      mService.Config = Project.Dependency; ;

      logger.InfoFormat("started mutation test execution");

      foreach (var mutant in mutants.OrderBy(m => m.Name))
      {
        foreach (var testcase in testCases.OrderBy(m => m.Name))
        {
          if (mTokenSource.IsCancellationRequested)
          {
            logger.InfoFormat("mutation test aborted\n");
            return;
          }

          ProgressMessage = string.Format(
            "Executing test case {0} on mutant {1} {2}/{3}",
            testcase.Name, mutant.Name, ++progress, total);

          try
          {
            var result = mService.Run(mutant, testcase);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
              Project.AddResult(result);
            });
          }
          catch (Exception ex)
          {
            logger.ErrorFormat(string.Format(
              "Exception occured while executing test case {0} mutant {1}: {2}",
              testcase.Name, mutant.Name, ex.Message));
          }
          ExecutionProgress = (progress / (double)total) * 100;
        }
      }

      logger.InfoFormat("finished mutation test execution\n");
      Project.IsRunning = false;
      ExecutionProgress = 0;
      ProgressMessage = string.Empty;
    }

    #endregion

    private void OnProjectCleared(object sender, EventArgs e)
    {
      if (Project.IsRunning)
      {
        Abort();
        Project.IsRunning = false;
      }
    }

    #region abort tests

    public const string AbortCaption = "Abort Mutation Tests";
    public const string ConfirmAbortion = "Are you sure?";

    private void OnAbortClick()
    {
      var dlg = mDlgService.YesNoDialog(ConfirmAbortion, AbortCaption);
      if (dlg)
      {
        Abort();
      }
    }

    private void Abort()
    {
      Project.IsRunning = false;
      mTokenSource.Cancel();
      ExecutionProgress = 0;
      ProgressMessage = string.Empty;
    } 

    #endregion

    private void CheckAllTestCase()
    {
      SelectionTestCases.SelectAll();
    }

    private void UnSelectAllTestCase()
    {
      SelectionTestCases.UnselectAll();
    }

    private void UnCheckMutantType(MutantType type)
    {
      foreach (var mutant in SelectionMutants.Where(m => m.Item.Type == type))
      {
        mutant.IsSelected = false;
      }
    }

    private void MutantTypeChecked(MutantType type)
    {
      foreach (var mutant in SelectionMutants.Where(m => m.Item.Type == type))
      {
        mutant.IsSelected = true;
      }
    }

    private void OnTestSuiteCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        SelectionTestCases.UnselectAll();
        SelectionTestCases.Clear();
      }

      if (e.NewItems != null)
      {
        foreach (MuAtlTestCase newItem in e.NewItems)
        {
          SelectionTestCases.Add(newItem);
        }
      }

      if (e.OldItems != null)
      {
        foreach (MuAtlTestCase oldItem in e.OldItems)
        {
          SelectionTestCases.Remove(oldItem);
        }
      }
    }

    private void OnMutantsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Reset)
        SelectionMutants.Clear();

      if (e.NewItems != null)
      {
        foreach (MuAtlMutant newItem in e.NewItems)
        {
          SelectionMutants.Add(newItem);
        }
      }

      if (e.OldItems != null)
      {
        foreach (MuAtlMutant oldItem in e.OldItems)
        {
          SelectionMutants.Remove(oldItem);
        }
      }
    }
  }
}