using System.Text.RegularExpressions;

namespace MuAtl.Service
{
  public static class ExpressionStatContextExtension
  {
    public static bool IsReturnStatement(this string statement)
    {
      return Regex.Match(statement, @"[\w]+;").Success;
    }
  }
}
