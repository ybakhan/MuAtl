namespace MuAtl.Model.MuAtlJavaArgs
{
  public enum AtlCompilerType
  {
    Default, Atl2004, Atl2006, Atl2010, 
  }
  
  class CompilerArg : MuAtlJavaArg
  {
    private const string ArgStr = "-compiler";

    public CompilerArg()
    {
      Arg = ArgStr;
      ArgValue = AtlCompilerType.Default.ToString();
    }

    public CompilerArg(AtlCompilerType compilerType)
    {
      Arg = ArgStr;
      string compilerTypeStr = compilerType.ToString();
      
      var  indexOf2 = compilerTypeStr.IndexOf('2');
      if(indexOf2  == 3)
      {
        ArgValue = compilerTypeStr.Substring(indexOf2);
      }
      else
      {
        ArgValue = AtlCompilerType.Default.ToString();
      }
    }
  }
}
