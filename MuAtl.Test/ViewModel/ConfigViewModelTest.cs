using NUnit.Framework;
using MuAtl.ViewModel;
using MuAtl.Service;
using NSubstitute;
using MuAtl.Model;
using System.Linq;

namespace MuAtl.Test.ViewModel
{
  [TestFixture]
  public class ConfigViewModelTest : MuAtlViewModelTestBase
  {
    #region instance vars

    private ConfigViewModel mViewModel;
    private ProjectViewModel mProjectViewModel;
    private IAtlParser mParser;
    private IRepository mRepository;
    private Metamodel mInMetamodel;
    private Metamodel mDepMetamodel;

    #endregion

    #region constants

    private const string InputMetamodelPath = "X:/../../../ucm.ecore";
    private const string OutputMetamodelPath = "X:/../../../uml.ecore";
    private const string LibPath = "X:/../../../testlib.atl";
    private const string SuperimposedModuleName = "superimposed";
    private const string SuperimposedModulePath = @"TestData\superimposed.atl";

    private const string UpdatedSuperimposedModuleName = "Updated Name";
    private const string UpdatedSuperimposedModulePath = @"TestData\superimposed-update.atl"; 

    #endregion

    [SetUp]
    public override void Init()
    {
      base.Init();
      mDlgSrvc = Substitute.For<IDialogService>();
      mParser = Substitute.For<IAtlParser>();
      mRepository = Substitute.For<IRepository>();
      mViewModel = new ConfigViewModel(mDlgSrvc, mParser, mRepository);
      mProjectViewModel = new ProjectViewModel(mDlgSrvc, mRepository);
      mViewModel.Project = mProject;
      mProjectViewModel.Project = mProject;

      mInMetamodel = new Metamodel
      {
        Name = "UCM",
        Path = InputMetamodelPath
      };

      mDepMetamodel = new Metamodel
      {
        Name = "URN",
        Path = "X:/../../../urn.ecore"
      };
    }

    #region Select Module

    [Test]
    public void TestSelectModule_ProjectNotSavedBefore_ModuleSelected()
    {
      var expected = "X:/../../../test.atl";
      mDlgSrvc.BrowseModule().Returns(expected);
      mViewModel.SelectModuleCmd.Execute(null);
      mDlgSrvc.DidNotReceive().YesNoDialog(Arg.Any<string>(), Arg.Any<string>());
      Assert.AreEqual(expected, mViewModel.Project.Module);
    }

    [Test]
    public void TestSelectModule_ProjectNotSavedBefore_ModuleSelectCancelled_ModuleNotSelected()
    {
      mViewModel.SelectModuleCmd.Execute(null);
      mDlgSrvc.Received().BrowseModule();
      Assert.IsNull(mViewModel.Project.Module);
    }

    [Test]
    public void TestSelectModule_ProjectSavedBefore_CreateNewProject_ProjectCleared_ModuleSelected()
    {
      mViewModel.PersistentProj = true;
      mDlgSrvc.YesNoDialog(ConfigViewModel.ModuleUpdateMsg, ConfigViewModel.SelectModuleCaption).Returns(true);

      var expected = "X:/../../../test.atl";
      mDlgSrvc.BrowseModule().Returns(expected);

      mViewModel.SelectModuleCmd.Execute(null);
      mDlgSrvc.Received().YesNoDialog(ConfigViewModel.ModuleUpdateMsg, ConfigViewModel.SelectModuleCaption);
      mDlgSrvc.Received().BrowseModule();
      Assert.AreEqual(string.Empty, mViewModel.Project.Name); //project cleared
      Assert.AreEqual(expected, mViewModel.Project.Module);
    }

    [Test]
    public void TestSelectModule_ProjectSavedPefore_DontCreateNewProject_ProjectNotCleated_ModuleNotSelected()
    {
      mViewModel.PersistentProj = true;
      mDlgSrvc.YesNoDialog(ConfigViewModel.ModuleUpdateMsg, ConfigViewModel.SelectModuleCaption).Returns(false);

      mViewModel.SelectModuleCmd.Execute(null);
      mDlgSrvc.Received().YesNoDialog(ConfigViewModel.ModuleUpdateMsg, ConfigViewModel.SelectModuleCaption);
      Assert.AreEqual("Test Project", mViewModel.Project.Name); //project not cleared
      Assert.AreEqual(null, mViewModel.Project.Module); //module not selected
    }

