using MuAtl.Model;
using MuAtl.Service;
using MuAtl.Service.Generator;
using MuAtl.Service.Reader.Model;
using MuAtl.ViewModel;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace MuAtl.Test.ViewModel
{
  [TestFixture]
  public class MutantsViewModelTest : MuAtlViewModelTestBase
  {
    #region instance vars

    private MutantsViewModel mViewModel;
    private IMutantGenerator mGenerator;
    private MuAtlMutant mMutant;
    private CstCandidate mCstCandidate;
    private CttCandidate mCttCandidate;
    private AamCandidate mAamCandidate;
    private AfeCandidate mAfeCandidate;
    private IDiffer mDiffSrvc;

    #endregion

    #region constant

    private const string MutantName = "TestMutant";
    private const string MutantPath = @"TestData\mutant.atl";
    private const string InvalidPath = "X:/../../../mutant.atl";
    private const string UpdatedMutantPath = @"TestData\mutant2.atl";
    private const string UpdatedMutantName = "Mutant2";
    private const string MutantSrcType = "UCM!\"ucm::map::MutantType";
    private const string MutantSrcType2 = "UCM!\"ucm::map::MutantType2";
    private const string MutantTargetType = "UML!MutantType";
    private const string MutantTargetType2 = "UML!MutantType2";
    private const string SrcType = "UCM!\"ucm::map::TestType";
    private const string SrcType2 = "UCM!\"ucm::map::TestType2";
    private const string TargetType = "UML!TestType";
    private const string TargetType2 = "UML!TestType2";
    private const string MuAttrMapping = "a <- b.x";
    private const string MuFltrExpr = "a.x > 1";

    #endregion

    [SetUp]
    public override void Init()
    {
      base.Init();
      mGenerator = Substitute.For<IMutantGenerator>();
      var parser = Substitute.For<IAtlParser>();
      mDiffSrvc = Substitute.For<IDiffer>();
      mViewModel = new MutantsViewModel(mDlgSrvc, mGenerator, parser, mDiffSrvc);
      mViewModel.Project = mProject;

      mMutant = new MuAtlMutant
      {
        Name = MutantName,
        Path = MutantPath,
        Type = MutantType.AAM,
        Status = MutantStatus.Undetermined
      };

      mCstCandidate = new CstCandidate
      {
        Line = 1,
        Rule = "StartPoint_To_InitialNode",
        SourceType = "UCM!\"ucm::map::StartPoint",
        MuSrcTypes = new ObservableCollection<string>(),
        SrcTypes = new ObservableCollection<string>()
      };
      mViewModel.CstCandidates.Add(mCstCandidate);

      mCttCandidate = new CttCandidate
      {
        Line = 1,
        Rule = "StartPoint_To_InitialNode",
        TargetType = "UML!InitialNode",
        TargetTypes = new ObservableCollection<string>(),
        MuTargetTypes = new ObservableCollection<string>()
      };
      mViewModel.CttCandidates.Add(mCttCandidate);

      mAamCandidate = new AamCandidate
      {
        Line = 1,
        MuMappings = new ObservableCollection<string>(),
        OutPattern = "to i: UML!InitialNode",
        Rule = "StartPoint_To_InitialNode"
      };
      mViewModel.AamCandidates.Add(mAamCandidate);

      mAfeCandidate = new AfeCandidate
      {
        Line = 1,
        MuFilteringExpressions = new ObservableCollection<string>(),
        Rule = "StartPoint_To_InitialNode"
      };
      mViewModel.AfeCandidates.Add(mAfeCandidate);
    }

    #region Mutant

    #region Add Mutant

    [Test]
    public void TestAddMutant_MutantAdded_FormCleared()
    {
      mViewModel.MutantName = mMutant.Name;
      mViewModel.MutantPath = mMutant.Path;
      mViewModel.MutantType = mMutant.Type;

      mViewModel.AddMutantCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsTrue(mViewModel.Project.Mutants.Contains(mMutant));
      Assert.AreEqual(string.Empty, mViewModel.MutantName);
      Assert.AreEqual(string.Empty, mViewModel.MutantPath);
      Assert.IsNull(mViewModel.MutantType);
    }

    [Test]
    public void TestAddMutant_FormInvalid_MutantNotAdded_FormNotCleared()
    {
      mViewModel.MutantName = string.Empty;
      mViewModel.MutantPath = mMutant.Path;
      mViewModel.MutantType = mMutant.Type;

      mViewModel.AddMutantCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsFalse(mViewModel.Project.Mutants.Contains(mMutant));
      Assert.AreEqual(string.Empty, mViewModel.MutantName);
      Assert.AreEqual(mMutant.Path, mViewModel.MutantPath);
      Assert.AreEqual(mMutant.Type, mViewModel.MutantType);
    }

    [Test]
    public void TestAddMutant_TypeNotSelected_MutantNotAdded_FormNotCleared()
    {
      mViewModel.MutantName = mMutant.Name;
      mViewModel.MutantPath = mMutant.Path;

      mViewModel.AddMutantCmd.Execute(null);
      mDlgSrvc.Received().Error(MutantsViewModel.AddMutantErrorMsg, MutantsViewModel.AddMutantCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Cannot add mutant {0} because it already exists", mMutant.Name), MutantsViewModel.AddMutantCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Mutant {0} does not exist", mMutant.Path), MutantsViewModel.AddMutantCaption);
      Assert.IsFalse(mViewModel.Project.Mutants.Contains(mMutant));
      Assert.AreEqual(mMutant.Name, mViewModel.MutantName);
      Assert.AreEqual(mMutant.Path, mViewModel.MutantPath);
      Assert.IsNull(mViewModel.MutantType);
    }

    [Test]
    public void TestAddMutant_MutantFileDoesNotExist_MutantNotAdded_FormNotCleared()
    {
      mViewModel.MutantName = mMutant.Name;
      mViewModel.MutantPath = InvalidPath;
      mViewModel.MutantType = mMutant.Type;

      mViewModel.AddMutantCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(MutantsViewModel.AddMutantErrorMsg, MutantsViewModel.AddMutantCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Cannot add mutant {0} because it already exists", mMutant.Name), MutantsViewModel.AddMutantCaption);
      mDlgSrvc.Received().Error(string.Format("Mutant {0} does not exist", InvalidPath), MutantsViewModel.AddMutantCaption);
      Assert.IsFalse(mViewModel.Project.Mutants.Contains(mMutant));
      Assert.AreEqual(mMutant.Name, mViewModel.MutantName);
      Assert.AreEqual(InvalidPath, mViewModel.MutantPath);
      Assert.AreEqual(mMutant.Type, mViewModel.MutantType);
    }

    [Test]
    public void TestAddMutant_MutantExists_MutantNotAdded_FormNotCleared()
    {
      mViewModel.Project.Mutants.Add(mMutant);

      var updatedMutant = new MuAtlMutant
      {
        Name = MutantName,
        Path = MutantPath,
        Type = MutantType.AFE
      };

      mViewModel.MutantName = updatedMutant.Name;
      mViewModel.MutantPath = updatedMutant.Path;
      mViewModel.MutantType = updatedMutant.Type;

      mViewModel.AddMutantCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(MutantsViewModel.AddMutantErrorMsg, MutantsViewModel.AddMutantCaption);
      mDlgSrvc.Received().Error(string.Format("Cannot add mutant {0} because it already exists", mMutant.Name), MutantsViewModel.AddMutantCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Mutant {0} does not exist", mMutant.Path), MutantsViewModel.AddMutantCaption);
      Assert.AreEqual(1, mViewModel.Project.Mutants.Count());

      Assert.AreEqual(mMutant.Name, mViewModel.Project.Mutants[0].Name);
      Assert.AreEqual(mMutant.Path, mViewModel.Project.Mutants[0].Path);
      Assert.AreEqual(mMutant.Type, mViewModel.Project.Mutants[0].Type);

      Assert.AreEqual(updatedMutant.Name, mViewModel.MutantName);
      Assert.AreEqual(updatedMutant.Path, mViewModel.MutantPath);
      Assert.AreEqual(updatedMutant.Type, mViewModel.MutantType);
    }

    #endregion

    #region Delete Mutant

    [Test]
    public void TestDeleteMutant_MutantSelected_MutantDeleted()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.SelectedMutant = mMutant;

      mViewModel.DelMutantCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsNull(mViewModel.Project.Mutants.SingleOrDefault(m => m.Name == mMutant.Name));
    }

    [Test]
    public void TestDeleteMutant_MutantNotSeleted_MutantNotDeleted()
    {
      mViewModel.Project.Mutants.Add(mMutant);

      mViewModel.DelMutantCmd.Execute(null);
      mDlgSrvc.Received().Error(MutantsViewModel.SelectMutantMsg, MutantsViewModel.DelMutantCaption);
      Assert.AreEqual(mMutant, mViewModel.Project.Mutants.SingleOrDefault(m => m.Name == mMutant.Name));
    }

    #endregion

    #region Update Mutant

    [Test]
    public void TestUpdateMutant_MutantSelected_FormValid_MutantUpdated()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.SelectedMutant = mMutant;

      mViewModel.MutantName = UpdatedMutantName;
      mViewModel.MutantPath = UpdatedMutantPath;
      mViewModel.MutantType = MutantType.AFE;

      mViewModel.UpdateMutantCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.AreEqual(UpdatedMutantName, mViewModel.Project.Mutants[0].Name);
      Assert.AreEqual(UpdatedMutantPath, mViewModel.Project.Mutants[0].Path);
      Assert.AreEqual(MutantType.AFE, mViewModel.Project.Mutants[0].Type);
    }

    [Test]
    public void TestUpdateMutant_MutantNotSelected_FormValid_MutantNotUpdated()
    {
      mViewModel.Project.Mutants.Add(mMutant);

      mViewModel.MutantName = UpdatedMutantName;
      mViewModel.MutantPath = UpdatedMutantPath;
      mViewModel.MutantType = MutantType.AFE;

      mViewModel.UpdateMutantCmd.Execute(null);
      mDlgSrvc.Received().Error(MutantsViewModel.SelectMutantMsg, MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.DidNotReceive().Error(MutantsViewModel.UpdateMutantErrorMsg, MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Mutant {0} already exists", UpdatedMutantName), MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Mutant {0} does not exist", UpdatedMutantPath), MutantsViewModel.UpdateMutantCaption);
      Assert.AreEqual(1, mViewModel.Project.Mutants.Count());
      Assert.AreEqual(mMutant.Name, mViewModel.Project.Mutants[0].Name);
      Assert.AreEqual(mMutant.Path, mViewModel.Project.Mutants[0].Path);
      Assert.AreEqual(mMutant.Type, mViewModel.Project.Mutants[0].Type);
    }

    [Test]
    public void TestUpdateMutant_FormInvalid_MutantNotUpdated()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.SelectedMutant = mMutant;

      mViewModel.MutantName = string.Empty;
      mViewModel.MutantPath = UpdatedMutantPath;
      mViewModel.MutantType = MutantType.AFE;

      mViewModel.UpdateMutantCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.AreEqual(1, mViewModel.Project.Mutants.Count());
      Assert.AreEqual(mMutant.Name, mViewModel.Project.Mutants[0].Name);
      Assert.AreEqual(mMutant.Path, mViewModel.Project.Mutants[0].Path);
      Assert.AreEqual(mMutant.Type, mViewModel.Project.Mutants[0].Type);
    }

    [Test]
    public void TestUpdateMutant_MutantSelected_TypeNotSelected_FormValid_MutantNotUpdated()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.SelectedMutant = mMutant;

      mViewModel.MutantName = UpdatedMutantName;
      mViewModel.MutantPath = UpdatedMutantPath;

      mViewModel.UpdateMutantCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(MutantsViewModel.SelectMutantMsg, MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.Received().Error(MutantsViewModel.UpdateMutantErrorMsg, MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Mutant {0} already exists", UpdatedMutantName), MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Mutant {0} does not exist", UpdatedMutantPath), MutantsViewModel.UpdateMutantCaption);
      Assert.AreEqual(1, mViewModel.Project.Mutants.Count());
      Assert.AreEqual(mMutant.Name, mViewModel.Project.Mutants[0].Name);
      Assert.AreEqual(mMutant.Path, mViewModel.Project.Mutants[0].Path);
      Assert.AreEqual(mMutant.Type, mViewModel.Project.Mutants[0].Type);
    }

    [Test]
    public void TestUpdate_MutantSelected_FormValid_MutantExists_MutantNotUpdated()
    {
      var mutant2 = new MuAtlMutant
      {
        Name = UpdatedMutantName,
        Path = UpdatedMutantPath,
        Type = MutantType.AFE
      };

      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.Project.Mutants.Add(mutant2);
      mViewModel.SelectedMutant = mutant2;

      mViewModel.MutantName = mMutant.Name;
      mViewModel.MutantPath = mutant2.Path;
      mViewModel.MutantType = mutant2.Type;

      mViewModel.UpdateMutantCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(MutantsViewModel.SelectMutantMsg, MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.DidNotReceive().Error(MutantsViewModel.UpdateMutantErrorMsg, MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.Received().Error(string.Format("Mutant {0} already exists", mMutant.Name), MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Mutant {0} does not exist", UpdatedMutantPath), MutantsViewModel.UpdateMutantCaption);
      Assert.AreEqual(2, mViewModel.Project.Mutants.Count());
      Assert.AreEqual(mutant2.Name, mViewModel.Project.Mutants[1].Name);
      Assert.AreEqual(mutant2.Path, mViewModel.Project.Mutants[1].Path);
      Assert.AreEqual(mutant2.Type, mViewModel.Project.Mutants[1].Type);
    }

    [Test]
    public void TestUpdateMutant_MutantSelected_MutantFileDoesNotExist_FormValid_MutantNotUpdated()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mViewModel.SelectedMutant = mMutant;

      mViewModel.MutantName = UpdatedMutantName;
      mViewModel.MutantPath = InvalidPath;
      mViewModel.MutantType = MutantType.AFE;

      mViewModel.UpdateMutantCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(MutantsViewModel.SelectMutantMsg, MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.DidNotReceive().Error(MutantsViewModel.UpdateMutantErrorMsg, MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Mutant {0} already exists", UpdatedMutantName), MutantsViewModel.UpdateMutantCaption);
      mDlgSrvc.Received().Error(string.Format("Mutant {0} does not exist", InvalidPath), MutantsViewModel.UpdateMutantCaption);
      Assert.AreEqual(1, mViewModel.Project.Mutants.Count());
      Assert.AreEqual(mMutant.Name, mViewModel.Project.Mutants[0].Name);
      Assert.AreEqual(mMutant.Path, mViewModel.Project.Mutants[0].Path);
      Assert.AreEqual(mMutant.Type, mViewModel.Project.Mutants[0].Type);
    }

    #endregion

    #region Find Mutant

    [Test]
    public void TestFindMutant_MutantExists_MutantDetailsShownOnForm()
    {
      mProject.AddMutant(mMutant);

      mViewModel.FindCmd.Execute(mMutant.Name);
      Assert.AreEqual(mMutant.Name, mViewModel.MutantName);
      Assert.AreEqual(mMutant.Path, mViewModel.MutantPath);
      Assert.AreEqual(mMutant.Type, mViewModel.MutantType);
    }

    [Test]
    public void TestFindMutant_MutantDoesNotExist_MutantDetailsNotShownOnForm()
    {
      mViewModel.FindCmd.Execute(mMutant.Name);
      Assert.IsNull(mViewModel.MutantName);
      Assert.IsNull(mViewModel.MutantPath);
      Assert.IsNull(mViewModel.MutantType);
    }

    public void TestFindMutant_MutantNameNotGiven_MutantDetailsNotShownOnForm()
    {
      mProject.AddMutant(mMutant);

      mViewModel.FindCmd.Execute(null);
      Assert.IsNull(mViewModel.MutantName);
      Assert.IsNull(mViewModel.MutantPath);
      Assert.IsNull(mViewModel.MutantType);
    }

    #endregion

    #region Diff Mutant

    [Test]
    public void TestFindMutant_MutantSelected_MutantExists_DiffShown()
    {
      mProject.AddMutant(mMutant);
      mProject.Module = Path.GetFullPath(@"TestData\ucm2ad.atl");
      mViewModel.SelectedMutant = mMutant;

      mViewModel.DiffCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      mDiffSrvc.Received().Diff(mProject.Module, mMutant.Path);
    }

    [Test]
    public void TestFindMutant_MutantNotSelected_DiffNotShown()
    {
      mProject.AddMutant(mMutant);

      mViewModel.DiffCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      mDiffSrvc.DidNotReceive().Diff(mProject.Module, mMutant.Path);
    }

    [Test]
    public void TestFindMutant_MutantSelected_MutantDoesNotExist_DiffNotShown()
    {
      mProject.AddMutant(mMutant);
      mMutant.Path = @"X:/../../../invalid.atl";
      mViewModel.SelectedMutant = mMutant;

      mViewModel.DiffCmd.Execute(null);
      mDlgSrvc.Received().Error(MutantsViewModel.MutantNotFoundMsg, MutantsViewModel.MutantNotFoundCaption);
      mDiffSrvc.DidNotReceive().Diff(mProject.Module, mMutant.Path);
    }

    #endregion

    #region Select Mutant

    [Test]
    public void TestSelectMutant_MutantSelected_MutantDetailsShownOnForm()
    {
      mViewModel.Project.AddMutant(mMutant);
      mViewModel.SelectedMutant = mMutant;

      mViewModel.MutantSlctCmd.Execute(mMutant);
      Assert.AreEqual(mMutant.Name, mViewModel.MutantName);
      Assert.AreEqual(mMutant.Path, mViewModel.MutantPath);
      Assert.AreEqual(mMutant.Type, mViewModel.MutantType);
    }

    [Test]
    public void TestSelectMutant_MutantNotSelected_FormCleared()
    {
      mViewModel.MutantName = mMutant.Name;
      mViewModel.MutantPath = mMutant.Path;
      mViewModel.MutantType = mMutant.Type;

      mViewModel.MutantSlctCmd.Execute(null);
      Assert.AreEqual(string.Empty, mViewModel.MutantName);
      Assert.AreEqual(string.Empty, mViewModel.MutantPath);
      Assert.IsNull(mViewModel.MutantType);
    }

    #endregion

    #region Select Mutant File

    [Test]
    public void TestSelectMutantFile_FileSelected_MutantPathSet()
    {
      mDlgSrvc.BrowseAtl(MutantsViewModel.MutantMsg).Returns(MutantPath);
      mViewModel.SelectMutantFileCmd.Execute(null);
      mDlgSrvc.Received().BrowseAtl(MutantsViewModel.MutantMsg);
      Assert.AreEqual(MutantPath, mViewModel.MutantPath);
    }

    [Test]
    public void TestSelectMutantFile_FileSelectedCancelled_MutantPathNotSet()
    {
      mDlgSrvc.BrowseAtl(MutantsViewModel.MutantMsg).Returns(string.Empty);
      mViewModel.SelectMutantFileCmd.Execute(null);
      mDlgSrvc.Received().BrowseAtl(MutantsViewModel.MutantMsg);
      Assert.IsNull(mViewModel.MutantPath);
    }

    #endregion

    #region Select Mutant Directory

    [Test]
    public void TestSelectMutantDirectory_DirectorySelected_MutantDirectoryPathSet()
    {
      const string muDir = "X:/../../mutants";
      mDlgSrvc.BrowseFolder(MutantsViewModel.SelectMuDirMsg).Returns(muDir);
      mViewModel.SelectMuDirCmd.Execute(null);
      mDlgSrvc.Received().BrowseFolder(MutantsViewModel.SelectMuDirMsg);
      Assert.AreEqual(muDir, mViewModel.MuDir);
    }

    [Test]
    public void TestSelectMutantDirectory_DirectorySelectCancelled_MutantDirectoryPathNotSet()
    {
      mDlgSrvc.BrowseFolder(MutantsViewModel.SelectMuDirMsg).Returns(string.Empty);
      mViewModel.SelectMuDirCmd.Execute(null);
      mDlgSrvc.Received().BrowseFolder(MutantsViewModel.SelectMuDirMsg);
      Assert.IsNull(mViewModel.MuDir);
    }

    #endregion

    #region Check Status

    [Test]
    public void TestCheckStatus_MutantHasAtleastOnePassResult_MutantStatusChangedToLive()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mProject.Results.Add(new MuAtlResult
      {
        Mutant = mMutant,
        Verdict = MuAtlVerdict.Pass
      });
      mProject.Results.Add(new MuAtlResult
      {
        Mutant = mMutant,
        Verdict = MuAtlVerdict.Fail
      });

      mViewModel.ChkStatusCmd.Execute(null);
      Assert.AreEqual(MutantStatus.Live, mMutant.Status);
    }

    [Test]
    public void TestCheckStatus_MutantHasAllResultsFail_MutantStatusChangedToDead()
    {
      mViewModel.Project.Mutants.Add(mMutant);
      mProject.Results.Add(new MuAtlResult
      {
        Mutant = mMutant,
        Verdict = MuAtlVerdict.Fail
      });
      mProject.Results.Add(new MuAtlResult
      {
        Mutant = mMutant,
        Verdict = MuAtlVerdict.Fail
      });

      mViewModel.ChkStatusCmd.Execute(null);
      Assert.AreEqual(MutantStatus.Dead, mMutant.Status);
    }

    [Test]
    public void TestCheckStatus_MutantHasNoResults_MutantStatusChangedToUndetermined()
    {
      mMutant.Status = MutantStatus.Live;
      mViewModel.Project.Mutants.Add(mMutant);

      mViewModel.ChkStatusCmd.Execute(null);
      Assert.AreEqual(MutantStatus.Undetermined, mMutant.Status);
    }

    #endregion

    #region Generate

    [Test]
    public void TestGenerate_MuDirSelected_CstCandidateSelected_MuSrcTypesCleared()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mViewModel.MuDir = "X:/../../testmudir";
      SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

      mViewModel.GenerateCmd.Execute(null);
      Thread.Sleep(TimeSpan.FromSeconds(1));
      Assert.IsNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == MutantSrcType));
      Assert.IsNotNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == MutantSrcType));
      Assert.IsFalse(mViewModel.MuGenerating);
    }

    [Test]
    public void TestGenerate_MuDirNotSelected_CstCandidateSelected_MuSrcTypesNotCleared()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);

      mViewModel.GenerateCmd.Execute(null);
      mDlgSrvc.Received().Error(MutantsViewModel.MutantDirNotSelectedErrorMsg, MutantsViewModel.MutantDirNotSelectedCaption);
      Assert.IsNotNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == MutantSrcType));
      Assert.IsNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == MutantSrcType));
      Assert.IsFalse(mViewModel.MuGenerating);
    }

    [Test]
    public void TestGenerate_MuDirSelected_CttCandidateSelected_MuTargetTypesCleared()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mViewModel.MuDir = "X:/../../testmudir";
      SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

      mViewModel.GenerateCmd.Execute(null);
      Thread.Sleep(TimeSpan.FromSeconds(1));
      Assert.IsNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == MutantTargetType));
      Assert.IsNotNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == MutantTargetType));
      Assert.IsFalse(mViewModel.MuGenerating);
    }

    [Test]
    public void TestGenerate_MuDirNotSelected_CttCandidateSelected_MuTargetTypesNotCleared()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);

      mViewModel.GenerateCmd.Execute(null);
      mDlgSrvc.Received().Error(MutantsViewModel.MutantDirNotSelectedErrorMsg, MutantsViewModel.MutantDirNotSelectedCaption);
      Assert.IsNotNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == MutantTargetType));
      Assert.IsNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == MutantTargetType));
      Assert.IsFalse(mViewModel.MuGenerating);
    }

    [Test]
    public void TestGenerate_MuDirSelected_AamCandidateSelected_MuAttrMappingsCleared()
    {
      mAamCandidate.MuMappings.Add(MuAttrMapping);
      mViewModel.MuDir = "X:/../../testmudir";
      SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

      mViewModel.GenerateCmd.Execute(null);
      mDlgSrvc.Received().Error(MutantsViewModel.MutantDirNotSelectedErrorMsg, MutantsViewModel.MutantDirNotSelectedCaption);
      Thread.Sleep(TimeSpan.FromSeconds(1));
      Assert.IsNull(mAamCandidate.MuMappings.SingleOrDefault(t => t == MuAttrMapping));
      Assert.IsFalse(mViewModel.MuGenerating);
    }

    [Test]
    public void TestGenerate_MuDirNotSelected_AamCandidateSelected_MuAttrMappingsNotCleared()
    {
      mAamCandidate.MuMappings.Add(MuAttrMapping);

      mViewModel.GenerateCmd.Execute(null);
      mDlgSrvc.Received().Error(MutantsViewModel.MutantDirNotSelectedErrorMsg, MutantsViewModel.MutantDirNotSelectedCaption);
      Assert.IsNotNull(mAamCandidate.MuMappings.SingleOrDefault(t => t == MuAttrMapping));
      Assert.IsFalse(mViewModel.MuGenerating);
    }

    [Test]
    public void TestGenerate_MuDirSelected_AfeCandidateSelected_MuFltrExprnsCleared()
    {
      mAfeCandidate.MuFilteringExpressions.Add(MuFltrExpr);
      mViewModel.MuDir = "X:/../../testmudir";
      SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

      mViewModel.GenerateCmd.Execute(null);
      Thread.Sleep(TimeSpan.FromSeconds(1));
      Assert.IsNull(mAfeCandidate.MuFilteringExpressions.SingleOrDefault(t => t == MuFltrExpr));
      Assert.IsFalse(mViewModel.MuGenerating);
    }

    [Test]
    public void TestGenerate_MuDirNotSelected_AfeCandidateSelected_MuFltrExprnsNotCleared()
    {
      mAfeCandidate.MuFilteringExpressions.Add(MuFltrExpr);

      mViewModel.GenerateCmd.Execute(null);
      mDlgSrvc.Received().Error(MutantsViewModel.MutantDirNotSelectedErrorMsg, MutantsViewModel.MutantDirNotSelectedCaption);
      Assert.IsNotNull(mAfeCandidate.MuFilteringExpressions.SingleOrDefault(t => t == MuFltrExpr));
      Assert.IsFalse(mViewModel.MuGenerating);
    }

    [Test]
    public void TestGenerate_MuDirSelected_CemSelected_CemCleared()
    {
      mViewModel.CemChecked = true;
      mViewModel.MuDir = "X:/../../testmudir";
      SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

      mViewModel.GenerateCmd.Execute(null);
      Thread.Sleep(TimeSpan.FromSeconds(1));
      Assert.IsFalse(mViewModel.CemChecked);
      Assert.IsFalse(mViewModel.MuGenerating);
    }

    [Test]
    public void TestGenerate_MuDirNotSelected_CemSelected_CemNotCleared()
    {
      mViewModel.CemChecked = true;

      mViewModel.GenerateCmd.Execute(null);
      mDlgSrvc.Received().Error(MutantsViewModel.MutantDirNotSelectedErrorMsg, MutantsViewModel.MutantDirNotSelectedCaption);
      Assert.IsTrue(mViewModel.CemChecked);
      Assert.IsFalse(mViewModel.MuGenerating);
    } 

    #endregion

    #endregion

    #region Mutant Candidates

    #region CST Candidate

    #region Add Source Type

    [Test]
    public void TestAddSourceType_CstCandidateSelected_SourceTypeDoesNotExist_SourceTypeAddedForAllCandidates_FormCleared()
    {
      mViewModel.CstCandidates.Add(new CstCandidate
      {
        Line = 1,
        SrcTypes = new ObservableCollection<string>(),
        Rule = "rule"
      });
      mViewModel.SelectedCstCandidate = mCstCandidate;
      mViewModel.NewSrcType = SrcType;
      mViewModel.AddSrcTypeCmd.Execute(SrcType);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      foreach (var candidate in mViewModel.CstCandidates)
      {
        Assert.IsNotNull(candidate.SrcTypes.SingleOrDefault(t => t == SrcType));
      }
      Assert.AreEqual(string.Empty, mViewModel.NewSrcType);
    }

    [Test]
    public void TestAddSourceType_CstCandidateNotSelected_SourceTypeDoesNotExist_SourceTypeNotAddedForAllCandidates_FormNotCleared()
    {
      mViewModel.CstCandidates.Add(new CstCandidate
      {
        Line = 1,
        SrcTypes = new ObservableCollection<string>(),
        Rule = "rule"
      });
      mViewModel.NewSrcType = SrcType;
      mViewModel.AddSrcTypeCmd.Execute(SrcType);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      foreach (var candidate in mViewModel.CstCandidates)
      {
        Assert.IsNull(candidate.SrcTypes.SingleOrDefault(t => t == SrcType));
      }
      Assert.AreEqual(SrcType, mViewModel.NewSrcType);
    }

    [Test]
    public void TestAddSourceType_CstCandidateSelected_SourceTypeExists_SourceTypeNotAddedForAllCandidates_FormNotCleared()
    {
      var cstCandidate2 = new CstCandidate
      {
        Line = 1,
        MuSrcTypes = new ObservableCollection<string>(),
        Rule = "EndPoint_To_FinalNode",
        SrcTypes = new ObservableCollection<string>(),
        SourceType = "UCM!\"ucm::map::EndPoint"
      };
      cstCandidate2.SrcTypes.Add(SrcType);
      mViewModel.CstCandidates.Add(cstCandidate2);

      mCstCandidate.SrcTypes.Add(SrcType);
      mViewModel.SelectedCstCandidate = mCstCandidate;
      mViewModel.NewSrcType = SrcType;

      mViewModel.AddSrcTypeCmd.Execute(SrcType);
      mDlgSrvc.Received().Error(string.Format(
          "Source type {0} already exists for CST mutant generation", SrcType), MutantsViewModel.AddSrcTypeCaption);
      foreach (var candidate in mViewModel.CstCandidates)
      {
        Assert.AreEqual(1, candidate.SrcTypes.Count());
      }
      Assert.AreEqual(SrcType, mViewModel.NewSrcType);
    }

    #endregion

    #region Delete Source Type

    [Test]
    public void TestDeleteSourceType_CstCandidateSelected_SrcTypeExists_SrcTypeDeleted()
    {
      mCstCandidate.SrcTypes.Add(SrcType);
      mViewModel.SelectedCstCandidate = mCstCandidate;
      mViewModel.SelectedSrcType = SrcType;

      mViewModel.DelSrcTypeCmd.Execute(null);
      Assert.IsNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == SrcType));
    }

    [Test]
    public void TestDeleteSourceType_CstCandidateNotSelected_SrcTypeExists_SrcTypeNotDeleted()
    {
      mCstCandidate.SrcTypes.Add(SrcType);
      mViewModel.SelectedSrcType = SrcType;

      mViewModel.DelSrcTypeCmd.Execute(null);
      Assert.IsNotNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == SrcType));
    }

    [Test]
    public void TestDeleteSourceType_CstCandidateSelected_SrcTypeNotSelected_SrcTypeExists_SrcTypeNotDeleted()
    {
      mCstCandidate.SrcTypes.Add(SrcType);
      mViewModel.SelectedCstCandidate = mCstCandidate;

      mViewModel.DelSrcTypeCmd.Execute(null);
      Assert.IsNotNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == SrcType));
    }

    #endregion

    #region Add Mutant Source Type

    [Test]
    public void TestAddMutantSourceType_CstCandidateSelected_SourceTypeDoesNotExist_SourceTypeAdded_FormCleared()
    {
      mViewModel.SelectedCstCandidate = mCstCandidate;
      mViewModel.NewMuSrcType = MutantSrcType;
      mViewModel.AddMuSrcCmd.Execute(MutantSrcType);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsTrue(mCstCandidate.MuSrcTypes.Contains(MutantSrcType));
      Assert.AreEqual(string.Empty, mViewModel.NewMuSrcType);
    }

    [Test]
    public void TestAddMutantSourceType_CstCandidateNotSelected_SourceTypeDoesNotExist_SourceTypeNotAdded_FormNotCleared()
    {
      mViewModel.NewMuSrcType = MutantSrcType;
      mViewModel.AddMuSrcCmd.Execute(MutantSrcType);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsFalse(mCstCandidate.MuSrcTypes.Contains(MutantSrcType));
      Assert.AreEqual(MutantSrcType, mViewModel.NewMuSrcType);
    }

    [Test]
    public void TestAddMutantSourceType_CstCandidateSelected_SourceTypeExists_SourceTypeNotAdded_FormNotCleared()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mViewModel.SelectedCstCandidate = mCstCandidate;
      mViewModel.NewMuSrcType = MutantSrcType;

      mViewModel.AddMuSrcCmd.Execute(MutantSrcType);
      mDlgSrvc.Received().Error(string.Format("Mutant source type {0} already added for rule {1}", MutantSrcType, mCstCandidate.Rule), MutantsViewModel.AddMuSrcTypeCaption);

      Assert.AreEqual(1, mCstCandidate.MuSrcTypes.Count());
      Assert.AreEqual(MutantSrcType, mViewModel.NewMuSrcType);
    }

    #endregion

    #region Delete Mutant Source Type

    [Test]
    public void TestDeleteMutantSourceType_CstCandidateSelected_SourceTypeExists_SourceTypeDeleted()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mViewModel.SelectedCstCandidate = mCstCandidate;
      mViewModel.SelectedMuSrcType = MutantSrcType;

      mViewModel.DelMuSrcTypeCmd.Execute(null);
      Assert.IsNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == MutantSrcType));
    }

    [Test]
    public void TestDeleteMutantSourceType_CstCandidateNotSelected_SourceTypeExists_SourceTypeNotDeleted()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mViewModel.SelectedMuSrcType = MutantSrcType;

      mViewModel.DelMuSrcTypeCmd.Execute(null);
      Assert.IsNotNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == MutantSrcType));
    }

    [Test]
    public void TestDeleteMutantSourceType_CstCandidateSelected_SourceTypeNotSelected_SourceTypeExists_SourceTypeNotDeleted()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mViewModel.SelectedCstCandidate = mCstCandidate;

      mViewModel.DelMuSrcTypeCmd.Execute(null);
      Assert.IsNotNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == MutantSrcType));
    }

    #endregion

    #region Move Source Type to Mutant Source Types

    [Test]
    public void TestMoveSourceTypeToMutantSourceTypes_CstCandidateSelected_SrcTypeSelected_SrcTypeDeletedFromSrcTypesAndAddedToMutantSrcTypes()
    {
      mCstCandidate.SrcTypes.Add(SrcType);
      mViewModel.SelectedCstCandidate = mCstCandidate;
      mViewModel.SelectedSrcType = SrcType;

      mViewModel.MoveSrcToMuSrcsCmd.Execute(null);
      Assert.IsNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == SrcType));
      Assert.IsNotNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == SrcType));
    }

    [Test]
    public void TestMoveSourceTypeToMutantSourceTypes_CstCandidateNotSelected_SrcTypeNotMoved()
    {
      mCstCandidate.SrcTypes.Add(SrcType);
      mViewModel.SelectedSrcType = SrcType;

      mViewModel.MoveSrcToMuSrcsCmd.Execute(null);
      Assert.IsNotNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == SrcType));
      Assert.IsNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == SrcType));
    }

    [Test]
    public void TestMoveSourceTypeToMutantSourceTypes_CstCandidateSelected_SrcTypeNotSelected_SrcTypeNotMoved()
    {
      mCstCandidate.SrcTypes.Add(SrcType);
      mViewModel.SelectedCstCandidate = mCstCandidate;

      mViewModel.MoveSrcToMuSrcsCmd.Execute(null);
      Assert.IsNotNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == SrcType));
      Assert.IsNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == SrcType));
    }

    [Test]
    public void TestMoveSourceTypeToMutantSourceTypes_CstCandidateSelected_SrcTypeSelected_SrcTypeExistsInMutantSrcTypes_SrcTypeNotMoved()
    {
      mCstCandidate.SrcTypes.Add(SrcType);
      mCstCandidate.MuSrcTypes.Add(SrcType);
      mViewModel.SelectedCstCandidate = mCstCandidate;
      mViewModel.SelectedSrcType = SrcType;

      mViewModel.MoveSrcToMuSrcsCmd.Execute(null);
      Assert.IsNotNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == SrcType));
      Assert.IsNotNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == SrcType));
      Assert.AreEqual(1, mCstCandidate.SrcTypes.Count());
      Assert.AreEqual(1, mCstCandidate.MuSrcTypes.Count());
    }

    #endregion

    #region Move Mutant Source Type to Source Types

    [Test]
    public void TestMoveMutantSrcTypeToSourceTypes_CstCandidateSelected_MuSrcTypeSelected_MuSrcTypeRemovedFromMuSrcTypeAndAddedToSrcTypes()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mViewModel.SelectedCstCandidate = mCstCandidate;
      mViewModel.SelectedMuSrcType = MutantSrcType;

      mViewModel.MoveMuSrcToSrcsCmd.Execute(null);
      Assert.IsNotNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == MutantSrcType));
      Assert.IsNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == MutantSrcType));
    }

    [Test]
    public void TestMoveMutantSrcTypeToSourceTypes_CstCandidateNotSelected_MuSrcTypeSelected_MuSrcTypeNotMoved()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mViewModel.SelectedMuSrcType = MutantSrcType;

      mViewModel.MoveMuSrcToSrcsCmd.Execute(null);
      Assert.IsNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == MutantSrcType));
      Assert.IsNotNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == MutantSrcType));
    }

    [Test]
    public void TestMoveMutantSrcTypeToSourceTypes_CstCandidateSelected_MuSrcTypeNotSelected_MuSrcTypeNotMoved()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mViewModel.SelectedMuSrcType = MutantSrcType;

      mViewModel.MoveMuSrcToSrcsCmd.Execute(null);
      Assert.IsNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == MutantSrcType));
      Assert.IsNotNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == MutantSrcType));
    }

    [Test]
    public void TestMoveMutantSrcTypeToSourceTypes_CstCandidateSelected_MuSrcTypeSelected_MuSrcTypeExistsInSrcTypes_MuSrcTypeNotMoved()
    {
      mCstCandidate.SrcTypes.Add(MutantSrcType);
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mViewModel.SelectedCstCandidate = mCstCandidate;
      mViewModel.SelectedMuSrcType = MutantSrcType;

      mViewModel.MoveMuSrcToSrcsCmd.Execute(null);
      Assert.IsNotNull(mCstCandidate.SrcTypes.SingleOrDefault(t => t == MutantSrcType));
      Assert.AreEqual(1, mCstCandidate.SrcTypes.Count());
      Assert.IsNotNull(mCstCandidate.MuSrcTypes.SingleOrDefault(t => t == MutantSrcType));
      Assert.AreEqual(1, mCstCandidate.MuSrcTypes.Count());
    }

    #endregion

    #region Move All Source Types to Mutant Source Types

    [Test]
    public void TestMoveAllSrcTypesToMuSrcTypes_CstCandidateSelected_SrcTypesDontExistInMuSrcTypes_AllSrcTypesRemovedFromSrcTypesAndAddedToMuSrcTypes()
    {
      mCstCandidate.SrcTypes.Add(SrcType);
      mCstCandidate.SrcTypes.Add(SrcType2);
      mViewModel.SelectedCstCandidate = mCstCandidate;

      mViewModel.MoveAllSrcsToMuSrcsCmd.Execute(null);
      Assert.IsFalse(mCstCandidate.SrcTypes.Any());
      Assert.AreEqual(2, mCstCandidate.MuSrcTypes.Count());
      Assert.AreEqual(SrcType, mCstCandidate.MuSrcTypes[0]);
      Assert.AreEqual(SrcType2, mCstCandidate.MuSrcTypes[1]);
    }

    [Test]
    public void TestMoveAllSrcTypesToMuSrcTypes_CstCandidateNotSelected_AllSrcTypesNotMoved()
    {
      mCstCandidate.SrcTypes.Add(SrcType);
      mCstCandidate.SrcTypes.Add(SrcType2);

      mViewModel.MoveAllSrcsToMuSrcsCmd.Execute(null);
      Assert.AreEqual(2, mCstCandidate.SrcTypes.Count());
      Assert.AreEqual(SrcType, mCstCandidate.SrcTypes[0]);
      Assert.AreEqual(SrcType2, mCstCandidate.SrcTypes[1]);
      Assert.IsFalse(mCstCandidate.MuSrcTypes.Any());
    }

    [Test]
    public void TestMoveAllSrcTypesToMuSrcTypes_CstCandidateSelected_SrcTypesExistInMuSrcTypes_AllSrcTypesNotMoved()
    {
      mCstCandidate.SrcTypes.Add(SrcType);
      mCstCandidate.SrcTypes.Add(SrcType2);
      mCstCandidate.MuSrcTypes.Add(SrcType);
      mCstCandidate.MuSrcTypes.Add(SrcType2);
      mViewModel.SelectedCstCandidate = mCstCandidate;

      mViewModel.MoveAllSrcsToMuSrcsCmd.Execute(null);
      Assert.AreEqual(2, mCstCandidate.SrcTypes.Count());
      Assert.AreEqual(SrcType, mCstCandidate.SrcTypes[0]);
      Assert.AreEqual(SrcType2, mCstCandidate.SrcTypes[1]);
      Assert.AreEqual(2, mCstCandidate.MuSrcTypes.Count());
      Assert.AreEqual(SrcType, mCstCandidate.MuSrcTypes[0]);
      Assert.AreEqual(SrcType2, mCstCandidate.MuSrcTypes[1]);
    }

    #endregion

    #region Move All Mutant Source Types to Source Types

    [Test]
    public void TestMoveAllMuSrscToSrcTypes_CstCandidateSelected_CandidateHasMuSrcTypes_AllMuSrcTypesMovedToSrcTypes()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mCstCandidate.MuSrcTypes.Add(MutantSrcType2);
      mViewModel.SelectedCstCandidate = mCstCandidate;

      mViewModel.MoveAllMuSrcsToSrcsCmd.Execute(null);
      Assert.IsFalse(mCstCandidate.MuSrcTypes.Any());
      Assert.AreEqual(2, mCstCandidate.SrcTypes.Count());
      Assert.AreEqual(MutantSrcType, mCstCandidate.SrcTypes[0]);
      Assert.AreEqual(MutantSrcType2, mCstCandidate.SrcTypes[1]);
    }

    [Test]
    public void TestMoveAllMuSrcToSrcTypes_CstCandidateNotSelected_AllMuSrcsNotMoved()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mCstCandidate.MuSrcTypes.Add(MutantSrcType2);

      mViewModel.MoveAllMuSrcsToSrcsCmd.Execute(null);
      Assert.IsFalse(mCstCandidate.SrcTypes.Any());
      Assert.AreEqual(2, mCstCandidate.MuSrcTypes.Count());
      Assert.AreEqual(MutantSrcType, mCstCandidate.MuSrcTypes[0]);
      Assert.AreEqual(MutantSrcType2, mCstCandidate.MuSrcTypes[1]);
    }

    [Test]
    public void TestMoveAllMuSrcToSrcTypes_CstCandidateSelected_MuSrcTypesExistInSrcTypes_AllMuSrcTypesNotMoved()
    {
      mCstCandidate.MuSrcTypes.Add(MutantSrcType);
      mCstCandidate.MuSrcTypes.Add(MutantSrcType2);
      mCstCandidate.SrcTypes.Add(MutantSrcType);
      mCstCandidate.SrcTypes.Add(MutantSrcType2);
      mViewModel.SelectedCstCandidate = mCstCandidate;

      mViewModel.MoveAllSrcsToMuSrcsCmd.Execute(null);
      Assert.AreEqual(2, mCstCandidate.SrcTypes.Count());
      Assert.AreEqual(2, mCstCandidate.MuSrcTypes.Count());
      Assert.AreEqual(MutantSrcType, mCstCandidate.SrcTypes[0]);
      Assert.AreEqual(MutantSrcType2, mCstCandidate.SrcTypes[1]);
      Assert.AreEqual(MutantSrcType, mCstCandidate.MuSrcTypes[0]);
      Assert.AreEqual(MutantSrcType2, mCstCandidate.MuSrcTypes[1]);
    }

    #endregion 

    #endregion

    #region CTT Candidate

    #region Add Target Type

    [Test]
    public void TestAddTargetType_CttCandidateSelected_CttCandidateDoesNotExist_TargetTypeAdded_FormCleared()
    {
      mViewModel.CttCandidates.Add(new CttCandidate
      {
        Line = 1,
        TargetTypes = new ObservableCollection<string>(),
        Rule = "rule"
      });
      mViewModel.SelectedCttCandidate = mCttCandidate;
      mViewModel.NewTrgtType = TargetType;
      mViewModel.AddTrgtTypeCmd.Execute(TargetType);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      foreach (var candidate in mViewModel.CttCandidates)
      {
        Assert.IsNotNull(candidate.TargetTypes.SingleOrDefault(t => t == TargetType));
      }
      Assert.AreEqual(string.Empty, mViewModel.NewTrgtType);
    }

    [Test]
    public void TestAddTargetType_CttCandidateNotSelected_CttCandidateDoesNotExist_TargetTypeNotAddedForAllCandidates_FormNotCleared()
    {
      mViewModel.CttCandidates.Add(new CttCandidate
      {
        Line = 1,
        TargetTypes = new ObservableCollection<string>(),
        Rule = "rule"
      });
      mViewModel.NewTrgtType = TargetType;
      mViewModel.AddTrgtTypeCmd.Execute(TargetType);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      foreach (var candidate in mViewModel.CttCandidates)
      {
        Assert.IsNull(candidate.TargetTypes.SingleOrDefault(t => t == TargetType));
      }
      Assert.AreEqual(TargetType, mViewModel.NewTrgtType);
    }

    [Test]
    public void TestAddTargetType_CttCandidateSelected_TargetTypeExists_TargetTypeNotAddedForAllCandidates_FormNotCleared()
    {
      var cttCandidate2 = new CttCandidate
      {
        MuTargetTypes = new ObservableCollection<string>(),
        TargetTypes = new ObservableCollection<string>(),
        Line = 1,
        Rule = "EndPoint_To_FinalNode",
        TargetType = "UML!ActivityFinalNode"
      };
      cttCandidate2.TargetTypes.Add(TargetType);
      mViewModel.CttCandidates.Add(cttCandidate2);

      mCttCandidate.TargetTypes.Add(TargetType);
      mViewModel.SelectedCttCandidate = mCttCandidate;
      mViewModel.NewTrgtType = TargetType;

      mViewModel.AddTrgtTypeCmd.Execute(TargetType);
      mDlgSrvc.Received().Error(string.Format("Source type {0} already exists for CTT mutant generation", TargetType), MutantsViewModel.AddTrgtTypeCaption);
      foreach (var candidate in mViewModel.CttCandidates)
      {
        Assert.AreEqual(1, candidate.TargetTypes.Count());
      }
      Assert.AreEqual(TargetType, mViewModel.NewTrgtType);
    }

    #endregion

    #region Delete Target Type

    [Test]
    public void TestDeleteTargetType_CttCandidateSelected_TargetTypeExists_TargetTypeDeleted()
    {
      mCttCandidate.TargetTypes.Add(TargetType);
      mViewModel.SelectedCttCandidate = mCttCandidate;
      mViewModel.SelectedTrgtType = TargetType;

      mViewModel.DelTrgtTypeCmd.Execute(null);
      Assert.IsNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == TargetType));
    }

    [Test]
    public void TestDeleteSourceType_CttCandidateNotSelected_TargetTypeExists_TargetTypeNotDeleted()
    {
      mCttCandidate.TargetTypes.Add(TargetType);
      mViewModel.SelectedTrgtType = TargetType;

      mViewModel.DelTrgtTypeCmd.Execute(null);
      Assert.IsNotNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == TargetType));
    }

    [Test]
    public void TestDeleteSourceType_CttCandidateSelected_TargetTypeNotSelected_TargetTypeExists_TargetTypeNotDeleted()
    {
      mCttCandidate.TargetTypes.Add(TargetType);
      mViewModel.SelectedCttCandidate = mCttCandidate;

      mViewModel.DelTrgtTypeCmd.Execute(null);
      Assert.IsNotNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == TargetType));
    }

    #endregion

    #region Add Mutant Target Type

    [Test]
    public void TestAddMutantTargetType_CttCandidateSelected_TargetTypeDoesNotExist_TargetTypeAdded_FormCleared()
    {
      mViewModel.SelectedCttCandidate = mCttCandidate;
      mViewModel.NewMuTrgtType = MutantTargetType;
      mViewModel.AddMuTrgtCmd.Execute(MutantTargetType);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsTrue(mCttCandidate.MuTargetTypes.Contains(MutantTargetType));
      Assert.AreEqual(string.Empty, mViewModel.NewMuTrgtType);
    }

    [Test]
    public void TestAddMutantTargetType_CttCandidateNotSelected_TargetTypeDoesNotExist_TargetTypeNotAdded_FormNotCleared()
    {
      mViewModel.NewMuTrgtType = MutantTargetType;
      mViewModel.AddMuTrgtCmd.Execute(MutantTargetType);

      Assert.IsFalse(mCttCandidate.MuTargetTypes.Contains(MutantTargetType));
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.AreEqual(MutantTargetType, mViewModel.NewMuTrgtType);
    }

    [Test]
    public void TestAddMutantTargetType_CttCandidateSelected_TargetTypeExists_TargetTypeNotAdded_FormNotCleared()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mViewModel.SelectedCttCandidate = mCttCandidate;
      mViewModel.NewMuTrgtType = MutantTargetType;

      mViewModel.AddMuTrgtCmd.Execute(MutantTargetType);
      mDlgSrvc.Received().Error(string.Format("Mutant target type {0} already added for rule {1}", MutantTargetType, mCttCandidate.Rule), MutantsViewModel.AddMuTrgtTypeCaption);
      Assert.AreEqual(1, mCttCandidate.MuTargetTypes.Count());
      Assert.AreEqual(MutantTargetType, mViewModel.NewMuTrgtType);
    }

    #endregion

    #region Delete Mutant Target Type

    [Test]
    public void TestDeleteMutantTargetType_CttCandidateSelected_TargetTypeExists_TargetTypeDeleted()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mViewModel.SelectedCttCandidate = mCttCandidate;
      mViewModel.SelectedMuTrgtType = MutantTargetType;

      mViewModel.DelMuTrgtTypeCmd.Execute(null);
      Assert.IsNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == MutantTargetType));
    }

    [Test]
    public void TestDeleteMutantTargetType_CttCandidateNotSelected_TargetTypeExists_TargetTypeNotDeleted()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mViewModel.SelectedMuTrgtType = MutantTargetType;

      mViewModel.DelMuTrgtTypeCmd.Execute(null);
      Assert.IsNotNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == MutantTargetType));
    }

    [Test]
    public void TestDeleteMutantTargetType_TargetTypeNotSelected_TargetTypeExists_TargetTypeNotDeleted()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mViewModel.SelectedCttCandidate = mCttCandidate;

      mViewModel.DelMuTrgtTypeCmd.Execute(null);
      Assert.IsNotNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == MutantTargetType));
    }

    #endregion

    #region Move Target Type to Mutant Target Types

    [Test]
    public void TestMoveTargetTypeToMutantTargetTypes_CttCandidateSelected_TargetTypeSelected_TargetTypeDeletedFromTargetTypesAndAddedToMutantTargetTypes()
    {
      mCttCandidate.TargetTypes.Add(TargetType);
      mViewModel.SelectedCttCandidate = mCttCandidate;
      mViewModel.SelectedTrgtType = TargetType;

      mViewModel.MoveTrgtToMuTrgtsCmd.Execute(null);
      Assert.IsNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == TargetType));
      Assert.IsNotNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == TargetType));
    }

    [Test]
    public void TestMoveTargetTypeToMutantTargetTypes_CttCandidateNotSelected_TargetTypeNotMoved()
    {
      mCttCandidate.TargetTypes.Add(TargetType);
      mViewModel.SelectedTrgtType = TargetType;

      mViewModel.MoveTrgtToMuTrgtsCmd.Execute(null);
      Assert.IsNotNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == TargetType));
      Assert.IsNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == TargetType));
    }

    [Test]
    public void TestMoveTargetTypeToMutantTargetTypes_CttCandidateSelected_TargetTypeNotSelected_TargetTypeNotMoved()
    {
      mCttCandidate.TargetTypes.Add(TargetType);
      mViewModel.SelectedCttCandidate = mCttCandidate;

      mViewModel.MoveTrgtToMuTrgtsCmd.Execute(null);
      Assert.IsNotNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == TargetType));
      Assert.IsNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == TargetType));
    }

    [Test]
    public void TestMoveTargetTypeToMutantTargetTypes_CttCandidateSelected_TargetTypeSelected_TargetTypeExistsInMutantTargetTypes_TargetTypeNotMoved()
    {
      mCttCandidate.TargetTypes.Add(TargetType);
      mCttCandidate.MuTargetTypes.Add(TargetType);
      mViewModel.SelectedCttCandidate = mCttCandidate;
      mViewModel.SelectedSrcType = TargetType;

      mViewModel.MoveTrgtToMuTrgtsCmd.Execute(null);
      Assert.IsNotNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == TargetType));
      Assert.IsNotNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == TargetType));
      Assert.AreEqual(1, mCttCandidate.TargetTypes.Count());
      Assert.AreEqual(1, mCttCandidate.MuTargetTypes.Count());
    }

    #endregion

    #region Move Mutant Target Type to Target Types

    [Test]
    public void TestMoveMutantTargetTypeToTargetTypes_CttCandidateSelected_MuTargetTypeSelected_MuTargetTypeRemovedFromMuTargetTypesAndAddedToTargetTypes()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mViewModel.SelectedCttCandidate = mCttCandidate;
      mViewModel.SelectedMuTrgtType = MutantTargetType;

      mViewModel.MoveMuTrgtToTrgtsCmd.Execute(null);
      Assert.IsNotNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == MutantTargetType));
      Assert.IsNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == MutantTargetType));
    }

    [Test]
    public void TestMoveMutantTargetTypeToTargetTypes_CttCandidateNotSelected_MuTargetTypeSelected_MuTargetTypeNotMoved()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mViewModel.SelectedMuTrgtType = MutantTargetType;

      mViewModel.MoveMuTrgtToTrgtsCmd.Execute(null);
      Assert.IsNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == MutantTargetType));
      Assert.IsNotNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == MutantTargetType));
    }

    [Test]
    public void TestMoveMutantTargetTypeToTargetTypes_CttCandidateSelected_MuTargetTypeNotSelected_MuTargetTypeNotMoved()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mViewModel.SelectedMuTrgtType = MutantTargetType;

      mViewModel.MoveMuTrgtToTrgtsCmd.Execute(null);
      Assert.IsNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == MutantTargetType));
      Assert.IsNotNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == MutantTargetType));
    }

    [Test]
    public void TestMoveMutantTargetTypeToTargetTypes_CttCandidateSelected_MuTargetTypeSelected_MuTargetTypeExistsInTargetTypes_MuTargetTypeNotMoved()
    {
      mCttCandidate.TargetTypes.Add(MutantTargetType);
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mViewModel.SelectedCttCandidate = mCttCandidate;
      mViewModel.SelectedMuTrgtType = MutantTargetType;

      mViewModel.MoveMuTrgtToTrgtsCmd.Execute(null);
      Assert.IsNotNull(mCttCandidate.TargetTypes.SingleOrDefault(t => t == MutantTargetType));
      Assert.AreEqual(1, mCttCandidate.TargetTypes.Count());
      Assert.IsNotNull(mCttCandidate.MuTargetTypes.SingleOrDefault(t => t == MutantTargetType));
      Assert.AreEqual(1, mCttCandidate.MuTargetTypes.Count());
    }

    #endregion

    #region Move All Target Types to Mutant Target Types

    [Test]
    public void TestMoveAllTargetTypesToMuTargetTypes_CttCandidateSelected_TargetTypesDontExistInMuTargetTypes_AllTargetTypesRemovedFromTargetTypesAndAddedToMuTargetTypes()
    {
      mCttCandidate.TargetTypes.Add(TargetType);
      mCttCandidate.TargetTypes.Add(TargetType2);
      mViewModel.SelectedCttCandidate = mCttCandidate;

      mViewModel.MoveAllTrgtsToMuTrgtsCmd.Execute(null);
      Assert.IsFalse(mCttCandidate.TargetTypes.Any());
      Assert.AreEqual(2, mCttCandidate.MuTargetTypes.Count());
      Assert.AreEqual(TargetType, mCttCandidate.MuTargetTypes[0]);
      Assert.AreEqual(TargetType2, mCttCandidate.MuTargetTypes[1]);
    }

    [Test]
    public void TestMoveAllTargetTypesToMuTargetTypes_CttCandidateNotSelected_AllTargetTypesNotMoved()
    {
      mCttCandidate.TargetTypes.Add(TargetType);
      mCttCandidate.TargetTypes.Add(TargetType2);

      mViewModel.MoveAllTrgtsToMuTrgtsCmd.Execute(null);
      Assert.AreEqual(2, mCttCandidate.TargetTypes.Count());
      Assert.AreEqual(TargetType, mCttCandidate.TargetTypes[0]);
      Assert.AreEqual(TargetType2, mCttCandidate.TargetTypes[1]);
      Assert.IsFalse(mCttCandidate.MuTargetTypes.Any());
    }

    [Test]
    public void TestMoveAllTargetTypesToMuTargetTypes_CttCandidateSelected_TargetTypesExistInMuTargetTypes_AllTargetTypesNotMoved()
    {
      mCttCandidate.TargetTypes.Add(TargetType);
      mCttCandidate.TargetTypes.Add(TargetType2);
      mCttCandidate.MuTargetTypes.Add(TargetType);
      mCttCandidate.MuTargetTypes.Add(TargetType2);
      mViewModel.SelectedCttCandidate = mCttCandidate;

      mViewModel.MoveAllTrgtsToMuTrgtsCmd.Execute(null);
      Assert.AreEqual(2, mCttCandidate.TargetTypes.Count());
      Assert.AreEqual(TargetType, mCttCandidate.TargetTypes[0]);
      Assert.AreEqual(TargetType2, mCttCandidate.TargetTypes[1]);
      Assert.AreEqual(2, mCttCandidate.MuTargetTypes.Count());
      Assert.AreEqual(TargetType, mCttCandidate.MuTargetTypes[0]);
      Assert.AreEqual(TargetType2, mCttCandidate.MuTargetTypes[1]);
    }

    #endregion

    #region Move Al Mutant Target Types to Target Types

    [Test]
    public void TestMoveAllMuTargetsToTargetTypes_CttCandidateSelected_CandidateHasMuTargetTypes_AllMuTargetTypesMovedToTargetTypes()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mCttCandidate.MuTargetTypes.Add(MutantTargetType2);
      mViewModel.SelectedCttCandidate = mCttCandidate;

      mViewModel.MoveAllMuTrgtsToTrgtsCmd.Execute(null);
      Assert.IsFalse(mCttCandidate.MuTargetTypes.Any());
      Assert.AreEqual(2, mCttCandidate.TargetTypes.Count());
      Assert.AreEqual(MutantTargetType, mCttCandidate.TargetTypes[0]);
      Assert.AreEqual(MutantTargetType2, mCttCandidate.TargetTypes[1]);
    }

    [Test]
    public void TestMoveAllMuTargetsToTargetTypes_CttCandidateNotSelected_AllMuTargetsNotMoved()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mCttCandidate.MuTargetTypes.Add(MutantTargetType2);

      mViewModel.MoveAllMuTrgtsToTrgtsCmd.Execute(null);
      Assert.IsFalse(mCttCandidate.TargetTypes.Any());
      Assert.AreEqual(2, mCttCandidate.MuTargetTypes.Count());
      Assert.AreEqual(MutantTargetType, mCttCandidate.MuTargetTypes[0]);
      Assert.AreEqual(MutantTargetType2, mCttCandidate.MuTargetTypes[1]);
    }

    [Test]
    public void TestMoveAllMuTargetToTargetTypes_CttCandidateSelected_MuTargetTypesExistInTargetTypes_AllMuTargetTypesNotMoved()
    {
      mCttCandidate.MuTargetTypes.Add(MutantTargetType);
      mCttCandidate.MuTargetTypes.Add(MutantTargetType2);
      mCttCandidate.TargetTypes.Add(MutantTargetType);
      mCttCandidate.TargetTypes.Add(MutantTargetType2);
      mViewModel.SelectedCttCandidate = mCttCandidate;

      mViewModel.MoveAllMuTrgtsToTrgtsCmd.Execute(null);
      Assert.AreEqual(2, mCttCandidate.TargetTypes.Count());
      Assert.AreEqual(2, mCttCandidate.MuTargetTypes.Count());
      Assert.AreEqual(MutantTargetType, mCttCandidate.TargetTypes[0]);
      Assert.AreEqual(MutantTargetType2, mCttCandidate.TargetTypes[1]);
      Assert.AreEqual(MutantTargetType, mCttCandidate.MuTargetTypes[0]);
      Assert.AreEqual(MutantTargetType2, mCttCandidate.MuTargetTypes[1]);
    }

    #endregion

    #endregion

    #region AAM Candidates

    #region Add Mutant Attribute Mapping

    [Test]
    public void TestAddMutantAttrMapping_AamCandidateSelected_MutantAttrMappingDoesNotExist_MutantAttrMappingAdded_FormCleared()
    {
      mViewModel.SelectedAamCandidate = mAamCandidate;
      mViewModel.NewMuAttr = MuAttrMapping;

      mViewModel.AddMuAttrCmd.Execute(MuAttrMapping);
      Assert.IsNotNull(mAamCandidate.MuMappings.SingleOrDefault(m => m == MuAttrMapping));
      Assert.AreEqual(string.Empty, mViewModel.NewMuAttr);
    }

    [Test]
    public void TestAddMutantAttrMapping_AamCandidateNotSelected_MutantAttrMappingDoesNotExist_MutantAttrMappingNotAdded_FormNotCleared()
    {
      mViewModel.NewMuAttr = MuAttrMapping;

      mViewModel.AddMuAttrCmd.Execute(MuAttrMapping);
      Assert.IsNull(mAamCandidate.MuMappings.SingleOrDefault(m => m == MuAttrMapping));
      Assert.AreEqual(MuAttrMapping, mViewModel.NewMuAttr);
    }

    [Test]
    public void TestAddMutantAttrMapping_AamCandidateSelected_MutantAttrMappingExists_MutantAttrMappingNotAdded_FormNotCleared()
    {
      mAamCandidate.MuMappings.Add(MuAttrMapping);
      mViewModel.SelectedAamCandidate = mAamCandidate;
      mViewModel.NewMuAttr = MuAttrMapping;

      mViewModel.AddMuAttrCmd.Execute(MuAttrMapping);
      Assert.AreEqual(1, mAamCandidate.MuMappings.Count());
      Assert.AreEqual(MuAttrMapping, mViewModel.NewMuAttr);
    }

    #endregion

    #region Delete Mutant Attribute Mapping

    [Test]
    public void TestDeleteMutantAttrMapping_AamCandidateSelected_MutantAttMappingSelected_MutantAttMappingDeleted()
    {
      mAamCandidate.MuMappings.Add(MuAttrMapping);
      mViewModel.SelectedAamCandidate = mAamCandidate;
      mViewModel.SelectedMuAttr = MuAttrMapping;

      mViewModel.DelMuAttrCmd.Execute(null);
      Assert.IsNull(mAamCandidate.MuMappings.SingleOrDefault(t => t == MuAttrMapping));
    }

    [Test]
    public void TestDeleteMutantAttributeMapping_AamCandidateNotSelected_MutantAttMappingSelected_AttributeMappingNotDeleted()
    {
      mAamCandidate.MuMappings.Add(MuAttrMapping);
      mViewModel.SelectedMuAttr = MuAttrMapping;

      mViewModel.DelMuAttrCmd.Execute(null);
      Assert.IsNotNull(mAamCandidate.MuMappings.SingleOrDefault(t => t == MuAttrMapping));
    }

    [Test]
    public void TestDeleteMutantAttributeMapping_AamCandidateSelected_AttrMappingNotSelected_AttrMappingExists_AttrMappingNotDeleted()
    {
      mAamCandidate.MuMappings.Add(MuAttrMapping);
      mViewModel.SelectedAamCandidate = mAamCandidate;

      mViewModel.DelMuAttrCmd.Execute(null);
      Assert.IsNotNull(mAamCandidate.MuMappings.SingleOrDefault(t => t == MuAttrMapping));
    }

    #endregion 

    #endregion

    #region AFE Candidates

    #region Add Mutant Filtering Expression

    [Test]
    public void TestAddMutantFltrExpr_AfeCandidateSelected_MutantFltrExprDoesNotExist_MutantFltrExprAdded_FormCleared()
    {
      mViewModel.SelectedAfeCandidate = mAfeCandidate;
      mViewModel.NewMuFiltrExpr = MuFltrExpr;

      mViewModel.AddMuFiltrExprCmd.Execute(MuFltrExpr);
      Assert.IsNotNull(mAfeCandidate.MuFilteringExpressions.SingleOrDefault(m => m == MuFltrExpr));
      Assert.AreEqual(string.Empty, mViewModel.NewMuFiltrExpr);
    }

    [Test]
    public void TestAddMutantFltrExpr_AfeCandidateNotSelected_MutantFltrExprDoesNotExist_MutantFltrExprNotAdded_FormNotCleared()
    {
      mViewModel.NewMuFiltrExpr = MuFltrExpr;

      mViewModel.AddMuFiltrExprCmd.Execute(MuFltrExpr);
      Assert.IsNull(mAfeCandidate.MuFilteringExpressions.SingleOrDefault(m => m == MuFltrExpr));
      Assert.AreEqual(MuFltrExpr, mViewModel.NewMuFiltrExpr);
    }

    [Test]
    public void TestAddMutantFltrExpr_AfeCandidateSelected_MutantFltrExprExists_MutantFltrExprNotAdded_FormNotCleared()
    {
      mAfeCandidate.MuFilteringExpressions.Add(MuFltrExpr);
      mViewModel.SelectedAfeCandidate = mAfeCandidate;
      mViewModel.NewMuFiltrExpr = MuFltrExpr;

      mViewModel.AddMuFiltrExprCmd.Execute(MuFltrExpr);
      Assert.AreEqual(1, mAfeCandidate.MuFilteringExpressions.Count());
      Assert.AreEqual(MuFltrExpr, mViewModel.NewMuFiltrExpr);
    }

    #endregion

    #region Delete Mutant Filtering Expression

    [Test]
    public void TestDeleteMutantFltrExpr_AfeCandidateSelected_MuFltrExprSelected_MutantFltrExprDeleted()
    {
      mAfeCandidate.MuFilteringExpressions.Add(MuFltrExpr);
      mViewModel.SelectedAfeCandidate = mAfeCandidate;
      mViewModel.SelectedMuFltrExpr = MuFltrExpr;

      mViewModel.DelMuFiltrExprCmd.Execute(null);
      Assert.IsNull(mAfeCandidate.MuFilteringExpressions.SingleOrDefault(t => t == MuFltrExpr));
    }

    [Test]
    public void TestDeleteMutantFltrExpr_AfeCandidateNotSelected_MuFltrExprSelected_MutantFltrExprNotDeleted()
    {
      mAfeCandidate.MuFilteringExpressions.Add(MuFltrExpr);
      mViewModel.SelectedMuFltrExpr = MuFltrExpr;

      mViewModel.DelMuFiltrExprCmd.Execute(null);
      Assert.IsNotNull(mAfeCandidate.MuFilteringExpressions.SingleOrDefault(t => t == MuFltrExpr));
    }

    [Test]
    public void TestDeleteMutantFltrExpr_AfeCandidateSelected_MuFltrExprNotSelected_MutantFltrExprNotDeleted()
    {
      mAfeCandidate.MuFilteringExpressions.Add(MuFltrExpr);
      mViewModel.SelectedAfeCandidate = mAfeCandidate;

      mViewModel.DelMuFiltrExprCmd.Execute(null);
      Assert.IsNotNull(mAfeCandidate.MuFilteringExpressions.SingleOrDefault(t => t == MuFltrExpr));
    }

    #endregion

    #endregion

    #endregion
  }
}
