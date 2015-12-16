using System.Diagnostics;

namespace MuAtl.Service
{
  public interface IDiffer
  {
    void Diff(string leftFile, string rightFile);
  }

  public class Differ : IDiffer
  {
    public void Diff(string leftFile, string rightFile)
    {
      if (!System.IO.File.Exists(leftFile) || !System.IO.File.Exists(rightFile))
        return;

      var pr = new Process();
      pr.StartInfo.FileName = "WinMergeU.exe";
      pr.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\" /wl /maximize", leftFile, rightFile);
      pr.Start();
    }
  }
}