    #endregion

    #region Select Input Metamodel

    [Test]
    public void TestSelectInputMetamodel_InputMetamodelSelected()
    {
      var inputMm = new Metamodel
      {
        Name = "UCM"
      };
      mDlgSrvc.BrowseMetamodel(ConfigViewModel.InputMetamodelTitle).Returns(InputMetamodelPath);

      mViewModel.SelectInMmCmd.Execute(inputMm);
      mDlgSrvc.Received().BrowseMetamodel(ConfigViewModel.InputMetamodelTitle);
      Assert.AreEqual(InputMetamodelPath, inputMm.Path);
    }

    [Test]
    public void TestSelectInputMetamodel_SelectionCancelled_InputMetamodelNotSelected()
    {
      var inputMm = new Metamodel
      {
        Name = "UCM"
      };
      mDlgSrvc.BrowseMetamodel(ConfigViewModel.InputMetamodelTitle).Returns(string.Empty);

      mViewModel.SelectInMmCmd.Execute(inputMm);
      mDlgSrvc.Received().BrowseMetamodel(ConfigViewModel.InputMetamodelTitle);
      Assert.IsNull(inputMm.Path);
    } 

    #endregion

    #region Add Dependent Metamodel

    [Test]
    public void TestAddDependentMetamodel_DependentMetamodelDoesNotExist_DependentMetamodelAdded()
    {
      mViewModel.Project.Dependency.InMetamodels.Add(mInMetamodel);
      mDlgSrvc.BrowseMetamodel(ConfigViewModel.DependentMetamodelMsg + " " + mInMetamodel.Name).Returns(mDepMetamodel.Path);

      mViewModel.AddDpndntMmCmd.Execute(mInMetamodel);
      mDlgSrvc.Received().BrowseMetamodel(ConfigViewModel.DependentMetamodelMsg + " " + mInMetamodel.Name);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsTrue(mInMetamodel.Dependencies.Contains(mDepMetamodel));
    }

    [Test]
    public void TestAddDependentMetamodel_DependentMetamodelExist_UpdateExistingNo_DependentMetamodelNotAdded()
    {
      mViewModel.Project.Dependency.InMetamodels.Add(mInMetamodel);
      mInMetamodel.Dependencies.Add(mDepMetamodel);
      mDlgSrvc.BrowseMetamodel(string.Format("{0} {1}", ConfigViewModel.DependentMetamodelMsg, mInMetamodel.Name)).Returns(mDepMetamodel.Path);
      mDlgSrvc.YesNoDialog(string.Format("Dependent metamodel {0} already exists. Update existing?", mDepMetamodel.Name), ConfigViewModel.AddDpndntMmCaption).Returns(false);

      mViewModel.AddDpndntMmCmd.Execute(mInMetamodel);
      mDlgSrvc.Received().BrowseMetamodel(string.Format("{0} {1}", ConfigViewModel.DependentMetamodelMsg, mInMetamodel.Name));
      mDlgSrvc.Received().YesNoDialog(string.Format("Dependent metamodel {0} already exists. Update existing?", mDepMetamodel.Name), ConfigViewModel.AddDpndntMmCaption);
      Assert.IsTrue(mInMetamodel.Dependencies.Contains(mDepMetamodel));
      Assert.AreEqual(1, mInMetamodel.Dependencies.Count());
    }

