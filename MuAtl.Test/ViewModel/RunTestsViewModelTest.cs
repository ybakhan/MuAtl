using NUnit.Framework;
using MuAtl.ViewModel;
using MuAtl.Service;
using NSubstitute;
using MuAtl.Model;
using MuAtl.Service.Runner;
using System;
using System.Threading;

namespace MuAtl.Test.ViewModel
{
  [TestFixture]
  public class RunTestsViewModelTest : MuAtlViewModelTestBase
  {
    #region constants

    private const string MutantName = "TestMutant";
    private const string MutantPath = @"TestData\mutant.atl";
    private const string TestCaseName = "SampleTestCase";
    private const string OutputName = "testoutput";
    private const string ExpectedPath = @"TestData\testoutput.xmi";
    private const string InputPath = @"TestData\testinput.xmi";
    private const string InputModelName = "input"; 

    #endregion

    #region instance vars

    private RunTestsViewModel mViewModel;
    private IMutantRunner mRunner;
    private MuAtlMutant mMutant;
    private MuAtlMutant mMutant2;
    private MuAtlTestCase mTestCase;
    private MuAtlTestCase mTestCase2; 

    #endregion

    [SetUp]
    public override void Init()
    {
      base.Init();
      mRunner = Substitute.For<IMutantRunner>();
      mDlgSrvc = Substitute.For<IDialogService>();
      mViewModel = new RunTestsViewModel(mRunner, mDlgSrvc);
      mViewModel.Project = mProject;

      mMutant = new MuAtlMutant
      {
        Name = MutantName,
        Path = MutantPath,
        Type = MutantType.AAM,
        Status = MutantStatus.Undetermined
      };

      mMutant2 = new MuAtlMutant
      {
        Name = MutantName+"2",
        Path = MutantPath,
        Type = MutantType.AAM,
        Status = MutantStatus.Undetermined
      };

      mTestCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath
        }
      };
      mTestCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });

      mTestCase2 = new MuAtlTestCase
      {
        Name = TestCaseName+"2",
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath
        }
      };
      mTestCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
    }

    #region Execute

    [Test]
    public void TestExecute_ProjectHasNoMutants_ErrorMessageShown()
    {
      mViewModel.ExecuteCmd.Execute(null);
      mDlgSrvc.Received().Error(RunTestsViewModel.NoMutantsErrorMsg, RunTestsViewModel.RunTestsCaption);
    }

    [Test]
    public void TestExecute_ProjectHasMutants_ProjectHasNoTestCases_ErrorMessageShown()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.ExecuteCmd.Execute(null);
      mDlgSrvc.Received().Error(RunTestsViewModel.NoTestCasesErrorMsg, RunTestsViewModel.RunTestsCaption);
    }

    [Test]
    public void TestExecute_ProjectHasMutantsAndTestCases_NoMutantsSelected_ErrorMessageShown()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.Project.TestSuite.Add(mTestCase);
      mViewModel.ExecuteCmd.Execute(null);
      mDlgSrvc.Received().Error(RunTestsViewModel.NoMutantsSelectedErrorMsg, RunTestsViewModel.RunTestsCaption);
    }

    [Test]
    public void TestExecute_ProjectHasMutantsAndTestCases_MutantsSelected_NoTestCasesSelected_ErrorMessageShown()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.Project.TestSuite.Add(mTestCase);
      mViewModel.SelectionMutants.SelectAll();

      mViewModel.ExecuteCmd.Execute(null);
      mDlgSrvc.Received().Error(RunTestsViewModel.NoTestCasesSelectedErrorMsg, RunTestsViewModel.RunTestsCaption);
    }

    [Test]
    public void TestExecute_MutantsAndTestCasesSelected_TestsRunForEachMutantTestCasePair()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.Project.Mutants.Add(mMutant2);
      mViewModel.Project.TestSuite.Add(mTestCase);
      mViewModel.Project.TestSuite.Add(mTestCase2);
      mViewModel.SelectionMutants.SelectAll();
      mViewModel.SelectionTestCases.SelectAll();

      mViewModel.ExecuteCmd.Execute(null);
      Thread.Sleep(TimeSpan.FromSeconds(1));

      Assert.IsFalse(mProject.IsRunning);
      Assert.AreEqual(0, mViewModel.ExecutionProgress);
      Assert.AreEqual(string.Empty, mViewModel.ProgressMessage);

      foreach (var mutant in mProject.Mutants)
        foreach (var testCase in mProject.TestSuite)
          mRunner.Received().Run(mutant, testCase);
    } 

    #endregion

    #region Abort

    [Test]
    public void TestAbort_TestRunStarted_AbortConfirmed_TestRunAborted()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.Project.Mutants.Add(mMutant2);
      mViewModel.Project.TestSuite.Add(mTestCase);
      mViewModel.Project.TestSuite.Add(mTestCase2);
      mViewModel.SelectionMutants.SelectAll();
      mViewModel.SelectionTestCases.SelectAll();

      mDlgSrvc.YesNoDialog(RunTestsViewModel.ConfirmAbortion, RunTestsViewModel.AbortCaption).Returns(true);

      mViewModel.ExecuteCmd.Execute(null);
      mViewModel.AbortCmd.Execute(null);
      Thread.Sleep(TimeSpan.FromSeconds(5));
      mDlgSrvc.Received().YesNoDialog(RunTestsViewModel.ConfirmAbortion, RunTestsViewModel.AbortCaption);
      Assert.IsFalse(mProject.IsRunning);
      Assert.AreEqual(0, mViewModel.ExecutionProgress);
      Assert.AreEqual(string.Empty, mViewModel.ProgressMessage);
    }

    [Test]
    public void TestAbort_TestRunStarted_AbortCancelled_TestRunNotAborted()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.Project.Mutants.Add(mMutant2);
      mViewModel.Project.TestSuite.Add(mTestCase);
      mViewModel.Project.TestSuite.Add(mTestCase2);
      mViewModel.SelectionMutants.SelectAll();
      mViewModel.SelectionTestCases.SelectAll();

      mDlgSrvc.YesNoDialog(RunTestsViewModel.ConfirmAbortion, RunTestsViewModel.AbortCaption).Returns(false);

      mViewModel.ExecuteCmd.Execute(null);
      mViewModel.AbortCmd.Execute(null);
      Thread.Sleep(TimeSpan.FromSeconds(1));

      mDlgSrvc.Received().YesNoDialog(RunTestsViewModel.ConfirmAbortion, RunTestsViewModel.AbortCaption);
      Assert.IsFalse(mProject.IsRunning);
      Assert.AreEqual(0, mViewModel.ExecutionProgress);
      Assert.AreEqual(string.Empty, mViewModel.ProgressMessage);

      foreach (var mutant in mProject.Mutants)
        foreach (var testCase in mProject.TestSuite)
          mRunner.Received().Run(mutant, testCase);
    } 

    #endregion
  }
}


