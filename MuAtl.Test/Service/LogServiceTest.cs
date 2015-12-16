using System.Linq;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using MuAtl.Service;
using System.IO;
using System;

namespace MuAtl.Test.Service
{
  [TestFixture]
  public class LogServiceTest
  {
    private LogService mSrvc;

    [SetUp]
    public void Inint()
    {
      mSrvc = new LogService();
    }

    [Test]
    public void TestOpen_PathValid_LogOpened()
    {
      mSrvc.Open(@"TestData\testresultlog.txt");
      Thread.Sleep(TimeSpan.FromSeconds(10));
      var process = Process.GetProcesses().SingleOrDefault(p => p.ProcessName == "notepad");
      Assert.IsNotNull(process);
      process.Kill();
    }

    [Test]
    public void TestOpen_PathInvalidValid_LogNotOpened()
    {
      mSrvc.Open("X:/../../testresultlog.txt");
      var process = Process.GetProcesses().SingleOrDefault(p => p.ProcessName == "notepad");
      Assert.IsNull(process);
    }
  }
}
