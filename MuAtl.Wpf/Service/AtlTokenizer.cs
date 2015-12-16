using Antlr4.Runtime;
using System.IO;

namespace MuAtl.Service
{
  public class AtlTokenizer
  {
    public ITokenStream GetTokens(string atlFilePath)
    {
      if (string.IsNullOrEmpty(atlFilePath) || !File.Exists(atlFilePath))
        return null;

      var moduleStr = File.ReadAllText(atlFilePath);
      var stream = new AntlrInputStream(moduleStr);
      var lexer = new ATLLexer(stream);
      var tokens = new CommonTokenStream(lexer);
      return tokens;
    }
  }
}
