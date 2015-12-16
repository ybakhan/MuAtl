using MuAtl.Model;
using MuAtl.ViewModel;
using NSubstitute;
using NUnit.Framework;
using System.Linq;

namespace MuAtl.Test.ViewModel
{
  [TestFixture]
  public class TestSuiteViewModelTest : MuAtlViewModelTestBase
  {
    private TestSuiteViewModel mViewModel;
    private MuAtlTestCase mTestCase;

    #region constants

    private const string TestCaseName = "SampleTestCase";
    private const string OutputName = "testoutput";
    private const string ExpectedPath = @"TestData\testoutput.xmi";
    private const string InputPath = @"TestData\testinput.xmi";
    private const string InputModelName = "input";

    private const string updatedTestCaseName = "UpdatedTestCaseName";
    private const string updatedInputPath = @"TestData\testinput2.xmi";
    private const string updatedOutputPath = @"TestData\testoutput2.xmi";
    private const string InvalidModelPath = "X:../../../doesnotexist.xmi"; 

    #endregion

    [SetUp]
    public override void Init()
    {
      base.Init();
      mViewModel = new TestSuiteViewModel(mDlgSrvc);
      mViewModel.Project = mProject;

      mTestCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath
        }
      };
      mTestCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
    }

    #region Add Test Case

    [Test]
    public void TestAddTestCase_FormValid_TestCaseDoesNotExist_TestCaseAdded_FormCleared()
    {
      mViewModel.TestCaseName = TestCaseName;
      mViewModel.ExpectedPath = ExpectedPath;
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });

      mViewModel.AddCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      var testCase = mViewModel.Project.TestSuite.SingleOrDefault(t => t.Name == TestCaseName);
      Assert.IsNotNull(testCase);
      Assert.AreEqual(ExpectedPath, testCase.OutModel.Path);
      Assert.AreEqual(1, testCase.InModels.Count());
      Assert.AreEqual(InputModelName, testCase.InModels[0].Name);
      Assert.AreEqual(InputPath, testCase.InModels[0].Path);

      Assert.AreEqual(string.Empty, mViewModel.TestCaseName);
      Assert.AreEqual(string.Empty, mViewModel.ExpectedPath);
      Assert.AreEqual(string.Empty, mViewModel.Input[0].Path);
    }

    [Test]
    public void TestAddTestCase_FormInvalid_TestCaseNotAdded_FormNotCleared()
    {
      mViewModel.TestCaseName = string.Empty;
      mViewModel.ExpectedPath = ExpectedPath;

      mViewModel.AddCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsNull(mViewModel.Project.TestSuite.SingleOrDefault(t => t.Name == TestCaseName));

      Assert.AreEqual(string.Empty, mViewModel.TestCaseName);
      Assert.AreEqual(ExpectedPath, mViewModel.ExpectedPath);
    }

    [Test]
    public void TestAddTestCase_FormValid_InputModelPathNotSelected_TestCaseNotAdded_FormNotCleared()
    {
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName
      });

      mViewModel.TestCaseName = TestCaseName;
      mViewModel.ExpectedPath = ExpectedPath;

      mViewModel.AddCmd.Execute(null);
      mDlgSrvc.Received().Error(TestSuiteViewModel.AddInputModelErrorMsg, TestSuiteViewModel.AddTestCaseCaption);
      Assert.IsNull(mViewModel.Project.TestSuite.SingleOrDefault(t => t.Name == TestCaseName));

      Assert.AreEqual(TestCaseName, mViewModel.TestCaseName);
      Assert.AreEqual(ExpectedPath, mViewModel.ExpectedPath);
    }

    [Test]
    public void TestAddTestCase_FormValid_TestCaseExists_TestCaseNotAdded_FormNotCleared()
    {
      mViewModel.Project.TestSuite.Add(mTestCase);

      mViewModel.TestCaseName = TestCaseName;
      mViewModel.ExpectedPath = ExpectedPath;
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
      mViewModel.AddCmd.Execute(null);
      mDlgSrvc.Received().Error(string.Format("Cannot add test case {0} because it already exists", TestCaseName), TestSuiteViewModel.AddTestCaseCaption);
      Assert.AreEqual(1, mViewModel.Project.TestSuite.Count());

      Assert.AreEqual(TestCaseName, mViewModel.TestCaseName);
      Assert.AreEqual(ExpectedPath, mViewModel.ExpectedPath);
    }

    [Test]
    public void TestAddTestCase_FormValid_InputModelPathInvalid_TesCaseNotAdded_FormNotCleared()
    {
      const string invalidPath = "X:/../../../invalid.xmi";
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = invalidPath
});

      mViewModel.TestCaseName = TestCaseName;
      mViewModel.ExpectedPath = ExpectedPath;

      mViewModel.AddCmd.Execute(null);
      mDlgSrvc.Received().Error(string.Format("Input model {0} does not exist", invalidPath), TestSuiteViewModel.AddTestCaseCaption);
      Assert.IsNull(mViewModel.Project.TestSuite.SingleOrDefault(t => t.Name == TestCaseName));

      Assert.AreEqual(TestCaseName, mViewModel.TestCaseName);
      Assert.AreEqual(ExpectedPath, mViewModel.ExpectedPath);
    }

    #endregion

    #region Delete Test Case

    [Test]
    public void TestDeleteTestCase_TestCaseSelected_TestCaseRemoved()
    {
      var testCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath
        }
      };
      testCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
      mViewModel.Project.TestSuite.Add(testCase);
      mViewModel.SelectedTestCase = testCase;

      mViewModel.DelCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.IsFalse(mViewModel.Project.TestSuite.Contains(testCase));
    }

    [Test]
    public void TestDeleteTestCase_TestCaseNotSelected_TestCaseNotRemoved()
    {
      var testCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath
        }
      };
      testCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
      mViewModel.Project.TestSuite.Add(testCase);

      mViewModel.DelCmd.Execute(null);
      mDlgSrvc.Received().Error(TestSuiteViewModel.SelectTestCaseMsg, TestSuiteViewModel.DelTestCaseCaption);
      Assert.IsTrue(mViewModel.Project.TestSuite.Contains(testCase));
    }

    #endregion

    #region Update Test Case

    [Test]
    public void TestUpdateTestCase_TestCaseSelected_FormValid_TestCaseUpdated()
    {
      var testCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath,
        }
      };
      testCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
      mViewModel.Project.TestSuite.Add(testCase);
      mViewModel.SelectedTestCase = testCase;

      mViewModel.TestCaseName = updatedTestCaseName;
      mViewModel.ExpectedPath = updatedOutputPath;
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = updatedInputPath
      });

      mViewModel.UpdateCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.AreEqual(updatedTestCaseName, mViewModel.Project.TestSuite[0].Name);
      Assert.AreEqual(updatedOutputPath, mViewModel.Project.TestSuite[0].OutModel.Path);
      Assert.AreEqual(InputModelName, mViewModel.Project.TestSuite[0].InModels[0].Name);
      Assert.AreEqual(updatedInputPath, mViewModel.Project.TestSuite[0].InModels[0].Path);
    }

    [Test]
    public void TestUpdateTestCase_TestCaseNotSelected_TestCaseNotUpdated()
    {
      var testCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath,
        }
      };
      testCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
      mViewModel.Project.TestSuite.Add(testCase);

      mViewModel.TestCaseName = updatedTestCaseName;
      mViewModel.ExpectedPath = updatedOutputPath;
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = updatedInputPath
      });

      mViewModel.UpdateCmd.Execute(null);
      mDlgSrvc.Received().Error(TestSuiteViewModel.SelectTestCaseMsg, TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(TestSuiteViewModel.UpdateErrorMsg, TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Test case {0} already exists", TestCaseName), TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Input model {0} does not exist", InputPath), TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Out model {0} does not exist", ExpectedPath), TestSuiteViewModel.UpdateTestCaseCaption);
      Assert.AreEqual(TestCaseName, mViewModel.Project.TestSuite[0].Name);
      Assert.AreEqual(ExpectedPath, mViewModel.Project.TestSuite[0].OutModel.Path);
      Assert.AreEqual(InputModelName, mViewModel.Project.TestSuite[0].InModels[0].Name);
      Assert.AreEqual(InputPath, mViewModel.Project.TestSuite[0].InModels[0].Path);
    }

    [Test]
    public void TestUpdateTestCase_TestCaseSelected_FormInvalid_TestCaseNotUpdated()
    {
      var testCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath,
        }
      };
      testCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
      mViewModel.Project.TestSuite.Add(testCase);
      mViewModel.SelectedTestCase = testCase;

      mViewModel.TestCaseName = updatedTestCaseName;
      mViewModel.ExpectedPath = string.Empty;
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = updatedInputPath
      });

      mViewModel.UpdateCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<string>());
      Assert.AreEqual(TestCaseName, mViewModel.Project.TestSuite[0].Name);
      Assert.AreEqual(ExpectedPath, mViewModel.Project.TestSuite[0].OutModel.Path);
      Assert.AreEqual(InputModelName, mViewModel.Project.TestSuite[0].InModels[0].Name);
      Assert.AreEqual(InputPath, mViewModel.Project.TestSuite[0].InModels[0].Path);
    }

    [Test]
    public void TestUpdateTestCase_TestCaseSelected_InputModelNotSelected_TestCaseNotUpdated()
    {
      var testCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath,
        }
      };
      testCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
      mViewModel.Project.TestSuite.Add(testCase);
      mViewModel.SelectedTestCase = testCase;

      mViewModel.TestCaseName = updatedTestCaseName;
      mViewModel.ExpectedPath = updatedOutputPath;
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = string.Empty
      });

      mViewModel.UpdateCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(TestSuiteViewModel.SelectTestCaseMsg, TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.Received().Error(TestSuiteViewModel.UpdateErrorMsg, TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Test case {0} already exists", TestCaseName), TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Input model {0} does not exist", InputPath), TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Out model {0} does not exist", ExpectedPath), TestSuiteViewModel.UpdateTestCaseCaption);
      Assert.AreEqual(TestCaseName, mViewModel.Project.TestSuite[0].Name);
      Assert.AreEqual(ExpectedPath, mViewModel.Project.TestSuite[0].OutModel.Path);
      Assert.AreEqual(InputModelName, mViewModel.Project.TestSuite[0].InModels[0].Name);
      Assert.AreEqual(InputPath, mViewModel.Project.TestSuite[0].InModels[0].Path);
    }

    [Test]
    public void TestUpdateTestCase_TestCaseSelected_InputModelFileDoesNotExist_TestCaseNotUpdated()
    {
      var testCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath,
        }
      };
      testCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
      mViewModel.Project.TestSuite.Add(testCase);
      mViewModel.SelectedTestCase = testCase;

      mViewModel.TestCaseName = updatedTestCaseName;
      mViewModel.ExpectedPath = updatedOutputPath;
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InvalidModelPath
      });

      mViewModel.UpdateCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(TestSuiteViewModel.SelectTestCaseMsg, TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(TestSuiteViewModel.UpdateErrorMsg, TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Test case {0} already exists", TestCaseName), TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.Received().Error(string.Format("Input model {0} does not exist", InvalidModelPath), TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Out model {0} does not exist", ExpectedPath), TestSuiteViewModel.UpdateTestCaseCaption);
      Assert.AreEqual(TestCaseName, mViewModel.Project.TestSuite[0].Name);
      Assert.AreEqual(ExpectedPath, mViewModel.Project.TestSuite[0].OutModel.Path);
      Assert.AreEqual(InputModelName, mViewModel.Project.TestSuite[0].InModels[0].Name);
      Assert.AreEqual(InputPath, mViewModel.Project.TestSuite[0].InModels[0].Path);
    }

    [Test]
    public void TestUpdateTestCase_TestCaseSelected_OutputModelFileDoesNotExist_TestCaseNotUpdated()
    {
      var testCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath,
        }
      };
      testCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
      mViewModel.Project.TestSuite.Add(testCase);
      mViewModel.SelectedTestCase = testCase;

      mViewModel.TestCaseName = updatedTestCaseName;
      mViewModel.ExpectedPath = InvalidModelPath;
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = updatedInputPath
      });

      mViewModel.UpdateCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(TestSuiteViewModel.SelectTestCaseMsg, TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(TestSuiteViewModel.UpdateErrorMsg, TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Test case {0} already exists", TestCaseName), TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Input model {0} does not exist", InputPath), TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.Received().Error(string.Format("Out model {0} does not exist", InvalidModelPath), TestSuiteViewModel.UpdateTestCaseCaption);
      Assert.AreEqual(TestCaseName, mViewModel.Project.TestSuite[0].Name);
      Assert.AreEqual(ExpectedPath, mViewModel.Project.TestSuite[0].OutModel.Path);
      Assert.AreEqual(InputModelName, mViewModel.Project.TestSuite[0].InModels[0].Name);
      Assert.AreEqual(InputPath, mViewModel.Project.TestSuite[0].InModels[0].Path);
    }

    [Test]
    public void TestUpdateTesCase_TestCaseSelected_TestCaseExists_TestCaseNotUpdated()
    {
      var testCase1 = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath,
        }
      };
      testCase1.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });

      var testCase2 = new MuAtlTestCase
      {
        Name = "TestCase2",
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath,
        }
      };
      testCase2.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });

      mViewModel.Project.TestSuite.Add(testCase1);
      mViewModel.Project.TestSuite.Add(testCase2);
      mViewModel.SelectedTestCase = testCase2;

      mViewModel.TestCaseName = TestCaseName;
      mViewModel.ExpectedPath = ExpectedPath;
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });

      mViewModel.UpdateCmd.Execute(null);
      mDlgSrvc.DidNotReceive().Error(TestSuiteViewModel.SelectTestCaseMsg, TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(TestSuiteViewModel.UpdateErrorMsg, TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.Received().Error(string.Format("Test case {0} already exists", TestCaseName), TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Input model {0} does not exist", InputPath), TestSuiteViewModel.UpdateTestCaseCaption);
      mDlgSrvc.DidNotReceive().Error(string.Format("Out model {0} does not exist", ExpectedPath), TestSuiteViewModel.UpdateTestCaseCaption);
      Assert.AreEqual(2, mViewModel.Project.TestSuite.Count());
      Assert.AreEqual(testCase2.Name, mViewModel.Project.TestSuite[1].Name);
      Assert.AreEqual(ExpectedPath, mViewModel.Project.TestSuite[1].OutModel.Path);
      Assert.AreEqual(InputModelName, mViewModel.Project.TestSuite[1].InModels[0].Name);
      Assert.AreEqual(InputPath, mViewModel.Project.TestSuite[1].InModels[0].Path);
    }

    #endregion

    #region Find Test Case

    [Test]
    public void TestFindTestCase_TestCaseExists_TestCaseDetailsShownOnForm()
    {
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName
      });
      mProject.AddTestCase(mTestCase);

      mViewModel.FindCmd.Execute(mTestCase.Name);
      Assert.AreEqual(mTestCase.Name, mViewModel.TestCaseName);
      Assert.AreEqual(mTestCase.OutModel.Path, mViewModel.ExpectedPath);
      Assert.AreEqual(mTestCase.InModels[0].Path, mViewModel.Input[0].Path);
    }

    [Test]
    public void TestFindTestCase_TestCaseDoesNotExists_TestCaseDetailsNotShownOnForm()
    {
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName
      });

      mViewModel.FindCmd.Execute(mTestCase.Name);
      Assert.IsNull(mViewModel.TestCaseName);
      Assert.IsNull(mViewModel.ExpectedPath);
      Assert.IsNull(mViewModel.Input[0].Path);
    }

    [Test]
    public void TestFindTestCase_TestCaseNameNotGiven_TestCaseDetailsNotShownOnForm()
    {
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName
      });

      mViewModel.FindCmd.Execute(mTestCase.Name);
      Assert.IsNull(mViewModel.TestCaseName);
      Assert.IsNull(mViewModel.ExpectedPath);
      Assert.IsNull(mViewModel.Input[0].Path);
    } 

    #endregion

    #region Select Test Case

    [Test]
    public void TestOnTableSelect_TestCaseSelected_TestCaseDetailsShownOnForm()
    {
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName
      });
      var testCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath,
        }
      };
      testCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
      mViewModel.Project.TestSuite.Add(testCase);
      mViewModel.SelectedTestCase = testCase;

      mViewModel.SelectCmd.Execute(null);
      Assert.AreEqual(TestCaseName, mViewModel.TestCaseName);
      Assert.AreEqual(ExpectedPath, mViewModel.ExpectedPath);
      Assert.AreEqual(InputPath, mViewModel.Input[0].Path);
    }

    [Test]
    public void TestOnTableSelect_TestCaseNotSelected_TestCaseDetailsNotShownOnForm()
    {
      mViewModel.Input.Add(new TestCaseInput
      {
        Name = InputModelName
      });
      var testCase = new MuAtlTestCase
      {
        Name = TestCaseName,
        OutModel = new TestCaseOutput
        {
          Name = OutputName,
          Path = ExpectedPath,
        }
      };
      testCase.InModels.Add(new TestCaseInput
      {
        Name = InputModelName,
        Path = InputPath
      });
      mViewModel.Project.TestSuite.Add(testCase);

      mViewModel.SelectCmd.Execute(null);
      Assert.AreEqual(null, mViewModel.TestCaseName);
      Assert.AreEqual(null, mViewModel.ExpectedPath);
      Assert.AreEqual(null, mViewModel.Input[0].Path);
    }

    #endregion

    #region Select Input Model

    [Test]
    public void TestSelectInputModel_InputModelPathSet()
    {
      var input = new TestCaseInput
      {
        Name = InputModelName
      };
      mDlgSrvc.BrowseModel(TestSuiteViewModel.InputModelMsg).Returns(InputPath);

      mViewModel.SelectInputCmd.Execute(input);
      mDlgSrvc.Received().BrowseModel(TestSuiteViewModel.InputModelMsg);
      Assert.AreEqual(InputPath, input.Path);
    }

    [Test]
    public void TestSelectInputModel_SelectCancelled_InputModelPathNotSet()
    {
      var input = new TestCaseInput
      {
        Name = InputModelName
      };
      mDlgSrvc.BrowseModel(TestSuiteViewModel.InputModelMsg).Returns(string.Empty);

      mViewModel.SelectInputCmd.Execute(input);
      mDlgSrvc.Received().BrowseModel(TestSuiteViewModel.InputModelMsg);
      Assert.IsNull(input.Path);
    }

    #endregion

    #region Select Output Model

    [Test]
    public void TestSelectOutputModel_ExpectedOutputModelSelected()
    {
      var input = new TestCaseOutput
      {
        Name = OutputName
      };
      mDlgSrvc.BrowseModel(TestSuiteViewModel.OutputModelMsg).Returns(ExpectedPath);

      mViewModel.SelectOutputCmd.Execute(null);
      mDlgSrvc.Received().BrowseModel(TestSuiteViewModel.OutputModelMsg);
      Assert.AreEqual(ExpectedPath, mViewModel.ExpectedPath);
    }

    [Test]
    public void TestSelectOutputModel_SelectCancelled_ExpectedOutputNotModelSelected()
    {
      var input = new TestCaseOutput
      {
        Name = OutputName
      };
      mDlgSrvc.BrowseModel(TestSuiteViewModel.OutputModelMsg).Returns(string.Empty);

      mViewModel.SelectOutputCmd.Execute(null);
      mDlgSrvc.Received().BrowseModel(TestSuiteViewModel.OutputModelMsg);
      Assert.IsNull(mViewModel.ExpectedPath);
    } 

    #endregion
  }
}
