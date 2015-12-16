using NUnit.Framework;
using MuAtl.ViewModel;
using MuAtl.Service;
using NSubstitute;
using MuAtl.Model;
using System.Collections.ObjectModel;
using System.Threading;
using System;

namespace MuAtl.Test.ViewModel
{
  [TestFixture]
  public class ResultsViewModelTest : MuAtlViewModelTestBase
  {
    #region instance vars

    private ResultsViewModel mViewModel;
    private ILogService mLogService;
    private IDialogService mDlgService;
    private MuAtlResult mResult;
    private MuAtlResult mResult2;
    private IOracle mOracle;
    private IResultExporter mExporter;

    #endregion

    [SetUp]
    public override void Init()
    {
      base.Init();
      mLogService = Substitute.For<ILogService>();
      mDlgService = Substitute.For<IDialogService>();
      mOracle = Substitute.For<IOracle>();
      mExporter = Substitute.For<IResultExporter>();
      mViewModel = new ResultsViewModel(mDlgService, mExporter, mLogService, mOracle);
      mViewModel.Project = mProject;
      mResult = new MuAtlResult
      {
        Comment = "Test Comment",
        Log = @"TestData\testresultlog.txt",
        Mutant = new MuAtlMutant
        {
          Name = "Test Mutant",
          Path = "X:/../../testmutant.atl",
          Type = MutantType.AAM
        },
        Verdict = MuAtlVerdict.Pass,
        Name = "Test Result",
        Output = new MuAtlTestCaseOutput
        {
          Name = "Test output",
          Path = @"TestData\testactualoutput.xmi"
        },
        TestCase = new MuAtlTestCase
        {
          Name = "Test test case",
          InModels = new ObservableCollection<TestCaseInput>(new[]
          {
            new TestCaseInput
            {
              Name = "Test input",
              Path = @"TestData\testinput.xmi"
            }
          }),
          OutModel = new TestCaseOutput
          {
            Name = "Test expected ouput",
            Path = @"TestData\testoutput.xmi"
          }
        }
      };
      mViewModel.Project.Results.Add(mResult);

      mResult2 = new MuAtlResult
      {
        Comment = "Test Comment",
        Log = @"TestData\testresultlog.txt",
        Mutant = new MuAtlMutant
        {
          Name = "Test Mutant",
          Path = "X:/../../testmutant.atl",
          Type = MutantType.AAM
        },
        Verdict = MuAtlVerdict.Pass,
        Name = "Test Result2",
        Output = new MuAtlTestCaseOutput
        {
          Name = "Test output",
          Path = @"TestData\testactualoutput.xmi"
        },
        TestCase = new MuAtlTestCase
        {
          Name = "Test test case",
          InModels = new ObservableCollection<TestCaseInput>(new[]
          {
            new TestCaseInput
            {
              Name = "Test input",
              Path =@"TestData\testinput.xmi"
            }
          }),
          OutModel = new TestCaseOutput
          {
            Name = "Test expected ouput",
            Path = @"TestData\testoutput.xmi"
          }
        }
      };
    }

    [TearDown]
    public void Cleanup()
    {
      mExporter.Dispose();
    }

    #region View Log

    [Test]
    public void TestViewLog_ResultSelected_LogDoesNotExist_ErrorMessageShown_LogNotShown()
    {
      mResult.Log = "X:/../../";
      mViewModel.ViewLogCmd.Execute(mResult);
      mDlgService.Received().Error(string.Format("Log file {0} does not exist", mResult.Log), "View log");
      mLogService.DidNotReceive().Open(Arg.Any<string>());
    }

    [Test]
    public void TestViewLog_ResultSelected_LogExists_LogShown()
    {
      mViewModel.ViewLogCmd.Execute(mResult);
      mDlgService.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      mLogService.Received().Open(mResult.Log);
    }

    [Test]
    public void TestViewLog_ResultNotSelected_ErrorMessageNotShown_LogNotShown()
    {
      mViewModel.ViewLogCmd.Execute(null);
      mDlgService.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      mLogService.DidNotReceive().Open(Arg.Any<string>());
    }

