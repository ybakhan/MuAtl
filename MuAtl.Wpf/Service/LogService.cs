using System.Diagnostics;

namespace MuAtl.Service
{
  public interface ILogService
  {
    void Open(string path);
  }

  public class LogService : ILogService
  {
    public void Open(string path)
    {
      if (!System.IO.File.Exists(path))
        return;

      using (Process.Start(path))
      {
      }
    }
  }
}
