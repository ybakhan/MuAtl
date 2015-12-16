namespace MuAtl.Model
{
  public class AtlLibrary : MuAtlModelBase
  {
    public override bool Equals(object obj)
    {
      var other = obj as AtlLibrary;
      if (other == null)
        return false;

      return Name.Equals(other.Name);
    }
  }
}
