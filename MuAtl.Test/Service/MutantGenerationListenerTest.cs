using NUnit.Framework;
using MuAtl.Service;
using System.IO;
using Antlr4.Runtime.Tree;
using MuAtl.Service.Generator;
using MuAtl.Model;
using MuAtl.Service.Reader.Model;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Threading;
using System.Linq;
using System.Collections.ObjectModel;

namespace MuAtl.Test.Service
{
  [TestFixture]
  public class MutantGenerationListenerTest
  {
    private readonly string Module1 = Path.GetFullPath(@"TestData\ucm2ad.atl");
    private readonly string MuDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "Mutants");

    private IParseTree mTree;
    private MutantGenerationListener mSrvc;
    private ParseTreeWalker mWalker;
    private MuAtlProject mProject;

    [SetUp]
    public void Init()
    {
      var tokens = new AtlTokenizer().GetTokens(Module1);
      mTree = new AtlParser().Parse(tokens);
      mProject = new Repository().Load<MuAtlProject>(@"TestData\ucm2ad_muatl_onlyconfig.xml");

      var dic = new Dictionary<MutantType, int>();
      dic.Add(MutantType.AAM, 1);
      dic.Add(MutantType.AFE, 1);
      dic.Add(MutantType.CEM, 1);
      dic.Add(MutantType.CST, 1);
      dic.Add(MutantType.CTT, 1);
      dic.Add(MutantType.DAM, 1);
      dic.Add(MutantType.DFE, 1);
      dic.Add(MutantType.DRS, 1);
      dic.Add(MutantType.DUS, 1);
      dic.Add(MutantType.L2M, 1);
      dic.Add(MutantType.M2L, 1);

      mSrvc = new MutantGenerationListener(tokens, MuDir, mProject, dic);
      mSrvc.DamCandidates = new List<DamCandidate>();
      mSrvc.AamCandidates = new List<AamCandidate>();
      mSrvc.AfeCandidates = new List<AfeCandidate>();
      mSrvc.DfeCandidates = new List<DfeCandidate>();
      mSrvc.CstCandidates = new List<CstCandidate>();
      mSrvc.CttCandidates = new List<CttCandidate>();
      mSrvc.DusCandidates = new List<DusCandidate>();
      mSrvc.M2lCandidates = new List<M2lCandidate>();
      mSrvc.L2mCandidates = new List<L2mCandidate>();
      mSrvc.DrsCandidates = new List<DrsCandidate>();

      mWalker = new ParseTreeWalker();
      DispatcherHelper.Initialize();
    }

