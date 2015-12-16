namespace MuAtl.Model.MuAtlJavaArgs
{
  public enum AtlVmType
  {
    Emf, Asm
  }

  class VmArg : MuAtlJavaArg
  {
    private const string ArgStr = "-vm";

    public VmArg()
    {
      Arg = ArgStr;
      ArgValue = AtlVmType.Emf.ToString().ToUpper();
    }

    public VmArg(AtlVmType vmType)
    {
      Arg = ArgStr;
      ArgValue = vmType.ToString().ToUpper();
    }
  }
}
