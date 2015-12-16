using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;

namespace MuAtl.Service
{
  public static class RuleContextExtension
  {
    public static RuleContext GetParentRule(this RuleContext context)
    {
      Type parentType = context.Parent.GetType();
      if (parentType == typeof(ATLParser.ModuleContext))
      {
        return null; //error, reached top of parse tree
      }

      if (parentType == typeof(ATLParser.CalledRuleContext) ||
          parentType == typeof(ATLParser.LazyMatchedRuleContext) ||
          parentType == typeof(ATLParser.MatchedRule_abstractContentsContext))
      {
        return context.Parent;
      }

      return GetParentRule(context.Parent);
    }

    public static string GetParentRuleName(this RuleContext context)
    {
      var ruleContext = context.GetParentRule();
      if (ruleContext == null)
      {
        return string.Empty;
      }

      var ruleType = ruleContext.GetType();
      ITerminalNode rule = null;

      if (ruleType == typeof(ATLParser.CalledRuleContext))
      {
        rule = (ruleContext as ATLParser.CalledRuleContext).IDENTIFIER();
      }
      else if (ruleType == typeof(ATLParser.LazyMatchedRuleContext))
      {
        rule = (ruleContext as ATLParser.LazyMatchedRuleContext).IDENTIFIER(0);
      }
      else if (ruleType == typeof(ATLParser.MatchedRule_abstractContentsContext))
      {
        rule = (ruleContext as ATLParser.MatchedRule_abstractContentsContext).IDENTIFIER(0);
      }

      return rule.Symbol.Text;
    }
  }
}
