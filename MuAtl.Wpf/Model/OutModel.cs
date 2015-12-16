namespace MuAtl.Model
{
  public class OutModel : MuAtlModelBase
  {
    private Metamodel mMetamodel;
    public Metamodel Metamodel
    {
      get
      {
        return mMetamodel;
      }
      set
      {
        mMetamodel = value;
        OnPropertyChanged("Metamodel");
      }
    }

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

    public OutModel()
    {
    }

    public OutModel(OutModel model)
    {
      Name = model.Name;
      Path = model.Path;
      Metamodel = new Metamodel(model.Metamodel);
      ExpectedPath = model.ExpectedPath;
    }
  }
}
