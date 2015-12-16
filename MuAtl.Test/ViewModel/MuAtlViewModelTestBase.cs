using MuAtl.Model;
using MuAtl.Service;
using NUnit.Framework;
using NSubstitute;

namespace MuAtl.Test.ViewModel
{
  public abstract class MuAtlViewModelTestBase
  {
    protected MuAtlProject mProject;
    protected IDialogService mDlgSrvc;

    [SetUp]
    public virtual void Init()
    {
      mProject = new MuAtlProject
      {
        Name = "Test Project"
      };
      mDlgSrvc = Substitute.For<IDialogService>();
    }
  }
}
