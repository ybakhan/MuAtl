using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using log4net;
using System;

namespace MuAtl.Service
{
  public interface IAtlParser
  {
    IParseTree Parse(ITokenStream tokens);
  }

  public class AtlParser : IAtlParser
  {
    private static readonly ILog logger = LogManager.GetLogger(typeof(AtlParser));

    public IParseTree Parse(ITokenStream tokens)
    {
      try
      {
        var parser = new ATLParser(tokens);
        if (parser.NumberOfSyntaxErrors > 0)
        {
          return null;
        }

        var tree = parser.unit();
        return tree;
      }
      catch (Exception ex)
      {
        logger.ErrorFormat("Exception '{0}' occured while parsing ATL file", ex.Message);
        return null;
      }
    }
  }
}
