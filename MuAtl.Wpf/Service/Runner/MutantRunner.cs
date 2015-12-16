using MuAtl.Model;
using MuAtl.Model.MuAtlJavaArgs;
using System.IO;
using System.Linq;
using log4net;

namespace MuAtl.Service.Runner
{
  public interface IMutantRunner
  {
    MuAtlResult Run(MuAtlMutant mutant, MuAtlTestCase testCase);
    AtlCompilerType Compiler { get; set; }
    AtlVmType Vm { get; set; }
    bool InterModelRefs { get; set; }
    bool Uml2Strtypes { get; set; }
    ProjectConfig Config { get; set; }
  }

  public class MutantRunner : IMutantRunner
  {
    private static readonly ILog logger = LogManager.GetLogger(typeof(MutantRunner));
    
    #region props

    public AtlCompilerType Compiler { get; set; }
    public AtlVmType Vm { get; set; }
    public bool InterModelRefs { get; set; }
    public bool Uml2Strtypes { get; set; }
    public ProjectConfig Config { get; set; }

    #endregion

    public MuAtlResult Run(MuAtlMutant mutant, MuAtlTestCase testCase)
    {
      var muTcDir = Path.Combine(Path.GetDirectoryName(mutant.Path), testCase.Name);
      if (!Directory.Exists(muTcDir))
      {
        Directory.CreateDirectory(muTcDir);
      }

      var logPath = Path.Combine(muTcDir, string.Format("{0}_{1}.txt", mutant.Name, testCase.Name));
      var batchPath = Path.Combine(muTcDir, string.Format("{0}_{1}.bat", mutant.Name, testCase.Name));

      var tcResult = new MuAtlResult
      {
        Mutant = mutant,
        TestCase = testCase,
        Log = logPath,
        Output = new MuAtlTestCaseOutput
        {
          Name = testCase.OutModel.Name,
          Path = Path.Combine(muTcDir, Path.GetFileName(testCase.OutModel.Path))
        }
      };

      var mJarService = new MuAtlJarRunner(
        Config,
        mutant.Path,
        Config.Libraries.Select(l => l.Path),
        Config.SuperImposedModules.Select(s => s.Path),
        testCase.InModels,
        tcResult.Output,
        Compiler,
        Vm,
        logPath,
        false,
        Uml2Strtypes,
        false,
        InterModelRefs);

      var result = mJarService.Run();
      switch (result)
      {
        case 0:

          tcResult.Comment = "Compare expected and actual output";

          logger.InfoFormat("test case {0} executed on mutant {1}. Compare expected and actual output",
            testCase.Name, mutant.Name);

          break;
        case -1:

          tcResult.Comment = "Output model not produced";
          tcResult.Verdict = MuAtlVerdict.Fail;

          logger.InfoFormat(string.Format(
            "test case {0} failed on mutant {1}",
            testCase.Name, mutant.Name));

          break;
        case -2:

          tcResult.Comment = "Unknown error occured";
          tcResult.Verdict = MuAtlVerdict.Error;

          logger.InfoFormat(string.Format("Unknown error occured while execuing test case {0} on mutant {1}",
            testCase.Name, mutant.Name));
          break;

        case -3:
          tcResult.Comment = "Error! MuAtl.Jar not found";
          tcResult.Verdict = MuAtlVerdict.Error;

          logger.InfoFormat(string.Format("MuAtl.jar not found while execuing test case {0} on mutant {1}",
            testCase.Name, mutant.Name));

          break;
      }

      return tcResult;
    }
  }
}