    [Test]
    public void TestWalk_M2LCandidateSelected_M2LMutantAdded()
    {
      mSrvc.M2lCandidates = new[]
      {
        new M2lCandidate
        {
          Line = 614,
          Rule = "URNDefinition_To_UMLPackage"
        }
      };
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_M2L_1",
        Path = Path.Combine(MuDir, "M2L", "ucm2ad_M2L_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.M2L
      };

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual(1, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestWalk_L2MCandidateSelected_L2MMutantAdded()
    {
      mSrvc.L2mCandidates = new[]
      {
        new L2mCandidate
        {
          Line = 56,
          Rule = "StartPoint_To_InitialNode"
        }
      };
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_L2M_1",
        Path = Path.Combine(MuDir, "L2M", "ucm2ad_L2M_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.L2M
      };

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual(1, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestWalk_CstCandidateSelected_CstMuantsAdded()
    {
      mSrvc.CstCandidates = new[]
      {
        new CstCandidate
        {
          Line = 51,
          Rule = "StartPoint_To_InitialNode",
          SourceType = "UCM!\"ucm::map::StartPoint\"",
          MuSrcTypes = new System.Collections.ObjectModel.ObservableCollection<string>(
            new [] { "UCM!\"ucm::map::EndPoint\"",  "UCM!\"ucm::map::RespRef\""} )
        }
      };

      var expectedtMutant1 = new MuAtlMutant
      {
        Name = "ucm2ad_CST_1",
        Path = Path.Combine(MuDir, "CST", "ucm2ad_CST_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.CST
      };

      var expectedtMutant2 = new MuAtlMutant
      {
        Name = "ucm2ad_CST_2",
        Path = Path.Combine(MuDir, "CST", "ucm2ad_CST_2", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.CST
      };

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual(2, mProject.Mutants.Count());
      var actualMutant1 = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant1.Name);
      Assert.IsNotNull(actualMutant1);
      Assert.AreEqual(expectedtMutant1.Name, actualMutant1.Name);
      Assert.AreEqual(expectedtMutant1.Path, actualMutant1.Path);
      Assert.AreEqual(expectedtMutant1.Status, actualMutant1.Status);
      Assert.AreEqual(expectedtMutant1.Type, actualMutant1.Type);

      var actualMutant2 = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant2.Name);
      Assert.IsNotNull(actualMutant1);
      Assert.AreEqual(expectedtMutant2.Name, actualMutant2.Name);
      Assert.AreEqual(expectedtMutant2.Path, actualMutant2.Path);
      Assert.AreEqual(expectedtMutant2.Status, actualMutant2.Status);
      Assert.AreEqual(expectedtMutant2.Type, actualMutant2.Type);
    }

    [Test]
    public void TestWalk_CttCandidateSelected_CttMuantsAdded()
    {
      mSrvc.CttCandidates = new[]
      {
        new CttCandidate
        {
          Line = 60,
          Rule = "StartPoint_To_InitialNode",
          TargetType = "UML!InitialNode",
          MuTargetTypes = new System.Collections.ObjectModel.ObservableCollection<string>(
            new [] { "UML!ActivityFinalNode", "UML!OpaqueAction" } )
        }
      };

      var expectedtMutant1 = new MuAtlMutant
      {
        Name = "ucm2ad_CTT_1",
        Path = Path.Combine(MuDir, "CTT", "ucm2ad_CTT_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.CTT
      };

      var expectedtMutant2 = new MuAtlMutant
      {
        Name = "ucm2ad_CTT_2",
        Path = Path.Combine(MuDir, "CTT", "ucm2ad_CTT_2", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.CTT
      };

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual(2, mProject.Mutants.Count());
      var actualMutant1 = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant1.Name);
      Assert.IsNotNull(actualMutant1);
      Assert.AreEqual(expectedtMutant1.Name, actualMutant1.Name);
      Assert.AreEqual(expectedtMutant1.Path, actualMutant1.Path);
      Assert.AreEqual(expectedtMutant1.Status, actualMutant1.Status);
      Assert.AreEqual(expectedtMutant1.Type, actualMutant1.Type);

      var actualMutant2 = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant2.Name);
      Assert.IsNotNull(actualMutant1);
      Assert.AreEqual(expectedtMutant2.Name, actualMutant2.Name);
      Assert.AreEqual(expectedtMutant2.Path, actualMutant2.Path);
      Assert.AreEqual(expectedtMutant2.Status, actualMutant2.Status);
      Assert.AreEqual(expectedtMutant2.Type, actualMutant2.Type);
    }

    [Test]
    public void TestWalk_AamCandidateSelected_AamMuantsAdded()
    {
      mSrvc.AamCandidates = new[]
      {
        new AamCandidate
        {
          Line = 60,
          Rule = "StartPoint_To_InitialNode",
          OutPattern = "n:UML!InitialNode(name<-p.name)",
          MuMappings = new ObservableCollection<string>(
            new [] { "a.x <- b.y", "c.x <- d.y" } )
        }
      };

      var expectedtMutant1 = new MuAtlMutant
      {
        Name = "ucm2ad_AAM_1",
        Path = Path.Combine(MuDir, "AAM", "ucm2ad_AAM_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.AAM
      };

      var expectedtMutant2 = new MuAtlMutant
      {
        Name = "ucm2ad_AAM_2",
        Path = Path.Combine(MuDir, "AAM", "ucm2ad_AAM_2", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.AAM
      };

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual(2, mProject.Mutants.Count());
      var actualMutant1 = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant1.Name);
      Assert.IsNotNull(actualMutant1);
      Assert.AreEqual(expectedtMutant1.Name, actualMutant1.Name);
      Assert.AreEqual(expectedtMutant1.Path, actualMutant1.Path);
      Assert.AreEqual(expectedtMutant1.Status, actualMutant1.Status);
      Assert.AreEqual(expectedtMutant1.Type, actualMutant1.Type);

      var actualMutant2 = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant2.Name);
      Assert.IsNotNull(actualMutant1);
      Assert.AreEqual(expectedtMutant2.Name, actualMutant2.Name);
      Assert.AreEqual(expectedtMutant2.Path, actualMutant2.Path);
      Assert.AreEqual(expectedtMutant2.Status, actualMutant2.Status);
      Assert.AreEqual(expectedtMutant2.Type, actualMutant2.Type);
    }

    [Test]
    public void TestWalk_DamCandidateSelected_DamMutantAdded()
    {
      mSrvc.DamCandidates = new[]
      {
        new DamCandidate
        {
          Line = 61,
          Rule = "StartPoint_To_InitialNode",
          Mapping = "name<-p.name"
        }
      };

      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_DAM_1",
        Path = Path.Combine(MuDir, "DAM", "ucm2ad_DAM_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.DAM
      };

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual(1, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestWalk_AfeCandidateSelected_AfeMutantAdded()
    {
      mSrvc.AfeCandidates = new[]
      {
        new AfeCandidate
        {
          Line = 58,
          Rule = "StartPoint_To_InitialNode",
          MuFilteringExpressions = new ObservableCollection<string>(
            new [] { "a.x > 0 ", "b.y > 1" } )
        }
      };

      var expectedtMutant1 = new MuAtlMutant
      {
        Name = "ucm2ad_AFE_1",
        Path = Path.Combine(MuDir, "AFE", "ucm2ad_AFE_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.AFE
      };

      var expectedtMutant2 = new MuAtlMutant
      {
        Name = "ucm2ad_AFE_2",
        Path = Path.Combine(MuDir, "AFE", "ucm2ad_AFE_2", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.AFE
      };

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual(2, mProject.Mutants.Count());
      var actualMutant1 = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant1.Name);
      Assert.IsNotNull(actualMutant1);
      Assert.AreEqual(expectedtMutant1.Name, actualMutant1.Name);
      Assert.AreEqual(expectedtMutant1.Path, actualMutant1.Path);
      Assert.AreEqual(expectedtMutant1.Status, actualMutant1.Status);
      Assert.AreEqual(expectedtMutant1.Type, actualMutant1.Type);

      var actualMutant2 = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant2.Name);
      Assert.IsNotNull(actualMutant2);
      Assert.AreEqual(expectedtMutant2.Name, actualMutant2.Name);
      Assert.AreEqual(expectedtMutant2.Path, actualMutant2.Path);
      Assert.AreEqual(expectedtMutant2.Status, actualMutant2.Status);
      Assert.AreEqual(expectedtMutant2.Type, actualMutant2.Type);
    }

    [Test]
    public void TestWalk_DfeCandidateSelected_DamMutantAdded()
    {
      mSrvc.DfeCandidates = new[]
      {
        new DfeCandidate
        {
          Line = 68,
          Rule = "StartPoint_To_InitialNode2",
          Filtering = "p.abc>1"
        }
      };

      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_DFE_1",
        Path = Path.Combine(MuDir, "DFE", "ucm2ad_DFE_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.DFE
      };

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual(1, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestWalk_DusCandidateSelected_DusMutantAdded()
    {
      mSrvc.DusCandidates = new[]
      {
        new DusCandidate
        {
          Line = 4,
          Library = "PathNode"
        }
      };

      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_DUS_1",
        Path = Path.Combine(MuDir, "DUS", "ucm2ad_DUS_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.DUS
      };

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual(1, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestWalk_DrsCandidateSelected_DusMutantAdded()
    {
      mSrvc.DrsCandidates = new[]
      {
        new DrsCandidate
        {
          Line = 4,
          Rule = "InitUmlEdge",
          Statement = "e;"
        }
      };

      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_DRS_1",
        Path = Path.Combine(MuDir, "DRS", "ucm2ad_DRS_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.DRS
      };

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual(1, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestWalk_CemSelected_CemMutantAdded()
    {
      mSrvc.IsCemCandidate = true;

      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_CEM_1",
        Path = Path.Combine(MuDir, "CEM", "ucm2ad_CEM_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.CEM
      };

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual(1, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [TearDown]
    public void CleanUp()
    {
      if (Directory.Exists(MuDir))
      {
        Directory.Delete(MuDir, true);
      }
    }
  }
}
