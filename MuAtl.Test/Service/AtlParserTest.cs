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
  public class AtlParserTest
  {
    private AtlParser mSrvc;
    private AtlTokenizer mTokenizer;
    private readonly string ModulePath = Path.GetFullPath(@"TestData\ucm2ad.atl");

    [SetUp]
    public void Inint()
    {
      mSrvc = new AtlParser();
      mTokenizer = new AtlTokenizer();
    }

    [Test]
    public void TestParse_ModuleIsValidAtlFile_ReturnsTree()
    {
      var tokens = mTokenizer.GetTokens(ModulePath);
      Assert.IsNotNull(mSrvc.Parse(tokens));
    }
  }
}
