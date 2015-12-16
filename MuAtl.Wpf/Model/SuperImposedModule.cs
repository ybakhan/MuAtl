namespace MuAtl.Model
{
  public class SuperImposedModule : MuAtlModelBase
  {
    public override bool Equals(object obj)
    {
      var other = obj as SuperImposedModule;
      if (other == null)
        return false;

      return Name.Equals(other.Name);
    }
  }
}
