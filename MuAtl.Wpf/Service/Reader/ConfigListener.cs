using Antlr4.Runtime;
using System.Collections.Generic;

namespace MuAtl.Service.Reader
{
  public class ConfigListener : ATLBaseListener
  {
    private ITokenStream mTokens;

    public List<string> LibNames { get; private set; }
    public string OutModelName { get; private set; }
    public string OutMetamodelName { get; private set; }

    public List<KeyValuePair<string, string>> InModels { get; private set; }

    public ConfigListener(ITokenStream tokens)
    {
      mTokens = tokens;
      LibNames = new List<string>();
      InModels = new List<KeyValuePair<string, string>>();
    }

    public override void EnterLibraryRef(ATLParser.LibraryRefContext context)
    {
      var token = mTokens.Get(context.SourceInterval.a);
      LibNames.Add(context.children[1].GetText());
    }

    public override void EnterTargetModelPattern(ATLParser.TargetModelPatternContext context)
    {
      var oclContext = context.oclModel(0);
      OutModelName = oclContext.IDENTIFIER(0).GetText();
      OutMetamodelName = oclContext.IDENTIFIER(1).GetText();
    }

    public override void EnterSourceModelPattern(ATLParser.SourceModelPatternContext context)
    {
      foreach(var oclContext in context.oclModel())
      {
        InModels.Add(new KeyValuePair<string, string>(
          oclContext.IDENTIFIER(0).GetText(), 
          oclContext.IDENTIFIER(1).GetText()));
      }
    }
  }
}
