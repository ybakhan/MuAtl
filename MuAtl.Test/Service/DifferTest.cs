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
  public class DifferTest
  {
    private Differ mSrvc;
    private readonly string Original = Path.GetFullPath(@"TestData\ucm2ad.atl");
    private readonly string Mutant = Path.GetFullPath(@"TestData\ucm2ad_m2l1.atl");
    private const string ProcName = "WinMergeU";

    [SetUp]
    public void Inint()
    {
      mSrvc = new Differ();
    }

    [Test]
    public void TestOpen_PathsValid_DiffOpened()
    {
      mSrvc.Diff(Original, Mutant);
      Thread.Sleep(TimeSpan.FromSeconds(10));
      var process = Process.GetProcesses().SingleOrDefault(p => p.ProcessName == ProcName);
      Assert.IsNotNull(process);
      process.Kill();
    }

    [Test]
    public void TestOpen_OrgPathInvalid_DiffNotOpened()
    {
      mSrvc.Diff("X:/../../ucm2ad.atl", Mutant);
      var process = Process.GetProcesses().SingleOrDefault(p => p.ProcessName == ProcName);
      Assert.IsNull(process);
    }

    [Test]
    public void TestOpen_MutantPathInvalid_DiffNotOpened()
    {
      mSrvc.Diff(Original, "X:/../../ucm2ad.atl");
      var process = Process.GetProcesses().SingleOrDefault(p => p.ProcessName == ProcName);
      Assert.IsNull(process);
    }
  }
}
