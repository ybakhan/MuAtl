using NUnit.Framework;
using MuAtl.Service;
using MuAtl.Model;
using System.Linq;
using System;
using System.IO;

namespace MuAtl.Test.Service
{
  [TestFixture]
  public class RepositoryTest
  {
    private Repository mSrvc;
    private const string TestProjPath = @"TestData\testproject.xml";

    [SetUp]
    public void Init()
    {
      mSrvc = new Repository();
    }

    #region Load

    [Test]
    public void TestLoad_PathValid_ProjectLoaded()
    {
      var project = mSrvc.Load<MuAtlProject>(@"TestData\ucm2ad_muatl.xml");
      Assert.AreEqual("ucm2ad_muatl", project.Name);
      Assert.AreEqual(@"TestData\ucm2ad.atl", project.Module);
      Assert.AreEqual("UCM", project.Dependency.InMetamodels[0].Name);
      Assert.AreEqual(@"TestData\ucm.ecore", project.Dependency.InMetamodels[0].Path);
      Assert.AreEqual("URN", project.Dependency.InMetamodels[0].Dependencies[0].Name);
      Assert.AreEqual(@"TestData\urn.ecore", project.Dependency.InMetamodels[0].Dependencies[0].Path);
      Assert.AreEqual("UML", project.Dependency.OutMetamodel.Name);
      Assert.AreEqual(@"TestData\UML.ecore", project.Dependency.OutMetamodel.Path);
      Assert.AreEqual("map", project.Dependency.InModels[0].Name);
      Assert.AreEqual("uml", project.Dependency.OutputModel.Name);
      Assert.AreEqual(6, project.Dependency.Libraries.Count());
      Assert.AreEqual("PathNode", project.Dependency.Libraries[0].Name);
      Assert.AreEqual(@"TestData\PathNode.atl", project.Dependency.Libraries[0].Path);
      Assert.AreEqual("Component", project.Dependency.Libraries[1].Name);
      Assert.AreEqual(@"TestData\Component.atl", project.Dependency.Libraries[1].Path);
      Assert.AreEqual("Stub", project.Dependency.Libraries[2].Name);
      Assert.AreEqual(@"TestData\Stub.atl", project.Dependency.Libraries[2].Path);
      Assert.AreEqual("UCMmap", project.Dependency.Libraries[3].Name);
      Assert.AreEqual(@"TestData\UCMmap.atl", project.Dependency.Libraries[3].Path);
      Assert.AreEqual("ComponentRef", project.Dependency.Libraries[4].Name);
      Assert.AreEqual(@"TestData\ComponentRef.atl", project.Dependency.Libraries[4].Path);
      Assert.AreEqual("NodeConnection", project.Dependency.Libraries[5].Name);
      Assert.AreEqual(@"TestData\NodeConnection.atl", project.Dependency.Libraries[5].Path);
      Assert.IsFalse(project.Dependency.SuperImposedModules.Any());
      Assert.AreEqual(13, project.TestSuite.Count());
      Assert.AreEqual(73, project.Mutants.Count());
      Assert.AreEqual(31, project.Results.Count());
    }

    [Test]
    public void TestLoad_PathInvalid_ThrowsException()
    {
      Assert.Throws(Is.InstanceOf<Exception>(), 
        () => mSrvc.Load<MuAtlProject>("X:/../../ucm2ad.atl"));
    }

    #endregion

    [Test]
    public void TestSave_DestinationValid_DoesNotThrowException()
    {
      var project = mSrvc.Load<MuAtlProject>(@"TestData\ucm2ad_muatl.xml");
      Assert.DoesNotThrow(() => mSrvc.Save(TestProjPath, project));
      Assert.IsTrue(File.Exists(TestProjPath));
    }

    [Test]
    public void TestSave_DestinationInvalid_ThrowsException()
    {
      var project = mSrvc.Load<MuAtlProject>(@"TestData\ucm2ad_muatl.xml");
      Assert.Throws(Is.InstanceOf<Exception>(),
        () => mSrvc.Save("X:/../../ucm2ad.atl", project));
    }

    [TearDown]
    public void Cleanup()
    {
      if (File.Exists(TestProjPath))
      {
        File.Delete(TestProjPath);
      }
    }
  }
}
