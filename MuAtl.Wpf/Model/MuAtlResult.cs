namespace MuAtl.Model
{
  public class MuAtlResult : ModelBase
  {
    private MuAtlMutant mMutant;
    public MuAtlMutant Mutant
    {
      get
      {
        return mMutant;
      }
      set
      {
        mMutant = value;
        OnPropertyChanged("Mutant");
      }
    }

    private MuAtlTestCase mTestCase;
    public MuAtlTestCase TestCase
    {
      get
      {
        return mTestCase;
      }
      set
      {
        mTestCase = value;
        OnPropertyChanged("TestCase");
      }
    }

    private MuAtlTestCaseOutput mOutput;
    public MuAtlTestCaseOutput Output
    {
      get
      {
        return mOutput;
      }
      set
      {
        mOutput = value;
        OnPropertyChanged("Output");
      }
    }

    private string mComment;
    public string Comment
    {
      get
      {
        return mComment;
      }
      set
      {
        mComment = value;
        OnPropertyChanged("Comment");
      }
    }

    private MuAtlVerdict? mVerdict;
    public MuAtlVerdict? Verdict
    {
      get
      {
        return mVerdict;
      }
      set
      {
        mVerdict = value;
        OnPropertyChanged("Verdict");
      }
    }

    private string mLog;
    public string Log
    {
      get
      {
        return mLog;
      }
      set
      {
        mLog = value;
        OnPropertyChanged("Log");
      }
    }

    public override bool Equals(object obj)
    {
      var other = obj as MuAtlResult;
      if (other == null)
        return false;

      return Mutant.Equals(other.Mutant) && TestCase.Equals(other.TestCase);
    }
  }
}
