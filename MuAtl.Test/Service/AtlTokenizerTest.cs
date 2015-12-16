using NUnit.Framework;
using MuAtl.Service;
using System.IO;

namespace MuAtl.Test.Service
{
  [TestFixture]
  public class AtlTokenizerTest
  {
    private AtlTokenizer mSrvc;
    private readonly string ModulePath = Path.GetFullPath(@"TestData\ucm2ad.atl");

    [SetUp]
    public void Inint()
    {
      mSrvc = new AtlTokenizer();
    }

    [Test]
    public void TestGetTokens_PathValid_TokensNotNull()
    {
      Assert.IsNotNull(mSrvc.GetTokens(ModulePath));
    }

    [Test]
    public void TestGetTokens_PathInvalid_ReturnNull()
    {
      Assert.IsNull(mSrvc.GetTokens("X:/../../ucm2ad.atl"));
    }
  }
}
