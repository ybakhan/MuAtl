using NUnit.Framework;
using MuAtl.ViewModel;
using MuAtl.Service;
using NSubstitute;
using MuAtl.Model;
using System.Linq;

namespace MuAtl.Test.ViewModel
{
  [TestFixture]
  public class ProjectViewModelTest : MuAtlViewModelTestBase
  {
    #region constants

    private const string MutantName = "TestMutant";
    private const string MutantPath = @"TestData\mutant.atl";
    private const string TestCaseName = "SampleTestCase";
    private const string OutputName = "testoutput";
    private const string ExpectedPath = @"TestData\testoutput.xmi";
    private const string InputPath = @"TestData\testinput.xmi";
    private const string InputModelName = "input";
    private const string TestProjName = "ucm2ad_muatl";
    private readonly string TestProjPath = string.Format(@"TestData\{0}.xml", TestProjName);

    #endregion

    #region instance vars

    private ProjectViewModel mViewModel;
    private MuAtlMutant mMutant;
    private MuAtlMutant mMutant2;
    private MuAtlTestCase mTestCase;
    private MuAtlTestCase mTestCase2;
    private IRepository mRepository; 

    #endregion

    [SetUp]
    public override void Init()
    {
      base.Init();
      mRepository = Substitute.For<IRepository>();
      mViewModel = new ProjectViewModel(mDlgSrvc, mRepository);
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
        Name = MutantName + "2",
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
        Name = TestCaseName + "2",
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

    #region New Project

    [Test]
    public void TestNewProject_ProjectNotRunning_ProjectCleared()
    {
      mProject.IsRunning = false;
      mProject.Mutants.Add(mMutant);
      mProject.Mutants.Add(mMutant2);
      mProject.TestSuite.Add(mTestCase);
      mProject.TestSuite.Add(mTestCase2);

      mViewModel.NewCmd.Execute(null);
      mDlgSrvc.DidNotReceive().YesNoDialog(Arg.Any<string>(), Arg.Any<string>());
      Assert.AreEqual(string.Empty, mProject.Name);
      Assert.IsFalse(mProject.Mutants.Any());
      Assert.IsFalse(mProject.TestSuite.Any());
    }

    [Test]
    public void TestNewProject_ProjectRunning_NewProjectConfirmed_ProjectCleared()
    {
      mProject.IsRunning = true;
      mProject.Mutants.Add(mMutant);
      mProject.Mutants.Add(mMutant2);
      mProject.TestSuite.Add(mTestCase);
      mProject.TestSuite.Add(mTestCase2);

      mDlgSrvc.YesNoDialog(ProjectViewModel.ConfirmNewProj, ProjectViewModel.NewProjCaption).Returns(true);
      mViewModel.NewCmd.Execute(null);
      mDlgSrvc.Received().YesNoDialog(ProjectViewModel.ConfirmNewProj, ProjectViewModel.NewProjCaption);
      Assert.AreEqual(string.Empty, mProject.Name);
      Assert.IsFalse(mProject.IsRunning);
      Assert.IsFalse(mProject.Mutants.Any());
      Assert.IsFalse(mProject.TestSuite.Any());
    }

    [Test]
    public void TestNewProject_ProjectRunning_NewProjectCancelled_ProjectNotCleared()
    {
      mProject.IsRunning = true;
      mProject.Mutants.Add(mMutant);
      mProject.Mutants.Add(mMutant2);
      mProject.TestSuite.Add(mTestCase);
      mProject.TestSuite.Add(mTestCase2);

      mDlgSrvc.YesNoDialog(ProjectViewModel.ConfirmNewProj, ProjectViewModel.NewProjCaption).Returns(false);
      mViewModel.NewCmd.Execute(null);
      mDlgSrvc.Received().YesNoDialog(ProjectViewModel.ConfirmNewProj, ProjectViewModel.NewProjCaption);
      Assert.AreNotEqual(string.Empty, mProject.Name);
      Assert.IsTrue(mProject.IsRunning);
      Assert.IsTrue(mProject.Mutants.Any());
      Assert.IsTrue(mProject.TestSuite.Any());
    }

    #endregion

    #region Open Project

    [Test]
    public void TestOpenProject_ProjectNotRunning_ProjectLoaded()
    {
      mProject.IsRunning = false;
      mDlgSrvc.BrowseXml(ProjectViewModel.OpenProjectMsg).Returns(TestProjPath);
      mRepository.Load<MuAtlProject>(TestProjPath).Returns(new MuAtlProject
      {
        Name = TestProjName,
        Module = @"TestData\ucm2ad.atl"
      });

      mViewModel.OpenCmd.Execute(null);
      mDlgSrvc.DidNotReceive().YesNoDialog(Arg.Any<string>(), Arg.Any<string>());
      mDlgSrvc.Received().BrowseXml(ProjectViewModel.OpenProjectMsg);
      mRepository.Received().Load<MuAtlProject>(TestProjPath);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void TestOpenProject_ProjectRunning_OpenProjectConfirmed_ProjectLoaded()
    {
      mProject.IsRunning = true;
      mDlgSrvc.YesNoDialog(ProjectViewModel.ConfirmOpenProjMsg, ProjectViewModel.OpenProjCaption).Returns(true);
      mDlgSrvc.BrowseXml(ProjectViewModel.OpenProjectMsg).Returns(TestProjPath);
      mRepository.Load<MuAtlProject>(TestProjPath).Returns(new MuAtlProject
      {
        Name = TestProjName,
        Module = "ucm2ad.atl"
      });

      mViewModel.OpenCmd.Execute(null);
      mDlgSrvc.Received().YesNoDialog(ProjectViewModel.ConfirmOpenProjMsg, ProjectViewModel.OpenProjCaption);
      mDlgSrvc.Received().BrowseXml(ProjectViewModel.OpenProjectMsg);
      mRepository.Received().Load<MuAtlProject>(TestProjPath);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void TestOpenProject_ProjectRunning_OpenProjectCancelled_ProjectNotLoaded()
    {
      mProject.IsRunning = true;
      mDlgSrvc.YesNoDialog(ProjectViewModel.ConfirmOpenProjMsg, ProjectViewModel.OpenProjCaption).Returns(false);

      mViewModel.OpenCmd.Execute(null);
      mDlgSrvc.Received().YesNoDialog(ProjectViewModel.ConfirmOpenProjMsg, ProjectViewModel.OpenProjCaption);
      mDlgSrvc.DidNotReceive().BrowseXml(Arg.Any<string>());
      mRepository.DidNotReceive().Load<MuAtlProject>(Arg.Any<string>());
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
    }

    #endregion

    #region Save Project

    [Test]
    public void TestSave_ConfigValid_ProjNotSavedBefore_ProjectSaved()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mDlgSrvc.SaveXml(ProjectViewModel.SaveProjMsg).Returns(TestProjPath);

      mViewModel.SaveCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      mDlgSrvc.Received().SaveXml(ProjectViewModel.SaveProjMsg);
      Assert.AreEqual(mViewModel.ProjectPath, TestProjPath);
      mRepository.Received().Save(TestProjPath, mViewModel.Project);
    }

    [Test]
    public void TestSave_ConfigValid_ProjSavedBefore_ProjectSaved()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mViewModel.ProjectPath = "ucm2ad.atl";

      mViewModel.SaveCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      mRepository.Received().Save(mViewModel.ProjectPath, mViewModel.Project);
    }

    [Test]
    public void TestSave_ConfigValid_ProjNotSavedBefore_SaveFailed_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mDlgSrvc.SaveXml(ProjectViewModel.SaveProjMsg).Returns(TestProjPath);

      mRepository.When(r => r.Save(TestProjPath, mViewModel.Project))
        .Do(r => { throw new System.Exception(); });

      mViewModel.SaveCmd.Execute(null);
      mRepository.Received().Save(TestProjPath, mViewModel.Project);
      mDlgSrvc.Received().Error(Arg.Any<string>(), ProjectViewModel.SaveProjCaption);
    }

