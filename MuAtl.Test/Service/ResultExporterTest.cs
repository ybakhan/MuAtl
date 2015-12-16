using NUnit.Framework;
using MuAtl.Service;
using MuAtl.Model;
using System.Collections.ObjectModel;

namespace MuAtl.Test.Service
{
  [TestFixture]
  public class ResultExporterTest
  {
    private MuAtlResult mResult;
    private readonly string Destination = System.IO.Path.Combine(
      TestContext.CurrentContext.TestDirectory, @"TestData\exportedResults.xlsx");

    [SetUp]
    public void Init()
    {
      mResult = new MuAtlResult
      {
        Comment = "Test Comment",
        Log = "testresultlog.txt",
        Mutant = new MuAtlMutant
        {
          Name = "Test Mutant",
          Path = "X:/../../testmutant.atl",
          Type = MutantType.AAM
        },
        Verdict = MuAtlVerdict.Pass,
        Name = "Test Result",
        Output = new MuAtlTestCaseOutput
        {
          Name = "Test output",
          Path = "testactualoutput.xmi"
        },
        TestCase = new MuAtlTestCase
        {
          Name = "Test test case",
          InModels = new ObservableCollection<TestCaseInput>(new[]
          {
            new TestCaseInput
            {
              Name = "Test input",
              Path ="testinput.xmi"
            }
          }),
          OutModel = new TestCaseOutput
          {
            Name = "Test expected ouput",
            Path = "testoutput.xmi"
          }
        }
      };
    }

    [Test]
    public void TestInit_DoesNotThrowException()
    {
      using (var mSrvc = new ResultExporter())
      {
        Assert.DoesNotThrow(() => mSrvc.Init());
      }
      
    }

    [Test]
    public void TestExport_DoesNotThrowException()
    {
      using (var mSrvc = new ResultExporter())
      {
        mSrvc.Init();
        Assert.DoesNotThrow(() => mSrvc.Export(mResult));
      }
    }

    [Test]
    public void TestSave_DoesNotThrowException()
    {
      using (var mSrvc = new ResultExporter())
      {
        mSrvc.Init();
        mSrvc.Export(mResult);
        Assert.DoesNotThrow(() => mSrvc.Save(Destination));
      }
    }

    [Test]
    public void TestSave_InvalidDestination_DoesNotThrowException()
    {
      using (var mSrvc = new ResultExporter())
      {
        mSrvc.Init();
        mSrvc.Export(mResult);
        Assert.DoesNotThrow(() => mSrvc.Save("X:/../../results.xlsx"));
      }
    }

    [TearDown]
    public void Cleanup()
    {
      if (System.IO.File.Exists(Destination))
      {
        System.IO.File.Delete(Destination);
      }
    }
  }
}
