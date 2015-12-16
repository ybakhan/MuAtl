using GalaSoft.MvvmLight.CommandWpf;
using MuAtl.Model;
using MuAtl.Service;
using MuAtl.ViewModel.Base;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MuAtl.ViewModel
{
  public class ResultsViewModel : MutantViewModelBase
  {
    private const string XlFilter = "Excel Files|*.xlsx";

    #region cmds

    public RelayCommand<object> ViewLogCmd { get; private set; }
    public RelayCommand<object> CompareCmd { get; private set; }
    public RelayCommand ExportCmd { get; private set; }

    #endregion

    #region props

    private MuAtlResult mFoundResult;
    public MuAtlResult FoundResult
    {
      get
      {
        return mFoundResult;
      }
      set
      {
        mFoundResult = value;
        RaisePropertyChanged("FoundResult");
      }
    }

    private bool mSavingResults;
    public bool SavingResults
    {
      get
      {
        return mSavingResults;
      }
      set
      {
        mSavingResults = value;
        RaisePropertyChanged("SavingResults");
      }
    }

    private double mSaveResultsProgress;
    public double SaveResultsProgress
    {
      get
      {
        return mSaveResultsProgress;
      }
      set
      {
        mSaveResultsProgress = value;
        RaisePropertyChanged("SaveResultsProgress");
      }
    } 

    #endregion

    private IResultExporter mExportService;
    private ILogService mLogService;
    private IOracle mOracle;

    public ResultsViewModel(IDialogService dlgService, IResultExporter exportService, ILogService logService, IOracle oracle)
      : base(dlgService)
    {
      mExportService = exportService;
      mLogService = logService;
      mOracle = oracle;

      #region cmd

      ViewLogCmd = new RelayCommand<object>(ViewLog);
      CompareCmd = new RelayCommand<object>(Compare);
      ExportCmd = new RelayCommand(Export);

      #endregion
    }

    private void ViewLog(object param)
    {
      var result = param as MuAtlResult;
      if (result == null)
        return;

      if (!File.Exists(result.Log))
      {
        mDlgService.Error(
          string.Format("Log file {0} does not exist", result.Log), "View log");
        return;
      }

      mLogService.Open(result.Log);
    }

    private void Compare(object param)
    {
      var result = param as MuAtlResult;
      if (result == null)
        return;

      var expected = result.TestCase.OutModel.Path;
      if (!File.Exists(expected))
      {
        mDlgService.Error(
          string.Format("Expected output model {0} does not exist", expected), "Compare");
        return;
      }

      var actual = result.Output.Path;
      if (!File.Exists(actual))
      {
        mDlgService.Error(
          string.Format("Actual output model {0} does not exist", actual), "Compare");
        return;
      }

      mOracle.Compare(expected, actual);
    }

    private void Export()
    {
      var path = mDlgService.Save("Export results", XlFilter);
      if (string.IsNullOrEmpty(path))
        return;

      Task.Factory.StartNew(() =>
      {
        SavingResults = true;
        mExportService.Init();

        var count = 0;
        foreach (var result in Project.Results)
        {
          mExportService.Export(result);
          SaveResultsProgress = (++count / (double)Project.Results.Count) * 100;
        }

        mExportService.Save(path);
        SavingResults = false;
      });
    }

    protected override void FindItem(string searchParam)
    {
      if (string.IsNullOrEmpty(searchParam))
        return;

      FoundResult = Project.Results.FirstOrDefault(r =>
        string.Compare(r.Mutant.Name, searchParam, true) == 0);
    }
  }
}