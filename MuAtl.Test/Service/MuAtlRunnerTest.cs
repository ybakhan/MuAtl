using NUnit.Framework;
using MuAtl.Service.Runner;
using MuAtl.Model;
using System.Collections.ObjectModel;
using MuAtl.Service;
using MuAtl.Model.MuAtlJavaArgs;
using System.IO;

namespace MuAtl.Test.Service
{
  [TestFixture]
  public class MuAtlRunnerTest
  {
    private MutantRunner runner;
    private MuAtlTestCase testCase;
    private MuAtlMutant mutant;
    private string muDir;

    [SetUp]
    public void Init()
    {
      runner = new MutantRunner();
      runner.Config = new Repository().Load<MuAtlProject>(@"TestData\ucm2ad_muatl_onlyconfig.xml").Dependency;

      foreach (var lib in runner.Config.Libraries)
        lib.Path = Path.Combine(TestContext.CurrentContext.TestDirectory, lib.Path);

      foreach (var supMod in runner.Config.SuperImposedModules)
        supMod.Path = Path.Combine(TestContext.CurrentContext.TestDirectory, supMod.Path);

      runner.Config.OutMetamodel.Path = Path.Combine(TestContext.CurrentContext.TestDirectory, runner.Config.OutMetamodel.Path);

      foreach (var inMm in runner.Config.InMetamodels)
      {
        inMm.Path = Path.Combine(TestContext.CurrentContext.TestDirectory, inMm.Path);
        foreach (var depMm in inMm.Dependencies)
          depMm.Path = Path.Combine(TestContext.CurrentContext.TestDirectory, depMm.Path);
      }

      runner.Compiler = AtlCompilerType.Default;
      runner.Vm = AtlVmType.Emf;

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

    }

    [Test]
    public void TestRun_LogCreated_OutputNotCreated_TestCaseFail()
    {
      muDir = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\mudir\CEM\ucm2ad_CEM_1");
      mutant = new MuAtlMutant
      {
        Path = Path.Combine(muDir, "ucm2ad.atl"),
        Name = "ucm2ad_CEM_1",
        Status = MutantStatus.Undetermined,
        Type = MutantType.CEM
      };

      var result = runner.Run(mutant, testCase);
      AssertResult(result);
      Assert.AreEqual("Output model not produced", result.Comment);
      Assert.IsFalse(File.Exists(result.Output.Path));
      Assert.IsTrue(File.Exists(result.Log));
      Assert.AreEqual(MuAtlVerdict.Fail, result.Verdict);
    }

    [Test]
    public void TestRun_LogCreated_OutputNotCreated_UnkownOutcome()
    {
      muDir = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\mudir\M2L\ucm2ad_M2L_1");
      mutant = new MuAtlMutant
      {
        Path = Path.Combine(muDir, "ucm2ad.atl"),
        Name = "ucm2ad_M2L_1",
        Status = MutantStatus.Undetermined,
        Type = MutantType.M2L
      };

      var result = runner.Run(mutant, testCase);
      AssertResult(result);
      Assert.AreEqual("Compare expected and actual output", result.Comment);
      Assert.IsTrue(File.Exists(result.Output.Path));
      Assert.IsTrue(File.Exists(result.Log));
      Assert.IsNull(result.Verdict);
    }

    [Test]
    public void TestRun_ConfigInvalid_LogCreated_OutputNotCreated_ErrorOutcome()
    {
      runner.Config.InMetamodels[0].Dependencies.Clear(); //error in config

      muDir = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\mudir\M2L\ucm2ad_M2L_1");
      mutant = new MuAtlMutant
      {
        Path = Path.Combine(muDir, "ucm2ad.atl"),
        Name = "ucm2ad_M2L_1",
        Status = MutantStatus.Undetermined,
        Type = MutantType.M2L
      };

      var result = runner.Run(mutant, testCase);
      AssertResult(result);
      Assert.AreEqual("Unknown error occured", result.Comment);
      Assert.IsTrue(File.Exists(result.Log));
      Assert.IsFalse(File.Exists(result.Output.Path));
      Assert.AreEqual(MuAtlVerdict.Error, result.Verdict);    
    }

    [TearDown]
    public void Cleanup()
    {
      var outDir = Path.Combine(TestContext.CurrentContext.TestDirectory, muDir, testCase.Name);
      if (Directory.Exists(outDir))
      {
        Directory.Delete(outDir, true);
      }
      File.Delete(Path.Combine(muDir, "ucm2ad.asm"));
    }

    private void AssertResult(MuAtlResult result)
    {
      Assert.AreEqual(testCase, result.TestCase);
      Assert.AreEqual(mutant, result.Mutant);
      Assert.AreEqual(Path.Combine(muDir, testCase.Name, mutant.Name + "_" + testCase.Name + ".txt"), result.Log);
      var expectedLogPath = Path.Combine(muDir, testCase.Name, mutant.Name + "_" + testCase.Name + ".txt");
      Assert.AreEqual(expectedLogPath, result.Log);
    }
  }
}