    [Test]
    public void TestSave_ConfigValid_ProjSavedBefore_SaveFailed_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mViewModel.ProjectPath = "ucm2ad.atl";
      mRepository.When(r => r.Save(mViewModel.ProjectPath, mViewModel.Project))
        .Do(r => { throw new System.Exception(); });

      mViewModel.SaveCmd.Execute(null);
      mRepository.Received().Save(mViewModel.ProjectPath, mViewModel.Project);
      mDlgSrvc.Received().Error(Arg.Any<string>(), ProjectViewModel.SaveProjCaption);
    }

    [Test]
    public void TestSave_ModuleNotSelected_ShowErrorMessage()
    {
      mProject.Module = null;
      mViewModel.SaveCmd.Execute(null);
      mDlgSrvc.Received().Error(ProjectViewModel.SelectModuleErrorMsg, ProjectViewModel.SaveProjCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Selectd ATL module '{0}' does not exist", mViewModel.Project.Module), ProjectViewModel.SaveAsProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSave_ModuleDoesNotExist_ShowErrorMessage()
    {
      mProject.Module = "X:/../../ucm2ad.atl";
      mViewModel.SaveCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(ProjectViewModel.SelectModuleErrorMsg, ProjectViewModel.SaveProjCaption);
      mDlgSrvc.Received().Error(
        string.Format("Selectd ATL module '{0}' does not exist", mProject.Module), ProjectViewModel.SaveProjCaption);       mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSave_InMmPathNotSet_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mViewModel.Project.Dependency.InMetamodels[0].Path = string.Empty;

      mViewModel.SaveCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(ProjectViewModel.SelectModuleErrorMsg, ProjectViewModel.SaveProjCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Selectd ATL module '{0}' does not exist", mViewModel.Project.Module), ProjectViewModel.SaveAsProjCaption);
      mDlgSrvc.Received().Error(
        string.Format("Enter path of input metamodel {0}\n",
          mViewModel.Project.Dependency.InMetamodels[0].Name)
        , ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSave_InMmDoesNotExist_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      const string invalidPath = "X:/../../ucm.ecore";
      mViewModel.Project.Dependency.InMetamodels[0].Path = invalidPath;

      mViewModel.SaveCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(ProjectViewModel.SelectModuleErrorMsg, ProjectViewModel.SaveProjCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Selectd ATL module '{0}' does not exist", mViewModel.Project.Module), ProjectViewModel.SaveAsProjCaption);
      mDlgSrvc.Received().Error(
        string.Format("Input metamodel {0} does not exist\n", invalidPath,
          mViewModel.Project.Dependency.InMetamodels[0].Name)
        , ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSave_OutMmPathNotSet_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mViewModel.Project.Dependency.OutMetamodel.Path = null;

      mViewModel.SaveCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(ProjectViewModel.SelectModuleErrorMsg, ProjectViewModel.SaveProjCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Selectd ATL module '{0}' does not exist", mViewModel.Project.Module), ProjectViewModel.SaveAsProjCaption);
      mDlgSrvc.Received().Error(
        string.Format("Enter path of output metamodel {0}\n",
          mViewModel.Project.Dependency.OutMetamodel.Name)
        , ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSave_OutMmPathInvalid_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      const string invalidPath = "X:/../../uml.ecore";
      mViewModel.Project.Dependency.OutMetamodel.Path = invalidPath;

      mViewModel.SaveCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(ProjectViewModel.SelectModuleErrorMsg, ProjectViewModel.SaveProjCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Selectd ATL module '{0}' does not exist", mViewModel.Project.Module), ProjectViewModel.SaveAsProjCaption);
      mDlgSrvc.Received().Error(
        string.Format("Output metamodel {0} does not exist\n", invalidPath),
        ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSave_LibPathNotSet_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mViewModel.Project.Dependency.Libraries[0].Path = null;

      mViewModel.SaveCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(ProjectViewModel.SelectModuleErrorMsg, ProjectViewModel.SaveProjCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Selectd ATL module '{0}' does not exist", mViewModel.Project.Module), ProjectViewModel.SaveAsProjCaption);
      mDlgSrvc.Received().Error(
        string.Format("Enter path of ATL library module {0}\n",
        mViewModel.Project.Dependency.Libraries[0].Name),
        ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSave_LibPathInvalid_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      const string invalidPath = "X:/../../lib.atl";
      mViewModel.Project.Dependency.Libraries[0].Path = invalidPath;

      mViewModel.SaveCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(ProjectViewModel.SelectModuleErrorMsg, ProjectViewModel.SaveProjCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Selectd ATL module '{0}' does not exist", mViewModel.Project.Module), ProjectViewModel.SaveAsProjCaption);
      mDlgSrvc.Received().Error(
        string.Format("ATL library module {0} does not exist\n",
        invalidPath),
        ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    #endregion

    #region Save As

    [Test]
    public void TestSaveAs_ConfigValid_ProjectSaved()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mDlgSrvc.SaveXml(ProjectViewModel.SaveAsProjMsg).Returns(TestProjPath);

      mViewModel.SaveAsCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.AreEqual(mViewModel.ProjectPath, TestProjPath);
      mRepository.Received().Save(TestProjPath, mViewModel.Project);
    }

    [Test]
    public void TestSaveAs_ConfigValid_SaveFailed_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mDlgSrvc.SaveXml(ProjectViewModel.SaveAsProjMsg).Returns(TestProjPath);

      mRepository.When(r => r.Save(TestProjPath, mViewModel.Project))
        .Do(r => { throw new System.Exception(); });

      mViewModel.SaveAsCmd.Execute(null);
      mRepository.Received().Save(TestProjPath, mViewModel.Project);
      mDlgSrvc.Received().Error(Arg.Any<string>(), ProjectViewModel.SaveProjCaption);
    }

    [Test]
    public void TestSaveAs_ModuleNotSelected_ShowErrorMessage()
    {
      mProject.Module = null;
      mViewModel.SaveAsCmd.Execute(null);
      mDlgSrvc.Received().Error(ProjectViewModel.SelectModuleErrorMsg, ProjectViewModel.SaveAsProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSaveAs_ModuleDoesNotExist_ShowErrorMessage()
    {
      mProject.Module = "X:/../../ucm2ad.atl";
      mViewModel.SaveAsCmd.Execute(null);
      mDlgSrvc.Received().Error(
        string.Format("Selectd ATL module '{0}' does not exist", mProject.Module), ProjectViewModel.SaveAsProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSaveAs_InMmPathNotSet_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mViewModel.Project.Dependency.InMetamodels[0].Path = string.Empty;

      mViewModel.SaveAsCmd.Execute(null);
      mDlgSrvc.Received().Error(
        string.Format("Enter path of input metamodel {0}\n",
          mViewModel.Project.Dependency.InMetamodels[0].Name)
        , ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSaveAs_InMmDoesNotExist_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      const string invalidPath = "X:/../../ucm.ecore";
      mViewModel.Project.Dependency.InMetamodels[0].Path = invalidPath;

      mViewModel.SaveAsCmd.Execute(null);
      mDlgSrvc.Received().Error(
        string.Format("Input metamodel {0} does not exist\n", invalidPath,
          mViewModel.Project.Dependency.InMetamodels[0].Name)
        , ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSaveAs_OutMmPathNotSet_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mViewModel.Project.Dependency.OutMetamodel.Path = null;

      mViewModel.SaveAsCmd.Execute(null);
      mDlgSrvc.Received().Error(
        string.Format("Enter path of output metamodel {0}\n",
          mViewModel.Project.Dependency.OutMetamodel.Name)
        , ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSaveAs_OutMmPathInvalid_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      const string invalidPath = "X:/../../uml.ecore";
      mViewModel.Project.Dependency.OutMetamodel.Path = invalidPath;

      mViewModel.SaveAsCmd.Execute(null);
      mDlgSrvc.Received().Error(
        string.Format("Output metamodel {0} does not exist\n", invalidPath),
        ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSaveAs_LibPathNotSet_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      mViewModel.Project.Dependency.Libraries[0].Path = null;

      mViewModel.SaveAsCmd.Execute(null);
      mDlgSrvc.Received().Error(
        string.Format("Enter path of ATL library module {0}\n",
        mViewModel.Project.Dependency.Libraries[0].Name),
        ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    [Test]
    public void TestSaveAs_LibPathInvalid_ShowErrorMessage()
    {
      mViewModel.Project = new Repository().Load<MuAtlProject>(TestProjPath);
      const string invalidPath = "X:/../../lib.atl";
      mViewModel.Project.Dependency.Libraries[0].Path = invalidPath;

      mViewModel.SaveAsCmd.Execute(null);
      mDlgSrvc.Received().Error(
        string.Format("ATL library module {0} does not exist\n",
        invalidPath),
        ProjectViewModel.SaveProjCaption);
      mRepository.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<MuAtlProject>());
    }

    #endregion
  }
}
