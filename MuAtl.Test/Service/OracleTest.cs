using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using MuAtl.Service;
using System.IO;

namespace MuAtl.Test.Service
{

  [TestFixture]
  public class OracleTest
  {
    private Oracle mSrvc;
    private readonly string expected = Path.GetFullPath(@"TestData\expected.uml");
    private readonly string actual = Path.GetFullPath(@"TestData\actual.uml");

    [SetUp]
    public void Inint()
    {
      mSrvc = new Oracle();
    }

    [Test]
    public void TestCompare_ExpectedAndActualPathValid_CompareProcessStarted()
    {
      mSrvc.Compare(expected, expected);
      Thread.Sleep(TimeSpan.FromMinutes(1));

      var process = Process.GetProcesses().SingleOrDefault(p => p.ProcessName == "eclipse");
      Assert.IsNotNull(process);
      process.Kill();
      Thread.Sleep(TimeSpan.FromSeconds(30));
    }

    [Test]
    public void TestCompare_ExpectedPathInvalid_CompareProcessNotStarted()
    {
      Assert.DoesNotThrow(() => mSrvc.Compare("X:/../../expected.xmi", actual));
      var process = Process.GetProcesses().SingleOrDefault(p => p.ProcessName == "eclipse");
      Assert.IsNull(process);
    }

    [Test]
    public void TestCompare_ActualPathInvalid_CompareProcessNotStarted()
    {
      Assert.DoesNotThrow(() => mSrvc.Compare(expected, "X:/../../actual.xmi"));
      var process = Process.GetProcesses().SingleOrDefault(p => p.ProcessName == "eclipse");
      Assert.IsNull(process);
    }
  }
}