    [Test]
    public void TestAddDependentMetamodel_DependentMetamodelExist_UpdateExistingYes_DependentMetamodelUpdated()
    {
      mViewModel.Project.Dependency.InMetamodels.Add(mInMetamodel);
      mInMetamodel.Dependencies.Add(mDepMetamodel);
      mDlgSrvc.BrowseMetamodel(string.Format("{0} {1}", ConfigViewModel.DependentMetamodelMsg, mInMetamodel.Name)).Returns(mDepMetamodel.Path);
      mDlgSrvc.YesNoDialog(string.Format("Dependent metamodel {0} already exists. Update existing?", mDepMetamodel.Name), ConfigViewModel.AddDpndntMmCaption).Returns(true);

      mViewModel.AddDpndntMmCmd.Execute(mInMetamodel);
      mDlgSrvc.Received().BrowseMetamodel(string.Format("{0} {1}", ConfigViewModel.DependentMetamodelMsg, mInMetamodel.Name));
      mDlgSrvc.Received().YesNoDialog(string.Format("Dependent metamodel {0} already exists. Update existing?", mDepMetamodel.Name), ConfigViewModel.AddDpndntMmCaption);
      Assert.IsTrue(mInMetamodel.Dependencies.Contains(mDepMetamodel));
      Assert.AreEqual(1, mInMetamodel.Dependencies.Count());
    }

