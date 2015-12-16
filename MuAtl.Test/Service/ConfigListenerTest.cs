using NUnit.Framework;
using MuAtl.Service;
using System.IO;
using MuAtl.Service.Reader;
using Antlr4.Runtime.Tree;

namespace MuAtl.Test.Service
{
  [TestFixture]
  public class ConfigListenerTest
  {
    private ConfigListener mSrvc;
    private ParseTreeWalker mWalker;
    private IParseTree mTree;
    private readonly string Module1 = Path.GetFullPath(@"TestData\ucm2ad.atl");
    private readonly string Module2 = Path.GetFullPath(@"TestData\ConcreteToAbstract.atl");

    [Test]
    public void TestWalk_ConfigRead()
    {
      var tokens = new AtlTokenizer().GetTokens(Module1);
      mTree = new AtlParser().Parse(tokens);
      mSrvc = new ConfigListener(tokens);
      mWalker = new ParseTreeWalker();

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual("PathNode", mSrvc.LibNames[0]);
      Assert.AreEqual("Component", mSrvc.LibNames[1]);
      Assert.AreEqual("Stub", mSrvc.LibNames[2]);
      Assert.AreEqual("UCMmap", mSrvc.LibNames[3]);
      Assert.AreEqual("ComponentRef", mSrvc.LibNames[4]);
      Assert.AreEqual("NodeConnection", mSrvc.LibNames[5]);
      Assert.AreEqual("uml", mSrvc.OutModelName);
      Assert.AreEqual("UML", mSrvc.OutMetamodelName);
      Assert.AreEqual("map", mSrvc.InModels[0].Key);
      Assert.AreEqual("UCM", mSrvc.InModels[0].Value);

      tokens = new AtlTokenizer().GetTokens(Module2);
      mTree = new AtlParser().Parse(tokens);
      mSrvc = new ConfigListener(tokens);
      mWalker = new ParseTreeWalker();

      mWalker.Walk(mSrvc, mTree);
      Assert.AreEqual("UseCase", mSrvc.LibNames[0]);
      Assert.AreEqual("Association", mSrvc.LibNames[1]);
      Assert.AreEqual("Actor", mSrvc.LibNames[2]);
      Assert.AreEqual("OUT", mSrvc.OutModelName);
      Assert.AreEqual("UML", mSrvc.OutMetamodelName);
      Assert.AreEqual("IN", mSrvc.InModels[0].Key);
      Assert.AreEqual("UML", mSrvc.InModels[0].Value);
    }
  }
}
