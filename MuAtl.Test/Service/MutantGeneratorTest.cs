using NUnit.Framework;
using MuAtl.Service;
using System.IO;
using MuAtl.Service.Generator;
using MuAtl.Model;
using MuAtl.Service.Reader.Model;
using GalaSoft.MvvmLight.Threading;
using System.Linq;
using System.Collections.ObjectModel;

namespace MuAtl.Test.Service
{
  [TestFixture]
  public class MutantGeneratorTest
  {
    private MutantGenerator mGenerator = new MutantGenerator();
    private MuAtlProject mProject;
    private readonly string MuDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "Mutants");

    [SetUp]
    public void Init()
    {
      mProject = new Repository().Load<MuAtlProject>(@"TestData\ucm2ad_muatl.xml");
      DispatcherHelper.Initialize();
    }

    [Test]
    public void TestGenerate_AamCandidatesSelected_AamMutantAdded()
    {
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_AAM_1",
        Path = Path.Combine(MuDir, "AAM", "ucm2ad_AAM_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.AAM
      };

      mGenerator.Generate(MuDir, mProject, new[]
      {
        new AamCandidate
        {
          Line = 60,
          Rule = "StartPoint_To_InitialNode",
          OutPattern = "n:UML!InitialNode(name<-p.name)",
          MuMappings = new ObservableCollection<string>(
            new [] { "a.x <- b.y" } )
        }
      });
      Assert.AreEqual(74, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestGenerate_DamCandidatesSelected_DamMutantAdded()
    {
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_DAM_40",
        Path = Path.Combine(MuDir, "DAM", "ucm2ad_DAM_40", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.DAM
      };

      mGenerator.Generate(MuDir, mProject, new[]
      {
        new DamCandidate
        {
          Line = 61,
          Rule = "StartPoint_To_InitialNode",
          Mapping = "name<-p.name"
        }
      });

      Assert.AreEqual(74, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestGenerate_AfeCandidateSelected_AfeMutantAdded()
    {
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_AFE_1",
        Path = Path.Combine(MuDir, "AFE", "ucm2ad_AFE_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.AFE
      };

      mGenerator.Generate(MuDir, mProject, new[]
      {
         new AfeCandidate
        {
          Line = 58,
          Rule = "StartPoint_To_InitialNode",
          MuFilteringExpressions = new ObservableCollection<string>(
            new [] { "a.x > 0 "} )
        }
      });

      Assert.AreEqual(74, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestGenerate_DfeCandidateSelected_DfeMutantAdded()
    {
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_DFE_3",
        Path = Path.Combine(MuDir, "DFE", "ucm2ad_DFE_3", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.DFE
      };

      mGenerator.Generate(MuDir, mProject, new[]
      {
        new DfeCandidate
        {
          Line = 68,
          Rule = "StartPoint_To_InitialNode2",
          Filtering = "p.abc>1"
        }
      });

      Assert.AreEqual(74, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestGenerate_CstCandidateSelected_CstMutantAdded()
    {
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_CST_1",
        Path = Path.Combine(MuDir, "CST", "ucm2ad_CST_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.CST
      };

      mGenerator.Generate(MuDir, mProject, new[]
      {
        new CstCandidate
        {
          Line = 51,
          Rule = "StartPoint_To_InitialNode",
          SourceType = "UCM!\"ucm::map::StartPoint\"",
          MuSrcTypes = new ObservableCollection<string>(
            new [] { "UCM!\"ucm::map::EndPoint\"" } )
        }
      });

      Assert.AreEqual(74, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestGenerate_CttCandidateSelected_CttMutantAdded()
    {
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_CTT_1",
        Path = Path.Combine(MuDir, "CTT", "ucm2ad_CTT_1", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.CTT
      };

      mGenerator.Generate(MuDir, mProject, new[]
      {
        new CttCandidate
        {
          Line = 60,
          Rule = "StartPoint_To_InitialNode",
          TargetType = "UML!InitialNode",
          MuTargetTypes = new ObservableCollection<string>(
            new [] { "UML!ActivityFinalNode" } )
        }
      });

      Assert.AreEqual(74, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestGenerate_M2lCandidateSelected_CttMutantAdded()
    {
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_M2L_2",
        Path = Path.Combine(MuDir, "M2L", "ucm2ad_M2L_2", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.M2L
      };

      mGenerator.Generate(MuDir, mProject, new[]
      {
        new M2lCandidate
        {
          Line = 614,
          Rule = "URNDefinition_To_UMLPackage"
        }
      });

      Assert.AreEqual(74, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestGenerate_L2mCandidateSelected_CttMutantAdded()
    {
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_L2M_14",
        Path = Path.Combine(MuDir, "L2M", "ucm2ad_L2M_14", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.L2M
      };

      mGenerator.Generate(MuDir, mProject, new[]
      {
        new L2mCandidate
        {
          Line = 56,
          Rule = "StartPoint_To_InitialNode"
        }
      });

      Assert.AreEqual(74, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestGenerate_DusCandidateSelected_CttMutantAdded()
    {
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_DUS_7",
        Path = Path.Combine(MuDir, "DUS", "ucm2ad_DUS_7", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.DUS
      };

      mGenerator.Generate(MuDir, mProject, new[]
      {
        new DusCandidate
        {
          Line = 4,
          Library = "PathNode"
        }
      });

      Assert.AreEqual(74, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestGenerate_DrsCandidateSelected_DrsMutantAdded()
    {
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_DRS_12",
        Path = Path.Combine(MuDir, "DRS", "ucm2ad_DRS_12", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.DRS
      };

      mGenerator.Generate(MuDir, mProject, new[]
      {
        new DrsCandidate
        {
          Line = 4,
          Rule = "InitUmlEdge",
          Statement = "e;"
        }
      });

      Assert.AreEqual(74, mProject.Mutants.Count());
      var actualMutant = mProject.Mutants.SingleOrDefault(m => m.Name == expectedtMutant.Name);
      Assert.IsNotNull(actualMutant);
      Assert.AreEqual(expectedtMutant.Name, actualMutant.Name);
      Assert.AreEqual(expectedtMutant.Path, actualMutant.Path);
      Assert.AreEqual(expectedtMutant.Status, actualMutant.Status);
      Assert.AreEqual(expectedtMutant.Type, actualMutant.Type);
    }

    [Test]
    public void TestGenerate_CemCandidateSelected_CemMutantAdded()
    {
      var expectedtMutant = new MuAtlMutant
      {
        Name = "ucm2ad_CEM_2",
        Path = Path.Combine(MuDir, "CEM", "ucm2ad_CEM_2", "ucm2ad.atl"),
        Status = MutantStatus.Undetermined,
        Type = MutantType.CEM
      };

      mGenerator.Generate(MuDir, mProject, new[]
      {
        new CemCandidate
        {
          Line = 2
        }
      });

      Assert.AreEqual(74, mProject.Mutants.Count());
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
