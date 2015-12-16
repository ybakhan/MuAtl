using Antlr4.Runtime;
using log4net;
using MuAtl.Service.Reader.Model;
using System.Collections.Generic;

namespace MuAtl.Service.Reader
{
  public class MuCandidateListener : ATLBaseListener
  {
    private ITokenStream mTokens;
    private static readonly ILog logger = LogManager.GetLogger(typeof(MuCandidateListener));

    #region Props

    public IList<MutationCandidate> MutationCandidates { get; private set; }
    public ISet<string> SrcTypes { get; private set; }
    public ISet<string> TargetTypes { get; private set; }

    #endregion

    public MuCandidateListener(ITokenStream tokens)
    {
      mTokens = tokens;    
      SrcTypes = new HashSet<string>();
      TargetTypes = new HashSet<string>();
      MutationCandidates = new List<MutationCandidate>();
    }

    public override void EnterMatchedRule_abstractContents(ATLParser.MatchedRule_abstractContentsContext context)
    {
      MutationCandidates.Add(new M2lCandidate
      {
        Rule = context.IDENTIFIER(0).Symbol.Text,
        Line = GetLine(context)
      });
    }

    public override void EnterLazyMatchedRule(ATLParser.LazyMatchedRuleContext context)
    {
      MutationCandidates.Add(new L2mCandidate
      {
        Rule = context.IDENTIFIER(0).Symbol.Text,
        Line = GetLine(context)
      });
    }

    public override void EnterLibraryRef(ATLParser.LibraryRefContext context)
    {
      var token = mTokens.Get(context.SourceInterval.a);
      MutationCandidates.Add(new DusCandidate
      {
        Library = context.children[1].GetText(),
        Line = token.Line
      });
    }

    public override void EnterExpressionStat(ATLParser.ExpressionStatContext context)
    {
      var statmnt = context.GetText();
      if (!statmnt.IsReturnStatement())
        return;

      var rule = context.GetParentRuleName();
      if (rule == null)
      {
        logger.ErrorFormat("Failed to find rule of statement '{0}'", context.GetText());
        return;
      }

      var token = mTokens.Get(context.SourceInterval.a);
      MutationCandidates.Add(new DrsCandidate
      {
        Rule = rule,
        Statement = context.GetText(),
        Line = GetLine(context)
      });
    }

    public override void EnterSimpleInPatternElement(ATLParser.SimpleInPatternElementContext context)
    {
      var rule = context.GetParentRuleName();
      if (rule == null)
      {
        logger.ErrorFormat("Failed to find corresponding rule of in-pattern '{0}'", context.GetText());
        return;
      }
      
      var srcType = context.oclType().GetText();
      MutationCandidates.Add(new CstCandidate
      {
        Rule = rule,
        SourceType = srcType,
        Line = GetLine(context)
      });

      SrcTypes.Add(srcType);
    }

    public override void EnterSimpleOutPatternElement(ATLParser.SimpleOutPatternElementContext context)
    {
      var rule = context.GetParentRuleName();
      if (rule == null)
      {
        logger.ErrorFormat("Failed to find corresponding rule of out-pattern '{0}'", context.GetText());
        return;
      }
      
      var trgtType = context.oclType().GetText();
      var line = GetLine(context);
      MutationCandidates.Add(new CttCandidate
      {
        Line = line,
        TargetType = trgtType,
        Rule = rule
      });

      TargetTypes.Add(trgtType);

      MutationCandidates.Add(new AamCandidate
      {
        Rule = rule,
        Line = line,
        OutPattern = trgtType
      });
    }

    public override void EnterBinding(ATLParser.BindingContext context)
    {
      var rule = context.GetParentRuleName();
      if (rule == null)
      {
        logger.ErrorFormat("Failed to find corresponding rule of binding '{0}'", context.GetText());
        return;
      }

      MutationCandidates.Add(new DamCandidate
      {
        Line = GetLine(context),
        Mapping = context.GetText(),
        Rule = rule
      });
    }

    public override void EnterInPattern(ATLParser.InPatternContext context)
    {
      var rule = context.GetParentRuleName();
      if (rule == null)
      {
        logger.ErrorFormat("Failed to find corresponding rule of in-pattern '{0}'", context.GetText());
        return;
      }

      var filtering = context.oclExpression();
      if (filtering == null)
      {
        return; //rule does not have filtering
      }

      MutationCandidates.Add(new DfeCandidate
      {
        Rule = rule,
        Filtering = filtering.GetText(),
        Line = GetLine(filtering)
      });
    }

    public override void EnterInPatternElement(ATLParser.InPatternElementContext context)
    {
      var rule = context.GetParentRuleName();
      if (string.IsNullOrEmpty(rule))
      {
        logger.ErrorFormat("Cannot find corresponding rule of in-pattern '{0}'", context.GetText());
        return;
      }

      var parntCntxt = context.Parent as ATLParser.InPatternContext;
      if (parntCntxt == null)
      {
        logger.ErrorFormat("Cannot find corresponding context of in-pattern '{0}'", context.GetText());
        return;
      }

      var filtering = parntCntxt.oclExpression();
      if (filtering != null)
        return;

      MutationCandidates.Add(new AfeCandidate
      {
        Line = GetLine(context),
        Rule = rule
      });
    }

    private int GetLine(RuleContext context)
    {
      return mTokens.Get(context.SourceInterval.a).Line;
    }
  }
}
