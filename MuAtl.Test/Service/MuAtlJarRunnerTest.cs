using System.Linq;
using NUnit.Framework;
using MuAtl.Service.Runner;
using MuAtl.Model;
using MuAtl.Service;
using System.IO;
using System.Collections.ObjectModel;
using MuAtl.Model.MuAtlJavaArgs;

namespace MuAtl.Test.Service
{
  [TestFixture]
  public class MuAtlJarRunnerTest
  {
    private MuAtlJarRunner runner;
    private ProjectConfig config;
    private MuAtlTestCase testCase;
    private MuAtlMutant mutant;
    private string muDir;
    private string tcDir;
    private string logPath;

    [SetUp]
    public void Init()
    {
      testCase = new MuAtlTestCase
      {
        Name = "tc1",
        InModels = new ObservableCollection<TestCaseInput>(
        new[]
        {
                new TestCaseInput
                {
                  Name = "map",
                  Path = Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    @"TestData\input.jucm")
                }
        }),
        OutModel = new TestCaseOutput
        {
          Name = "uml",
          Path = Path.Combine(
          TestContext.CurrentContext.TestDirectory,
          @"TestData\default.uml")
        }
      };

      config = new Repository().Load<MuAtlProject>(@"TestData\ucm2ad_muatl_onlyconfig.xml").Dependency;
      config.OutMetamodel.Path = Path.Combine(TestContext.CurrentContext.TestDirectory, config.OutMetamodel.Path);
      foreach (var inMm in config.InMetamodels)
      {
        inMm.Path = Path.Combine(TestContext.CurrentContext.TestDirectory, inMm.Path);
        foreach (var depMm in inMm.Dependencies)
          depMm.Path = Path.Combine(TestContext.CurrentContext.TestDirectory, depMm.Path);
      }
    }

    [Test]
    public void TestRun_LogCreated_OutputNotCreated_ResultFail()
    {
      muDir = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\mudir\CEM\ucm2ad_CEM_1");
      mutant = new MuAtlMutant
      {
        Path = Path.Combine(muDir, "ucm2ad.atl"),
        Name = "ucm2ad_CEM_1",
        Status = MutantStatus.Undetermined,
        Type = MutantType.CEM
      };

      tcDir = Path.Combine(muDir, testCase.Name);
      Directory.CreateDirectory(tcDir);
      var tcOut = new MuAtlTestCaseOutput
      {
        Name = testCase.OutModel.Name,
        Path = Path.Combine(tcDir, Path.GetFileName(testCase.OutModel.Path))
      };
      logPath = Path.Combine(tcDir, mutant.Name + "_" + testCase.Name + ".txt");

      runner = new MuAtlJarRunner(config, mutant.Path,
        config.Libraries.Select(l => Path.Combine(TestContext.CurrentContext.TestDirectory, l.Path)),
        config.SuperImposedModules.Select(s => Path.Combine(TestContext.CurrentContext.TestDirectory, s.Path)),
        testCase.InModels, tcOut, AtlCompilerType.Default, AtlVmType.Emf, logPath,
        false, false, false, false);
      Assert.AreEqual(-1, runner.Run());
      Assert.IsFalse(File.Exists(tcOut.Path));
      Assert.IsTrue(File.Exists(logPath));
    }

    [Test]
    public void TestRun_LogCreated_OutputNotCreated_ResultUnknown()
    {
      muDir = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\mudir\M2L\ucm2ad_M2L_1");
      mutant = new MuAtlMutant
      {
        Path = Path.Combine(muDir, "ucm2ad.atl"),
        Name = "ucm2ad_M2L_1",
        Status = MutantStatus.Undetermined,
        Type = MutantType.M2L
      };

      tcDir = Path.Combine(muDir, testCase.Name);
      Directory.CreateDirectory(tcDir);
      var tcOut = new MuAtlTestCaseOutput
      {
        Name = testCase.OutModel.Name,
        Path = Path.Combine(tcDir, Path.GetFileName(testCase.OutModel.Path))
      };
      logPath = Path.Combine(tcDir, mutant.Name + "_" + testCase.Name + ".txt");

      runner = new MuAtlJarRunner(config, mutant.Path,
        config.Libraries.Select(l => Path.Combine(TestContext.CurrentContext.TestDirectory, l.Path)),
        config.SuperImposedModules.Select(s => Path.Combine(TestContext.CurrentContext.TestDirectory, s.Path)),
        testCase.InModels, tcOut, AtlCompilerType.Default, AtlVmType.Emf, logPath,
        false, false, false, false);
      Assert.AreEqual(0, runner.Run());
      Assert.IsTrue(File.Exists(tcOut.Path));
      Assert.IsTrue(File.Exists(logPath));
    }

    [Test]
    public void TestRun_ConfigInvalid_LogCreated_OutputCreated_ErrorOutcome()
    {
      config.InMetamodels[0].Dependencies.Clear(); //error in config

      muDir = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\mudir\M2L\ucm2ad_M2L_1");
      mutant = new MuAtlMutant
      {
        Path = Path.Combine(muDir, "ucm2ad.atl"),
        Name = "ucm2ad_M2L_1",
        Status = MutantStatus.Undetermined,
        Type = MutantType.M2L
      };

      tcDir = Path.Combine(muDir, testCase.Name);
      Directory.CreateDirectory(tcDir);
      var tcOut = new MuAtlTestCaseOutput
      {
        Name = testCase.OutModel.Name,
        Path = Path.Combine(tcDir, Path.GetFileName(testCase.OutModel.Path))
      };
      logPath = Path.Combine(tcDir, mutant.Name + "_" + testCase.Name + ".txt");

      runner = new MuAtlJarRunner(config, mutant.Path,
        config.Libraries.Select(l => Path.Combine(TestContext.CurrentContext.TestDirectory, l.Path)),
        config.SuperImposedModules.Select(s => Path.Combine(TestContext.CurrentContext.TestDirectory, s.Path)),
        testCase.InModels, tcOut, AtlCompilerType.Default, AtlVmType.Emf, logPath,
        false, false, false, false);
      Assert.AreEqual(-2, runner.Run());
      Assert.IsFalse(File.Exists(tcOut.Path));
      Assert.IsTrue(File.Exists(logPath));
    }

    [TearDown]
    public void Cleanup()
    {
      if (Directory.Exists(tcDir))
      {
        Directory.Delete(tcDir, true);
      }
      File.Delete(Path.Combine(muDir, "ucm2ad.asm"));
    }
  }
}
