using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using GalaSoft.MvvmLight.Threading;
using log4net;
using MuAtl.Model;
using MuAtl.Service.Reader.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MuAtl.Service.Generator
{
  public class MutantGenerationListener : ATLBaseListener
  {
    #region constants

    private const string RefiningMode = "refining";
    private const string TransformationMode = "from";

    #endregion

    private static readonly ILog logger = LogManager.GetLogger(typeof(MutantGenerationListener));

    #region props

    public IEnumerable<M2lCandidate> M2lCandidates { get; set; }
    public IEnumerable<L2mCandidate> L2mCandidates { get; set; }
    public IEnumerable<DrsCandidate> DrsCandidates { get; set; }
    public IEnumerable<DusCandidate> DusCandidates { get; set; }
    public IEnumerable<DamCandidate> DamCandidates { get; set; }
    public IEnumerable<DfeCandidate> DfeCandidates { get; set; }
    public IEnumerable<CstCandidate> CstCandidates { get; set; }
    public IEnumerable<CttCandidate> CttCandidates { get; set; }
    public IEnumerable<AamCandidate> AamCandidates { get; set; }
    public IEnumerable<AfeCandidate> AfeCandidates { get; set; }

    public bool IsCemCandidate { get; set; }

    #endregion

    #region instance vars

    private ITokenStream mTokens;
    private string mMutantDir;
    private string mModule;
    private Dictionary<MutantType, int> mMutantTypes;
    private MuAtlProject mProject;

    #endregion;

    public MutantGenerationListener(ITokenStream tokens, string mutantDir, MuAtlProject project, Dictionary<MutantType, int> mutantTypes)
    {
      mTokens = tokens;
      mMutantDir = mutantDir;
      mModule = Path.GetFileName(project.Module);
      mProject = project;

      mMutantTypes = mutantTypes;
    }

    #region overrides

    public override void EnterMatchedRule_abstractContents(ATLParser.MatchedRule_abstractContentsContext context)
    {
      var rule = context.IDENTIFIER(0).Symbol.Text;
      if (!M2lCandidates.Any(c => c.Rule == rule)) return;

      try
      {
        var rewriter = GetWriter();
        rewriter.InsertBefore(context.start, "lazy ");
        SaveMutant(MutantType.M2L, rewriter.GetText());
      }
      catch (Exception ex)
      {
        logger.ErrorFormat("Exception '{0}' occured while generating M2L mutant for matched rule '{1}'", ex.Message, rule);
      }
    }

    public override void EnterLazyMatchedRule(ATLParser.LazyMatchedRuleContext context)
    {
      var rule = context.IDENTIFIER(0).Symbol.Text;
      if (!L2mCandidates.Any(c => c.Rule == rule)) return;

      try
      {
        var rewriter = GetWriter();
        rewriter.Delete(context.Start);
        SaveMutant(MutantType.L2M, rewriter.GetText());
      }
      catch (Exception ex)
      {
        logger.ErrorFormat("Exception '{0}' occured while generating L2M mutant for lazy rule '{1}'", ex.Message, rule);
      }
    }

    public override void EnterExpressionStat(ATLParser.ExpressionStatContext context)
    {
      var stmnt = context.GetText();
      if (!stmnt.IsReturnStatement())
        return;

      var rule = context.GetParentRuleName();
      if (string.IsNullOrEmpty(rule))
      {
        logger.ErrorFormat("Failed to create DRS mutant for return statement '{0}'. Cannot find corresponding rule", stmnt);
        return;
      }

      if (!DrsCandidates.Any(c => c.Rule == rule && c.Statement == stmnt))
        return;

      try
      {
        var rewriter = GetWriter();
        rewriter.Delete(context.Start, context.Stop);
        SaveMutant(MutantType.DRS, rewriter.GetText());
      }
      catch (Exception ex)
      {
        logger.ErrorFormat("Exception '{0}' occured while generating DRS mutant for return statement '{1}' of rule '{2}'",
          ex.Message, stmnt, rule);
      }
    }

    public override void EnterInPattern(ATLParser.InPatternContext context)
    {
      var rule = context.GetParentRuleName();
      if (string.IsNullOrEmpty(rule))
      {
        logger.ErrorFormat("Failed to generate mutants for in pattern '{0}'. Cannot find corresponding rule", context.GetText());
        return;
      }

      var filtering = context.oclExpression();

      #region DFE

      if (filtering != null && (DfeCandidates.Any(c => c.Rule == rule && c.Filtering == filtering.GetText())))
      {
        try
        {
          var fExpIndex = context.children.IndexOf(filtering);
          var startParanthesis = (Antlr4.Runtime.Tree.ITerminalNode)context.children.ElementAt(fExpIndex - 1);
          var endParanthesis = (Antlr4.Runtime.Tree.ITerminalNode)context.children.ElementAt(fExpIndex + 1);

          var rewriter = new TokenStreamRewriter(mTokens);
          rewriter.Delete(startParanthesis.Symbol);
          rewriter.Delete(filtering.Start, filtering.Stop);
          rewriter.Delete(endParanthesis.Symbol);

          SaveMutant(MutantType.DFE, rewriter.GetText());
        }
        catch (Exception ex)
        {
          logger.ErrorFormat("Exception '{0}' occured while generating DFE mutant for rule '{1}'", ex.Message, rule);
        }
      }

      #endregion
    }

    public override void EnterInPatternElement(ATLParser.InPatternElementContext context)
    {
      var rule = context.GetParentRuleName();
      if (string.IsNullOrEmpty(rule))
      {
        logger.ErrorFormat("Failed to generate AFE mutants for in pattern '{0}'. Cannot find corresponding rule", context.GetText());
        return;
      }

      var parntCntxt = context.Parent as ATLParser.InPatternContext;
      if(parntCntxt == null)
      {
        logger.ErrorFormat("Failed to generate AFE mutants for in pattern '{0}'. Cannot find InPatternContext", context.GetText());
        return;
      }

      var filtering = parntCntxt.oclExpression();
      var candidate = AfeCandidates.SingleOrDefault(c => c.Rule == rule);
      if (filtering != null || candidate == null)
        return;

      foreach (var filter in candidate.MuFilteringExpressions)
      {
        try
        {
          var rewriter = GetWriter();
          rewriter.InsertAfter(context.Stop, string.Format(" ( {0} )", filter));
          SaveMutant(MutantType.AFE, rewriter.GetText());
        }
        catch (Exception ex)
        {
          logger.ErrorFormat("Exception '{0}' occured while generating AFE mutant for rule '{1}', in pattern '{2}', filtering expression '{3}'", ex.Message, rule, context.GetText(), filter);
        }
      }
    }

    public override void EnterLibraryRef(ATLParser.LibraryRefContext context)
    {
      var library = context.children[1].GetText();
      var libUsingStmnt = context.GetText();

      if (!DusCandidates.Any(c => c.Library == library))
        return;

      try
      {
        var rewriter = GetWriter();
        rewriter.Delete(context.Start, context.Stop);
        SaveMutant(MutantType.DUS, rewriter.GetText());
      }
      catch (Exception ex)
      {
        logger.ErrorFormat("Exception '{0}' occured while generating DUS mutant for using statement '{1}'",
          ex.Message, libUsingStmnt);
      }
    }

    public override void EnterTransformationMode(ATLParser.TransformationModeContext context)
    {
      if (!IsCemCandidate)
        return;

      var module = (context.Parent as ATLParser.ModuleContext).IDENTIFIER();
      try
      {
        var rewriter = GetWriter();
        var mode = context.GetText();

        if (mode == RefiningMode)
        {
          rewriter.Replace(context.Start, TransformationMode);
        }
        else if (mode == TransformationMode)
        {
          rewriter.Replace(context.Start, RefiningMode);
        }

        SaveMutant(MutantType.CEM, rewriter.GetText());
      }
      catch (Exception ex)
      {
        logger.ErrorFormat("Exception '{0}' occured while generating CEM mutant for module '{1}'", ex.Message, module);
      }
    }

    public override void EnterOutPatternElement(ATLParser.OutPatternElementContext context)
    {
      var outPattern = context.GetText();
      var rule = context.GetParentRuleName();

      if (string.IsNullOrEmpty(rule))
      {
        logger.ErrorFormat("Cannot find corresponding rule of out pattern '{0}'", outPattern);
        return;
      }

      var candidate = AamCandidates.SingleOrDefault(c => c.Rule == rule && outPattern.Contains(c.OutPattern));
      if (candidate == null) return;

      var simpleOutPattern = context.simpleOutPatternElement();
      var toBindings = simpleOutPattern.binding();

      foreach (var mapping in candidate.MuMappings)
      {
        try
        {
          var rewriter = GetWriter();
          if (!toBindings.Any())
          {
            rewriter.InsertAfter(simpleOutPattern.Stop, string.Format(" ( {0} )", mapping));
          }
          else
          {
            var lastBinding = toBindings.Last();
            rewriter.InsertAfter(lastBinding.Stop, string.Format(", {0}", mapping));
          }
          SaveMutant(MutantType.AAM, rewriter.GetText());
        }
        catch (Exception ex)
        {
          logger.ErrorFormat("Exception '{0}' occured while generating AAM mutant for rule '{1}', out-pattern '{2}, attribute mapping '{3}'", ex.Message, rule, outPattern, mapping);
        }
      }
    }

    public override void EnterSimpleInPatternElement(ATLParser.SimpleInPatternElementContext context)
    {
      var rule = context.GetParentRuleName();
      var inPattern = context.GetText();

      if (string.IsNullOrEmpty(rule))
      {
        logger.ErrorFormat("Cannot find corresponding rule of in pattern '{0}'", inPattern);
        return;
      }

      var srcType = context.oclType();
      var candidate = CstCandidates.SingleOrDefault(c => c.Rule == rule && c.SourceType == srcType.GetText());
      if (candidate == null)
        return;

      foreach (var mutantType in candidate.MuSrcTypes)
      {
        try
        {
          var rewriter = GetWriter();
          rewriter.Replace(srcType.Start, srcType.Stop, mutantType);
          SaveMutant(MutantType.CST, rewriter.GetText());
        }
        catch (Exception ex)
        {
          logger.ErrorFormat("Exception '{0}' occured while generating CST mutant for rule '{1}', in pattern '{2}', mutant source type '{3}'", ex.Message, rule, inPattern, mutantType);
        }
      }
    }

    public override void EnterSimpleOutPatternElement(ATLParser.SimpleOutPatternElementContext context)
    {
      var rule = context.GetParentRuleName();
      if (string.IsNullOrEmpty(rule))
      {
        logger.ErrorFormat("Cannot find corresponding rule of out pattern '{0}'", context.GetText());
        return;
      }

      #region DAM

      var candidates = DamCandidates.Where(c => c.Rule == rule);
      if (candidates.Any())
      {
        var bindings = context.binding();
        int index = 0;
        foreach (var binding in bindings)
        {
          var bindingStr = binding.GetText();
          if (!candidates.Any(c => c.Mapping == bindingStr))
            continue;

          try
          {
            var rewriter = GetWriter();
            if (bindings.Count > 1)
            {
              var bindingIndex = context.children.IndexOf(binding);
              ITerminalNode comma;
              if (++index == bindings.Count) //last binding, so delete previous comma
              {
                comma = (ITerminalNode)context.children.ElementAt(bindingIndex - 1);
              }
              else //delete the next comma
              {
                comma = (ITerminalNode)context.children.ElementAt(bindingIndex + 1);
              }

              rewriter.Delete(binding.Start, binding.Stop);
              rewriter.Delete(comma.Symbol);
            }
            else //only one binding, no commas
            {
              rewriter.Delete(binding.Start, binding.Stop);
            }

            SaveMutant(MutantType.DAM, rewriter.GetText());
          }
          catch (Exception ex)
          {
            logger.ErrorFormat("Exception '{0}' occured while generating DAM mutant for rule '{1}', binding expression '{2}'",
              ex.Message, rule, bindingStr);
          }
        }
      }

      #endregion

      #region CTT

      var trgtType = context.oclType();
      var trgtTypeStr = trgtType.GetText();

      var candidate = CttCandidates.SingleOrDefault(c => c.Rule == rule && c.TargetType == trgtTypeStr);
      if (candidate == null)
        return;

      foreach (var muType in candidate.MuTargetTypes)
      {
        try
        {
          var rewriter = GetWriter();
          rewriter.Replace(trgtType.Start, trgtType.Stop, muType);
          SaveMutant(MutantType.CTT, rewriter.GetText());
        }
        catch (Exception ex)
        {
          logger.ErrorFormat("Exception '{0}' occured while generating CTT mutant for rule '{1}', target type '{2}', mutant type '{3}'", ex.Message, rule, trgtTypeStr, muType);
        }
      }

      #endregion
    }

    #endregion

    private TokenStreamRewriter GetWriter()
    {
      return new TokenStreamRewriter(mTokens);
    }

    private void SaveMutant(MutantType mutantType, string mutantContent)
    {
      var muTypeDir = Path.Combine(mMutantDir, mutantType.ToString());
      if (!Directory.Exists(muTypeDir))
      {
        Directory.CreateDirectory(muTypeDir);
      }

      int mutantIndex;
      if (!mMutantTypes.TryGetValue(mutantType, out mutantIndex))
      {
        logger.ErrorFormat("Error: Mutant type '{0}' cannot be saved", mutantType);
        return;
      }

      var muName = string.Format("{0}_{1}_{2}", Path.GetFileNameWithoutExtension(mModule), mutantType, mutantIndex);
      var muDir = Path.Combine(muTypeDir, muName);
      if (!Directory.Exists(muDir))
      {
        Directory.CreateDirectory(muDir);
      }

      var muPath = Path.Combine(muDir, mModule);
      using (var writer = new StreamWriter(muPath))
      {
        writer.Write(mutantContent);
      }

      DispatcherHelper.UIDispatcher.Invoke(() =>
      {
        mProject.Mutants.Add(new MuAtlMutant
        {
          Name = muName,
          Type = mutantType,
          Path = muPath
        });
      });

      mMutantTypes[mutantType] = mutantIndex + 1;
      logger.InfoFormat("mutant {0} of type {1} saved at  {2}", muName, mutantType, muPath);
    }
  }
}