    #endregion

    #region Compare

    [Test]
    public void TestCompare_ResultSelected_ExpectedOutputExists_ActualOutputExists_OutputsCompared()
    {
      mViewModel.CompareCmd.Execute(mResult);

      mDlgService.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      mOracle.Received().Compare(mResult.TestCase.OutModel.Path, mResult.Output.Path);
    }

    [Test]
    public void TestCompare_ResultSelected_ExpectedOutputExists_ActualOutputDoesNotExists_ErrorMessageShown_OutputNotCompared()
    {
      mResult.Output.Path = "X:/../../actualoutput.xmi";
      mViewModel.CompareCmd.Execute(mResult);

      mDlgService.Received().Error(
        string.Format("Actual output model {0} does not exist", mResult.Output.Path), "Compare");

      mDlgService.DidNotReceive().Error(
        string.Format("Expected output model {0} does not exist", mResult.TestCase.OutModel.Path), "Compare");
      mOracle.DidNotReceive().Compare(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void TestCompare_ResultSelected_ExpectedOutputDoesNotExist_ErrorMessageShown_OutputNotCompared()
    {
      mResult.TestCase.OutModel.Path = "X:/../../expectedout.xmi";
      mViewModel.CompareCmd.Execute(mResult);

      mDlgService.Received().Error(
        string.Format("Expected output model {0} does not exist", mResult.TestCase.OutModel.Path), "Compare");

      mDlgService.DidNotReceive().Error(
        string.Format("Actual output model {0} does not exist", mResult.Output.Path), "Compare");

      mOracle.DidNotReceive().Compare(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void TestCompare_ResultNotSelected_ErrorMessageNotShown_OutputNotCompared()
    {
      mViewModel.CompareCmd.Execute(null);

      mDlgService.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      mOracle.DidNotReceive().Compare(Arg.Any<string>(), Arg.Any<string>());
    }

    #endregion

    #region Export

    [Test]
    public void TestExport_DestinationSelected_ResultsExported()
    {
      mProject.Results.Add(mResult2);

      const string destination = "results.xlsx";
      mDlgService.Save(Arg.Any<string>(), Arg.Any<string>()).Returns(destination);
      mViewModel.ExportCmd.Execute(null);
      Thread.Sleep(TimeSpan.FromSeconds(1));

      mExporter.Received().Init();
      mExporter.Received().Export(mResult);
      mExporter.Received().Export(mResult2);
      mExporter.Received().Save(destination);
    }

    [Test]
    public void TestExport_DestinationNotSelected_ResultsExported()
    {
      mProject.Results.Add(mResult2);

      mDlgService.Save(Arg.Any<string>(), Arg.Any<string>()).Returns(string.Empty);
      mViewModel.ExportCmd.Execute(null);

      mExporter.DidNotReceive().Init();
      mExporter.DidNotReceive().Export(mResult);
      mExporter.DidNotReceive().Export(mResult2);
      mExporter.DidNotReceive().Save(Arg.Any<string>());
    }

    #endregion

    #region Find

    [Test]
    public void TestFind_MutantNameEntered_ResultsExist_FirstResultFound()
    {
      mProject.Results.Add(mResult2);

      mViewModel.FindCmd.Execute(mResult.Mutant.Name);
      Assert.AreEqual(mResult, mViewModel.FoundResult);
    }

    [Test]
    public void TestFind_MutantNameNotEntered_ResultExist_FirstResultNotFound()
    {
      mProject.Results.Add(mResult2);

      mViewModel.FindCmd.Execute(null);
      Assert.IsNull(mViewModel.FoundResult);
    }

    [Test]
    public void TestFind_MutantNameEntered_ResultsDontExist_FirstResultNotFound()
    {
      mProject.Results.Clear();

      mViewModel.FindCmd.Execute(mResult.Mutant.Name);
      Assert.IsNull(mViewModel.FoundResult);
    } 

    #endregion
  }
}
