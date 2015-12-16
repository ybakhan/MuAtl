using MuAtl.Model;
using MuAtl.Model.MuAtlJavaArgs;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System;
using System.Diagnostics;

namespace MuAtl.Service.Runner
{
  public class MuAtlJarRunner
  {
    private static readonly string JarFile;

    private const string BatExt = ".bat";
    private List<MuAtlJavaArg> mMuAtlArgs = new List<MuAtlJavaArg>();

    static MuAtlJarRunner()
    {
      JarFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "muatl.jar");
    }

    public MuAtlJarRunner(
      ProjectConfig config,
      string module,
      IEnumerable<string> libs,
      IEnumerable<string> superImposedModules,
      IEnumerable<TestCaseInput> input,
      MuAtlTestCaseOutput outModel,
      AtlCompilerType compilerType,
      AtlVmType vmType,
      string logFile,
      bool stopInMain,
      bool supportUml2StereoTypes,
      bool contentType,
      bool intermodelReferences)
    {
      mMuAtlArgs.Add(new ModuleArg
      {
        ArgValue = string.Format("\"{0}\"", module)
      });

      #region lib

      if (libs.Any())
      {
        var libBuilder = new StringBuilder();
        libBuilder.Append("\"");
        foreach (var lib in libs)
        {
          libBuilder.Append(lib);
          libBuilder.Append(";");
        }
        libBuilder.Append("\"");

        var libArgsStr = libBuilder.ToString();
        var indexOfSemiColon = libArgsStr.LastIndexOf(';');
        if (indexOfSemiColon > -1)
          libArgsStr = libArgsStr.Remove(indexOfSemiColon, 1);

        mMuAtlArgs.Add(new LibArg
        {
          ArgValue = libArgsStr
        });
      }

      #endregion

      #region sup

      if (superImposedModules.Any())
      {
        var supModulesBuilder = new StringBuilder();
        supModulesBuilder.Append("\"");
        foreach (var supImposedModule in superImposedModules)
        {
          supModulesBuilder.Append(supImposedModule);
          supModulesBuilder.Append(";");
        }
        supModulesBuilder.Append("\"");

        var supArgsStr = supModulesBuilder.ToString();

        var indexOfSemiColon = supArgsStr.LastIndexOf(';');
        if (indexOfSemiColon > -1)
          supArgsStr = supArgsStr.Remove(indexOfSemiColon, 1);

        mMuAtlArgs.Add(new SuperimposedModuleArg
        {
          ArgValue = supArgsStr
        });
      }

      #endregion

      #region in model

      var handlersArgsBuiler = new StringBuilder();
      var inModelArgBuilder = new StringBuilder();

      foreach (var inModel in input)
      {
        inModelArgBuilder.Append(inModel.Name);
        inModelArgBuilder.Append(";");

        inModelArgBuilder.Append("\"");
        inModelArgBuilder.Append(inModel.Path);
        inModelArgBuilder.Append("\";");

        var model = config.InModels.SingleOrDefault(m => m.Name.Equals(inModel.Name));
        if (model == null)
          continue;

        var inMm = config.InMetamodels.SingleOrDefault(m => m.Name == model.Metamodels.Name);

        inModelArgBuilder.Append(inMm.Name);
        inModelArgBuilder.Append(";");

        inModelArgBuilder.Append("\"");
        inModelArgBuilder.Append(inMm.Path);
        inModelArgBuilder.Append("\";");

        if (!string.IsNullOrEmpty(inMm.Handler))
        {
          handlersArgsBuiler.Append(inMm.Name);
          handlersArgsBuiler.Append(";");
          handlersArgsBuiler.Append(inMm.Handler);
          handlersArgsBuiler.Append(";");
        }

        #region dep metamodels


        foreach (var metamodel in inMm.Dependencies)
        {
          inModelArgBuilder.Append(metamodel.Name);
          inModelArgBuilder.Append(";");

          inModelArgBuilder.Append("\"");
          inModelArgBuilder.Append(metamodel.Path);
          inModelArgBuilder.Append("\";");

          if (!string.IsNullOrEmpty(metamodel.Handler))
          {
            handlersArgsBuiler.Append(metamodel.Name);
            handlersArgsBuiler.Append(";");
            handlersArgsBuiler.Append(metamodel.Handler);
            handlersArgsBuiler.Append(";");
          }
        }
      }

      #endregion

      var inModelArgStr = inModelArgBuilder.ToString();
      inModelArgStr = inModelArgStr.Remove(inModelArgStr.LastIndexOf(';'), 1);

      mMuAtlArgs.Add(new InArg
      {
        ArgValue = inModelArgStr
      });

      #endregion

      #region out model

      var outArgBuilder = new StringBuilder();
      outArgBuilder.Append(outModel.Name);
      outArgBuilder.Append(";");

      outArgBuilder.Append("\"");
      outArgBuilder.Append(outModel.Path);
      outArgBuilder.Append("\";");

      outArgBuilder.Append(config.OutMetamodel.Name);
      outArgBuilder.Append(";");

      outArgBuilder.Append("\"");
      outArgBuilder.Append(config.OutMetamodel.Path);
      outArgBuilder.Append("\"");

      if (!string.IsNullOrEmpty(config.OutMetamodel.Handler))
      {
        handlersArgsBuiler.Append(config.OutMetamodel.Name);
        handlersArgsBuiler.Append(";");
        handlersArgsBuiler.Append(config.OutMetamodel.Handler);
      }

      mMuAtlArgs.Add(new OutArg
      {
        ArgValue = outArgBuilder.ToString()
      });

      #endregion

      if (!string.IsNullOrEmpty(handlersArgsBuiler.ToString()))
      {
        mMuAtlArgs.Add(new HandlerArg
        {
          ArgValue = handlersArgsBuiler.ToString()
        });
      }

      mMuAtlArgs.Add(new CompilerArg(compilerType));
      mMuAtlArgs.Add(new VmArg(vmType));

      mMuAtlArgs.Add(new LogFileArg
      {
        ArgValue = string.Format("\"{0}\"", logFile)
      });

      #region options

      mMuAtlArgs.Add(new StopInMainArg
      {
        ArgValue = Convert.ToInt32(stopInMain).ToString()
      });

      mMuAtlArgs.Add(new Uml2StereoTypesArg
      {
        ArgValue = Convert.ToInt32(supportUml2StereoTypes).ToString()
      });

      mMuAtlArgs.Add(new ContentTypeArg
      {
        ArgValue = Convert.ToInt32(contentType).ToString()
      });

      mMuAtlArgs.Add(new InterModelReferencesArg
      {
        ArgValue = Convert.ToInt32(intermodelReferences).ToString()
      });

      #endregion
    }

    public int Run()
    {
      if (!File.Exists(JarFile))
      {
        return -3;
      }

      var args = new StringBuilder();
      args.AppendFormat("java -jar \"{0}\" ", JarFile);
      foreach (var muAtlArg in mMuAtlArgs)
      {
        args.Append(muAtlArg.Arg);
        args.Append(" ");
        args.Append(muAtlArg.ArgValue);
        args.Append(" ");
      }

      var processInfo = new ProcessStartInfo
      {
        FileName = "cmd.exe",
        Arguments = "/c " + args.ToString(),
        WindowStyle = ProcessWindowStyle.Hidden
      };

      using (var process = Process.Start(processInfo))
      {
        process.WaitForExit();
        return process.ExitCode;
      }
    }
  }
}
