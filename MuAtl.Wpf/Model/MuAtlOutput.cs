using System.ComponentModel;
namespace MuAtl.Model
{
  public class MuAtlOutput : ModelBase
  {
    private string mExpectedPath;
    public string ExpectedPath 
    {
      get
      {
        return mExpectedPath;
      }
      set
      {
        mExpectedPath = value;
        OnPropertyChanged("ExpectedPath");
      }
    }
  }
}