    [Test]
    public void TestAddDependentMetamodel_DependentMetamodelEqualInMetamodel_DependentMetamodelNotAdded()
    {
      mViewModel.Project.Dependency.InMetamodels.Add(mInMetamodel);
      mDlgSrvc.BrowseMetamodel(string.Format("{0} {1}", ConfigViewModel.DependentMetamodelMsg, mInMetamodel.Name)).Returns(mInMetamodel.Path);

      mViewModel.AddDpndntMmCmd.Execute(mInMetamodel);
      mDlgSrvc.Received().BrowseMetamodel(string.Format("{0} {1}", ConfigViewModel.DependentMetamodelMsg, mInMetamodel.Name));
      mDlgSrvc.Received().Error(string.Format("{0} {1}", ConfigViewModel.AddDpndntMmFailMsg, mInMetamodel.Name), ConfigViewModel.AddDpndntMmCaption);
      mDlgSrvc.DidNotReceive().YesNoDialog(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsFalse(mInMetamodel.Dependencies.Any());
    }

    [Test]
    public void TestAddDependentMetamodel_BrowseCanceled_DependentMetamodelNotAdded()
    {
      mViewModel.Project.Dependency.InMetamodels.Add(mInMetamodel);
      mViewModel.AddDpndntMmCmd.Execute(mInMetamodel);
      mDlgSrvc.Received().BrowseMetamodel(string.Format("{0} {1}", ConfigViewModel.DependentMetamodelMsg, mInMetamodel.Name));
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      mDlgSrvc.DidNotReceive().YesNoDialog(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsFalse(mInMetamodel.Dependencies.Any());
    }

    #endregion

    #region Delete Dependent Metamodel

    [Test]
    public void TestDeleteDependentMetamodel_DependentMetamodelSelected_DependentMetamodelExists_DependentMetamodelRemoved()
    {
      mViewModel.Project.Dependency.InMetamodels.Add(mInMetamodel);
      mInMetamodel.Dependencies.Add(mDepMetamodel);
      mViewModel.SelectedDepMm = mDepMetamodel;

      mViewModel.DelDpndntMmCmd.Execute(mInMetamodel);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsFalse(mInMetamodel.Dependencies.Any());
    }

    [Test]
    public void TestDeleteDependentMetamodel_DependentMetamodelSelected_DependentMetamodelDoesNotExists_DependentMetamodelNotRemoved()
    {
      mViewModel.Project.Dependency.InMetamodels.Add(mInMetamodel);
      mViewModel.SelectedDepMm = mDepMetamodel;

      mViewModel.DelDpndntMmCmd.Execute(mInMetamodel);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsFalse(mInMetamodel.Dependencies.Any());
    }

    [Test]
    public void TestDeleteDependentMetamodel_DependentMetamodelNotSelected_DependentMetamodelExists_DependentMetamodelNotRemoved()
    {
      mViewModel.Project.Dependency.InMetamodels.Add(mInMetamodel);
      mInMetamodel.Dependencies.Add(mDepMetamodel);
      mViewModel.DelDpndntMmCmd.Execute(mInMetamodel);
      mDlgSrvc.Received().Error(ConfigViewModel.SelectDpndntMmMsg, ConfigViewModel.DelDpndntMmCaption);
      Assert.AreEqual(1, mInMetamodel.Dependencies.Count());
    }

    [Test]
    public void TestDeleteDependentMetamodel_DependentMetamodelNotSeleted_DependentMetamodelDoesNotExist_DependentMetamodelNotRemoved()
    {
      mViewModel.Project.Dependency.InMetamodels.Add(mInMetamodel);
      mViewModel.DelDpndntMmCmd.Execute(mInMetamodel);
      mDlgSrvc.Received().Error(ConfigViewModel.SelectDpndntMmMsg, ConfigViewModel.DelDpndntMmCaption);
      Assert.IsFalse(mInMetamodel.Dependencies.Any());
    }

    #endregion

    #region Select Output Metamodel

    [Test]
    public void TestOutputMetamodel_OutputMetamodelSelected()
    {
      var outMetamodeL = new Metamodel
      {
        Name = "UML"
      };
      mViewModel.Project.Dependency.OutMetamodel = outMetamodeL;
      mDlgSrvc.BrowseMetamodel(ConfigViewModel.OutMmMsg).Returns(OutputMetamodelPath);
      mViewModel.SelectOutMmCmd.Execute(null);
      mDlgSrvc.Received().BrowseMetamodel(ConfigViewModel.OutMmMsg);
      Assert.AreEqual(OutputMetamodelPath, outMetamodeL.Path);
    }

    [Test]
    public void TestOutputMetamodel_SelectCancelled_OutputMetamodelNotSelected()
    {
      var outMetamodeL = new Metamodel
      {
        Name = "UML"
      };
      mViewModel.Project.Dependency.OutMetamodel = outMetamodeL;
      mDlgSrvc.BrowseMetamodel(ConfigViewModel.OutMmMsg).Returns(string.Empty);

      mViewModel.SelectOutMmCmd.Execute(null);
      mDlgSrvc.Received().BrowseMetamodel(ConfigViewModel.OutMmMsg);
      Assert.IsNull(outMetamodeL.Path);
    }

    #endregion

    #region Select Library

    [Test]
    public void TestSelectLib_LibrarySelected()
    {
      var lib = new AtlLibrary
      {
        Name = "Test Lib"
      };
      mDlgSrvc.BrowseAtl(ConfigViewModel.LibMsg).Returns(LibPath);

      mViewModel.SelectLibCmd.Execute(lib);
      mDlgSrvc.Received().BrowseAtl(ConfigViewModel.LibMsg);
      Assert.AreEqual(LibPath, lib.Path);
    }

    [Test]
    public void TestSelectLib_SelectCancelled_LibraryNotSelected()
    {
      var lib = new AtlLibrary
      {
        Name = "Test Lib"
      };
      mDlgSrvc.BrowseAtl(ConfigViewModel.LibMsg).Returns(string.Empty);

      mViewModel.SelectLibCmd.Execute(lib);
      mDlgSrvc.Received().BrowseAtl(ConfigViewModel.LibMsg);
      Assert.IsNull(lib.Path);
    }

    #endregion

    #region Add Superimposed Module

    [Test]
    public void AddSuperimposedModule_FormValid_SuperimposedModuleAdded_FormCleared()
    {
      mViewModel.SupModName = SuperimposedModuleName;
      mViewModel.SupModPath = SuperimposedModulePath;

      mViewModel.AddSupModCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      var supMod = new SuperImposedModule
      {
        Name = SuperimposedModuleName,
        Path = SuperimposedModulePath
      };
      Assert.IsTrue(mViewModel.Project.Dependency.SuperImposedModules.Contains(supMod));
      Assert.AreEqual(string.Empty, mViewModel.SupModName);
      Assert.AreEqual(string.Empty, mViewModel.SupModPath);
    }

    [Test]
    public void AddSuperimposedModule_FormInvalid_SuperimposedModuleNotAdded_FormNotCleared()
    {
      mViewModel.SupModName = string.Empty;
      mViewModel.SupModPath = SuperimposedModulePath;

      mViewModel.AddSupModCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsFalse(mViewModel.Project.Dependency.SuperImposedModules.Any());
      Assert.AreEqual(string.Empty, mViewModel.SupModName);
      Assert.AreEqual(SuperimposedModulePath, mViewModel.SupModPath);
    }

    [Test]
    public void AddSuperimposedModule_SuperimposedModuleExists_SuperimposedModuleNotChanged_SuperimposedModuleNotAdded_FormNotCleared()
    {
      var superImposedModule = new SuperImposedModule
      {
        Name = SuperimposedModuleName,
        Path = SuperimposedModulePath
      };
      mViewModel.Project.Dependency.AddSuperImposedModule(superImposedModule);

      mViewModel.SupModName = SuperimposedModuleName;
      mViewModel.SupModPath = UpdatedSuperimposedModulePath;

      mViewModel.AddSupModCmd.Execute(null);
      mDlgSrvc.Received().Error(string.Format("Cannot add superimposed module {0} because it already exists", SuperimposedModuleName), ConfigViewModel.AppSupModCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Superimposed module {0} does not exist", SuperimposedModulePath),
         ConfigViewModel.AppSupModCaption);
      Assert.AreEqual(SuperimposedModulePath, superImposedModule.Path); //path not changed
      Assert.AreEqual(1, mViewModel.Project.Dependency.SuperImposedModules.Count());
      Assert.AreEqual(SuperimposedModuleName, mViewModel.SupModName);
      Assert.AreEqual(UpdatedSuperimposedModulePath, mViewModel.SupModPath);
    }

    [Test]
    public void AddSuperimposedModule_ModuleDoesNotExist_SuperimposedModuleNotAdded_FormNotCleared()
    {
      mViewModel.SupModName = SuperimposedModuleName;
      const string invalidPath = "X:/../../../superimposed.atl";
      mViewModel.SupModPath = invalidPath;

      mViewModel.AddSupModCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(string.Format("Cannot add superimposed module {0} because it already exists", SuperimposedModuleName), ConfigViewModel.AppSupModCaption);
      mDlgSrvc.Received().Error(string.Format("Superimposed module {0} does not exist", invalidPath),
         ConfigViewModel.AppSupModCaption);
      Assert.IsFalse(mViewModel.Project.Dependency.SuperImposedModules.Any());
      Assert.AreEqual(SuperimposedModuleName, mViewModel.SupModName);
      Assert.AreEqual(invalidPath, mViewModel.SupModPath);
    }

    #endregion

    #region Delete Superimposed Module

    [Test]
    public void TestDeleteSuperimposedModule_SuperimposedModuleSelected_SuperimposedModuleRemoved_FormCleared()
    {
      var superImposedModule = new SuperImposedModule
      {
        Name = SuperimposedModuleName,
        Path = SuperimposedModulePath
      };
      mViewModel.Project.Dependency.SuperImposedModules.Add(superImposedModule);
      mViewModel.SelectedSupMod = superImposedModule;

      mViewModel.DelSupModCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsFalse(mViewModel.Project.Dependency.SuperImposedModules.Contains(superImposedModule));
      Assert.AreEqual(string.Empty, mViewModel.SupModName);
      Assert.AreEqual(string.Empty, mViewModel.SupModPath);
    }

    [Test]
    public void TestDeleteSuperimposedModule_SuperimposedModuleNotSelected_SuperimposedModuleNotRemoved_FormNotCleared()
    {
      var superImposedModule = new SuperImposedModule
      {
        Name = SuperimposedModuleName,
        Path = SuperimposedModulePath
      };
      mViewModel.Project.Dependency.SuperImposedModules.Add(superImposedModule);
      mViewModel.SelectedSupMod = null;

      mViewModel.SupModName = SuperimposedModuleName;
      mViewModel.SupModPath = SuperimposedModulePath;

      mViewModel.DelSupModCmd.Execute(null);
      mDlgSrvc.Received().Error(ConfigViewModel.SelectSupModMsg, ConfigViewModel.DelSupModCaption);
      Assert.IsTrue(mViewModel.Project.Dependency.SuperImposedModules.Contains(superImposedModule));
      Assert.AreEqual(SuperimposedModuleName, mViewModel.SupModName);
      Assert.AreEqual(SuperimposedModulePath, mViewModel.SupModPath);
    }

    #endregion

    #region Update Superimposed Module

    [Test]
    public void TestUpdateSuperimposedModule_FormValid_SuperimposedModuleSelected_SuperimposedModuleUpdated()
    {
      var superImposedModule = new SuperImposedModule
      {
        Name = SuperimposedModuleName,
        Path = SuperimposedModulePath
      };
      mViewModel.Project.Dependency.SuperImposedModules.Add(superImposedModule);
      mViewModel.SelectedSupMod = superImposedModule;

      mViewModel.SupModName = UpdatedSuperimposedModuleName;
      mViewModel.SupModPath = UpdatedSuperimposedModulePath;

      mViewModel.UpdateSupModCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.AreEqual(UpdatedSuperimposedModuleName, superImposedModule.Name);
      Assert.AreEqual(UpdatedSuperimposedModulePath, superImposedModule.Path);
    }

    [Test]
    public void TestUpdateSuperimposedModule_FormValid_SuperimposedModuleNotSelected_SuperimposedModuleNotUpdated()
    {
      var superImposedModule = new SuperImposedModule
      {
        Name = SuperimposedModuleName,
        Path = SuperimposedModulePath
      };
      mViewModel.Project.Dependency.SuperImposedModules.Add(superImposedModule);
      mViewModel.SelectedSupMod = null;

      mViewModel.SupModName = UpdatedSuperimposedModuleName;
      mViewModel.SupModPath = UpdatedSuperimposedModulePath;

      mViewModel.UpdateSupModCmd.Execute(null);
      mDlgSrvc.Received().Error(ConfigViewModel.SelectSupModMsg, ConfigViewModel.UpdateSupModCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Superimposed module {0} already exists", SuperimposedModuleName), ConfigViewModel.UpdateSupModCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Superimposed module {0} does not exist", SuperimposedModulePath), ConfigViewModel.UpdateSupModCaption);
      Assert.AreEqual(SuperimposedModuleName, superImposedModule.Name);
      Assert.AreEqual(SuperimposedModulePath, superImposedModule.Path);
    }

    [Test]
    public void TestUpdateSuperimposedModule_FormInvalid_SuperimposedModuleSelected_SuperimposedModuleNotUpdated()
    {
      var superImposedModule = new SuperImposedModule
      {
        Name = SuperimposedModuleName,
        Path = SuperimposedModulePath
      };
      mViewModel.Project.Dependency.SuperImposedModules.Add(superImposedModule);
      mViewModel.SelectedSupMod = superImposedModule;

      mViewModel.SupModName = string.Empty;
      mViewModel.SupModPath = string.Empty;

      mViewModel.UpdateSupModCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.AreEqual(SuperimposedModuleName, superImposedModule.Name);
      Assert.AreEqual(SuperimposedModulePath, superImposedModule.Path);
    }

    [Test]
    public void TestUpdateSuperimposedModule_FormValid_SuperimposedModuleExists_SuperimposedModuleNotUpdated()
    {
      var superImposedModule1 = new SuperImposedModule
      {
        Name = SuperimposedModuleName,
        Path = SuperimposedModulePath
      };

      const string otherName = "other superimposed module";
      const string otherPath = "superimposed-update.atl";

      var superImposedModule2 = new SuperImposedModule
      {
        Name = otherName,
        Path = otherPath
      };

      mViewModel.Project.Dependency.SuperImposedModules.Add(superImposedModule1);
      mViewModel.Project.Dependency.SuperImposedModules.Add(superImposedModule2);

      mViewModel.SelectedSupMod = superImposedModule2;

      mViewModel.SupModName = SuperimposedModuleName; //update name of other to name of first
      mViewModel.SupModPath = otherPath;

      mViewModel.UpdateSupModCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(ConfigViewModel.SelectSupModMsg, ConfigViewModel.UpdateSupModCaption);
      mDlgSrvc.Received().Error(string.Format("Superimposed module {0} already exists", SuperimposedModuleName), ConfigViewModel.UpdateSupModCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Superimposed module {0} does not exist", SuperimposedModulePath), ConfigViewModel.UpdateSupModCaption);
      Assert.AreEqual(otherName, superImposedModule2.Name);
      Assert.AreEqual(otherPath, superImposedModule2.Path);
    }

    [Test]
    public void TestUpdateSuperimposedModule_FormValid_SuperimposedModuleFileDoesNotExist_SuperimposedModuleNotUpdated()
    {
      var superImposedModule = new SuperImposedModule
      {
        Name = SuperimposedModuleName,
        Path = SuperimposedModulePath
      };
      mViewModel.Project.Dependency.SuperImposedModules.Add(superImposedModule);
      mViewModel.SelectedSupMod = superImposedModule;

      mViewModel.SupModName = UpdatedSuperimposedModuleName;
      mViewModel.SupModPath = "X:/../../superimposedModule.atl";

      mViewModel.UpdateSupModCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(ConfigViewModel.SelectSupModMsg, ConfigViewModel.UpdateSupModCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Superimposed module {0} already exists", SuperimposedModuleName), ConfigViewModel.UpdateSupModCaption);
      mDlgSrvc.Received().Error(string.Format("Superimposed module {0} does not exist", mViewModel.SupModPath), ConfigViewModel.UpdateSupModCaption);
      Assert.AreEqual(SuperimposedModuleName, superImposedModule.Name);
      Assert.AreEqual(SuperimposedModulePath, superImposedModule.Path);
    }

    #endregion

    #region Select Superimposed Module

    [Test]
    public void TestSelectSuperimposedModule_NameIsSet_PathUpdatedOnForm()
    {
      mDlgSrvc.BrowseAtl(ConfigViewModel.SupModMsg).Returns(SuperimposedModulePath);
      mViewModel.SupModName = SuperimposedModuleName;

      mViewModel.SelectSupModCmd.Execute(null);
      mDlgSrvc.Received().BrowseAtl(ConfigViewModel.SupModMsg);
      Assert.AreEqual(SuperimposedModulePath, mViewModel.SupModPath);
    }

    [Test]
    public void TestSelectSuperimposedModule_NameNotSet_NameSetOnForm_PathUpdatedOnForm()
    {
      mDlgSrvc.BrowseAtl(ConfigViewModel.SupModMsg).Returns(SuperimposedModulePath);

      mViewModel.SelectSupModCmd.Execute(null);
      mDlgSrvc.Received().BrowseAtl(ConfigViewModel.SupModMsg);
      Assert.AreEqual(SuperimposedModuleName, mViewModel.SupModName);
      Assert.AreEqual(SuperimposedModulePath, mViewModel.SupModPath);
    }

    #endregion

    #region Table Select Superimposed Module

    [Test]
    public void TestOnTableSelectSuperimposedModule_SuperimposedModuleSelected_FormUpdated()
    {
      mViewModel.SelectedSupMod = new SuperImposedModule
      {
        Name = SuperimposedModuleName,
        Path = SuperimposedModulePath
      };

      mViewModel.OnTableSelectCmd.Execute(null);
      Assert.AreEqual(SuperimposedModuleName, mViewModel.SupModName);
      Assert.AreEqual(SuperimposedModulePath, mViewModel.SupModPath);
    }

    [Test]
    public void TestOnTableSelectedSuperimposedModule_SuperimposedModuleNotSelected_FormNotUpdated()
    {
      mViewModel.OnTableSelectCmd.Execute(null);
      Assert.IsNull(mViewModel.SupModName);
      Assert.IsNull(mViewModel.SupModPath);
    } 

    #endregion
  }
}
