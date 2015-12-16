using System.Diagnostics;
using System.IO;
using System.Text;

namespace MuAtl.Service
{
  public interface IOracle
  {
    void Compare(string expected, string actual);
  }

  public class Oracle : IOracle
  {
    private string mEclipseDir;

    public Oracle()
    {
      mEclipseDir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "eclipse");
    }
    
    public void Compare(string expected, string actual)
    {
      if(!File.Exists(expected) || !File.Exists(actual))
      {
        return;
      }

      var cmdBuilder = new StringBuilder();
      cmdBuilder.Append("eclipse -application com.example.headless.application -consoleLog ");
      cmdBuilder.AppendFormat("\"{0}\" ", expected);
      cmdBuilder.AppendFormat("\"{0}\"", actual);

      var processInfo = new ProcessStartInfo
      {
        FileName = "cmd.exe",
        Arguments = "/c " + cmdBuilder.ToString(),
        WindowStyle = ProcessWindowStyle.Hidden,
        WorkingDirectory = mEclipseDir
      };

      using (Process.Start(processInfo))
      {
      }
    }
  }
}
