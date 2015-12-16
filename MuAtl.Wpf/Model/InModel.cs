using System.Text;

namespace MuAtl.Model
{
  public class InModel : MuAtlModelBase
  {
    private Metamodel mMetamodel;
    public Metamodel Metamodels
    {
      get
      {
        return mMetamodel;
      }
      set
      {
        mMetamodel = value;
        OnPropertyChanged("Metamodels");
      }
    }

    private string mMetamodelInfo;
    public string MetamodelInfo
    {
      get
      {
        var sb = new StringBuilder();
        sb.Append(Metamodels.Name);
        sb.Append(";");
        mMetamodelInfo = sb.ToString();
        return mMetamodelInfo;
      }
      set
      {
        mMetamodelInfo = value;
        OnPropertyChanged("MetamodelInfo");
      }
    }

    public override bool Equals(object obj)
    {
      var other = obj as InModel;
      if (other == null)
        return false;

      return Name.Equals(other.Name);
    }
  }
}


